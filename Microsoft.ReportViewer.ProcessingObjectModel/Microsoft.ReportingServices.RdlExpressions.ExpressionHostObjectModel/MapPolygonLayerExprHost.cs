using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapPolygonLayerExprHost : MapVectorLayerExprHost
	{
		public MapPolygonTemplateExprHost MapPolygonTemplateHost;

		public MapPolygonRulesExprHost MapPolygonRulesHost;

		public MapPointTemplateExprHost MapPointTemplateHost;

		public MapPointRulesExprHost MapPointRulesHost;

		[CLSCompliant(false)]
		protected IList<MapPolygonExprHost> m_mapPolygonsHostsRemotable;

		internal IList<MapPolygonExprHost> MapPolygonsHostsRemotable => m_mapPolygonsHostsRemotable;
	}
}
