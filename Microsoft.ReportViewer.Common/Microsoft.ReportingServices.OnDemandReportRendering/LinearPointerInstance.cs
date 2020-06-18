using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearPointerInstance : GaugePointerInstance
	{
		private LinearPointer m_defObject;

		private LinearPointerTypes? m_type;

		public LinearPointerTypes Type
		{
			get
			{
				if (!m_type.HasValue)
				{
					m_type = ((Microsoft.ReportingServices.ReportIntermediateFormat.LinearPointer)m_defObject.GaugePointerDef).EvaluateType(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_type.Value;
			}
		}

		internal LinearPointerInstance(LinearPointer defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_type = null;
		}
	}
}
