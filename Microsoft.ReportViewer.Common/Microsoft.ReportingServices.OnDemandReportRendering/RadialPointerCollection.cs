using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialPointerCollection : GaugePanelObjectCollectionBase<RadialPointer>
	{
		private GaugePanel m_gaugePanel;

		private RadialScale m_radialScale;

		public RadialPointer this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer radialPointer = m_radialScale.RadialScaleDef.GaugePointers[i];
					if (string.CompareOrdinal(name, radialPointer.Name) == 0)
					{
						return base[i];
					}
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsNotInCollection, name);
			}
		}

		public override int Count => m_radialScale.RadialScaleDef.GaugePointers.Count;

		internal RadialPointerCollection(RadialScale radialScale, GaugePanel gaugePanel)
		{
			m_radialScale = radialScale;
			m_gaugePanel = gaugePanel;
		}

		protected override RadialPointer CreateGaugePanelObject(int index)
		{
			return new RadialPointer(m_radialScale.RadialScaleDef.GaugePointers[index], m_gaugePanel);
		}
	}
}
