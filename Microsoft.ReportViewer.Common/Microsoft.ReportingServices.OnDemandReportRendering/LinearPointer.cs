using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearPointer : GaugePointer
	{
		private ReportEnumProperty<LinearPointerTypes> m_type;

		private Thermometer m_thermometer;

		public ReportEnumProperty<LinearPointerTypes> Type
		{
			get
			{
				if (m_type == null && LinearPointerDef.Type != null)
				{
					m_type = new ReportEnumProperty<LinearPointerTypes>(LinearPointerDef.Type.IsExpression, LinearPointerDef.Type.OriginalText, EnumTranslator.TranslateLinearPointerTypes(LinearPointerDef.Type.StringValue, null));
				}
				return m_type;
			}
		}

		public Thermometer Thermometer
		{
			get
			{
				if (m_thermometer == null && LinearPointerDef.Thermometer != null)
				{
					m_thermometer = new Thermometer(LinearPointerDef.Thermometer, m_gaugePanel);
				}
				return m_thermometer;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.LinearPointer LinearPointerDef => (Microsoft.ReportingServices.ReportIntermediateFormat.LinearPointer)m_defObject;

		public new LinearPointerInstance Instance => (LinearPointerInstance)GetInstance();

		internal LinearPointer(Microsoft.ReportingServices.ReportIntermediateFormat.LinearPointer defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal override GaugePointerInstance GetInstance()
		{
			if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new LinearPointerInstance(this);
			}
			return (GaugePointerInstance)m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_thermometer != null)
			{
				m_thermometer.SetNewContext();
			}
		}
	}
}
