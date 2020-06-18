namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class FrameImageExprHost : BaseGaugeImageExprHost
	{
		public virtual object HueColorExpr => null;

		public virtual object TransparencyExpr => null;

		public virtual object ClipImageExpr => null;
	}
}
