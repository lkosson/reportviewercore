namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSizeInstance : BaseInstance
	{
		private MapSize m_defObject;

		private double? m_width;

		private double? m_height;

		private Unit? m_unit;

		public double Width
		{
			get
			{
				if (!m_width.HasValue)
				{
					m_width = m_defObject.MapSizeDef.EvaluateWidth(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_width.Value;
			}
		}

		public double Height
		{
			get
			{
				if (!m_height.HasValue)
				{
					m_height = m_defObject.MapSizeDef.EvaluateHeight(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_height.Value;
			}
		}

		public Unit Unit
		{
			get
			{
				if (!m_unit.HasValue)
				{
					m_unit = m_defObject.MapSizeDef.EvaluateUnit(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_unit.Value;
			}
		}

		internal MapSizeInstance(MapSize defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			m_width = null;
			m_height = null;
			m_unit = null;
		}
	}
}
