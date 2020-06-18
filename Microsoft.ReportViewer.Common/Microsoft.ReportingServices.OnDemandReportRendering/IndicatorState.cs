using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class IndicatorState : GaugePanelObjectCollectionItem
	{
		private GaugePanel m_gaugePanel;

		private Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState m_defObject;

		private GaugeInputValue m_startValue;

		private GaugeInputValue m_endValue;

		private ReportColorProperty m_color;

		private ReportDoubleProperty m_scaleFactor;

		private ReportEnumProperty<GaugeStateIndicatorStyles> m_indicatorStyle;

		private IndicatorImage m_indicatorImage;

		public string Name => m_defObject.Name;

		public GaugeInputValue StartValue
		{
			get
			{
				if (m_startValue == null && m_defObject.StartValue != null)
				{
					m_startValue = new GaugeInputValue(m_defObject.StartValue, m_gaugePanel);
				}
				return m_startValue;
			}
		}

		public GaugeInputValue EndValue
		{
			get
			{
				if (m_endValue == null && m_defObject.EndValue != null)
				{
					m_endValue = new GaugeInputValue(m_defObject.EndValue, m_gaugePanel);
				}
				return m_endValue;
			}
		}

		public ReportColorProperty Color
		{
			get
			{
				if (m_color == null && m_defObject.Color != null)
				{
					ExpressionInfo color = m_defObject.Color;
					if (color != null)
					{
						m_color = new ReportColorProperty(color.IsExpression, m_defObject.Color.OriginalText, color.IsExpression ? null : new ReportColor(color.StringValue.Trim(), allowTransparency: true), color.IsExpression ? new ReportColor("", System.Drawing.Color.Empty, parsed: true) : null);
					}
				}
				return m_color;
			}
		}

		public ReportDoubleProperty ScaleFactor
		{
			get
			{
				if (m_scaleFactor == null && m_defObject.ScaleFactor != null)
				{
					m_scaleFactor = new ReportDoubleProperty(m_defObject.ScaleFactor);
				}
				return m_scaleFactor;
			}
		}

		public ReportEnumProperty<GaugeStateIndicatorStyles> IndicatorStyle
		{
			get
			{
				if (m_indicatorStyle == null && m_defObject.IndicatorStyle != null)
				{
					m_indicatorStyle = new ReportEnumProperty<GaugeStateIndicatorStyles>(m_defObject.IndicatorStyle.IsExpression, m_defObject.IndicatorStyle.OriginalText, EnumTranslator.TranslateGaugeStateIndicatorStyles(m_defObject.IndicatorStyle.StringValue, null));
				}
				return m_indicatorStyle;
			}
		}

		public IndicatorImage IndicatorImage
		{
			get
			{
				if (m_indicatorImage == null && m_defObject.IndicatorImage != null)
				{
					m_indicatorImage = new IndicatorImage(m_defObject.IndicatorImage, m_gaugePanel);
				}
				return m_indicatorImage;
			}
		}

		internal GaugePanel GaugePanelDef => m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState IndicatorStateDef => m_defObject;

		public IndicatorStateInstance Instance
		{
			get
			{
				if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new IndicatorStateInstance(this);
				}
				return (IndicatorStateInstance)m_instance;
			}
		}

		internal IndicatorState(Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState defObject, GaugePanel gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_startValue != null)
			{
				m_startValue.SetNewContext();
			}
			if (m_endValue != null)
			{
				m_endValue.SetNewContext();
			}
			if (m_indicatorImage != null)
			{
				m_indicatorImage.SetNewContext();
			}
		}
	}
}
