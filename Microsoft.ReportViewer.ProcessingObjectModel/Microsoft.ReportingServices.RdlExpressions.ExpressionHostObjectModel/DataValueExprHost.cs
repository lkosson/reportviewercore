namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class DataValueExprHost : ReportObjectModelProxy
	{
		public virtual object DataValueNameExpr => null;

		public virtual object DataValueValueExpr => null;
	}
}
