using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapExprHost : ReportItemExprHost
	{
		public MapViewportExprHost MapViewportHost;

		[CLSCompliant(false)]
		protected IList<MapPolygonLayerExprHost> m_mapPolygonLayersHostsRemotable;

		[CLSCompliant(false)]
		protected IList<MapPointLayerExprHost> m_mapPointLayersHostsRemotable;

		[CLSCompliant(false)]
		protected IList<MapLineLayerExprHost> m_mapLineLayersHostsRemotable;

		[CLSCompliant(false)]
		protected IList<MapTileLayerExprHost> m_mapTileLayersHostsRemotable;

		[CLSCompliant(false)]
		protected IList<MapLegendExprHost> m_mapLegendsHostsRemotable;

		[CLSCompliant(false)]
		protected IList<MapTitleExprHost> m_mapTitlesHostsRemotable;

		public MapDistanceScaleExprHost MapDistanceScaleHost;

		public MapColorScaleExprHost MapColorScaleHost;

		public MapBorderSkinExprHost MapBorderSkinHost;

		internal IList<MapPolygonLayerExprHost> MapPolygonLayersHostsRemotable => m_mapPolygonLayersHostsRemotable;

		internal IList<MapPointLayerExprHost> MapPointLayersHostsRemotable => m_mapPointLayersHostsRemotable;

		internal IList<MapLineLayerExprHost> MapLineLayersHostsRemotable => m_mapLineLayersHostsRemotable;

		internal IList<MapTileLayerExprHost> MapTileLayersHostsRemotable => m_mapTileLayersHostsRemotable;

		internal IList<MapLegendExprHost> MapLegendsHostsRemotable => m_mapLegendsHostsRemotable;

		internal IList<MapTitleExprHost> MapTitlesHostsRemotable => m_mapTitlesHostsRemotable;

		public virtual object AntiAliasingExpr => null;

		public virtual object TextAntiAliasingQualityExpr => null;

		public virtual object ShadowIntensityExpr => null;

		public virtual object TileLanguageExpr => null;
	}
}
