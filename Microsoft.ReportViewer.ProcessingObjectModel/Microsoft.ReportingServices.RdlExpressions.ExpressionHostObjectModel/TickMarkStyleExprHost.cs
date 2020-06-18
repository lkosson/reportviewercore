namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class TickMarkStyleExprHost : StyleExprHost
	{
		public TopImageExprHost TickMarkImageHost;

		public virtual object DistanceFromScaleExpr => null;

		public virtual object PlacementExpr => null;

		public virtual object EnableGradientExpr => null;

		public virtual object GradientDensityExpr => null;

		public virtual object LengthExpr => null;

		public virtual object WidthExpr => null;

		public virtual object ShapeExpr => null;

		public virtual object HiddenExpr => null;
	}
}
