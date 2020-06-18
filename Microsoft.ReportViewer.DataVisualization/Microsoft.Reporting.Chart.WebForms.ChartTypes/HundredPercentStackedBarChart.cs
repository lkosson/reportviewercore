using System;
using System.Collections;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class HundredPercentStackedBarChart : StackedBarChart
	{
		private Hashtable stackedGroupsTotalPerPoint;

		public override string Name => "100%StackedBar";

		public override bool HundredPercent => true;

		public override bool HundredPercentSupportNegative => true;

		public HundredPercentStackedBarChart()
		{
			hundredPercentStacked = true;
		}

		public override void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			stackedGroupsTotalPerPoint = null;
			base.Paint(graph, common, area, seriesToDraw);
		}

		public override double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			double[] array = null;
			string seriesStackGroupName = StackedColumnChart.GetSeriesStackGroupName(series);
			if (stackedGroupsTotalPerPoint == null)
			{
				stackedGroupsTotalPerPoint = new Hashtable();
				foreach (string stackGroupName in stackGroupNames)
				{
					Series[] seriesByStackedGroupName = StackedColumnChart.GetSeriesByStackedGroupName(common, stackGroupName, series.ChartTypeName, series.ChartArea);
					common.DataManipulator.CheckXValuesAlignment(seriesByStackedGroupName);
					double[] array2 = new double[series.Points.Count];
					for (int i = 0; i < series.Points.Count; i++)
					{
						array2[i] = 0.0;
						Series[] array3 = seriesByStackedGroupName;
						foreach (Series series2 in array3)
						{
							array2[i] += Math.Abs(series2.Points[i].YValues[0]);
						}
					}
					stackedGroupsTotalPerPoint.Add(stackGroupName, array2);
				}
			}
			array = (double[])stackedGroupsTotalPerPoint[seriesStackGroupName];
			if (!area.Area3DStyle.Enable3D && (point.YValues[0] == 0.0 || point.Empty))
			{
				return 0.0;
			}
			if (!area.Area3DStyle.Enable3D || yValueIndex == -2)
			{
				if (array[pointIndex] == 0.0)
				{
					return 0.0;
				}
				return point.YValues[0] / array[pointIndex] * 100.0;
			}
			double num = double.NaN;
			if (yValueIndex == -1)
			{
				double num2 = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName).Crossing;
				num = GetYValue(common, area, series, point, pointIndex, 0);
				if (num >= 0.0)
				{
					if (!double.IsNaN(prevPosY))
					{
						num2 = prevPosY;
					}
				}
				else if (!double.IsNaN(prevNegY))
				{
					num2 = prevNegY;
				}
				return num - num2;
			}
			prevPosY = double.NaN;
			prevNegY = double.NaN;
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(series.ChartArea, item.ChartArea, StringComparison.Ordinal) != 0 || string.Compare(series.ChartTypeName, item.ChartTypeName, StringComparison.OrdinalIgnoreCase) != 0 || !item.IsVisible() || seriesStackGroupName != StackedColumnChart.GetSeriesStackGroupName(item))
				{
					continue;
				}
				if (double.IsNaN(num))
				{
					num = ((array[pointIndex] != 0.0) ? (item.Points[pointIndex].YValues[0] / array[pointIndex] * 100.0) : 0.0);
				}
				else
				{
					num = ((array[pointIndex] != 0.0) ? (item.Points[pointIndex].YValues[0] / array[pointIndex] * 100.0) : 0.0);
					if (num >= 0.0 && !double.IsNaN(prevPosY))
					{
						num += prevPosY;
					}
					if (num < 0.0 && !double.IsNaN(prevNegY))
					{
						num += prevNegY;
					}
				}
				if (string.Compare(series.Name, item.Name, StringComparison.Ordinal) == 0)
				{
					break;
				}
				if (num >= 0.0)
				{
					prevPosY = num;
				}
				else
				{
					prevNegY = num;
				}
			}
			if (!(num > 100.0))
			{
				return num;
			}
			return 100.0;
		}
	}
}
