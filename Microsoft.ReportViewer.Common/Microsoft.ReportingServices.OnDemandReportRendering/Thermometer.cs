using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Thermometer : IROMStyleDefinitionContainer
	{
		private GaugePanel m_gaugePanel;

		private Microsoft.ReportingServices.ReportIntermediateFormat.Thermometer m_defObject;

		private ThermometerInstance m_instance;

		private Style m_style;

		private ReportDoubleProperty m_bulbOffset;

		private ReportDoubleProperty m_bulbSize;

		private ReportEnumProperty<GaugeThermometerStyles> m_thermometerStyle;

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

		public ReportDoubleProperty BulbOffset
		{
			get
			{
				if (m_bulbOffset == null && m_defObject.BulbOffset != null)
				{
					m_bulbOffset = new ReportDoubleProperty(m_defObject.BulbOffset);
				}
				return m_bulbOffset;
			}
		}

		public ReportDoubleProperty BulbSize
		{
			get
			{
				if (m_bulbSize == null && m_defObject.BulbSize != null)
				{
					m_bulbSize = new ReportDoubleProperty(m_defObject.BulbSize);
				}
				return m_bulbSize;
			}
		}

		public ReportEnumProperty<GaugeThermometerStyles> ThermometerStyle
		{
			get
			{
				if (m_thermometerStyle == null && m_defObject.ThermometerStyle != null)
				{
					m_thermometerStyle = new ReportEnumProperty<GaugeThermometerStyles>(m_defObject.ThermometerStyle.IsExpression, m_defObject.ThermometerStyle.OriginalText, EnumTranslator.TranslateGaugeThermometerStyles(m_defObject.ThermometerStyle.StringValue, null));
				}
				return m_thermometerStyle;
			}
		}

		internal GaugePanel GaugePanelDef => m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Thermometer ThermometerDef => m_defObject;

		public ThermometerInstance Instance
		{
			get
			{
				if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ThermometerInstance(this);
				}
				return m_instance;
			}
		}

		internal Thermometer(Microsoft.ReportingServices.ReportIntermediateFormat.Thermometer defObject, GaugePanel gaugePanel)
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
		}
	}
}
