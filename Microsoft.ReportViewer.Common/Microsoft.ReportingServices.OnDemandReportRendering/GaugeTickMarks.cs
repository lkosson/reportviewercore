using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeTickMarks : TickMarkStyle
	{
		private ReportDoubleProperty m_interval;

		private ReportDoubleProperty m_intervalOffset;

		public ReportDoubleProperty Interval
		{
			get
			{
				if (m_interval == null && GaugeTickMarksDef.Interval != null)
				{
					m_interval = new ReportDoubleProperty(GaugeTickMarksDef.Interval);
				}
				return m_interval;
			}
		}

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (m_intervalOffset == null && GaugeTickMarksDef.IntervalOffset != null)
				{
					m_intervalOffset = new ReportDoubleProperty(GaugeTickMarksDef.IntervalOffset);
				}
				return m_intervalOffset;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugeTickMarks GaugeTickMarksDef => (Microsoft.ReportingServices.ReportIntermediateFormat.GaugeTickMarks)m_defObject;

		public new GaugeTickMarksInstance Instance
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
				return (GaugeTickMarksInstance)m_instance;
			}
		}

		internal GaugeTickMarks(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeTickMarks defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}

		protected override TickMarkStyleInstance GetInstance()
		{
			return new GaugeTickMarksInstance(this);
		}
	}
}
