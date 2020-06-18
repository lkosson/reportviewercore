using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTableCell : ShimCell
	{
		private int m_colSpan;

		private TableCell m_renderCellContents;

		private Microsoft.ReportingServices.ReportRendering.ReportItem m_renderReportItem;

		public override CellContents CellContents
		{
			get
			{
				if (m_cellContents == null)
				{
					m_cellContents = new CellContents(this, m_inSubtotal, m_renderReportItem, 1, m_colSpan, m_owner.RenderingContext);
				}
				return m_cellContents;
			}
		}

		internal ShimTableCell(Tablix owner, int rowIndex, int colIndex, int colSpan, Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
			: base(owner, rowIndex, colIndex, owner.InSubtotal)
		{
			m_colSpan = colSpan;
			m_renderReportItem = renderReportItem;
		}

		internal void SetCellContents(TableCell renderCellContents)
		{
			if (renderCellContents != null)
			{
				m_renderCellContents = renderCellContents;
				if (renderCellContents.ReportItem != null)
				{
					m_renderReportItem = renderCellContents.ReportItem;
				}
			}
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_cellContents != null)
			{
				m_cellContents.UpdateRenderReportItem(renderCellContents?.ReportItem);
			}
		}
	}
}
