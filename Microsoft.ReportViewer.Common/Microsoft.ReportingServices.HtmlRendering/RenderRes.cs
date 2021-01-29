using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.HtmlRendering
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class RenderRes
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					ResourceManager resourceManager = resourceMan = new ResourceManager("Microsoft.ReportingServices.HtmlRendering.RenderRes", typeof(RenderRes).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static string AccessibleChartLabel => ResourceManager.GetString("AccessibleChartLabel", resourceCulture);

		internal static string AccessibleChartNavigationGroupLabel => ResourceManager.GetString("AccessibleChartNavigationGroupLabel", resourceCulture);

		internal static string AccessibleImageLabel => ResourceManager.GetString("AccessibleImageLabel", resourceCulture);

		internal static string AccessibleImageNavigationGroupLabel => ResourceManager.GetString("AccessibleImageNavigationGroupLabel", resourceCulture);

		internal static string AccessibleTableBoxLabel => ResourceManager.GetString("AccessibleTableBoxLabel", resourceCulture);

		internal static string AccessibleTextBoxLabel => ResourceManager.GetString("AccessibleTextBoxLabel", resourceCulture);

		internal static string BlankAltText => ResourceManager.GetString("BlankAltText", resourceCulture);

		internal static string DefaultDocMapLabel => ResourceManager.GetString("DefaultDocMapLabel", resourceCulture);

		internal static string DocumentMap => ResourceManager.GetString("DocumentMap", resourceCulture);

		internal static string HideDocMapTooltip => ResourceManager.GetString("HideDocMapTooltip", resourceCulture);

		internal static string HTML40LocalizedName => ResourceManager.GetString("HTML40LocalizedName", resourceCulture);

		internal static string HTML5LocalizedName => ResourceManager.GetString("HTML5LocalizedName", resourceCulture);

		internal static string MHTMLLocalizedName => ResourceManager.GetString("MHTMLLocalizedName", resourceCulture);

		internal static string rrInvalidDeviceInfo => ResourceManager.GetString("rrInvalidDeviceInfo", resourceCulture);

		internal static string rrInvalidSectionError => ResourceManager.GetString("rrInvalidSectionError", resourceCulture);

		internal static string SortAscAltText => ResourceManager.GetString("SortAscAltText", resourceCulture);

		internal static string SortDescAltText => ResourceManager.GetString("SortDescAltText", resourceCulture);

		internal static string ToggleStateCollapse => ResourceManager.GetString("ToggleStateCollapse", resourceCulture);

		internal static string ToggleStateExpand => ResourceManager.GetString("ToggleStateExpand", resourceCulture);

		internal static string UnsortedAltText => ResourceManager.GetString("UnsortedAltText", resourceCulture);

		internal RenderRes()
		{
		}
	}
}
