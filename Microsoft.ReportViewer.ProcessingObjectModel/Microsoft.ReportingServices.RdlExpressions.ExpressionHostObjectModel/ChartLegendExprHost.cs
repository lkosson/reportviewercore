using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartLegendExprHost : StyleExprHost
	{
		[CLSCompliant(false)]
		protected IList<ChartLegendCustomItemExprHost> m_legendCustomItemsHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartLegendColumnExprHost> m_legendColumnsHostsRemotable;

		public ChartLegendTitleExprHost TitleExprHost;

		public ChartElementPositionExprHost ChartElementPositionHost;

		internal IList<ChartLegendCustomItemExprHost> ChartLegendCustomItemsHostsRemotable => m_legendCustomItemsHostsRemotable;

		internal IList<ChartLegendColumnExprHost> ChartLegendColumnsHostsRemotable => m_legendColumnsHostsRemotable;

		public virtual object ChartLegendPositionExpr => null;

		public virtual object LayoutExpr => null;

		public virtual object HiddenExpr => null;

		public virtual object DockOutsideChartAreaExpr => null;

		public virtual object AutoFitTextDisabledExpr => null;

		public virtual object MinFontSizeExpr => null;

		public virtual object HeaderSeparatorExpr => null;

		public virtual object HeaderSeparatorColorExpr => null;

		public virtual object ColumnSeparatorExpr => null;

		public virtual object ColumnSeparatorColorExpr => null;

		public virtual object ColumnSpacingExpr => null;

		public virtual object InterlacedRowsExpr => null;

		public virtual object InterlacedRowsColorExpr => null;

		public virtual object EquallySpacedItemsExpr => null;

		public virtual object ReversedExpr => null;

		public virtual object MaxAutoSizeExpr => null;

		public virtual object TextWrapThresholdExpr => null;
	}
}
