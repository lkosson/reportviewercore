using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMatrixCell : ShimCell
	{
		private Microsoft.ReportingServices.ReportRendering.ReportItem m_renderReportItem;

		private ShimMatrixMember m_rowParentMember;

		private ShimMatrixMember m_colParentMember;

		public override CellContents CellContents
		{
			get
			{
				if (m_cellContents == null)
				{
					m_cellContents = new CellContents(this, m_inSubtotal, CachedRenderReportItem, 1, 1, m_owner.RenderingContext);
				}
				else if (m_renderReportItem == null)
				{
					m_cellContents.UpdateRenderReportItem(CachedRenderReportItem);
				}
				return m_cellContents;
			}
		}

		private Microsoft.ReportingServices.ReportRendering.ReportItem CachedRenderReportItem
		{
			get
			{
				if (m_renderReportItem == null)
				{
					int cachedMemberCellIndex = m_rowParentMember.CurrentRenderMatrixMember.CachedMemberCellIndex;
					int cellIndex = m_colParentMember.CurrentMatrixMemberCellIndexes.GetCellIndex(m_colParentMember);
					MatrixCell matrixCell = m_owner.RenderMatrix.CellCollection[cachedMemberCellIndex, cellIndex];
					if (matrixCell != null)
					{
						m_renderReportItem = matrixCell.ReportItem;
					}
				}
				return m_renderReportItem;
			}
		}

		internal ShimMatrixCell(Tablix owner, int rowIndex, int colIndex, ShimMatrixMember rowParentMember, ShimMatrixMember colParentMember, bool inSubtotal)
			: base(owner, rowIndex, colIndex, inSubtotal)
		{
			m_rowParentMember = rowParentMember;
			m_colParentMember = colParentMember;
		}

		internal void ResetCellContents()
		{
			m_renderReportItem = null;
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
