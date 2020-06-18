using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTitleInstance : MapDockableSubItemInstance
	{
		private MapTitle m_defObject;

		private string m_text;

		private bool m_textEvaluated;

		private double? m_angle;

		private ReportSize m_textShadowOffset;

		public string Text
		{
			get
			{
				if (!m_textEvaluated)
				{
					m_text = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle)m_defObject.MapDockableSubItemDef).EvaluateText(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
					m_textEvaluated = true;
				}
				return m_text;
			}
		}

		public double Angle
		{
			get
			{
				if (!m_angle.HasValue)
				{
					m_angle = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle)m_defObject.MapDockableSubItemDef).EvaluateAngle(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_angle.Value;
			}
		}

		public ReportSize TextShadowOffset
		{
			get
			{
				if (m_textShadowOffset == null)
				{
					m_textShadowOffset = new ReportSize(((Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle)m_defObject.MapDockableSubItemDef).EvaluateTextShadowOffset(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_textShadowOffset;
			}
		}

		internal MapTitleInstance(MapTitle defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_text = null;
			m_textEvaluated = false;
			m_angle = null;
			m_textShadowOffset = null;
		}
	}
}
