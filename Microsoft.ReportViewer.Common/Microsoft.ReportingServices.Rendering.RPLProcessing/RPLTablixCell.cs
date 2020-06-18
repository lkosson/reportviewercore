namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLTablixCell
	{
		private IRPLItemFactory m_cellElement;

		private byte m_cellElementState;

		private RPLSizes m_contentSizes;

		protected int m_colSpan = 1;

		private int m_columnIndex = -1;

		private int m_rowIndex = -1;

		public int ColSpan
		{
			get
			{
				return m_colSpan;
			}
			set
			{
				m_colSpan = value;
			}
		}

		public virtual int RowSpan
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}

		public int ColIndex
		{
			get
			{
				return m_columnIndex;
			}
			set
			{
				m_columnIndex = value;
			}
		}

		public int RowIndex
		{
			get
			{
				return m_rowIndex;
			}
			set
			{
				m_rowIndex = value;
			}
		}

		public RPLItem Element
		{
			get
			{
				if (m_cellElement != null)
				{
					return m_cellElement.GetRPLItem();
				}
				return null;
			}
			set
			{
				m_cellElement = value;
			}
		}

		public RPLSizes ContentSizes
		{
			get
			{
				return m_contentSizes;
			}
			set
			{
				m_contentSizes = value;
			}
		}

		public byte ElementState
		{
			get
			{
				return m_cellElementState;
			}
			set
			{
				m_cellElementState = value;
			}
		}

		internal RPLTablixCell()
		{
		}

		internal RPLTablixCell(RPLItem element, byte elementState)
		{
			m_cellElement = element;
			m_cellElementState = elementState;
		}

		internal void SetOffset(long offset, RPLContext context)
		{
			if (offset >= 0)
			{
				m_cellElement = new OffsetItemInfo(offset, context);
			}
		}
	}
}
