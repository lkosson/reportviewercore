using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapPointLayerExprHost : MapVectorLayerExprHost
	{
		public MapPointTemplateExprHost MapPointTemplateHost;

		public MapPointRulesExprHost MapPointRulesHost;

		[CLSCompliant(false)]
		protected IList<MapPointExprHost> m_mapPointsHostsRemotable;

		internal IList<MapPointExprHost> MapPointsHostsRemotable => m_mapPointsHostsRemotable;
	}
}
