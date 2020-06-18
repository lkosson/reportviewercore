using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialGaugeCollection : GaugePanelObjectCollectionBase<RadialGauge>
	{
		private GaugePanel m_gaugePanel;

		public RadialGauge this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge radialGauge = m_gaugePanel.GaugePanelDef.RadialGauges[i];
					if (string.CompareOrdinal(name, radialGauge.Name) == 0)
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
				if (m_gaugePanel.GaugePanelDef.RadialGauges != null)
				{
					return m_gaugePanel.GaugePanelDef.RadialGauges.Count;
				}
				return 0;
			}
		}

		internal RadialGaugeCollection(GaugePanel gaugePanel)
		{
			m_gaugePanel = gaugePanel;
		}

		protected override RadialGauge CreateGaugePanelObject(int index)
		{
			return new RadialGauge(m_gaugePanel.GaugePanelDef.RadialGauges[index], m_gaugePanel);
		}
	}
}
