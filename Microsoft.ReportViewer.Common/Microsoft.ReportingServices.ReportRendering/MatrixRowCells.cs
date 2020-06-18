using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class MatrixRowCells
	{
		private int m_count;

		private MatrixCell[] m_matrixRowCells;

		internal MatrixCell this[int index]
		{
			get
			{
				if (index < 0 || index >= m_count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, m_count);
				}
				if (m_matrixRowCells != null)
				{
					return m_matrixRowCells[index];
				}
				return null;
			}
			set
			{
				if (index < 0 || index >= m_count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, m_count);
				}
				if (m_matrixRowCells == null)
				{
					m_matrixRowCells = new MatrixCell[m_count];
				}
				m_matrixRowCells[index] = value;
			}
		}

		internal MatrixRowCells(int count)
		{
			m_count = count;
		}
	}
}
