using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.WinForms
{
	[ComVisible(false)]
	public sealed class HyperlinkEventArgs : CancelEventArgs
	{
		private string m_hyperlink;

		public string Hyperlink => m_hyperlink;

		internal HyperlinkEventArgs(string hyperlink)
		{
			m_hyperlink = hyperlink;
		}
	}
}
