namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapSubItemExprHost : StyleExprHost
	{
		public MapLocationExprHost MapLocationHost;

		public MapSizeExprHost MapSizeHost;

		public virtual object LeftMarginExpr => null;

		public virtual object RightMarginExpr => null;

		public virtual object TopMarginExpr => null;

		public virtual object BottomMarginExpr => null;

		public virtual object ZIndexExpr => null;
	}
}
