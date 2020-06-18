using Microsoft.Reporting.Map.WebForms;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class PolygonLayerMapper : VectorLayerMapper
	{
		private CoreShapeManager m_shapeManager;

		private ColorRuleMapper m_polygonColorRuleMapper;

		private PolygonTemplateMapper m_polygonTemplateMapper;

		protected override ISpatialElementCollection SpatialElementCollection => MapPolygonLayer.MapPolygons;

		private MapPolygonLayer MapPolygonLayer => (MapPolygonLayer)m_mapVectorLayer;

		internal PolygonLayerMapper(MapPolygonLayer mapPolygonLayer, MapControl coreMap, MapMapper mapMapper)
			: base(mapPolygonLayer, coreMap, mapMapper)
		{
			m_polygonTemplateMapper = new PolygonTemplateMapper(m_mapMapper, this, MapPolygonLayer);
			m_pointTemplateMapper = CreatePointTemplateMapper();
		}

		protected override CoreSpatialElementManager GetSpatialElementManager()
		{
			if (m_shapeManager == null)
			{
				m_shapeManager = new CoreShapeManager(m_coreMap, m_mapVectorLayer);
			}
			return m_shapeManager;
		}

		internal bool HasColorRule(Shape shape)
		{
			if (!HasColorRule())
			{
				return false;
			}
			return m_polygonColorRuleMapper.HasDataValue(shape);
		}

		private bool HasColorRule()
		{
			MapColorRule mapColorRule = MapPolygonLayer.MapPolygonRules?.MapColorRule;
			return mapColorRule != null;
		}

		protected override void CreateRules()
		{
			MapPolygonRules mapPolygonRules = MapPolygonLayer.MapPolygonRules;
			if (mapPolygonRules != null && mapPolygonRules.MapColorRule != null)
			{
				m_polygonColorRuleMapper = new ColorRuleMapper(mapPolygonRules.MapColorRule, this, GetSpatialElementManager());
				m_polygonColorRuleMapper.CreatePolygonRule();
			}
			MapPointRules mapCenterPointRules = MapPolygonLayer.MapCenterPointRules;
			if (mapCenterPointRules != null)
			{
				CreatePointRules(mapCenterPointRules);
			}
		}

		protected override void RenderRules()
		{
			MapPolygonRules mapPolygonRules = MapPolygonLayer.MapPolygonRules;
			if (mapPolygonRules != null && mapPolygonRules.MapColorRule != null)
			{
				m_polygonColorRuleMapper.RenderPolygonRule(m_polygonTemplateMapper);
			}
			MapPointRules mapCenterPointRules = MapPolygonLayer.MapCenterPointRules;
			if (mapCenterPointRules != null)
			{
				RenderPointRules(mapCenterPointRules);
			}
		}

		protected override void RenderSpatialElement(SpatialElementInfo spatialElementInfo, bool hasScope)
		{
			InitializeSpatialElement(spatialElementInfo.CoreSpatialElement);
			if (hasScope)
			{
				RenderPolygonRulesField((Shape)spatialElementInfo.CoreSpatialElement);
			}
			RenderPolygonTemplate((MapPolygon)spatialElementInfo.MapSpatialElement, (Shape)spatialElementInfo.CoreSpatialElement, hasScope);
			RenderPolygonCenterPoint(spatialElementInfo, hasScope);
		}

		internal override MapPointRules GetMapPointRules()
		{
			return MapPolygonLayer.MapCenterPointRules;
		}

		internal override MapPointTemplate GetMapPointTemplate()
		{
			return MapPolygonLayer.MapCenterPointTemplate;
		}

		private bool HasCenterPointRule()
		{
			if (!HasPointColorRule() && !HasPointSizeRule())
			{
				return HasMarkerRule();
			}
			return true;
		}

		private bool HasCenterPointTemplate(MapPolygon mapPolygon, MapPointTemplate pointTemplate, bool hasScope)
		{
			if (mapPolygon == null || !PointTemplateMapper.PolygonUseCustomTemplate(mapPolygon, hasScope))
			{
				return pointTemplate != null;
			}
			return mapPolygon.MapCenterPointTemplate != null;
		}

		private void RenderPolygonCenterPoint(SpatialElementInfo spatialElementInfo, bool hasScope)
		{
			if (HasCenterPointRule() || HasCenterPointTemplate((MapPolygon)spatialElementInfo.MapSpatialElement, MapPolygonLayer.MapCenterPointTemplate, hasScope))
			{
				Symbol symbol = (Symbol)GetSymbolManager().CreateSpatialElement();
				symbol.Layer = spatialElementInfo.CoreSpatialElement.Layer;
				symbol.Category = spatialElementInfo.CoreSpatialElement.Category;
				symbol.ParentShape = spatialElementInfo.CoreSpatialElement.Name;
				CopyFieldsToPoint((Shape)spatialElementInfo.CoreSpatialElement, symbol);
				GetSymbolManager().AddSpatialElement(symbol);
				RenderPoint(spatialElementInfo.MapSpatialElement, symbol, hasScope);
			}
		}

		private void CopyFieldsToPoint(Shape shape, Symbol symbol)
		{
			foreach (string key in shape.fields.Keys)
			{
				CopyFieldToPoint(shape, symbol, key);
			}
		}

		private void CopyFieldToPoint(Shape shape, Symbol symbol, string fieldName)
		{
			if (m_coreMap.SymbolFields.GetByName(fieldName) == null)
			{
				Microsoft.Reporting.Map.WebForms.Field field = new Microsoft.Reporting.Map.WebForms.Field();
				field.Name = fieldName;
				field.Type = ((Microsoft.Reporting.Map.WebForms.Field)m_coreMap.ShapeFields.GetByName(fieldName)).Type;
				m_coreMap.SymbolFields.Add(field);
			}
			symbol[fieldName] = shape[fieldName];
		}

		private void RenderPolygonRulesField(Shape shape)
		{
			if (m_polygonColorRuleMapper != null)
			{
				m_polygonColorRuleMapper.SetRuleFieldValue(shape);
			}
		}

		private void RenderPolygonTemplate(MapPolygon mapPolygon, Shape coreShape, bool hasScope)
		{
			m_polygonTemplateMapper.Render(mapPolygon, coreShape, hasScope);
		}

		protected override void RenderSymbolTemplate(MapSpatialElement mapSpatialElement, Symbol coreSymbol, bool hasScope)
		{
			m_pointTemplateMapper.RenderPolygonCenterPoint((MapPolygon)mapSpatialElement, coreSymbol, hasScope);
		}

		internal override bool IsValidSpatialElement(ISpatialElement spatialElement)
		{
			return spatialElement is Shape;
		}

		internal override void OnSpatialElementAdded(ISpatialElement spatialElement)
		{
			m_mapMapper.Simplify((Shape)spatialElement);
		}
	}
}
