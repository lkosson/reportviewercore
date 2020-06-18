using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class DataRowCells
	{
		private int m_count;

		private DataCell[] m_rowCells;

		internal DataCell this[int index]
		{
			get
			{
				if (index < 0 || index >= m_count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, m_count);
				}
				if (m_rowCells != null)
				{
					return m_rowCells[index];
				}
				return null;
			}
			set
			{
				if (index < 0 || index >= m_count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, m_count);
				}
				if (m_rowCells == null)
				{
					m_rowCells = new DataCell[m_count];
				}
				m_rowCells[index] = value;
			}
		}

		internal DataRowCells(int count)
		{
			m_count = count;
		}
	}
}
