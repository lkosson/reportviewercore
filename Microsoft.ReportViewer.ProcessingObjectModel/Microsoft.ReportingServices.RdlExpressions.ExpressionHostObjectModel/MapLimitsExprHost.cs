namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapLimitsExprHost : ReportObjectModelProxy
	{
		public virtual object MinimumXExpr => null;

		public virtual object MinimumYExpr => null;

		public virtual object MaximumXExpr => null;

		public virtual object MaximumYExpr => null;

		public virtual object LimitToDataExpr => null;
	}
}
