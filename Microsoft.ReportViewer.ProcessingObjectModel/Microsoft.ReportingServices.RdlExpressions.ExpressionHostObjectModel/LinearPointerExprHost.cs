namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class LinearPointerExprHost : GaugePointerExprHost
	{
		public ThermometerExprHost ThermometerHost;

		public virtual object TypeExpr => null;
	}
}
