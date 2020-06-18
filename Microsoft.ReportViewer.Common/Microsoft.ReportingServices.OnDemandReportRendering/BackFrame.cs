using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class BackFrame : IROMStyleDefinitionContainer
	{
		private GaugePanel m_gaugePanel;

		private Microsoft.ReportingServices.ReportIntermediateFormat.BackFrame m_defObject;

		private BackFrameInstance m_instance;

		private Style m_style;

		private ReportEnumProperty<GaugeFrameStyles> m_frameStyle;

		private ReportEnumProperty<GaugeFrameShapes> m_frameShape;

		private ReportDoubleProperty m_frameWidth;

		private ReportEnumProperty<GaugeGlassEffects> m_glassEffect;

		private FrameBackground m_frameBackground;

		private FrameImage m_frameImage;

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

		public ReportEnumProperty<GaugeFrameStyles> FrameStyle
		{
			get
			{
				if (m_frameStyle == null && m_defObject.FrameStyle != null)
				{
					m_frameStyle = new ReportEnumProperty<GaugeFrameStyles>(m_defObject.FrameStyle.IsExpression, m_defObject.FrameStyle.OriginalText, EnumTranslator.TranslateGaugeFrameStyles(m_defObject.FrameStyle.StringValue, null));
				}
				return m_frameStyle;
			}
		}

		public ReportEnumProperty<GaugeFrameShapes> FrameShape
		{
			get
			{
				if (m_frameShape == null && m_defObject.FrameShape != null)
				{
					m_frameShape = new ReportEnumProperty<GaugeFrameShapes>(m_defObject.FrameShape.IsExpression, m_defObject.FrameShape.OriginalText, EnumTranslator.TranslateGaugeFrameShapes(m_defObject.FrameShape.StringValue, null));
				}
				return m_frameShape;
			}
		}

		public ReportDoubleProperty FrameWidth
		{
			get
			{
				if (m_frameWidth == null && m_defObject.FrameWidth != null)
				{
					m_frameWidth = new ReportDoubleProperty(m_defObject.FrameWidth);
				}
				return m_frameWidth;
			}
		}

		public ReportEnumProperty<GaugeGlassEffects> GlassEffect
		{
			get
			{
				if (m_glassEffect == null && m_defObject.GlassEffect != null)
				{
					m_glassEffect = new ReportEnumProperty<GaugeGlassEffects>(m_defObject.GlassEffect.IsExpression, m_defObject.GlassEffect.OriginalText, EnumTranslator.TranslateGaugeGlassEffects(m_defObject.GlassEffect.StringValue, null));
				}
				return m_glassEffect;
			}
		}

		public FrameBackground FrameBackground
		{
			get
			{
				if (m_frameBackground == null && m_defObject.FrameBackground != null)
				{
					m_frameBackground = new FrameBackground(m_defObject.FrameBackground, m_gaugePanel);
				}
				return m_frameBackground;
			}
		}

		public FrameImage FrameImage
		{
			get
			{
				if (m_frameImage == null && m_defObject.FrameImage != null)
				{
					m_frameImage = new FrameImage(m_defObject.FrameImage, m_gaugePanel);
				}
				return m_frameImage;
			}
		}

		internal GaugePanel GaugePanelDef => m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.BackFrame BackFrameDef => m_defObject;

		public BackFrameInstance Instance
		{
			get
			{
				if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new BackFrameInstance(this);
				}
				return m_instance;
			}
		}

		internal BackFrame(Microsoft.ReportingServices.ReportIntermediateFormat.BackFrame defObject, GaugePanel gaugePanel)
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
			if (m_frameBackground != null)
			{
				m_frameBackground.SetNewContext();
			}
			if (m_frameImage != null)
			{
				m_frameImage.SetNewContext();
			}
		}
	}
}
