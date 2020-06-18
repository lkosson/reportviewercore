using System;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ChartPaintEventArgs : EventArgs
	{
		private ChartGraphics chartGraph;

		private CommonElements common;

		private Chart chart;

		private ElementPosition position;

		public ChartGraphics ChartGraphics => chartGraph;

		internal CommonElements CommonElements => common;

		public ElementPosition Position => position;

		internal Chart Chart
		{
			get
			{
				if (chart == null && common != null && common.container != null)
				{
					chart = (Chart)common.container.GetService(typeof(Chart));
				}
				return chart;
			}
		}

		private ChartPaintEventArgs()
		{
		}

		public ChartPaintEventArgs(ChartGraphics chartGraph, CommonElements common, ElementPosition position)
		{
			this.chartGraph = chartGraph;
			this.common = common;
			this.position = position;
		}
	}
}
