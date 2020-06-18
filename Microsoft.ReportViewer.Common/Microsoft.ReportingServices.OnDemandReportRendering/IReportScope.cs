using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IReportScope
	{
		IReportScopeInstance ReportScopeInstance
		{
			get;
		}

		IRIFReportScope RIFReportScope
		{
			get;
		}
	}
}
