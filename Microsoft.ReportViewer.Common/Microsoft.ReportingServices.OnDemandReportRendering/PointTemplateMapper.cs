using Microsoft.Reporting.Map.WebForms;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class PointTemplateMapper : SpatialElementTemplateMapper
	{
		private VectorLayerMapper m_vectorLayerMapper;

		private static string m_defaultSymbolSizeString = "5.25pt";

		private static ReportSize m_defaultSymbolSize = new ReportSize(m_defaultSymbolSizeString);

		private MapPointLayer MapPointLayer => (MapPointLayer)m_mapVectorLayer;

		protected override MapSpatialElementTemplate DefaultTemplate
		{
			get
			{
				if (m_mapVectorLayer is MapPolygonLayer)
				{
					return ((MapPolygonLayer)m_mapVectorLayer).MapCenterPointTemplate;
				}
				return MapPointLayer.MapPointTemplate;
			}
		}

		internal PointTemplateMapper(MapMapper mapMapper, VectorLayerMapper vectorLayerMapper, MapVectorLayer mapVectorLayer)
			: base(mapMapper, mapVectorLayer)
		{
			m_vectorLayerMapper = vectorLayerMapper;
		}

		internal void Render(MapPoint mapPoint, Symbol coreSymbol, bool hasScope)
		{
			bool flag = UseCustomTemplate(mapPoint, hasScope);
			MapPointTemplate mapPointTemplate = (!flag) ? MapPointLayer.MapPointTemplate : mapPoint.MapPointTemplate;
			RenderPointTemplate(mapPointTemplate, coreSymbol, flag, !flag && m_vectorLayerMapper.HasPointColorRule(coreSymbol) && hasScope, !flag && m_vectorLayerMapper.HasPointSizeRule(coreSymbol) && hasScope, !flag && m_vectorLayerMapper.HasMarkerRule(coreSymbol) && hasScope, hasScope);
		}

		internal void RenderPolygonCenterPoint(MapPolygon mapPolygon, Symbol coreSymbol, bool hasScope)
		{
			bool flag = PolygonUseCustomTemplate(mapPolygon, hasScope);
			MapPointTemplate mapPointTemplate = (!flag) ? m_vectorLayerMapper.GetMapPointTemplate() : mapPolygon.MapCenterPointTemplate;
			RenderPointTemplate(mapPointTemplate, coreSymbol, flag, !flag && m_vectorLayerMapper.HasPointColorRule(coreSymbol) && hasScope, !flag && m_vectorLayerMapper.HasPointSizeRule(coreSymbol) && hasScope, !flag && m_vectorLayerMapper.HasMarkerRule(coreSymbol) && hasScope, hasScope);
		}

		protected virtual void RenderPointTemplate(MapPointTemplate mapPointTemplate, Symbol coreSymbol, bool customTemplate, bool ignoreBackgroundColor, bool ignoreSize, bool ignoreMarker, bool hasScope)
		{
			if (mapPointTemplate == null)
			{
				RenderStyle(null, null, coreSymbol, ignoreBackgroundColor, hasScope);
				coreSymbol.BorderStyle = GetBorderStyle(null, null, hasScope);
				return;
			}
			RenderSpatialElementTemplate(mapPointTemplate, coreSymbol, ignoreBackgroundColor, hasScope);
			Style style = mapPointTemplate.Style;
			StyleInstance style2 = mapPointTemplate.Instance.Style;
			coreSymbol.BorderStyle = GetBorderStyle(style, style2, hasScope);
			if (!ignoreSize)
			{
				int size = GetSize(mapPointTemplate, hasScope);
				float num3 = coreSymbol.Width = (coreSymbol.Height = size);
			}
			ReportEnumProperty<MapPointLabelPlacement> labelPlacement = mapPointTemplate.LabelPlacement;
			TextAlignment textAlignment = TextAlignment.Bottom;
			if (labelPlacement != null)
			{
				if (!labelPlacement.IsExpression)
				{
					textAlignment = GetTextAlignment(labelPlacement.Value);
				}
				else if (hasScope)
				{
					textAlignment = GetTextAlignment(mapPointTemplate.Instance.LabelPlacement);
				}
			}
			coreSymbol.TextAlignment = textAlignment;
		}

		internal int GetSize(MapPointTemplate mapPointTemplate, bool hasScope)
		{
			ReportSizeProperty size = mapPointTemplate.Size;
			if (size != null)
			{
				if (!size.IsExpression)
				{
					return MappingHelper.ToIntPixels(size.Value, m_mapMapper.DpiX);
				}
				if (hasScope)
				{
					return MappingHelper.ToIntPixels(mapPointTemplate.Instance.Size, m_mapMapper.DpiX);
				}
				return GetDefaultSymbolSize(m_mapMapper.DpiX);
			}
			return GetDefaultSymbolSize(m_mapMapper.DpiX);
		}

		internal static int GetDefaultSymbolSize(float dpi)
		{
			return MappingHelper.ToIntPixels(m_defaultSymbolSize, dpi);
		}

		private static bool UseCustomTemplate(MapPoint mapPoint, bool hasScope)
		{
			if (mapPoint == null)
			{
				return false;
			}
			bool result = false;
			ReportBoolProperty useCustomPointTemplate = mapPoint.UseCustomPointTemplate;
			if (useCustomPointTemplate != null)
			{
				if (!useCustomPointTemplate.IsExpression)
				{
					result = useCustomPointTemplate.Value;
				}
				else if (hasScope)
				{
					result = mapPoint.Instance.UseCustomPointTemplate;
				}
			}
			return result;
		}

		internal static bool PolygonUseCustomTemplate(MapPolygon mapPolygon, bool hasScope)
		{
			if (mapPolygon == null)
			{
				return false;
			}
			bool result = false;
			ReportBoolProperty useCustomCenterPointTemplate = mapPolygon.UseCustomCenterPointTemplate;
			if (useCustomCenterPointTemplate != null)
			{
				if (!useCustomCenterPointTemplate.IsExpression)
				{
					result = useCustomCenterPointTemplate.Value;
				}
				else if (hasScope)
				{
					result = mapPolygon.Instance.UseCustomCenterPointTemplate;
				}
			}
			return result;
		}

		private TextAlignment GetTextAlignment(MapPointLabelPlacement placement)
		{
			switch (placement)
			{
			case MapPointLabelPlacement.Center:
				return TextAlignment.Center;
			case MapPointLabelPlacement.Left:
				return TextAlignment.Left;
			case MapPointLabelPlacement.Right:
				return TextAlignment.Right;
			case MapPointLabelPlacement.Top:
				return TextAlignment.Top;
			default:
				return TextAlignment.Bottom;
			}
		}
	}
}
