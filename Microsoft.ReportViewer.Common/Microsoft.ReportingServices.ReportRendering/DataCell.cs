using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class DataCell
	{
		private CustomReportItem m_owner;

		private int m_rowIndex;

		private int m_columnIndex;

		private CustomReportItemCellInstance m_cellInstance;

		private DataValueCollection m_dataValueCollection;

		public DataValueCollection DataValues
		{
			get
			{
				if (m_cellInstance == null)
				{
					return null;
				}
				DataValueCollection dataValueCollection = m_dataValueCollection;
				if (m_dataValueCollection == null)
				{
					dataValueCollection = new DataValueCollection(GetCellDefinition(), m_cellInstance.DataValueInstances);
					if (m_owner.UseCache)
					{
						m_dataValueCollection = dataValueCollection;
					}
				}
				return dataValueCollection;
			}
		}

		internal int ColumnIndex
		{
			get
			{
				if (m_cellInstance == null)
				{
					return -1;
				}
				return m_cellInstance.ColumnIndex;
			}
		}

		internal int RowIndex
		{
			get
			{
				if (m_cellInstance == null)
				{
					return -1;
				}
				return m_cellInstance.RowIndex;
			}
		}

		internal DataCell(CustomReportItem owner, int rowIndex, int columnIndex)
		{
			m_owner = owner;
			m_rowIndex = rowIndex;
			m_columnIndex = columnIndex;
			if (!owner.CustomData.NoRows)
			{
				CustomReportItemCellInstancesList cells = m_owner.CriInstance.Cells;
				m_cellInstance = cells[rowIndex][columnIndex];
			}
		}

		private DataValueCRIList GetCellDefinition()
		{
			Global.Tracer.Assert(!m_owner.CustomData.NoRows && m_owner.CriDefinition.DataRowCells != null && m_cellInstance.RowIndex < m_owner.CriDefinition.DataRowCells.Count && m_cellInstance.ColumnIndex < m_owner.CriDefinition.DataRowCells[m_cellInstance.RowIndex].Count && 0 < m_owner.CriDefinition.DataRowCells[m_cellInstance.RowIndex][m_cellInstance.ColumnIndex].Count);
			return m_owner.CriDefinition.DataRowCells[m_cellInstance.RowIndex][m_cellInstance.ColumnIndex];
		}
	}
}
