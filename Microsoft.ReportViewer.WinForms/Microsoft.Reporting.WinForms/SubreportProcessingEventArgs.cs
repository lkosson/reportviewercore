using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Reporting.WinForms
{
	public sealed class SubreportProcessingEventArgs : EventArgs
	{
		private string m_subReportName;

		private ReportParameterInfoCollection m_paramMetaData;

		private IList<string> m_dsNames;

		private ReportParameter[] m_userParams = new ReportParameter[0];

		private ReportDataSourceCollection m_dataSources = new ReportDataSourceCollection(new object());

		public string ReportPath => m_subReportName;

		public ReportParameterInfoCollection Parameters => m_paramMetaData;

		public IList<string> DataSourceNames => m_dsNames;

		public ReportDataSourceCollection DataSources => m_dataSources;

		internal SubreportProcessingEventArgs(string subreportName, ReportParameterInfoCollection paramMetaData, string[] dataSetNames)
		{
			m_subReportName = subreportName;
			m_paramMetaData = paramMetaData;
			m_dsNames = new ReadOnlyCollection<string>(dataSetNames);
		}
	}
}
