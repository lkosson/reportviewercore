using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class DoughnutChart : PieChart
	{
		public override string Name => "Doughnut";

		public override bool Stacked => false;

		public override bool RequireAxes => false;

		public override bool SupportLogarithmicAxes => false;

		public override bool SwitchValueAxes => false;

		public override bool SideBySideSeries => false;

		public override bool ZeroCrossing => false;

		public override bool DataPointsInLegend => true;

		public override bool ExtraYValuesConnectedToYAxis => false;

		public override bool ApplyPaletteColorsToPoints => true;

		public override int YValuesPerPoint => 1;

		public override bool Doughnut => true;

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public override LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}
	}
}
