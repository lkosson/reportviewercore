using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class MatrixCellCollection
	{
		private Matrix m_owner;

		private int m_columnsCount;

		private int m_rowsCount;

		private MatrixCell m_firstCell;

		private MatrixRowCells m_firstMatrixColumnCells;

		private MatrixRowCells m_firstMatrixRowCells;

		private MatrixRowCells[] m_cells;

		public MatrixCell this[int row, int column]
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
				MatrixCell matrixCell = null;
				if (row == 0 && column == 0)
				{
					matrixCell = m_firstCell;
				}
				else if (row == 0)
				{
					if (m_firstMatrixRowCells != null)
					{
						matrixCell = m_firstMatrixRowCells[column - 1];
					}
				}
				else if (column == 0)
				{
					if (m_firstMatrixColumnCells != null)
					{
						matrixCell = m_firstMatrixColumnCells[row - 1];
					}
				}
				else if (m_cells != null && m_cells[row - 1] != null)
				{
					matrixCell = m_cells[row - 1][column - 1];
				}
				if (matrixCell == null)
				{
					matrixCell = new MatrixCell(m_owner, row, column);
					if (m_owner.RenderingContext.CacheState)
					{
						if (row == 0 && column == 0)
						{
							m_firstCell = matrixCell;
						}
						else if (row == 0)
						{
							if (m_firstMatrixRowCells == null)
							{
								m_firstMatrixRowCells = new MatrixRowCells(m_columnsCount - 1);
							}
							m_firstMatrixRowCells[column - 1] = matrixCell;
						}
						else if (column == 0)
						{
							if (m_firstMatrixColumnCells == null)
							{
								m_firstMatrixColumnCells = new MatrixRowCells(m_rowsCount - 1);
							}
							m_firstMatrixColumnCells[row - 1] = matrixCell;
						}
						else
						{
							if (m_cells == null)
							{
								m_cells = new MatrixRowCells[m_rowsCount - 1];
							}
							if (m_cells[row - 1] == null)
							{
								m_cells[row - 1] = new MatrixRowCells(m_columnsCount - 1);
							}
							m_cells[row - 1][column - 1] = matrixCell;
						}
					}
				}
				return matrixCell;
			}
		}

		public int Count => m_rowsCount * m_columnsCount;

		public int RowCount => m_rowsCount;

		public int ColumnCount => m_columnsCount;

		internal MatrixCellCollection(Matrix owner, int rowsCount, int columnsCount)
		{
			m_owner = owner;
			m_rowsCount = rowsCount;
			m_columnsCount = columnsCount;
		}
	}
}
