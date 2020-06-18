namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class IndicatorImageExprHost : BaseGaugeImageExprHost
	{
		public virtual object HueColorExpr => null;

		public virtual object TransparencyExpr => null;
	}
}
