namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScaleRangeInstance : BaseInstance
	{
		private ScaleRange m_defObject;

		private StyleInstance m_style;

		private double? m_distanceFromScale;

		private double? m_startWidth;

		private double? m_endWidth;

		private ReportColor m_inRangeBarPointerColor;

		private ReportColor m_inRangeLabelColor;

		private ReportColor m_inRangeTickMarksColor;

		private BackgroundGradientTypes? m_backgroundGradientType;

		private ScaleRangePlacements? m_placement;

		private string m_toolTip;

		private bool? m_hidden;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_defObject, m_defObject.GaugePanelDef, m_defObject.GaugePanelDef.RenderingContext);
				}
				return m_style;
			}
		}

		public double DistanceFromScale
		{
			get
			{
				if (!m_distanceFromScale.HasValue)
				{
					m_distanceFromScale = m_defObject.ScaleRangeDef.EvaluateDistanceFromScale(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_distanceFromScale.Value;
			}
		}

		public double StartWidth
		{
			get
			{
				if (!m_startWidth.HasValue)
				{
					m_startWidth = m_defObject.ScaleRangeDef.EvaluateStartWidth(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_startWidth.Value;
			}
		}

		public double EndWidth
		{
			get
			{
				if (!m_endWidth.HasValue)
				{
					m_endWidth = m_defObject.ScaleRangeDef.EvaluateEndWidth(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_endWidth.Value;
			}
		}

		public ReportColor InRangeBarPointerColor
		{
			get
			{
				if (m_inRangeBarPointerColor == null)
				{
					m_inRangeBarPointerColor = new ReportColor(m_defObject.ScaleRangeDef.EvaluateInRangeBarPointerColor(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext), allowTransparency: true);
				}
				return m_inRangeBarPointerColor;
			}
		}

		public ReportColor InRangeLabelColor
		{
			get
			{
				if (m_inRangeLabelColor == null)
				{
					m_inRangeLabelColor = new ReportColor(m_defObject.ScaleRangeDef.EvaluateInRangeLabelColor(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext), allowTransparency: true);
				}
				return m_inRangeLabelColor;
			}
		}

		public ReportColor InRangeTickMarksColor
		{
			get
			{
				if (m_inRangeTickMarksColor == null)
				{
					m_inRangeTickMarksColor = new ReportColor(m_defObject.ScaleRangeDef.EvaluateInRangeTickMarksColor(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return m_inRangeTickMarksColor;
			}
		}

		public BackgroundGradientTypes BackgroundGradientType
		{
			get
			{
				if (!m_backgroundGradientType.HasValue)
				{
					m_backgroundGradientType = m_defObject.ScaleRangeDef.EvaluateBackgroundGradientType(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_backgroundGradientType.Value;
			}
		}

		public ScaleRangePlacements Placement
		{
			get
			{
				if (!m_placement.HasValue)
				{
					m_placement = m_defObject.ScaleRangeDef.EvaluatePlacement(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_placement.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					m_toolTip = m_defObject.ScaleRangeDef.EvaluateToolTip(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_toolTip;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue)
				{
					m_hidden = m_defObject.ScaleRangeDef.EvaluateHidden(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		internal ScaleRangeInstance(ScaleRange defObject)
			: base(defObject.GaugePanelDef)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_distanceFromScale = null;
			m_startWidth = null;
			m_endWidth = null;
			m_inRangeBarPointerColor = null;
			m_inRangeLabelColor = null;
			m_inRangeTickMarksColor = null;
			m_placement = null;
			m_toolTip = null;
			m_hidden = null;
			m_backgroundGradientType = null;
		}
	}
}
