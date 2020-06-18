using System;

namespace Microsoft.Reporting.WinForms
{
	[Serializable]
	public sealed class ReportViewerHeaderCollection : SyncList<string>
	{
		internal ReportViewerHeaderCollection(object syncObject)
			: base(syncObject)
		{
		}
	}
}
