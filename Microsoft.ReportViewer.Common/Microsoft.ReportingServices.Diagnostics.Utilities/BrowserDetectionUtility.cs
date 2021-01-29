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

		public static NameValueCollection GetBrowserInfoFromRequest(HttpRequest request)
		{
			NameValueCollection nameValueCollection = new NameValueCollection(9);
			if (request == null)
			{
				return nameValueCollection;
			}
			nameValueCollection.Add(UserAgentKey, request.UserAgent);
			HttpBrowserCapabilities browser = request.Browser;
			try
			{
				if (browser != null)
				{
					nameValueCollection.Add(TypeKey, browser.Type);
					nameValueCollection.Add(ActiveXControlsKey, browser.ActiveXControls.ToString(CultureInfo.InvariantCulture));
					nameValueCollection.Add(EcmaScriptVersionKey, browser.EcmaScriptVersion.ToString());
					nameValueCollection.Add(JavaScriptKey, SupportsJavaScript(browser).ToString(CultureInfo.InvariantCulture));
					nameValueCollection.Add(TablesKey, browser.Tables.ToString(CultureInfo.InvariantCulture));
					nameValueCollection.Add(MajorVersionKey, browser.MajorVersion.ToString(CultureInfo.InvariantCulture));
					nameValueCollection.Add(MinorVersionKey, browser.MinorVersion.ToString(CultureInfo.InvariantCulture));
					nameValueCollection.Add(Win32Key, browser.Win32.ToString(CultureInfo.InvariantCulture));
					return nameValueCollection;
				}
				return nameValueCollection;
			}
			catch (HttpUnhandledException)
			{
				return nameValueCollection;
			}
		}

		internal static ClientArchitecture GetClientArchitecture()
		{
			HttpContext current = HttpContext.Current;
			if (current != null && current.Request != null)
			{
				string userAgent = current.Request.UserAgent;
				if (!string.IsNullOrEmpty(userAgent))
				{
					string[] array = userAgent.Split(userAgentDelimiter, StringSplitOptions.RemoveEmptyEntries);
					string[] array2 = array;
					foreach (string text in array2)
					{
						string text2 = text.Trim();
						if (text2.Equals("x64", StringComparison.OrdinalIgnoreCase))
						{
							return ClientArchitecture.X64;
						}
						if (text2.Equals("WOW64", StringComparison.OrdinalIgnoreCase))
						{
							return ClientArchitecture.X86;
						}
						if (text2.Equals("IA64", StringComparison.OrdinalIgnoreCase))
						{
							return ClientArchitecture.IA64;
						}
					}
				}
			}
			return ClientArchitecture.X86;
		}

		public static bool IsIE55OrHigher(HttpRequest request)
		{
			if (request == null)
			{
				return false;
			}
			HttpBrowserCapabilities browser = request.Browser;
			if (browser == null)
			{
				return false;
			}
			try
			{
				return IsIE55OrHigher(browser.Type, browser.MajorVersion, browser.MinorVersion);
			}
			catch (HttpUnhandledException)
			{
			}
			return false;
		}

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

		public static bool IsIOSSafari()
		{
			return IsIOSSafari(HttpContext.Current.Request);
		}

		public static bool IsIOSSafari(HttpRequest request)
		{
			if (request == null || request.UserAgent == null)
			{
				return false;
			}
			if (request.UserAgent.IndexOf(IPadUserAgent, StringComparison.OrdinalIgnoreCase) < 0)
			{
				return request.UserAgent.IndexOf(IPhoneUserAgent, StringComparison.OrdinalIgnoreCase) >= 0;
			}
			return true;
		}

		public static bool IsSafari(HttpRequest request)
		{
			if (request == null)
			{
				return false;
			}
			return IsSafari(request.UserAgent);
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

		public static bool IsMac(HttpRequest request)
		{
			if (request == null || request.UserAgent == null)
			{
				return false;
			}
			return request.UserAgent.IndexOf(MacintoshUserAgent, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static bool IsArm(HttpRequest request)
		{
			if (request == null || request.UserAgent == null)
			{
				return false;
			}
			string[] source = request.UserAgent.Split(userAgentDelimiter, StringSplitOptions.RemoveEmptyEntries);
			return source.Any((string section) => section.Trim().Equals(ArmUserAgent, StringComparison.OrdinalIgnoreCase));
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

		internal static bool IsTransparentBorderSupported(HttpRequest request)
		{
			if (request == null)
			{
				return true;
			}
			HttpBrowserCapabilities browser = request.Browser;
			string type = browser.Type;
			if (type == null || type.Length < 3 || string.Compare(type, 0, IEUserAgentPrefix, 0, 2, StringComparison.OrdinalIgnoreCase) != 0)
			{
				return true;
			}
			if (browser.MajorVersion < 7)
			{
				return false;
			}
			return true;
		}

		private static bool SupportsJavaScript(HttpBrowserCapabilities browser)
		{
			return browser.EcmaScriptVersion.Major >= 1;
		}
	}
}
