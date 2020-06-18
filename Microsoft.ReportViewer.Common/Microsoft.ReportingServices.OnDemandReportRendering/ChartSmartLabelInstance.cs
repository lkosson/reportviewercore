namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartSmartLabelInstance : BaseInstance
	{
		private ChartSmartLabel m_chartSmartLabelDef;

		private ChartAllowOutsideChartArea? m_allowOutSidePlotArea;

		private ReportColor m_calloutBackColor;

		private ChartCalloutLineAnchor? m_calloutLineAnchor;

		private ReportColor m_calloutLineColor;

		private ChartCalloutLineStyle? m_calloutLineStyle;

		private ReportSize m_calloutLineWidth;

		private ChartCalloutStyle? m_calloutStyle;

		private bool? m_showOverlapped;

		private bool? m_markerOverlapping;

		private ReportSize m_maxMovingDistance;

		private ReportSize m_minMovingDistance;

		private bool? m_disabled;

		public ChartAllowOutsideChartArea AllowOutSidePlotArea
		{
			get
			{
				if (!m_allowOutSidePlotArea.HasValue && !m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					m_allowOutSidePlotArea = m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateAllowOutSidePlotArea(ReportScopeInstance, m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_allowOutSidePlotArea.Value;
			}
		}

		public ReportColor CalloutBackColor
		{
			get
			{
				if (m_calloutBackColor == null && !m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					m_calloutBackColor = new ReportColor(m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateCalloutBackColor(ReportScopeInstance, m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext), allowTransparency: true);
				}
				return m_calloutBackColor;
			}
		}

		public ChartCalloutLineAnchor CalloutLineAnchor
		{
			get
			{
				if (!m_calloutLineAnchor.HasValue && !m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					m_calloutLineAnchor = m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateCalloutLineAnchor(ReportScopeInstance, m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_calloutLineAnchor.Value;
			}
		}

		public ReportColor CalloutLineColor
		{
			get
			{
				if (m_calloutLineColor == null && !m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					m_calloutLineColor = new ReportColor(m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateCalloutLineColor(ReportScopeInstance, m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext), allowTransparency: true);
				}
				return m_calloutLineColor;
			}
		}

		public ChartCalloutLineStyle CalloutLineStyle
		{
			get
			{
				if (!m_calloutLineStyle.HasValue && !m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					m_calloutLineStyle = m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateCalloutLineStyle(ReportScopeInstance, m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_calloutLineStyle.Value;
			}
		}

		public ReportSize CalloutLineWidth
		{
			get
			{
				if (m_calloutLineWidth == null && !m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					m_calloutLineWidth = new ReportSize(m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateCalloutLineWidth(ReportScopeInstance, m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext));
				}
				return m_calloutLineWidth;
			}
		}

		public ChartCalloutStyle CalloutStyle
		{
			get
			{
				if (!m_calloutStyle.HasValue && !m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					m_calloutStyle = m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateCalloutStyle(ReportScopeInstance, m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_calloutStyle.Value;
			}
		}

		public bool ShowOverlapped
		{
			get
			{
				if (!m_showOverlapped.HasValue && !m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					m_showOverlapped = m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateShowOverlapped(ReportScopeInstance, m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_showOverlapped.Value;
			}
		}

		public bool MarkerOverlapping
		{
			get
			{
				if (!m_markerOverlapping.HasValue && !m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					m_markerOverlapping = m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateMarkerOverlapping(ReportScopeInstance, m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_markerOverlapping.Value;
			}
		}

		public bool Disabled
		{
			get
			{
				if (!m_disabled.HasValue && !m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					m_disabled = m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateDisabled(ReportScopeInstance, m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return m_disabled.Value;
			}
		}

		public ReportSize MaxMovingDistance
		{
			get
			{
				if (m_maxMovingDistance == null && !m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					m_maxMovingDistance = new ReportSize(m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateMaxMovingDistance(ReportScopeInstance, m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext));
				}
				return m_maxMovingDistance;
			}
		}

		public ReportSize MinMovingDistance
		{
			get
			{
				if (m_minMovingDistance == null && !m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					m_minMovingDistance = new ReportSize(m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateMinMovingDistance(ReportScopeInstance, m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext));
				}
				return m_minMovingDistance;
			}
		}

		internal ChartSmartLabelInstance(ChartSmartLabel chartSmartLabelDef)
			: base(chartSmartLabelDef.ReportScope)
		{
			m_chartSmartLabelDef = chartSmartLabelDef;
		}

		protected override void ResetInstanceCache()
		{
			m_allowOutSidePlotArea = null;
			m_calloutBackColor = null;
			m_calloutLineAnchor = null;
			m_calloutLineColor = null;
			m_calloutLineStyle = null;
			m_calloutLineWidth = null;
			m_calloutStyle = null;
			m_showOverlapped = null;
			m_markerOverlapping = null;
			m_maxMovingDistance = null;
			m_minMovingDistance = null;
			m_disabled = null;
		}
	}
}
