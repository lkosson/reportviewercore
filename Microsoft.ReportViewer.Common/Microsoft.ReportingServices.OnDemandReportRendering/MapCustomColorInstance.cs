namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapCustomColorInstance : BaseInstance
	{
		private MapCustomColor m_defObject;

		private ReportColor m_color;

		public ReportColor Color
		{
			get
			{
				if (m_color == null)
				{
					m_color = new ReportColor(m_defObject.MapCustomColorDef.EvaluateColor(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_color;
			}
		}

		internal MapCustomColorInstance(MapCustomColor defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_color = null;
		}
	}
}
