namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartDataPointInLegendExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public virtual object LegendTextExpr => null;

		public virtual object ToolTipExpr => null;

		public virtual object HiddenExpr => null;
	}
}
