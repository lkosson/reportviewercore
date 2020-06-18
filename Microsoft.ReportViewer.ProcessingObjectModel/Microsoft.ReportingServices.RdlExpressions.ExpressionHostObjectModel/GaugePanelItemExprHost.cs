namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugePanelItemExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public virtual object TopExpr => null;

		public virtual object LeftExpr => null;

		public virtual object HeightExpr => null;

		public virtual object WidthExpr => null;

		public virtual object ZIndexExpr => null;

		public virtual object HiddenExpr => null;

		public virtual object ToolTipExpr => null;
	}
}
