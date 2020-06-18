using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class PaginationInfo
	{
		private ArrayList m_pages;

		private int m_totalPageNumber;

		internal int TotalPageNumber
		{
			get
			{
				return m_totalPageNumber;
			}
			set
			{
				m_totalPageNumber = value;
			}
		}

		internal Page this[int pageNumber]
		{
			get
			{
				return (Page)m_pages[pageNumber];
			}
			set
			{
				m_pages[pageNumber] = value;
			}
		}

		internal int CurrentPageCount => m_pages.Count;

		internal PaginationInfo()
		{
			m_pages = new ArrayList();
		}

		internal void AddPage(Page page)
		{
			m_pages.Add(page);
		}

		internal void Clear()
		{
			m_pages.Clear();
		}

		internal void InsertPage(int pageNumber, Page page)
		{
			m_pages.Insert(pageNumber, page);
		}

		internal void RemovePage(int pageNumber)
		{
			m_pages.RemoveAt(pageNumber);
		}
	}
}
