using System.IO;
using System.Reflection;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal static class TemplateHelper
	{
		public const string TemplateResourcePrefix = "Microsoft.ReportingServices.Rendering.HtmlRenderer.Templates.";

		public static string GetTemplate(Template template)
		{
			string name = "Microsoft.ReportingServices.Rendering.HtmlRenderer.Templates." + template.ToFullName();
			return GetText(name);
		}

		private static string GetText(string name)
		{
			using (StreamReader streamReader = new StreamReader(GetStream(name)))
			{
				return streamReader.ReadToEnd();
			}
		}

		private static Stream GetStream(string name)
		{
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
		}
	}
}
