using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeImage : GaugePanelItem
	{
		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage GaugeImageDef => (Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage)m_defObject;

		public new GaugeImageInstance Instance => (GaugeImageInstance)GetInstance();

		internal GaugeImage(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage defObject, GaugePanel gaugePanel)
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
				m_instance = new GaugeImageInstance(this);
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
		}
	}
}
