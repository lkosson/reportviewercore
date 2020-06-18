using Microsoft.Reporting.Chart.WebForms;
using System;

namespace Microsoft.Reporting.Chart.Helpers
{
	internal class HistogramHelper
	{
		public int SegmentIntervalNumber = 20;

		public double SegmentIntervalWidth;

		public bool ShowPercentOnSecondaryYAxis = true;

		public void CreateHistogram(Microsoft.Reporting.Chart.WebForms.Chart chartControl, string dataSeriesName, string histogramSeriesName, string histogramLegendText)
		{
			if (chartControl == null)
			{
				throw new ArgumentNullException("chartControl");
			}
			int index = chartControl.Series.GetIndex(dataSeriesName);
			if (index < 0)
			{
				throw new ArgumentException("Series with name'" + dataSeriesName + "' was not found.", "dataSeriesName");
			}
			Series series = chartControl.Series[dataSeriesName];
			Series series2 = null;
			if (chartControl.Series.GetIndex(histogramSeriesName) < 0)
			{
				series2 = new Series(histogramSeriesName);
				chartControl.Series.Insert(index, series2);
				series2.ChartType = series.ChartType;
				series2.LegendText = histogramLegendText;
				if (series.Points.Count > 0)
				{
					series2.BorderColor = series.Points[0].BorderColor;
					series2.BorderWidth = series.Points[0].BorderWidth;
					series2.BorderStyle = series.Points[0].BorderStyle;
				}
				series2.Color = series.Color;
				series2.BackGradientEndColor = series.BackGradientEndColor;
				series2.BackGradientType = series.BackGradientType;
				series2.Legend = series.Legend;
				series2.ChartArea = series.ChartArea;
				DataPointAttributes dataPointAttributes = series;
				for (int i = 0; i < series.Points.Count; i++)
				{
					if (!series.Points[i].Empty)
					{
						dataPointAttributes = series.Points[i];
						break;
					}
				}
				series2.LabelBackColor = dataPointAttributes.LabelBackColor;
				series2.LabelBorderColor = dataPointAttributes.LabelBorderColor;
				series2.LabelBorderWidth = dataPointAttributes.LabelBorderWidth;
				series2.LabelBorderStyle = dataPointAttributes.LabelBorderStyle;
				series2.LabelFormat = dataPointAttributes.LabelFormat;
				series2.ShowLabelAsValue = dataPointAttributes.ShowLabelAsValue;
				series2.Font = dataPointAttributes.Font;
				series2.FontColor = dataPointAttributes.FontColor;
				series2.FontAngle = dataPointAttributes.FontAngle;
			}
			double num = double.MaxValue;
			double num2 = double.MinValue;
			int num3 = 0;
			foreach (DataPoint point in series.Points)
			{
				if (!point.Empty)
				{
					if (point.YValues[0] > num2)
					{
						num2 = point.YValues[0];
					}
					if (point.YValues[0] < num)
					{
						num = point.YValues[0];
					}
					num3++;
				}
			}
			if (SegmentIntervalWidth == 0.0)
			{
				SegmentIntervalWidth = (num2 - num) / (double)SegmentIntervalNumber;
				SegmentIntervalWidth = RoundInterval(SegmentIntervalWidth);
			}
			num = Math.Floor(num / SegmentIntervalWidth) * SegmentIntervalWidth;
			num2 = Math.Ceiling(num2 / SegmentIntervalWidth) * SegmentIntervalWidth;
			double num4 = num;
			for (num4 = num; num4 <= num2; num4 += SegmentIntervalWidth)
			{
				int num5 = 0;
				foreach (DataPoint point2 in series.Points)
				{
					if (!point2.Empty)
					{
						double num6 = num4 + SegmentIntervalWidth;
						if (point2.YValues[0] >= num4 && point2.YValues[0] < num6)
						{
							num5++;
						}
						else if (num6 >= num2 && point2.YValues[0] >= num4 && point2.YValues[0] <= num6)
						{
							num5++;
						}
					}
				}
				series2.Points.AddXY(num4 + SegmentIntervalWidth / 2.0, num5);
			}
			series2["PointWidth"] = "1";
			ChartArea chartArea = chartControl.ChartAreas[series2.ChartArea];
			chartArea.AxisY.Title = "Frequency";
			chartArea.AxisX.Minimum = num;
			chartArea.AxisX.Maximum = num2;
			double num7 = SegmentIntervalWidth;
			bool flag = false;
			while ((num2 - num) / num7 > 10.0)
			{
				num7 *= 2.0;
				flag = true;
			}
			chartArea.AxisX.Interval = num7;
			if (chartArea.AxisX.LabelStyle.ShowEndLabels && flag)
			{
				chartArea.AxisX.Maximum = num + Math.Ceiling((num2 - num) / num7) * num7;
			}
			chartControl.Series.Remove(series);
			chartArea.AxisY2.Enabled = AxisEnabled.Auto;
			if (ShowPercentOnSecondaryYAxis)
			{
				chartArea.Recalculate();
				chartArea.AxisY2.Enabled = AxisEnabled.True;
				chartArea.AxisY2.LabelStyle.Format = "P0";
				chartArea.AxisY2.MajorGrid.Enabled = false;
				chartArea.AxisY2.Title = "Percent of Total";
				chartArea.AxisY2.Minimum = chartArea.AxisY.Minimum / (double)num3;
				chartArea.AxisY2.Maximum = chartArea.AxisY.Maximum / (double)num3;
				double num8 = (chartArea.AxisY2.Maximum > 0.2) ? 0.05 : 0.01;
				chartArea.AxisY2.Interval = Math.Ceiling(chartArea.AxisY2.Maximum / 5.0 / num8) * num8;
			}
		}

		internal double RoundInterval(double interval)
		{
			if (interval == 0.0)
			{
				throw new ArgumentOutOfRangeException("interval", "Interval can not be zero.");
			}
			double num = -1.0;
			double num2 = interval;
			while (num2 > 1.0)
			{
				num += 1.0;
				num2 /= 10.0;
				if (num > 1000.0)
				{
					throw new InvalidOperationException("Auto interval error due to invalid point values or axis minimum/maximum.");
				}
			}
			num2 = interval;
			if (num2 < 1.0)
			{
				num = 0.0;
			}
			while (num2 < 1.0)
			{
				num -= 1.0;
				num2 *= 10.0;
				if (num < -1000.0)
				{
					throw new InvalidOperationException("Auto interval error due to invalid point values or axis minimum/maximum.");
				}
			}
			double num3 = interval / Math.Pow(10.0, num);
			num3 = ((num3 < 3.0) ? 2.0 : ((!(num3 < 7.0)) ? 10.0 : 5.0));
			return num3 * Math.Pow(10.0, num);
		}
	}
}
