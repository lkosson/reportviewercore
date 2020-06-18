using System;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal static class RenderingExtensionsHelper
	{
		public static void Populate(ToolStripDropDownItem dropDown, EventHandler handler, RenderingExtension[] extensions)
		{
			dropDown.DropDownItems.Clear();
			foreach (RenderingExtension renderingExtension in extensions)
			{
				if (ShouldDisplay(renderingExtension))
				{
					ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
					toolStripMenuItem.Text = GetEncodedDisplayName(renderingExtension);
					toolStripMenuItem.Tag = renderingExtension;
					if (handler != null)
					{
						toolStripMenuItem.Click += handler;
					}
					dropDown.DropDownItems.Add(toolStripMenuItem);
				}
			}
		}

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
