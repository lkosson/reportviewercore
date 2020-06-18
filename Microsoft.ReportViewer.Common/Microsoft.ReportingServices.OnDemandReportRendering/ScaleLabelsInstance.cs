namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScaleLabelsInstance : BaseInstance
	{
		private ScaleLabels m_defObject;

		private StyleInstance m_style;

		private double? m_interval;

		private double? m_intervalOffset;

		private bool? m_allowUpsideDown;

		private double? m_distanceFromScale;

		private double? m_fontAngle;

		private GaugeLabelPlacements? m_placement;

		private bool? m_rotateLabels;

		private bool? m_showEndLabels;

		private bool? m_hidden;

		private bool? m_useFontPercent;

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

		public double Interval
		{
			get
			{
				if (!m_interval.HasValue)
				{
					m_interval = m_defObject.ScaleLabelsDef.EvaluateInterval(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_interval.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!m_intervalOffset.HasValue)
				{
					m_intervalOffset = m_defObject.ScaleLabelsDef.EvaluateIntervalOffset(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_intervalOffset.Value;
			}
		}

		public bool AllowUpsideDown
		{
			get
			{
				if (!m_allowUpsideDown.HasValue)
				{
					m_allowUpsideDown = m_defObject.ScaleLabelsDef.EvaluateAllowUpsideDown(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_allowUpsideDown.Value;
			}
		}

		public double DistanceFromScale
		{
			get
			{
				if (!m_distanceFromScale.HasValue)
				{
					m_distanceFromScale = m_defObject.ScaleLabelsDef.EvaluateDistanceFromScale(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_distanceFromScale.Value;
			}
		}

		public double FontAngle
		{
			get
			{
				if (!m_fontAngle.HasValue)
				{
					m_fontAngle = m_defObject.ScaleLabelsDef.EvaluateFontAngle(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_fontAngle.Value;
			}
		}

		public GaugeLabelPlacements Placement
		{
			get
			{
				if (!m_placement.HasValue)
				{
					m_placement = m_defObject.ScaleLabelsDef.EvaluatePlacement(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_placement.Value;
			}
		}

		public bool RotateLabels
		{
			get
			{
				if (!m_rotateLabels.HasValue)
				{
					m_rotateLabels = m_defObject.ScaleLabelsDef.EvaluateRotateLabels(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_rotateLabels.Value;
			}
		}

		public bool ShowEndLabels
		{
			get
			{
				if (!m_showEndLabels.HasValue)
				{
					m_showEndLabels = m_defObject.ScaleLabelsDef.EvaluateShowEndLabels(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_showEndLabels.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue)
				{
					m_hidden = m_defObject.ScaleLabelsDef.EvaluateHidden(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		public bool UseFontPercent
		{
			get
			{
				if (!m_useFontPercent.HasValue)
				{
					m_useFontPercent = m_defObject.ScaleLabelsDef.EvaluateUseFontPercent(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_useFontPercent.Value;
			}
		}

		internal ScaleLabelsInstance(ScaleLabels defObject)
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
			m_interval = null;
			m_intervalOffset = null;
			m_allowUpsideDown = null;
			m_distanceFromScale = null;
			m_fontAngle = null;
			m_placement = null;
			m_rotateLabels = null;
			m_showEndLabels = null;
			m_hidden = null;
			m_useFontPercent = null;
		}
	}
}
