using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class PointerCap : IROMStyleDefinitionContainer
	{
		private GaugePanel m_gaugePanel;

		private Microsoft.ReportingServices.ReportIntermediateFormat.PointerCap m_defObject;

		private PointerCapInstance m_instance;

		private Style m_style;

		private CapImage m_capImage;

		private ReportBoolProperty m_onTop;

		private ReportBoolProperty m_reflection;

		private ReportEnumProperty<GaugeCapStyles> m_capStyle;

		private ReportBoolProperty m_hidden;

		private ReportDoubleProperty m_width;

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

		public CapImage CapImage
		{
			get
			{
				if (m_capImage == null && m_defObject.CapImage != null)
				{
					m_capImage = new CapImage(m_defObject.CapImage, m_gaugePanel);
				}
				return m_capImage;
			}
		}

		public ReportBoolProperty OnTop
		{
			get
			{
				if (m_onTop == null && m_defObject.OnTop != null)
				{
					m_onTop = new ReportBoolProperty(m_defObject.OnTop);
				}
				return m_onTop;
			}
		}

		public ReportBoolProperty Reflection
		{
			get
			{
				if (m_reflection == null && m_defObject.Reflection != null)
				{
					m_reflection = new ReportBoolProperty(m_defObject.Reflection);
				}
				return m_reflection;
			}
		}

		public ReportEnumProperty<GaugeCapStyles> CapStyle
		{
			get
			{
				if (m_capStyle == null && m_defObject.CapStyle != null)
				{
					m_capStyle = new ReportEnumProperty<GaugeCapStyles>(m_defObject.CapStyle.IsExpression, m_defObject.CapStyle.OriginalText, EnumTranslator.TranslateGaugeCapStyles(m_defObject.CapStyle.StringValue, null));
				}
				return m_capStyle;
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

		internal GaugePanel GaugePanelDef => m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.PointerCap PointerCapDef => m_defObject;

		public PointerCapInstance Instance
		{
			get
			{
				if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new PointerCapInstance(this);
				}
				return m_instance;
			}
		}

		internal PointerCap(Microsoft.ReportingServices.ReportIntermediateFormat.PointerCap defObject, GaugePanel gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			if (m_capImage != null)
			{
				m_capImage.SetNewContext();
			}
		}
	}
}
