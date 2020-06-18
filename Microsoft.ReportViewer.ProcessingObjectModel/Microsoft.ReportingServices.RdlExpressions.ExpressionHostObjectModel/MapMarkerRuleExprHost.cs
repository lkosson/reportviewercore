using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapMarkerRuleExprHost : MapAppearanceRuleExprHost
	{
		[CLSCompliant(false)]
		protected IList<MapMarkerExprHost> m_mapMarkersHostsRemotable;

		internal IList<MapMarkerExprHost> MapMarkersHostsRemotable => m_mapMarkersHostsRemotable;
	}
}
