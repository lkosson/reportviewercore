using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class StateIndicatorCollection : GaugePanelObjectCollectionBase<StateIndicator>
	{
		private GaugePanel m_gaugePanel;

		public StateIndicator this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator = m_gaugePanel.GaugePanelDef.StateIndicators[i];
					if (string.CompareOrdinal(name, stateIndicator.Name) == 0)
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
				if (m_gaugePanel.GaugePanelDef.StateIndicators != null)
				{
					return m_gaugePanel.GaugePanelDef.StateIndicators.Count;
				}
				return 0;
			}
		}

		internal StateIndicatorCollection(GaugePanel gaugePanel)
		{
			m_gaugePanel = gaugePanel;
		}

		protected override StateIndicator CreateGaugePanelObject(int index)
		{
			return new StateIndicator(m_gaugePanel.GaugePanelDef.StateIndicators[index], m_gaugePanel);
		}
	}
}
