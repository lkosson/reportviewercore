using System;

namespace Microsoft.Reporting.NETCore
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
