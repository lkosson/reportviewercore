using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartDataPointExprHost : CellExprHost
	{
		public StyleExprHost StyleHost;

		public ChartDataPointInLegendExprHost DataPointInLegendHost;

		public ChartDataLabelExprHost DataLabelHost;

		public ChartMarkerExprHost ChartMarkerHost;

		public ActionInfoExprHost ActionInfoHost;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_customPropertyHostsRemotable;

		public virtual object DataLabelValueExpr => null;

		public virtual object DataLabelVisibleExpr => null;

		public virtual object DataPointValuesXExpr => null;

		public virtual object DataPointValuesYExpr => null;

		public virtual object DataPointValuesSizeExpr => null;

		public virtual object DataPointValuesHighExpr => null;

		public virtual object DataPointValuesLowExpr => null;

		public virtual object DataPointValuesStartExpr => null;

		public virtual object DataPointValuesEndExpr => null;

		public virtual object DataPointValuesMeanExpr => null;

		public virtual object DataPointValuesMedianExpr => null;

		public virtual object DataPointValuesHighlightXExpr => null;

		public virtual object DataPointValuesHighlightYExpr => null;

		public virtual object DataPointValuesHighlightSizeExpr => null;

		public virtual object DataPointValuesFormatXExpr => null;

		public virtual object DataPointValuesFormatYExpr => null;

		public virtual object DataPointValuesFormatSizeExpr => null;

		public virtual object DataPointValuesCurrencyLanguageXExpr => null;

		public virtual object DataPointValuesCurrencyLanguageYExpr => null;

		public virtual object DataPointValuesCurrencyLanguageSizeExpr => null;

		public virtual object AxisLabelExpr => null;

		internal IList<DataValueExprHost> CustomPropertyHostsRemotable => m_customPropertyHostsRemotable;

		public virtual object ToolTipExpr => null;
	}
}
