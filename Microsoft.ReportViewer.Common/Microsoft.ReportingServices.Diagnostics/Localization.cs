using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal static class Localization
	{
		public static string ClientBrowserCultureName => ClientPrimaryCulture.Name;

		public static string ClientCurrentCultureName => ClientPrimaryCulture.Name;

		public static CultureInfo ClientPrimaryCulture => Thread.CurrentThread.CurrentCulture;

		public static CultureInfo SqlCulture => ClientPrimaryCulture;

		public static CultureInfo DefaultReportServerSpecificCulture
		{
			get
			{
				CultureInfo installedUICulture = CultureInfo.InstalledUICulture;
				if (installedUICulture.IsNeutralCulture)
				{
					return new CultureInfo(CultureInfo.CreateSpecificCulture(installedUICulture.Name).LCID, useUserOverride: false);
				}
				return installedUICulture;
			}
		}

		public static CultureInfo CatalogCulture => CultureInfo.InvariantCulture;

		public static int CatalogCultureCompare(string a, string b)
		{
			return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
		}
	}
}
