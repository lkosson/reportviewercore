using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartEmptyPointsExprHost : StyleExprHost
	{
		public ChartMarkerExprHost ChartMarkerHost;

		public ChartDataLabelExprHost DataLabelHost;

		public ChartDataPointInLegendExprHost DataPointInLegendHost;

		public ActionInfoExprHost ActionInfoHost;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		public virtual object AxisLabelExpr => null;

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable => m_customPropertyHostsRemotable;

		public virtual object ToolTipExpr => null;
	}
}
