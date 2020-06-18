namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeLabelExprHost : GaugePanelItemExprHost
	{
		public virtual object TextExpr => null;

		public virtual object AngleExpr => null;

		public virtual object ResizeModeExpr => null;

		public virtual object TextShadowOffsetExpr => null;

		public virtual object UseFontPercentExpr => null;
	}
}
