namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class TableExprHost : DataRegionExprHost
	{
		public TableGroupExprHost TableGroupsHost;

		public IndexedExprHost TableRowVisibilityHiddenExpressions;

		public IndexedExprHost TableColumnVisibilityHiddenExpressions;
	}
}
