using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearScale : GaugeScale
	{
		private LinearPointerCollection m_gaugePointers;

		private ReportDoubleProperty m_startMargin;

		private ReportDoubleProperty m_endMargin;

		private ReportDoubleProperty m_position;

		public LinearPointerCollection GaugePointers
		{
			get
			{
				if (m_gaugePointers == null && LinearScaleDef.GaugePointers != null)
				{
					m_gaugePointers = new LinearPointerCollection(this, m_gaugePanel);
				}
				return m_gaugePointers;
			}
		}

		public ReportDoubleProperty StartMargin
		{
			get
			{
				if (m_startMargin == null && LinearScaleDef.StartMargin != null)
				{
					m_startMargin = new ReportDoubleProperty(LinearScaleDef.StartMargin);
				}
				return m_startMargin;
			}
		}

		public ReportDoubleProperty EndMargin
		{
			get
			{
				if (m_endMargin == null && LinearScaleDef.EndMargin != null)
				{
					m_endMargin = new ReportDoubleProperty(LinearScaleDef.EndMargin);
				}
				return m_endMargin;
			}
		}

		public ReportDoubleProperty Position
		{
			get
			{
				if (m_position == null && LinearScaleDef.Position != null)
				{
					m_position = new ReportDoubleProperty(LinearScaleDef.Position);
				}
				return m_position;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale LinearScaleDef => (Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale)m_defObject;

		public new LinearScaleInstance Instance => (LinearScaleInstance)GetInstance();

		internal LinearScale(Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale defObject, GaugePanel gaugePanel)
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
				m_instance = new LinearScaleInstance(this);
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
