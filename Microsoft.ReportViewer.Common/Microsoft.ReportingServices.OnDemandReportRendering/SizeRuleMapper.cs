using Microsoft.Reporting.Map.WebForms;
using System;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class SizeRuleMapper : RuleMapper
	{
		internal SizeRuleMapper(MapSizeRule mapColorRule, VectorLayerMapper vectorLayerMapper, CoreSpatialElementManager coreSpatialElementManager)
			: base(mapColorRule, vectorLayerMapper, coreSpatialElementManager)
		{
		}

		internal void RenderSymbolRule(PointTemplateMapper symbolTemplateMapper, Color? color, MarkerStyle? markerStyle)
		{
			SymbolRule symbolRule = (SymbolRule)m_coreRule;
			SetRuleLegendProperties(symbolRule);
			SetRuleDistribution(symbolRule);
			SetSymbolRuleSizes(symbolRule.PredefinedSymbols);
			InitializePredefinedSymbols(symbolRule.PredefinedSymbols, symbolTemplateMapper, color, markerStyle);
		}

		private void InitializePredefinedSymbols(PredefinedSymbolCollection predefinedSymbols, PointTemplateMapper symbolTemplateMapper, Color? color, MarkerStyle? markerStyle)
		{
			foreach (PredefinedSymbol predefinedSymbol in predefinedSymbols)
			{
				if (color.HasValue)
				{
					predefinedSymbol.Color = color.Value;
				}
				if (markerStyle.HasValue)
				{
					predefinedSymbol.MarkerStyle = markerStyle.Value;
				}
				InitializePredefinedSymbols(predefinedSymbol, symbolTemplateMapper);
			}
		}

		private void SetSymbolRuleSizes(PredefinedSymbolCollection customSymbols)
		{
			int bucketCount = GetBucketCount();
			if (bucketCount == 0)
			{
				return;
			}
			double startSize = GetStartSize();
			double num = (GetEndSize() - startSize) / (double)bucketCount;
			MapBucketCollection mapBuckets = m_mapRule.MapBuckets;
			bool flag = GetDistributionType() == MapRuleDistributionType.Custom;
			for (int i = 0; i < bucketCount; i++)
			{
				PredefinedSymbol predefinedSymbol = new PredefinedSymbol();
				float num4 = predefinedSymbol.Width = (predefinedSymbol.Height = (int)Math.Round(startSize + (double)i * num));
				if (flag)
				{
					MapBucket bucket = mapBuckets[i];
					predefinedSymbol.FromValue = GetFromValue(bucket);
					predefinedSymbol.ToValue = GetToValue(bucket);
				}
				customSymbols.Add(predefinedSymbol);
			}
		}

		private double GetStartSize()
		{
			MapSizeRule mapSizeRule = (MapSizeRule)m_mapRule;
			ReportSizeProperty startSize = mapSizeRule.StartSize;
			ReportSize size = startSize.IsExpression ? mapSizeRule.Instance.StartSize : startSize.Value;
			return MappingHelper.ToPixels(size, m_mapMapper.DpiX);
		}

		private double GetEndSize()
		{
			MapSizeRule mapSizeRule = (MapSizeRule)m_mapRule;
			ReportSizeProperty startSize = mapSizeRule.StartSize;
			startSize = mapSizeRule.EndSize;
			ReportSize size = startSize.IsExpression ? mapSizeRule.Instance.EndSize : startSize.Value;
			return MappingHelper.ToPixels(size, m_mapMapper.DpiX);
		}

		internal virtual PathWidthRule CreatePathRule()
		{
			PathWidthRule pathWidthRule = (PathWidthRule)(m_coreRule = new PathWidthRule());
			pathWidthRule.Text = "";
			pathWidthRule.Category = m_mapVectorLayer.Name;
			pathWidthRule.Field = "";
			m_coreMap.PathRules.Add(pathWidthRule);
			SetRuleFieldName();
			return pathWidthRule;
		}

		internal void RenderLineRule(SpatialElementTemplateMapper spatialElementTemplateMapper, Color? color)
		{
			PathWidthRule pathWidthRule = (PathWidthRule)m_coreRule;
			pathWidthRule.UseCustomWidths = true;
			SetRuleLegendProperties(pathWidthRule);
			SetRuleDistribution(pathWidthRule);
			SetPathRuleSizes(pathWidthRule.CustomWidths);
		}

		private void SetPathRuleSizes(CustomWidthCollection customWidths)
		{
			int bucketCount = GetBucketCount();
			if (bucketCount == 0)
			{
				return;
			}
			double startSize = GetStartSize();
			double num = (GetEndSize() - startSize) / (double)bucketCount;
			MapBucketCollection mapBuckets = m_mapRule.MapBuckets;
			bool flag = GetDistributionType() == MapRuleDistributionType.Custom;
			for (int i = 0; i < bucketCount; i++)
			{
				CustomWidth customWidth = new CustomWidth();
				customWidth.Width = (int)Math.Round(startSize + (double)i * num);
				if (flag)
				{
					MapBucket bucket = mapBuckets[i];
					customWidth.FromValue = GetFromValue(bucket);
					customWidth.ToValue = GetToValue(bucket);
				}
				customWidths.Add(customWidth);
			}
		}

		internal override SymbolRule CreateSymbolRule()
		{
			SymbolRule symbolRule = base.CreateSymbolRule();
			symbolRule.AffectedAttributes = AffectedSymbolAttributes.SizeOnly;
			return symbolRule;
		}
	}
}
