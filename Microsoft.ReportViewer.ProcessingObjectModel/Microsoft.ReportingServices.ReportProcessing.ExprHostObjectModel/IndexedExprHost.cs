namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class IndexedExprHost : ReportObjectModelProxy
	{
		public virtual object this[int index] => null;
	}
}
