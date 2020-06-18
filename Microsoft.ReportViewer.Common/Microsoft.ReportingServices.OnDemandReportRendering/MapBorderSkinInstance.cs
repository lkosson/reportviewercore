namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBorderSkinInstance : BaseInstance
	{
		private MapBorderSkin m_defObject;

		private StyleInstance m_style;

		private MapBorderSkinType? m_mapBorderSkinType;

		public StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_defObject, m_defObject.MapDef.ReportScope, m_defObject.MapDef.RenderingContext);
				}
				return m_style;
			}
		}

		public MapBorderSkinType MapBorderSkinType
		{
			get
			{
				if (!m_mapBorderSkinType.HasValue)
				{
					m_mapBorderSkinType = m_defObject.MapBorderSkinDef.EvaluateMapBorderSkinType(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_mapBorderSkinType.Value;
			}
		}

		internal MapBorderSkinInstance(MapBorderSkin defObject)
			: base(defObject.MapDef.ReportScope)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			m_mapBorderSkinType = null;
		}
	}
}
