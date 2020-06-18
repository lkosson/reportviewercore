namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class SearchContext
	{
		private int m_searchPage = -1;

		private int m_itemStartPage = -1;

		private int m_itemEndPage = -1;

		private string m_findValue;

		internal int SearchPage => m_searchPage;

		internal string FindValue => m_findValue;

		internal int ItemStartPage
		{
			get
			{
				return m_itemStartPage;
			}
			set
			{
				m_itemStartPage = value;
			}
		}

		internal int ItemEndPage
		{
			get
			{
				return m_itemEndPage;
			}
			set
			{
				m_itemEndPage = value;
			}
		}

		internal bool IsItemOnSearchPage
		{
			get
			{
				if (m_itemStartPage <= m_searchPage && m_searchPage <= m_itemEndPage)
				{
					return true;
				}
				return false;
			}
		}

		internal SearchContext(int searchPage, string findValue, int itemStartPage, int itemEndPage)
		{
			m_searchPage = searchPage;
			m_findValue = findValue;
			m_itemStartPage = itemStartPage;
			m_itemEndPage = itemEndPage;
		}

		internal SearchContext(SearchContext copy)
		{
			m_searchPage = copy.SearchPage;
			m_findValue = copy.FindValue;
		}
	}
}
