namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugePointerExprHost : StyleExprHost
	{
		public GaugeInputValueExprHost GaugeInputValueHost;

		public PointerImageExprHost PointerImageHost;

		public ActionInfoExprHost ActionInfoHost;

		public virtual object BarStartExpr => null;

		public virtual object DistanceFromScaleExpr => null;

		public virtual object MarkerLengthExpr => null;

		public virtual object MarkerStyleExpr => null;

		public virtual object PlacementExpr => null;

		public virtual object SnappingEnabledExpr => null;

		public virtual object SnappingIntervalExpr => null;

		public virtual object ToolTipExpr => null;

		public virtual object HiddenExpr => null;

		public virtual object WidthExpr => null;
	}
}
