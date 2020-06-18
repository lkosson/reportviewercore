using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAlignType
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartAlignType m_chartAlignTypeDef;

		private ChartAlignTypeInstance m_instance;

		private ReportBoolProperty m_axesView;

		private ReportBoolProperty m_cursor;

		private ReportBoolProperty m_position;

		private ReportBoolProperty m_innerPlotPosition;

		public ReportBoolProperty AxesView
		{
			get
			{
				if (m_axesView == null && !m_chart.IsOldSnapshot && m_chartAlignTypeDef.AxesView != null)
				{
					m_axesView = new ReportBoolProperty(m_chartAlignTypeDef.AxesView);
				}
				return m_axesView;
			}
		}

		public ReportBoolProperty Cursor
		{
			get
			{
				if (m_cursor == null && !m_chart.IsOldSnapshot && m_chartAlignTypeDef.Cursor != null)
				{
					m_cursor = new ReportBoolProperty(m_chartAlignTypeDef.Cursor);
				}
				return m_cursor;
			}
		}

		public ReportBoolProperty Position
		{
			get
			{
				if (m_position == null && !m_chart.IsOldSnapshot && m_chartAlignTypeDef.Position != null)
				{
					m_position = new ReportBoolProperty(m_chartAlignTypeDef.Position);
				}
				return m_position;
			}
		}

		public ReportBoolProperty InnerPlotPosition
		{
			get
			{
				if (m_innerPlotPosition == null && !m_chart.IsOldSnapshot && m_chartAlignTypeDef.InnerPlotPosition != null)
				{
					m_innerPlotPosition = new ReportBoolProperty(m_chartAlignTypeDef.InnerPlotPosition);
				}
				return m_innerPlotPosition;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartAlignType ChartAlignTypeDef => m_chartAlignTypeDef;

		public ChartAlignTypeInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartAlignTypeInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartAlignType(Microsoft.ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignTypeDef, Chart chart)
		{
			m_chartAlignTypeDef = chartAlignTypeDef;
			m_chart = chart;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
