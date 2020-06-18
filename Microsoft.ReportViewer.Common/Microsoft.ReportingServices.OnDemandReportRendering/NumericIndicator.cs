using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class NumericIndicator : GaugePanelItem
	{
		internal Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator NumericIndicatorDef => (Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator)m_defObject;

		public new NumericIndicatorInstance Instance => (NumericIndicatorInstance)GetInstance();

		internal NumericIndicator(Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
		}

		internal override BaseInstance GetInstance()
		{
			if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new NumericIndicatorInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
		}
	}
}
