namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartSmartLabelExprHost : StyleExprHost
	{
		public ChartNoMoveDirectionsExprHost NoMoveDirectionsHost;

		public virtual object AllowOutSidePlotAreaExpr => null;

		public virtual object CalloutBackColorExpr => null;

		public virtual object CalloutLineAnchorExpr => null;

		public virtual object CalloutLineColorExpr => null;

		public virtual object CalloutLineStyleExpr => null;

		public virtual object CalloutLineWidthExpr => null;

		public virtual object CalloutStyleExpr => null;

		public virtual object ShowOverlappedExpr => null;

		public virtual object MarkerOverlappingExpr => null;

		public virtual object MaxMovingDistanceExpr => null;

		public virtual object MinMovingDistanceExpr => null;

		public virtual object DisabledExpr => null;
	}
}
