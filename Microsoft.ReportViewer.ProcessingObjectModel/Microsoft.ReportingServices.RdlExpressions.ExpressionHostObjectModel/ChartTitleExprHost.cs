namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartTitleExprHost : ChartTitleBaseExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public ChartElementPositionExprHost ChartElementPositionHost;

		public virtual object HiddenExpr => null;

		public virtual object DockingExpr => null;

		public virtual object DockingOffsetExpr => null;

		public virtual object DockOutsideChartAreaExpr => null;

		public virtual object ToolTipExpr => null;

		public virtual object TextOrientationExpr => null;
	}
}
