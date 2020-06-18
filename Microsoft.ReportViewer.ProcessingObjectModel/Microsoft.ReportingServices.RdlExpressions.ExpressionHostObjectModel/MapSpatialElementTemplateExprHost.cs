namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapSpatialElementTemplateExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public virtual object HiddenExpr => null;

		public virtual object OffsetXExpr => null;

		public virtual object OffsetYExpr => null;

		public virtual object LabelExpr => null;

		public virtual object ToolTipExpr => null;

		public virtual object DataElementLabelExpr => null;
	}
}
