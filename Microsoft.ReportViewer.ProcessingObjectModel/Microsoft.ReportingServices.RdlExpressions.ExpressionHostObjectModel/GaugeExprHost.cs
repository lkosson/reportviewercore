namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeExprHost : GaugePanelItemExprHost
	{
		public BackFrameExprHost BackFrameHost;

		public TopImageExprHost TopImageHost;

		public virtual object ClipContentExpr => null;

		public virtual object AspectRatioExpr => null;
	}
}
