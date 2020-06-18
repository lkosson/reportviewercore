namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapLegendTitleExprHost : StyleExprHost
	{
		public virtual object CaptionExpr => null;

		public virtual object TitleSeparatorExpr => null;

		public virtual object TitleSeparatorColorExpr => null;
	}
}
