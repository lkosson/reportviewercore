using System.Collections;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class SortPageItemIndexesOnTop : IComparer
	{
		private PageItem[] m_children;

		internal SortPageItemIndexesOnTop(PageItem[] children)
		{
			m_children = children;
		}

		int IComparer.Compare(object o1, object o2)
		{
			int num = (int)o1;
			int num2 = (int)o2;
			if (m_children[num] == null || m_children[num2] == null)
			{
				return 0;
			}
			if (m_children[num].ItemPageSizes.Top == m_children[num2].ItemPageSizes.Top)
			{
				if (m_children[num].ItemPageSizes.Left == m_children[num2].ItemPageSizes.Left)
				{
					return 0;
				}
				if (m_children[num].ItemPageSizes.Left < m_children[num2].ItemPageSizes.Left)
				{
					return -1;
				}
				return 1;
			}
			if (m_children[num].ItemPageSizes.Top < m_children[num2].ItemPageSizes.Top)
			{
				return -1;
			}
			return 1;
		}
	}
}
