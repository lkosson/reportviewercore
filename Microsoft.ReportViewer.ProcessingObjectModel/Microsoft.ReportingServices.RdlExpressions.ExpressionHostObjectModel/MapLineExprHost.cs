namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapLineExprHost : MapSpatialElementExprHost
	{
		public MapLineTemplateExprHost MapLineTemplateHost;

		public virtual object UseCustomLineTemplateExpr => null;
	}
}
