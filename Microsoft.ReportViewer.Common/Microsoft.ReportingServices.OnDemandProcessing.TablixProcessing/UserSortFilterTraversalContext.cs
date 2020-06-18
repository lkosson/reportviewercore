namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal class UserSortFilterTraversalContext : ITraversalContext
	{
		private RuntimeSortFilterEventInfo m_eventInfo;

		private RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj m_expressionScope;

		public RuntimeSortFilterEventInfo EventInfo => m_eventInfo;

		public RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj ExpressionScope
		{
			get
			{
				return m_expressionScope;
			}
			set
			{
				m_expressionScope = value;
			}
		}

		public UserSortFilterTraversalContext(RuntimeSortFilterEventInfo eventInfo)
		{
			m_eventInfo = eventInfo;
		}
	}
}
