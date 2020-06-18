namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ImageExprHost : ReportItemExprHost
	{
		public virtual object ValueExpr => null;

		public virtual object MIMETypeExpr => null;
	}
}
