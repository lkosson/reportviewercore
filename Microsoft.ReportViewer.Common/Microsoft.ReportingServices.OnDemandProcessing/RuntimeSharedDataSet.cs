using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeSharedDataSet : RuntimeParameterDataSet
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkWriter m_dataChunkWriter;

		private Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow m_currentRow;

		private IRowConsumer m_consumerRequest;

		protected override bool WritesDataChunk => m_odpContext.ExternalDataSetContext.MustCreateDataChunk;

		public RuntimeSharedDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSource, Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext processingContext)
			: base(dataSource, dataSet, dataSetInstance, processingContext, dataSet.DataSetCore.Filters != null || dataSet.DataSetCore.HasCalculatedFields(), null)
		{
			m_consumerRequest = m_odpContext.ExternalDataSetContext.ConsumerRequest;
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
			base.InitializeBeforeProcessingRows(aReaderExtensionsSupported);
			if (WritesDataChunk)
			{
				m_dataChunkWriter = new Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkWriter(m_dataReader.RecordSetInfo, m_dataSetInstance, m_odpContext);
			}
		}

		protected override void InitializeBeforeFirstRow(bool hasRows)
		{
			base.InitializeBeforeFirstRow(hasRows);
			if (WritesDataChunk)
			{
				if (hasRows)
				{
					m_dataReader.RecordSetInfo.PopulateExtendedFieldsProperties(m_dataSetInstance);
				}
				m_dataChunkWriter.CreateDataChunkAndWriteHeader(m_dataReader.RecordSetInfo);
			}
			if (m_consumerRequest != null)
			{
				m_consumerRequest.SetProcessingDataReader(m_dataReader);
			}
		}

		protected override void ProcessRow(Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow row, int rowNumber)
		{
			if (m_mustEvaluateThroughReportObjectModel)
			{
				base.ProcessRow(row, rowNumber);
				return;
			}
			m_currentRow = row;
			PostFilterNextRow();
		}

		protected override void AllRowsRead()
		{
			m_dataSetInstance.RecordSetSize = base.NumRowsRead;
			base.AllRowsRead();
		}

		internal override void EraseDataChunk()
		{
			if (WritesDataChunk)
			{
				RuntimeDataSet.EraseDataChunk(m_odpContext, m_dataSetInstance, ref m_dataChunkWriter);
			}
		}

		protected override void FinalCleanup()
		{
			base.FinalCleanup();
			if (m_dataChunkWriter != null)
			{
				m_dataChunkWriter.Close();
				m_dataChunkWriter = null;
			}
		}

		public override void PostFilterNextRow()
		{
			if (m_mustEvaluateThroughReportObjectModel)
			{
				m_currentRow = new Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow(m_odpContext.ReportObjectModel.FieldsImpl, m_dataSet.DataSetCore.Fields.Count, m_dataSetInstance.FieldInfos);
			}
			if (WritesDataChunk)
			{
				m_dataChunkWriter.WriteRecordRow(m_currentRow);
			}
			if (m_consumerRequest != null)
			{
				m_consumerRequest.NextRow(m_currentRow);
			}
		}
	}
}
