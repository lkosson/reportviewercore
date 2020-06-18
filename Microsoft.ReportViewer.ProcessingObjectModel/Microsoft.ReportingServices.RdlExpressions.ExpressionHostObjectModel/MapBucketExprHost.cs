namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapBucketExprHost : ReportObjectModelProxy
	{
		public virtual object StartValueExpr => null;

		public virtual object EndValueExpr => null;
	}
}
