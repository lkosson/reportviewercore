using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapAppearanceRuleExprHost : ReportObjectModelProxy
	{
		[CLSCompliant(false)]
		protected IList<MapBucketExprHost> m_mapBucketsHostsRemotable;

		public virtual object DataValueExpr => null;

		public virtual object DistributionTypeExpr => null;

		public virtual object BucketCountExpr => null;

		public virtual object StartValueExpr => null;

		public virtual object EndValueExpr => null;

		internal IList<MapBucketExprHost> MapBucketsHostsRemotable => m_mapBucketsHostsRemotable;

		public virtual object LegendTextExpr => null;
	}
}
