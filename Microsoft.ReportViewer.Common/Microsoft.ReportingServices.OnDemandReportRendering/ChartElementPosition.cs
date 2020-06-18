using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartElementPosition
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartElementPosition m_defObject;

		private ChartElementPositionInstance m_instance;

		private ReportDoubleProperty m_top;

		private ReportDoubleProperty m_left;

		private ReportDoubleProperty m_height;

		private ReportDoubleProperty m_width;

		public ReportDoubleProperty Top
		{
			get
			{
				if (m_top == null && m_defObject.Top != null)
				{
					m_top = new ReportDoubleProperty(m_defObject.Top);
				}
				return m_top;
			}
		}

		public ReportDoubleProperty Left
		{
			get
			{
				if (m_left == null && m_defObject.Left != null)
				{
					m_left = new ReportDoubleProperty(m_defObject.Left);
				}
				return m_left;
			}
		}

		public ReportDoubleProperty Height
		{
			get
			{
				if (m_height == null && m_defObject.Height != null)
				{
					m_height = new ReportDoubleProperty(m_defObject.Height);
				}
				return m_height;
			}
		}

		public ReportDoubleProperty Width
		{
			get
			{
				if (m_width == null && m_defObject.Width != null)
				{
					m_width = new ReportDoubleProperty(m_defObject.Width);
				}
				return m_width;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartElementPosition ChartElementPositionDef => m_defObject;

		public ChartElementPositionInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartElementPositionInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartElementPosition(Microsoft.ReportingServices.ReportIntermediateFormat.ChartElementPosition defObject, Chart chart)
		{
			m_defObject = defObject;
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
