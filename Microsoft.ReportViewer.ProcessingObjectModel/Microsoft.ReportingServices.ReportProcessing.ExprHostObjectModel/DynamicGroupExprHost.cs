namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class DynamicGroupExprHost : ReportObjectModelProxy, IVisibilityHiddenExprHost
	{
		public GroupingExprHost GroupingHost;

		public SortingExprHost SortingHost;

		public DynamicGroupExprHost SubGroupHost;

		public virtual object VisibilityHiddenExpr => null;
	}
}
