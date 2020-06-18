using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapShapefileExprHost : MapSpatialDataExprHost
	{
		[CLSCompliant(false)]
		protected IList<MapFieldNameExprHost> m_mapFieldNamesHostsRemotable;

		public virtual object SourceExpr => null;

		internal IList<MapFieldNameExprHost> MapFieldNamesHostsRemotable => m_mapFieldNamesHostsRemotable;
	}
}
