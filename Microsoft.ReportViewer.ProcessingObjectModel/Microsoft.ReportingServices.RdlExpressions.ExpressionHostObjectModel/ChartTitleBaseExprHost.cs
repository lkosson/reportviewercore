namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartTitleBaseExprHost : StyleExprHost
	{
		public virtual object CaptionExpr => null;

		public virtual object ChartTitlePositionExpr => null;
	}
}
