using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapTileLayerExprHost : MapLayerExprHost
	{
		[CLSCompliant(false)]
		protected IList<MapTileExprHost> m_mapTilesHostsRemotable;

		public virtual object ServiceUrlExpr => null;

		public virtual object TileStyleExpr => null;

		public virtual object UseSecureConnectionExpr => null;

		internal IList<MapTileExprHost> MapTilesHostsRemotable => m_mapTilesHostsRemotable;
	}
}
