namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAlignTypeInstance : BaseInstance
	{
		private ChartAlignType m_chartAlignTypeDef;

		private bool? m_axesView;

		private bool? m_cursor;

		private bool? m_position;

		private bool? m_innerPlotPosition;

		public bool AxesView
		{
			get
			{
				if (!m_axesView.HasValue && !m_chartAlignTypeDef.ChartDef.IsOldSnapshot)
				{
					m_axesView = m_chartAlignTypeDef.ChartAlignTypeDef.EvaluateAxesView(ReportScopeInstance, m_chartAlignTypeDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_axesView.Value;
			}
		}

		public bool Cursor
		{
			get
			{
				if (!m_cursor.HasValue && !m_chartAlignTypeDef.ChartDef.IsOldSnapshot)
				{
					m_cursor = m_chartAlignTypeDef.ChartAlignTypeDef.EvaluateCursor(ReportScopeInstance, m_chartAlignTypeDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_cursor.Value;
			}
		}

		public bool Position
		{
			get
			{
				if (!m_position.HasValue && !m_chartAlignTypeDef.ChartDef.IsOldSnapshot)
				{
					m_position = m_chartAlignTypeDef.ChartAlignTypeDef.EvaluatePosition(ReportScopeInstance, m_chartAlignTypeDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_position.Value;
			}
		}

		public bool InnerPlotPosition
		{
			get
			{
				if (!m_innerPlotPosition.HasValue && !m_chartAlignTypeDef.ChartDef.IsOldSnapshot)
				{
					m_innerPlotPosition = m_chartAlignTypeDef.ChartAlignTypeDef.EvaluateInnerPlotPosition(ReportScopeInstance, m_chartAlignTypeDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_innerPlotPosition.Value;
			}
		}

		internal ChartAlignTypeInstance(ChartAlignType chartAlignTypeDef)
			: base(chartAlignTypeDef.ChartDef)
		{
			m_chartAlignTypeDef = chartAlignTypeDef;
		}

		protected override void ResetInstanceCache()
		{
			m_axesView = null;
			m_cursor = null;
			m_position = null;
			m_innerPlotPosition = null;
		}
	}
}
