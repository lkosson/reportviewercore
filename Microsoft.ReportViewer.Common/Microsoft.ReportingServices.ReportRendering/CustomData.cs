namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class CustomData
	{
		private CustomReportItem m_owner;

		private DataCellCollection m_datacells;

		private DataGroupingCollection m_columns;

		private DataGroupingCollection m_rows;

		public bool NoRows
		{
			get
			{
				if (m_owner.CriInstance == null || m_owner.CriInstance.Cells == null || m_owner.CriInstance.Cells.Count == 0 || m_owner.CriInstance.Cells[0].Count == 0)
				{
					return true;
				}
				return false;
			}
		}

		public DataCellCollection DataCells
		{
			get
			{
				DataCellCollection dataCellCollection = m_datacells;
				if (m_datacells == null)
				{
					if (!NoRows)
					{
						dataCellCollection = new DataCellCollection(m_owner, m_owner.CriInstance.CellRowCount, m_owner.CriInstance.CellColumnCount);
					}
					if (m_owner.UseCache)
					{
						m_datacells = dataCellCollection;
					}
				}
				return dataCellCollection;
			}
		}

		public DataGroupingCollection DataColumnGroupings
		{
			get
			{
				DataGroupingCollection dataGroupingCollection = m_columns;
				if (m_columns == null && m_owner.CriDefinition.Columns != null)
				{
					dataGroupingCollection = new DataGroupingCollection(m_owner, null, m_owner.CriDefinition.Columns, (m_owner.CriInstance == null) ? null : m_owner.CriInstance.ColumnInstances);
					if (m_owner.UseCache)
					{
						m_columns = dataGroupingCollection;
					}
				}
				return dataGroupingCollection;
			}
		}

		public DataGroupingCollection DataRowGroupings
		{
			get
			{
				DataGroupingCollection dataGroupingCollection = m_rows;
				if (m_rows == null && m_owner.CriDefinition.Rows != null)
				{
					dataGroupingCollection = new DataGroupingCollection(m_owner, null, m_owner.CriDefinition.Rows, (m_owner.CriInstance == null) ? null : m_owner.CriInstance.RowInstances);
					if (m_owner.UseCache)
					{
						m_rows = dataGroupingCollection;
					}
				}
				return dataGroupingCollection;
			}
		}

		internal CustomData(CustomReportItem owner)
		{
			m_owner = owner;
		}
	}
}
