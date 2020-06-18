using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class GaugeInstance : GaugePanelItemInstance
	{
		private Gauge m_defObject;

		private bool? m_clipContent;

		private double? m_aspectRatio;

		public bool ClipContent
		{
			get
			{
				if (!m_clipContent.HasValue)
				{
					m_clipContent = ((Microsoft.ReportingServices.ReportIntermediateFormat.Gauge)m_defObject.GaugePanelItemDef).EvaluateClipContent(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_clipContent.Value;
			}
		}

		public double AspectRatio
		{
			get
			{
				if (!m_aspectRatio.HasValue)
				{
					m_aspectRatio = ((Microsoft.ReportingServices.ReportIntermediateFormat.Gauge)m_defObject.GaugePanelItemDef).EvaluateAspectRatio(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_aspectRatio.Value;
			}
		}

		internal GaugeInstance(Gauge defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_clipContent = null;
			m_aspectRatio = null;
		}
	}
}
