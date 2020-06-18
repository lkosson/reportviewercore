using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class ProcessPageBreaks
	{
		private bool m_pbAtStart;

		private double m_minAtStart;

		private bool m_spanItems;

		private double m_maxSpanItems;

		private List<double> m_pbsAtEnd;

		private double m_maxAtEnd;

		internal void ProcessItemPageBreaks(PageItem pageItem)
		{
			if (pageItem.ItemState != PageItem.State.OnPage && pageItem.ItemState != PageItem.State.OnPageHidden)
			{
				if (pageItem.ItemState == PageItem.State.OnPagePBEnd)
				{
					if (m_pbsAtEnd != null)
					{
						m_maxAtEnd = Math.Max(m_maxAtEnd, pageItem.ItemPageSizes.Bottom);
					}
					else
					{
						m_pbsAtEnd = new List<double>();
						m_maxAtEnd = pageItem.ItemPageSizes.Bottom;
					}
					m_pbsAtEnd.Add(pageItem.ItemPageSizes.Bottom);
				}
				else if (pageItem.ItemState == PageItem.State.TopNextPage)
				{
					if (m_pbAtStart)
					{
						m_minAtStart = Math.Min(m_minAtStart, pageItem.ItemPageSizes.Top);
						return;
					}
					m_pbAtStart = true;
					m_minAtStart = pageItem.ItemPageSizes.Top;
				}
				else if (pageItem.ItemState == PageItem.State.SpanPages)
				{
					m_spanItems = true;
					m_maxSpanItems = Math.Max(m_maxSpanItems, pageItem.ItemPageSizes.Top);
					ResolveItemPosition(m_maxSpanItems, checkSpanItems: false);
				}
			}
			else
			{
				ResolveItemPosition(pageItem.ItemPageSizes.Bottom, checkSpanItems: true);
			}
		}

		internal bool HasPageBreaks(ref double breakPosition, ref double pageItemHeight)
		{
			if (m_pbAtStart)
			{
				breakPosition = m_minAtStart;
				pageItemHeight = m_minAtStart;
				return true;
			}
			if (m_pbsAtEnd != null)
			{
				breakPosition = m_maxAtEnd;
				return true;
			}
			if (m_spanItems)
			{
				breakPosition = m_maxSpanItems;
				return true;
			}
			return false;
		}

		private void ResolveItemPosition(double itemPosition, bool checkSpanItems)
		{
			if (m_pbAtStart && (RoundedDouble)itemPosition >= m_minAtStart)
			{
				m_pbAtStart = false;
				m_minAtStart = 0.0;
			}
			if (checkSpanItems && m_spanItems && (RoundedDouble)itemPosition >= m_maxSpanItems)
			{
				m_spanItems = false;
				m_maxSpanItems = 0.0;
			}
			if (m_pbsAtEnd == null || !((RoundedDouble)itemPosition < m_maxAtEnd))
			{
				return;
			}
			m_maxAtEnd = 0.0;
			int num = 0;
			while (num < m_pbsAtEnd.Count)
			{
				if ((RoundedDouble)itemPosition < m_pbsAtEnd[num])
				{
					m_pbsAtEnd.RemoveAt(num);
					continue;
				}
				num++;
				m_maxAtEnd = Math.Max(m_maxAtEnd, m_pbsAtEnd[num]);
			}
			if (m_pbsAtEnd.Count == 0)
			{
				m_pbsAtEnd = null;
			}
		}
	}
}
