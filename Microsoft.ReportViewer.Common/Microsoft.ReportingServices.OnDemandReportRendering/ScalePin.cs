using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScalePin : TickMarkStyle
	{
		private ReportDoubleProperty m_location;

		private ReportBoolProperty m_enable;

		private PinLabel m_pinLabel;

		public ReportDoubleProperty Location
		{
			get
			{
				if (m_location == null && ScalePinDef.Location != null)
				{
					m_location = new ReportDoubleProperty(ScalePinDef.Location);
				}
				return m_location;
			}
		}

		public ReportBoolProperty Enable
		{
			get
			{
				if (m_enable == null && ScalePinDef.Enable != null)
				{
					m_enable = new ReportBoolProperty(ScalePinDef.Enable);
				}
				return m_enable;
			}
		}

		public PinLabel PinLabel
		{
			get
			{
				if (m_pinLabel == null && ScalePinDef.PinLabel != null)
				{
					m_pinLabel = new PinLabel(ScalePinDef.PinLabel, m_gaugePanel);
				}
				return m_pinLabel;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ScalePin ScalePinDef => (Microsoft.ReportingServices.ReportIntermediateFormat.ScalePin)m_defObject;

		public new ScalePinInstance Instance
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
				return (ScalePinInstance)m_instance;
			}
		}

		internal ScalePin(Microsoft.ReportingServices.ReportIntermediateFormat.ScalePin defObject, GaugePanel gaugePanel)
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
			if (m_pinLabel != null)
			{
				m_pinLabel.SetNewContext();
			}
		}

		protected override TickMarkStyleInstance GetInstance()
		{
			return new ScalePinInstance(this);
		}
	}
}
