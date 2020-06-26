using System;
using System.Net;

namespace Microsoft.Reporting.NETCore
{
	[Serializable]
	public sealed class ReportViewerCookieCollection : SyncList<Cookie>
	{
		internal ReportViewerCookieCollection(object syncObject)
			: base(syncObject)
		{
		}
	}
}
