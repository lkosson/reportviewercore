using System;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class BubbleChart : PointChart
	{
		private bool scaleDetected;

		private double maxPossibleBubbleSize = 15.0;

		private double minPossibleBubbleSize = 3.0;

		private float maxBubleSize;

		private float minBubleSize;

		private double minAll = double.MaxValue;

		private double maxAll = double.MinValue;

		private double valueDiff;

		private double valueScale = 1.0;

		public override string Name => "Bubble";

		public override int YValuesPerPoint => 2;

		public override bool SecondYScale => true;

		public BubbleChart()
			: base(alwaysDrawMarkers: true)
		{
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		protected override void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			scaleDetected = false;
			base.ProcessChartType(selection, graph, common, area, seriesToDraw);
		}

		protected override int GetMarkerBorderSize(DataPointAttributes point)
		{
			return point.BorderWidth;
		}

		protected override SizeF GetMarkerSize(ChartGraphics graph, CommonElements common, ChartArea area, DataPoint point, int markerSize, string markerImage)
		{
			if (point.YValues.Length < YValuesPerPoint)
			{
				throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(Name, YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
			}
			SizeF result = new SizeF(markerSize, markerSize);
			if (point.series.YValuesPerPoint > 1 && !point.Empty)
			{
				result.Width = ScaleBubbleSize(graph, common, area, point.YValues[1]);
				result.Height = ScaleBubbleSize(graph, common, area, point.YValues[1]);
			}
			return result;
		}

		private float ScaleBubbleSize(ChartGraphics graph, CommonElements common, ChartArea area, double value)
		{
			if (!scaleDetected)
			{
				minAll = double.MaxValue;
				maxAll = double.MinValue;
				foreach (Series item in common.DataManager.Series)
				{
					if (string.Compare(item.ChartTypeName, Name, ignoreCase: true, CultureInfo.CurrentCulture) != 0 || !(item.ChartArea == area.Name) || !item.IsVisible())
					{
						continue;
					}
					if (item.IsAttributeSet("BubbleScaleMin"))
					{
						minAll = Math.Min(minAll, CommonElements.ParseDouble(item["BubbleScaleMin"]));
					}
					if (item.IsAttributeSet("BubbleScaleMax"))
					{
						maxAll = Math.Max(maxAll, CommonElements.ParseDouble(item["BubbleScaleMax"]));
					}
					if (item.IsAttributeSet("BubbleMaxSize"))
					{
						maxPossibleBubbleSize = CommonElements.ParseDouble(item["BubbleMaxSize"]);
						if (maxPossibleBubbleSize < 0.0 || maxPossibleBubbleSize > 100.0)
						{
							throw new ArgumentException(SR.ExceptionCustomAttributeIsNotInRange0to100("BubbleMaxSize"));
						}
					}
					if (item.IsAttributeSet("BubbleMinSize"))
					{
						minPossibleBubbleSize = CommonElements.ParseDouble(item["BubbleMinSize"]);
						if (minPossibleBubbleSize < 0.0 || minPossibleBubbleSize > 100.0)
						{
							throw new ArgumentException(SR.ExceptionCustomAttributeIsNotInRange0to100("BubbleMinSize"));
						}
					}
					labelYValueIndex = 0;
					if (item.IsAttributeSet("BubbleUseSizeForLabel") && string.Compare(item["BubbleUseSizeForLabel"], "true", StringComparison.OrdinalIgnoreCase) == 0)
					{
						labelYValueIndex = 1;
						break;
					}
				}
				if (minAll == double.MaxValue || maxAll == double.MinValue)
				{
					double val = double.MaxValue;
					double val2 = double.MinValue;
					foreach (Series item2 in common.DataManager.Series)
					{
						if (!(item2.ChartTypeName == Name) || !(item2.ChartArea == area.Name) || !item2.IsVisible())
						{
							continue;
						}
						foreach (DataPoint point in item2.Points)
						{
							if (!point.Empty)
							{
								if (point.YValues.Length < YValuesPerPoint)
								{
									throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(Name, YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
								}
								val = Math.Min(val, point.YValues[1]);
								val2 = Math.Max(val2, point.YValues[1]);
							}
						}
					}
					if (minAll == double.MaxValue)
					{
						minAll = val;
					}
					if (maxAll == double.MinValue)
					{
						maxAll = val2;
					}
				}
				SizeF absoluteSize = graph.GetAbsoluteSize(area.PlotAreaPosition.GetSize());
				maxBubleSize = (float)((double)Math.Min(absoluteSize.Width, absoluteSize.Height) / (100.0 / maxPossibleBubbleSize));
				minBubleSize = (float)((double)Math.Min(absoluteSize.Width, absoluteSize.Height) / (100.0 / minPossibleBubbleSize));
				if (maxAll == minAll)
				{
					valueScale = 1.0;
					valueDiff = minAll - (double)((maxBubleSize - minBubleSize) / 2f);
				}
				else
				{
					valueScale = (double)(maxBubleSize - minBubleSize) / (maxAll - minAll);
					valueDiff = minAll;
				}
				scaleDetected = true;
			}
			if (value > maxAll)
			{
				return 0f;
			}
			if (value < minAll)
			{
				return 0f;
			}
			return (float)((value - valueDiff) * valueScale) + minBubleSize;
		}

		internal static double AxisScaleBubbleSize(ChartGraphics graph, CommonElements common, ChartArea area, double value, bool yValue)
		{
			double num = double.MaxValue;
			double num2 = double.MinValue;
			double num3 = 15.0;
			double num4 = 3.0;
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, "Bubble", StringComparison.OrdinalIgnoreCase) != 0 || !(item.ChartArea == area.Name) || !item.IsVisible())
				{
					continue;
				}
				if (item.IsAttributeSet("BubbleScaleMin"))
				{
					num = Math.Min(num, CommonElements.ParseDouble(item["BubbleScaleMin"]));
				}
				if (item.IsAttributeSet("BubbleScaleMax"))
				{
					num2 = Math.Max(num2, CommonElements.ParseDouble(item["BubbleScaleMax"]));
				}
				if (item.IsAttributeSet("BubbleMaxSize"))
				{
					num3 = CommonElements.ParseDouble(item["BubbleMaxSize"]);
					if (num3 < 0.0 || num3 > 100.0)
					{
						throw new ArgumentException(SR.ExceptionCustomAttributeIsNotInRange0to100("BubbleMaxSize"));
					}
				}
				if (item.IsAttributeSet("BubbleUseSizeForLabel") && string.Compare(item["BubbleUseSizeForLabel"], "true", StringComparison.OrdinalIgnoreCase) == 0)
				{
					break;
				}
			}
			double num5 = double.MaxValue;
			double num6 = double.MinValue;
			double num7 = double.MaxValue;
			double num8 = double.MinValue;
			foreach (Series item2 in common.DataManager.Series)
			{
				if (string.Compare(item2.ChartTypeName, "Bubble", StringComparison.OrdinalIgnoreCase) != 0 || !(item2.ChartArea == area.Name) || !item2.IsVisible())
				{
					continue;
				}
				foreach (DataPoint point in item2.Points)
				{
					if (!point.Empty)
					{
						num7 = Math.Min(num7, point.YValues[1]);
						num8 = Math.Max(num8, point.YValues[1]);
						if (yValue)
						{
							num5 = Math.Min(num5, point.YValues[0]);
							num6 = Math.Max(num6, point.YValues[0]);
						}
						else
						{
							num5 = Math.Min(num5, point.XValue);
							num6 = Math.Max(num6, point.XValue);
						}
					}
				}
			}
			if (num == double.MaxValue)
			{
				num = num7;
			}
			if (num2 == double.MinValue)
			{
				num2 = num8;
			}
			graph.GetAbsoluteSize(area.PlotAreaPosition.GetSize());
			float num9 = (float)((num6 - num5) / (100.0 / num3));
			float num10 = (float)((num6 - num5) / (100.0 / num4));
			double num11;
			double num12;
			if (num2 == num)
			{
				num11 = 1.0;
				num12 = num - (double)((num9 - num10) / 2f);
			}
			else
			{
				num11 = (double)(num9 - num10) / (num2 - num);
				num12 = num;
			}
			if (value > num2)
			{
				return 0.0;
			}
			if (value < num)
			{
				return 0.0;
			}
			return (float)((value - num12) * num11) + num10;
		}

		internal static double GetBubbleMaxSize(ChartArea area)
		{
			double num = 15.0;
			foreach (Series item in area.Common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, "Bubble", StringComparison.OrdinalIgnoreCase) == 0 && item.ChartArea == area.Name && item.IsVisible() && item.IsAttributeSet("BubbleMaxSize"))
				{
					num = CommonElements.ParseDouble(item["BubbleMaxSize"]);
					if (num < 0.0 || num > 100.0)
					{
						throw new ArgumentException(SR.ExceptionCustomAttributeIsNotInRange0to100("BubbleMaxSize"));
					}
				}
			}
			return num / 100.0;
		}
	}
}
