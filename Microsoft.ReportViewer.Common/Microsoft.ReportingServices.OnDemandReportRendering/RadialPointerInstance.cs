using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialPointerInstance : GaugePointerInstance
	{
		private RadialPointer m_defObject;

		private RadialPointerTypes? m_type;

		private RadialPointerNeedleStyles? m_needleStyle;

		public RadialPointerTypes Type
		{
			get
			{
				if (!m_type.HasValue)
				{
					m_type = ((Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer)m_defObject.GaugePointerDef).EvaluateType(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_type.Value;
			}
		}

		public RadialPointerNeedleStyles NeedleStyle
		{
			get
			{
				if (!m_needleStyle.HasValue)
				{
					m_needleStyle = ((Microsoft.ReportingServices.ReportIntermediateFormat.RadialPointer)m_defObject.GaugePointerDef).EvaluateNeedleStyle(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_needleStyle.Value;
			}
		}

		internal RadialPointerInstance(RadialPointer defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_type = null;
			m_needleStyle = null;
		}
	}
}
