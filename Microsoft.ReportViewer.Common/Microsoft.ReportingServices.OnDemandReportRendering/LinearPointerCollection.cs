using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearPointerCollection : GaugePanelObjectCollectionBase<LinearPointer>
	{
		private GaugePanel m_gaugePanel;

		private LinearScale m_linearScale;

		public LinearPointer this[string name]
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.LinearPointer linearPointer = m_linearScale.LinearScaleDef.GaugePointers[i];
					if (string.CompareOrdinal(name, linearPointer.Name) == 0)
					{
						return base[i];
					}
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsNotInCollection, name);
			}
		}

		public override int Count => m_linearScale.LinearScaleDef.GaugePointers.Count;

		internal LinearPointerCollection(LinearScale linearScale, GaugePanel gaugePanel)
		{
			m_linearScale = linearScale;
			m_gaugePanel = gaugePanel;
		}

		protected override LinearPointer CreateGaugePanelObject(int index)
		{
			return new LinearPointer(m_linearScale.LinearScaleDef.GaugePointers[index], m_gaugePanel);
		}
	}
}
