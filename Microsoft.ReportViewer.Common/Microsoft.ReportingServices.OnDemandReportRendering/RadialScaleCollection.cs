using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialScaleCollection : GaugePanelObjectCollectionBase<RadialScale>
	{
		private GaugePanel m_gaugePanel;

		private RadialGauge m_radialGauge;

		public RadialScale this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale radialScale = m_radialGauge.RadialGaugeDef.GaugeScales[i];
					if (string.CompareOrdinal(name, radialScale.Name) == 0)
					{
						return base[i];
					}
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsNotInCollection, name);
			}
		}

		public override int Count => m_radialGauge.RadialGaugeDef.GaugeScales.Count;

		internal RadialScaleCollection(RadialGauge radialGauge, GaugePanel gaugePanel)
		{
			m_radialGauge = radialGauge;
			m_gaugePanel = gaugePanel;
		}

		protected override RadialScale CreateGaugePanelObject(int index)
		{
			return new RadialScale(m_radialGauge.RadialGaugeDef.GaugeScales[index], m_gaugePanel);
		}
	}
}
