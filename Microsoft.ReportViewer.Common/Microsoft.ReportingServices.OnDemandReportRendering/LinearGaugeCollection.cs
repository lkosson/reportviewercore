using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearGaugeCollection : GaugePanelObjectCollectionBase<LinearGauge>
	{
		private GaugePanel m_gaugePanel;

		public LinearGauge this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.LinearGauge linearGauge = m_gaugePanel.GaugePanelDef.LinearGauges[i];
					if (string.CompareOrdinal(name, linearGauge.Name) == 0)
					{
						return base[i];
					}
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsNotInCollection, name);
			}
		}

		public override int Count
		{
			get
			{
				if (m_gaugePanel.GaugePanelDef.LinearGauges != null)
				{
					return m_gaugePanel.GaugePanelDef.LinearGauges.Count;
				}
				return 0;
			}
		}

		internal LinearGaugeCollection(GaugePanel gaugePanel)
		{
			m_gaugePanel = gaugePanel;
		}

		protected override LinearGauge CreateGaugePanelObject(int index)
		{
			return new LinearGauge(m_gaugePanel.GaugePanelDef.LinearGauges[index], m_gaugePanel);
		}
	}
}
