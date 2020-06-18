using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal class RuntimePrefetchDataSet : RuntimeAtomicDataSet
	{
		private ChunkManager.DataChunkWriter m_dataChunkWriter;

		protected readonly bool m_canWriteDataChunk;

		private RecordSetInfo m_recordSetInfo;

		protected override bool WritesDataChunk => true;

		public RuntimePrefetchDataSet(DataSource dataSource, DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext processingContext, bool canWriteDataChunk, bool processRetrievedData)
			: base(dataSource, dataSet, dataSetInstance, processingContext, processRetrievedData)
		{
			m_canWriteDataChunk = canWriteDataChunk;
		}

		protected override void ProcessRow(RecordRow aRow, int rowNumber)
		{
			if (!m_dataSet.IsReferenceToSharedDataSet && m_canWriteDataChunk)
			{
				m_dataChunkWriter.WriteRecordRow(aRow);
			}
		}

		protected override void ProcessExtendedPropertyMappings()
		{
			if (!m_dataSet.IsReferenceToSharedDataSet)
			{
				m_recordSetInfo.PopulateExtendedFieldsProperties(m_dataSetInstance);
			}
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
			if (!m_dataSet.IsReferenceToSharedDataSet)
			{
				if (m_dataReader != null)
				{
					m_recordSetInfo = m_dataReader.RecordSetInfo;
				}
				else
				{
					m_recordSetInfo = new RecordSetInfo(aReaderExtensionsSupported, m_odpContext.IsSharedDataSetExecutionOnly, m_dataSetInstance, m_odpContext.ExecutionTime);
				}
				if (m_canWriteDataChunk)
				{
					m_dataChunkWriter = new ChunkManager.DataChunkWriter(m_recordSetInfo, m_dataSetInstance, m_odpContext);
				}
			}
		}

		protected override void AllRowsRead()
		{
			m_dataSetInstance.RecordSetSize = base.NumRowsRead;
		}

		protected override void CleanupProcess()
		{
			base.CleanupProcess();
			if (m_dataChunkWriter != null)
			{
				m_dataChunkWriter.Close();
				m_dataChunkWriter = null;
			}
		}

		internal override void EraseDataChunk()
		{
			if (!m_dataSet.IsReferenceToSharedDataSet && m_canWriteDataChunk)
			{
				RuntimeDataSet.EraseDataChunk(m_odpContext, m_dataSetInstance, ref m_dataChunkWriter);
			}
		}
	}
}
