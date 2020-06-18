namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapColorRangeRuleExprHost : MapColorRuleExprHost
	{
		public virtual object StartColorExpr => null;

		public virtual object MiddleColorExpr => null;

		public virtual object EndColorExpr => null;
	}
}
