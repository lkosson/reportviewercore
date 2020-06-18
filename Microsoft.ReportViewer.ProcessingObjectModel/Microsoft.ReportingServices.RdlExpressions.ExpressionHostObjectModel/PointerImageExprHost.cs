namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class PointerImageExprHost : BaseGaugeImageExprHost
	{
		public virtual object HueColorExpr => null;

		public virtual object TransparencyExpr => null;

		public virtual object OffsetXExpr => null;

		public virtual object OffsetYExpr => null;
	}
}
