using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class ListLevelStack
	{
		private List<ListLevel> m_listLevels = new List<ListLevel>();

		internal void PushTo(IHtmlReportWriter renderer, int listLevel, RPLFormat.ListStyles style, bool writeNoVerticalMargin)
		{
			if (listLevel == 0)
			{
				PopAll();
				return;
			}
			if (m_listLevels.Count == 0)
			{
				Push(renderer, listLevel, style, writeNoVerticalMargin);
				return;
			}
			ListLevel listLevel2 = m_listLevels[m_listLevels.Count - 1];
			if (listLevel == listLevel2.Level)
			{
				if (style != listLevel2.Style)
				{
					Pop();
					Push(renderer, listLevel, style, writeNoVerticalMargin);
				}
				return;
			}
			if (listLevel > listLevel2.Level)
			{
				Push(renderer, listLevel, style, writeNoVerticalMargin);
				return;
			}
			while (listLevel < listLevel2.Level)
			{
				Pop();
				if (m_listLevels.Count == 0)
				{
					listLevel2 = null;
					break;
				}
				listLevel2 = m_listLevels[m_listLevels.Count - 1];
			}
			if (listLevel2 != null && listLevel2.Style != style)
			{
				Pop();
			}
			Push(renderer, listLevel, style, writeNoVerticalMargin);
		}

		internal void Pop()
		{
			if (m_listLevels.Count != 0)
			{
				ListLevel listLevel = m_listLevels[m_listLevels.Count - 1];
				m_listLevels.RemoveAt(m_listLevels.Count - 1);
				listLevel.Close();
			}
		}

		internal void PopAll()
		{
			for (int num = m_listLevels.Count - 1; num > -1; num--)
			{
				Pop();
			}
		}

		internal ListLevel Push(IHtmlReportWriter renderer, int listLevel, RPLFormat.ListStyles style, bool writeNoVerticalMarginClass)
		{
			int num = listLevel - m_listLevels.Count;
			ListLevel listLevel2 = null;
			while (num > 0)
			{
				listLevel2 = new ListLevel(renderer, m_listLevels.Count + 1, style);
				m_listLevels.Add(listLevel2);
				listLevel2.Open(writeNoVerticalMarginClass);
				num--;
			}
			return listLevel2;
		}
	}
}
