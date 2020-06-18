namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class JoinConditionExprHost : ReportObjectModelProxy
	{
		public virtual object ForeignKeyExpr => null;

		public virtual object PrimaryKeyExpr => null;
	}
}
