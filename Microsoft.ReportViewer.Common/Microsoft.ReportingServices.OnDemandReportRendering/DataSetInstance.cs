using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataSetInstance
	{
		private readonly DataSet m_dataSetDef;

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataSetInstance m_dataSetInstance;

		private RowInstance m_row;

		private IRecordRowReader m_dataReader;

		private IDataComparer m_comparer;

		internal IDataComparer Comparer
		{
			get
			{
				if (m_dataSetInstance == null)
				{
					return null;
				}
				if (m_comparer == null)
				{
					m_comparer = m_dataSetInstance.CreateProcessingComparer(m_dataSetDef.RenderingContext.OdpContext);
				}
				return m_comparer;
			}
		}

		public RowInstance Row
		{
			get
			{
				if (m_dataReader == null)
				{
					return null;
				}
				if (m_row == null)
				{
					m_row = new RowInstance(m_dataSetInstance.FieldInfos, m_dataReader.RecordRow);
				}
				return m_row;
			}
		}

		internal DataSetInstance(DataSet dataSetDef)
		{
			m_dataSetDef = dataSetDef;
		}

		public void ResetContext()
		{
			if (m_dataReader == null)
			{
				CreateDataReader();
			}
			else if (!m_dataReader.MoveToFirstRow())
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "OnDemandReportRendering.DataSetInstance triggered a second query execution or second chunk open for dataset: {0} in report {1}", m_dataSetDef.Name.MarkAsPrivate(), m_dataSetDef.RenderingContext.OdpContext.ReportContext.ItemPathAsString.MarkAsPrivate());
				Close();
				CreateDataReader();
			}
			m_row = null;
		}

		public bool MoveNext()
		{
			if (m_dataReader == null)
			{
				return false;
			}
			if (m_dataReader.GetNextRow())
			{
				if (m_row != null)
				{
					m_row.UpdateRecordRow(m_dataReader.RecordRow);
				}
				return true;
			}
			return false;
		}

		public void Close()
		{
			if (m_dataReader != null)
			{
				m_dataReader.Close();
				m_dataReader = null;
			}
			m_row = null;
		}

		private void CreateDataReader()
		{
			OnDemandProcessingContext odpContext = m_dataSetDef.RenderingContext.OdpContext;
			m_dataReader = odpContext.CreateSequentialDataReader(m_dataSetDef.DataSetDef, out m_dataSetInstance);
		}

		internal void SetNewContext()
		{
			Close();
		}
	}
}
