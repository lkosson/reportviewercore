namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartLegendColumnExprHost : StyleExprHost
	{
		public ChartLegendColumnHeaderExprHost HeaderHost;

		public ActionInfoExprHost ActionInfoHost;

		public virtual object ColumnTypeExpr => null;

		public virtual object ValueExpr => null;

		public virtual object ToolTipExpr => null;

		public virtual object MinimumWidthExpr => null;

		public virtual object MaximumWidthExpr => null;

		public virtual object SeriesSymbolWidthExpr => null;

		public virtual object SeriesSymbolHeightExpr => null;
	}
}
