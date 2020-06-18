namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class MatrixExprHost : DataRegionExprHost
	{
		public MatrixDynamicGroupExprHost RowGroupingsHost;

		public MatrixDynamicGroupExprHost ColumnGroupingsHost;
	}
}
