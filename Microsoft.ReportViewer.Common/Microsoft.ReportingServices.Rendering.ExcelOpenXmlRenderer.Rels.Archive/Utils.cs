using System;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Archive
{
	internal static class Utils
	{
		public static string CleanName(string name)
		{
			name = name.Replace('\\', '/');
			if (name.StartsWith("/", StringComparison.Ordinal) || name.StartsWith("#", StringComparison.Ordinal))
			{
				return name;
			}
			return "/" + name;
		}
	}
}
