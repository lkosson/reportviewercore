namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartMemberExprHost : MemberNodeExprHost<ChartMemberExprHost>
	{
		public ChartSeriesExprHost ChartSeriesHost;

		public virtual object MemberLabelExpr => null;
	}
}
