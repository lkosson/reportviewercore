namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class PageItemContainerHelper : PageItemHelper
	{
		private bool m_itemsCreated;

		private int[] m_indexesLeftToRight;

		private int[] m_indexesTopToBottom;

		private PageItemHelper[] m_repeatWithItems;

		private PageItemHelper m_rightEdgeItem;

		private PageItemHelper[] m_children;

		internal bool ItemsCreated
		{
			get
			{
				return m_itemsCreated;
			}
			set
			{
				m_itemsCreated = value;
			}
		}

		internal int[] IndexesLeftToRight
		{
			get
			{
				return m_indexesLeftToRight;
			}
			set
			{
				m_indexesLeftToRight = value;
			}
		}

		internal int[] IndexesTopToBottom
		{
			get
			{
				return m_indexesTopToBottom;
			}
			set
			{
				m_indexesTopToBottom = value;
			}
		}

		internal PageItemHelper[] RepeatWithItems
		{
			get
			{
				return m_repeatWithItems;
			}
			set
			{
				m_repeatWithItems = value;
			}
		}

		internal PageItemHelper RightEdgeItem
		{
			get
			{
				return m_rightEdgeItem;
			}
			set
			{
				m_rightEdgeItem = value;
			}
		}

		internal PageItemHelper[] Children
		{
			get
			{
				return m_children;
			}
			set
			{
				m_children = value;
			}
		}

		internal PageItemContainerHelper(byte type)
			: base(type)
		{
		}
	}
}
