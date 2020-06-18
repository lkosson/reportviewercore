namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapPolygonTemplateExprHost : MapSpatialElementTemplateExprHost
	{
		public virtual object ScaleFactorExpr => null;

		public virtual object CenterPointOffsetXExpr => null;

		public virtual object CenterPointOffsetYExpr => null;

		public virtual object ShowLabelExpr => null;

		public virtual object LabelPlacementExpr => null;
	}
}
