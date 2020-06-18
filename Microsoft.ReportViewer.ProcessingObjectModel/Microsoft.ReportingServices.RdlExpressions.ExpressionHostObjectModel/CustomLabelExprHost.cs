namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class CustomLabelExprHost : StyleExprHost
	{
		public TickMarkStyleExprHost TickMarkStyleHost;

		public virtual object TextExpr => null;

		public virtual object AllowUpsideDownExpr => null;

		public virtual object DistanceFromScaleExpr => null;

		public virtual object FontAngleExpr => null;

		public virtual object PlacementExpr => null;

		public virtual object RotateLabelExpr => null;

		public virtual object ValueExpr => null;

		public virtual object HiddenExpr => null;

		public virtual object UseFontPercentExpr => null;
	}
}
