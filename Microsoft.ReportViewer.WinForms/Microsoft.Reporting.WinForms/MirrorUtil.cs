using System.Windows.Forms;

namespace Microsoft.Reporting.WinForms
{
	internal static class MirrorUtil
	{
		private const int WS_EX_RIGHT = 4096;

		private const int WS_EX_RTLREADING = 8192;

		private const int WS_EX_LEFTSCROLLBAR = 16384;

		private const int WS_EX_LAYOUTRTL = 4194304;

		private const int WS_EX_NOINHERITLAYOUT = 1048576;

		public static CreateParams CreateMirrorParams(CreateParams baseParams, RightToLeft rtl)
		{
			if (rtl == RightToLeft.Yes)
			{
				baseParams.ExStyle = (baseParams.ExStyle | 0x400000 | 0x100000);
				baseParams.ExStyle &= -28673;
			}
			return baseParams;
		}
	}
}
