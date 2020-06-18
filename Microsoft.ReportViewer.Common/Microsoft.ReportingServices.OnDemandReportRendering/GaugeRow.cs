using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeRow : IDataRegionRow
	{
		private GaugePanel m_gaugePanel;

		private GaugeCell m_cell;

		private Microsoft.ReportingServices.ReportIntermediateFormat.GaugeRow m_rowDef;

		public GaugeCell GaugeCell
		{
			get
			{
				if (m_cell == null && m_rowDef.GaugeCell != null)
				{
					m_cell = new GaugeCell(m_gaugePanel, m_rowDef.GaugeCell);
				}
				return m_cell;
			}
		}

		int IDataRegionRow.Count => 1;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugeRow GaugeRowDef => m_rowDef;

		internal GaugePanel GaugePanelDef => m_gaugePanel;

		internal GaugeRow(GaugePanel gaugePanel)
		{
			m_gaugePanel = gaugePanel;
		}

		internal GaugeRow(GaugePanel gaugePanel, Microsoft.ReportingServices.ReportIntermediateFormat.GaugeRow rowDef)
		{
			m_gaugePanel = gaugePanel;
			m_rowDef = rowDef;
		}

		IDataRegionCell IDataRegionRow.GetIfExists(int columnIndex)
		{
			if (columnIndex == 0)
			{
				return GaugeCell;
			}
			return null;
		}

		internal void SetNewContext()
		{
			if (m_cell != null)
			{
				m_cell.SetNewContext();
			}
		}
	}
}
