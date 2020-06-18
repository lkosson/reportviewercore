using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialScaleInstance : GaugeScaleInstance
	{
		private RadialScale m_defObject;

		private double? m_radius;

		private double? m_startAngle;

		private double? m_sweepAngle;

		public double Radius
		{
			get
			{
				if (!m_radius.HasValue)
				{
					m_radius = ((Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale)m_defObject.GaugeScaleDef).EvaluateRadius(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_radius.Value;
			}
		}

		public double StartAngle
		{
			get
			{
				if (!m_startAngle.HasValue)
				{
					m_startAngle = ((Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale)m_defObject.GaugeScaleDef).EvaluateStartAngle(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_startAngle.Value;
			}
		}

		public double SweepAngle
		{
			get
			{
				if (!m_sweepAngle.HasValue)
				{
					m_sweepAngle = ((Microsoft.ReportingServices.ReportIntermediateFormat.RadialScale)m_defObject.GaugeScaleDef).EvaluateSweepAngle(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_sweepAngle.Value;
			}
		}

		internal RadialScaleInstance(RadialScale defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_radius = null;
			m_startAngle = null;
			m_sweepAngle = null;
		}
	}
}
