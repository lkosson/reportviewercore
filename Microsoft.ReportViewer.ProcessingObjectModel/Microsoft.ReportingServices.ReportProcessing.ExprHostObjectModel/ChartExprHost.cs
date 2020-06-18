using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class ChartExprHost : DataRegionExprHost
	{
		public MultiChartExprHost MultiChartHost;

		public ChartDynamicGroupExprHost RowGroupingsHost;

		public IndexedExprHost StaticRowLabelsHost;

		public ChartDynamicGroupExprHost ColumnGroupingsHost;

		public IndexedExprHost StaticColumnLabelsHost;

		protected ChartDataPointExprHost[] ChartDataPointHosts;

		[CLSCompliant(false)]
		protected IList<ChartDataPointExprHost> m_chartDataPointHostsRemotable;

		public ChartTitleExprHost TitleHost;

		public AxisExprHost CategoryAxisHost;

		public AxisExprHost ValueAxisHost;

		public StyleExprHost LegendHost;

		public StyleExprHost PlotAreaHost;

		internal IList<ChartDataPointExprHost> ChartDataPointHostsRemotable
		{
			get
			{
				if (m_chartDataPointHostsRemotable == null && ChartDataPointHosts != null)
				{
					m_chartDataPointHostsRemotable = new RemoteArrayWrapper<ChartDataPointExprHost>(ChartDataPointHosts);
				}
				return m_chartDataPointHostsRemotable;
			}
		}
	}
}
