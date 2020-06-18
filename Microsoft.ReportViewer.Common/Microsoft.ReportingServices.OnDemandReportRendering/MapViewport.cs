using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapViewport : MapSubItem
	{
		private ReportEnumProperty<MapCoordinateSystem> m_mapCoordinateSystem;

		private ReportEnumProperty<MapProjection> m_mapProjection;

		private ReportDoubleProperty m_projectionCenterX;

		private ReportDoubleProperty m_projectionCenterY;

		private MapLimits m_mapLimits;

		private MapView m_mapView;

		private ReportDoubleProperty m_maximumZoom;

		private ReportDoubleProperty m_minimumZoom;

		private ReportSizeProperty m_contentMargin;

		private MapGridLines m_mapMeridians;

		private MapGridLines m_mapParallels;

		private ReportBoolProperty m_gridUnderContent;

		private ReportDoubleProperty m_simplificationResolution;

		public ReportEnumProperty<MapCoordinateSystem> MapCoordinateSystem
		{
			get
			{
				if (m_mapCoordinateSystem == null && MapViewportDef.MapCoordinateSystem != null)
				{
					m_mapCoordinateSystem = new ReportEnumProperty<MapCoordinateSystem>(MapViewportDef.MapCoordinateSystem.IsExpression, MapViewportDef.MapCoordinateSystem.OriginalText, EnumTranslator.TranslateMapCoordinateSystem(MapViewportDef.MapCoordinateSystem.StringValue, null));
				}
				return m_mapCoordinateSystem;
			}
		}

		public ReportEnumProperty<MapProjection> MapProjection
		{
			get
			{
				if (m_mapProjection == null && MapViewportDef.MapProjection != null)
				{
					m_mapProjection = new ReportEnumProperty<MapProjection>(MapViewportDef.MapProjection.IsExpression, MapViewportDef.MapProjection.OriginalText, EnumTranslator.TranslateMapProjection(MapViewportDef.MapProjection.StringValue, null));
				}
				return m_mapProjection;
			}
		}

		public ReportDoubleProperty ProjectionCenterX
		{
			get
			{
				if (m_projectionCenterX == null && MapViewportDef.ProjectionCenterX != null)
				{
					m_projectionCenterX = new ReportDoubleProperty(MapViewportDef.ProjectionCenterX);
				}
				return m_projectionCenterX;
			}
		}

		public ReportDoubleProperty ProjectionCenterY
		{
			get
			{
				if (m_projectionCenterY == null && MapViewportDef.ProjectionCenterY != null)
				{
					m_projectionCenterY = new ReportDoubleProperty(MapViewportDef.ProjectionCenterY);
				}
				return m_projectionCenterY;
			}
		}

		public MapLimits MapLimits
		{
			get
			{
				if (m_mapLimits == null && MapViewportDef.MapLimits != null)
				{
					m_mapLimits = new MapLimits(MapViewportDef.MapLimits, m_map);
				}
				return m_mapLimits;
			}
		}

		public MapView MapView
		{
			get
			{
				if (m_mapView == null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.MapView mapView = MapViewportDef.MapView;
					if (mapView != null)
					{
						if (mapView is Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView)
						{
							m_mapView = new MapCustomView((Microsoft.ReportingServices.ReportIntermediateFormat.MapCustomView)MapViewportDef.MapView, m_map);
						}
						else if (mapView is Microsoft.ReportingServices.ReportIntermediateFormat.MapElementView)
						{
							m_mapView = new MapElementView((Microsoft.ReportingServices.ReportIntermediateFormat.MapElementView)MapViewportDef.MapView, m_map);
						}
						if (mapView is Microsoft.ReportingServices.ReportIntermediateFormat.MapDataBoundView)
						{
							m_mapView = new MapDataBoundView((Microsoft.ReportingServices.ReportIntermediateFormat.MapDataBoundView)MapViewportDef.MapView, m_map);
						}
					}
				}
				return m_mapView;
			}
		}

		public ReportDoubleProperty MaximumZoom
		{
			get
			{
				if (m_maximumZoom == null && MapViewportDef.MaximumZoom != null)
				{
					m_maximumZoom = new ReportDoubleProperty(MapViewportDef.MaximumZoom);
				}
				return m_maximumZoom;
			}
		}

		public ReportDoubleProperty MinimumZoom
		{
			get
			{
				if (m_minimumZoom == null && MapViewportDef.MinimumZoom != null)
				{
					m_minimumZoom = new ReportDoubleProperty(MapViewportDef.MinimumZoom);
				}
				return m_minimumZoom;
			}
		}

		public ReportSizeProperty ContentMargin
		{
			get
			{
				if (m_contentMargin == null && MapViewportDef.ContentMargin != null)
				{
					m_contentMargin = new ReportSizeProperty(MapViewportDef.ContentMargin);
				}
				return m_contentMargin;
			}
		}

		public ReportDoubleProperty SimplificationResolution
		{
			get
			{
				if (m_simplificationResolution == null && MapViewportDef.SimplificationResolution != null)
				{
					m_simplificationResolution = new ReportDoubleProperty(MapViewportDef.SimplificationResolution);
				}
				return m_simplificationResolution;
			}
		}

		public MapGridLines MapMeridians
		{
			get
			{
				if (m_mapMeridians == null && MapViewportDef.MapMeridians != null)
				{
					m_mapMeridians = new MapGridLines(MapViewportDef.MapMeridians, m_map);
				}
				return m_mapMeridians;
			}
		}

		public MapGridLines MapParallels
		{
			get
			{
				if (m_mapParallels == null && MapViewportDef.MapParallels != null)
				{
					m_mapParallels = new MapGridLines(MapViewportDef.MapParallels, m_map);
				}
				return m_mapParallels;
			}
		}

		public ReportBoolProperty GridUnderContent
		{
			get
			{
				if (m_gridUnderContent == null && MapViewportDef.GridUnderContent != null)
				{
					m_gridUnderContent = new ReportBoolProperty(MapViewportDef.GridUnderContent);
				}
				return m_gridUnderContent;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport MapViewportDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport)m_defObject;

		public new MapViewportInstance Instance => (MapViewportInstance)GetInstance();

		internal MapViewport(Microsoft.ReportingServices.ReportIntermediateFormat.MapViewport defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapSubItemInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapViewportInstance(this);
			}
			return (MapSubItemInstance)m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_mapLimits != null)
			{
				m_mapLimits.SetNewContext();
			}
			if (m_mapView != null)
			{
				m_mapView.SetNewContext();
			}
			if (m_mapMeridians != null)
			{
				m_mapMeridians.SetNewContext();
			}
			if (m_mapParallels != null)
			{
				m_mapParallels.SetNewContext();
			}
		}
	}
}
