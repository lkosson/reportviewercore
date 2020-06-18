using System.Collections.Specialized;

namespace Microsoft.Reporting.WinForms
{
	internal abstract class AsyncRenderingOperation : AsyncReportOperation
	{
		private PageCountMode m_pageCountMode;

		private string m_format;

		private string m_deviceInfo;

		private bool m_allowInternalRenderers;

		private PostRenderArgs m_postRenderArgs;

		protected Warning[] m_warnings;

		public PostRenderArgs PostRenderArgs => m_postRenderArgs;

		public Warning[] Warnings => m_warnings;

		protected PageCountMode PageCountMode => m_pageCountMode;

		protected string Format => m_format;

		protected string DeviceInfo => m_deviceInfo;

		protected bool AllowInternalRenderers => m_allowInternalRenderers;

		protected AsyncRenderingOperation(Report report, PageCountMode pageCountMode, string format, string deviceInfo, bool allowInternalRenderers, PostRenderArgs postRenderArgs)
			: base(report)
		{
			m_pageCountMode = pageCountMode;
			m_format = format;
			m_deviceInfo = deviceInfo;
			m_allowInternalRenderers = allowInternalRenderers;
			m_postRenderArgs = postRenderArgs;
		}

		protected abstract void RenderServerReport(ServerReport report);

		protected abstract void RenderLocalReport(LocalReport report);

		protected override void PerformOperation()
		{
			ServerReport serverReport = base.Report as ServerReport;
			if (serverReport != null)
			{
				RenderServerReport(serverReport);
			}
			else
			{
				RenderLocalReport((LocalReport)base.Report);
			}
		}

		protected NameValueCollection GetBaseServerUrlParameters()
		{
			return new NameValueCollection
			{
				{
					"rs:PageCountMode",
					m_pageCountMode.ToString()
				}
			};
		}
	}
}
