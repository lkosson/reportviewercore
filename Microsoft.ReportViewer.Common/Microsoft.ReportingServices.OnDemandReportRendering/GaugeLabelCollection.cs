using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeLabelCollection : GaugePanelObjectCollectionBase<GaugeLabel>
	{
		private GaugePanel m_gaugePanel;

		public GaugeLabel this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.GaugeLabel gaugeLabel = m_gaugePanel.GaugePanelDef.GaugeLabels[i];
					if (string.CompareOrdinal(name, gaugeLabel.Name) == 0)
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
				if (m_gaugePanel.GaugePanelDef.GaugeLabels != null)
				{
					return m_gaugePanel.GaugePanelDef.GaugeLabels.Count;
				}
				return 0;
			}
		}

		internal GaugeLabelCollection(GaugePanel gaugePanel)
		{
			m_gaugePanel = gaugePanel;
		}

		protected override GaugeLabel CreateGaugePanelObject(int index)
		{
			return new GaugeLabel(m_gaugePanel.GaugePanelDef.GaugeLabels[index], m_gaugePanel);
		}
	}
}
