namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ScalePinExprHost : TickMarkStyleExprHost
	{
		public PinLabelExprHost PinLabelHost;

		public virtual object LocationExpr => null;

		public virtual object EnableExpr => null;
	}
}
