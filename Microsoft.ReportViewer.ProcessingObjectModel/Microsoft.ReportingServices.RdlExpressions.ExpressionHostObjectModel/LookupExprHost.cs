namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class LookupExprHost : ReportObjectModelProxy
	{
		public virtual object SourceExpr => null;

		public virtual object ResultExpr => null;
	}
}
