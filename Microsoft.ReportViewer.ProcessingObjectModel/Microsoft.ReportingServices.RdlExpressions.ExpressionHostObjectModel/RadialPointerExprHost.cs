namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class RadialPointerExprHost : GaugePointerExprHost
	{
		public PointerCapExprHost PointerCapHost;

		public virtual object TypeExpr => null;

		public virtual object NeedleStyleExpr => null;
	}
}
