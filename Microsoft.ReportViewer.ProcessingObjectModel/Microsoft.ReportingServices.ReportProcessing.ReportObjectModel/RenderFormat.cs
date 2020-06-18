using System;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class RenderFormat : MarshalByRefObject
	{
		private RenderFormatImplBase m_renderFormatImpl;

		public string Name => m_renderFormatImpl.Name;

		public bool IsInteractive => m_renderFormatImpl.IsInteractive;

		public ReadOnlyNameValueCollection DeviceInfo => m_renderFormatImpl.DeviceInfo;

		internal RenderFormat(RenderFormatImplBase renderFormatImpl)
		{
			m_renderFormatImpl = renderFormatImpl;
		}
	}
}
