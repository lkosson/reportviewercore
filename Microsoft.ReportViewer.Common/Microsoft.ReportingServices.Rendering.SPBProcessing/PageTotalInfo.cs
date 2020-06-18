using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class PageTotalInfo
	{
		private List<KeyValuePair<int, int>> m_regionPageTotals;

		private List<KeyValuePair<int, string>> m_pageNames;

		private bool m_isCounting;

		private bool m_isDone;

		internal bool CalculationDone
		{
			get
			{
				return m_isDone;
			}
			set
			{
				m_isDone = value;
			}
		}

		internal bool IsCounting => m_isCounting;

		internal PageTotalInfo(string initialPageName)
		{
			m_regionPageTotals = new List<KeyValuePair<int, int>>();
			m_pageNames = new List<KeyValuePair<int, string>>();
			if (initialPageName != null)
			{
				m_pageNames.Add(new KeyValuePair<int, string>(1, initialPageName));
			}
		}

		internal void RegisterPageNumberForStart(int currentOverallPageNumber)
		{
			if (!m_isDone)
			{
				if (!m_isCounting)
				{
					m_regionPageTotals.Add(new KeyValuePair<int, int>(currentOverallPageNumber, 1));
					m_isCounting = true;
				}
				else
				{
					int key = m_regionPageTotals[m_regionPageTotals.Count - 1].Key;
					int value = m_regionPageTotals[m_regionPageTotals.Count - 1].Value;
					m_regionPageTotals[m_regionPageTotals.Count - 1] = new KeyValuePair<int, int>(key, value + 1);
				}
			}
		}

		internal void SetPageName(int currentOverallPageNumber, string pageName)
		{
			if (pageName != null)
			{
				m_pageNames.Add(new KeyValuePair<int, string>(currentOverallPageNumber, pageName));
			}
		}

		internal void FinalizePageNumberForTotal()
		{
			if (!m_isDone)
			{
				m_isCounting = false;
			}
		}

		internal List<KeyValuePair<int, int>> GetPageNumberList()
		{
			return m_regionPageTotals;
		}

		internal List<KeyValuePair<int, string>> GetPageNameList()
		{
			return m_pageNames;
		}

		internal void RetrievePageBreakData(int currentOverallPageNumber, out int pageNumber, out int totalPages)
		{
			pageNumber = 1;
			totalPages = 1;
			if (m_regionPageTotals != null && m_regionPageTotals.Count != 0)
			{
				KeyValuePair<int, int> keyValuePair = Search(m_regionPageTotals, currentOverallPageNumber);
				int key = keyValuePair.Key;
				totalPages = keyValuePair.Value;
				pageNumber = currentOverallPageNumber - key + 1;
			}
		}

		internal string GetPageName(int currentOverallPageNumber)
		{
			if (m_pageNames == null || m_pageNames.Count == 0)
			{
				return null;
			}
			if (currentOverallPageNumber < m_pageNames[0].Key)
			{
				return null;
			}
			return Search(m_pageNames, currentOverallPageNumber).Value;
		}

		private KeyValuePair<int, TValue> Search<TValue>(List<KeyValuePair<int, TValue>> collection, int currentOverallPageNumber)
		{
			int num = collection.Count;
			KeyValuePair<int, TValue> result = collection[num - 1];
			if (result.Key <= currentOverallPageNumber)
			{
				return result;
			}
			int num2 = 0;
			int num3 = (num - num2) / 2;
			int num4 = 1;
			while (num3 > 0)
			{
				num3 += num2;
				result = collection[num3 - 1];
				if (result.Key == currentOverallPageNumber)
				{
					return result;
				}
				if (result.Key < currentOverallPageNumber)
				{
					num2 = num3;
					num4 = num3;
				}
				else
				{
					num = num3;
				}
				num3 = (num - num2) / 2;
			}
			return collection[num4 - 1];
		}

		internal void SetupPageTotalInfo(bool isCalculationDone, bool isCounting, List<KeyValuePair<int, int>> pageNumberList, List<KeyValuePair<int, string>> pageNameList)
		{
			m_regionPageTotals.Clear();
			m_regionPageTotals = pageNumberList;
			m_pageNames.Clear();
			m_pageNames = pageNameList;
			m_isCounting = isCounting;
			m_isDone = isCalculationDone;
		}
	}
}
