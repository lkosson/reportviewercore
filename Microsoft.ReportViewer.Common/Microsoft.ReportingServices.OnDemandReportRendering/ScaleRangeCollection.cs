using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScaleRangeCollection : GaugePanelObjectCollectionBase<ScaleRange>
	{
		private GaugePanel m_gaugePanel;

		private GaugeScale m_gaugeScale;

		public ScaleRange this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange = m_gaugeScale.GaugeScaleDef.ScaleRanges[i];
					if (string.CompareOrdinal(name, scaleRange.Name) == 0)
					{
						return base[i];
					}
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsNotInCollection, name);
			}
		}

		public override int Count => m_gaugeScale.GaugeScaleDef.ScaleRanges.Count;

		internal ScaleRangeCollection(GaugeScale gaugeScale, GaugePanel gaugePanel)
		{
			m_gaugeScale = gaugeScale;
			m_gaugePanel = gaugePanel;
		}

		protected override ScaleRange CreateGaugePanelObject(int index)
		{
			return new ScaleRange(m_gaugeScale.GaugeScaleDef.ScaleRanges[index], m_gaugePanel);
		}
	}
}
