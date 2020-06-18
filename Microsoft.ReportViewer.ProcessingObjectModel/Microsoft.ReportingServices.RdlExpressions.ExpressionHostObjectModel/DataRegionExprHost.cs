using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class DataRegionExprHost<TMemberType, TCellType> : ReportItemExprHost where TMemberType : MemberNodeExprHost<TMemberType> where TCellType : CellExprHost
	{
		[CLSCompliant(false)]
		protected IList<FilterExprHost> m_filterHostsRemotable;

		protected SortExprHost m_sortHost;

		[CLSCompliant(false)]
		protected IList<IMemberNode> m_memberTreeHostsRemotable;

		[CLSCompliant(false)]
		protected IList<TCellType> m_cellHostsRemotable;

		public IndexedExprHost UserSortExpressionsHost;

		[CLSCompliant(false)]
		protected IList<JoinConditionExprHost> m_joinConditionExprHostsRemotable;

		public virtual object NoRowsExpr => null;

		internal IList<FilterExprHost> FilterHostsRemotable => m_filterHostsRemotable;

		internal SortExprHost SortHost => m_sortHost;

		internal IList<IMemberNode> MemberTreeHostsRemotable => m_memberTreeHostsRemotable;

		internal IList<TCellType> CellHostsRemotable => m_cellHostsRemotable;

		internal IList<JoinConditionExprHost> JoinConditionExprHostsRemotable => m_joinConditionExprHostsRemotable;
	}
}
