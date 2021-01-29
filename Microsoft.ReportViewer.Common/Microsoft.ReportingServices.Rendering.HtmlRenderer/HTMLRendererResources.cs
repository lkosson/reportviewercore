using Microsoft.Reporting.Common;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal static class HTMLRendererResources
	{
		public const string ResourceNamespace = "Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.";

		public const string TogglePlus = "TogglePlus.gif";

		public const string ToggleMinus = "ToggleMinus.gif";

		public const string SortAsc = "sortAsc.gif";

		public const string SortDesc = "sortDesc.gif";

		public const string SortNone = "unsorted.gif";

		public const string Blank = "Blank.gif";

		public const string CommonScript = "Common.js";

		public const string FitProportionalScript = "FitProportional.js";

		public const string FixedHeaderScript = "FixedHeader.js";

		public const string CanGrowFalseScript = "CanGrowFalse.js";

		public const string ImageConsolidationScript = "ImageConsolidation.js";

		public const string Html5ToolbarCss = "Html5Toolbar.css";

		public const string Html40ViewerCss = "Html40Viewer.css";

		public const string ReportingServices40Css = "ReportingServices40.css";

		public const string Html5RendererCss = "Html5Renderer.css";

		public const string JQueryUiCss = "jqueryui.min.css";

		public const string ReportingServicesHybridCss = "ReportingServicesHybrid.css";

		public const string ReportingServicesCss = "ReportingServices.css";

		public const string Html5RenderingExtensionJs = "Html5RenderingExtensionJs.js";

		public const string Html5RendererJs = "Html5Renderer.js";

		public const string ReportingServicesJs = "ReportingServices.js";

		public const string JQueryJs = "jquery.min.js";

		public const string JQueryUiJs = "jqueryui.min.js";

		public const string KnockoutJs = "knockoutjs.js";

		public const string ReportingServicesHybridJs = "ReportingServicesHybrid.js";

		private static ResourceList m_resourceList;

		internal static ResourceList Resources => m_resourceList;

		static HTMLRendererResources()
		{
			m_resourceList = new ResourceList();
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.TogglePlus.gif", "image/gif");
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.ToggleMinus.gif", "image/gif");
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortAsc.gif", "image/gif");
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.sortDesc.gif", "image/gif");
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.unsorted.gif", "image/gif");
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.Blank.gif", "image/gif");
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.Common.js", "application/javascript", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.FitProportional.js", "application/javascript", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.FixedHeader.js", "application/javascript", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.CanGrowFalse.js", "application/javascript", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.ImageConsolidation.js", "application/javascript", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.ReportingServices.js", "application/javascript", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.Html40Viewer.css", "text/css", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.Html5Renderer.css", "text/css", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.Html5Toolbar.css", "text/css", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.jqueryui.min.css", "text/css", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.ReportingServices40.css", "text/css", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.ReportingServicesHybrid.css", "text/css", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.ReportingServices.css", "text/css", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.Html5Renderer.js", "application/javascript", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.Html5RenderingExtensionJs.js", "application/javascript", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.jquery.min.js", "application/javascript", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.jqueryui.min.js", "application/javascript", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.knockoutjs.js", "application/javascript", hasDebugMode: true);
			m_resourceList.Add("Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources.ReportingServicesHybrid.js", "application/javascript", hasDebugMode: true);
		}

		public static void PopulateResources(Dictionary<string, byte[]> nameToResourceMap, string prefix)
		{
			Encoding uTF = Encoding.UTF8;
			nameToResourceMap["TogglePlus.gif"] = CreateFullName(uTF, prefix, "TogglePlus.gif");
			nameToResourceMap["ToggleMinus.gif"] = CreateFullName(uTF, prefix, "ToggleMinus.gif");
			nameToResourceMap["sortAsc.gif"] = CreateFullName(uTF, prefix, "sortAsc.gif");
			nameToResourceMap["sortDesc.gif"] = CreateFullName(uTF, prefix, "sortDesc.gif");
			nameToResourceMap["unsorted.gif"] = CreateFullName(uTF, prefix, "unsorted.gif");
			nameToResourceMap["Blank.gif"] = CreateFullName(uTF, prefix, "Blank.gif");
		}

		public static Stream GetStream(string name, out string mimeType)
		{
			return EmbeddedResources.GetStream(m_resourceList, "Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources." + name, out mimeType);
		}

		private static byte[] CreateFullName(Encoding encoding, string prefix, string name)
		{
			return encoding.GetBytes(prefix + name);
		}

		public static byte[] GetBytes(string name)
		{
			string mimeType = null;
			return GetBytes(name, out mimeType);
		}

		public static byte[] GetBytes(string name, out string mimeType)
		{
			return EmbeddedResources.Get(m_resourceList, "Microsoft.ReportingServices.Rendering.HtmlRenderer.RendererResources." + name, out mimeType);
		}

		public static byte[] GetBytesFullname(string nameWithNamespace, out string mimeType)
		{
			return EmbeddedResources.Get(m_resourceList, nameWithNamespace, out mimeType);
		}
	}
}
