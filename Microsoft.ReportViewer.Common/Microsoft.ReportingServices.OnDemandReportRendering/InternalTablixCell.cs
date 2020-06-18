using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixCell : TablixCell
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.TablixCell m_cellDef;

		public override string ID => m_cellDef.RenderingModelID;

		public override string DataElementName => m_cellDef.DataElementName;

		public override DataElementOutputTypes DataElementOutput => m_cellDef.DataElementOutput;

		public override CellContents CellContents
		{
			get
			{
				if (m_cellContents == null)
				{
					m_cellContents = new CellContents(this, this, m_cellDef.CellContents, m_cellDef.RowSpan, m_cellDef.ColSpan, m_owner.RenderingContext);
				}
				return m_cellContents;
			}
		}

		internal InternalTablixCell(Tablix owner, int rowIndex, int colIndex, Microsoft.ReportingServices.ReportIntermediateFormat.TablixCell cellDef)
			: base(cellDef, owner, rowIndex, colIndex)
		{
			m_cellDef = cellDef;
		}
	}
}
