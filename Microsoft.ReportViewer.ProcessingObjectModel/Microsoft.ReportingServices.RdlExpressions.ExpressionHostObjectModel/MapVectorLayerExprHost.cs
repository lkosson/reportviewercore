using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapVectorLayerExprHost : MapLayerExprHost
	{
		[CLSCompliant(false)]
		protected IList<MapBindingFieldPairExprHost> m_mapBindingFieldPairsHostsRemotable;

		public MapSpatialDataExprHost MapSpatialDataHost;

		internal IList<MapBindingFieldPairExprHost> MapBindingFieldPairsHostsRemotable => m_mapBindingFieldPairsHostsRemotable;
	}
}
