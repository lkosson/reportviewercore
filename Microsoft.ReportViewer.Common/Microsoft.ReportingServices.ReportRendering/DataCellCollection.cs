using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class DataCellCollection
	{
		private CustomReportItem m_owner;

		private int m_columnsCount;

		private int m_rowsCount;

		private DataCell m_firstCell;

		private DataRowCells m_firstColumnCells;

		private DataRowCells m_firstRowCells;

		private DataRowCells[] m_cells;

		public DataCell this[int row, int column]
		{
			get
			{
				if (row < 0 || row >= m_rowsCount)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, row, 0, m_rowsCount);
				}
				if (column < 0 || column >= m_columnsCount)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, column, 0, m_columnsCount);
				}
				DataCell dataCell = null;
				if (row == 0 && column == 0)
				{
					dataCell = m_firstCell;
				}
				else if (row == 0)
				{
					if (m_firstRowCells != null)
					{
						dataCell = m_firstRowCells[column - 1];
					}
				}
				else if (column == 0)
				{
					if (m_firstColumnCells != null)
					{
						dataCell = m_firstColumnCells[row - 1];
					}
				}
				else if (m_cells != null && m_cells[row - 1] != null)
				{
					dataCell = m_cells[row - 1][column - 1];
				}
				if (dataCell == null)
				{
					dataCell = new DataCell(m_owner, row, column);
					if (m_owner.UseCache)
					{
						if (row == 0 && column == 0)
						{
							m_firstCell = dataCell;
						}
						else if (row == 0)
						{
							if (m_firstRowCells == null)
							{
								m_firstRowCells = new DataRowCells(m_columnsCount - 1);
							}
							m_firstRowCells[column - 1] = dataCell;
						}
						else if (column == 0)
						{
							if (m_firstColumnCells == null)
							{
								m_firstColumnCells = new DataRowCells(m_rowsCount - 1);
							}
							m_firstColumnCells[row - 1] = dataCell;
						}
						else
						{
							if (m_cells == null)
							{
								m_cells = new DataRowCells[m_rowsCount - 1];
							}
							if (m_cells[row - 1] == null)
							{
								m_cells[row - 1] = new DataRowCells(m_columnsCount - 1);
							}
							m_cells[row - 1][column - 1] = dataCell;
						}
					}
				}
				return dataCell;
			}
		}

		public int Count => m_rowsCount * m_columnsCount;

		public int RowCount => m_rowsCount;

		public int ColumnCount => m_columnsCount;

		internal DataCellCollection(CustomReportItem owner, int rowsCount, int columnsCount)
		{
			m_owner = owner;
			m_rowsCount = rowsCount;
			m_columnsCount = columnsCount;
		}
	}
}
