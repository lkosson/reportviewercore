using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class AutoWidthComboBox : ComboBox
	{
		public override Size GetPreferredSize(Size proposedSize)
		{
			Size result = default(Size);
			foreach (object item in base.Items)
			{
				Size size = TextRenderer.MeasureText(item.ToString(), Font);
				if (size.Width > result.Width)
				{
					result = size;
				}
			}
			result.Width += SystemInformation.HorizontalScrollBarArrowWidth + 2 * SystemInformation.Border3DSize.Width;
			if (proposedSize.Width > 0)
			{
				result.Width = Math.Min(proposedSize.Width, result.Width);
			}
			if (proposedSize.Height > 0)
			{
				result.Height = Math.Min(proposedSize.Height, result.Height);
			}
			return result;
		}
	}
}
