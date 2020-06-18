namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartTickMarksExprHost : StyleExprHost
	{
		public virtual object EnabledExpr => null;

		public virtual object TypeExpr => null;

		public virtual object LengthExpr => null;

		public virtual object IntervalExpr => null;

		public virtual object IntervalTypeExpr => null;

		public virtual object IntervalOffsetExpr => null;

		public virtual object IntervalOffsetTypeExpr => null;
	}
}
