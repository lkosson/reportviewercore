namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class IndicatorStateExprHost : ReportObjectModelProxy
	{
		public GaugeInputValueExprHost StartValueHost;

		public GaugeInputValueExprHost EndValueHost;

		public IndicatorImageExprHost IndicatorImageHost;

		public virtual object ColorExpr => null;

		public virtual object ScaleFactorExpr => null;

		public virtual object IndicatorStyleExpr => null;
	}
}
