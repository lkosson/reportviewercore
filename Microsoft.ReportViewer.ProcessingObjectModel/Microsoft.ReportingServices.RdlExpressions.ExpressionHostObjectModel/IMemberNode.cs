using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public interface IMemberNode
	{
		GroupExprHost GroupHost
		{
			get;
		}

		SortExprHost SortHost
		{
			get;
		}

		IList<DataValueExprHost> CustomPropertyHostsRemotable
		{
			get;
		}

		IList<IMemberNode> MemberTreeHostsRemotable
		{
			get;
		}

		IList<JoinConditionExprHost> JoinConditionExprHostsRemotable
		{
			get;
		}
	}
}
