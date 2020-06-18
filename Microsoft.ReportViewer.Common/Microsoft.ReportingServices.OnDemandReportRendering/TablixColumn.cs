using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixColumn
	{
		private Tablix m_owner;

		private int m_columnIndex;

		private ReportSize m_width;

		public ReportSize Width
		{
			get
			{
				if (m_width == null)
				{
					if (m_owner.IsOldSnapshot)
					{
						switch (m_owner.SnapshotTablixType)
						{
						case DataRegion.Type.List:
							m_width = new ReportSize(m_owner.RenderList.Width);
							break;
						case DataRegion.Type.Table:
							m_width = new ReportSize(m_owner.RenderTable.Columns[m_columnIndex].Width);
							break;
						case DataRegion.Type.Matrix:
						{
							int index = m_owner.MatrixColDefinitionMapping[m_columnIndex];
							m_width = new ReportSize(m_owner.RenderMatrix.CellWidths[index]);
							break;
						}
						}
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.TablixColumn tablixColumn = m_owner.TablixDef.TablixColumns[m_columnIndex];
						m_width = new ReportSize(tablixColumn.Width, tablixColumn.WidthValue);
					}
				}
				return m_width;
			}
		}

		internal TablixColumn(Tablix owner, int columnIndex)
		{
			m_owner = owner;
			m_columnIndex = columnIndex;
		}
	}
}
