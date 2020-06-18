namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class PinLabelInstance : BaseInstance
	{
		private PinLabel m_defObject;

		private StyleInstance m_style;

		private string m_text;

		private bool? m_allowUpsideDown;

		private double? m_distanceFromScale;

		private double? m_fontAngle;

		private GaugeLabelPlacements? m_placement;

		private bool? m_rotateLabel;

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

		public string Text
		{
			get
			{
				if (m_text == null)
				{
					m_text = m_defObject.PinLabelDef.EvaluateText(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_text;
			}
		}

		public bool AllowUpsideDown
		{
			get
			{
				if (!m_allowUpsideDown.HasValue)
				{
					m_allowUpsideDown = m_defObject.PinLabelDef.EvaluateAllowUpsideDown(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					m_distanceFromScale = m_defObject.PinLabelDef.EvaluateDistanceFromScale(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					m_fontAngle = m_defObject.PinLabelDef.EvaluateFontAngle(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					m_placement = m_defObject.PinLabelDef.EvaluatePlacement(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_placement.Value;
			}
		}

		public bool RotateLabel
		{
			get
			{
				if (!m_rotateLabel.HasValue)
				{
					m_rotateLabel = m_defObject.PinLabelDef.EvaluateRotateLabel(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_rotateLabel.Value;
			}
		}

		public bool UseFontPercent
		{
			get
			{
				if (!m_useFontPercent.HasValue)
				{
					m_useFontPercent = m_defObject.PinLabelDef.EvaluateUseFontPercent(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_useFontPercent.Value;
			}
		}

		internal PinLabelInstance(PinLabel defObject)
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
			m_text = null;
			m_allowUpsideDown = null;
			m_distanceFromScale = null;
			m_fontAngle = null;
			m_placement = null;
			m_rotateLabel = null;
			m_useFontPercent = null;
		}
	}
}
