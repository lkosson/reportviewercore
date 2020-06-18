using Microsoft.Reporting.Map.WebForms;
using Microsoft.ReportingServices.ReportProcessing;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class MarkerRuleMapper : RuleMapper
	{
		internal MarkerRuleMapper(MapMarkerRule mapColorRule, VectorLayerMapper vectorLayerMapper, CoreSpatialElementManager coreSpatialElementManager)
			: base(mapColorRule, vectorLayerMapper, coreSpatialElementManager)
		{
		}

		internal void RenderPointRule(PointTemplateMapper pointTemplateMapper, Color? color, int? size)
		{
			SymbolRule symbolRule = (SymbolRule)m_coreRule;
			SetRuleLegendProperties(symbolRule);
			SetRuleDistribution(symbolRule);
			SetSymbolRuleMarkers(symbolRule.PredefinedSymbols);
			InitializePredefinedSymbols(symbolRule.PredefinedSymbols, pointTemplateMapper, color, size);
		}

		private void InitializePredefinedSymbols(PredefinedSymbolCollection predefinedSymbols, PointTemplateMapper spatialElementTemplateMapper, Color? color, int? size)
		{
			foreach (PredefinedSymbol predefinedSymbol in predefinedSymbols)
			{
				if (color.HasValue)
				{
					predefinedSymbol.Color = color.Value;
				}
				if (size.HasValue)
				{
					float num3 = predefinedSymbol.Width = (predefinedSymbol.Height = size.Value);
				}
				InitializePredefinedSymbols(predefinedSymbol, spatialElementTemplateMapper);
			}
		}

		private void SetSymbolRuleMarkers(PredefinedSymbolCollection customSymbols)
		{
			int bucketCount = GetBucketCount();
			MapMarkerCollection mapMarkers = ((MapMarkerRule)m_mapRule).MapMarkers;
			int count = mapMarkers.Count;
			MapBucketCollection mapBuckets = m_mapRule.MapBuckets;
			bool flag = GetDistributionType() == MapRuleDistributionType.Custom;
			for (int i = 0; i < bucketCount; i++)
			{
				PredefinedSymbol predefinedSymbol = new PredefinedSymbol();
				if (i < count)
				{
					RenderMarker(predefinedSymbol, mapMarkers[i]);
				}
				else
				{
					predefinedSymbol.MarkerStyle = MarkerStyle.None;
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

		private void RenderMarker(PredefinedSymbol customSymbol, MapMarker mapMarker)
		{
			MapMarkerStyle markerStyle = MapMapper.GetMarkerStyle(mapMarker, hasScope: true);
			if (markerStyle != MapMarkerStyle.Image)
			{
				customSymbol.MarkerStyle = MapMapper.GetMarkerStyle(markerStyle);
				return;
			}
			MapMarkerImage mapMarkerImage = mapMarker.MapMarkerImage;
			if (mapMarkerImage == null)
			{
				throw new RenderingObjectModelException(RPRes.rsMapLayerMissingProperty(RPRes.rsObjectTypeMap, m_mapRule.MapDef.Name, m_mapVectorLayer.Name, "MapMarkerImage"));
			}
			customSymbol.Image = m_mapMapper.AddImage(mapMarkerImage);
			customSymbol.ImageResizeMode = m_mapMapper.GetImageResizeMode(mapMarkerImage);
			customSymbol.ImageTransColor = m_mapMapper.GetImageTransColor(mapMarkerImage);
		}

		internal override SymbolRule CreateSymbolRule()
		{
			SymbolRule symbolRule = base.CreateSymbolRule();
			symbolRule.AffectedAttributes = AffectedSymbolAttributes.MarkerOnly;
			return symbolRule;
		}
	}
}
