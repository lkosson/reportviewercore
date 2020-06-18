using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialGauge : Gauge
	{
		private RadialScaleCollection m_gaugeScales;

		private ReportDoubleProperty m_pivotX;

		private ReportDoubleProperty m_pivotY;

		public RadialScaleCollection GaugeScales
		{
			get
			{
				if (m_gaugeScales == null && RadialGaugeDef.GaugeScales != null)
				{
					m_gaugeScales = new RadialScaleCollection(this, m_gaugePanel);
				}
				return m_gaugeScales;
			}
		}

		public ReportDoubleProperty PivotX
		{
			get
			{
				if (m_pivotX == null && RadialGaugeDef.PivotX != null)
				{
					m_pivotX = new ReportDoubleProperty(RadialGaugeDef.PivotX);
				}
				return m_pivotX;
			}
		}

		public ReportDoubleProperty PivotY
		{
			get
			{
				if (m_pivotY == null && RadialGaugeDef.PivotY != null)
				{
					m_pivotY = new ReportDoubleProperty(RadialGaugeDef.PivotY);
				}
				return m_pivotY;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge RadialGaugeDef => (Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge)m_defObject;

		public new RadialGaugeInstance Instance => (RadialGaugeInstance)GetInstance();

		internal RadialGauge(Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge defObject, GaugePanel gaugePanel)
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
				m_instance = new RadialGaugeInstance(this);
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
