namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ThermometerExprHost : StyleExprHost
	{
		public virtual object BulbOffsetExpr => null;

		public virtual object BulbSizeExpr => null;

		public virtual object ThermometerStyleExpr => null;
	}
}
