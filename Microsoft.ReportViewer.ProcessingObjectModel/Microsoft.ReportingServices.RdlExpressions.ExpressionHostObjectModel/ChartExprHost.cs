using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ChartExprHost : DataRegionExprHost<ChartMemberExprHost, ChartDataPointExprHost>
	{
		[CLSCompliant(false)]
		protected IList<ChartSeriesExprHost> m_seriesCollectionHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartDerivedSeriesExprHost> m_derivedSeriesCollectionHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartAreaExprHost> m_chartAreasHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartTitleExprHost> m_titlesHostsRemotable;

		[CLSCompliant(false)]
		protected IList<ChartLegendExprHost> m_legendsHostsRemotable;

		[CLSCompliant(false)]
		protected IList<DataValueExprHost> m_codeParametersHostsRemotable;

		public ChartTitleExprHost NoDataMessageHost;

		public ChartBorderSkinExprHost BorderSkinHost;

		[CLSCompliant(false)]
		protected IList<ChartCustomPaletteColorExprHost> m_customPaletteColorHostsRemotable;

		internal IList<ChartSeriesExprHost> SeriesCollectionHostsRemotable => m_seriesCollectionHostsRemotable;

		internal IList<ChartDerivedSeriesExprHost> ChartDerivedSeriesCollectionHostsRemotable => m_derivedSeriesCollectionHostsRemotable;

		internal IList<ChartAreaExprHost> ChartAreasHostsRemotable => m_chartAreasHostsRemotable;

		internal IList<ChartTitleExprHost> TitlesHostsRemotable => m_titlesHostsRemotable;

		internal IList<ChartLegendExprHost> LegendsHostsRemotable => m_legendsHostsRemotable;

		internal IList<DataValueExprHost> CodeParametersHostsRemotable => m_codeParametersHostsRemotable;

		public virtual object DynamicHeightExpr => null;

		public virtual object DynamicWidthExpr => null;

		public virtual object PaletteExpr => null;

		internal IList<ChartCustomPaletteColorExprHost> CustomPaletteColorHostsRemotable => m_customPaletteColorHostsRemotable;

		public virtual object PaletteHatchBehaviorExpr => null;
	}
}
