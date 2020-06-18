namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ScaleRangeExprHost : StyleExprHost
	{
		public GaugeInputValueExprHost StartValueHost;

		public GaugeInputValueExprHost EndValueHost;

		public ActionInfoExprHost ActionInfoHost;

		public virtual object DistanceFromScaleExpr => null;

		public virtual object StartWidthExpr => null;

		public virtual object EndWidthExpr => null;

		public virtual object InRangeBarPointerColorExpr => null;

		public virtual object InRangeLabelColorExpr => null;

		public virtual object InRangeTickMarksColorExpr => null;

		public virtual object PlacementExpr => null;

		public virtual object ToolTipExpr => null;

		public virtual object HiddenExpr => null;
	}
}
