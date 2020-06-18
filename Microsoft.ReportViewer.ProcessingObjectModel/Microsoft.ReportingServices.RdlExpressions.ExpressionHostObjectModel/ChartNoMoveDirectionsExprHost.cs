namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartNoMoveDirectionsExprHost : ReportObjectModelProxy
	{
		public virtual object UpExpr => null;

		public virtual object DownExpr => null;

		public virtual object LeftExpr => null;

		public virtual object RightExpr => null;

		public virtual object UpLeftExpr => null;

		public virtual object UpRightExpr => null;

		public virtual object DownLeftExpr => null;

		public virtual object DownRightExpr => null;
	}
}
