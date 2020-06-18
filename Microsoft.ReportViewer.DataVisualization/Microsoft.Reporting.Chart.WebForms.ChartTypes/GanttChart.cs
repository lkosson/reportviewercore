namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class GanttChart : BarChart
	{
		public override string Name => "Gantt";

		public override bool ZeroCrossing => true;

		public override int YValuesPerPoint => 2;

		public override bool ExtraYValuesConnectedToYAxis => true;

		public GanttChart()
		{
			useTwoValues = true;
			defLabelDrawingStyle = BarValueLabelDrawingStyle.Center;
		}
	}
}
