namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeImageExprHost : GaugePanelItemExprHost
	{
		public virtual object SourceExpr => null;

		public virtual object ValueExpr => null;
	}
}
