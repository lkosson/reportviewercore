using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartNoMoveDirections
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections m_chartNoMoveDirectionsDef;

		private ChartNoMoveDirectionsInstance m_instance;

		private ReportBoolProperty m_up;

		private ReportBoolProperty m_down;

		private ReportBoolProperty m_left;

		private ReportBoolProperty m_right;

		private ReportBoolProperty m_upLeft;

		private ReportBoolProperty m_upRight;

		private ReportBoolProperty m_downLeft;

		private ReportBoolProperty m_downRight;

		private InternalChartSeries m_chartSeries;

		public ReportBoolProperty Up
		{
			get
			{
				if (m_up == null && !m_chart.IsOldSnapshot && m_chartNoMoveDirectionsDef.Up != null)
				{
					m_up = new ReportBoolProperty(m_chartNoMoveDirectionsDef.Up);
				}
				return m_up;
			}
		}

		public ReportBoolProperty Down
		{
			get
			{
				if (m_down == null && !m_chart.IsOldSnapshot && m_chartNoMoveDirectionsDef.Down != null)
				{
					m_down = new ReportBoolProperty(m_chartNoMoveDirectionsDef.Down);
				}
				return m_down;
			}
		}

		public ReportBoolProperty Left
		{
			get
			{
				if (m_left == null && !m_chart.IsOldSnapshot && m_chartNoMoveDirectionsDef.Left != null)
				{
					m_left = new ReportBoolProperty(m_chartNoMoveDirectionsDef.Left);
				}
				return m_left;
			}
		}

		public ReportBoolProperty Right
		{
			get
			{
				if (m_right == null && !m_chart.IsOldSnapshot && m_chartNoMoveDirectionsDef.Right != null)
				{
					m_right = new ReportBoolProperty(m_chartNoMoveDirectionsDef.Right);
				}
				return m_right;
			}
		}

		public ReportBoolProperty UpLeft
		{
			get
			{
				if (m_upLeft == null && !m_chart.IsOldSnapshot && m_chartNoMoveDirectionsDef.UpLeft != null)
				{
					m_upLeft = new ReportBoolProperty(m_chartNoMoveDirectionsDef.UpLeft);
				}
				return m_upLeft;
			}
		}

		public ReportBoolProperty UpRight
		{
			get
			{
				if (m_upRight == null && !m_chart.IsOldSnapshot && m_chartNoMoveDirectionsDef.UpRight != null)
				{
					m_upRight = new ReportBoolProperty(m_chartNoMoveDirectionsDef.UpRight);
				}
				return m_upRight;
			}
		}

		public ReportBoolProperty DownLeft
		{
			get
			{
				if (m_downLeft == null && !m_chart.IsOldSnapshot && m_chartNoMoveDirectionsDef.DownLeft != null)
				{
					m_downLeft = new ReportBoolProperty(m_chartNoMoveDirectionsDef.DownLeft);
				}
				return m_downLeft;
			}
		}

		public ReportBoolProperty DownRight
		{
			get
			{
				if (m_downRight == null && !m_chart.IsOldSnapshot && m_chartNoMoveDirectionsDef.DownRight != null)
				{
					m_downRight = new ReportBoolProperty(m_chartNoMoveDirectionsDef.DownRight);
				}
				return m_downRight;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				if (m_chartSeries != null)
				{
					return m_chartSeries.ReportScope;
				}
				return m_chart;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections ChartNoMoveDirectionsDef => m_chartNoMoveDirectionsDef;

		public ChartNoMoveDirectionsInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartNoMoveDirectionsInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartNoMoveDirections(InternalChartSeries chartSeries, Microsoft.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirectionsDef, Chart chart)
		{
			m_chartSeries = chartSeries;
			m_chartNoMoveDirectionsDef = chartNoMoveDirectionsDef;
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
