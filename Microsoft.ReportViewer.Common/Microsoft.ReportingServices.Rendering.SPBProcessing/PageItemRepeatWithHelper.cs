namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class PageItemRepeatWithHelper : PageItemHelper
	{
		private double m_relativeTop;

		private double m_relativeBottom;

		private double m_relativeTopToBottom;

		private int m_dataRegionIndex;

		private ItemSizes m_renderItemSize;

		internal double RelativeTop
		{
			get
			{
				return m_relativeTop;
			}
			set
			{
				m_relativeTop = value;
			}
		}

		internal double RelativeBottom
		{
			get
			{
				return m_relativeBottom;
			}
			set
			{
				m_relativeBottom = value;
			}
		}

		internal double RelativeTopToBottom
		{
			get
			{
				return m_relativeTopToBottom;
			}
			set
			{
				m_relativeTopToBottom = value;
			}
		}

		internal int DataRegionIndex
		{
			get
			{
				return m_dataRegionIndex;
			}
			set
			{
				m_dataRegionIndex = value;
			}
		}

		internal ItemSizes RenderItemSize
		{
			get
			{
				return m_renderItemSize;
			}
			set
			{
				m_renderItemSize = value;
			}
		}

		internal PageItemRepeatWithHelper(byte type)
			: base(type)
		{
		}
	}
}
