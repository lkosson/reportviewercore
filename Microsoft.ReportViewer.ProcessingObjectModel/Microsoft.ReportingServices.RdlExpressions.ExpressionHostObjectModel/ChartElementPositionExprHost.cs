namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartElementPositionExprHost : ReportObjectModelProxy
	{
		public virtual object TopExpr => null;

		public virtual object LeftExpr => null;

		public virtual object HeightExpr => null;

		public virtual object WidthExpr => null;
	}
}
