using Microsoft.ReportingServices.Interfaces;
using System.Collections.Specialized;
using System.IO;

namespace Microsoft.Reporting.WinForms
{
	internal class AsyncAllStreamsRenderingOperation : AsyncRenderingOperation
	{
		private CreateAndRegisterStream m_streamCallback;

		public AsyncAllStreamsRenderingOperation(Report report, PageCountMode pageCountMode, string format, string deviceInfo, bool allowInternalRenderers, PostRenderArgs postRenderArgs, CreateAndRegisterStream streamCallback)
			: base(report, pageCountMode, format, deviceInfo, allowInternalRenderers, postRenderArgs)
		{
			m_streamCallback = streamCallback;
		}

		protected override void RenderServerReport(ServerReport report)
		{
			NameValueCollection baseServerUrlParameters = GetBaseServerUrlParameters();
			baseServerUrlParameters.Add("rs:PersistStreams", "True");
			bool flag = true;
			while (true)
			{
				using (Stream stream = new MemoryStream())
				{
					report.InternalRender(isAbortable: true, base.Format, base.DeviceInfo, baseServerUrlParameters, stream, out string _, out string _);
					stream.Position = 0L;
					int num = stream.ReadByte();
					if (num == -1)
					{
						return;
					}
					Stream stream2 = PrintStream();
					stream2.WriteByte((byte)num);
					byte[] buffer = new byte[81920];
					int num2 = 0;
					while ((num2 = stream.Read(buffer, 0, 81920)) > 0)
					{
						stream2.Write(buffer, 0, num2);
					}
					if (flag)
					{
						baseServerUrlParameters.Clear();
						baseServerUrlParameters.Add("rs:GetNextStream", "True");
					}
					flag = false;
				}
			}
		}

		private Stream PrintStream()
		{
			return m_streamCallback(null, null, null, null, willSeek: true, StreamOper.CreateAndRegister);
		}

		protected override void RenderLocalReport(LocalReport report)
		{
			report.InternalRender(base.Format, base.AllowInternalRenderers, base.DeviceInfo, base.PageCountMode, m_streamCallback, out m_warnings);
		}
	}
}
