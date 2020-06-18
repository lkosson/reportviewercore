using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixCornerCell : IDefinitionPath
	{
		private Tablix m_owner;

		private int m_rowIndex;

		private int m_columnIndex;

		private string m_definitionPath;

		private Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell m_cellDef;

		private Microsoft.ReportingServices.ReportRendering.ReportItem m_cornerReportItem;

		private CellContents m_cellContents;

		public string DefinitionPath
		{
			get
			{
				if (m_definitionPath == null)
				{
					m_definitionPath = DefinitionPathConstants.GetTablixCellDefinitionPath(m_owner, m_rowIndex, m_columnIndex, isTablixBodyCell: false);
				}
				return m_definitionPath;
			}
		}

		public IDefinitionPath ParentDefinitionPath => m_owner;

		public CellContents CellContents
		{
			get
			{
				if (m_cellContents == null)
				{
					if (m_owner.IsOldSnapshot)
					{
						if (m_cornerReportItem != null)
						{
							int columns = m_owner.Columns;
							int rows = m_owner.Rows;
							m_cellContents = new CellContents(this, m_owner.InSubtotal, m_cornerReportItem, columns, rows, m_owner.RenderingContext);
						}
					}
					else
					{
						m_cellContents = new CellContents(m_owner.ReportScope, this, m_cellDef.CellContents, m_cellDef.RowSpan, m_cellDef.ColSpan, m_owner.RenderingContext);
					}
				}
				return m_cellContents;
			}
		}

		internal TablixCornerCell(Tablix owner, int rowIndex, int colIndex, Microsoft.ReportingServices.ReportIntermediateFormat.TablixCornerCell cellDef)
		{
			m_owner = owner;
			m_rowIndex = rowIndex;
			m_columnIndex = colIndex;
			m_cellDef = cellDef;
		}

		internal TablixCornerCell(Tablix owner, int rowIndex, int colIndex, Microsoft.ReportingServices.ReportRendering.ReportItem cornerReportItem)
		{
			m_owner = owner;
			m_rowIndex = rowIndex;
			m_columnIndex = colIndex;
			m_cornerReportItem = cornerReportItem;
		}
	}
}
