namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartDataLabelExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public virtual object VisibleExpr => null;

		public virtual object LabelExpr => null;

		public virtual object ChartDataLabelPositionExpr => null;

		public virtual object RotationExpr => null;

		public virtual object UseValueAsLabelExpr => null;

		public virtual object ToolTipExpr => null;
	}
}
