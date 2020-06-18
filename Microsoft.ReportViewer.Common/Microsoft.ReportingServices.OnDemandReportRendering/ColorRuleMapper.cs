using Microsoft.Reporting.Map.WebForms;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class ColorRuleMapper : RuleMapper
	{
		private static Color m_defaultFromColor = Color.Green;

		private static Color m_defaultMiddleColor = Color.Yellow;

		private static Color m_defaultToColor = Color.Red;

		internal ColorRuleMapper(MapColorRule mapColorRule, VectorLayerMapper vectorLayerMapper, CoreSpatialElementManager coreSpatialElementManager)
			: base(mapColorRule, vectorLayerMapper, coreSpatialElementManager)
		{
		}

		internal PathRule CreatePathRule()
		{
			PathRule pathRule = (PathRule)(m_coreRule = new PathRule());
			pathRule.BorderColor = Color.Empty;
			pathRule.Text = "";
			pathRule.Category = m_mapVectorLayer.Name;
			pathRule.Field = "";
			m_coreMap.PathRules.Add(pathRule);
			SetRuleFieldName();
			return pathRule;
		}

		internal void RenderPolygonRule(PolygonTemplateMapper shapeTemplateMapper)
		{
			ShapeRule shapeRule = (ShapeRule)m_coreRule;
			SetRuleLegendProperties(shapeRule);
			SetRuleDistribution(shapeRule);
			shapeRule.ShowInColorSwatch = GetShowInColorScale();
			if (m_mapRule is MapColorRangeRule)
			{
				RenderPolygonColorRangeRule(shapeRule);
			}
			else if (m_mapRule is MapColorPaletteRule)
			{
				RenderPolygonColorPaletteRule(shapeRule);
			}
			else
			{
				RenderPolygonCustomColorRule(shapeRule);
			}
			InitializeCustomColors(shapeRule.CustomColors, shapeTemplateMapper);
		}

		internal void RenderSymbolRule(PointTemplateMapper symbolTemplateMapper, int? size, MarkerStyle? markerStyle)
		{
			SymbolRule symbolRule = (SymbolRule)m_coreRule;
			SetRuleLegendProperties(symbolRule);
			SetRuleDistribution(symbolRule);
			symbolRule.ShowInColorSwatch = GetShowInColorScale();
			if (m_mapRule is MapColorRangeRule)
			{
				RenderSymbolColorRangeRule(symbolRule);
			}
			else if (m_mapRule is MapColorPaletteRule)
			{
				RenderSymbolColorPaletteRule(symbolRule);
			}
			else
			{
				SetSymbolRuleColors(GetCustomColors(((MapCustomColorRule)m_mapRule).MapCustomColors), symbolRule.PredefinedSymbols);
			}
			InitializePredefinedSymbols(symbolRule.PredefinedSymbols, symbolTemplateMapper, size, markerStyle);
		}

		private void InitializePredefinedSymbols(PredefinedSymbolCollection predefinedSymbols, PointTemplateMapper symbolTemplateMapper, int? size, MarkerStyle? markerStyle)
		{
			foreach (PredefinedSymbol predefinedSymbol in predefinedSymbols)
			{
				if (size.HasValue)
				{
					float num3 = predefinedSymbol.Width = (predefinedSymbol.Height = size.Value);
				}
				if (markerStyle.HasValue)
				{
					predefinedSymbol.MarkerStyle = markerStyle.Value;
				}
				InitializePredefinedSymbols(predefinedSymbol, symbolTemplateMapper);
			}
		}

		internal void RenderLineRule(LineTemplateMapper pathTemplateMapper, int? size)
		{
			PathRule pathRule = (PathRule)m_coreRule;
			SetRuleLegendProperties(pathRule);
			SetRuleDistribution(pathRule);
			pathRule.ShowInColorSwatch = GetShowInColorScale();
			if (m_mapRule is MapColorRangeRule)
			{
				RenderLineColorRangeRule(pathRule);
			}
			else if (m_mapRule is MapColorPaletteRule)
			{
				RenderLineColorPaletteRule(pathRule);
			}
			else
			{
				RenderLineCustomColorRule(pathRule);
			}
			InitializePathRule(pathRule, pathTemplateMapper, size);
		}

		private void InitializePathRule(PathRule pathRule, LineTemplateMapper pathTemplateMapper, int? size)
		{
			InitializeCustomColors(pathRule.CustomColors, pathTemplateMapper);
		}

		private void InitializeCustomColors(CustomColorCollection customColors, SpatialElementTemplateMapper spatialEementTemplateMapper)
		{
			foreach (CustomColor customColor in customColors)
			{
				customColor.BorderColor = spatialEementTemplateMapper.GetBorderColor(hasScope: false);
				customColor.SecondaryColor = spatialEementTemplateMapper.GetBackGradientEndColor(hasScope: false);
				customColor.GradientType = spatialEementTemplateMapper.GetGradientType(hasScope: false);
				customColor.HatchStyle = spatialEementTemplateMapper.GetHatchStyle(hasScope: false);
				customColor.LegendText = "";
				customColor.Text = "";
			}
		}

		private void RenderLineColorRangeRule(PathRule pathRule)
		{
			MapColorRangeRule colorRangeRule = (MapColorRangeRule)m_mapRule;
			RenderPathCustomColors(pathRule, ColoringMode.ColorRange, MapColorPalette.Dundas, GetFromColor(colorRangeRule), GetMiddleColor(colorRangeRule), GetToColor(colorRangeRule));
		}

		private void RenderLineColorPaletteRule(PathRule pathRule)
		{
			RenderPathCustomColors(pathRule, ColoringMode.DistinctColors, GetColorPalette(), Color.Empty, Color.Empty, Color.Empty);
		}

		private void RenderLineCustomColorRule(PathRule pathRule)
		{
			pathRule.UseCustomColors = true;
			SetRuleColors(GetCustomColors(((MapCustomColorRule)m_mapRule).MapCustomColors), pathRule.CustomColors);
		}

		private Color GetFromColor(MapColorRangeRule colorRangeRule)
		{
			ReportColorProperty startColor = colorRangeRule.StartColor;
			Color color = m_defaultFromColor;
			if (startColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(startColor, ref color))
				{
					return color;
				}
				if (colorRangeRule.Instance.StartColor != null)
				{
					return colorRangeRule.Instance.StartColor.ToColor();
				}
			}
			return color;
		}

		private Color GetMiddleColor(MapColorRangeRule colorRangeRule)
		{
			ReportColorProperty middleColor = colorRangeRule.MiddleColor;
			Color color = m_defaultMiddleColor;
			if (middleColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(middleColor, ref color))
				{
					return color;
				}
				if (colorRangeRule.Instance.MiddleColor != null)
				{
					return colorRangeRule.Instance.MiddleColor.ToColor();
				}
			}
			return color;
		}

		private Color GetToColor(MapColorRangeRule colorRangeRule)
		{
			ReportColorProperty endColor = colorRangeRule.EndColor;
			Color color = m_defaultToColor;
			if (endColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(endColor, ref color))
				{
					return color;
				}
				if (colorRangeRule.Instance.EndColor != null)
				{
					return colorRangeRule.Instance.EndColor.ToColor();
				}
			}
			return color;
		}

		private bool GetShowInColorScale()
		{
			ReportBoolProperty showInColorScale = ((MapColorRule)m_mapRule).ShowInColorScale;
			if (showInColorScale != null)
			{
				if (!showInColorScale.IsExpression)
				{
					return showInColorScale.Value;
				}
				return ((MapColorRule)m_mapRule).Instance.ShowInColorScale;
			}
			return false;
		}

		private MapColorPalette GetColorPalette()
		{
			MapColorPaletteRule mapColorPaletteRule = (MapColorPaletteRule)m_mapRule;
			ReportEnumProperty<MapPalette> palette = mapColorPaletteRule.Palette;
			if (palette != null)
			{
				if (!palette.IsExpression)
				{
					return GetMapColorPalette(palette.Value);
				}
				return GetMapColorPalette(mapColorPaletteRule.Instance.Palette);
			}
			return MapColorPalette.Random;
		}

		private Color[] GetCustomColors(MapCustomColorCollection mapCustomColors)
		{
			Color[] array = new Color[mapCustomColors.Count];
			for (int i = 0; i < mapCustomColors.Count; i++)
			{
				array[i] = GetCustomColor(mapCustomColors[i]);
			}
			return array;
		}

		private void SetRuleColors(Color[] colorRange, CustomColorCollection customColors)
		{
			MapBucketCollection mapBuckets = m_mapRule.MapBuckets;
			bool flag = GetDistributionType() == MapRuleDistributionType.Custom;
			int bucketCount = GetBucketCount();
			for (int i = 0; i < bucketCount; i++)
			{
				CustomColor customColor = new CustomColor();
				if (i < colorRange.Length)
				{
					customColor.Color = colorRange[i];
				}
				else
				{
					customColor.Color = Color.Empty;
				}
				if (flag)
				{
					MapBucket bucket = mapBuckets[i];
					customColor.FromValue = GetFromValue(bucket);
					customColor.ToValue = GetToValue(bucket);
				}
				customColors.Add(customColor);
			}
		}

		private void SetSymbolRuleColors(Color[] colorRange, PredefinedSymbolCollection customSymbols)
		{
			MapBucketCollection mapBuckets = m_mapRule.MapBuckets;
			bool flag = GetDistributionType() == MapRuleDistributionType.Custom;
			int bucketCount = GetBucketCount();
			for (int i = 0; i < bucketCount; i++)
			{
				PredefinedSymbol predefinedSymbol = new PredefinedSymbol();
				if (i < colorRange.Length)
				{
					predefinedSymbol.Color = colorRange[i];
				}
				else
				{
					predefinedSymbol.Color = Color.Empty;
				}
				if (flag)
				{
					MapBucket bucket = mapBuckets[i];
					predefinedSymbol.FromValue = GetFromValue(bucket);
					predefinedSymbol.ToValue = GetToValue(bucket);
				}
				customSymbols.Add(predefinedSymbol);
			}
		}

		private void RenderPolygonColorRangeRule(ShapeRule shapeRule)
		{
			MapColorRangeRule colorRangeRule = (MapColorRangeRule)m_mapRule;
			RenderShapeCustomColors(shapeRule, ColoringMode.ColorRange, MapColorPalette.Dundas, GetFromColor(colorRangeRule), GetMiddleColor(colorRangeRule), GetToColor(colorRangeRule));
		}

		private void RenderPolygonColorPaletteRule(ShapeRule shapeRule)
		{
			RenderShapeCustomColors(shapeRule, ColoringMode.DistinctColors, GetColorPalette(), Color.Empty, Color.Empty, Color.Empty);
		}

		private void RenderPolygonCustomColorRule(ShapeRule shapeRule)
		{
			shapeRule.UseCustomColors = true;
			SetRuleColors(GetCustomColors(((MapCustomColorRule)m_mapRule).MapCustomColors), shapeRule.CustomColors);
		}

		private void RenderSymbolColorRangeRule(SymbolRule symbolRule)
		{
			MapColorRangeRule colorRangeRule = (MapColorRangeRule)m_mapRule;
			SetSymbolRuleColors(symbolRule.GetColors(ColoringMode.ColorRange, MapColorPalette.Dundas, GetFromColor(colorRangeRule), GetMiddleColor(colorRangeRule), GetToColor(colorRangeRule), GetBucketCount()), symbolRule.PredefinedSymbols);
		}

		private void RenderSymbolColorPaletteRule(SymbolRule symbolRule)
		{
			SetSymbolRuleColors(symbolRule.GetColors(ColoringMode.DistinctColors, GetColorPalette(), Color.Empty, Color.Empty, Color.Empty, GetBucketCount()), symbolRule.PredefinedSymbols);
		}

		private void RenderShapeCustomColors(ShapeRule shapeRule, ColoringMode coloringMode, MapColorPalette palette, Color fromColor, Color middleColor, Color toColor)
		{
			shapeRule.UseCustomColors = true;
			SetRuleColors(shapeRule.GetColors(coloringMode, palette, fromColor, middleColor, toColor, GetBucketCount()), shapeRule.CustomColors);
		}

		private void RenderPathCustomColors(PathRule pathRule, ColoringMode coloringMode, MapColorPalette palette, Color fromColor, Color middleColor, Color toColor)
		{
			pathRule.UseCustomColors = true;
			SetRuleColors(pathRule.GetColors(coloringMode, palette, fromColor, middleColor, toColor, GetBucketCount()), pathRule.CustomColors);
		}

		private MapColorPalette GetMapColorPalette(MapPalette palette)
		{
			switch (palette)
			{
			case MapPalette.BrightPastel:
				return MapColorPalette.Dundas;
			case MapPalette.Light:
				return MapColorPalette.Light;
			case MapPalette.SemiTransparent:
				return MapColorPalette.SemiTransparent;
			case MapPalette.Pacific:
				return MapColorPalette.Pacific;
			default:
				return MapColorPalette.Random;
			}
		}

		private Color GetCustomColor(MapCustomColor mapCustomColor)
		{
			ReportColorProperty color = mapCustomColor.Color;
			Color color2 = Color.Empty;
			if (color != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(color, ref color2))
				{
					return color2;
				}
				ReportColor color3 = mapCustomColor.Instance.Color;
				if (color3 != null)
				{
					return color3.ToColor();
				}
			}
			return color2;
		}

		internal override SymbolRule CreateSymbolRule()
		{
			SymbolRule symbolRule = base.CreateSymbolRule();
			symbolRule.AffectedAttributes = AffectedSymbolAttributes.ColorOnly;
			return symbolRule;
		}
	}
}
