namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ImageExprHost : ReportItemExprHost
	{
		public virtual object ValueExpr => null;

		public virtual object MIMETypeExpr => null;

		public virtual object TagExpr => null;
	}
}
