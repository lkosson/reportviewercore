using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearScaleCollection : GaugePanelObjectCollectionBase<LinearScale>
	{
		private GaugePanel m_gaugePanel;

		private LinearGauge m_linearGauge;

		public LinearScale this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale linearScale = m_linearGauge.LinearGaugeDef.GaugeScales[i];
					if (string.CompareOrdinal(name, linearScale.Name) == 0)
					{
						return base[i];
					}
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsNotInCollection, name);
			}
		}

		public override int Count => m_linearGauge.LinearGaugeDef.GaugeScales.Count;

		internal LinearScaleCollection(LinearGauge linearGauge, GaugePanel gaugePanel)
		{
			m_linearGauge = linearGauge;
			m_gaugePanel = gaugePanel;
		}

		protected override LinearScale CreateGaugePanelObject(int index)
		{
			return new LinearScale(m_linearGauge.LinearGaugeDef.GaugeScales[index], m_gaugePanel);
		}
	}
}
