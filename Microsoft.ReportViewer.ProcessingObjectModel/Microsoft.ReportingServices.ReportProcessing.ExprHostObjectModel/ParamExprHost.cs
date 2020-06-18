namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ParamExprHost : ReportObjectModelProxy
	{
		public virtual object ValueExpr => null;

		public virtual object OmitExpr => null;
	}
}
