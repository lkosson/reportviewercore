using System;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class HundredPercentStackedAreaChart : StackedAreaChart
	{
		private double[] totalPerPoint;

		public override string Name => "100%StackedArea";

		public override bool HundredPercent => true;

		public HundredPercentStackedAreaChart()
		{
			hundredPercentStacked = true;
		}

		public override void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			totalPerPoint = null;
			base.Paint(graph, common, area, seriesToDraw);
		}

		public override double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			if (totalPerPoint == null)
			{
				int num = 0;
				foreach (Series item in common.DataManager.Series)
				{
					if (string.Compare(item.ChartTypeName, Name, ignoreCase: true, CultureInfo.CurrentCulture) == 0 && item.ChartArea == area.Name && item.IsVisible())
					{
						num++;
					}
				}
				Series[] array = new Series[num];
				int num2 = 0;
				foreach (Series item2 in common.DataManager.Series)
				{
					if (string.Compare(item2.ChartTypeName, Name, ignoreCase: true, CultureInfo.CurrentCulture) == 0 && item2.ChartArea == area.Name && item2.IsVisible())
					{
						array[num2++] = item2;
					}
				}
				common.DataManipulator.CheckXValuesAlignment(array);
				totalPerPoint = new double[series.Points.Count];
				for (int i = 0; i < series.Points.Count; i++)
				{
					totalPerPoint[i] = 0.0;
					Series[] array2 = array;
					foreach (Series series4 in array2)
					{
						totalPerPoint[i] += Math.Abs(series4.Points[i].YValues[0]);
					}
				}
			}
			if (!area.Area3DStyle.Enable3D)
			{
				if (totalPerPoint[pointIndex] == 0.0)
				{
					int num3 = 0;
					foreach (Series item3 in common.DataManager.Series)
					{
						if (string.Compare(item3.ChartTypeName, Name, ignoreCase: true, CultureInfo.CurrentCulture) == 0 && item3.ChartArea == area.Name && item3.IsVisible())
						{
							num3++;
						}
					}
					return 100.0 / (double)num3;
				}
				return point.YValues[0] / totalPerPoint[pointIndex] * 100.0;
			}
			double num4 = double.NaN;
			if (yValueIndex == -1)
			{
				double num5 = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName).Crossing;
				num4 = GetYValue(common, area, series, point, pointIndex, 0);
				if (area.Area3DStyle.Enable3D && num4 < 0.0)
				{
					num4 = 0.0 - num4;
				}
				if (num4 >= 0.0)
				{
					if (!double.IsNaN(prevPosY))
					{
						num5 = prevPosY;
					}
				}
				else if (!double.IsNaN(prevNegY))
				{
					num5 = prevNegY;
				}
				return num4 - num5;
			}
			prevPosY = double.NaN;
			prevNegY = double.NaN;
			prevPositionX = double.NaN;
			foreach (Series item4 in common.DataManager.Series)
			{
				if (string.Compare(series.ChartArea, item4.ChartArea, ignoreCase: true, CultureInfo.CurrentCulture) == 0 && string.Compare(series.ChartTypeName, item4.ChartTypeName, ignoreCase: true, CultureInfo.CurrentCulture) == 0 && series.IsVisible())
				{
					num4 = item4.Points[pointIndex].YValues[0] / totalPerPoint[pointIndex] * 100.0;
					if (!double.IsNaN(num4) && area.Area3DStyle.Enable3D && num4 < 0.0)
					{
						num4 = 0.0 - num4;
					}
					if (num4 >= 0.0 && !double.IsNaN(prevPosY))
					{
						num4 += prevPosY;
					}
					if (num4 < 0.0 && !double.IsNaN(prevNegY))
					{
						num4 += prevNegY;
					}
					if (string.Compare(series.Name, item4.Name, StringComparison.Ordinal) == 0)
					{
						break;
					}
					if (num4 >= 0.0)
					{
						prevPosY = num4;
					}
					else
					{
						prevNegY = num4;
					}
					prevPositionX = item4.Points[pointIndex].XValue;
					if (prevPositionX == 0.0 && ChartElement.IndexedSeries(series))
					{
						prevPositionX = pointIndex + 1;
					}
				}
			}
			if (num4 > 100.0)
			{
				return 100.0;
			}
			return num4;
		}
	}
}
