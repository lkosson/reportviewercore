using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class ParagraphNumberCalculator
	{
		private List<int> m_paragraphLevelList = new List<int>();

		internal void UpdateParagraph(IParagraphProps paragraphProps)
		{
			int listLevel = paragraphProps.ListLevel;
			if (listLevel == 0)
			{
				m_paragraphLevelList.Clear();
				return;
			}
			int count = m_paragraphLevelList.Count;
			if (paragraphProps.ListStyle == RPLFormat.ListStyles.Numbered)
			{
				int num = 1;
				if (count > listLevel)
				{
					m_paragraphLevelList.RemoveRange(listLevel, count - listLevel);
					num = m_paragraphLevelList[listLevel - 1] + 1;
					m_paragraphLevelList[listLevel - 1] = num;
				}
				else if (count == listLevel)
				{
					num = m_paragraphLevelList[listLevel - 1] + 1;
					m_paragraphLevelList[listLevel - 1] = num;
				}
				else
				{
					for (int i = count; i < listLevel - 1; i++)
					{
						m_paragraphLevelList.Add(0);
					}
					m_paragraphLevelList.Add(1);
				}
				paragraphProps.ParagraphNumber = num;
			}
			else if (count >= listLevel)
			{
				m_paragraphLevelList.RemoveRange(listLevel - 1, count - listLevel + 1);
			}
		}
	}
}
