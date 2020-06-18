using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class PageBreakInfo
	{
		private PageBreakLocation m_breakLocation;

		private bool m_disabled;

		private bool m_resetPageNumber;

		private string m_reportItemName;

		internal PageBreakLocation BreakLocation => m_breakLocation;

		internal bool Disabled => m_disabled;

		internal bool ResetPageNumber => m_resetPageNumber;

		internal string ReportItemName => m_reportItemName;

		internal PageBreakInfo(PageBreak pageBreak, string reportItemName)
		{
			if (pageBreak != null)
			{
				m_breakLocation = pageBreak.BreakLocation;
				m_disabled = pageBreak.Instance.Disabled;
				m_resetPageNumber = pageBreak.Instance.ResetPageNumber;
				if (RenderingDiagnostics.Enabled)
				{
					m_reportItemName = reportItemName;
				}
			}
		}
	}
}
