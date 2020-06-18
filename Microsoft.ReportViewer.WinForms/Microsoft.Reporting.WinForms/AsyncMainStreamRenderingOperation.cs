using System.IO;

namespace Microsoft.Reporting.WinForms
{
	internal class AsyncMainStreamRenderingOperation : AsyncRenderingOperation
	{
		private byte[] m_mainStreamBytes;

		private string m_mainStreamMimeType;

		private string m_mainStreamFileNameExt;

		public byte[] ReportBytes => m_mainStreamBytes;

		public string FileNameExtension => m_mainStreamFileNameExt;

		public AsyncMainStreamRenderingOperation(Report report, PageCountMode pageCountMode, string format, string deviceInfo, bool allowInternalRenderers, PostRenderArgs postRenderArgs)
			: base(report, pageCountMode, format, deviceInfo, allowInternalRenderers, postRenderArgs)
		{
		}

		protected override void RenderServerReport(ServerReport report)
		{
			using (Stream stream = new MemoryStream())
			{
				report.InternalRender(isAbortable: true, base.Format, base.DeviceInfo, GetBaseServerUrlParameters(), stream, out m_mainStreamMimeType, out m_mainStreamFileNameExt);
				stream.Position = 0L;
				stream.Position = 0L;
				m_mainStreamBytes = new byte[stream.Length];
				stream.Read(m_mainStreamBytes, 0, (int)stream.Length);
			}
		}

		protected override void RenderLocalReport(LocalReport report)
		{
			m_mainStreamBytes = report.InternalRender(base.Format, base.AllowInternalRenderers, base.DeviceInfo, base.PageCountMode, out m_mainStreamMimeType, out string _, out m_mainStreamFileNameExt, out string[] _, out m_warnings);
		}
	}
}
