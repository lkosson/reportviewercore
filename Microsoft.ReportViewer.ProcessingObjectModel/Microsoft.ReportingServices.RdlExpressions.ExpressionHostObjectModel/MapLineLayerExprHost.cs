using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapLineLayerExprHost : MapVectorLayerExprHost
	{
		public MapLineTemplateExprHost MapLineTemplateHost;

		public MapLineRulesExprHost MapLineRulesHost;

		[CLSCompliant(false)]
		protected IList<MapLineExprHost> m_mapLinesHostsRemotable;

		internal IList<MapLineExprHost> MapLinesHostsRemotable => m_mapLinesHostsRemotable;
	}
}
