namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class TickMarkStyleInstance : BaseInstance
	{
		protected TickMarkStyle m_defObject;

		private StyleInstance m_style;

		private double? m_distanceFromScale;

		private GaugeLabelPlacements? m_placement;

		private bool? m_enableGradient;

		private double? m_gradientDensity;

		private double? m_length;

		private double? m_width;

		private GaugeTickMarkShapes? m_shape;

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
					m_distanceFromScale = m_defObject.TickMarkStyleDef.EvaluateDistanceFromScale(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_distanceFromScale.Value;
			}
		}

		public GaugeLabelPlacements Placement
		{
			get
			{
				if (!m_placement.HasValue)
				{
					m_placement = m_defObject.TickMarkStyleDef.EvaluatePlacement(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_placement.Value;
			}
		}

		public bool EnableGradient
		{
			get
			{
				if (!m_enableGradient.HasValue)
				{
					m_enableGradient = m_defObject.TickMarkStyleDef.EvaluateEnableGradient(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_enableGradient.Value;
			}
		}

		public double GradientDensity
		{
			get
			{
				if (!m_gradientDensity.HasValue)
				{
					m_gradientDensity = m_defObject.TickMarkStyleDef.EvaluateGradientDensity(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_gradientDensity.Value;
			}
		}

		public double Length
		{
			get
			{
				if (!m_length.HasValue)
				{
					m_length = m_defObject.TickMarkStyleDef.EvaluateLength(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_length.Value;
			}
		}

		public double Width
		{
			get
			{
				if (!m_width.HasValue)
				{
					m_width = m_defObject.TickMarkStyleDef.EvaluateWidth(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_width.Value;
			}
		}

		public GaugeTickMarkShapes Shape
		{
			get
			{
				if (!m_shape.HasValue)
				{
					m_shape = m_defObject.TickMarkStyleDef.EvaluateShape(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_shape.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!m_hidden.HasValue)
				{
					m_hidden = m_defObject.TickMarkStyleDef.EvaluateHidden(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_hidden.Value;
			}
		}

		internal TickMarkStyleInstance(TickMarkStyle defObject)
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
			m_placement = null;
			m_enableGradient = null;
			m_gradientDensity = null;
			m_length = null;
			m_width = null;
			m_shape = null;
			m_hidden = null;
		}
	}
}
