namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapDockableSubItemExprHost : MapSubItemExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public virtual object MapPositionExpr => null;

		public virtual object DockOutsideViewportExpr => null;

		public virtual object HiddenExpr => null;

		public virtual object ToolTipExpr => null;
	}
}
