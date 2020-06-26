using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.NETCore
{
	[ComVisible(false)]
	public sealed class DocumentMapNavigationEventArgs : CancelEventArgs
	{
		private string m_docMapID;

		public string DocumentMapId => m_docMapID;

		public DocumentMapNavigationEventArgs(string docMapID)
		{
			m_docMapID = docMapID;
		}
	}
}
