using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimListCell : ShimCell
	{
		private ListContent m_currentListContents;

		private Rectangle m_shimContainer;

		public override CellContents CellContents
		{
			get
			{
				if (m_cellContents == null)
				{
					m_shimContainer = new Rectangle(this, m_inSubtotal, m_currentListContents, m_owner.RenderingContext);
					m_cellContents = new CellContents(m_shimContainer, 1, 1, m_owner.RenderingContext);
				}
				return m_cellContents;
			}
		}

		internal ShimListCell(Tablix owner)
			: base(owner, 0, 0, owner.InSubtotal)
		{
			m_currentListContents = owner.RenderList.Contents[0];
		}

		internal void SetCellContents(ListContent renderContents)
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			m_currentListContents = renderContents;
			if (m_shimContainer != null)
			{
				m_cellContents.SetNewContext();
				m_shimContainer.UpdateListContents(m_currentListContents);
			}
		}
	}
}
