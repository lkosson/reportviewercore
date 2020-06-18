using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Map : ReportItem, IROMActionOwner, IPageBreakItem
	{
		private MapDataRegionCollection m_mapDataRegions;

		private PageBreak m_pageBreak;

		private ReportStringProperty m_pageName;

		private MapViewport m_mapViewport;

		private MapLayerCollection m_mapLayers;

		private MapLegendCollection m_mapLegends;

		private MapTitleCollection m_mapTitles;

		private MapDistanceScale m_mapDistanceScale;

		private MapColorScale m_mapColorScale;

		private MapBorderSkin m_mapBorderSkin;

		private ReportEnumProperty<MapAntiAliasing> m_antiAliasing;

		private ReportEnumProperty<MapTextAntiAliasingQuality> m_textAntiAliasingQuality;

		private ReportDoubleProperty m_shadowIntensity;

		private ReportStringProperty m_tileLanguage;

		private ActionInfo m_actionInfo;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Map MapDef => (Microsoft.ReportingServices.ReportIntermediateFormat.Map)m_reportItemDef;

		public MapDataRegionCollection MapDataRegions
		{
			get
			{
				if (m_mapDataRegions == null && MapDef.MapDataRegions != null)
				{
					m_mapDataRegions = new MapDataRegionCollection(this);
				}
				return m_mapDataRegions;
			}
		}

		public new MapInstance Instance => (MapInstance)GetOrCreateInstance();

		public MapViewport MapViewport
		{
			get
			{
				if (m_mapViewport == null && MapDef.MapViewport != null)
				{
					m_mapViewport = new MapViewport(MapDef.MapViewport, this);
				}
				return m_mapViewport;
			}
		}

		public MapLayerCollection MapLayers
		{
			get
			{
				if (m_mapLayers == null && MapDef.MapLayers != null)
				{
					m_mapLayers = new MapLayerCollection(this);
				}
				return m_mapLayers;
			}
		}

		public MapLegendCollection MapLegends
		{
			get
			{
				if (m_mapLegends == null && MapDef.MapLegends != null)
				{
					m_mapLegends = new MapLegendCollection(this);
				}
				return m_mapLegends;
			}
		}

		public MapTitleCollection MapTitles
		{
			get
			{
				if (m_mapTitles == null && MapDef.MapTitles != null)
				{
					m_mapTitles = new MapTitleCollection(this);
				}
				return m_mapTitles;
			}
		}

		public MapDistanceScale MapDistanceScale
		{
			get
			{
				if (m_mapDistanceScale == null && MapDef.MapDistanceScale != null)
				{
					m_mapDistanceScale = new MapDistanceScale(MapDef.MapDistanceScale, this);
				}
				return m_mapDistanceScale;
			}
		}

		public MapColorScale MapColorScale
		{
			get
			{
				if (m_mapColorScale == null && MapDef.MapColorScale != null)
				{
					m_mapColorScale = new MapColorScale(MapDef.MapColorScale, this);
				}
				return m_mapColorScale;
			}
		}

		public MapBorderSkin MapBorderSkin
		{
			get
			{
				if (m_mapBorderSkin == null && MapDef.MapBorderSkin != null)
				{
					m_mapBorderSkin = new MapBorderSkin(MapDef.MapBorderSkin, this);
				}
				return m_mapBorderSkin;
			}
		}

		public ReportEnumProperty<MapAntiAliasing> AntiAliasing
		{
			get
			{
				if (m_antiAliasing == null && MapDef.AntiAliasing != null)
				{
					m_antiAliasing = new ReportEnumProperty<MapAntiAliasing>(MapDef.AntiAliasing.IsExpression, MapDef.AntiAliasing.OriginalText, EnumTranslator.TranslateMapAntiAliasing(MapDef.AntiAliasing.StringValue, null));
				}
				return m_antiAliasing;
			}
		}

		public ReportEnumProperty<MapTextAntiAliasingQuality> TextAntiAliasingQuality
		{
			get
			{
				if (m_textAntiAliasingQuality == null && MapDef.TextAntiAliasingQuality != null)
				{
					m_textAntiAliasingQuality = new ReportEnumProperty<MapTextAntiAliasingQuality>(MapDef.TextAntiAliasingQuality.IsExpression, MapDef.TextAntiAliasingQuality.OriginalText, EnumTranslator.TranslateMapTextAntiAliasingQuality(MapDef.TextAntiAliasingQuality.StringValue, null));
				}
				return m_textAntiAliasingQuality;
			}
		}

		public ReportDoubleProperty ShadowIntensity
		{
			get
			{
				if (m_shadowIntensity == null && MapDef.ShadowIntensity != null)
				{
					m_shadowIntensity = new ReportDoubleProperty(MapDef.ShadowIntensity);
				}
				return m_shadowIntensity;
			}
		}

		public ReportStringProperty TileLanguage
		{
			get
			{
				if (m_tileLanguage == null && MapDef.TileLanguage != null)
				{
					m_tileLanguage = new ReportStringProperty(MapDef.TileLanguage);
				}
				return m_tileLanguage;
			}
		}

		public int MaximumSpatialElementCount => MapDef.MaximumSpatialElementCount;

		public int MaximumTotalPointCount => MapDef.MaximumTotalPointCount;

		public PageBreak PageBreak
		{
			get
			{
				if (m_pageBreak == null)
				{
					IPageBreakOwner pageBreakOwner = (Microsoft.ReportingServices.ReportIntermediateFormat.Map)m_reportItemDef;
					m_pageBreak = new PageBreak(m_renderingContext, ReportScope, pageBreakOwner);
				}
				return m_pageBreak;
			}
		}

		public ReportStringProperty PageName
		{
			get
			{
				if (m_pageName == null)
				{
					if (m_isOldSnapshot)
					{
						m_pageName = new ReportStringProperty();
					}
					else
					{
						m_pageName = new ReportStringProperty(((Microsoft.ReportingServices.ReportIntermediateFormat.Map)m_reportItemDef).PageName);
					}
				}
				return m_pageName;
			}
		}

		[Obsolete("Use PageBreak.BreakLocation instead.")]
		PageBreakLocation IPageBreakItem.PageBreakLocation
		{
			get
			{
				if (((IPageBreakOwner)base.ReportItemDef).PageBreak != null)
				{
					PageBreak pageBreak = PageBreak;
					if (pageBreak.HasEnabledInstance)
					{
						return pageBreak.BreakLocation;
					}
				}
				return PageBreakLocation.None;
			}
		}

		string IROMActionOwner.UniqueName => MapDef.UniqueName;

		List<string> IROMActionOwner.FieldsUsedInValueExpression => null;

		public bool SpecialBorderHandling
		{
			get
			{
				MapBorderSkin mapBorderSkin = MapBorderSkin;
				if (mapBorderSkin == null)
				{
					return false;
				}
				ReportEnumProperty<MapBorderSkinType> mapBorderSkinType = mapBorderSkin.MapBorderSkinType;
				if (mapBorderSkinType == null)
				{
					return false;
				}
				MapBorderSkinType mapBorderSkinType2 = mapBorderSkinType.IsExpression ? mapBorderSkin.Instance.MapBorderSkinType : mapBorderSkinType.Value;
				return mapBorderSkinType2 != MapBorderSkinType.None;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.Action action = MapDef.Action;
					if (action != null)
					{
						m_actionInfo = new ActionInfo(base.RenderingContext, ReportScope, action, m_reportItemDef, this, m_reportItemDef.ObjectType, m_reportItemDef.Name, this);
					}
				}
				return m_actionInfo;
			}
		}

		internal Map(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.Map reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (m_instance == null)
			{
				m_instance = new MapInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContextChildren()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_mapDataRegions != null)
			{
				m_mapDataRegions.SetNewContext();
			}
			if (m_mapViewport != null)
			{
				m_mapViewport.SetNewContext();
			}
			if (m_mapLayers != null)
			{
				m_mapLayers.SetNewContext();
			}
			if (m_mapLegends != null)
			{
				m_mapLegends.SetNewContext();
			}
			if (m_mapTitles != null)
			{
				m_mapTitles.SetNewContext();
			}
			if (m_mapDistanceScale != null)
			{
				m_mapDistanceScale.SetNewContext();
			}
			if (m_mapColorScale != null)
			{
				m_mapColorScale.SetNewContext();
			}
			if (m_mapBorderSkin != null)
			{
				m_mapBorderSkin.SetNewContext();
			}
			if (m_actionInfo != null)
			{
				m_actionInfo.SetNewContext();
			}
			if (m_pageBreak != null)
			{
				m_pageBreak.SetNewContext();
			}
		}
	}
}
