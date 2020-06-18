using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDataPointInstance : BaseInstance, IReportScopeInstance
	{
		private ChartDataPoint m_chartDataPointDef;

		private StyleInstance m_style;

		private object m_axisLabel;

		private bool m_isNewContext = true;

		private string m_toolTip;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartDataPointDef, m_chartDataPointDef, m_chartDataPointDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public object AxisLabel
		{
			get
			{
				if (m_axisLabel == null && !m_chartDataPointDef.ChartDef.IsOldSnapshot)
				{
					m_axisLabel = m_chartDataPointDef.DataPointDef.EvaluateAxisLabel(ReportScopeInstance, m_chartDataPointDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return m_axisLabel;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					m_toolTip = m_chartDataPointDef.DataPointDef.EvaluateToolTip(ReportScopeInstance, m_chartDataPointDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_toolTip;
			}
		}

		string IReportScopeInstance.UniqueName
		{
			get
			{
				if (m_chartDataPointDef.ChartDef.IsOldSnapshot)
				{
					return m_chartDataPointDef.ChartDef.ID + "i" + m_chartDataPointDef.RenderItem.InstanceInfo.DataPointIndex.ToString(CultureInfo.InvariantCulture);
				}
				return m_chartDataPointDef.DataPointDef.UniqueName;
			}
		}

		bool IReportScopeInstance.IsNewContext
		{
			get
			{
				return m_isNewContext;
			}
			set
			{
				m_isNewContext = value;
			}
		}

		IReportScope IReportScopeInstance.ReportScope => m_reportScope;

		internal ChartDataPointInstance(ChartDataPoint chartDataPointDef)
			: base(chartDataPointDef)
		{
			m_chartDataPointDef = chartDataPointDef;
		}

		internal override void SetNewContext()
		{
			if (!m_isNewContext)
			{
				m_isNewContext = true;
				base.SetNewContext();
			}
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_axisLabel = null;
			m_toolTip = null;
		}
	}
}
