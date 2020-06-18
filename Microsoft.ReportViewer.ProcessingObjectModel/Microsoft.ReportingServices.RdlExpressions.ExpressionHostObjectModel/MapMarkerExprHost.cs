namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapMarkerExprHost : ReportObjectModelProxy
	{
		public MapMarkerImageExprHost MapMarkerImageHost;

		public virtual object MapMarkerStyleExpr => null;
	}
}
