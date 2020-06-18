namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class BackFrameExprHost : StyleExprHost
	{
		public FrameBackgroundExprHost FrameBackgroundHost;

		public FrameImageExprHost FrameImageHost;

		public virtual object FrameStyleExpr => null;

		public virtual object FrameShapeExpr => null;

		public virtual object FrameWidthExpr => null;

		public virtual object GlassEffectExpr => null;
	}
}
