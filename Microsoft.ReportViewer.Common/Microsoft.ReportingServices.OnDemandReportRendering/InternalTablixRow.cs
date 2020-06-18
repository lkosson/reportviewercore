using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixRow : TablixRow
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.TablixRow m_rowDef;

		private ReportSize m_height;

		private TablixCell[] m_cellROMDefs;

		public override TablixCell this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				Microsoft.ReportingServices.ReportIntermediateFormat.TablixCell tablixCell = m_rowDef.TablixCells[index];
				if (tablixCell.ColSpan > 0 && m_cellROMDefs[index] == null)
				{
					m_cellROMDefs[index] = new InternalTablixCell(m_owner, m_rowIndex, index, tablixCell);
				}
				return m_cellROMDefs[index];
			}
		}

		public override int Count => m_rowDef.Cells.Count;

		public override ReportSize Height
		{
			get
			{
				if (m_height == null)
				{
					m_height = new ReportSize(m_rowDef.Height, m_rowDef.HeightValue);
				}
				return m_height;
			}
		}

		internal InternalTablixRow(Tablix owner, int rowIndex, Microsoft.ReportingServices.ReportIntermediateFormat.TablixRow rowDef)
			: base(owner, rowIndex)
		{
			m_rowDef = rowDef;
			m_cellROMDefs = new TablixCell[rowDef.Cells.Count];
		}

		internal override IDataRegionCell GetIfExists(int index)
		{
			if (m_cellROMDefs != null && index >= 0 && index < Count)
			{
				return m_cellROMDefs[index];
			}
			return null;
		}
	}
}
