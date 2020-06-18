namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapLayerExprHost : ReportObjectModelProxy
	{
		public virtual object VisibilityModeExpr => null;

		public virtual object MinimumZoomExpr => null;

		public virtual object MaximumZoomExpr => null;

		public virtual object TransparencyExpr => null;
	}
}
