using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class LiveRecordRowReader : IRecordRowReader, IDisposable
	{
		private RuntimeLiveReaderDataSource m_dataSource;

		private RecordRow m_currentRow;

		public RecordRow RecordRow => m_currentRow;

		public DataSetInstance DataSetInstance => m_dataSource.DataSetInstance;

		public LiveRecordRowReader(DataSet dataSet, OnDemandProcessingContext odpContext)
		{
			m_dataSource = new RuntimeLiveReaderDataSource(odpContext.ReportDefinition, dataSet, odpContext);
			m_dataSource.Initialize();
		}

		public bool GetNextRow()
		{
			m_currentRow = m_dataSource.ReadNextRow();
			return m_currentRow != null;
		}

		public bool MoveToFirstRow()
		{
			return false;
		}

		public void Close()
		{
			if (m_dataSource != null)
			{
				m_dataSource.RecordTimeDataRetrieval();
				m_dataSource.Teardown();
				m_dataSource = null;
			}
		}

		public void Dispose()
		{
			Close();
		}
	}
}
