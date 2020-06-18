using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class NavigationItem
	{
		private readonly Microsoft.ReportingServices.ReportIntermediateFormat.NavigationItem m_navigationItem;

		public string ReportItemReference => m_navigationItem.ReportItemReference;

		public ReportItem ReportItem => null;

		internal NavigationItem(Microsoft.ReportingServices.ReportIntermediateFormat.NavigationItem navigationItem)
		{
			m_navigationItem = navigationItem;
		}
	}
}
