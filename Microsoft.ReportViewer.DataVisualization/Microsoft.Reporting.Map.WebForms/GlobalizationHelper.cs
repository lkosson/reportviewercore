using System.Globalization;
using System.Windows.Forms;

namespace Microsoft.Reporting.Map.WebForms
{
	internal static class GlobalizationHelper
	{
		public static MessageBoxOptions GetMessageBoxOptions(Control owner)
		{
			if (owner != null && owner.RightToLeft == RightToLeft.Yes)
			{
				return MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
			}
			return GetMessageBoxOptions();
		}

		public static MessageBoxOptions GetMessageBoxOptions()
		{
			if (CultureInfo.CurrentCulture.TextInfo.IsRightToLeft)
			{
				return MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
			}
			return (MessageBoxOptions)0;
		}
	}
}
