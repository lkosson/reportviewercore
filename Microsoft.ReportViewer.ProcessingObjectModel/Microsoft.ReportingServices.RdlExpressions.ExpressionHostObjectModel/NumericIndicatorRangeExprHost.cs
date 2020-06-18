namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class NumericIndicatorRangeExprHost : ReportObjectModelProxy
	{
		public GaugeInputValueExprHost StartValueHost;

		public GaugeInputValueExprHost EndValueHost;

		public virtual object DecimalDigitColorExpr => null;

		public virtual object DigitColorExpr => null;
	}
}
