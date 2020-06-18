using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class NumericIndicatorCollection : GaugePanelObjectCollectionBase<NumericIndicator>
	{
		private GaugePanel m_gaugePanel;

		public NumericIndicator this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator = m_gaugePanel.GaugePanelDef.NumericIndicators[i];
					if (string.CompareOrdinal(name, numericIndicator.Name) == 0)
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
				if (m_gaugePanel.GaugePanelDef.NumericIndicators != null)
				{
					return m_gaugePanel.GaugePanelDef.NumericIndicators.Count;
				}
				return 0;
			}
		}

		internal NumericIndicatorCollection(GaugePanel gaugePanel)
		{
			m_gaugePanel = gaugePanel;
		}

		protected override NumericIndicator CreateGaugePanelObject(int index)
		{
			return new NumericIndicator(m_gaugePanel.GaugePanelDef.NumericIndicators[index], m_gaugePanel);
		}
	}
}
