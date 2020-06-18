using System;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal static class ZoomMenuHelper
	{
		private static int[] DefaultPercents = new int[7]
		{
			500,
			200,
			150,
			100,
			75,
			50,
			25
		};

		private static bool NeedCustomEntry(ZoomMode zoomMode, int zoomPercent)
		{
			if (zoomMode != ZoomMode.Percent)
			{
				return false;
			}
			int[] defaultPercents = DefaultPercents;
			foreach (int num in defaultPercents)
			{
				if (zoomPercent == num)
				{
					return false;
				}
			}
			return true;
		}

		public static void Populate(ToolStripDropDownItem parentItem, EventHandler handler, ZoomMode selectedMode, int selectedPercent)
		{
			parentItem.DropDownItems.Clear();
			parentItem.DropDownItems.Add(CreateMenuItem(ZoomMode.PageWidth, 100, handler, selectedMode == ZoomMode.PageWidth));
			parentItem.DropDownItems.Add(CreateMenuItem(ZoomMode.FullPage, 100, handler, selectedMode == ZoomMode.FullPage));
			int[] defaultPercents = DefaultPercents;
			foreach (int num in defaultPercents)
			{
				bool selected = selectedMode == ZoomMode.Percent && selectedPercent == num;
				parentItem.DropDownItems.Add(CreateMenuItem(ZoomMode.Percent, num, handler, selected));
			}
			if (NeedCustomEntry(selectedMode, selectedPercent))
			{
				parentItem.DropDownItems.Add(CreateMenuItem(selectedMode, selectedPercent, handler, selected: true));
			}
		}

		private static ToolStripMenuItem CreateMenuItem(ZoomMode mode, int zoomPercent, EventHandler handler, bool selected)
		{
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
			toolStripMenuItem.Click += handler;
			toolStripMenuItem.Checked = selected;
			ZoomItem zoomItem = new ZoomItem(mode, zoomPercent);
			toolStripMenuItem.Text = zoomItem.ToString();
			toolStripMenuItem.Tag = zoomItem;
			return toolStripMenuItem;
		}

		public static void Populate(ToolStripComboBox comboBox, ZoomMode selectedMode, int selectedPercent)
		{
			comboBox.Items.Clear();
			comboBox.Items.Add(new ZoomItem(ZoomMode.PageWidth, 100));
			comboBox.Items.Add(new ZoomItem(ZoomMode.FullPage, 100));
			if (selectedMode != ZoomMode.Percent)
			{
				comboBox.SelectedIndex = ((selectedMode != ZoomMode.PageWidth) ? 1 : 0);
			}
			int[] defaultPercents = DefaultPercents;
			foreach (int num in defaultPercents)
			{
				comboBox.Items.Add(new ZoomItem(ZoomMode.Percent, num));
				if (selectedMode == ZoomMode.Percent && selectedPercent == num)
				{
					comboBox.SelectedIndex = comboBox.Items.Count - 1;
				}
			}
			if (NeedCustomEntry(selectedMode, selectedPercent))
			{
				comboBox.Items.Add(new ZoomItem(selectedMode, selectedPercent));
				comboBox.SelectedIndex = comboBox.Items.Count - 1;
			}
		}
	}
}
