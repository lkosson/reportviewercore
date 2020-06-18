namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxisTitleInstance : BaseInstance
	{
		private ChartAxisTitle m_chartAxisTitleDef;

		private StyleInstance m_style;

		private bool m_captionEvaluated;

		private string m_caption;

		private ChartAxisTitlePositions? m_position;

		private TextOrientations? m_textOrientation;

		public string Caption
		{
			get
			{
				if (!m_captionEvaluated)
				{
					m_captionEvaluated = true;
					if (m_chartAxisTitleDef.ChartDef.IsOldSnapshot)
					{
						m_caption = m_chartAxisTitleDef.RenderChartTitleInstance.Caption;
					}
					else
					{
						m_caption = m_chartAxisTitleDef.ChartAxisTitleDef.EvaluateCaption(ReportScopeInstance, m_chartAxisTitleDef.ChartDef.RenderingContext.OdpContext);
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
					m_style = new StyleInstance(m_chartAxisTitleDef, m_chartAxisTitleDef.ChartDef, m_chartAxisTitleDef.ChartDef.RenderingContext);
				}
				return m_style;
			}
		}

		public ChartAxisTitlePositions Position
		{
			get
			{
				if (!m_position.HasValue && !m_chartAxisTitleDef.ChartDef.IsOldSnapshot)
				{
					m_position = m_chartAxisTitleDef.ChartAxisTitleDef.EvaluatePosition(ReportScopeInstance, m_chartAxisTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_position.Value;
			}
		}

		public TextOrientations TextOrientation
		{
			get
			{
				if (!m_textOrientation.HasValue)
				{
					m_textOrientation = m_chartAxisTitleDef.ChartAxisTitleDef.EvaluateTextOrientation(ReportScopeInstance, m_chartAxisTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_textOrientation.Value;
			}
		}

		internal ChartAxisTitleInstance(ChartAxisTitle chartAxisTitleDef)
			: base(chartAxisTitleDef.ChartDef)
		{
			m_chartAxisTitleDef = chartAxisTitleDef;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_captionEvaluated = false;
			m_position = null;
			m_textOrientation = null;
		}
	}
}
