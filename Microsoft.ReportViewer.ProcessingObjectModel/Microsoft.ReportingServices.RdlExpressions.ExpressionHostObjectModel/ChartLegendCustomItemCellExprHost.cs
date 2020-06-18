namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartLegendCustomItemCellExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public virtual object CellTypeExpr => null;

		public virtual object TextExpr => null;

		public virtual object CellSpanExpr => null;

		public virtual object ToolTipExpr => null;

		public virtual object ImageWidthExpr => null;

		public virtual object ImageHeightExpr => null;

		public virtual object SymbolHeightExpr => null;

		public virtual object SymbolWidthExpr => null;

		public virtual object AlignmentExpr => null;

		public virtual object TopMarginExpr => null;

		public virtual object BottomMarginExpr => null;

		public virtual object LeftMarginExpr => null;

		public virtual object RightMarginExpr => null;
	}
}
