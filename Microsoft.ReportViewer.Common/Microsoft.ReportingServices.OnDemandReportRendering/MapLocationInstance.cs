namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLocationInstance : BaseInstance
	{
		private MapLocation m_defObject;

		private double? m_left;

		private double? m_top;

		private Unit? m_unit;

		public double Left
		{
			get
			{
				if (!m_left.HasValue)
				{
					m_left = m_defObject.MapLocationDef.EvaluateLeft(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_left.Value;
			}
		}

		public double Top
		{
			get
			{
				if (!m_top.HasValue)
				{
					m_top = m_defObject.MapLocationDef.EvaluateTop(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_top.Value;
			}
		}

		public Unit Unit
		{
			get
			{
				if (!m_unit.HasValue)
				{
					m_unit = m_defObject.MapLocationDef.EvaluateUnit(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_unit.Value;
			}
		}

		internal MapLocationInstance(MapLocation defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_left = null;
			m_top = null;
			m_unit = null;
		}
	}
}
