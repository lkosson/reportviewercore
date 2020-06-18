using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DrillthroughInfo
	{
		private string m_reportName;

		private DrillthroughParameters m_reportParameters;

		internal string ReportName => m_reportName;

		internal DrillthroughParameters ReportParameters => m_reportParameters;

		internal DrillthroughInfo(string reportName, DrillthroughParameters parameters)
		{
			m_reportName = reportName;
			m_reportParameters = parameters;
		}
	}
}
