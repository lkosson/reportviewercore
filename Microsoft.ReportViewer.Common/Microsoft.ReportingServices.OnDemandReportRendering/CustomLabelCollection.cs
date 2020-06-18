using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomLabelCollection : GaugePanelObjectCollectionBase<CustomLabel>
	{
		private GaugePanel m_gaugePanel;

		private GaugeScale m_gaugeScale;

		public CustomLabel this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel = m_gaugeScale.GaugeScaleDef.CustomLabels[i];
					if (string.CompareOrdinal(name, customLabel.Name) == 0)
					{
						return base[i];
					}
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsNotInCollection, name);
			}
		}

		public override int Count => m_gaugeScale.GaugeScaleDef.CustomLabels.Count;

		internal CustomLabelCollection(GaugeScale gaugeScale, GaugePanel gaugePanel)
		{
			m_gaugeScale = gaugeScale;
			m_gaugePanel = gaugePanel;
		}

		protected override CustomLabel CreateGaugePanelObject(int index)
		{
			return new CustomLabel(m_gaugeScale.GaugeScaleDef.CustomLabels[index], m_gaugePanel);
		}
	}
}
