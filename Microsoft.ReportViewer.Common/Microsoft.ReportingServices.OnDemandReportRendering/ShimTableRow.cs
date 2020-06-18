using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTableRow : TablixRow
	{
		private List<ShimTableCell> m_cells;

		private ReportSize m_height;

		private int[] m_rowCellDefinitionMapping;

		public override TablixCell this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_rowCellDefinitionMapping[index] < 0)
				{
					return null;
				}
				return m_cells[m_rowCellDefinitionMapping[index]];
			}
		}

		public override int Count => m_rowCellDefinitionMapping.Length;

		public override ReportSize Height => m_height;

		internal ShimTableRow(Tablix owner, int rowIndex, Microsoft.ReportingServices.ReportRendering.TableRow renderRow)
			: base(owner, rowIndex)
		{
			m_cells = new List<ShimTableCell>();
			m_height = new ReportSize(renderRow.Height);
			TableCellCollection tableCellCollection = renderRow.TableCellCollection;
			if (tableCellCollection == null)
			{
				return;
			}
			int count = tableCellCollection.Count;
			m_rowCellDefinitionMapping = new int[owner.RenderTable.Columns.Count];
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				int colSpan = tableCellCollection[i].ColSpan;
				for (int j = 0; j < colSpan; j++)
				{
					m_rowCellDefinitionMapping[num] = ((j == 0) ? i : (-1));
					num++;
				}
				m_cells.Add(new ShimTableCell(owner, rowIndex, i, colSpan, tableCellCollection[i].ReportItem));
			}
		}

		internal void UpdateCells(Microsoft.ReportingServices.ReportRendering.TableRow renderRow)
		{
			int count = m_cells.Count;
			TableCellCollection tableCellCollection = renderRow?.TableCellCollection;
			for (int i = 0; i < count; i++)
			{
				m_cells[i].SetCellContents(tableCellCollection?[i]);
			}
		}
	}
}
