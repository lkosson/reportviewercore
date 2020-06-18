using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class TickMarkStyle : IROMStyleDefinitionContainer
	{
		internal GaugePanel m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle m_defObject;

		protected TickMarkStyleInstance m_instance;

		private Style m_style;

		private ReportDoubleProperty m_distanceFromScale;

		private ReportEnumProperty<GaugeLabelPlacements> m_placement;

		private ReportBoolProperty m_enableGradient;

		private ReportDoubleProperty m_gradientDensity;

		private TopImage m_tickMarkImage;

		private ReportDoubleProperty m_length;

		private ReportDoubleProperty m_width;

		private ReportEnumProperty<GaugeTickMarkShapes> m_shape;

		private ReportBoolProperty m_hidden;

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new Style(m_gaugePanel, m_gaugePanel, m_defObject, m_gaugePanel.RenderingContext);
				}
				return m_style;
			}
		}

		public ReportDoubleProperty DistanceFromScale
		{
			get
			{
				if (m_distanceFromScale == null && m_defObject.DistanceFromScale != null)
				{
					m_distanceFromScale = new ReportDoubleProperty(m_defObject.DistanceFromScale);
				}
				return m_distanceFromScale;
			}
		}

		public ReportEnumProperty<GaugeLabelPlacements> Placement
		{
			get
			{
				if (m_placement == null && m_defObject.Placement != null)
				{
					m_placement = new ReportEnumProperty<GaugeLabelPlacements>(m_defObject.Placement.IsExpression, m_defObject.Placement.OriginalText, EnumTranslator.TranslateGaugeLabelPlacements(m_defObject.Placement.StringValue, null));
				}
				return m_placement;
			}
		}

		public ReportBoolProperty EnableGradient
		{
			get
			{
				if (m_enableGradient == null && m_defObject.EnableGradient != null)
				{
					m_enableGradient = new ReportBoolProperty(m_defObject.EnableGradient);
				}
				return m_enableGradient;
			}
		}

		public ReportDoubleProperty GradientDensity
		{
			get
			{
				if (m_gradientDensity == null && m_defObject.GradientDensity != null)
				{
					m_gradientDensity = new ReportDoubleProperty(m_defObject.GradientDensity);
				}
				return m_gradientDensity;
			}
		}

		public TopImage TickMarkImage
		{
			get
			{
				if (m_tickMarkImage == null && m_defObject.TickMarkImage != null)
				{
					m_tickMarkImage = new TopImage(m_defObject.TickMarkImage, m_gaugePanel);
				}
				return m_tickMarkImage;
			}
		}

		public ReportDoubleProperty Length
		{
			get
			{
				if (m_length == null && m_defObject.Length != null)
				{
					m_length = new ReportDoubleProperty(m_defObject.Length);
				}
				return m_length;
			}
		}

		public ReportDoubleProperty Width
		{
			get
			{
				if (m_width == null && m_defObject.Width != null)
				{
					m_width = new ReportDoubleProperty(m_defObject.Width);
				}
				return m_width;
			}
		}

		public ReportEnumProperty<GaugeTickMarkShapes> Shape
		{
			get
			{
				if (m_shape == null && m_defObject.Shape != null)
				{
					m_shape = new ReportEnumProperty<GaugeTickMarkShapes>(m_defObject.Shape.IsExpression, m_defObject.Shape.OriginalText, EnumTranslator.TranslateGaugeTickMarkShapes(m_defObject.Shape.StringValue, null));
				}
				return m_shape;
			}
		}

		public ReportBoolProperty Hidden
		{
			get
			{
				if (m_hidden == null && m_defObject.Hidden != null)
				{
					m_hidden = new ReportBoolProperty(m_defObject.Hidden);
				}
				return m_hidden;
			}
		}

		internal GaugePanel GaugePanelDef => m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle TickMarkStyleDef => m_defObject;

		public TickMarkStyleInstance Instance
		{
			get
			{
				if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = GetInstance();
				}
				return m_instance;
			}
		}

		internal TickMarkStyle(Microsoft.ReportingServices.ReportIntermediateFormat.TickMarkStyle defObject, GaugePanel gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal virtual void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			if (m_tickMarkImage != null)
			{
				m_tickMarkImage.SetNewContext();
			}
		}

		protected virtual TickMarkStyleInstance GetInstance()
		{
			return new TickMarkStyleInstance(this);
		}
	}
}
