using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearGaugeInstance : GaugeInstance
	{
		private LinearGauge m_defObject;

		private GaugeOrientations? m_orientation;

		public GaugeOrientations Orientation
		{
			get
			{
				if (!m_orientation.HasValue)
				{
					m_orientation = ((Microsoft.ReportingServices.ReportIntermediateFormat.LinearGauge)m_defObject.GaugeDef).EvaluateOrientation(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_orientation.Value;
			}
		}

		internal LinearGaugeInstance(LinearGauge defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_orientation = null;
		}
	}
}
