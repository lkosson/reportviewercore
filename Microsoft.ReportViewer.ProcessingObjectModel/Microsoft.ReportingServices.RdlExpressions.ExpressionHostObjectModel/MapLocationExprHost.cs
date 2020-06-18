namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapLocationExprHost : ReportObjectModelProxy
	{
		public virtual object LeftExpr => null;

		public virtual object TopExpr => null;

		public virtual object UnitExpr => null;
	}
}
