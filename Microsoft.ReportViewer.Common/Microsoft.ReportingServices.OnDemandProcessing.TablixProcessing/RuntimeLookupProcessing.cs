using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class RuntimeLookupProcessing
	{
		private RuntimeOnDemandDataSetObj m_lookupOwner;

		private OnDemandProcessingContext m_odpContext;

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		private DataSetInstance m_dataSetInstance;

		private bool m_mustBufferAllRows;

		private int m_firstRowCacheIndex = -1;

		internal bool MustBufferAllRows => m_mustBufferAllRows;

		internal RuntimeLookupProcessing(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, RuntimeOnDemandDataSetObj lookupOwner)
		{
			m_odpContext = odpContext;
			m_dataSet = dataSet;
			m_dataSetInstance = dataSetInstance;
			m_lookupOwner = lookupOwner;
			m_mustBufferAllRows = dataSet.HasSameDataSetLookups;
			InitializeRuntimeStructures();
		}

		private void InitializeRuntimeStructures()
		{
			Global.Tracer.Assert(m_dataSet.LookupDestinationInfos != null && m_dataSet.LookupDestinationInfos.Count > 0, "Attempted to perform Lookup processing on a DataSet with no Lookups");
			IScalabilityCache tablixProcessingScalabilityCache = m_odpContext.TablixProcessingScalabilityCache;
			Global.Tracer.Assert(tablixProcessingScalabilityCache != null, "Cannot start Lookup processing unless Scalability is setup");
			if (m_mustBufferAllRows)
			{
				m_odpContext.TablixProcessingLookupRowCache = new CommonRowCache(tablixProcessingScalabilityCache);
			}
			int count = m_dataSet.LookupDestinationInfos.Count;
			List<LookupObjResult> list = new List<LookupObjResult>(count);
			m_dataSetInstance.LookupResults = list;
			for (int i = 0; i < count; i++)
			{
				LookupDestinationInfo lookupDestinationInfo = m_dataSet.LookupDestinationInfos[i];
				LookupObjResult item = new LookupObjResult(new LookupTable(tablixProcessingScalabilityCache, m_odpContext.ProcessingComparer, lookupDestinationInfo.UsedInSameDataSetTablixProcessing));
				list.Add(item);
			}
		}

		internal void NextRow()
		{
			long streamOffset = m_odpContext.ReportObjectModel.FieldsImpl.StreamOffset;
			int num = -1;
			CommonRowCache tablixProcessingLookupRowCache = m_odpContext.TablixProcessingLookupRowCache;
			if (m_mustBufferAllRows)
			{
				num = tablixProcessingLookupRowCache.AddRow(RuntimeDataTablixObj.SaveData(m_odpContext));
				if (m_firstRowCacheIndex == -1)
				{
					m_firstRowCacheIndex = num;
				}
			}
			IScalabilityCache tablixProcessingScalabilityCache = m_odpContext.TablixProcessingScalabilityCache;
			for (int i = 0; i < m_dataSet.LookupDestinationInfos.Count; i++)
			{
				LookupDestinationInfo lookupDestinationInfo = m_dataSet.LookupDestinationInfos[i];
				LookupObjResult lookupObjResult = m_dataSetInstance.LookupResults[i];
				if (lookupObjResult.ErrorOccured)
				{
					continue;
				}
				Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = lookupDestinationInfo.EvaluateDestExpr(m_odpContext, lookupObjResult);
				if (variantResult.ErrorOccurred)
				{
					lookupObjResult.DataFieldStatus = variantResult.FieldStatus;
					continue;
				}
				object value = variantResult.Value;
				LookupTable lookupTable = lookupObjResult.GetLookupTable(m_odpContext);
				try
				{
					if (!lookupTable.TryGetAndPinValue(value, out LookupMatches matches, out IDisposable cleanupRef))
					{
						matches = ((!lookupDestinationInfo.UsedInSameDataSetTablixProcessing) ? new LookupMatches() : new LookupMatchesWithRows());
						cleanupRef = lookupTable.AddAndPin(value, matches);
					}
					if (lookupDestinationInfo.IsMultiValue || !matches.HasRow)
					{
						matches.AddRow(streamOffset, num, tablixProcessingScalabilityCache);
					}
					cleanupRef.Dispose();
				}
				catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError)
				{
					throw new ReportProcessingException(m_lookupOwner.RegisterSpatialElementComparisonError(reportProcessingException_SpatialTypeComparisonError.Type));
				}
			}
			if (!m_mustBufferAllRows)
			{
				m_lookupOwner.PostLookupNextRow();
			}
		}

		internal void FinishReadingRows()
		{
			if (!m_mustBufferAllRows)
			{
				return;
			}
			CommonRowCache tablixProcessingLookupRowCache = m_odpContext.TablixProcessingLookupRowCache;
			if (m_firstRowCacheIndex != -1)
			{
				for (int i = m_firstRowCacheIndex; i < tablixProcessingLookupRowCache.Count; i++)
				{
					tablixProcessingLookupRowCache.SetupRow(i, m_odpContext);
					m_lookupOwner.PostLookupNextRow();
				}
			}
		}

		internal void CompleteLookupProcessing()
		{
			for (int i = 0; i < m_dataSetInstance.LookupResults.Count; i++)
			{
				m_dataSetInstance.LookupResults[i].TransferToLookupCache(m_odpContext);
			}
			if (m_odpContext.TablixProcessingLookupRowCache != null)
			{
				m_odpContext.TablixProcessingLookupRowCache.Dispose();
				m_odpContext.TablixProcessingLookupRowCache = null;
			}
		}
	}
}
