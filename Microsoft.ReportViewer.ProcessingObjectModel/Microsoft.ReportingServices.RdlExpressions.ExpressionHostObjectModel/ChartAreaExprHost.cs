using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartAreaExprHost : StyleExprHost
	{
		[CLSCompliant(false)]
		protected IList<ChartAxisExprHost> m_categoryAxesHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartAxisExprHost> m_valueAxesHostsRemotable;

		public Chart3DPropertiesExprHost Chart3DPropertiesHost;

		public ChartElementPositionExprHost ChartElementPositionHost;

		public ChartElementPositionExprHost ChartInnerPlotPositionHost;

		internal IList<ChartAxisExprHost> CategoryAxesHostsRemotable => m_categoryAxesHostsRemotable;

		internal IList<ChartAxisExprHost> ValueAxesHostsRemotable => m_valueAxesHostsRemotable;

		public virtual object HiddenExpr => null;

		public virtual object AlignOrientationExpr => null;

		public virtual object EquallySizedAxesFontExpr => null;

		public virtual object CursorExpr => null;

		public virtual object AxesViewExpr => null;

		public virtual object ChartAlignTypePositionExpr => null;

		public virtual object InnerPlotPositionExpr => null;
	}
}
