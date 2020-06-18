namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeTickMarksExprHost : TickMarkStyleExprHost
	{
		public virtual object IntervalExpr => null;

		public virtual object IntervalOffsetExpr => null;
	}
}
