using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class ThreeLineBreakChart : IChartType
	{
		public virtual string Name => "ThreeLineBreak";

		public virtual bool Stacked => false;

		public virtual bool SupportStackedGroups => false;

		public bool StackSign => false;

		public virtual bool RequireAxes => true;

		public bool SecondYScale => false;

		public bool CircularChartArea => false;

		public virtual bool SupportLogarithmicAxes => true;

		public virtual bool SwitchValueAxes => false;

		public bool SideBySideSeries => false;

		public virtual bool DataPointsInLegend => false;

		public virtual bool ZeroCrossing => false;

		public virtual bool ApplyPaletteColorsToPoints => false;

		public virtual bool ExtraYValuesConnectedToYAxis => true;

		public virtual bool HundredPercent => false;

		public virtual bool HundredPercentSupportNegative => false;

		public virtual int YValuesPerPoint => 1;

		internal static void PrepareData(Series series, IServiceContainer serviceContainer)
		{
			if (string.Compare(series.ChartTypeName, "ThreeLineBreak", StringComparison.OrdinalIgnoreCase) != 0 || !series.IsVisible())
			{
				return;
			}
			Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
			if (chart == null)
			{
				throw new InvalidOperationException(SR.ExceptionThreeLineBreakNullReference);
			}
			ChartArea chartArea = chart.ChartAreas[series.ChartArea];
			foreach (Series item in chart.Series)
			{
				if (item.IsVisible() && item != series && chartArea == chart.ChartAreas[item.ChartArea])
				{
					throw new InvalidOperationException(SR.ExceptionThreeLineBreakCanNotCobine);
				}
			}
			Series series3 = new Series("THREELINEBREAK_ORIGINAL_DATA_" + series.Name, series.YValuesPerPoint);
			series3.Enabled = false;
			series3.ShowInLegend = false;
			chart.Series.Add(series3);
			foreach (DataPoint point in series.Points)
			{
				series3.Points.Add(point);
			}
			series.Points.Clear();
			if (series.IsAttributeSet("TempDesignData"))
			{
				series3["TempDesignData"] = "true";
			}
			series["OldXValueIndexed"] = series.XValueIndexed.ToString(CultureInfo.InvariantCulture);
			series["OldYValuesPerPoint"] = series.YValuesPerPoint.ToString(CultureInfo.InvariantCulture);
			series.ChartType = SeriesChartType.RangeColumn;
			series.XValueIndexed = true;
			series.YValuesPerPoint = 2;
			if (series.ChartArea.Length > 0 && series.IsXValueDateTime())
			{
				Axis axis = chartArea.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
				if (axis.Interval == 0.0 && axis.IntervalType == DateTimeIntervalType.Auto)
				{
					bool flag = false;
					double num = double.MaxValue;
					double num2 = double.MinValue;
					foreach (DataPoint point2 in series3.Points)
					{
						if (!point2.Empty)
						{
							if (point2.XValue != 0.0)
							{
								flag = true;
							}
							if (point2.XValue > num2)
							{
								num2 = point2.XValue;
							}
							if (point2.XValue < num)
							{
								num = point2.XValue;
							}
						}
					}
					if (flag)
					{
						series["OldAutomaticXAxisInterval"] = "true";
						DateTimeIntervalType type = DateTimeIntervalType.Auto;
						axis.interval = axis.CalcInterval(num, num2, date: true, out type, series.XValueType);
						axis.intervalType = type;
					}
				}
			}
			FillThreeLineBreakData(series, series3);
		}

		internal static bool UnPrepareData(Series series, IServiceContainer serviceContainer)
		{
			if (series.Name.StartsWith("THREELINEBREAK_ORIGINAL_DATA_", StringComparison.Ordinal))
			{
				Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
				if (chart == null)
				{
					throw new InvalidOperationException(SR.ExceptionThreeLineBreakNullReference);
				}
				Series series2 = chart.Series[series.Name.Substring(29)];
				series2.Points.Clear();
				if (!series.IsAttributeSet("TempDesignData"))
				{
					foreach (DataPoint point in series.Points)
					{
						series2.Points.Add(point);
					}
				}
				series2.ChartType = SeriesChartType.ThreeLineBreak;
				try
				{
					series2.XValueIndexed = bool.Parse(series2["OldXValueIndexed"]);
					series2.YValuesPerPoint = int.Parse(series2["OldYValuesPerPoint"], CultureInfo.InvariantCulture);
				}
				catch
				{
				}
				series2.DeleteAttribute("OldXValueIndexed");
				series2.DeleteAttribute("OldYValuesPerPoint");
				series["OldAutomaticXAxisInterval"] = "true";
				if (series2.IsAttributeSet("OldAutomaticXAxisInterval"))
				{
					series2.DeleteAttribute("OldAutomaticXAxisInterval");
					if (series2.ChartArea.Length > 0)
					{
						Axis axis = chart.ChartAreas[series2.ChartArea].GetAxis(AxisName.X, series2.XAxisType, series2.XSubAxisName);
						axis.interval = 0.0;
						axis.intervalType = DateTimeIntervalType.Auto;
					}
				}
				chart.Series.Remove(series);
				return true;
			}
			return false;
		}

		private static void FillThreeLineBreakData(Series series, Series originalData)
		{
			int num = 0;
			if (series.IsAttributeSet("UsedYValue"))
			{
				try
				{
					num = int.Parse(series["UsedYValue"], CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionThreeLineBreakUsedYValueInvalid);
				}
				if (num >= series.YValuesPerPoint)
				{
					throw new InvalidOperationException(SR.ExceptionThreeLineBreakUsedYValueOutOfRange);
				}
			}
			int num2 = 3;
			if (series.IsAttributeSet("NumberOfLinesInBreak"))
			{
				try
				{
					num2 = int.Parse(series["NumberOfLinesInBreak"], CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionThreeLineBreakNumberOfLinesInBreakFormatInvalid);
				}
				if (num2 <= 0)
				{
					throw new InvalidOperationException(SR.ExceptionThreeLineBreakNumberOfLinesInBreakValueInvalid);
				}
			}
			ArrayList arrayList = new ArrayList();
			double num3 = double.NaN;
			double num4 = double.NaN;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			foreach (DataPoint point in originalData.Points)
			{
				int num8 = 0;
				if (point.Empty)
				{
					num7++;
					continue;
				}
				if (double.IsNaN(num3) || double.IsNaN(num4))
				{
					num4 = point.YValues[num];
					num3 = point.YValues[num];
					num7++;
					continue;
				}
				Color color = Color.Transparent;
				string text = point["PriceUpColor"];
				if (text == null)
				{
					text = series["PriceUpColor"];
				}
				if (text != null)
				{
					try
					{
						color = (Color)new ColorConverter().ConvertFromString(null, CultureInfo.InvariantCulture, text);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionThreeLineBreakUpBrickColorInvalid);
					}
				}
				num8 = ((point.YValues[num] > num4) ? 1 : ((point.YValues[num] < num3) ? (-1) : 0));
				if (num8 != 0)
				{
					if (num6 == num8)
					{
						num5++;
					}
					else
					{
						if (num5 >= num2)
						{
							switch (num8)
							{
							case 1:
							{
								double num10 = double.MinValue;
								for (int j = 0; j < arrayList.Count; j += 2)
								{
									if ((double)arrayList[j] > num10)
									{
										num10 = (double)arrayList[j];
									}
								}
								if (point.YValues[num] <= num10)
								{
									num8 = 0;
								}
								break;
							}
							case -1:
							{
								double num9 = double.MaxValue;
								for (int i = 1; i < arrayList.Count; i += 2)
								{
									if ((double)arrayList[i] < num9)
									{
										num9 = (double)arrayList[i];
									}
								}
								if (point.YValues[num] >= num9)
								{
									num8 = 0;
								}
								break;
							}
							}
						}
						if (num8 != 0)
						{
							num5 = 1;
						}
					}
					if (num8 != 0)
					{
						DataPoint dataPoint2 = point.Clone();
						dataPoint2["OriginalPointIndex"] = num7.ToString(CultureInfo.InvariantCulture);
						dataPoint2.series = series;
						dataPoint2.YValues = new double[2];
						dataPoint2.XValue = point.XValue;
						if (num8 == 1)
						{
							dataPoint2.YValues[1] = num4;
							dataPoint2.YValues[0] = point.YValues[num];
							num3 = num4;
							num4 = point.YValues[num];
							dataPoint2.Color = color;
							if (dataPoint2.BorderWidth < 1)
							{
								dataPoint2.BorderWidth = 1;
							}
							if (dataPoint2.BorderStyle == ChartDashStyle.NotSet)
							{
								dataPoint2.BorderStyle = ChartDashStyle.Solid;
							}
							if ((dataPoint2.BorderColor == Color.Empty || dataPoint2.BorderColor == Color.Transparent) && (dataPoint2.Color == Color.Empty || dataPoint2.Color == Color.Transparent))
							{
								dataPoint2.BorderColor = series.Color;
							}
						}
						else
						{
							dataPoint2.YValues[1] = num3;
							dataPoint2.YValues[0] = point.YValues[num];
							num4 = num3;
							num3 = point.YValues[num];
						}
						series.Points.Add(dataPoint2);
						arrayList.Add(num4);
						arrayList.Add(num3);
						if (arrayList.Count > num2 * 2)
						{
							arrayList.RemoveAt(0);
							arrayList.RemoveAt(0);
						}
					}
				}
				if (num8 != 0)
				{
					num6 = num8;
				}
				num7++;
			}
		}

		public virtual void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
		}

		public virtual LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
