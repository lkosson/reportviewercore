using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class DataSetImpl : Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.DataSet
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		private DataSetInstance m_dataSetInstance;

		private DateTime m_dataSetExecutionTime;

		public override string CommandText
		{
			get
			{
				string text = null;
				if (m_dataSetInstance != null)
				{
					text = m_dataSetInstance.CommandText;
				}
				if (text == null && m_dataSet.Query != null && m_dataSet.Query.CommandText != null && !m_dataSet.Query.CommandText.IsExpression && m_dataSet.Query.CommandText.Value != null)
				{
					text = m_dataSet.Query.CommandText.Value.ToString();
				}
				return text;
			}
		}

		public override string RewrittenCommandText
		{
			get
			{
				string result = null;
				if (m_dataSetInstance != null)
				{
					result = m_dataSetInstance.RewrittenCommandText;
				}
				return result;
			}
		}

		public override DateTime ExecutionTime => m_dataSetExecutionTime;

		internal DataSetImpl(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSetDef, DataSetInstance dataSetInstance, DateTime reportExecutionTime)
		{
			m_dataSet = dataSetDef;
			Update(dataSetInstance, reportExecutionTime);
		}

		internal void Update(DataSetInstance dataSetInstance, DateTime reportExecutionTime)
		{
			m_dataSetInstance = dataSetInstance;
			if (dataSetInstance != null)
			{
				m_dataSetExecutionTime = dataSetInstance.GetQueryExecutionTime(reportExecutionTime);
			}
			else
			{
				m_dataSetExecutionTime = reportExecutionTime;
			}
		}
	}
}
