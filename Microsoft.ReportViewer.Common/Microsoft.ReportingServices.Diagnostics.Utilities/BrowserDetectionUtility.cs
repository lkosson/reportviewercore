using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal static class BrowserDetectionUtility
	{
		private static readonly char[] userAgentDelimiter = new char[3]
		{
			';',
			'(',
			')'
		};

		public static string UserAgentKey => "UserAgent";

		public static string TypeKey => "Type";

		public static string ActiveXControlsKey => "ActiveXControls";

		public static string EcmaScriptVersionKey => "EcmaScriptVersion";

		public static string JavaScriptKey => "JavaScript";

		public static string TablesKey => "Tables";

		public static string MajorVersionKey => "MajorVersion";

		public static string MinorVersionKey => "MinorVersion";

		public static string Win32Key => "Win32";

		public static string IEUserAgentPrefix => "IE";

		public static string IEModernUserAgentPrefix => "InternetExplorer";

		public static string IELayoutEngineName => "Trident";

		public static string EdgeUserAgent => "Edge";

		private static string GeckoUserAgent => "GECKO";

		private static string SafariUserAgent => "SAFARI";

		private static string IPadUserAgent => "IPAD";

		private static string IPhoneUserAgent => "IPHONE";

		private static string MacintoshUserAgent => "Macintosh";

		private static string ChromeUserAgent => "CHROME";

		private static string ArmUserAgent => "ARM";


		public static bool IsIE55OrHigher(NameValueCollection browserCapabilities)
		{
			if (browserCapabilities == null)
			{
				return false;
			}
			string type = browserCapabilities[TypeKey];
			if (!int.TryParse(browserCapabilities[MajorVersionKey], out int result) || !double.TryParse(browserCapabilities[MinorVersionKey], out double result2))
			{
				return false;
			}
			return IsIE55OrHigher(type, result, result2);
		}

		public static bool IsIE55OrHigher(string type, int majorVersion, double minorVersion)
		{
			if (type == null || type.Length < 3 || (string.Compare(type, 0, IEUserAgentPrefix, 0, IEUserAgentPrefix.Length, StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(type, 0, IEModernUserAgentPrefix, 0, IEModernUserAgentPrefix.Length, StringComparison.OrdinalIgnoreCase) != 0))
			{
				return false;
			}
			if (majorVersion >= 6 || (majorVersion == 5 && minorVersion >= 5.0))
			{
				return true;
			}
			return false;
		}

		public static bool IsSafari(string userAgent)
		{
			if (userAgent == null)
			{
				return false;
			}
			if (userAgent.IndexOf(ChromeUserAgent, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return false;
			}
			return userAgent.IndexOf(SafariUserAgent, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static bool IsChrome(string userAgent)
		{
			if (userAgent != null && userAgent.IndexOf(ChromeUserAgent, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return userAgent.IndexOf(EdgeUserAgent, StringComparison.OrdinalIgnoreCase) < 0;
			}
			return false;
		}

		public static bool IsGeckoBrowserEngine(string userAgent)
		{
			if (userAgent == null)
			{
				return false;
			}
			if (userAgent.IndexOf(GeckoUserAgent, StringComparison.OrdinalIgnoreCase) >= 0 && userAgent.IndexOf(SafariUserAgent, StringComparison.OrdinalIgnoreCase) < 0)
			{
				return userAgent.IndexOf(IELayoutEngineName, StringComparison.OrdinalIgnoreCase) < 0;
			}
			return false;
		}
	}
}
