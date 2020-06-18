namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeInputValueExprHost : ReportObjectModelProxy
	{
		public virtual object ValueExpr => null;

		public virtual object FormulaExpr => null;

		public virtual object MinPercentExpr => null;

		public virtual object MaxPercentExpr => null;

		public virtual object MultiplierExpr => null;

		public virtual object AddConstantExpr => null;
	}
}
