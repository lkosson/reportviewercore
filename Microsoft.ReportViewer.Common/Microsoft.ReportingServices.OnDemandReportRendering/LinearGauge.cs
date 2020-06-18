using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearGauge : Gauge
	{
		private LinearScaleCollection m_gaugeScales;

		private ReportEnumProperty<GaugeOrientations> m_orientation;

		public LinearScaleCollection GaugeScales
		{
			get
			{
				if (m_gaugeScales == null && LinearGaugeDef.GaugeScales != null)
				{
					m_gaugeScales = new LinearScaleCollection(this, m_gaugePanel);
				}
				return m_gaugeScales;
			}
		}

		public ReportEnumProperty<GaugeOrientations> Orientation
		{
			get
			{
				if (m_orientation == null && LinearGaugeDef.Orientation != null)
				{
					m_orientation = new ReportEnumProperty<GaugeOrientations>(LinearGaugeDef.Orientation.IsExpression, LinearGaugeDef.Orientation.OriginalText, EnumTranslator.TranslateGaugeOrientations(LinearGaugeDef.Orientation.StringValue, null));
				}
				return m_orientation;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.LinearGauge LinearGaugeDef => (Microsoft.ReportingServices.ReportIntermediateFormat.LinearGauge)m_defObject;

		public new LinearGaugeInstance Instance => (LinearGaugeInstance)GetInstance();

		internal LinearGauge(Microsoft.ReportingServices.ReportIntermediateFormat.LinearGauge defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal override BaseInstance GetInstance()
		{
			if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new LinearGaugeInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_gaugeScales != null)
			{
				m_gaugeScales.SetNewContext();
			}
		}
	}
}
