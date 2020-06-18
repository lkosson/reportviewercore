using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class CellExprHost : ReportObjectModelProxy
	{
		[CLSCompliant(false)]
		protected IList<JoinConditionExprHost> m_joinConditionExprHostsRemotable;

		internal IList<JoinConditionExprHost> JoinConditionExprHostsRemotable => m_joinConditionExprHostsRemotable;
	}
}
