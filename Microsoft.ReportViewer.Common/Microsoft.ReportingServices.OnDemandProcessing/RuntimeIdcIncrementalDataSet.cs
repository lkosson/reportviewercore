using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeIdcIncrementalDataSet : RuntimeIncrementalDataSet
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow m_currentRow;

		internal RuntimeIdcIncrementalDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			: base(dataSource, dataSet, dataSetInstance, odpContext)
		{
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow GetNextRow()
		{
			SetupNextRow();
			return m_currentRow;
		}

		private bool SetupNextRow()
		{
			try
			{
				m_currentRow = ReadOneRow(out int rowIndex);
				if (m_currentRow == null)
				{
					return false;
				}
				FieldsImpl fieldsImpl = m_odpContext.ReportObjectModel.FieldsImpl;
				fieldsImpl.NewRow();
				if (fieldsImpl.AddRowIndex)
				{
					fieldsImpl.SetRowIndex(rowIndex);
				}
				m_odpContext.ReportObjectModel.UpdateFieldValues(reuseFieldObjects: false, m_currentRow, m_dataSetInstance, base.HasServerAggregateMetadata);
				return true;
			}
			catch (Exception)
			{
				CleanupForException();
				FinalCleanup();
				throw;
			}
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
			Global.Tracer.Assert(m_odpContext.ReportObjectModel != null && m_odpContext.ReportRuntime != null);
			m_odpContext.SetupFieldsForNewDataSet(m_dataSet, m_dataSetInstance, addRowIndex: true, noRows: true);
			PopulateFieldsWithReaderFlags();
			m_dataSet.SetFilterExprHost(m_odpContext.ReportObjectModel);
		}

		protected override void ProcessExtendedPropertyMappings()
		{
		}
	}
}
