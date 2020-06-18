using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class Bookmarks
	{
		private BookmarksHashtable m_reportBookmarks;

		public Bookmark this[string bookmarkId]
		{
			get
			{
				if (bookmarkId == null || m_reportBookmarks == null)
				{
					return null;
				}
				BookmarkInformation bookmarkInformation = m_reportBookmarks[bookmarkId];
				if (bookmarkInformation != null)
				{
					return new Bookmark(bookmarkId, bookmarkInformation);
				}
				return null;
			}
		}

		public IDictionaryEnumerator BookmarksEnumerator
		{
			get
			{
				if (m_reportBookmarks == null)
				{
					return null;
				}
				return m_reportBookmarks.GetEnumerator();
			}
		}

		internal Bookmarks(BookmarksHashtable reportBookmarks)
		{
			Global.Tracer.Assert(reportBookmarks != null, "The bookmark hashtable being wrapped cannot be null.");
			m_reportBookmarks = reportBookmarks;
		}
	}
}
