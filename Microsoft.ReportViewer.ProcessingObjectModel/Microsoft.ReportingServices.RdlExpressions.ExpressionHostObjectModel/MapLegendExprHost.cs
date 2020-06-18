namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapLegendExprHost : MapDockableSubItemExprHost
	{
		public MapLegendTitleExprHost MapLegendTitleHost;

		public virtual object LayoutExpr => null;

		public virtual object AutoFitTextDisabledExpr => null;

		public virtual object MinFontSizeExpr => null;

		public virtual object InterlacedRowsExpr => null;

		public virtual object InterlacedRowsColorExpr => null;

		public virtual object EquallySpacedItemsExpr => null;

		public virtual object TextWrapThresholdExpr => null;
	}
}
