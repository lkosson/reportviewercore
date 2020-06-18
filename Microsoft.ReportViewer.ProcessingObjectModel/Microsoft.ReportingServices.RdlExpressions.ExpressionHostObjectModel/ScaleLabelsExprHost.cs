namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ScaleLabelsExprHost : StyleExprHost
	{
		public virtual object IntervalExpr => null;

		public virtual object IntervalOffsetExpr => null;

		public virtual object AllowUpsideDownExpr => null;

		public virtual object DistanceFromScaleExpr => null;

		public virtual object FontAngleExpr => null;

		public virtual object PlacementExpr => null;

		public virtual object RotateLabelsExpr => null;

		public virtual object ShowEndLabelsExpr => null;

		public virtual object HiddenExpr => null;

		public virtual object UseFontPercentExpr => null;
	}
}
