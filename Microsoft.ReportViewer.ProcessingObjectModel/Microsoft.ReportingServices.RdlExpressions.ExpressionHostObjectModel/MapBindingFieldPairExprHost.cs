namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapBindingFieldPairExprHost : ReportObjectModelProxy
	{
		public virtual object FieldNameExpr => null;

		public virtual object BindingExpressionExpr => null;
	}
}
