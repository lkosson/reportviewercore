using Microsoft.ReportingServices.Diagnostics;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class ServerDeviceInfo : DeviceInfo
	{
		public ServerDeviceInfo()
		{
			BrowserMode = BrowserMode.Quirks;
		}

		public override bool IsSupported(string value, bool isTrue, out bool isRelative)
		{
			IPathManager instance = RSPathUtil.Instance;
			return instance.IsSupportedUrl(value, checkProtocol: true, out isRelative);
		}
	}
}
