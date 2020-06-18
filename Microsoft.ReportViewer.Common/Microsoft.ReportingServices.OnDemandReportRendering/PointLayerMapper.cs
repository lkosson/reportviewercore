using Microsoft.Reporting.Map.WebForms;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class PointLayerMapper : VectorLayerMapper
	{
		private CoreSymbolManager m_symbolManager;

		protected override ISpatialElementCollection SpatialElementCollection => MapPointLayer.MapPoints;

		private MapPointLayer MapPointLayer => (MapPointLayer)m_mapVectorLayer;

		internal PointLayerMapper(MapPointLayer mapPointLayer, MapControl coreMap, MapMapper mapMapper)
			: base(mapPointLayer, coreMap, mapMapper)
		{
			if (mapPointLayer.MapPointTemplate != null)
			{
				m_pointTemplateMapper = CreatePointTemplateMapper();
			}
		}

		protected override CoreSpatialElementManager GetSpatialElementManager()
		{
			if (m_symbolManager == null)
			{
				m_symbolManager = new CoreSymbolManager(m_coreMap, m_mapVectorLayer);
			}
			return m_symbolManager;
		}

		protected override void CreateRules()
		{
			MapPointRules mapPointRules = MapPointLayer.MapPointRules;
			if (mapPointRules != null)
			{
				CreatePointRules(mapPointRules);
			}
		}

		protected override void RenderRules()
		{
			MapPointRules mapPointRules = MapPointLayer.MapPointRules;
			if (mapPointRules != null)
			{
				RenderPointRules(mapPointRules);
			}
		}

		protected override void RenderSpatialElement(SpatialElementInfo spatialElementInfo, bool hasScope)
		{
			InitializeSpatialElement(spatialElementInfo.CoreSpatialElement);
			RenderPoint((MapPoint)spatialElementInfo.MapSpatialElement, (Symbol)spatialElementInfo.CoreSpatialElement, hasScope);
		}

		protected override void RenderSymbolTemplate(MapSpatialElement mapSpatialElement, Symbol coreSymbol, bool hasScope)
		{
			m_pointTemplateMapper.Render((MapPoint)mapSpatialElement, coreSymbol, hasScope);
		}

		internal override MapPointRules GetMapPointRules()
		{
			return MapPointLayer.MapPointRules;
		}

		internal override MapPointTemplate GetMapPointTemplate()
		{
			return MapPointLayer.MapPointTemplate;
		}

		internal override bool IsValidSpatialElement(ISpatialElement spatialElement)
		{
			return spatialElement is Symbol;
		}

		internal override void OnSpatialElementAdded(ISpatialElement spatialElement)
		{
		}
	}
}
