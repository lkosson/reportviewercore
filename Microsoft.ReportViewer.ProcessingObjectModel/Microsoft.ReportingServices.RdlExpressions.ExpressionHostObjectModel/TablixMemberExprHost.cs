namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class TablixMemberExprHost : MemberNodeExprHost<TablixMemberExprHost>, IVisibilityHiddenExprHost
	{
		public virtual object VisibilityHiddenExpr => null;
	}
}
