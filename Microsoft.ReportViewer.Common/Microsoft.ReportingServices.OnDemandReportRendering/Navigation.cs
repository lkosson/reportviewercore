using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class Navigation
	{
		internal readonly Microsoft.ReportingServices.ReportIntermediateFormat.Navigation m_navigation;

		internal Navigation(Microsoft.ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayout)
		{
			m_navigation = bandLayout.Navigation;
		}
	}
}
