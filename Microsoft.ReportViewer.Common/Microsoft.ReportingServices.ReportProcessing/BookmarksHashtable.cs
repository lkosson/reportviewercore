using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class BookmarksHashtable : HashtableInstanceInfo
	{
		internal BookmarkInformation this[string key]
		{
			get
			{
				return (BookmarkInformation)m_hashtable[key];
			}
			set
			{
				m_hashtable[key] = value;
			}
		}

		internal BookmarksHashtable()
		{
		}

		internal BookmarksHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(string bookmark, BookmarkInformation bookmarkInfo)
		{
			m_hashtable.Add(bookmark, bookmarkInfo);
		}

		internal void Add(string bookmark, int page, string id)
		{
			BookmarkInformation bookmarkInformation = null;
			if (m_hashtable.Contains(bookmark))
			{
				bookmarkInformation = this[bookmark];
				if (bookmarkInformation.Page > page)
				{
					bookmarkInformation.Page = page;
					bookmarkInformation.Id = id;
				}
			}
			else
			{
				bookmarkInformation = new BookmarkInformation(id, page);
				m_hashtable.Add(bookmark, bookmarkInformation);
			}
		}
	}
}
