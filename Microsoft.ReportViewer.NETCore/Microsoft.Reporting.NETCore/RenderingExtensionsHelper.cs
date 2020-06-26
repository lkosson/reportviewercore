using System;
using System.Windows.Forms;

namespace Microsoft.Reporting.NETCore
{
	internal static class RenderingExtensionsHelper
	{
		private static bool ShouldDisplay(RenderingExtension extension)
		{
			return extension.Visible;
		}

		private static string GetEncodedDisplayName(RenderingExtension extension)
		{
			string text = ((LocalizationHelper)LocalizationHelper.Current).GetLocalizedNameForRenderingExtension(extension);
			if (string.IsNullOrEmpty(text))
			{
				text = extension.Name;
			}
			return text.Replace("&", "&&");
		}
	}
}
