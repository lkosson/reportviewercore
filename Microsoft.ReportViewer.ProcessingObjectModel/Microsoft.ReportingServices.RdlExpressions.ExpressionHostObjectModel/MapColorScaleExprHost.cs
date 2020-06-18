namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapColorScaleExprHost : MapDockableSubItemExprHost
	{
		public MapColorScaleTitleExprHost MapColorScaleTitleHost;

		public virtual object TickMarkLengthExpr => null;

		public virtual object ColorBarBorderColorExpr => null;

		public virtual object LabelIntervalExpr => null;

		public virtual object LabelFormatExpr => null;

		public virtual object LabelPlacementExpr => null;

		public virtual object LabelBehaviorExpr => null;

		public virtual object HideEndLabelsExpr => null;

		public virtual object RangeGapColorExpr => null;

		public virtual object NoDataTextExpr => null;
	}
}
