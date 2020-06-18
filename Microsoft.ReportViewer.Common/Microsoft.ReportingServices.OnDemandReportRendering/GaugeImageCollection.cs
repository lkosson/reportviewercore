using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeImageCollection : GaugePanelObjectCollectionBase<GaugeImage>
	{
		private GaugePanel m_gaugePanel;

		public GaugeImage this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage = m_gaugePanel.GaugePanelDef.GaugeImages[i];
					if (string.CompareOrdinal(name, gaugeImage.Name) == 0)
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
				if (m_gaugePanel.GaugePanelDef.GaugeImages != null)
				{
					return m_gaugePanel.GaugePanelDef.GaugeImages.Count;
				}
				return 0;
			}
		}

		internal GaugeImageCollection(GaugePanel gaugePanel)
		{
			m_gaugePanel = gaugePanel;
		}

		protected override GaugeImage CreateGaugePanelObject(int index)
		{
			return new GaugeImage(m_gaugePanel.GaugePanelDef.GaugeImages[index], m_gaugePanel);
		}
	}
}
