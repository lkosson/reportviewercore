using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapSpatialDataSetExprHost : MapSpatialDataExprHost
	{
		[CLSCompliant(false)]
		protected IList<MapFieldNameExprHost> m_mapFieldNamesHostsRemotable;

		public virtual object DataSetNameExpr => null;

		public virtual object SpatialFieldExpr => null;

		internal IList<MapFieldNameExprHost> MapFieldNamesHostsRemotable => m_mapFieldNamesHostsRemotable;
	}
}
