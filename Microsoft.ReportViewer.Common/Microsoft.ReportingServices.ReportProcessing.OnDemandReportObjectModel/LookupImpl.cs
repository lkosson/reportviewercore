using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class LookupImpl : Lookup
	{
		private LookupInfo m_lookupInfo;

		private Microsoft.ReportingServices.RdlExpressions.ReportRuntime m_reportRuntime;

		private static readonly object[] EmptyResult = new object[0];

		public override object Value
		{
			get
			{
				object result = null;
				object[] array = EvaluateLookup();
				if (array != null && array.Length != 0)
				{
					result = array[0];
				}
				return result;
			}
		}

		public override object[] Values => EvaluateLookup();

		internal string Name => m_lookupInfo.Name;

		internal LookupImpl(LookupInfo lookupInfo, Microsoft.ReportingServices.RdlExpressions.ReportRuntime reportRuntime)
		{
			m_lookupInfo = lookupInfo;
			m_reportRuntime = reportRuntime;
		}

		internal object[] EvaluateLookup()
		{
			bool flag = m_lookupInfo.ReturnFirstMatchOnly();
			OnDemandProcessingContext odpContext = m_reportRuntime.ReportObjectModel.OdpContext;
			Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet = odpContext.ReportDefinition.MappingDataSetIndexToDataSet[m_lookupInfo.DataSetIndexInCollection];
			DataSetInstance dataSetInstance = odpContext.GetDataSetInstance(dataSet);
			if (dataSetInstance == null)
			{
				throw new ReportProcessingException_InvalidOperationException();
			}
			if (dataSetInstance.NoRows)
			{
				return EmptyResult;
			}
			if (dataSetInstance.LookupResults == null || dataSetInstance.LookupResults[m_lookupInfo.DestinationIndexInCollection] == null)
			{
				if (!odpContext.CalculateLookup(m_lookupInfo))
				{
					return EmptyResult;
				}
				Global.Tracer.Assert(dataSetInstance.LookupResults != null, "Lookup not initialized correctly by tablix processing");
			}
			LookupObjResult lookupObjResult = dataSetInstance.LookupResults[m_lookupInfo.DestinationIndexInCollection];
			if (lookupObjResult.ErrorOccured)
			{
				IErrorContext reportRuntime = m_reportRuntime;
				if (lookupObjResult.DataFieldStatus == DataFieldStatus.None && lookupObjResult.ErrorCode != 0)
				{
					reportRuntime.Register(lookupObjResult.ErrorCode, lookupObjResult.ErrorSeverity, lookupObjResult.ErrorMessageArgs);
				}
				else if (lookupObjResult.DataFieldStatus == DataFieldStatus.UnSupportedDataType)
				{
					reportRuntime.Register(ProcessingErrorCode.rsLookupOfInvalidExpressionDataType, Severity.Warning, lookupObjResult.ErrorMessageArgs);
				}
				throw new ReportProcessingException_InvalidOperationException();
			}
			Microsoft.ReportingServices.RdlExpressions.VariantResult result = m_lookupInfo.EvaluateSourceExpr(m_reportRuntime);
			CheckExprResultError(result);
			bool flag2 = lookupObjResult.HasBeenTransferred || odpContext.CurrentDataSetIndex != dataSet.IndexInCollection;
			List<object> list = null;
			CompareInfo compareInfo = null;
			CompareOptions clrCompareOptions = CompareOptions.None;
			bool nullsAsBlanks = false;
			bool useOrdinalStringKeyGeneration = false;
			try
			{
				if (flag2)
				{
					compareInfo = odpContext.CompareInfo;
					clrCompareOptions = odpContext.ClrCompareOptions;
					nullsAsBlanks = odpContext.NullsAsBlanks;
					useOrdinalStringKeyGeneration = odpContext.UseOrdinalStringKeyGeneration;
					dataSetInstance.SetupCollationSettings(odpContext);
				}
				LookupTable lookupTable = lookupObjResult.GetLookupTable(odpContext);
				Global.Tracer.Assert(lookupTable != null, "LookupTable must not be null");
				ObjectModelImpl reportObjectModel = odpContext.ReportObjectModel;
				Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader = null;
				if (flag2)
				{
					dataChunkReader = odpContext.GetDataChunkReader(dataSet.IndexInCollection);
				}
				using (reportObjectModel.SetupNewFieldsWithBackup(dataSet, dataSetInstance, dataChunkReader))
				{
					object[] array = result.Value as object[];
					if (array == null)
					{
						array = new object[1]
						{
							result.Value
						};
					}
					else
					{
						list = new List<object>(array.Length);
					}
					object[] array2 = array;
					foreach (object key in array2)
					{
						if (lookupTable.TryGetValue(key, out LookupMatches matches))
						{
							int num = flag ? 1 : matches.MatchCount;
							if (list == null)
							{
								list = new List<object>(num);
							}
							for (int j = 0; j < num; j++)
							{
								matches.SetupRow(j, odpContext);
								Microsoft.ReportingServices.RdlExpressions.VariantResult result2 = m_lookupInfo.EvaluateResultExpr(m_reportRuntime);
								CheckExprResultError(result2);
								list.Add(result2.Value);
							}
						}
					}
				}
			}
			finally
			{
				if (compareInfo != null)
				{
					odpContext.SetComparisonInformation(compareInfo, clrCompareOptions, nullsAsBlanks, useOrdinalStringKeyGeneration);
				}
			}
			object[] result3 = EmptyResult;
			if (list != null)
			{
				result3 = list.ToArray();
			}
			return result3;
		}

		private void CheckExprResultError(Microsoft.ReportingServices.RdlExpressions.VariantResult result)
		{
			if (result.ErrorOccurred)
			{
				throw new ReportProcessingException_InvalidOperationException();
			}
		}
	}
}
