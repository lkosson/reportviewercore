using System.Globalization;
using System.Windows.Forms;

namespace Microsoft.ReportingServices.Common
{
	internal static class MessageBoxWrappers
	{
		private static MessageBoxOptions GetMessageBoxOptions(Control owner)
		{
			if (owner != null)
			{
				if (owner.RightToLeft == RightToLeft.Yes)
				{
					return MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
				}
				if (owner.RightToLeft == RightToLeft.Inherit)
				{
					return GetMessageBoxOptions(owner.Parent);
				}
				return (MessageBoxOptions)0;
			}
			if (CultureInfo.CurrentCulture.TextInfo.IsRightToLeft)
			{
				return MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
			}
			return (MessageBoxOptions)0;
		}

		public static DialogResult ShowMessageBox(Control owner, string text)
		{
			return ShowMessageBox(owner, text, string.Empty);
		}

		public static DialogResult ShowMessageBox(Control owner, string text, string caption)
		{
			return ShowMessageBox(owner, text, caption, MessageBoxButtons.OK);
		}

		public static DialogResult ShowMessageBox(Control owner, string text, string caption, MessageBoxButtons buttons)
		{
			return ShowMessageBox(owner, text, caption, buttons, MessageBoxIcon.None);
		}

		public static DialogResult ShowMessageBox(Control owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return ShowMessageBox(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
		}

		public static DialogResult ShowMessageBox(Control owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, GetMessageBoxOptions(owner));
		}
	}
}
