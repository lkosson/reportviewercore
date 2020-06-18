using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartAxisExprHost : StyleExprHost
	{
		public ChartAxisTitleExprHost TitleHost;

		public ChartGridLinesExprHost MajorGridLinesHost;

		public ChartGridLinesExprHost MinorGridLinesHost;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartStripLineExprHost> m_stripLinesHostsRemotable;

		public ChartTickMarksExprHost MajorTickMarksHost;

		public ChartTickMarksExprHost MinorTickMarksHost;

		public ChartAxisScaleBreakExprHost AxisScaleBreakHost;

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable => m_customPropertyHostsRemotable;

		internal IList<ChartStripLineExprHost> ChartStripLinesHostsRemotable => m_stripLinesHostsRemotable;

		public virtual object AxisMinExpr => null;

		public virtual object AxisMaxExpr => null;

		public virtual object AxisCrossAtExpr => null;

		public virtual object VisibleExpr => null;

		public virtual object MarginExpr => null;

		public virtual object IntervalExpr => null;

		public virtual object IntervalTypeExpr => null;

		public virtual object IntervalOffsetExpr => null;

		public virtual object IntervalOffsetTypeExpr => null;

		public virtual object MarksAlwaysAtPlotEdgeExpr => null;

		public virtual object ReverseExpr => null;

		public virtual object LocationExpr => null;

		public virtual object InterlacedExpr => null;

		public virtual object InterlacedColorExpr => null;

		public virtual object LogScaleExpr => null;

		public virtual object LogBaseExpr => null;

		public virtual object HideLabelsExpr => null;

		public virtual object AngleExpr => null;

		public virtual object ArrowsExpr => null;

		public virtual object PreventFontShrinkExpr => null;

		public virtual object PreventFontGrowExpr => null;

		public virtual object PreventLabelOffsetExpr => null;

		public virtual object PreventWordWrapExpr => null;

		public virtual object AllowLabelRotationExpr => null;

		public virtual object IncludeZeroExpr => null;

		public virtual object LabelsAutoFitDisabledExpr => null;

		public virtual object MinFontSizeExpr => null;

		public virtual object MaxFontSizeExpr => null;

		public virtual object OffsetLabelsExpr => null;

		public virtual object HideEndLabelsExpr => null;

		public virtual object VariableAutoIntervalExpr => null;

		public virtual object LabelIntervalExpr => null;

		public virtual object LabelIntervalTypeExpr => null;

		public virtual object LabelIntervalOffsetExpr => null;

		public virtual object LabelIntervalOffsetTypeExpr => null;
	}
}
