namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLReportSection
	{
		private string m_id;

		private float m_columnSpacing;

		private int m_columnCount;

		private RPLSizes m_bodyArea;

		private RPLItemMeasurement m_header;

		private RPLItemMeasurement m_footer;

		private RPLItemMeasurement[] m_columns;

		private long m_endOffset = -1L;

		public string ID
		{
			get
			{
				return m_id;
			}
			set
			{
				m_id = value;
			}
		}

		public float ColumnSpacing
		{
			get
			{
				return m_columnSpacing;
			}
			set
			{
				m_columnSpacing = value;
			}
		}

		public int ColumnCount
		{
			get
			{
				return m_columnCount;
			}
			set
			{
				m_columnCount = value;
			}
		}

		public RPLSizes BodyArea
		{
			get
			{
				return m_bodyArea;
			}
			set
			{
				m_bodyArea = value;
			}
		}

		public RPLItemMeasurement Header
		{
			get
			{
				return m_header;
			}
			set
			{
				m_header = value;
			}
		}

		public RPLItemMeasurement Footer
		{
			get
			{
				return m_footer;
			}
			set
			{
				m_footer = value;
			}
		}

		public RPLItemMeasurement[] Columns
		{
			get
			{
				return m_columns;
			}
			set
			{
				m_columns = value;
			}
		}

		internal RPLReportSection(int columns)
		{
			m_columns = new RPLItemMeasurement[columns];
		}

		internal RPLReportSection(long endOffset)
		{
			m_endOffset = endOffset;
		}
	}
}
