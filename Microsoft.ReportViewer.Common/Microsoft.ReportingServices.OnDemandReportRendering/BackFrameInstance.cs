namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class BackFrameInstance : BaseInstance
	{
		private BackFrame m_defObject;

		private StyleInstance m_style;

		private GaugeFrameStyles? m_frameStyle;

		private GaugeFrameShapes? m_frameShape;

		private double? m_frameWidth;

		private GaugeGlassEffects? m_glassEffect;

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

		public GaugeFrameStyles FrameStyle
		{
			get
			{
				if (!m_frameStyle.HasValue)
				{
					m_frameStyle = m_defObject.BackFrameDef.EvaluateFrameStyle(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_frameStyle.Value;
			}
		}

		public GaugeFrameShapes FrameShape
		{
			get
			{
				if (!m_frameShape.HasValue)
				{
					m_frameShape = m_defObject.BackFrameDef.EvaluateFrameShape(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_frameShape.Value;
			}
		}

		public double FrameWidth
		{
			get
			{
				if (!m_frameWidth.HasValue)
				{
					m_frameWidth = m_defObject.BackFrameDef.EvaluateFrameWidth(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_frameWidth.Value;
			}
		}

		public GaugeGlassEffects GlassEffect
		{
			get
			{
				if (!m_glassEffect.HasValue)
				{
					m_glassEffect = m_defObject.BackFrameDef.EvaluateGlassEffect(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_glassEffect.Value;
			}
		}

		internal BackFrameInstance(BackFrame defObject)
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
			m_frameStyle = null;
			m_frameShape = null;
			m_glassEffect = null;
			m_frameWidth = null;
		}
	}
}
