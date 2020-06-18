namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class DataValueExprHost : ReportObjectModelProxy
	{
		public virtual object DataValueNameExpr => null;

		public virtual object DataValueValueExpr => null;
	}
}
