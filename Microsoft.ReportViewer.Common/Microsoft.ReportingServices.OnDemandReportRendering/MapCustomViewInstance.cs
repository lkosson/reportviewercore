using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapCustomViewInstance : MapViewInstance
	{
		private MapCustomView m_defObject;

		private double? m_centerX;

		private double? m_centerY;

		public double CenterX
		{
			get
			{
				if (!m_centerX.HasValue)
				{
					m_centerX = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView)m_defObject.MapViewDef).EvaluateCenterX(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_centerX.Value;
			}
		}

		public double CenterY
		{
			get
			{
				if (!m_centerY.HasValue)
				{
					m_centerY = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView)m_defObject.MapViewDef).EvaluateCenterY(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_centerY.Value;
			}
		}

		internal MapCustomViewInstance(MapCustomView defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_centerX = null;
			m_centerY = null;
		}
	}
}
