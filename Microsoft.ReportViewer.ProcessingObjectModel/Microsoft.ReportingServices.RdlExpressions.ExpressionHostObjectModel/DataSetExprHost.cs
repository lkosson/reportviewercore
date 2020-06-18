using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class DataSetExprHost : ReportObjectModelProxy
	{
		[CLSCompliant(false)]
		protected IList<CalcFieldExprHost> m_fieldHostsRemotable;

		[CLSCompliant(false)]
		protected IList<JoinConditionExprHost> m_joinConditionExprHostsRemotable;

		public IndexedExprHost QueryParametersHost;

		[CLSCompliant(false)]
		protected IList<FilterExprHost> m_filterHostsRemotable;

		public IndexedExprHost UserSortExpressionsHost;

		internal IList<CalcFieldExprHost> FieldHostsRemotable => m_fieldHostsRemotable;

		internal IList<JoinConditionExprHost> JoinConditionExprHostsRemotable => m_joinConditionExprHostsRemotable;

		public virtual object QueryCommandTextExpr => null;

		internal IList<FilterExprHost> FilterHostsRemotable => m_filterHostsRemotable;
	}
}
