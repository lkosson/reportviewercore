namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapPolygonExprHost : MapSpatialElementExprHost
	{
		public MapPolygonTemplateExprHost MapPolygonTemplateHost;

		public MapPointTemplateExprHost MapPointTemplateHost;

		public virtual object UseCustomPolygonTemplateExpr => null;

		public virtual object UseCustomPointTemplateExpr => null;
	}
}
