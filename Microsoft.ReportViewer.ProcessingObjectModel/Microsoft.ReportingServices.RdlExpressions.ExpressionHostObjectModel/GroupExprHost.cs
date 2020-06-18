using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GroupExprHost : IndexedExprHost
	{
		public IndexedExprHost ParentExpressionsHost;

		public IndexedExprHost ReGroupExpressionsHost;

		public IndexedExprHost VariableValueHosts;

		[CLSCompliant(false)]
		protected IList<FilterExprHost> m_filterHostsRemotable;

		public IndexedExprHost UserSortExpressionsHost;

		public PageBreakExprHost PageBreakExprHost;

		public virtual object LabelExpr => null;

		public virtual object PageNameExpr => null;

		internal IList<FilterExprHost> FilterHostsRemotable => m_filterHostsRemotable;
	}
}
