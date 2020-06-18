using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTablix : RPLItem
	{
		private int m_columnHeaderRows;

		private int m_rowHeaderColumns;

		private int m_colsBeforeRowHeaders;

		private RPLFormat.Directions m_layoutDirection;

		private float[] m_columnsWidths;

		private float[] m_rowHeights;

		private float m_contentLeft;

		private float m_contentTop;

		private bool[] m_fixedColumns;

		private byte[] m_rowsState;

		private Queue<RPLTablixRow> m_rows;

		private long m_nextRowStart = -1L;

		private RPLTablixMemberDef[] m_tablixRowMembersDef;

		private RPLTablixMemberDef[] m_tablixColMembersDef;

		public RPLFormat.Directions LayoutDirection
		{
			get
			{
				return m_layoutDirection;
			}
			set
			{
				m_layoutDirection = value;
			}
		}

		public int ColumnHeaderRows
		{
			get
			{
				return m_columnHeaderRows;
			}
			set
			{
				m_columnHeaderRows = value;
			}
		}

		public int RowHeaderColumns
		{
			get
			{
				return m_rowHeaderColumns;
			}
			set
			{
				m_rowHeaderColumns = value;
			}
		}

		public int ColsBeforeRowHeaders
		{
			get
			{
				return m_colsBeforeRowHeaders;
			}
			set
			{
				m_colsBeforeRowHeaders = value;
			}
		}

		public float ContentTop
		{
			get
			{
				return m_contentTop;
			}
			set
			{
				m_contentTop = value;
			}
		}

		public float ContentLeft
		{
			get
			{
				return m_contentLeft;
			}
			set
			{
				m_contentLeft = value;
			}
		}

		public float[] ColumnWidths
		{
			get
			{
				return m_columnsWidths;
			}
			set
			{
				m_columnsWidths = value;
			}
		}

		public float[] RowHeights
		{
			get
			{
				return m_rowHeights;
			}
			set
			{
				m_rowHeights = value;
			}
		}

		public bool[] FixedColumns
		{
			get
			{
				return m_fixedColumns;
			}
			set
			{
				m_fixedColumns = value;
			}
		}

		public byte[] RowsState
		{
			get
			{
				return m_rowsState;
			}
			set
			{
				m_rowsState = value;
			}
		}

		internal long NextRowStart
		{
			set
			{
				m_nextRowStart = value;
			}
		}

		internal Queue<RPLTablixRow> Rows
		{
			set
			{
				m_rows = value;
			}
		}

		internal RPLTablixMemberDef[] TablixRowMembersDef
		{
			get
			{
				return m_tablixRowMembersDef;
			}
			set
			{
				m_tablixRowMembersDef = value;
			}
		}

		internal RPLTablixMemberDef[] TablixColMembersDef
		{
			get
			{
				return m_tablixColMembersDef;
			}
			set
			{
				m_tablixColMembersDef = value;
			}
		}

		internal RPLTablix()
		{
			m_rplElementProps = new RPLItemProps();
			m_rplElementProps.Definition = new RPLItemPropsDef();
		}

		internal RPLTablix(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}

		internal RPLTablix(RPLItemProps rplElementProps)
			: base(rplElementProps)
		{
		}

		public bool FixedRow(int index)
		{
			if (index < 0 || m_rowsState == null || index >= m_rowsState.Length)
			{
				return false;
			}
			return (m_rowsState[index] & 1) > 0;
		}

		public bool SharedLayoutRow(int index)
		{
			if (index < 0 || m_rowsState == null || index >= m_rowsState.Length)
			{
				return false;
			}
			return (m_rowsState[index] & 2) > 0;
		}

		public bool UseSharedLayoutRow(int index)
		{
			if (index < 0 || m_rowsState == null || index >= m_rowsState.Length)
			{
				return false;
			}
			return (m_rowsState[index] & 4) > 0;
		}

		public float GetRowHeight(int index, int span)
		{
			float num = 0f;
			for (int i = index; i < index + span; i++)
			{
				num += m_rowHeights[i];
			}
			return num;
		}

		public float GetColumnWidth(int index, int span)
		{
			float num = 0f;
			for (int i = index; i < index + span; i++)
			{
				num += m_columnsWidths[i];
			}
			return num;
		}

		public RPLTablixRow GetNextRow()
		{
			if (m_rows != null)
			{
				if (m_rows.Count == 0)
				{
					m_rows = null;
					return null;
				}
				return m_rows.Dequeue();
			}
			if (m_nextRowStart >= 0)
			{
				return RPLReader.ReadTablixRow(m_nextRowStart, m_context, m_tablixRowMembersDef, m_tablixColMembersDef, ref m_nextRowStart);
			}
			return null;
		}

		internal void AddRow(RPLTablixRow row)
		{
			if (m_rows == null)
			{
				m_rows = new Queue<RPLTablixRow>();
			}
			m_rows.Enqueue(row);
		}
	}
}
