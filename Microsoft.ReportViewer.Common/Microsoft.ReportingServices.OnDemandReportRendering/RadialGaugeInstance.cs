using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialGaugeInstance : GaugeInstance
	{
		private RadialGauge m_defObject;

		private double? m_pivotX;

		private double? m_pivotY;

		public double PivotX
		{
			get
			{
				if (!m_pivotX.HasValue)
				{
					m_pivotX = ((Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge)m_defObject.GaugeDef).EvaluatePivotX(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_pivotX.Value;
			}
		}

		public double PivotY
		{
			get
			{
				if (!m_pivotY.HasValue)
				{
					m_pivotY = ((Microsoft.ReportingServices.ReportIntermediateFormat.RadialGauge)m_defObject.GaugeDef).EvaluatePivotY(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_pivotY.Value;
			}
		}

		internal RadialGaugeInstance(RadialGauge defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_pivotX = null;
			m_pivotY = null;
		}
	}
}
