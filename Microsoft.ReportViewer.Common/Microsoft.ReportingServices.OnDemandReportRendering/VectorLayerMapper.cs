using Microsoft.Reporting.Map.WebForms;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class VectorLayerMapper
	{
		internal MapVectorLayer m_mapVectorLayer;

		internal MapControl m_coreMap;

		internal MapMapper m_mapMapper;

		protected SpatialDataMapper m_spatialDataMapper;

		protected PointTemplateMapper m_pointTemplateMapper;

		private ColorRuleMapper m_pointColorRuleMapper;

		private SizeRuleMapper m_pointlSizeRuleMapper;

		private MarkerRuleMapper m_pointMarkerRuleMapper;

		private CoreSymbolManager m_symbolManager;

		private Dictionary<SpatialElementKey, SpatialElementInfoGroup> m_spatialElementsDictionary = new Dictionary<SpatialElementKey, SpatialElementInfoGroup>();

		protected bool IsEmbeddedLayer => SpatialElementCollection != null;

		protected abstract ISpatialElementCollection SpatialElementCollection
		{
			get;
		}

		internal VectorLayerMapper(MapVectorLayer mapVectorLayer, MapControl coreMap, MapMapper mapMapper)
		{
			m_mapVectorLayer = mapVectorLayer;
			m_coreMap = coreMap;
			m_mapMapper = mapMapper;
		}

		internal void Render()
		{
			PopulateSpatialElements();
			CreateRules();
			RenderSpatialElements();
			RenderRules();
			UpdateView();
		}

		private void PopulateSpatialElements()
		{
			CreateSpatialDataMapper();
			if (m_spatialDataMapper != null)
			{
				m_spatialDataMapper.Populate();
			}
		}

		private void CreateSpatialDataMapper()
		{
			_ = m_mapVectorLayer.MapSpatialData;
			if (IsEmbeddedLayer)
			{
				m_spatialDataMapper = new EmbeddedSpatialDataMapper(this, m_spatialElementsDictionary, SpatialElementCollection, GetSpatialElementManager(), m_coreMap, m_mapMapper);
			}
			else if (m_mapVectorLayer.MapSpatialData is MapSpatialDataSet)
			{
				m_spatialDataMapper = new SpatialDataSetMapper(this, m_spatialElementsDictionary, GetSpatialElementManager(), m_coreMap, m_mapMapper);
			}
			else if (m_mapVectorLayer.MapSpatialData is MapShapefile)
			{
				m_spatialDataMapper = new ShapefileMapper(this, m_spatialElementsDictionary, m_coreMap, m_mapMapper);
			}
			else
			{
				m_spatialDataMapper = null;
			}
		}

		private void RenderGrouping(MapMember mapMember)
		{
			if (!mapMember.IsStatic)
			{
				MapDynamicMemberInstance mapDynamicMemberInstance = (MapDynamicMemberInstance)mapMember.Instance;
				mapDynamicMemberInstance.ResetContext();
				while (mapDynamicMemberInstance.MoveNext())
				{
					if (mapMember.ChildMapMember != null)
					{
						RenderGrouping(mapMember.ChildMapMember);
					}
					else
					{
						RenderInnerMostMember();
					}
				}
			}
			else if (mapMember.ChildMapMember != null)
			{
				RenderGrouping(mapMember.ChildMapMember);
			}
			else
			{
				RenderInnerMostMember();
			}
		}

		private void RenderSpatialElements()
		{
			MapDataRegion mapDataRegion = m_mapVectorLayer.MapDataRegion;
			if (mapDataRegion != null)
			{
				RenderGrouping(mapDataRegion.MapMember);
			}
			RenderNonBoundSpatialElements();
		}

		private void RenderInnerMostMember()
		{
			SpatialElementInfoGroup spatialElementInfoGroup = (m_spatialDataMapper == null) ? CreateSpatialElementFromDataRegion() : GetSpatialElementsFromDataRegionKey();
			if (spatialElementInfoGroup != null)
			{
				if (spatialElementInfoGroup.BoundToData)
				{
					throw new RenderingObjectModelException(RPRes.rsMapSpatialElementHasMoreThanOnMatchingGroupInstance(RPRes.rsObjectTypeMap, m_mapVectorLayer.MapDef.Name, m_mapVectorLayer.Name));
				}
				RenderSpatialElementGroup(spatialElementInfoGroup, hasScope: true);
				spatialElementInfoGroup.BoundToData = true;
			}
		}

		private void RenderSpatialElementGroup(SpatialElementInfoGroup group, bool hasScope)
		{
			foreach (SpatialElementInfo element in group.Elements)
			{
				RenderSpatialElement(element, hasScope);
			}
		}

		protected void InitializeSpatialElement(ISpatialElement spatialElement)
		{
			spatialElement.Text = "";
		}

		private void RenderNonBoundSpatialElements()
		{
			if (m_spatialDataMapper == null)
			{
				return;
			}
			MapDataRegion mapDataRegion = m_mapVectorLayer.MapDataRegion;
			foreach (KeyValuePair<SpatialElementKey, SpatialElementInfoGroup> item in m_spatialElementsDictionary)
			{
				if (!item.Value.BoundToData)
				{
					RenderSpatialElementGroup(item.Value, mapDataRegion == null);
				}
			}
		}

		private SpatialElementInfoGroup GetSpatialElementsFromDataRegionKey()
		{
			MapBindingFieldPairCollection mapBindingFieldPairs = m_mapVectorLayer.MapBindingFieldPairs;
			if (mapBindingFieldPairs != null)
			{
				SpatialElementKey spatialElementKey = CreateDataRegionSpatialElementKey(mapBindingFieldPairs);
				ValidateKey(spatialElementKey, mapBindingFieldPairs);
				if (m_spatialElementsDictionary.TryGetValue(spatialElementKey, out SpatialElementInfoGroup value))
				{
					return value;
				}
			}
			return null;
		}

		internal static SpatialElementKey CreateDataRegionSpatialElementKey(MapBindingFieldPairCollection mapBindingFieldPairs)
		{
			List<object> list = new List<object>();
			for (int i = 0; i < mapBindingFieldPairs.Count; i++)
			{
				list.Add(EvaluateBindingExpression(mapBindingFieldPairs[i]));
			}
			return new SpatialElementKey(list);
		}

		internal void ValidateKey(SpatialElementKey spatialElementKey, MapBindingFieldPairCollection mapBindingFieldPairs)
		{
			if (m_spatialDataMapper.KeyTypes == null)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num >= spatialElementKey.KeyValues.Count)
				{
					return;
				}
				object obj = spatialElementKey.KeyValues[num];
				if (obj != null)
				{
					Type type = obj.GetType();
					Type type2 = m_spatialDataMapper.KeyTypes[num];
					if (!(type2 == null) && type != type2)
					{
						object obj2 = Convert(obj, type, type2);
						if (obj2 == null)
						{
							break;
						}
						spatialElementKey.KeyValues[num] = obj2;
					}
				}
				num++;
			}
			throw new RenderingObjectModelException(RPRes.rsMapFieldBindingExpressionTypeMismatch(RPRes.rsObjectTypeMap, m_mapVectorLayer.MapDef.Name, m_mapVectorLayer.Name, SpatialDataMapper.GetBindingFieldName(mapBindingFieldPairs[num])));
		}

		private static object Convert(object value, Type type, Type conversionType)
		{
			TypeCode typeCode = Type.GetTypeCode(conversionType);
			TypeCode typeCode2 = Type.GetTypeCode(type);
			switch (typeCode)
			{
			case TypeCode.Int32:
			{
				int convertedValue;
				switch (typeCode2)
				{
				case TypeCode.Decimal:
					if (TryConvertDecimalToInt((decimal)value, out convertedValue))
					{
						return convertedValue;
					}
					break;
				case TypeCode.Double:
					if (TryConvertDoubleToInt((double)value, out convertedValue))
					{
						return convertedValue;
					}
					break;
				}
				break;
			}
			case TypeCode.Decimal:
				if (typeCode2 == TypeCode.Int32)
				{
					return (decimal)(int)value;
				}
				break;
			case TypeCode.Double:
				switch (typeCode2)
				{
				case TypeCode.Int32:
					return (double)(int)value;
				case TypeCode.Single:
					return (double)(float)value;
				}
				break;
			}
			return null;
		}

		private static bool TryConvertDecimalToInt(decimal value, out int convertedValue)
		{
			if (value > 2147483647m || value < -2147483648m)
			{
				convertedValue = 0;
				return false;
			}
			convertedValue = (int)value;
			return value.Equals(convertedValue);
		}

		private static bool TryConvertDoubleToInt(double value, out int convertedValue)
		{
			if (value > 2147483647.0 || value < -2147483648.0)
			{
				convertedValue = 0;
				return false;
			}
			convertedValue = (int)value;
			return value.Equals(convertedValue);
		}

		internal static object EvaluateBindingExpression(MapBindingFieldPair mapBindingFieldPair)
		{
			ReportVariantProperty bindingExpression = mapBindingFieldPair.BindingExpression;
			if (!bindingExpression.IsExpression)
			{
				return bindingExpression.Value;
			}
			return mapBindingFieldPair.Instance.BindingExpression;
		}

		private SpatialElementInfoGroup CreateSpatialElementFromDataRegion()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			if (!m_mapMapper.CanAddSpatialElement)
			{
				return null;
			}
			MapSpatialDataRegion mapSpatialDataRegion = (MapSpatialDataRegion)m_mapVectorLayer.MapSpatialData;
			if (mapSpatialDataRegion == null)
			{
				return null;
			}
			object vectorData = mapSpatialDataRegion.Instance.VectorData;
			if (vectorData == null)
			{
				return null;
			}
			ISpatialElement spatialElement = null;
			if (spatialElement == null)
			{
				return null;
			}
			SpatialElementInfo spatialElementInfo = new SpatialElementInfo();
			spatialElementInfo.CoreSpatialElement = spatialElement;
			spatialElementInfo.MapSpatialElement = null;
			SpatialElementInfoGroup spatialElementInfoGroup = new SpatialElementInfoGroup();
			spatialElementInfoGroup.Elements.Add(spatialElementInfo);
			m_spatialElementsDictionary.Add(new SpatialElementKey(null), spatialElementInfoGroup);
			OnSpatialElementAdded(spatialElement);
			m_mapMapper.OnSpatialElementAdded(spatialElementInfo);
			return spatialElementInfoGroup;
		}

		protected void RenderSymbolRuleFields(Symbol symbol)
		{
			if (m_pointColorRuleMapper != null)
			{
				m_pointColorRuleMapper.SetRuleFieldValue(symbol);
			}
			if (m_pointlSizeRuleMapper != null)
			{
				m_pointlSizeRuleMapper.SetRuleFieldValue(symbol);
			}
			if (m_pointMarkerRuleMapper != null)
			{
				m_pointMarkerRuleMapper.SetRuleFieldValue(symbol);
			}
		}

		protected void RenderPoint(MapSpatialElement mapSpatialElement, Symbol symbol, bool hasScope)
		{
			if (hasScope)
			{
				RenderSymbolRuleFields(symbol);
			}
			RenderSymbolTemplate(mapSpatialElement, symbol, hasScope);
		}

		protected void CreatePointRules(MapPointRules mapPointRules)
		{
			if (mapPointRules.MapColorRule != null)
			{
				m_pointColorRuleMapper = new ColorRuleMapper(mapPointRules.MapColorRule, this, GetSymbolManager());
				m_pointColorRuleMapper.CreateSymbolRule();
			}
			if (mapPointRules.MapSizeRule != null)
			{
				m_pointlSizeRuleMapper = new SizeRuleMapper(mapPointRules.MapSizeRule, this, GetSymbolManager());
				m_pointlSizeRuleMapper.CreateSymbolRule();
			}
			if (mapPointRules.MapMarkerRule != null)
			{
				m_pointMarkerRuleMapper = new MarkerRuleMapper(mapPointRules.MapMarkerRule, this, GetSymbolManager());
				m_pointMarkerRuleMapper.CreateSymbolRule();
			}
		}

		protected void RenderPointRules(MapPointRules mapPointRules)
		{
			int? legendSymbolSize = GetLegendSymbolSize();
			Color? legendSymbolColor = GetLegendSymbolColor();
			MarkerStyle? legendSymbolMarker = GetLegendSymbolMarker();
			if (mapPointRules.MapColorRule != null)
			{
				m_pointColorRuleMapper.RenderSymbolRule(m_pointTemplateMapper, legendSymbolSize, legendSymbolMarker);
			}
			if (mapPointRules.MapSizeRule != null)
			{
				m_pointlSizeRuleMapper.RenderSymbolRule(m_pointTemplateMapper, legendSymbolColor, legendSymbolMarker);
			}
			if (mapPointRules.MapMarkerRule != null)
			{
				m_pointMarkerRuleMapper.RenderPointRule(m_pointTemplateMapper, legendSymbolColor, legendSymbolSize);
			}
		}

		private Color? GetLegendSymbolColor()
		{
			if (m_pointTemplateMapper == null)
			{
				return Color.Empty;
			}
			return m_pointTemplateMapper.GetBackgroundColor(hasScope: false);
		}

		private int? GetLegendSymbolSize()
		{
			if (m_pointTemplateMapper == null)
			{
				return PointTemplateMapper.GetDefaultSymbolSize(m_mapMapper.DpiX);
			}
			return m_pointTemplateMapper.GetSize(GetMapPointTemplate(), hasScope: false);
		}

		private MarkerStyle? GetLegendSymbolMarker()
		{
			if (!(m_pointTemplateMapper is SymbolMarkerTemplateMapper))
			{
				return MarkerStyle.None;
			}
			return MapMapper.GetMarkerStyle(MapMapper.GetMarkerStyle(((MapMarkerTemplate)GetMapPointTemplate()).MapMarker, hasScope: false));
		}

		protected virtual void RenderSymbolTemplate(MapSpatialElement mapSpatialElement, Symbol coreSymbol, bool hasScope)
		{
		}

		internal virtual MapPointRules GetMapPointRules()
		{
			return null;
		}

		internal virtual MapPointTemplate GetMapPointTemplate()
		{
			return null;
		}

		internal bool HasPointColorRule(Symbol symbol)
		{
			if (!HasPointColorRule())
			{
				return false;
			}
			return m_pointColorRuleMapper.HasDataValue(symbol);
		}

		protected bool HasPointColorRule()
		{
			MapPointRules mapPointRules = GetMapPointRules();
			if (mapPointRules == null)
			{
				return false;
			}
			if (mapPointRules.MapColorRule != null)
			{
				return true;
			}
			return false;
		}

		internal bool HasPointSizeRule(Symbol symbol)
		{
			if (!HasPointSizeRule())
			{
				return false;
			}
			return m_pointlSizeRuleMapper.HasDataValue(symbol);
		}

		protected bool HasPointSizeRule()
		{
			MapPointRules mapPointRules = GetMapPointRules();
			if (mapPointRules == null)
			{
				return false;
			}
			if (mapPointRules.MapSizeRule != null)
			{
				return true;
			}
			return false;
		}

		internal bool HasMarkerRule(Symbol symbol)
		{
			if (!HasMarkerRule())
			{
				return false;
			}
			return m_pointMarkerRuleMapper.HasDataValue(symbol);
		}

		protected bool HasMarkerRule()
		{
			MapPointRules mapPointRules = GetMapPointRules();
			if (mapPointRules == null)
			{
				return false;
			}
			if (mapPointRules.MapMarkerRule != null)
			{
				return true;
			}
			return false;
		}

		protected PointTemplateMapper CreatePointTemplateMapper()
		{
			return new SymbolMarkerTemplateMapper(m_mapMapper, this, m_mapVectorLayer);
		}

		internal static string AddPrefixToFieldNames(string layerName, string expression)
		{
			if (expression == null)
			{
				return null;
			}
			string[] array = expression.Split('#');
			string text = "";
			if (array.Length == 1)
			{
				return expression;
			}
			for (int i = 0; i < array.Length; i++)
			{
				text = ((!(array[i] == "")) ? string.Format(CultureInfo.InvariantCulture, "{0}{1}_{2}", text, layerName, array[i]) : string.Format(CultureInfo.InvariantCulture, "{0}{1}", text, "#"));
			}
			return text;
		}

		protected CoreSymbolManager GetSymbolManager()
		{
			if (m_symbolManager == null)
			{
				m_symbolManager = new CoreSymbolManager(m_coreMap, m_mapVectorLayer);
			}
			return m_symbolManager;
		}

		private void UpdateView()
		{
			m_coreMap.mapCore.UpdateCachedBounds();
			MapView mapView = m_mapVectorLayer.MapDef.MapViewport.MapView;
			if (mapView is MapDataBoundView)
			{
				AddBoundSpatialElementsToView();
			}
			else if (mapView is MapElementView)
			{
				AddSpatialElementToView((MapElementView)mapView);
			}
		}

		private void AddBoundSpatialElementsToView()
		{
			foreach (KeyValuePair<SpatialElementKey, SpatialElementInfoGroup> item in m_spatialElementsDictionary)
			{
				if (item.Value.BoundToData)
				{
					AddSpatialElementGroupToView(item.Value);
				}
			}
		}

		private void AddSpatialElementGroupToView(SpatialElementInfoGroup group)
		{
			foreach (SpatialElementInfo element in group.Elements)
			{
				m_mapMapper.AddSpatialElementToView(element.CoreSpatialElement);
			}
		}

		private void AddSpatialElementToView(MapElementView mapView)
		{
			if (GetElementViewLayerName(mapView) != m_mapVectorLayer.Name)
			{
				return;
			}
			List<ISpatialElement> elementViewSpatialElements = GetElementViewSpatialElements(mapView);
			if (elementViewSpatialElements != null)
			{
				foreach (ISpatialElement item in elementViewSpatialElements)
				{
					m_mapMapper.AddSpatialElementToView(item);
				}
				return;
			}
			foreach (KeyValuePair<SpatialElementKey, SpatialElementInfoGroup> item2 in m_spatialElementsDictionary)
			{
				AddSpatialElementGroupToView(item2.Value);
			}
		}

		private List<ISpatialElement> GetElementViewSpatialElements(MapElementView mapView)
		{
			MapBindingFieldPairCollection mapBindingFieldPairs = mapView.MapBindingFieldPairs;
			if (mapBindingFieldPairs == null)
			{
				return null;
			}
			SpatialElementKey obj = CreateDataRegionSpatialElementKey(mapBindingFieldPairs);
			List<ISpatialElement> list = null;
			foreach (KeyValuePair<SpatialElementKey, SpatialElementInfoGroup> item in m_spatialElementsDictionary)
			{
				foreach (SpatialElementInfo element in item.Value.Elements)
				{
					if (SpatialDataMapper.CreateCoreSpatialElementKey(element.CoreSpatialElement, mapView.MapBindingFieldPairs, m_mapVectorLayer.MapDef.Name, m_mapVectorLayer.Name).Equals(obj))
					{
						if (list == null)
						{
							list = new List<ISpatialElement>();
						}
						list.Add(element.CoreSpatialElement);
					}
				}
			}
			return list;
		}

		private string GetElementViewLayerName(MapElementView mapView)
		{
			ReportStringProperty layerName = mapView.LayerName;
			if (!layerName.IsExpression)
			{
				return layerName.Value;
			}
			return mapView.Instance.LayerName;
		}

		protected abstract CoreSpatialElementManager GetSpatialElementManager();

		protected abstract void CreateRules();

		protected abstract void RenderRules();

		protected abstract void RenderSpatialElement(SpatialElementInfo spatialElementInfo, bool hasScope);

		internal abstract bool IsValidSpatialElement(ISpatialElement spatialElement);

		internal abstract void OnSpatialElementAdded(ISpatialElement spatialElement);
	}
}
