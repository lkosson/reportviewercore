namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapPointExprHost : MapSpatialElementExprHost
	{
		public MapPointTemplateExprHost MapPointTemplateHost;

		public virtual object UseCustomPointTemplateExpr => null;
	}
}
