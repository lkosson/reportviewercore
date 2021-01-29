using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal abstract class DeviceInfo
	{
		internal string ActionScript;

		internal bool AllowScript = true;

		internal string BookmarkId;

		internal bool ExpandContent;

		internal bool HasActionScript;

		internal bool HTMLFragment;

		internal bool OnlyVisibleStyles = true;

		internal bool EnablePowerBIFeatures;

		internal string FindString;

		internal string HtmlPrefixId = "";

		internal string JavascriptPrefixId = "";

		internal string LinkTarget;

		internal string ReplacementRoot;

		internal string ResourceStreamRoot;

		internal int Section;

		internal string StylePrefixId = "a";

		internal bool StyleStream;

		internal bool OutlookCompat;

		internal int Zoom = 100;

		internal bool AccessibleTablix;

		internal DataVisualizationFitSizing DataVisualizationFitSizing;

		internal bool IsBrowserIE = true;

		internal bool IsBrowserSafari;

		internal bool IsBrowserGeckoEngine;

		internal bool IsBrowserIE6Or7StandardsMode;

		internal bool IsBrowserIE6;

		internal bool IsBrowserIE7;

		internal BrowserMode BrowserMode;

		internal readonly string BrowserMode_Quirks = "quirks";

		internal readonly string BrowserMode_Standards = "standards";

		internal string NavigationId;

		internal bool ImageConsolidation = true;

		private static readonly Regex m_safeForJavascriptRegex = new Regex("^([a-zA-Z0-9_\\.]+)$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		private NameValueCollection m_rawDeviceInfo;

		public NameValueCollection RawDeviceInfo => m_rawDeviceInfo;

		public void ParseDeviceInfo(NameValueCollection deviceInfo, NameValueCollection browserCaps)
		{
			m_rawDeviceInfo = deviceInfo;
			string text = deviceInfo["HTMLFragment"];
			if (string.IsNullOrEmpty(text))
			{
				text = deviceInfo["MHTMLFragment"];
			}
			if (!string.IsNullOrEmpty(text))
			{
				HTMLFragment = ParseBool(text, defaultValue: false);
			}
			object obj = browserCaps[BrowserDetectionUtility.TypeKey];
			if (obj != null && ((string)obj).StartsWith("Netscape", StringComparison.OrdinalIgnoreCase))
			{
				IsBrowserIE = false;
			}
			if (HTMLFragment)
			{
				string text2 = deviceInfo["PrefixId"];
				if (!string.IsNullOrEmpty(text2))
				{
					VerifySafeForJavascript(text2);
					HtmlPrefixId = text2;
					StylePrefixId = (JavascriptPrefixId = "A" + Guid.NewGuid().ToString().Replace("-", ""));
				}
			}
			text = deviceInfo["BookmarkId"];
			if (!string.IsNullOrEmpty(text))
			{
				BookmarkId = text;
			}
			text = deviceInfo["JavaScript"];
			if (!string.IsNullOrEmpty(text))
			{
				AllowScript = ParseBool(text, defaultValue: true);
			}
			if (AllowScript)
			{
				text = browserCaps[BrowserDetectionUtility.JavaScriptKey];
				if (!string.IsNullOrEmpty(text))
				{
					AllowScript = ParseBool(text, defaultValue: true);
				}
				if (AllowScript)
				{
					text = deviceInfo["ActionScript"];
					if (!string.IsNullOrEmpty(text))
					{
						VerifySafeForJavascript(text);
						ActionScript = text;
						HasActionScript = true;
					}
				}
			}
			string text3 = deviceInfo["UserAgent"];
			if (text3 == null && browserCaps != null)
			{
				text3 = browserCaps["UserAgent"];
			}
			if (text3 != null && text3.Contains("MSIE 6.0"))
			{
				IsBrowserIE6 = true;
			}
			if (text3 != null && text3.Contains("MSIE 7.0"))
			{
				IsBrowserIE7 = true;
			}
			IsBrowserGeckoEngine = BrowserDetectionUtility.IsGeckoBrowserEngine(text3);
			if (IsBrowserGeckoEngine)
			{
				IsBrowserIE = false;
			}
			else if (BrowserDetectionUtility.IsSafari(text3) || BrowserDetectionUtility.IsChrome(text3))
			{
				IsBrowserSafari = true;
				IsBrowserIE = false;
			}
			text = deviceInfo["ExpandContent"];
			if (!string.IsNullOrEmpty(text))
			{
				ExpandContent = ParseBool(text, defaultValue: false);
			}
			text = deviceInfo["EnablePowerBIFeatures"];
			if (!string.IsNullOrEmpty(text))
			{
				EnablePowerBIFeatures = ParseBool(text, defaultValue: false);
			}
			text = deviceInfo["Section"];
			if (!string.IsNullOrEmpty(text))
			{
				Section = ParseInt(text, 0);
			}
			text = deviceInfo["FindString"];
			if (!string.IsNullOrEmpty(text) && text.LastIndexOfAny(HTML4Renderer.m_standardLineBreak.ToCharArray()) < 0)
			{
				FindString = text.ToUpperInvariant();
			}
			text = deviceInfo["LinkTarget"];
			if (!string.IsNullOrEmpty(text))
			{
				VerifySafeForJavascript(text);
				LinkTarget = text;
			}
			text = deviceInfo["OutlookCompat"];
			if (!string.IsNullOrEmpty(text))
			{
				OutlookCompat = ParseBool(text, defaultValue: false);
			}
			text = deviceInfo["AccessibleTablix"];
			if (!string.IsNullOrEmpty(text))
			{
				AccessibleTablix = ParseBool(text, defaultValue: false);
			}
			text = deviceInfo["StyleStream"];
			if (!string.IsNullOrEmpty(text))
			{
				StyleStream = ParseBool(text, defaultValue: false);
			}
			OnlyVisibleStyles = !StyleStream;
			text = deviceInfo["OnlyVisibleStyles"];
			if (!string.IsNullOrEmpty(text))
			{
				OnlyVisibleStyles = ParseBool(text, OnlyVisibleStyles);
			}
			text = deviceInfo["ResourceStreamRoot"];
			if (!string.IsNullOrEmpty(text))
			{
				VerifySafeForRoots(text);
				ResourceStreamRoot = text;
			}
			text = deviceInfo["StreamRoot"];
			if (!string.IsNullOrEmpty(text))
			{
				VerifySafeForRoots(text);
			}
			if (IsBrowserIE)
			{
				text = deviceInfo["Zoom"];
				if (!string.IsNullOrEmpty(text))
				{
					Zoom = ParseInt(text, 100);
				}
			}
			text = deviceInfo["ReplacementRoot"];
			if (!string.IsNullOrEmpty(text))
			{
				VerifySafeForRoots(text);
				ReplacementRoot = text;
			}
			text = deviceInfo["ImageConsolidation"];
			if (!string.IsNullOrEmpty(text))
			{
				ImageConsolidation = ParseBool(text, ImageConsolidation);
			}
			text = deviceInfo["BrowserMode"];
			if (!string.IsNullOrEmpty(text))
			{
				if (string.Compare(text, BrowserMode_Quirks, StringComparison.OrdinalIgnoreCase) == 0)
				{
					BrowserMode = BrowserMode.Quirks;
				}
				else if (string.Compare(text, BrowserMode_Standards, StringComparison.OrdinalIgnoreCase) == 0)
				{
					BrowserMode = BrowserMode.Standards;
					if (IsBrowserIE && text3 != null && (IsBrowserIE7 || IsBrowserIE6))
					{
						IsBrowserIE6Or7StandardsMode = true;
					}
				}
			}
			if (IsBrowserIE && ImageConsolidation && string.IsNullOrEmpty(deviceInfo["ImageConsolidation"]) && IsBrowserIE6)
			{
				ImageConsolidation = false;
			}
			if (!AllowScript)
			{
				ImageConsolidation = false;
			}
			text = deviceInfo["DataVisualizationFitSizing"];
			if (!string.IsNullOrEmpty(text) && string.Compare(text, "Approximate", StringComparison.OrdinalIgnoreCase) == 0)
			{
				DataVisualizationFitSizing = DataVisualizationFitSizing.Approximate;
			}
		}

		public abstract bool IsSupported(string value, bool isTrue, out bool isRelative);

		public virtual void VerifySafeForJavascript(string value)
		{
			if (value != null)
			{
				Match match = m_safeForJavascriptRegex.Match(value.Trim());
				if (!match.Success)
				{
					throw new ArgumentOutOfRangeException("value");
				}
			}
		}

		internal void VerifySafeForRoots(string value)
		{
			if (IsSupported(value, isTrue: true, out bool isRelative) && !isRelative)
			{
				return;
			}
			int num = value.IndexOf(':');
			int num2 = value.IndexOf('?');
			int num3 = value.IndexOf('&');
			if (num != -1 || num3 != -1)
			{
				if (num2 == -1 && (num != -1 || num3 != -1))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (num != -1 && num < num2)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (num3 != -1 && num3 < num2)
				{
					throw new ArgumentOutOfRangeException("value");
				}
			}
		}

		private static bool ParseBool(string boolValue, bool defaultValue)
		{
			if (bool.TryParse(boolValue, out bool result))
			{
				return result;
			}
			return defaultValue;
		}

		private static int ParseInt(string intValue, int defaultValue)
		{
			if (int.TryParse(intValue, out int result))
			{
				return result;
			}
			return defaultValue;
		}
	}
}
