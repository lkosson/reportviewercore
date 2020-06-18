using Microsoft.Reporting.Chart.WebForms;
using System;

namespace Microsoft.Reporting.Chart.Helpers
{
	internal class ParetoHelper
	{
		public void MakeParetoChart(Microsoft.Reporting.Chart.WebForms.Chart chart, string srcSeriesName, string destSeriesName)
		{
			string chartArea = chart.Series[srcSeriesName].ChartArea;
			chart.Series[srcSeriesName].ChartType = SeriesChartType.Column;
			chart.DataManipulator.Sort(PointsSortOrder.Descending, srcSeriesName);
			double num = 0.0;
			foreach (DataPoint point in chart.Series[srcSeriesName].Points)
			{
				if (!point.Empty && !double.IsNaN(point.YValues[0]))
				{
					num += point.YValues[0];
				}
			}
			chart.ChartAreas[chartArea].AxisY.Maximum = num;
			Series series = new Series(destSeriesName);
			chart.Series.Add(series);
			series.ChartType = SeriesChartType.Line;
			series.ChartArea = chart.Series[srcSeriesName].ChartArea;
			if (chart.Series[srcSeriesName].Points.Count > 0)
			{
				series.BorderWidth = chart.Series[srcSeriesName].Points[0].BorderWidth;
			}
			series.Font = chart.Series[srcSeriesName].Font;
			series.FontAngle = chart.Series[srcSeriesName].FontAngle;
			series.FontColor = chart.Series[srcSeriesName].FontColor;
			series.Legend = chart.Series[srcSeriesName].Legend;
			series.YAxisType = AxisType.Secondary;
			chart.ChartAreas[chartArea].AxisY2.Maximum = 1.0;
			chart.ChartAreas[chartArea].AxisY2.LabelStyle.Format = "P0";
			double num2 = 0.0;
			foreach (DataPoint point2 in chart.Series[srcSeriesName].Points)
			{
				num2 += point2.YValues[0] / num;
				series.Points.Add(Math.Round(num2, 2));
			}
		}
	}
}
