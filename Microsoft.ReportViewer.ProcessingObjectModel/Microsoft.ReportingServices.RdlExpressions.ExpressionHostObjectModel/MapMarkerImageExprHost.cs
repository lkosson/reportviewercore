namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapMarkerImageExprHost : ReportObjectModelProxy
	{
		public virtual object SourceExpr => null;

		public virtual object ValueExpr => null;

		public virtual object MIMETypeExpr => null;

		public virtual object TransparentColorExpr => null;

		public virtual object ResizeModeExpr => null;
	}
}
