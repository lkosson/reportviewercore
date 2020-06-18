using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapMemberExprHost : MemberNodeExprHost<MapMemberExprHost>
	{
		[CLSCompliant(false)]
		protected IList<MapPolygonLayerExprHost> m_mapPolygonLayersHostsRemotable;

		[CLSCompliant(false)]
		protected IList<MapPointLayerExprHost> m_mapPointLayersHostsRemotable;

		[CLSCompliant(false)]
		protected IList<MapLineLayerExprHost> m_mapLineLayersHostsRemotable;

		internal IList<MapPolygonLayerExprHost> MapPolygonLayersHostsRemotable => m_mapPolygonLayersHostsRemotable;

		internal IList<MapPointLayerExprHost> MapPointLayersHostsRemotable => m_mapPointLayersHostsRemotable;

		internal IList<MapLineLayerExprHost> MapLineLayersHostsRemotable => m_mapLineLayersHostsRemotable;
	}
}
