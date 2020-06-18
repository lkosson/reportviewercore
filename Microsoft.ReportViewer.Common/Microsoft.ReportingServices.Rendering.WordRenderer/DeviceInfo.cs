using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class DeviceInfo
	{
		private AutoFit m_autoFit = AutoFit.Default;

		private bool m_expandToggles;

		private bool m_fixedPageWidth;

		private bool m_omitHyperlinks;

		private bool m_omitDrillthroughs;

		private NameValueCollection m_rawDeviceInfo;

		internal AutoFit AutoFit
		{
			get
			{
				return m_autoFit;
			}
			set
			{
				m_autoFit = value;
			}
		}

		internal bool ExpandToggles => m_expandToggles;

		internal bool FixedPageWidth => m_fixedPageWidth;

		internal bool OmitHyperlinks => m_omitHyperlinks;

		internal bool OmitDrillthroughs => m_omitDrillthroughs;

		internal NameValueCollection RawDeviceInfo => m_rawDeviceInfo;

		internal DeviceInfo(NameValueCollection deviceInfo)
		{
			m_rawDeviceInfo = deviceInfo;
			string value = deviceInfo["AutoFit"];
			if (!string.IsNullOrEmpty(value))
			{
				try
				{
					AutoFit = (AutoFit)Enum.Parse(AutoFit.GetType(), value, ignoreCase: true);
				}
				catch (Exception)
				{
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "AutoFit value is not valid");
				}
			}
			m_expandToggles = ParseBool(deviceInfo["ExpandToggles"], m_expandToggles);
			m_fixedPageWidth = ParseBool(deviceInfo["FixedPageWidth"], m_fixedPageWidth);
			m_omitHyperlinks = ParseBool(deviceInfo["OmitHyperlinks"], m_omitHyperlinks);
			m_omitDrillthroughs = ParseBool(deviceInfo["OmitDrillthroughs"], m_omitDrillthroughs);
		}

		private static bool ParseBool(string boolValue, bool defaultValue)
		{
			if (!string.IsNullOrEmpty(boolValue))
			{
				bool result = false;
				if (bool.TryParse(boolValue, out result))
				{
					return result;
				}
			}
			return defaultValue;
		}
	}
}
