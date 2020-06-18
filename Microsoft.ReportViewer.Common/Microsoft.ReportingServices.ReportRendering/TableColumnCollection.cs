using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class TableColumnCollection
	{
		private Table m_owner;

		private TableColumnList m_columnDefs;

		private TableColumn[] m_columns;

		public TableColumn this[int index]
		{
			get
			{
				if (index < 0 || index >= m_columnDefs.Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				TableColumn tableColumn = null;
				if (m_columns == null || m_columns[index] == null)
				{
					tableColumn = new TableColumn(m_owner, m_columnDefs[index], index);
					if (m_owner.RenderingContext.CacheState)
					{
						if (m_columns == null)
						{
							m_columns = new TableColumn[m_columnDefs.Count];
						}
						m_columns[index] = tableColumn;
					}
				}
				else
				{
					tableColumn = m_columns[index];
				}
				return tableColumn;
			}
		}

		public int Count => m_columnDefs.Count;

		internal TableColumnCollection(Table owner, TableColumnList columnDefs)
		{
			m_owner = owner;
			m_columnDefs = columnDefs;
		}
	}
}
