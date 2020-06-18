using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class IndicatorStateCollection : GaugePanelObjectCollectionBase<IndicatorState>
	{
		private StateIndicator m_stateIndicator;

		private GaugePanel m_gaugePanel;

		public IndicatorState this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState = m_stateIndicator.StateIndicatorDef.IndicatorStates[i];
					if (string.CompareOrdinal(name, indicatorState.Name) == 0)
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
				if (m_stateIndicator.StateIndicatorDef.IndicatorStates != null)
				{
					return m_stateIndicator.StateIndicatorDef.IndicatorStates.Count;
				}
				return 0;
			}
		}

		internal IndicatorStateCollection(StateIndicator stateIndicator, GaugePanel gaugePanel)
		{
			m_stateIndicator = stateIndicator;
			m_gaugePanel = gaugePanel;
		}

		protected override IndicatorState CreateGaugePanelObject(int index)
		{
			return new IndicatorState(m_stateIndicator.StateIndicatorDef.IndicatorStates[index], m_gaugePanel);
		}
	}
}
