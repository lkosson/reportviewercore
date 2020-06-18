namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapViewportExprHost : MapSubItemExprHost
	{
		public MapLimitsExprHost MapLimitsHost;

		public MapViewExprHost MapViewHost;

		public MapGridLinesExprHost MapMeridiansHost;

		public MapGridLinesExprHost MapParallelsHost;

		public virtual object MapCoordinateSystemExpr => null;

		public virtual object MapProjectionExpr => null;

		public virtual object ProjectionCenterXExpr => null;

		public virtual object ProjectionCenterYExpr => null;

		public virtual object MaximumZoomExpr => null;

		public virtual object MinimumZoomExpr => null;

		public virtual object ContentMarginExpr => null;

		public virtual object GridUnderContentExpr => null;

		public virtual object SimplificationResolutionExpr => null;
	}
}
