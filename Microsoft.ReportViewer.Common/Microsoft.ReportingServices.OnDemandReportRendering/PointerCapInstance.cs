namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class PointerCapInstance : BaseInstance
	{
		private PointerCap m_defObject;

		private StyleInstance m_style;

		private bool? m_onTop;

		private bool? m_reflection;

		private GaugeCapStyles? m_capStyle;

		private bool? m_hidden;

		private double? m_width;

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

		public bool OnTop
		{
			get
			{
				if (!m_onTop.HasValue)
				{
					m_onTop = m_defObject.PointerCapDef.EvaluateOnTop(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_onTop.Value;
			}
		}

		public bool Reflection
		{
			get
			{
				if (!m_reflection.HasValue)
				{
					m_reflection = m_defObject.PointerCapDef.EvaluateReflection(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_reflection.Value;
			}
		}

		public GaugeCapStyles CapStyle
		{
			get
			{
				if (!m_capStyle.HasValue)
				{
					m_capStyle = m_defObject.PointerCapDef.EvaluateCapStyle(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_capStyle.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue)
				{
					m_hidden = m_defObject.PointerCapDef.EvaluateHidden(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		public double Width
		{
			get
			{
				if (!m_width.HasValue)
				{
					m_width = m_defObject.PointerCapDef.EvaluateWidth(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_width.Value;
			}
		}

		internal PointerCapInstance(PointerCap defObject)
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
			m_onTop = null;
			m_reflection = null;
			m_capStyle = null;
			m_hidden = null;
			m_width = null;
		}
	}
}
