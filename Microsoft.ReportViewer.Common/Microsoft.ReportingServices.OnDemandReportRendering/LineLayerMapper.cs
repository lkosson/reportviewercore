using Microsoft.Reporting.Map.WebForms;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class LineLayerMapper : VectorLayerMapper
	{
		private CorePathManager m_pathManager;

		private ColorRuleMapper m_lineColorRuleMapper;

		private SizeRuleMapper m_lineSizeRuleMapper;

		private LineTemplateMapper m_lineTemplateMapper;

		protected override ISpatialElementCollection SpatialElementCollection => MapLineLayer.MapLines;

		private MapLineLayer MapLineLayer => (MapLineLayer)m_mapVectorLayer;

		internal LineLayerMapper(MapLineLayer mapLineLayer, MapControl coreMap, MapMapper mapMapper)
			: base(mapLineLayer, coreMap, mapMapper)
		{
			if (mapLineLayer.MapLineTemplate != null)
			{
				m_lineTemplateMapper = new LineTemplateMapper(m_mapMapper, this, MapLineLayer);
			}
		}

		protected override CoreSpatialElementManager GetSpatialElementManager()
		{
			if (m_pathManager == null)
			{
				m_pathManager = new CorePathManager(m_coreMap, m_mapVectorLayer);
			}
			return m_pathManager;
		}

		internal bool HasColorRule(Path path)
		{
			if (!HasColorRule())
			{
				return false;
			}
			return m_lineColorRuleMapper.HasDataValue(path);
		}

		private bool HasColorRule()
		{
			MapColorRule mapColorRule = MapLineLayer.MapLineRules?.MapColorRule;
			return mapColorRule != null;
		}

		internal bool HasSizeRule(Path path)
		{
			if (!HasSizeRule())
			{
				return false;
			}
			return m_lineSizeRuleMapper.HasDataValue(path);
		}

		private bool HasSizeRule()
		{
			MapSizeRule mapSizeRule = MapLineLayer.MapLineRules?.MapSizeRule;
			return mapSizeRule != null;
		}

		protected override void CreateRules()
		{
			MapLineRules mapLineRules = MapLineLayer.MapLineRules;
			if (mapLineRules != null)
			{
				if (mapLineRules.MapColorRule != null)
				{
					m_lineColorRuleMapper = new ColorRuleMapper(mapLineRules.MapColorRule, this, GetSpatialElementManager());
					m_lineColorRuleMapper.CreatePathRule();
				}
				if (mapLineRules.MapSizeRule != null)
				{
					m_lineSizeRuleMapper = new SizeRuleMapper(mapLineRules.MapSizeRule, this, GetSpatialElementManager());
					m_lineSizeRuleMapper.CreatePathRule();
				}
			}
		}

		protected override void RenderRules()
		{
			MapLineRules mapLineRules = MapLineLayer.MapLineRules;
			if (mapLineRules != null)
			{
				if (mapLineRules.MapColorRule != null)
				{
					m_lineColorRuleMapper.RenderLineRule(m_lineTemplateMapper, GetLegendSize());
				}
				if (mapLineRules.MapSizeRule != null)
				{
					m_lineSizeRuleMapper.RenderLineRule(m_lineTemplateMapper, GetLegendColor());
				}
			}
		}

		private Color? GetLegendColor()
		{
			return m_lineTemplateMapper.GetBackgroundColor(hasScope: false);
		}

		private int? GetLegendSize()
		{
			if (m_lineTemplateMapper == null)
			{
				return LineTemplateMapper.GetDefaultSize(m_mapMapper.DpiX);
			}
			return m_lineTemplateMapper.GetSize(MapLineLayer.MapLineTemplate, hasScope: false);
		}

		protected override void RenderSpatialElement(SpatialElementInfo spatialElementInfo, bool hasScope)
		{
			InitializeSpatialElement(spatialElementInfo.CoreSpatialElement);
			if (hasScope)
			{
				RenderLineRuleFields((Path)spatialElementInfo.CoreSpatialElement);
			}
			RenderLineTemplate((MapLine)spatialElementInfo.MapSpatialElement, (Path)spatialElementInfo.CoreSpatialElement, hasScope);
		}

		protected void RenderLineRuleFields(Path corePath)
		{
			if (m_lineColorRuleMapper != null)
			{
				m_lineColorRuleMapper.SetRuleFieldValue(corePath);
			}
			if (m_lineSizeRuleMapper != null)
			{
				m_lineSizeRuleMapper.SetRuleFieldValue(corePath);
			}
		}

		private void RenderLineTemplate(MapLine mapLine, Path path, bool hasScope)
		{
			m_lineTemplateMapper.Render(mapLine, path, hasScope);
		}

		internal override bool IsValidSpatialElement(ISpatialElement spatialElement)
		{
			return spatialElement is Path;
		}

		internal override void OnSpatialElementAdded(ISpatialElement spatialElement)
		{
			m_mapMapper.Simplify((Path)spatialElement);
		}
	}
}
