using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class TableCellCollection
	{
		private Table m_table;

		private TableCell[] m_cells;

		private ReportItemCollection m_cellReportItems;

		private Microsoft.ReportingServices.ReportProcessing.TableRow m_rowDef;

		private TableRowInstance m_rowInstance;

		public TableCell this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				TableCell tableCell = null;
				if (m_cells == null || m_cells[index] == null)
				{
					tableCell = new TableCell((Microsoft.ReportingServices.ReportProcessing.Table)m_table.ReportItemDef, index, this);
					if (m_table.RenderingContext.CacheState)
					{
						if (m_cells == null)
						{
							m_cells = new TableCell[Count];
						}
						m_cells[index] = tableCell;
					}
				}
				else
				{
					tableCell = m_cells[index];
				}
				return tableCell;
			}
		}

		public int Count => m_rowDef.ReportItems.Count;

		internal ReportItemCollection ReportItems
		{
			get
			{
				if (m_cellReportItems == null)
				{
					m_cellReportItems = new ReportItemCollection(m_rowDef.ReportItems, (m_rowInstance == null) ? null : m_rowInstance.TableRowReportItemColInstance, m_table.RenderingContext, null);
				}
				return m_cellReportItems;
			}
		}

		internal IntList ColSpans => m_rowDef.ColSpans;

		internal Microsoft.ReportingServices.ReportProcessing.TableRow RowDef => m_rowDef;

		internal RenderingContext RenderingContext => m_table.RenderingContext;

		internal TableCellCollection(Table table, Microsoft.ReportingServices.ReportProcessing.TableRow rowDef, TableRowInstance rowInstance)
		{
			m_table = table;
			m_rowDef = rowDef;
			m_rowInstance = rowInstance;
		}
	}
}
