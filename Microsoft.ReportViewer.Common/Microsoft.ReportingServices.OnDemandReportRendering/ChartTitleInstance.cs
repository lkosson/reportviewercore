namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartTitleInstance : BaseInstance
	{
		private ChartTitle m_chartTitleDef;

		private StyleInstance m_style;

		private bool m_captionEvaluated;

		private string m_caption;

		private bool? m_hidden;

		private int? m_dockOffset;

		private bool? m_dockOutsideChartArea;

		private string m_toolTip;

		private ChartTitlePositions? m_position;

		private TextOrientations? m_textOrientation;

		public string Caption
		{
			get
			{
				if (!m_captionEvaluated)
				{
					m_captionEvaluated = true;
					if (m_chartTitleDef.ChartDef.IsOldSnapshot)
					{
						m_caption = m_chartTitleDef.RenderChartTitleInstance.Caption;
					}
					else
					{
						m_caption = m_chartTitleDef.ChartTitleDef.EvaluateCaption(ReportScopeInstance, m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_caption;
			}
		}

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_chartTitleDef, m_chartTitleDef.ChartDef, m_chartTitleDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public ChartTitlePositions Position
		{
			get
			{
				if (!m_position.HasValue && !m_chartTitleDef.ChartDef.IsOldSnapshot)
				{
					m_position = m_chartTitleDef.ChartTitleDef.EvaluatePosition(ReportScopeInstance, m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_position.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue && !m_chartTitleDef.ChartDef.IsOldSnapshot)
				{
					m_hidden = m_chartTitleDef.ChartTitleDef.EvaluateHidden(ReportScopeInstance, m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		public int DockOffset
		{
			get
			{
				if (!m_dockOffset.HasValue && !m_chartTitleDef.ChartDef.IsOldSnapshot)
				{
					m_dockOffset = m_chartTitleDef.ChartTitleDef.EvaluateDockOffset(ReportScopeInstance, m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_dockOffset.Value;
			}
		}

		public bool DockOutsideChartArea
		{
			get
			{
				if (!m_dockOutsideChartArea.HasValue && !m_chartTitleDef.ChartDef.IsOldSnapshot)
				{
					m_dockOutsideChartArea = m_chartTitleDef.ChartTitleDef.EvaluateDockOutsideChartArea(ReportScopeInstance, m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_dockOutsideChartArea.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null && !m_chartTitleDef.ChartDef.IsOldSnapshot)
				{
					m_toolTip = m_chartTitleDef.ChartTitleDef.EvaluateToolTip(ReportScopeInstance, m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_toolTip;
			}
		}

		public TextOrientations TextOrientation
		{
			get
			{
				if (!m_textOrientation.HasValue)
				{
					m_textOrientation = m_chartTitleDef.ChartTitleDef.EvaluateTextOrientation(ReportScopeInstance, m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_textOrientation.Value;
			}
		}

		internal ChartTitleInstance(ChartTitle chartTitleDef)
			: base(chartTitleDef.ChartDef)
		{
			m_chartTitleDef = chartTitleDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_captionEvaluated = false;
			m_hidden = null;
			m_dockOffset = null;
			m_dockOutsideChartArea = null;
			m_toolTip = null;
			m_position = null;
			m_textOrientation = null;
		}
	}
}
