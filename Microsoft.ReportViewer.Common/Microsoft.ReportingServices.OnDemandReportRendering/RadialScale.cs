using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialScale : GaugeScale
	{
		private RadialPointerCollection m_gaugePointers;

		private ReportDoubleProperty m_radius;

		private ReportDoubleProperty m_startAngle;

		private ReportDoubleProperty m_sweepAngle;

		public RadialPointerCollection GaugePointers
		{
			get
			{
				if (m_gaugePointers == null && RadialScaleDef.GaugePointers != null)
				{
					m_gaugePointers = new RadialPointerCollection(this, m_gaugePanel);
				}
				return m_gaugePointers;
			}
		}

		public ReportDoubleProperty Radius
		{
			get
			{
				if (m_radius == null && RadialScaleDef.Radius != null)
				{
					m_radius = new ReportDoubleProperty(RadialScaleDef.Radius);
				}
				return m_radius;
			}
		}

		public ReportDoubleProperty StartAngle
		{
			get
			{
				if (m_startAngle == null && RadialScaleDef.StartAngle != null)
				{
					m_startAngle = new ReportDoubleProperty(RadialScaleDef.StartAngle);
				}
				return m_startAngle;
			}
		}

		public ReportDoubleProperty SweepAngle
		{
			get
			{
				if (m_sweepAngle == null && RadialScaleDef.SweepAngle != null)
				{
					m_sweepAngle = new ReportDoubleProperty(RadialScaleDef.SweepAngle);
				}
				return m_sweepAngle;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale RadialScaleDef => (Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale)m_defObject;

		public new RadialScaleInstance Instance => (RadialScaleInstance)GetInstance();

		internal RadialScale(Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal override GaugeScaleInstance GetInstance()
		{
			if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new RadialScaleInstance(this);
			}
			return (GaugeScaleInstance)m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_gaugePointers != null)
			{
				m_gaugePointers.SetNewContext();
			}
		}
	}
}
