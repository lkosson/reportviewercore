namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapSizeExprHost : ReportObjectModelProxy
	{
		public virtual object WidthExpr => null;

		public virtual object HeightExpr => null;

		public virtual object UnitExpr => null;
	}
}
