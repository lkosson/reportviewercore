using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapInstance : ReportItemInstance, IDynamicImageInstance
	{
		private float m_dpiX = 96f;

		private float m_dpiY = 96f;

		private double? m_widthOverride;

		private double? m_heightOverride;

		private MapAntiAliasing? m_antiAliasing;

		private MapTextAntiAliasingQuality? m_textAntiAliasingQuality;

		private double? m_shadowIntensity;

		private string m_tileLanguage;

		private bool m_tileLanguageEvaluated;

		private bool m_pageNameEvaluated;

		private string m_pageName;

		public MapAntiAliasing AntiAliasing
		{
			get
			{
				if (!m_antiAliasing.HasValue)
				{
					m_antiAliasing = MapDef.MapDef.EvaluateAntiAliasing(ReportScopeInstance, MapDef.RenderingContext.OdpContext);
				}
				return m_antiAliasing.Value;
			}
		}

		public MapTextAntiAliasingQuality TextAntiAliasingQuality
		{
			get
			{
				if (!m_textAntiAliasingQuality.HasValue)
				{
					m_textAntiAliasingQuality = MapDef.MapDef.EvaluateTextAntiAliasingQuality(ReportScopeInstance, MapDef.RenderingContext.OdpContext);
				}
				return m_textAntiAliasingQuality.Value;
			}
		}

		public double ShadowIntensity
		{
			get
			{
				if (!m_shadowIntensity.HasValue)
				{
					m_shadowIntensity = MapDef.MapDef.EvaluateShadowIntensity(ReportScopeInstance, MapDef.RenderingContext.OdpContext);
				}
				return m_shadowIntensity.Value;
			}
		}

		public string TileLanguage
		{
			get
			{
				if (!m_tileLanguageEvaluated)
				{
					m_tileLanguage = MapDef.MapDef.EvaluateTileLanguage(ReportScopeInstance, MapDef.RenderingContext.OdpContext);
					m_tileLanguageEvaluated = true;
				}
				return m_tileLanguage;
			}
		}

		public string PageName
		{
			get
			{
				if (!m_pageNameEvaluated)
				{
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_pageName = null;
					}
					else
					{
						m_pageNameEvaluated = true;
						Microsoft.ReportingServices.ReportIntermediateFormat.Map mapDef = MapDef.MapDef;
						ExpressionInfo pageName = mapDef.PageName;
						if (pageName != null)
						{
							if (pageName.IsExpression)
							{
								m_pageName = mapDef.EvaluatePageName(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext);
							}
							else
							{
								m_pageName = pageName.StringValue;
							}
						}
					}
				}
				return m_pageName;
			}
		}

		private Map MapDef => (Map)m_reportElementDef;

		private int WidthInPixels => MappingHelper.ToIntPixels(((ReportItem)m_reportElementDef).Width, m_dpiX);

		private int HeightInPixels => MappingHelper.ToIntPixels(((ReportItem)m_reportElementDef).Height, m_dpiX);

		internal MapInstance(Map reportItemDef)
			: base(reportItemDef)
		{
		}

		public void SetDpi(int xDpi, int yDpi)
		{
			m_dpiX = xDpi;
			m_dpiY = yDpi;
		}

		public void SetSize(double width, double height)
		{
			m_widthOverride = width;
			m_heightOverride = height;
		}

		public Stream GetImage()
		{
			bool hasImageMap;
			return GetImage(DynamicImageInstance.ImageType.PNG, out hasImageMap);
		}

		public Stream GetImage(DynamicImageInstance.ImageType type)
		{
			bool hasImageMap;
			return GetImage(type, out hasImageMap);
		}

		public Stream GetImage(out ActionInfoWithDynamicImageMapCollection actionImageMaps)
		{
			return GetImage(DynamicImageInstance.ImageType.PNG, out actionImageMaps);
		}

		public Stream GetImage(DynamicImageInstance.ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps)
		{
			try
			{
				GetImage(type, out actionImageMaps, out Stream image);
				return image;
			}
			catch (Exception exception)
			{
				actionImageMaps = null;
				return DynamicImageInstance.CreateExceptionImage(exception, WidthInPixels, HeightInPixels, m_dpiX, m_dpiY);
			}
		}

		private Stream GetImage(DynamicImageInstance.ImageType type, out bool hasImageMap)
		{
			ActionInfoWithDynamicImageMapCollection actionImageMaps;
			Stream image = GetImage(type, out actionImageMaps);
			hasImageMap = (actionImageMaps != null);
			return image;
		}

		private void GetImage(DynamicImageInstance.ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps, out Stream image)
		{
			using (IMapMapper mapMapper = MapMapperFactory.CreateMapMapperInstance((Map)m_reportElementDef, GetDefaultFontFamily()))
			{
				mapMapper.DpiX = m_dpiX;
				mapMapper.DpiY = m_dpiY;
				mapMapper.WidthOverride = m_widthOverride;
				mapMapper.HeightOverride = m_heightOverride;
				mapMapper.RenderMap();
				image = mapMapper.GetImage(type);
				actionImageMaps = mapMapper.GetImageMaps();
			}
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_antiAliasing = null;
			m_textAntiAliasingQuality = null;
			m_shadowIntensity = null;
			m_tileLanguage = null;
			m_tileLanguageEvaluated = false;
			m_pageNameEvaluated = false;
			m_pageName = null;
		}

		public Stream GetCoreXml()
		{
			using (IMapMapper mapMapper = MapMapperFactory.CreateMapMapperInstance((Map)m_reportElementDef, GetDefaultFontFamily()))
			{
				mapMapper.DpiX = m_dpiX;
				mapMapper.DpiY = m_dpiY;
				mapMapper.WidthOverride = m_widthOverride;
				mapMapper.HeightOverride = m_heightOverride;
				mapMapper.RenderMap();
				return mapMapper.GetCoreXml();
			}
		}
	}
}
