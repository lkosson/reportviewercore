using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeScaleExprHost : StyleExprHost
	{
		[CLSCompliant(false)]
		protected IList<ScaleRangeExprHost> m_scaleRangesHostsRemotable;

		[CLSCompliant(false)]
		protected IList<CustomLabelExprHost> m_customLabelsHostsRemotable;

		public GaugeInputValueExprHost MaximumValueHost;

		public GaugeInputValueExprHost MinimumValueHost;

		public GaugeTickMarksExprHost GaugeMajorTickMarksHost;

		public GaugeTickMarksExprHost GaugeMinorTickMarksHost;

		public ScalePinExprHost MaximumPinHost;

		public ScalePinExprHost MinimumPinHost;

		public ScaleLabelsExprHost ScaleLabelsHost;

		public ActionInfoExprHost ActionInfoHost;

		internal IList<ScaleRangeExprHost> ScaleRangesHostsRemotable => m_scaleRangesHostsRemotable;

		internal IList<CustomLabelExprHost> CustomLabelsHostsRemotable => m_customLabelsHostsRemotable;

		public virtual object IntervalExpr => null;

		public virtual object IntervalOffsetExpr => null;

		public virtual object LogarithmicExpr => null;

		public virtual object LogarithmicBaseExpr => null;

		public virtual object MultiplierExpr => null;

		public virtual object ReversedExpr => null;

		public virtual object TickMarksOnTopExpr => null;

		public virtual object ToolTipExpr => null;

		public virtual object HiddenExpr => null;

		public virtual object WidthExpr => null;
	}
}
