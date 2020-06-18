namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartGridLinesExprHost : StyleExprHost
	{
		public virtual object EnabledExpr => null;

		public virtual object IntervalExpr => null;

		public virtual object IntervalTypeExpr => null;

		public virtual object IntervalOffsetExpr => null;

		public virtual object IntervalOffsetTypeExpr => null;
	}
}
