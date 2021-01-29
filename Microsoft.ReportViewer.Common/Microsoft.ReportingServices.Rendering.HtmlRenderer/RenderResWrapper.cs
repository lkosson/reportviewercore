using Microsoft.ReportingServices.HtmlRendering;
using System.Globalization;
using System.Resources;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class RenderResWrapper
	{
		public class Keys
		{
			private static ResourceManager resourceManager = RenderRes.ResourceManager;

			private static CultureInfo _culture = null;

			public const string AccessibleChartNavigationGroupLabel = "AccessibleChartNavigationGroupLabel";

			public const string AccessibleImageNavigationGroupLabel = "AccessibleImageNavigationGroupLabel";

			private Keys()
			{
			}

			public static string GetString(string key, object arg0)
			{
				return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0);
			}
		}

		protected RenderResWrapper()
		{
		}

		public static string AccessibleChartNavigationGroupLabel(string name)
		{
			return Keys.GetString("AccessibleChartNavigationGroupLabel", name);
		}

		public static string AccessibleImageNavigationGroupLabel(string name)
		{
			return Keys.GetString("AccessibleImageNavigationGroupLabel", name);
		}
	}
}
