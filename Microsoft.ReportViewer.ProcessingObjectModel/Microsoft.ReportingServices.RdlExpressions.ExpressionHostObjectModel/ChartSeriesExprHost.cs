using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartSeriesExprHost : StyleExprHost
	{
		public ActionInfoExprHost ActionInfoHost;

		public ChartEmptyPointsExprHost EmptyPointsHost;

		public ChartSmartLabelExprHost SmartLabelHost;

		public ChartDataLabelExprHost DataLabelHost;

		public ChartMarkerExprHost ChartMarkerHost;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartDerivedSeriesExprHost> m_derivedSeriesCollectionHostsRemotable;

		public ChartDataPointInLegendExprHost DataPointInLegendHost;

		public virtual object TypeExpr => null;

		public virtual object SubtypeExpr => null;

		public virtual object LegendNameExpr => null;

		public virtual object LegendTextExpr => null;

		public virtual object ChartAreaNameExpr => null;

		public virtual object ValueAxisNameExpr => null;

		public virtual object CategoryAxisNameExpr => null;

		public virtual object HiddenExpr => null;

		public virtual object HideInLegendExpr => null;

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable => m_customPropertyHostsRemotable;

		internal IList<ChartDerivedSeriesExprHost> ChartDerivedSeriesCollectionHostsRemotable => m_derivedSeriesCollectionHostsRemotable;

		public virtual object ToolTipExpr => null;
	}
}
