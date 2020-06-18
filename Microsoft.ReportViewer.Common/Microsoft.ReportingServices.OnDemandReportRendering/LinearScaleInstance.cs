using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearScaleInstance : GaugeScaleInstance
	{
		private LinearScale m_defObject;

		private double? m_startMargin;

		private double? m_endMargin;

		private double? m_position;

		public double StartMargin
		{
			get
			{
				if (!m_startMargin.HasValue)
				{
					m_startMargin = ((Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale)m_defObject.GaugeScaleDef).EvaluateStartMargin(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_startMargin.Value;
			}
		}

		public double EndMargin
		{
			get
			{
				if (!m_endMargin.HasValue)
				{
					m_endMargin = ((Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale)m_defObject.GaugeScaleDef).EvaluateEndMargin(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_endMargin.Value;
			}
		}

		public double Position
		{
			get
			{
				if (!m_position.HasValue)
				{
					m_position = ((Microsoft.ReportingServices.ReportIntermediateFormat.LinearScale)m_defObject.GaugeScaleDef).EvaluatePosition(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_position.Value;
			}
		}

		internal LinearScaleInstance(LinearScale defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_startMargin = null;
			m_endMargin = null;
			m_position = null;
		}
	}
}
