using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapCustomColorRuleExprHost : MapColorRuleExprHost
	{
		[CLSCompliant(false)]
		protected IList<MapCustomColorExprHost> m_mapCustomColorsHostsRemotable;

		internal IList<MapCustomColorExprHost> MapCustomColorsHostsRemotable => m_mapCustomColorsHostsRemotable;
	}
}
