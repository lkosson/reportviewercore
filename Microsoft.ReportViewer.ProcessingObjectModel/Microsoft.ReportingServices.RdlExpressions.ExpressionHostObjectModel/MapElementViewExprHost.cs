using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapElementViewExprHost : MapViewExprHost
	{
		[CLSCompliant(false)]
		protected IList<MapBindingFieldPairExprHost> m_mapBindingFieldPairsHostsRemotable;

		public virtual object LayerNameExpr => null;

		internal IList<MapBindingFieldPairExprHost> MapBindingFieldPairsHostsRemotable => m_mapBindingFieldPairsHostsRemotable;
	}
}
