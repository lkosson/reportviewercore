namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartStripLineExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public virtual object TitleExpr => null;

		public virtual object TitleAngleExpr => null;

		public virtual object TextOrientationExpr => null;

		public virtual object ToolTipExpr => null;

		public virtual object IntervalExpr => null;

		public virtual object IntervalTypeExpr => null;

		public virtual object IntervalOffsetExpr => null;

		public virtual object IntervalOffsetTypeExpr => null;

		public virtual object StripWidthExpr => null;

		public virtual object StripWidthTypeExpr => null;
	}
}
