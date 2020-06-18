using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeTickMarksInstance : TickMarkStyleInstance
	{
		private double? m_interval;

		private double? m_intervalOffset;

		public double Interval
		{
			get
			{
				if (!m_interval.HasValue)
				{
					m_interval = ((Microsoft.ReportingServices.ReportIntermediateFormat.GaugeTickMarks)m_defObject.TickMarkStyleDef).EvaluateInterval(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_interval.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!m_intervalOffset.HasValue)
				{
					m_intervalOffset = ((Microsoft.ReportingServices.ReportIntermediateFormat.GaugeTickMarks)m_defObject.TickMarkStyleDef).EvaluateIntervalOffset(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_intervalOffset.Value;
			}
		}

		internal GaugeTickMarksInstance(GaugeTickMarks defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_interval = null;
			m_intervalOffset = null;
		}
	}
}
