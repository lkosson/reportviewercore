using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimListRow : TablixRow
	{
		private ReportSize m_height;

		private ShimListCell m_cell;

		public override TablixCell this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				return m_cell;
			}
		}

		public override int Count => 1;

		public override ReportSize Height
		{
			get
			{
				if (m_height == null)
				{
					m_height = new ReportSize(m_owner.RenderList.Height);
				}
				return m_height;
			}
		}

		internal ShimListRow(Tablix owner)
			: base(owner, 0)
		{
			m_cell = new ShimListCell(owner);
		}

		internal void UpdateCells(ListContent renderContents)
		{
			m_cell.SetCellContents(renderContents);
		}
	}
}
