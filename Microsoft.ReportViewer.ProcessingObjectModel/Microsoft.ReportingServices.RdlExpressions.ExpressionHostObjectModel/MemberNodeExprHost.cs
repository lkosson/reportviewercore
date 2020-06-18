using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MemberNodeExprHost<TMemberType> : ReportObjectModelProxy, IMemberNode where TMemberType : IMemberNode
	{
		protected GroupExprHost m_groupHost;

		protected SortExprHost m_sortHost;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		[CLSCompliant(false)]
		protected IList<IMemberNode> m_memberTreeHostsRemotable;

		[CLSCompliant(false)]
		protected IList<JoinConditionExprHost> m_joinConditionExprHostsRemotable;

		GroupExprHost IMemberNode.GroupHost => m_groupHost;

		SortExprHost IMemberNode.SortHost => m_sortHost;

		IList<DataValueExprHost> IMemberNode.CustomPropertyHostsRemotable => m_customPropertyHostsRemotable;

		IList<IMemberNode> IMemberNode.MemberTreeHostsRemotable => m_memberTreeHostsRemotable;

		IList<JoinConditionExprHost> IMemberNode.JoinConditionExprHostsRemotable => m_joinConditionExprHostsRemotable;
	}
}
