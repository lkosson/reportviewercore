namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ReportParamExprHost : IndexedExprHost
	{
		public IndexedExprHost ValidValuesHost;

		public IndexedExprHost ValidValueLabelsHost;

		public virtual object PromptExpr => null;

		public virtual object ValidationExpressionExpr => null;
	}
}
