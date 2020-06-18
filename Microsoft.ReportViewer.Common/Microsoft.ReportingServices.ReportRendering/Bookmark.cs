using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class Bookmark
	{
		private BookmarkInformation m_underlyingNode;

		private string m_bookmarkId;

		public string BookmarkId => m_bookmarkId;

		public string UniqueName => m_underlyingNode.Id;

		public int Page => m_underlyingNode.Page;

		internal Bookmark(string bookmarkId, BookmarkInformation underlyingNode)
		{
			Global.Tracer.Assert(underlyingNode != null, "The bookmark node being wrapped cannot be null.");
			m_bookmarkId = bookmarkId;
			m_underlyingNode = underlyingNode;
		}
	}
}
