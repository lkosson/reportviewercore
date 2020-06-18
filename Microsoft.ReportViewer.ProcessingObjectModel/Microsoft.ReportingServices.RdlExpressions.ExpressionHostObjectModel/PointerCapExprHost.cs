namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class PointerCapExprHost : StyleExprHost
	{
		public CapImageExprHost CapImageHost;

		public virtual object OnTopExpr => null;

		public virtual object ReflectionExpr => null;

		public virtual object CapStyleExpr => null;

		public virtual object HiddenExpr => null;

		public virtual object WidthExpr => null;
	}
}
