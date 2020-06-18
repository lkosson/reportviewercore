namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class PageBreakExprHost : ReportObjectModelProxy
	{
		public virtual object DisabledExpr => null;

		public virtual object ResetPageNumberExpr => null;
	}
}
