using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class PieChart : IChartType
	{
		private enum LabelsMode
		{
			Off,
			Draw,
			EstimateSize,
			LabelsOverlap
		}

		internal class LabelColumn
		{
			private RectangleF chartAreaPosition;

			private RectangleF innerPlotPosition;

			internal float columnHeight;

			internal int numOfItems;

			private int numOfInsertedLabels;

			private DataPoint[] points;

			private float[] yPositions;

			private bool rightPosition = true;

			private float labelLineSize;

			public LabelColumn(RectangleF position)
			{
				chartAreaPosition = position;
			}

			internal int GetLabelIndex(float y)
			{
				if (y < chartAreaPosition.Y)
				{
					y = chartAreaPosition.Y;
				}
				else if (y > chartAreaPosition.Bottom)
				{
					y = chartAreaPosition.Bottom - columnHeight;
				}
				return (int)((y - chartAreaPosition.Y) / columnHeight);
			}

			internal void Sort()
			{
				for (int i = 0; i < points.Length; i++)
				{
					for (int j = 0; j < i; j++)
					{
						if (yPositions[i] < yPositions[j] && points[i] != null && points[j] != null)
						{
							float num = yPositions[i];
							DataPoint dataPoint = points[i];
							yPositions[i] = yPositions[j];
							points[i] = points[j];
							yPositions[j] = num;
							points[j] = dataPoint;
						}
					}
				}
			}

			internal float GetLabelPosition(int index)
			{
				if (index < 0 || index > numOfItems - 1)
				{
					throw new InvalidOperationException(SR.Exception3DPieLabelsIndexInvalid);
				}
				return chartAreaPosition.Y + columnHeight * (float)index + columnHeight / 2f;
			}

			internal PointF GetLabelPosition(DataPoint dataPoint)
			{
				PointF empty = PointF.Empty;
				int num = 0;
				DataPoint[] array = points;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == dataPoint)
					{
						empty.Y = GetLabelPosition(num);
						break;
					}
					num++;
				}
				if (rightPosition)
				{
					empty.X = innerPlotPosition.Right + chartAreaPosition.Width * labelLineSize;
				}
				else
				{
					empty.X = innerPlotPosition.Left - chartAreaPosition.Width * labelLineSize;
				}
				float num2 = (float)Math.Atan((empty.Y - innerPlotPosition.Top - innerPlotPosition.Height / 2f) / (empty.X - innerPlotPosition.Left - innerPlotPosition.Width / 2f));
				float num3 = (Math.Cos(num2) != 0.0) ? ((float)((double)innerPlotPosition.Width * 0.4 - (double)innerPlotPosition.Width * 0.4 / Math.Cos(num2))) : 0f;
				if (rightPosition)
				{
					empty.X += num3;
				}
				else
				{
					empty.X -= num3;
				}
				return empty;
			}

			internal void InsertLabel(DataPoint point, float yCoordinate, int pointIndx)
			{
				int labelIndex = GetLabelIndex(yCoordinate);
				if (points[labelIndex] != null)
				{
					if (pointIndx % 2 == 0)
					{
						if (CheckFreeSpace(labelIndex, upDirection: false))
						{
							MoveLabels(labelIndex, upDirection: false);
						}
						else
						{
							MoveLabels(labelIndex, upDirection: true);
						}
					}
					else if (CheckFreeSpace(labelIndex, upDirection: true))
					{
						MoveLabels(labelIndex, upDirection: true);
					}
					else
					{
						MoveLabels(labelIndex, upDirection: false);
					}
				}
				points[labelIndex] = point;
				yPositions[labelIndex] = yCoordinate;
				numOfInsertedLabels++;
			}

			private void MoveLabels(int position, bool upDirection)
			{
				if (upDirection)
				{
					DataPoint dataPoint = points[position];
					float num = yPositions[position];
					points[position] = null;
					yPositions[position] = 0f;
					int num2 = position;
					while (true)
					{
						if (num2 > 0)
						{
							if (points[num2 - 1] == null)
							{
								break;
							}
							DataPoint dataPoint2 = points[num2 - 1];
							float num3 = yPositions[num2 - 1];
							points[num2 - 1] = dataPoint;
							yPositions[num2 - 1] = num;
							dataPoint = dataPoint2;
							num = num3;
							num2--;
							continue;
						}
						return;
					}
					points[num2 - 1] = dataPoint;
					yPositions[num2 - 1] = num;
					return;
				}
				DataPoint dataPoint3 = points[position];
				float num4 = yPositions[position];
				points[position] = null;
				yPositions[position] = 0f;
				int num5 = position;
				while (true)
				{
					if (num5 < numOfItems - 1)
					{
						if (points[num5 + 1] == null)
						{
							break;
						}
						DataPoint dataPoint4 = points[num5 + 1];
						float num6 = yPositions[num5 + 1];
						points[num5 + 1] = dataPoint3;
						yPositions[num5 + 1] = num4;
						dataPoint3 = dataPoint4;
						num4 = num6;
						num5++;
						continue;
					}
					return;
				}
				points[num5 + 1] = dataPoint3;
				yPositions[num5 + 1] = num4;
			}

			internal void AdjustPositions()
			{
				int num = 0;
				int num2 = 0;
				if (numOfInsertedLabels < points.Length / 2)
				{
					return;
				}
				for (int i = 0; i < points.Length && points[i] == null; i++)
				{
					num++;
				}
				int num3 = points.Length - 1;
				while (num3 >= 0 && points[num3] == null)
				{
					num2++;
					num3--;
				}
				bool flag = (num > num2) ? true : false;
				int num4 = (num + num2) / 2;
				if (Math.Abs(num - num2) < 2)
				{
					return;
				}
				if (flag)
				{
					int num5 = 0;
					for (int j = num4; j < points.Length; j++)
					{
						if (num + num5 > points.Length - 1)
						{
							break;
						}
						points[j] = points[num + num5];
						points[num + num5] = null;
						num5++;
					}
				}
				else
				{
					int num6 = points.Length - 1;
					int num7 = points.Length - 1 - num4;
					while (num7 >= 0 && num6 - num2 >= 0)
					{
						points[num7] = points[num6 - num2];
						points[num6 - num2] = null;
						num6--;
						num7--;
					}
				}
			}

			private bool CheckFreeSpace(int position, bool upDirection)
			{
				if (upDirection)
				{
					if (position == 0)
					{
						return false;
					}
					for (int num = position - 1; num >= 0; num--)
					{
						if (points[num] == null)
						{
							return true;
						}
					}
				}
				else
				{
					if (position == numOfItems - 1)
					{
						return false;
					}
					for (int i = position + 1; i < numOfItems; i++)
					{
						if (points[i] == null)
						{
							return true;
						}
					}
				}
				return false;
			}

			internal void Initialize(RectangleF rectangle, bool rightPosition, int maxNumOfRows, float labelLineSize)
			{
				numOfItems = Math.Max(numOfItems, maxNumOfRows);
				columnHeight = chartAreaPosition.Height / (float)numOfItems;
				innerPlotPosition = rectangle;
				points = new DataPoint[numOfItems];
				yPositions = new float[numOfItems];
				this.rightPosition = rightPosition;
				this.labelLineSize = labelLineSize;
			}
		}

		private bool labelsFit = true;

		private float sizeCorrection = 0.95f;

		private bool sliceExploded;

		private bool labelsOverlap;

		internal LabelColumn labelColumnLeft;

		internal LabelColumn labelColumnRight;

		private ArrayList labelsRectangles = new ArrayList();

		public virtual string Name => "Pie";

		public virtual bool Stacked => false;

		public virtual bool SupportStackedGroups => false;

		public bool StackSign => false;

		public virtual bool RequireAxes => false;

		public bool SecondYScale => false;

		public bool CircularChartArea => false;

		public virtual bool SupportLogarithmicAxes => false;

		public virtual bool SwitchValueAxes => false;

		public virtual bool SideBySideSeries => false;

		public virtual bool ZeroCrossing => false;

		public virtual bool DataPointsInLegend => true;

		public virtual bool ExtraYValuesConnectedToYAxis => false;

		public virtual bool HundredPercent => false;

		public virtual bool HundredPercentSupportNegative => false;

		public virtual bool ApplyPaletteColorsToPoints => true;

		public virtual int YValuesPerPoint => 1;

		public virtual bool Doughnut => false;

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public virtual LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		internal static void PrepareData(Series series, IServiceContainer serviceContainer)
		{
			if (string.Compare(series.ChartTypeName, "Pie", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(series.ChartTypeName, "Doughnut", StringComparison.OrdinalIgnoreCase) != 0)
			{
				return;
			}
			double num = 0.0;
			if (series.IsAttributeSet("CollectedThreshold"))
			{
				try
				{
					num = double.Parse(series["CollectedThreshold"], CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionDoughnutCollectedThresholdInvalidFormat);
				}
				if (num < 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionDoughnutThresholdInvalid);
				}
			}
			if (series.IsAttributeSet("CollectedStyle") && string.Equals(series["CollectedStyle"], CollectedPieStyle.SingleSlice.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				if (!series.IsAttributeSet("CollectedThreshold"))
				{
					num = 5.0;
				}
			}
			else
			{
				num = 0.0;
			}
			if (!(num > 0.0))
			{
				return;
			}
			Chart obj2 = ((Chart)serviceContainer.GetService(typeof(Chart))) ?? throw new InvalidOperationException(SR.ExceptionDoughnutNullReference);
			Series series2 = new Series("PIE_ORIGINAL_DATA_" + series.Name, series.YValuesPerPoint)
			{
				Enabled = false,
				ShowInLegend = false
			};
			obj2.Series.Add(series2);
			foreach (DataPoint point in series.Points)
			{
				series2.Points.Add(point.Clone());
			}
			if (series.IsAttributeSet("TempDesignData"))
			{
				series2["TempDesignData"] = "true";
			}
			double num2 = 0.0;
			foreach (DataPoint point2 in series.Points)
			{
				if (!point2.Empty)
				{
					num2 += Math.Abs(point2.YValues[0]);
				}
			}
			bool flag = true;
			if (series.IsAttributeSet("CollectedThresholdUsePercent"))
			{
				if (string.Compare(series["CollectedThresholdUsePercent"], "True", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
				}
				else
				{
					if (string.Compare(series["CollectedThresholdUsePercent"], "False", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new InvalidOperationException(SR.ExceptionDoughnutCollectedThresholdUsePercentInvalid);
					}
					flag = false;
				}
			}
			if (flag)
			{
				if (num > 100.0)
				{
					throw new InvalidOperationException(SR.ExceptionDoughnutCollectedThresholdInvalidRange);
				}
				num = num2 * num / 100.0;
			}
			DataPoint dataPoint3 = null;
			double num3 = 0.0;
			int num4 = 0;
			int index = 0;
			int num5 = 0;
			for (int i = 0; i < series.Points.Count; i++)
			{
				DataPoint dataPoint4 = series.Points[i];
				if (!dataPoint4.Empty && Math.Abs(dataPoint4.YValues[0]) <= num)
				{
					num4++;
					num3 += Math.Abs(dataPoint4.YValues[0]);
					if (dataPoint3 == null)
					{
						index = i;
						dataPoint3 = dataPoint4.Clone();
						dataPoint3.ToolTip = string.Empty;
					}
					if (num4 == 2)
					{
						series.Points.RemoveAt(index);
						i--;
					}
					if (num4 > 1)
					{
						series.Points.RemoveAt(i);
						i--;
					}
				}
				dataPoint4["OriginalPointIndex"] = num5.ToString(CultureInfo.InvariantCulture);
				num5++;
			}
			if (num4 <= 1 || dataPoint3 == null)
			{
				return;
			}
			dataPoint3["_COLLECTED_DATA_POINT"] = "TRUE";
			dataPoint3.YValues[0] = num3;
			series.Points.Add(dataPoint3);
			if (series.IsAttributeSet("CollectedColor"))
			{
				ColorConverter colorConverter = new ColorConverter();
				try
				{
					dataPoint3.Color = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, series["CollectedColor"]);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionDoughnutCollectedColorInvalidFormat);
				}
			}
			if (series.IsAttributeSet("CollectedSliceExploded"))
			{
				dataPoint3["Exploded"] = series["CollectedSliceExploded"];
			}
			if (series.IsAttributeSet("CollectedToolTip"))
			{
				dataPoint3.ToolTip = series["CollectedToolTip"];
			}
			if (series.IsAttributeSet("CollectedLegendText"))
			{
				dataPoint3.LegendText = series["CollectedLegendText"];
			}
			else
			{
				dataPoint3.LegendText = "Other";
			}
			if (series.IsAttributeSet("CollectedLabel"))
			{
				dataPoint3.Label = series["CollectedLabel"];
			}
		}

		internal static bool UnPrepareData(Series series, IServiceContainer serviceContainer)
		{
			if (series.Name.StartsWith("PIE_ORIGINAL_DATA_", StringComparison.Ordinal))
			{
				Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
				if (chart == null)
				{
					throw new InvalidOperationException(SR.ExceptionDoughnutNullReference);
				}
				Series series2 = chart.Series[series.Name.Substring(18)];
				series2.Points.Clear();
				if (!series.IsAttributeSet("TempDesignData"))
				{
					foreach (DataPoint point in series.Points)
					{
						series2.Points.Add(point);
					}
				}
				chart.Series.Remove(series);
				return true;
			}
			return false;
		}

		public void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			foreach (Series item in common.DataManager.Series)
			{
				if (item.IsVisible() && item.ChartArea == area.Name && string.Compare(item.ChartTypeName, Name, ignoreCase: true, CultureInfo.CurrentCulture) != 0 && !common.ChartPicture.SuppressExceptions)
				{
					throw new InvalidOperationException(SR.ExceptionChartCanNotCombine(Name));
				}
			}
			if (area.Area3DStyle.Enable3D)
			{
				float pieWidth = 10 * area.Area3DStyle.PointDepth / 100;
				graph.SetClip(area.Position.ToRectangleF());
				area.Area3DStyle.XAngle *= -1;
				int yAngle = area.Area3DStyle.YAngle;
				area.Area3DStyle.YAngle = area.GetRealYAngle();
				ProcessChartType3D(selection: false, graph, common, area, shadow: false, LabelsMode.Off, seriesToDraw, pieWidth);
				area.Area3DStyle.XAngle *= -1;
				area.Area3DStyle.YAngle = yAngle;
				graph.ResetClip();
				return;
			}
			labelsOverlap = false;
			graph.SetClip(area.Position.ToRectangleF());
			SizeCorrection(graph, common, area);
			ProcessChartType(selection: false, graph, common, area, shadow: false, LabelsMode.LabelsOverlap, seriesToDraw);
			if (labelsOverlap)
			{
				SizeCorrection(graph, common, area);
				labelsOverlap = false;
				ProcessChartType(selection: false, graph, common, area, shadow: false, LabelsMode.LabelsOverlap, seriesToDraw);
			}
			ProcessChartType(selection: false, graph, common, area, shadow: true, LabelsMode.Off, seriesToDraw);
			ProcessChartType(selection: false, graph, common, area, shadow: false, LabelsMode.Off, seriesToDraw);
			ProcessChartType(selection: false, graph, common, area, shadow: false, LabelsMode.Draw, seriesToDraw);
			graph.ResetClip();
		}

		private double MinimumRelativePieSize(ChartArea area)
		{
			double num = 0.3;
			ArrayList seriesFromChartType = area.GetSeriesFromChartType(Name);
			SeriesCollection series = area.Common.DataManager.Series;
			if (series[seriesFromChartType[0]].IsAttributeSet("MinimumRelativePieSize"))
			{
				num = (double)CommonElements.ParseFloat(series[seriesFromChartType[0]]["MinimumRelativePieSize"]) / 100.0;
				if (num < 0.1 || num > 0.7)
				{
					throw new ArgumentException(SR.ExceptionPieMinimumRelativePieSizeInvalid);
				}
			}
			return num;
		}

		private void SizeCorrection(ChartGraphics graph, CommonElements common, ChartArea area)
		{
			float num = labelsOverlap ? sizeCorrection : 0.95f;
			sliceExploded = false;
			if (area.InnerPlotPosition.Auto)
			{
				while (num >= (float)MinimumRelativePieSize(area))
				{
					sizeCorrection = num;
					ProcessChartType(selection: false, graph, common, area, shadow: false, LabelsMode.EstimateSize, null);
					if (labelsFit)
					{
						break;
					}
					num -= 0.05f;
				}
				if (sliceExploded && sizeCorrection > 0.8f)
				{
					sizeCorrection = 0.8f;
				}
			}
			else
			{
				sizeCorrection = 0.95f;
			}
		}

		private void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, bool shadow, LabelsMode labels, Series seriesToDraw)
		{
			float num = 0f;
			SeriesCollection series = common.DataManager.Series;
			if (labels == LabelsMode.LabelsOverlap)
			{
				labelsRectangles.Clear();
			}
			ArrayList seriesFromChartType = area.GetSeriesFromChartType(Name);
			if (seriesFromChartType.Count == 0)
			{
				return;
			}
			if (seriesFromChartType.Count > 0 && series[seriesFromChartType[0]].IsAttributeSet("PieStartAngle"))
			{
				try
				{
					num = float.Parse(series[seriesFromChartType[0]]["PieStartAngle"], CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeAngleOutOfRange("PieStartAngle"));
				}
				if (num > 360f || num < 0f)
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeAngleOutOfRange("PieStartAngle"));
				}
			}
			if (!selection)
			{
				common.EventsManager.OnBackPaint(series[seriesFromChartType[0]], new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
			}
			double num2 = 0.0;
			foreach (DataPoint point in series[seriesFromChartType[0]].Points)
			{
				if (!point.Empty)
				{
					num2 += Math.Abs(point.YValues[0]);
				}
			}
			if (num2 == 0.0)
			{
				return;
			}
			bool explodedShadow = false;
			foreach (DataPoint point2 in series[seriesFromChartType[0]].Points)
			{
				if (point2.IsAttributeSet("Exploded") && string.Compare(point2["Exploded"], "true", StringComparison.OrdinalIgnoreCase) == 0)
				{
					explodedShadow = true;
				}
			}
			float num3 = 60f;
			if (series[seriesFromChartType[0]].IsAttributeSet("DoughnutRadius"))
			{
				num3 = CommonElements.ParseFloat(series[seriesFromChartType[0]]["DoughnutRadius"]);
				if (num3 < 0f || num3 > 99f)
				{
					throw new ArgumentException(SR.ExceptionPieRadiusInvalid);
				}
			}
			CheckPaleteColors(series[seriesFromChartType[0]].Points);
			int num4 = 0;
			int num5 = 0;
			foreach (DataPoint point3 in series[seriesFromChartType[0]].Points)
			{
				if (point3.Empty)
				{
					num4++;
					continue;
				}
				RectangleF rectangleF = area.InnerPlotPosition.Auto ? new RectangleF(area.Position.ToRectangleF().X, area.Position.ToRectangleF().Y, area.Position.ToRectangleF().Width, area.Position.ToRectangleF().Height) : new RectangleF(area.PlotAreaPosition.ToRectangleF().X, area.PlotAreaPosition.ToRectangleF().Y, area.PlotAreaPosition.ToRectangleF().Width, area.PlotAreaPosition.ToRectangleF().Height);
				if (rectangleF.Width < 0f || rectangleF.Height < 0f)
				{
					return;
				}
				SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF(rectangleF.Width, rectangleF.Height));
				float num6 = (absoluteSize.Width < absoluteSize.Height) ? absoluteSize.Width : absoluteSize.Height;
				SizeF relativeSize = graph.GetRelativeSize(new SizeF(num6, num6));
				PointF middlePoint = new PointF(rectangleF.X + rectangleF.Width / 2f, rectangleF.Y + rectangleF.Height / 2f);
				rectangleF = new RectangleF(middlePoint.X - relativeSize.Width / 2f, middlePoint.Y - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height);
				if (sizeCorrection != 1f)
				{
					rectangleF.X += rectangleF.Width * (1f - sizeCorrection) / 2f;
					rectangleF.Y += rectangleF.Height * (1f - sizeCorrection) / 2f;
					rectangleF.Width *= sizeCorrection;
					rectangleF.Height *= sizeCorrection;
					if (area.InnerPlotPosition.Auto)
					{
						RectangleF rectangleF2 = rectangleF;
						rectangleF2.X = (rectangleF2.X - area.Position.X) / area.Position.Width * 100f;
						rectangleF2.Y = (rectangleF2.Y - area.Position.Y) / area.Position.Height * 100f;
						rectangleF2.Width = rectangleF2.Width / area.Position.Width * 100f;
						rectangleF2.Height = rectangleF2.Height / area.Position.Height * 100f;
						area.InnerPlotPosition.SetPositionNoAuto(rectangleF2.X, rectangleF2.Y, rectangleF2.Width, rectangleF2.Height);
					}
				}
				float num7 = (float)(Math.Abs(point3.YValues[0]) / num2 * 360.0);
				bool flag = false;
				if (point3.IsAttributeSet("Exploded"))
				{
					flag = ((string.Compare(point3["Exploded"], "true", StringComparison.OrdinalIgnoreCase) == 0) ? true : false);
				}
				Color pieLineColor = Color.Empty;
				ColorConverter colorConverter = new ColorConverter();
				if (point3.IsAttributeSet("PieLineColor") || series[seriesFromChartType[0]].IsAttributeSet("PieLineColor"))
				{
					try
					{
						pieLineColor = (Color)colorConverter.ConvertFromString(point3.IsAttributeSet("PieLineColor") ? point3["PieLineColor"] : series[seriesFromChartType[0]]["PieLineColor"]);
					}
					catch
					{
						pieLineColor = (Color)colorConverter.ConvertFromInvariantString(point3.IsAttributeSet("PieLineColor") ? point3["PieLineColor"] : series[seriesFromChartType[0]]["PieLineColor"]);
					}
				}
				float num8;
				if (flag)
				{
					sliceExploded = true;
					num8 = (2f * num + num7) / 2f;
					double num9 = Math.Cos((double)num8 * Math.PI / 180.0) * (double)rectangleF.Width / 10.0;
					double num10 = Math.Sin((double)num8 * Math.PI / 180.0) * (double)rectangleF.Height / 10.0;
					rectangleF.Offset((float)num9, (float)num10);
				}
				if (common.ProcessModeRegions && labels == LabelsMode.Draw)
				{
					Map(common, point3, num, num7, rectangleF, Doughnut, num3, graph, num4);
				}
				if (common.ProcessModePaint)
				{
					if (shadow)
					{
						double num11 = graph.GetRelativeSize(new SizeF(point3.series.ShadowOffset, point3.series.ShadowOffset)).Width;
						if (num11 == 0.0)
						{
							break;
						}
						RectangleF rect = new RectangleF(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
						rect.Offset((float)num11, (float)num11);
						Color color = default(Color);
						Color color2 = default(Color);
						Color color3 = default(Color);
						color = ((point3.Color.A == byte.MaxValue) ? point3.series.ShadowColor : Color.FromArgb((int)point3.Color.A / 2, point3.series.ShadowColor));
						color2 = (point3.BackGradientEndColor.IsEmpty ? Color.Empty : ((point3.BackGradientEndColor.A == byte.MaxValue) ? point3.series.ShadowColor : Color.FromArgb((int)point3.BackGradientEndColor.A / 2, point3.series.ShadowColor)));
						color3 = (point3.BorderColor.IsEmpty ? Color.Empty : ((point3.BorderColor.A == byte.MaxValue) ? point3.series.ShadowColor : Color.FromArgb((int)point3.BorderColor.A / 2, point3.series.ShadowColor)));
						graph.StartAnimation();
						graph.DrawPieRel(rect, num, num7, color, ChartHatchStyle.None, "", point3.BackImageMode, point3.BackImageTransparentColor, point3.BackImageAlign, point3.BackGradientType, color2, color3, point3.BorderWidth, point3.BorderStyle, PenAlignment.Inset, shadow: true, point3.series.ShadowOffset, Doughnut, num3, explodedShadow, PieDrawingStyle.Default);
						graph.StopAnimation();
					}
					else if (labels == LabelsMode.Off)
					{
						graph.StartHotRegion(point3);
						graph.StartAnimation();
						graph.DrawPieRel(rectangleF, num, num7, point3.Color, point3.BackHatchStyle, point3.BackImage, point3.BackImageMode, point3.BackImageTransparentColor, point3.BackImageAlign, point3.BackGradientType, point3.BackGradientEndColor, point3.BorderColor, point3.BorderWidth, point3.BorderStyle, PenAlignment.Inset, shadow: false, point3.series.ShadowOffset, Doughnut, num3, explodedShadow, ChartGraphics.GetPieDrawingStyle(point3));
						graph.StopAnimation();
						graph.EndHotRegion();
					}
					if (labels == LabelsMode.EstimateSize)
					{
						EstimateLabels(graph, middlePoint, rectangleF.Size, num, num7, point3, num3, flag, area);
						if (!labelsFit)
						{
							return;
						}
					}
					if (labels == LabelsMode.LabelsOverlap)
					{
						DrawLabels(graph, middlePoint, rectangleF.Size, num, num7, point3, num3, flag, area, overlapTest: true, num5, pieLineColor);
					}
					graph.StartAnimation();
					if (labels == LabelsMode.Draw)
					{
						DrawLabels(graph, middlePoint, rectangleF.Size, num, num7, point3, num3, flag, area, overlapTest: false, num5, pieLineColor);
					}
					graph.StopAnimation();
				}
				if (common.ProcessModeRegions && labels == LabelsMode.Draw && !common.ProcessModePaint)
				{
					DrawLabels(graph, middlePoint, rectangleF.Size, num, num7, point3, num3, flag, area, overlapTest: false, num5, pieLineColor);
				}
				point3.positionRel = new PointF(float.NaN, float.NaN);
				float num12 = 1f;
				if (flag)
				{
					num12 = 1.2f;
				}
				num8 = num + num7 / 2f;
				point3.positionRel.X = (float)Math.Cos((double)num8 * Math.PI / 180.0) * rectangleF.Width * num12 / 2f + middlePoint.X;
				point3.positionRel.Y = (float)Math.Sin((double)num8 * Math.PI / 180.0) * rectangleF.Height * num12 / 2f + middlePoint.Y;
				num4++;
				num5++;
				num += num7;
				if (num >= 360f)
				{
					num -= 360f;
				}
			}
			if (labels == LabelsMode.LabelsOverlap && labelsOverlap)
			{
				labelsOverlap = PrepareLabels(area.Position.ToRectangleF());
			}
			if (!selection)
			{
				common.EventsManager.OnPaint(series[seriesFromChartType[0]], new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
			}
		}

		public void DrawLabels(ChartGraphics graph, PointF middlePoint, SizeF relativeSize, float startAngle, float sweepAngle, DataPoint point, float doughnutRadius, bool exploded, ChartArea area, bool overlapTest, int pointIndex, Color pieLineColor)
		{
			bool flag = false;
			float num = 1f;
			float num2 = 1f;
			Region clip = graph.Clip;
			graph.Clip = new Region();
			string labelText = GetLabelText(point);
			if (labelText.Length == 0)
			{
				return;
			}
			Series series = point.series;
			PieLabelStyle pieLabelStyle = PieLabelStyle.Inside;
			if (series.IsAttributeSet("LabelStyle"))
			{
				string strA = series["LabelStyle"];
				pieLabelStyle = ((string.Compare(strA, "disabled", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Disabled : ((string.Compare(strA, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Outside : PieLabelStyle.Inside));
			}
			else if (series.IsAttributeSet("PieLabelStyle"))
			{
				string strA2 = series["PieLabelStyle"];
				pieLabelStyle = ((string.Compare(strA2, "disabled", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Disabled : ((string.Compare(strA2, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Outside : PieLabelStyle.Inside));
			}
			if (point.IsAttributeSet("LabelStyle"))
			{
				string strA3 = point["LabelStyle"];
				pieLabelStyle = ((string.Compare(strA3, "disabled", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Disabled : ((string.Compare(strA3, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Outside : PieLabelStyle.Inside));
			}
			else if (point.IsAttributeSet("PieLabelStyle"))
			{
				string strA4 = point["PieLabelStyle"];
				pieLabelStyle = ((string.Compare(strA4, "disabled", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Disabled : ((string.Compare(strA4, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Outside : PieLabelStyle.Inside));
			}
			if (series.IsAttributeSet("LabelsRadialLineSize"))
			{
				num2 = CommonElements.ParseFloat(series["LabelsRadialLineSize"]);
				if (num2 < 0f || num2 > 100f)
				{
					throw new ArgumentException(SR.ExceptionPieRadialLineSizeInvalid, "LabelsRadialLineSize");
				}
			}
			if (point.IsAttributeSet("LabelsRadialLineSize"))
			{
				num2 = CommonElements.ParseFloat(point["LabelsRadialLineSize"]);
				if (num2 < 0f || num2 > 100f)
				{
					throw new ArgumentException(SR.ExceptionPieRadialLineSizeInvalid, "LabelsRadialLineSize");
				}
			}
			if (series.IsAttributeSet("LabelsHorizontalLineSize"))
			{
				num = CommonElements.ParseFloat(series["LabelsHorizontalLineSize"]);
				if (num < 0f || num > 100f)
				{
					throw new ArgumentException(SR.ExceptionPieHorizontalLineSizeInvalid, "LabelsHorizontalLineSize");
				}
			}
			if (point.IsAttributeSet("LabelsHorizontalLineSize"))
			{
				num = CommonElements.ParseFloat(point["LabelsHorizontalLineSize"]);
				if (num < 0f || num > 100f)
				{
					throw new ArgumentException(SR.ExceptionPieHorizontalLineSizeInvalid, "LabelsHorizontalLineSize");
				}
			}
			float num3 = 1f;
			if (pieLabelStyle == PieLabelStyle.Inside && !overlapTest)
			{
				if (exploded)
				{
					num3 = 1.4f;
				}
				float num4 = 4f;
				if (point.IsAttributeSet("InsideLabelOffset"))
				{
					num4 = float.Parse(point["InsideLabelOffset"], CultureInfo.InvariantCulture);
					if (num4 < 0f || num4 > 100f)
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to100("InsideLabelOffset"));
					}
					num4 = 4f / (1f + num4 / 100f);
				}
				float num5;
				float num6;
				if (Doughnut)
				{
					num5 = relativeSize.Width * num3 / num4 * (1f + (100f - doughnutRadius) / 100f);
					num6 = relativeSize.Height * num3 / num4 * (1f + (100f - doughnutRadius) / 100f);
				}
				else
				{
					num5 = relativeSize.Width * num3 / num4;
					num6 = relativeSize.Height * num3 / num4;
				}
				float x = (float)Math.Cos((double)(startAngle + sweepAngle / 2f) * Math.PI / 180.0) * num5 + middlePoint.X;
				float y = (float)Math.Sin((double)(startAngle + sweepAngle / 2f) * Math.PI / 180.0) * num6 + middlePoint.Y;
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				SizeF relativeSize2 = graph.GetRelativeSize(graph.MeasureString(labelText.Replace("\\n", "\n"), point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
				RectangleF empty = RectangleF.Empty;
				SizeF size = new SizeF(relativeSize2.Width, relativeSize2.Height);
				size.Height += size.Height / 8f;
				size.Width += size.Width / (float)labelText.Length;
				empty = PointChart.GetLabelPosition(graph, new PointF(x, y), size, stringFormat, adjustForDrawing: true);
				graph.DrawPointLabelStringRel(area.Common, labelText, point.Font, new SolidBrush(point.FontColor), new PointF(x, y), stringFormat, point.FontAngle, empty, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, series, point, pointIndex);
			}
			else if (pieLabelStyle == PieLabelStyle.Outside)
			{
				float num7 = 0.5f + num2 * 0.1f;
				if (exploded)
				{
					num3 = 1.2f;
				}
				float num8 = startAngle + sweepAngle / 2f;
				float x2 = (float)Math.Cos((double)num8 * Math.PI / 180.0) * relativeSize.Width * num3 / 2f + middlePoint.X;
				float y2 = (float)Math.Sin((double)num8 * Math.PI / 180.0) * relativeSize.Height * num3 / 2f + middlePoint.Y;
				float x3 = (float)Math.Cos((double)num8 * Math.PI / 180.0) * relativeSize.Width * num7 * num3 + middlePoint.X;
				float y3 = (float)Math.Sin((double)num8 * Math.PI / 180.0) * relativeSize.Height * num7 * num3 + middlePoint.Y;
				if (pieLineColor == Color.Empty)
				{
					pieLineColor = point.BorderColor;
				}
				if (!overlapTest)
				{
					graph.DrawLineRel(pieLineColor, point.BorderWidth, ChartDashStyle.Solid, new PointF(x2, y2), new PointF(x3, y3));
				}
				StringFormat stringFormat2 = new StringFormat();
				stringFormat2.Alignment = StringAlignment.Center;
				stringFormat2.LineAlignment = StringAlignment.Center;
				float y4 = (float)Math.Sin((double)num8 * Math.PI / 180.0) * relativeSize.Height * num7 * num3 + middlePoint.Y;
				RectangleF empty2 = RectangleF.Empty;
				RectangleF empty3 = RectangleF.Empty;
				float x4;
				float num9;
				if (num8 > 90f && num8 < 270f)
				{
					stringFormat2.Alignment = StringAlignment.Far;
					x4 = (0f - relativeSize.Width) * num7 * num3 + middlePoint.X - relativeSize.Width / 10f * num;
					num9 = (float)Math.Cos((double)num8 * Math.PI / 180.0) * relativeSize.Width * num7 * num3 + middlePoint.X - relativeSize.Width / 10f * num;
					if (overlapTest)
					{
						x4 = num9;
					}
					_ = labelsOverlap;
					empty2 = GetLabelRect(new PointF(num9, y4), area, labelText, stringFormat2, graph, point, leftOrientation: true);
					empty3 = GetLabelRect(new PointF(x4, y4), area, labelText, stringFormat2, graph, point, leftOrientation: true);
				}
				else
				{
					stringFormat2.Alignment = StringAlignment.Near;
					x4 = relativeSize.Width * num7 * num3 + middlePoint.X + relativeSize.Width / 10f * num;
					num9 = (float)Math.Cos((double)num8 * Math.PI / 180.0) * relativeSize.Width * num7 * num3 + middlePoint.X + relativeSize.Width / 10f * num;
					if (overlapTest)
					{
						x4 = num9;
					}
					_ = labelsOverlap;
					empty2 = GetLabelRect(new PointF(num9, y4), area, labelText, stringFormat2, graph, point, leftOrientation: false);
					empty3 = GetLabelRect(new PointF(x4, y4), area, labelText, stringFormat2, graph, point, leftOrientation: false);
				}
				if (!overlapTest)
				{
					if (labelsOverlap)
					{
						float y5 = (((RectangleF)labelsRectangles[pointIndex]).Top + ((RectangleF)labelsRectangles[pointIndex]).Bottom) / 2f;
						graph.DrawLineRel(pieLineColor, point.BorderWidth, ChartDashStyle.Solid, new PointF(x3, y3), new PointF(x4, y5));
					}
					else
					{
						graph.DrawLineRel(pieLineColor, point.BorderWidth, ChartDashStyle.Solid, new PointF(x3, y3), new PointF(num9, y4));
					}
				}
				if (!overlapTest)
				{
					RectangleF position = new RectangleF(empty2.Location, empty2.Size);
					if (labelsOverlap)
					{
						position = (RectangleF)labelsRectangles[pointIndex];
						position.X = empty3.X;
						position.Width = empty3.Width;
					}
					SizeF sizeF = graph.MeasureStringRel(labelText.Replace("\\n", "\n"), point.Font);
					sizeF.Height += sizeF.Height / 8f;
					float num10 = sizeF.Width / (float)labelText.Length / 2f;
					sizeF.Width += num10;
					RectangleF backPosition = new RectangleF(position.X, position.Y + position.Height / 2f - sizeF.Height / 2f, sizeF.Width, sizeF.Height);
					if (stringFormat2.Alignment == StringAlignment.Near)
					{
						backPosition.X -= num10 / 2f;
					}
					else if (stringFormat2.Alignment == StringAlignment.Center)
					{
						backPosition.X = position.X + (position.Width - sizeF.Width) / 2f;
					}
					else if (stringFormat2.Alignment == StringAlignment.Far)
					{
						backPosition.X = position.Right - sizeF.Width - num10 / 2f;
					}
					graph.DrawPointLabelStringRel(area.Common, labelText, point.Font, new SolidBrush(point.FontColor), position, stringFormat2, point.FontAngle, backPosition, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, series, point, pointIndex);
				}
				else
				{
					InsertOverlapLabel(empty3);
					flag = true;
				}
			}
			graph.Clip = clip;
			if (!flag)
			{
				InsertOverlapLabel(RectangleF.Empty);
			}
		}

		private RectangleF GetLabelRect(PointF labelPosition, ChartArea area, string text, StringFormat format, ChartGraphics graph, DataPoint point, bool leftOrientation)
		{
			RectangleF empty = RectangleF.Empty;
			if (leftOrientation)
			{
				empty.X = area.Position.X;
				empty.Y = area.Position.Y;
				empty.Width = labelPosition.X - area.Position.X;
				empty.Height = area.Position.Height;
			}
			else
			{
				empty.X = labelPosition.X;
				empty.Y = area.Position.Y;
				empty.Width = area.Position.Right() - labelPosition.X;
				empty.Height = area.Position.Height;
			}
			SizeF sizeF = graph.MeasureStringRel(text.Replace("\\n", "\n"), point.Font, empty.Size, format);
			empty.Y = labelPosition.Y - sizeF.Height / 2f * 1.8f;
			empty.Height = sizeF.Height * 1.8f;
			return empty;
		}

		private PieLabelStyle GetLabelStyle(DataPoint point)
		{
			Series series = point.series;
			PieLabelStyle result = PieLabelStyle.Inside;
			if (series.IsAttributeSet("LabelStyle"))
			{
				string strA = series["LabelStyle"];
				result = ((string.Compare(strA, "disabled", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Disabled : ((string.Compare(strA, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Outside : PieLabelStyle.Inside));
			}
			else if (series.IsAttributeSet("PieLabelStyle"))
			{
				string strA2 = series["PieLabelStyle"];
				result = ((string.Compare(strA2, "disabled", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Disabled : ((string.Compare(strA2, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Outside : PieLabelStyle.Inside));
			}
			if (point.IsAttributeSet("LabelStyle"))
			{
				string strA3 = point["LabelStyle"];
				result = ((string.Compare(strA3, "disabled", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Disabled : ((string.Compare(strA3, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Outside : PieLabelStyle.Inside));
			}
			else if (point.IsAttributeSet("PieLabelStyle"))
			{
				string strA4 = point["PieLabelStyle"];
				result = ((string.Compare(strA4, "disabled", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Disabled : ((string.Compare(strA4, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Outside : PieLabelStyle.Inside));
			}
			return result;
		}

		public bool EstimateLabels(ChartGraphics graph, PointF middlePoint, SizeF relativeSize, float startAngle, float sweepAngle, DataPoint point, float doughnutRadius, bool exploded, ChartArea area)
		{
			float num = 1f;
			float num2 = 1f;
			string pointLabel = GetPointLabel(point);
			Series series = point.series;
			PieLabelStyle pieLabelStyle = PieLabelStyle.Inside;
			if (series.IsAttributeSet("LabelStyle"))
			{
				string strA = series["LabelStyle"];
				pieLabelStyle = ((string.Compare(strA, "disabled", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Disabled : ((string.Compare(strA, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Outside : PieLabelStyle.Inside));
			}
			else if (series.IsAttributeSet("PieLabelStyle"))
			{
				string strA2 = series["PieLabelStyle"];
				pieLabelStyle = ((string.Compare(strA2, "disabled", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Disabled : ((string.Compare(strA2, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Outside : PieLabelStyle.Inside));
			}
			if (point.IsAttributeSet("LabelStyle"))
			{
				string strA3 = point["LabelStyle"];
				pieLabelStyle = ((string.Compare(strA3, "disabled", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Disabled : ((string.Compare(strA3, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Outside : PieLabelStyle.Inside));
			}
			else if (point.IsAttributeSet("PieLabelStyle"))
			{
				string strA4 = point["PieLabelStyle"];
				pieLabelStyle = ((string.Compare(strA4, "disabled", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Disabled : ((string.Compare(strA4, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? PieLabelStyle.Outside : PieLabelStyle.Inside));
			}
			if (series.IsAttributeSet("LabelsRadialLineSize"))
			{
				num2 = CommonElements.ParseFloat(series["LabelsRadialLineSize"]);
				if (num2 < 0f || num2 > 100f)
				{
					throw new ArgumentException(SR.ExceptionPieRadialLineSizeInvalid, "LabelsRadialLineSize");
				}
			}
			if (point.IsAttributeSet("LabelsRadialLineSize"))
			{
				num2 = CommonElements.ParseFloat(point["LabelsRadialLineSize"]);
				if (num2 < 0f || num2 > 100f)
				{
					throw new ArgumentException(SR.ExceptionPieRadialLineSizeInvalid, "LabelsRadialLineSize");
				}
			}
			if (series.IsAttributeSet("LabelsHorizontalLineSize"))
			{
				num = CommonElements.ParseFloat(series["LabelsHorizontalLineSize"]);
				if (num < 0f || num > 100f)
				{
					throw new ArgumentException(SR.ExceptionPieHorizontalLineSizeInvalid, "LabelsHorizontalLineSize");
				}
			}
			if (point.IsAttributeSet("LabelsHorizontalLineSize"))
			{
				num = CommonElements.ParseFloat(point["LabelsHorizontalLineSize"]);
				if (num < 0f || num > 100f)
				{
					throw new ArgumentException(SR.ExceptionPieHorizontalLineSizeInvalid, "LabelsHorizontalLineSize");
				}
			}
			float num3 = 1f;
			if (pieLabelStyle == PieLabelStyle.Outside)
			{
				float num4 = 0.5f + num2 * 0.1f;
				if (exploded)
				{
					num3 = 1.2f;
				}
				float num5 = startAngle + sweepAngle / 2f;
				_ = (float)Math.Cos((double)num5 * Math.PI / 180.0) * relativeSize.Width * num3 / 2f;
				_ = middlePoint.X;
				_ = (float)Math.Sin((double)num5 * Math.PI / 180.0) * relativeSize.Height * num3 / 2f;
				_ = middlePoint.Y;
				Math.Cos((double)num5 * Math.PI / 180.0);
				_ = relativeSize.Width;
				_ = middlePoint.X;
				Math.Sin((double)num5 * Math.PI / 180.0);
				_ = relativeSize.Height;
				_ = middlePoint.Y;
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				float num6 = (float)Math.Sin((double)num5 * Math.PI / 180.0) * relativeSize.Height * num4 * num3 + middlePoint.Y;
				float num7;
				if (num5 > 90f && num5 < 270f)
				{
					num7 = (float)Math.Cos((double)num5 * Math.PI / 180.0) * relativeSize.Width * num4 * num3 + middlePoint.X - relativeSize.Width / 10f * num;
					stringFormat.Alignment = StringAlignment.Far;
				}
				else
				{
					num7 = (float)Math.Cos((double)num5 * Math.PI / 180.0) * relativeSize.Width * num4 * num3 + middlePoint.X + relativeSize.Width / 10f * num;
					stringFormat.Alignment = StringAlignment.Near;
				}
				string text;
				if (pointLabel.Length == 0 && point.ShowLabelAsValue)
				{
					text = ValueConverter.FormatValue(series.chart, point, point.YValues[0], point.LabelFormat, point.series.YValueType, ChartElementType.DataPoint);
				}
				else
				{
					text = pointLabel;
					if (series.chart != null && series.chart.LocalizeTextHandler != null)
					{
						text = series.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
					}
				}
				SizeF sizeF = graph.MeasureStringRel(text.Replace("\\n", "\n"), point.Font);
				labelsFit = true;
				if (labelsOverlap)
				{
					if (num5 > 90f && num5 < 270f)
					{
						if ((0f - relativeSize.Width) * num4 * num3 + middlePoint.X - relativeSize.Width / 10f * num - sizeF.Width < area.Position.X)
						{
							labelsFit = false;
						}
					}
					else if (relativeSize.Width * num4 * num3 + middlePoint.X + relativeSize.Width / 10f * num + sizeF.Width > area.Position.Right())
					{
						labelsFit = false;
					}
				}
				else
				{
					if (num5 > 90f && num5 < 270f)
					{
						if (num7 - sizeF.Width < area.PlotAreaPosition.ToRectangleF().Left)
						{
							labelsFit = false;
						}
					}
					else if (num7 + sizeF.Width > area.PlotAreaPosition.ToRectangleF().Right)
					{
						labelsFit = false;
					}
					if (num5 > 180f && num5 < 360f)
					{
						if (num6 - sizeF.Height / 2f < area.PlotAreaPosition.ToRectangleF().Top)
						{
							labelsFit = false;
						}
					}
					else if (num6 + sizeF.Height / 2f > area.PlotAreaPosition.ToRectangleF().Bottom)
					{
						labelsFit = false;
					}
				}
			}
			return true;
		}

		public static bool CreateMapAreaPath(float startAngle, float sweepAngle, RectangleF rectangle, bool doughnut, float doughnutRadius, ChartGraphics graph, out GraphicsPath path, out float[] coord)
		{
			path = new GraphicsPath();
			RectangleF empty = RectangleF.Empty;
			empty.X = rectangle.X + rectangle.Width * (1f - (100f - doughnutRadius) / 100f) / 2f;
			empty.Y = rectangle.Y + rectangle.Height * (1f - (100f - doughnutRadius) / 100f) / 2f;
			empty.Width = rectangle.Width * (100f - doughnutRadius) / 100f;
			empty.Height = rectangle.Height * (100f - doughnutRadius) / 100f;
			rectangle = graph.GetAbsoluteRectangle(rectangle);
			path.AddPie(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, startAngle, sweepAngle);
			if (sweepAngle <= 0f)
			{
				path = null;
				coord = null;
				return false;
			}
			if (doughnut)
			{
				empty = graph.GetAbsoluteRectangle(empty);
				path.AddPie(empty.X, empty.Y, empty.Width, empty.Width, startAngle, sweepAngle);
			}
			path.Flatten(new Matrix(), 1f);
			PointF[] array = new PointF[path.PointCount];
			for (int i = 0; i < path.PointCount; i++)
			{
				array[i] = graph.GetRelativePoint(path.PathPoints[i]);
			}
			coord = new float[path.PointCount * 2];
			for (int j = 0; j < path.PointCount; j++)
			{
				coord[2 * j] = array[j].X;
				coord[2 * j + 1] = array[j].Y;
			}
			return true;
		}

		public static void Map(CommonElements common, DataPoint point, float startAngle, float sweepAngle, RectangleF rectangle, bool doughnut, float doughnutRadius, ChartGraphics graph, int pointIndex)
		{
			if (CreateMapAreaPath(startAngle, sweepAngle, rectangle, doughnut, doughnutRadius, graph, out GraphicsPath path, out float[] coord))
			{
				if (point.IsAttributeSet("_COLLECTED_DATA_POINT"))
				{
					common.HotRegionsList.AddHotRegion(graph, path, relativePath: false, point.ReplaceKeywords(point.ToolTip), point.ReplaceKeywords(point.Href), point.ReplaceKeywords(point.MapAreaAttributes), point, ChartElementType.DataPoint);
				}
				else
				{
					common.HotRegionsList.AddHotRegion(graph, path, relativePath: false, coord, point, point.series.Name, pointIndex);
				}
			}
		}

		private void CheckPaleteColors(DataPointCollection points)
		{
			DataPoint dataPoint = points[0];
			DataPoint dataPoint2 = points[points.Count - 1];
			if (dataPoint.tempColorIsSet && dataPoint2.tempColorIsSet && dataPoint.Color == dataPoint2.Color)
			{
				dataPoint2.Color = points[points.Count / 2].Color;
				dataPoint2.tempColorIsSet = true;
			}
		}

		private bool PrepareLabels(RectangleF area)
		{
			float num = area.X + area.Width / 2f;
			int num2 = 0;
			int num3 = 0;
			foreach (RectangleF labelsRectangle in labelsRectangles)
			{
				if (labelsRectangle.X < num)
				{
					num2++;
				}
				else
				{
					num3++;
				}
			}
			bool flag = true;
			if (num2 > 0)
			{
				double[] array = new double[num2];
				double[] array2 = new double[num2];
				int[] array3 = new int[num2];
				int num4 = 0;
				for (int i = 0; i < labelsRectangles.Count; i++)
				{
					RectangleF rectangleF = (RectangleF)labelsRectangles[i];
					if (rectangleF.X < num)
					{
						array[num4] = rectangleF.Top;
						array2[num4] = rectangleF.Bottom;
						array3[num4] = i;
						num4++;
					}
				}
				SortIntervals(array, array2, array3);
				if (ArrangeOverlappingIntervals(array, array2, area.Top, area.Bottom))
				{
					num4 = 0;
					for (int j = 0; j < labelsRectangles.Count; j++)
					{
						RectangleF rectangleF2 = (RectangleF)labelsRectangles[j];
						if (rectangleF2.X < num)
						{
							rectangleF2.Y = (float)array[num4];
							rectangleF2.Height = (float)(array2[num4] - (double)rectangleF2.Top);
							labelsRectangles[array3[num4]] = rectangleF2;
							num4++;
						}
					}
				}
				else
				{
					flag = false;
				}
			}
			bool flag2 = true;
			if (num3 > 0)
			{
				double[] array4 = new double[num3];
				double[] array5 = new double[num3];
				int[] array6 = new int[num3];
				int num5 = 0;
				for (int k = 0; k < labelsRectangles.Count; k++)
				{
					RectangleF rectangleF3 = (RectangleF)labelsRectangles[k];
					if (rectangleF3.X >= num)
					{
						array4[num5] = rectangleF3.Top;
						array5[num5] = rectangleF3.Bottom;
						array6[num5] = k;
						num5++;
					}
				}
				SortIntervals(array4, array5, array6);
				if (ArrangeOverlappingIntervals(array4, array5, area.Top, area.Bottom))
				{
					num5 = 0;
					for (int l = 0; l < labelsRectangles.Count; l++)
					{
						RectangleF rectangleF4 = (RectangleF)labelsRectangles[l];
						if (rectangleF4.X >= num)
						{
							rectangleF4.Y = (float)array4[num5];
							rectangleF4.Height = (float)(array5[num5] - (double)rectangleF4.Top);
							labelsRectangles[array6[num5]] = rectangleF4;
							num5++;
						}
					}
				}
				else
				{
					flag2 = false;
				}
			}
			if (flag && flag2)
			{
				return false;
			}
			return true;
		}

		private void SortIntervals(double[] startOfIntervals, double[] endOfIntervals, int[] positinIndex)
		{
			for (int i = 0; i < startOfIntervals.Length; i++)
			{
				for (int j = i; j < startOfIntervals.Length; j++)
				{
					double num = (startOfIntervals[i] + endOfIntervals[i]) / 2.0;
					double num2 = (startOfIntervals[j] + endOfIntervals[j]) / 2.0;
					if (num > num2)
					{
						double num3 = startOfIntervals[i];
						startOfIntervals[i] = startOfIntervals[j];
						startOfIntervals[j] = num3;
						num3 = endOfIntervals[i];
						endOfIntervals[i] = endOfIntervals[j];
						endOfIntervals[j] = num3;
						int num4 = positinIndex[i];
						positinIndex[i] = positinIndex[j];
						positinIndex[j] = num4;
					}
				}
			}
		}

		private void InsertOverlapLabel(RectangleF labelRect)
		{
			if (!labelRect.IsEmpty)
			{
				foreach (RectangleF labelsRectangle in labelsRectangles)
				{
					if (labelRect.IntersectsWith(labelsRectangle))
					{
						labelsOverlap = true;
					}
				}
			}
			labelsRectangles.Add(labelRect);
		}

		private bool ArrangeOverlappingIntervals(double[] startOfIntervals, double[] endOfIntervals, double startArea, double endArea)
		{
			if (startOfIntervals.Length != endOfIntervals.Length)
			{
				throw new InvalidOperationException(SR.ExceptionPieIntervalsInvalid);
			}
			ShiftOverlappingIntervals(startOfIntervals, endOfIntervals);
			double num = 0.0;
			for (int i = 0; i < startOfIntervals.Length - 1; i++)
			{
				_ = startOfIntervals[i + 1];
				_ = endOfIntervals[i];
				num += startOfIntervals[i + 1] - endOfIntervals[i];
			}
			double num2 = endOfIntervals[endOfIntervals.Length - 1] - endArea + (startArea - startOfIntervals[0]);
			if (num2 <= 0.0)
			{
				ShiftIntervals(startOfIntervals, endOfIntervals, startArea, endArea);
				return true;
			}
			if (num2 > num)
			{
				return false;
			}
			ReduceEmptySpace(startOfIntervals, endOfIntervals, (num - num2) / num);
			ShiftIntervals(startOfIntervals, endOfIntervals, startArea, endArea);
			return true;
		}

		private void ReduceEmptySpace(double[] startOfIntervals, double[] endOfIntervals, double reduction)
		{
			for (int i = 0; i < startOfIntervals.Length - 1; i++)
			{
				_ = startOfIntervals[i + 1];
				_ = endOfIntervals[i];
				double num = startOfIntervals[i + 1] - endOfIntervals[i] - (startOfIntervals[i + 1] - endOfIntervals[i]) * reduction;
				for (int j = i + 1; j < startOfIntervals.Length; j++)
				{
					startOfIntervals[j] -= num;
					endOfIntervals[j] -= num;
				}
			}
		}

		private void ShiftIntervals(double[] startOfIntervals, double[] endOfIntervals, double startArea, double endArea)
		{
			double num = 0.0;
			if (startOfIntervals[0] < startArea)
			{
				num = startArea - startOfIntervals[0];
			}
			else if (endOfIntervals[endOfIntervals.Length - 1] > endArea)
			{
				num = endArea - endOfIntervals[endOfIntervals.Length - 1];
			}
			for (int i = 0; i < startOfIntervals.Length; i++)
			{
				startOfIntervals[i] += num;
				endOfIntervals[i] += num;
			}
		}

		private void ShiftOverlappingIntervals(double[] startOfIntervals, double[] endOfIntervals)
		{
			if (startOfIntervals.Length != endOfIntervals.Length)
			{
				throw new InvalidOperationException(SR.ExceptionPieIntervalsInvalid);
			}
			for (int i = 0; i < startOfIntervals.Length - 1; i++)
			{
				if (endOfIntervals[i] > startOfIntervals[i + 1])
				{
					double num = endOfIntervals[i] - startOfIntervals[i + 1];
					SpreadInterval(startOfIntervals, endOfIntervals, i, Math.Floor(num / 2.0));
				}
			}
		}

		private void SpreadInterval(double[] startOfIntervals, double[] endOfIntervals, int splitIndex, double overlapShift)
		{
			endOfIntervals[splitIndex] -= overlapShift;
			startOfIntervals[splitIndex] -= overlapShift;
			endOfIntervals[splitIndex + 1] += overlapShift;
			startOfIntervals[splitIndex + 1] += overlapShift;
			if (splitIndex > 0)
			{
				int num = splitIndex - 1;
				while (num >= 0 && endOfIntervals[num] > startOfIntervals[num + 1] - overlapShift)
				{
					endOfIntervals[num] -= overlapShift;
					startOfIntervals[num] -= overlapShift;
					num--;
				}
			}
			if (splitIndex + 2 < startOfIntervals.Length - 1)
			{
				for (int i = splitIndex + 2; i < startOfIntervals.Length && startOfIntervals[i] > endOfIntervals[i - 1] + overlapShift; i++)
				{
					endOfIntervals[i] += overlapShift;
					startOfIntervals[i] += overlapShift;
				}
			}
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		private void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, bool shadow, LabelsMode labels, Series seriesToDraw, float pieWidth)
		{
			SeriesCollection series = common.DataManager.Series;
			ArrayList seriesFromChartType = area.GetSeriesFromChartType(Name);
			if (seriesFromChartType.Count == 0)
			{
				return;
			}
			if (series[seriesFromChartType[0]].IsAttributeSet("PieStartAngle"))
			{
				try
				{
					int num = int.Parse(series[seriesFromChartType[0]]["PieStartAngle"], CultureInfo.InvariantCulture);
					if (num > 180 && num <= 360)
					{
						num = -(360 - num);
					}
					area.Area3DStyle.YAngle = num;
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeAngleOutOfRange("PieStartAngle"));
				}
				if (area.Area3DStyle.YAngle > 180 || area.Area3DStyle.YAngle < -180)
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeAngleOutOfRange("PieStartAngle"));
				}
			}
			if (!selection)
			{
				common.EventsManager.OnBackPaint(series[seriesFromChartType[0]], new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
			}
			double num2 = 0.0;
			foreach (DataPoint point2 in series[seriesFromChartType[0]].Points)
			{
				if (!point2.Empty)
				{
					num2 += Math.Abs(point2.YValues[0]);
				}
			}
			bool flag = false;
			foreach (DataPoint point3 in series[seriesFromChartType[0]].Points)
			{
				if (point3.IsAttributeSet("Exploded") && string.Compare(point3["Exploded"], "true", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
				}
			}
			float num3 = 60f;
			if (series[seriesFromChartType[0]].IsAttributeSet("DoughnutRadius"))
			{
				num3 = CommonElements.ParseFloat(series[seriesFromChartType[0]]["DoughnutRadius"]);
				if (num3 < 0f || num3 > 99f)
				{
					throw new ArgumentException(SR.ExceptionPieRadiusInvalid);
				}
			}
			float num4 = 100f;
			if (series[seriesFromChartType[0]].IsAttributeSet("3DLabelLineSize"))
			{
				num4 = CommonElements.ParseFloat(series[seriesFromChartType[0]]["3DLabelLineSize"]);
				if (num4 < 30f || num4 > 200f)
				{
					throw new ArgumentException(SR.ExceptionPie3DLabelLineSizeInvalid);
				}
			}
			num4 = num4 * 0.1f / 100f;
			CheckPaleteColors(series[seriesFromChartType[0]].Points);
			float[] newStartAngleList;
			float[] newSweepAngleList;
			int[] newPointIndexList;
			bool sameBackFrontPoint;
			DataPoint[] array = PointOrder(series[seriesFromChartType[0]], area, out newStartAngleList, out newSweepAngleList, out newPointIndexList, out sameBackFrontPoint);
			if (array == null)
			{
				return;
			}
			RectangleF pieRectangle = new RectangleF(area.Position.ToRectangleF().X + 1f, area.Position.ToRectangleF().Y + 1f, area.Position.ToRectangleF().Width - 2f, area.Position.ToRectangleF().Height - 2f);
			bool flag2 = false;
			DataPoint[] array2 = array;
			foreach (DataPoint point in array2)
			{
				if (GetLabelStyle(point) == PieLabelStyle.Outside)
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				InitPieSize(graph, area, ref pieRectangle, ref pieWidth, array, newStartAngleList, newSweepAngleList, newPointIndexList, series[seriesFromChartType[0]], num4);
			}
			area.matrix3D.Initialize(pieRectangle, pieWidth, area.Area3DStyle.XAngle, 0f, 0f, rightAngleAxis: false);
			area.matrix3D.InitLight(area.Area3DStyle.Light);
			for (int j = 0; j < 5; j++)
			{
				int num5 = 0;
				array2 = array;
				foreach (DataPoint dataPoint3 in array2)
				{
					dataPoint3.positionRel = PointF.Empty;
					if (dataPoint3.Empty)
					{
						num5++;
						continue;
					}
					float num6 = newSweepAngleList[num5];
					float num7 = newStartAngleList[num5];
					RectangleF rectangleF = area.InnerPlotPosition.Auto ? new RectangleF(pieRectangle.X, pieRectangle.Y, pieRectangle.Width, pieRectangle.Height) : new RectangleF(area.PlotAreaPosition.ToRectangleF().X, area.PlotAreaPosition.ToRectangleF().Y, area.PlotAreaPosition.ToRectangleF().Width, area.PlotAreaPosition.ToRectangleF().Height);
					SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF(rectangleF.Width, rectangleF.Height));
					float num8 = (absoluteSize.Width < absoluteSize.Height) ? absoluteSize.Width : absoluteSize.Height;
					SizeF relativeSize = graph.GetRelativeSize(new SizeF(num8, num8));
					PointF pointF = new PointF(rectangleF.X + rectangleF.Width / 2f, rectangleF.Y + rectangleF.Height / 2f);
					rectangleF = new RectangleF(pointF.X - relativeSize.Width / 2f, pointF.Y - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height);
					bool flag3 = false;
					if (dataPoint3.IsAttributeSet("Exploded"))
					{
						flag3 = ((string.Compare(dataPoint3["Exploded"], "true", StringComparison.OrdinalIgnoreCase) == 0) ? true : false);
					}
					float num9 = 1f;
					if (flag)
					{
						num9 = 0.82f;
						rectangleF.X += rectangleF.Width * (1f - num9) / 2f;
						rectangleF.Y += rectangleF.Height * (1f - num9) / 2f;
						rectangleF.Width *= num9;
						rectangleF.Height *= num9;
					}
					if (flag3)
					{
						sliceExploded = true;
						float num10 = (2f * num7 + num6) / 2f;
						double num11 = Math.Cos((double)num10 * Math.PI / 180.0) * (double)rectangleF.Width / 10.0;
						double num12 = Math.Sin((double)num10 * Math.PI / 180.0) * (double)rectangleF.Height / 10.0;
						rectangleF.Offset((float)num11, (float)num12);
					}
					if (area.InnerPlotPosition.Auto)
					{
						RectangleF rectangleF2 = rectangleF;
						rectangleF2.X = (rectangleF2.X - area.Position.X) / area.Position.Width * 100f;
						rectangleF2.Y = (rectangleF2.Y - area.Position.Y) / area.Position.Height * 100f;
						rectangleF2.Width = rectangleF2.Width / area.Position.Width * 100f;
						rectangleF2.Height = rectangleF2.Height / area.Position.Height * 100f;
						area.InnerPlotPosition.SetPositionNoAuto(rectangleF2.X, rectangleF2.Y, rectangleF2.Width, rectangleF2.Height);
					}
					graph.StartHotRegion(dataPoint3);
					bool isSelected = false;
					Draw3DPie(j, graph, dataPoint3, area, rectangleF, num7, num6, num3, pieWidth, selection, ref isSelected, sameBackFrontPoint, flag3, newPointIndexList[num5]);
					graph.EndHotRegion();
					if (j == 1 && GetLabelStyle(dataPoint3) == PieLabelStyle.Outside)
					{
						FillPieLabelOutside(graph, area, rectangleF, pieWidth, dataPoint3, num7, num6, num5, num3, flag3);
					}
					if (j == 2 && GetLabelStyle(dataPoint3) == PieLabelStyle.Outside && num5 == 0)
					{
						labelColumnLeft.Sort();
						labelColumnLeft.AdjustPositions();
						labelColumnRight.Sort();
						labelColumnRight.AdjustPositions();
					}
					num5++;
				}
			}
			if (!selection)
			{
				common.EventsManager.OnPaint(series[seriesFromChartType[0]], new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
			}
		}

		private void Draw3DPie(int turn, ChartGraphics graph, DataPoint point, ChartArea area, RectangleF rectangle, float startAngle, float sweepAngle, float doughnutRadius, float pieWidth, bool selection, ref bool isSelected, bool sameBackFront, bool exploded, int pointIndex)
		{
			_ = area.Common;
			SolidBrush solidBrush = new SolidBrush(point.Color);
			Color empty = Color.Empty;
			Color color = Color.Empty;
			empty = ((point.BorderColor == Color.Empty && area.Area3DStyle.Light == LightStyle.None) ? ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.5) : ((!(point.BorderColor == Color.Empty)) ? point.BorderColor : point.Color));
			if (point.BorderColor != Color.Empty || area.Area3DStyle.Light == LightStyle.None)
			{
				color = empty;
			}
			Pen pen = new Pen(empty, point.BorderWidth);
			pen.DashStyle = graph.GetPenStyle(point.BorderStyle);
			Pen pen2 = (!(point.BorderColor == Color.Empty)) ? pen : new Pen(point.Color);
			Pen pen3 = new Pen(color, point.BorderWidth);
			pen3.DashStyle = graph.GetPenStyle(point.BorderStyle);
			PointF[] piePoints = GetPiePoints(graph, area, pieWidth, rectangle, startAngle, sweepAngle, relativeCoordinates: true, doughnutRadius, exploded);
			if (piePoints == null)
			{
				return;
			}
			point.positionRel.X = piePoints[10].X;
			point.positionRel.Y = piePoints[10].Y;
			point.positionRel = graph.GetRelativePoint(point.positionRel);
			float num = startAngle + sweepAngle / 2f;
			float num2 = startAngle + sweepAngle;
			switch (turn)
			{
			case 0:
				graph.StartAnimation();
				if (!Doughnut)
				{
					graph.FillPieSlice(area, point, solidBrush, pen2, piePoints[15], piePoints[6], piePoints[16], piePoints[7], piePoints[9], startAngle, sweepAngle, fill: false, pointIndex);
				}
				else
				{
					graph.FillDoughnutSlice(area, point, solidBrush, pen2, piePoints[15], piePoints[6], piePoints[16], piePoints[7], piePoints[24], piePoints[23], piePoints[9], startAngle, sweepAngle, fill: false, doughnutRadius, pointIndex);
				}
				graph.StopAnimation();
				break;
			case 1:
				graph.StartAnimation();
				if (sameBackFront)
				{
					if ((num > -90f && num < 90f) || (num > 270f && num < 450f))
					{
						if (Doughnut)
						{
							DrawDoughnutCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, rightPosition: false, sameBackFront: true, pointIndex);
						}
						DrawPieCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, rightPosition: true, sameBackFront: true, pointIndex);
					}
					else
					{
						if (Doughnut)
						{
							DrawDoughnutCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, rightPosition: true, sameBackFront: true, pointIndex);
						}
						DrawPieCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, rightPosition: false, sameBackFront: true, pointIndex);
					}
					graph.FillPieSides(area, area.Area3DStyle.XAngle, startAngle, sweepAngle, piePoints, solidBrush, pen, Doughnut);
				}
				else
				{
					if (Doughnut)
					{
						DrawDoughnutCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, rightPosition: false, sameBackFront: false, pointIndex);
					}
					graph.FillPieSides(area, area.Area3DStyle.XAngle, startAngle, sweepAngle, piePoints, solidBrush, pen, Doughnut);
					DrawPieCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, rightPosition: false, sameBackFront: false, pointIndex);
				}
				graph.StopAnimation();
				break;
			case 2:
				graph.StartAnimation();
				if (sameBackFront && sweepAngle > 180f)
				{
					bool flag = ((startAngle > -180f && startAngle < 0f) || (startAngle > 180f && startAngle < 360f)) && ((num2 > -180f && num2 < 0f) || (num2 > 180f && num2 < 360f));
					if (area.Area3DStyle.XAngle > 0)
					{
						flag = !flag;
					}
					if ((num > -90f && num < 90f) || (num > 270f && num < 450f))
					{
						if (Doughnut && flag && sweepAngle > 300f)
						{
							DrawDoughnutCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, rightPosition: true, sameBackFront: true, pointIndex);
						}
						DrawPieCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, rightPosition: false, sameBackFront: true, pointIndex);
					}
					else
					{
						if (Doughnut && flag && sweepAngle > 300f)
						{
							DrawDoughnutCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, rightPosition: false, sameBackFront: true, pointIndex);
						}
						DrawPieCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, rightPosition: true, sameBackFront: true, pointIndex);
					}
				}
				graph.StopAnimation();
				break;
			case 3:
				graph.StartAnimation();
				if (!Doughnut)
				{
					graph.FillPieSlice(area, point, solidBrush, pen, piePoints[13], piePoints[4], piePoints[14], piePoints[5], piePoints[8], startAngle, sweepAngle, fill: true, pointIndex);
					graph.FillPieSlice(area, point, solidBrush, pen, piePoints[13], piePoints[4], piePoints[14], piePoints[5], piePoints[8], startAngle, sweepAngle, fill: false, pointIndex);
				}
				else
				{
					graph.FillDoughnutSlice(area, point, solidBrush, pen, piePoints[13], piePoints[4], piePoints[14], piePoints[5], piePoints[22], piePoints[21], piePoints[8], startAngle, sweepAngle, fill: true, doughnutRadius, pointIndex);
					graph.FillDoughnutSlice(area, point, solidBrush, pen, piePoints[13], piePoints[4], piePoints[14], piePoints[5], piePoints[22], piePoints[21], piePoints[8], startAngle, sweepAngle, fill: false, doughnutRadius, pointIndex);
				}
				graph.StopAnimation();
				graph.StartAnimation();
				if (GetLabelStyle(point) == PieLabelStyle.Outside)
				{
					if (point.IsAttributeSet("PieLineColor") || (point.series != null && point.series.IsAttributeSet("PieLineColor")))
					{
						ColorConverter colorConverter = new ColorConverter();
						Color color2 = pen.Color;
						try
						{
							if (point.IsAttributeSet("PieLineColor"))
							{
								color2 = (Color)colorConverter.ConvertFromString(point["PieLineColor"]);
							}
							else if (point.series != null && point.series.IsAttributeSet("PieLineColor"))
							{
								color2 = (Color)colorConverter.ConvertFromString(point.series["PieLineColor"]);
							}
						}
						catch
						{
							if (point.IsAttributeSet("PieLineColor"))
							{
								color2 = (Color)colorConverter.ConvertFromInvariantString(point["PieLineColor"]);
							}
							else if (point.series != null && point.series.IsAttributeSet("PieLineColor"))
							{
								color2 = (Color)colorConverter.ConvertFromInvariantString(point.series["PieLineColor"]);
							}
						}
						pen = new Pen(color2, pen.Width);
					}
					Draw3DOutsideLabels(graph, area, pen, piePoints, point, num, pointIndex);
				}
				graph.StopAnimation();
				break;
			default:
				graph.StartAnimation();
				if (GetLabelStyle(point) == PieLabelStyle.Inside)
				{
					Draw3DInsideLabels(graph, piePoints, point, pointIndex);
				}
				graph.StopAnimation();
				break;
			}
		}

		private PointF[] GetPiePoints(ChartGraphics graph, ChartArea area, float pieWidth, RectangleF rectangle, float startAngle, float sweepAngle, bool relativeCoordinates, float doughnutRadius, bool exploded)
		{
			doughnutRadius = 1f - doughnutRadius / 100f;
			Point3D[] array;
			PointF[] array2;
			if (Doughnut)
			{
				array = new Point3D[29];
				array2 = new PointF[29];
			}
			else
			{
				array = new Point3D[17];
				array2 = new PointF[17];
			}
			array[0] = new Point3D(rectangle.X + (float)Math.Cos(Math.PI) * rectangle.Width / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin(Math.PI) * rectangle.Height / 2f + rectangle.Height / 2f, pieWidth);
			array[1] = new Point3D(rectangle.X + (float)Math.Cos(Math.PI) * rectangle.Width / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin(Math.PI) * rectangle.Height / 2f + rectangle.Height / 2f, 0f);
			array[2] = new Point3D(rectangle.X + (float)Math.Cos(0.0) * rectangle.Width / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin(0.0) * rectangle.Height / 2f + rectangle.Height / 2f, pieWidth);
			array[3] = new Point3D(rectangle.X + (float)Math.Cos(0.0) * rectangle.Width / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin(0.0) * rectangle.Height / 2f + rectangle.Height / 2f, 0f);
			array[4] = new Point3D(rectangle.X + (float)Math.Cos((double)startAngle * Math.PI / 180.0) * rectangle.Width / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin((double)startAngle * Math.PI / 180.0) * rectangle.Height / 2f + rectangle.Height / 2f, pieWidth);
			array[5] = new Point3D(rectangle.X + (float)Math.Cos((double)(startAngle + sweepAngle) * Math.PI / 180.0) * rectangle.Width / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin((double)(startAngle + sweepAngle) * Math.PI / 180.0) * rectangle.Height / 2f + rectangle.Height / 2f, pieWidth);
			array[6] = new Point3D(rectangle.X + (float)Math.Cos((double)startAngle * Math.PI / 180.0) * rectangle.Width / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin((double)startAngle * Math.PI / 180.0) * rectangle.Height / 2f + rectangle.Height / 2f, 0f);
			array[7] = new Point3D(rectangle.X + (float)Math.Cos((double)(startAngle + sweepAngle) * Math.PI / 180.0) * rectangle.Width / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin((double)(startAngle + sweepAngle) * Math.PI / 180.0) * rectangle.Height / 2f + rectangle.Height / 2f, 0f);
			array[8] = new Point3D(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f, pieWidth);
			array[9] = new Point3D(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f, 0f);
			array[10] = new Point3D(rectangle.X + (float)Math.Cos((double)(startAngle + sweepAngle / 2f) * Math.PI / 180.0) * rectangle.Width / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin((double)(startAngle + sweepAngle / 2f) * Math.PI / 180.0) * rectangle.Height / 2f + rectangle.Height / 2f, pieWidth);
			float num = (!exploded) ? 1.3f : 1.1f;
			array[11] = new Point3D(rectangle.X + (float)Math.Cos((double)(startAngle + sweepAngle / 2f) * Math.PI / 180.0) * rectangle.Width * num / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin((double)(startAngle + sweepAngle / 2f) * Math.PI / 180.0) * rectangle.Height * num / 2f + rectangle.Height / 2f, pieWidth);
			if (Doughnut)
			{
				array[12] = new Point3D(rectangle.X + (float)Math.Cos((double)(startAngle + sweepAngle / 2f) * Math.PI / 180.0) * rectangle.Width * (1f + doughnutRadius) / 4f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin((double)(startAngle + sweepAngle / 2f) * Math.PI / 180.0) * rectangle.Height * (1f + doughnutRadius) / 4f + rectangle.Height / 2f, pieWidth);
			}
			else
			{
				array[12] = new Point3D(rectangle.X + (float)Math.Cos((double)(startAngle + sweepAngle / 2f) * Math.PI / 180.0) * rectangle.Width * 0.5f / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin((double)(startAngle + sweepAngle / 2f) * Math.PI / 180.0) * rectangle.Height * 0.5f / 2f + rectangle.Height / 2f, pieWidth);
			}
			array[13] = new Point3D(rectangle.X, rectangle.Y, pieWidth);
			array[14] = new Point3D(rectangle.Right, rectangle.Bottom, pieWidth);
			array[15] = new Point3D(rectangle.X, rectangle.Y, 0f);
			array[16] = new Point3D(rectangle.Right, rectangle.Bottom, 0f);
			if (Doughnut)
			{
				array[17] = new Point3D(rectangle.X + (float)Math.Cos(Math.PI) * rectangle.Width * doughnutRadius / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin(Math.PI) * rectangle.Height * doughnutRadius / 2f + rectangle.Height / 2f, pieWidth);
				array[18] = new Point3D(rectangle.X + (float)Math.Cos(Math.PI) * rectangle.Width * doughnutRadius / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin(Math.PI) * rectangle.Height * doughnutRadius / 2f + rectangle.Height / 2f, 0f);
				array[19] = new Point3D(rectangle.X + (float)Math.Cos(0.0) * rectangle.Width * doughnutRadius / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin(0.0) * rectangle.Height * doughnutRadius / 2f + rectangle.Height / 2f, pieWidth);
				array[20] = new Point3D(rectangle.X + (float)Math.Cos(0.0) * rectangle.Width * doughnutRadius / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin(0.0) * rectangle.Height * doughnutRadius / 2f + rectangle.Height / 2f, 0f);
				array[21] = new Point3D(rectangle.X + (float)Math.Cos((double)startAngle * Math.PI / 180.0) * rectangle.Width * doughnutRadius / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin((double)startAngle * Math.PI / 180.0) * rectangle.Height * doughnutRadius / 2f + rectangle.Height / 2f, pieWidth);
				array[22] = new Point3D(rectangle.X + (float)Math.Cos((double)(startAngle + sweepAngle) * Math.PI / 180.0) * rectangle.Width * doughnutRadius / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin((double)(startAngle + sweepAngle) * Math.PI / 180.0) * rectangle.Height * doughnutRadius / 2f + rectangle.Height / 2f, pieWidth);
				array[23] = new Point3D(rectangle.X + (float)Math.Cos((double)startAngle * Math.PI / 180.0) * rectangle.Width * doughnutRadius / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin((double)startAngle * Math.PI / 180.0) * rectangle.Height * doughnutRadius / 2f + rectangle.Height / 2f, 0f);
				array[24] = new Point3D(rectangle.X + (float)Math.Cos((double)(startAngle + sweepAngle) * Math.PI / 180.0) * rectangle.Width * doughnutRadius / 2f + rectangle.Width / 2f, rectangle.Y + (float)Math.Sin((double)(startAngle + sweepAngle) * Math.PI / 180.0) * rectangle.Height * doughnutRadius / 2f + rectangle.Height / 2f, 0f);
				rectangle.Inflate((0f - rectangle.Width) * (1f - doughnutRadius) / 2f, (0f - rectangle.Height) * (1f - doughnutRadius) / 2f);
				array[25] = new Point3D(rectangle.X, rectangle.Y, pieWidth);
				array[26] = new Point3D(rectangle.Right, rectangle.Bottom, pieWidth);
				array[27] = new Point3D(rectangle.X, rectangle.Y, 0f);
				array[28] = new Point3D(rectangle.Right, rectangle.Bottom, 0f);
			}
			area.matrix3D.TransformPoints(array);
			int num2 = 0;
			Point3D[] array3 = array;
			foreach (Point3D point3D in array3)
			{
				array2[num2] = point3D.PointF;
				if (relativeCoordinates)
				{
					array2[num2] = graph.GetAbsolutePoint(array2[num2]);
				}
				num2++;
			}
			return array2;
		}

		private void DrawPieCurves(ChartGraphics graph, ChartArea area, DataPoint dataPoint, float startAngle, float sweepAngle, PointF[] points, SolidBrush brushWithoutLight, Pen pen, bool rightPosition, bool sameBackFront, int pointIndex)
		{
			Brush brush = (area.Area3DStyle.Light != 0) ? graph.GetGradientBrush(graph.GetAbsoluteRectangle(area.Position.ToRectangleF()), Color.FromArgb(brushWithoutLight.Color.A, 0, 0, 0), brushWithoutLight.Color, GradientType.VerticalCenter) : brushWithoutLight;
			float num = startAngle + sweepAngle;
			if (sweepAngle > 180f && DrawPieCurvesBigSlice(graph, area, dataPoint, startAngle, sweepAngle, points, brush, pen, rightPosition, sameBackFront, pointIndex))
			{
				return;
			}
			if (startAngle < 180f && num > 180f)
			{
				if (area.Area3DStyle.XAngle < 0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[0], points[6], points[1], startAngle, 180f - startAngle, pointIndex);
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[0], points[5], points[1], points[7], 180f, startAngle + sweepAngle - 180f, pointIndex);
				}
			}
			else if (startAngle < 0f && num > 0f)
			{
				if (area.Area3DStyle.XAngle > 0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[2], points[6], points[3], startAngle, 0f - startAngle, pointIndex);
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[2], points[5], points[3], points[7], 0f, sweepAngle + startAngle, pointIndex);
				}
			}
			else if (startAngle < 360f && num > 360f)
			{
				if (area.Area3DStyle.XAngle > 0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[2], points[6], points[3], startAngle, 360f - startAngle, pointIndex);
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[2], points[5], points[3], points[7], 0f, num - 360f, pointIndex);
				}
			}
			else if ((startAngle < 180f && startAngle >= 0f && area.Area3DStyle.XAngle < 0) || (startAngle < 540f && startAngle >= 360f && area.Area3DStyle.XAngle < 0) || (startAngle >= 180f && startAngle < 360f && area.Area3DStyle.XAngle > 0) || (startAngle >= -180f && startAngle < 0f && area.Area3DStyle.XAngle > 0))
			{
				graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[5], points[6], points[7], startAngle, sweepAngle, pointIndex);
			}
		}

		private bool DrawPieCurvesBigSlice(ChartGraphics graph, ChartArea area, DataPoint dataPoint, float startAngle, float sweepAngle, PointF[] points, Brush brush, Pen pen, bool rightPosition, bool sameBackFront, int pointIndex)
		{
			float num = startAngle + sweepAngle;
			if (area.Area3DStyle.XAngle > 0)
			{
				if (startAngle < 180f && num > 360f)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[2], points[0], points[3], points[1], 0f, -180f, pointIndex);
				}
				else
				{
					if (!(startAngle < 0f) || !(num > 180f))
					{
						return false;
					}
					if (sameBackFront)
					{
						if (rightPosition)
						{
							graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[0], points[5], points[1], points[7], 180f, num - 180f, pointIndex);
						}
						else
						{
							graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[2], points[6], points[3], startAngle, 0f - startAngle, pointIndex);
						}
					}
					else
					{
						graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[2], points[6], points[3], startAngle, 0f - startAngle, pointIndex);
						graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[0], points[5], points[1], points[7], 180f, num - 180f, pointIndex);
					}
				}
			}
			else if (startAngle < 0f && num > 180f)
			{
				graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[2], points[0], points[3], points[1], 0f, 180f, pointIndex);
			}
			else
			{
				if (!(startAngle < 180f) || !(num > 360f))
				{
					return false;
				}
				if (sameBackFront)
				{
					if (rightPosition)
					{
						graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[0], points[6], points[1], startAngle, 180f - startAngle, pointIndex);
					}
					else
					{
						graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[2], points[5], points[3], points[7], 0f, num - 360f, pointIndex);
					}
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[2], points[5], points[3], points[7], 0f, num - 360f, pointIndex);
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[0], points[6], points[1], startAngle, 180f - startAngle, pointIndex);
				}
			}
			return true;
		}

		private void DrawDoughnutCurves(ChartGraphics graph, ChartArea area, DataPoint dataPoint, float startAngle, float sweepAngle, PointF[] points, SolidBrush brushWithoutLight, Pen pen, bool rightPosition, bool sameBackFront, int pointIndex)
		{
			Brush brush = (area.Area3DStyle.Light != 0) ? graph.GetGradientBrush(graph.GetAbsoluteRectangle(area.Position.ToRectangleF()), Color.FromArgb(brushWithoutLight.Color.A, 0, 0, 0), brushWithoutLight.Color, GradientType.VerticalCenter) : brushWithoutLight;
			float num = startAngle + sweepAngle;
			if (sweepAngle > 180f && DrawDoughnutCurvesBigSlice(graph, area, dataPoint, startAngle, sweepAngle, points, brush, pen, rightPosition, sameBackFront, pointIndex))
			{
				return;
			}
			if (startAngle < 180f && num > 180f)
			{
				if (area.Area3DStyle.XAngle > 0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[17], points[23], points[18], startAngle, 180f - startAngle, pointIndex);
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[17], points[22], points[18], points[24], 180f, startAngle + sweepAngle - 180f, pointIndex);
				}
			}
			else if (startAngle < 0f && num > 0f)
			{
				if (area.Area3DStyle.XAngle < 0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[19], points[23], points[20], startAngle, 0f - startAngle, pointIndex);
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[19], points[22], points[20], points[24], 0f, sweepAngle + startAngle, pointIndex);
				}
			}
			else if (startAngle < 360f && num > 360f)
			{
				if (area.Area3DStyle.XAngle < 0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[19], points[23], points[20], startAngle, 360f - startAngle, pointIndex);
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[19], points[22], points[20], points[24], 0f, num - 360f, pointIndex);
				}
			}
			else if ((startAngle < 180f && startAngle >= 0f && area.Area3DStyle.XAngle > 0) || (startAngle < 540f && startAngle >= 360f && area.Area3DStyle.XAngle > 0) || (startAngle >= 180f && startAngle < 360f && area.Area3DStyle.XAngle < 0) || (startAngle >= -180f && startAngle < 0f && area.Area3DStyle.XAngle < 0))
			{
				graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[22], points[23], points[24], startAngle, sweepAngle, pointIndex);
			}
		}

		private bool DrawDoughnutCurvesBigSlice(ChartGraphics graph, ChartArea area, DataPoint dataPoint, float startAngle, float sweepAngle, PointF[] points, Brush brush, Pen pen, bool rightPosition, bool sameBackFront, int pointIndex)
		{
			float num = startAngle + sweepAngle;
			if (area.Area3DStyle.XAngle < 0)
			{
				if (startAngle < 180f && num > 360f)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[19], points[17], points[20], points[18], 0f, -180f, pointIndex);
				}
				else
				{
					if (!(startAngle < 0f) || !(num > 180f))
					{
						return false;
					}
					if (sameBackFront)
					{
						if (rightPosition)
						{
							graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[17], points[22], points[18], points[24], 180f, num - 180f, pointIndex);
						}
						else
						{
							graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[19], points[23], points[20], startAngle, 0f - startAngle, pointIndex);
						}
					}
					else
					{
						graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[19], points[23], points[20], startAngle, 0f - startAngle, pointIndex);
						graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[17], points[22], points[18], points[24], 180f, num - 180f, pointIndex);
					}
				}
			}
			else if (startAngle < 0f && num > 180f)
			{
				graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[19], points[17], points[20], points[18], 0f, 180f, pointIndex);
			}
			else
			{
				if (!(startAngle < 180f) || !(num > 360f))
				{
					return false;
				}
				if (sameBackFront)
				{
					if (rightPosition)
					{
						graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[17], points[23], points[18], startAngle, 180f - startAngle, pointIndex);
					}
					else
					{
						graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[19], points[22], points[20], points[24], 0f, num - 360f, pointIndex);
					}
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[19], points[22], points[20], points[24], 0f, num - 360f, pointIndex);
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[17], points[23], points[18], startAngle, 180f - startAngle, pointIndex);
				}
			}
			return true;
		}

		private DataPoint[] PointOrder(Series series, ChartArea area, out float[] newStartAngleList, out float[] newSweepAngleList, out int[] newPointIndexList, out bool sameBackFrontPoint)
		{
			int num = -1;
			int num2 = -1;
			sameBackFrontPoint = false;
			double num3 = 0.0;
			int num4 = 0;
			foreach (DataPoint point in series.Points)
			{
				if (point.Empty)
				{
					num4++;
				}
				if (!point.Empty)
				{
					num3 += Math.Abs(point.YValues[0]);
				}
			}
			int num5 = series.Points.Count - num4;
			DataPoint[] points = new DataPoint[num5];
			float[] array = new float[num5];
			float[] array2 = new float[num5];
			int[] array3 = new int[num5];
			newStartAngleList = new float[num5];
			newSweepAngleList = new float[num5];
			newPointIndexList = new int[num5];
			if (num3 <= 0.0)
			{
				return null;
			}
			int num6 = 0;
			double num7 = area.Area3DStyle.YAngle;
			foreach (DataPoint point2 in series.Points)
			{
				if (!point2.Empty)
				{
					double num8 = (float)(Math.Abs(point2.YValues[0]) * 360.0 / num3);
					double num9 = num7 + num8;
					array[num6] = (float)num7;
					array2[num6] = (float)num8;
					array3[num6] = num6;
					if ((num7 <= -90.0 && num9 > -90.0) || (num7 <= 270.0 && num9 > 270.0 && points[0] == null))
					{
						num = num6;
						points[0] = point2;
						newStartAngleList[0] = array[num6];
						newSweepAngleList[0] = array2[num6];
						newPointIndexList[0] = array3[num6];
					}
					if ((num7 <= 90.0 && num9 > 90.0) || (num7 <= 450.0 && num9 > 450.0 && num2 == -1 && (points[points.Length - 1] == null || points.Length == 1)))
					{
						num2 = num6;
						points[points.Length - 1] = point2;
						newStartAngleList[points.Length - 1] = array[num6];
						newSweepAngleList[points.Length - 1] = array2[num6];
						newPointIndexList[points.Length - 1] = array3[num6];
					}
					num6++;
					num7 += num8;
				}
			}
			if (num2 == -1 || num == -1)
			{
				throw new InvalidOperationException(SR.ExceptionPieUnassignedFrontBackPoints);
			}
			if (num2 == num && points.Length != 1)
			{
				points[points.Length - 1] = null;
				newStartAngleList[points.Length - 1] = 0f;
				newSweepAngleList[points.Length - 1] = 0f;
				newPointIndexList[points.Length - 1] = 0;
				sameBackFrontPoint = true;
			}
			if (num2 == num)
			{
				float num10 = array[num] + array2[num] / 2f;
				bool flag = false;
				if ((num10 > -90f && num10 < 90f) || (num10 > 270f && num10 < 450f))
				{
					flag = true;
				}
				int num11 = num5 - num2;
				num6 = 0;
				foreach (DataPoint point3 in series.Points)
				{
					if (point3.Empty)
					{
						continue;
					}
					if (num6 == num2)
					{
						num6++;
						continue;
					}
					if (num6 < num2)
					{
						if (points[num11] != null)
						{
							throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
						}
						points[num11] = point3;
						newStartAngleList[num11] = array[num6];
						newSweepAngleList[num11] = array2[num6];
						newPointIndexList[num11] = array3[num6];
						num11++;
					}
					num6++;
				}
				num6 = 0;
				num11 = 1;
				foreach (DataPoint point4 in series.Points)
				{
					if (point4.Empty)
					{
						continue;
					}
					if (num6 == num2)
					{
						num6++;
						continue;
					}
					if (num6 > num2)
					{
						if (points[num11] != null)
						{
							throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
						}
						points[num11] = point4;
						newStartAngleList[num11] = array[num6];
						newSweepAngleList[num11] = array2[num6];
						newPointIndexList[num11] = array3[num6];
						num11++;
					}
					num6++;
				}
				if (flag)
				{
					SwitchPoints(num5, ref points, ref newStartAngleList, ref newSweepAngleList, ref newPointIndexList, num == num2);
				}
			}
			else if (num2 < num)
			{
				num6 = 0;
				int num12 = 1;
				foreach (DataPoint point5 in series.Points)
				{
					if (point5.Empty)
					{
						continue;
					}
					if (num6 == num2 || num6 == num)
					{
						num6++;
						continue;
					}
					if (num6 > num)
					{
						if (points[num12] != null)
						{
							throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
						}
						points[num12] = point5;
						newStartAngleList[num12] = array[num6];
						newSweepAngleList[num12] = array2[num6];
						newPointIndexList[num12] = array3[num6];
						num12++;
					}
					num6++;
				}
				num6 = 0;
				foreach (DataPoint point6 in series.Points)
				{
					if (point6.Empty)
					{
						continue;
					}
					if (num6 == num2 || num6 == num)
					{
						num6++;
						continue;
					}
					if (num6 < num2)
					{
						if (points[num12] != null)
						{
							throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
						}
						points[num12] = point6;
						newStartAngleList[num12] = array[num6];
						newSweepAngleList[num12] = array2[num6];
						newPointIndexList[num12] = array3[num6];
						num12++;
					}
					num6++;
				}
				num12 = points.Length - 2;
				num6 = 0;
				foreach (DataPoint point7 in series.Points)
				{
					if (point7.Empty)
					{
						continue;
					}
					if (num6 == num2 || num6 == num)
					{
						num6++;
						continue;
					}
					if (num6 > num2 && num6 < num)
					{
						if (points[num12] != null)
						{
							throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
						}
						points[num12] = point7;
						newStartAngleList[num12] = array[num6];
						newSweepAngleList[num12] = array2[num6];
						newPointIndexList[num12] = array3[num6];
						num12--;
					}
					num6++;
				}
			}
			else
			{
				int num13 = 1;
				num6 = 0;
				foreach (DataPoint point8 in series.Points)
				{
					if (point8.Empty)
					{
						continue;
					}
					if (num6 == num2 || num6 == num)
					{
						num6++;
						continue;
					}
					if (num6 > num && num6 < num2)
					{
						if (points[num13] != null)
						{
							throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
						}
						points[num13] = point8;
						newStartAngleList[num13] = array[num6];
						newSweepAngleList[num13] = array2[num6];
						newPointIndexList[num13] = array3[num6];
						num13++;
					}
					num6++;
				}
				num13 = points.Length - 2;
				num6 = 0;
				foreach (DataPoint point9 in series.Points)
				{
					if (point9.Empty)
					{
						continue;
					}
					if (num6 == num2 || num6 == num)
					{
						num6++;
						continue;
					}
					if (num6 > num2)
					{
						if (points[num13] != null)
						{
							throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
						}
						points[num13] = point9;
						newStartAngleList[num13] = array[num6];
						newSweepAngleList[num13] = array2[num6];
						newPointIndexList[num13] = array3[num6];
						num13--;
					}
					num6++;
				}
				num6 = 0;
				foreach (DataPoint point10 in series.Points)
				{
					if (point10.Empty)
					{
						continue;
					}
					if (num6 == num2 || num6 == num)
					{
						num6++;
						continue;
					}
					if (num6 < num)
					{
						if (points[num13] != null)
						{
							throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
						}
						points[num13] = point10;
						newStartAngleList[num13] = array[num6];
						newSweepAngleList[num13] = array2[num6];
						newPointIndexList[num13] = array3[num6];
						num13--;
					}
					num6++;
				}
			}
			if (area.Area3DStyle.XAngle > 0)
			{
				SwitchPoints(num5, ref points, ref newStartAngleList, ref newSweepAngleList, ref newPointIndexList, num == num2);
			}
			return points;
		}

		private void SwitchPoints(int numOfPoints, ref DataPoint[] points, ref float[] newStartAngleList, ref float[] newSweepAngleList, ref int[] newPointIndexList, bool sameBackFront)
		{
			float[] array = new float[numOfPoints];
			float[] array2 = new float[numOfPoints];
			int[] array3 = new int[numOfPoints];
			DataPoint[] array4 = new DataPoint[numOfPoints];
			int num = 0;
			if (sameBackFront)
			{
				num = 1;
				array4[0] = points[0];
				array[0] = newStartAngleList[0];
				array2[0] = newSweepAngleList[0];
				array3[0] = newPointIndexList[0];
			}
			for (int i = num; i < numOfPoints; i++)
			{
				if (points[i] == null)
				{
					throw new InvalidOperationException(SR.ExceptionPieOrderOperationInvalid);
				}
				array4[numOfPoints - i - 1 + num] = points[i];
				array[numOfPoints - i - 1 + num] = newStartAngleList[i];
				array2[numOfPoints - i - 1 + num] = newSweepAngleList[i];
				array3[numOfPoints - i - 1 + num] = newPointIndexList[i];
			}
			points = array4;
			newStartAngleList = array;
			newSweepAngleList = array2;
			newPointIndexList = array3;
		}

		private void InitPieSize(ChartGraphics graph, ChartArea area, ref RectangleF pieRectangle, ref float pieWidth, DataPoint[] dataPoints, float[] startAngleList, float[] sweepAngleList, int[] pointIndexList, Series series, float labelLineSize)
		{
			labelColumnLeft = new LabelColumn(area.Position.ToRectangleF());
			labelColumnRight = new LabelColumn(area.Position.ToRectangleF());
			float num = float.MinValue;
			float num2 = float.MinValue;
			int num3 = 0;
			foreach (DataPoint dataPoint in dataPoints)
			{
				if (!dataPoint.Empty)
				{
					float num4 = startAngleList[num3] + sweepAngleList[num3] / 2f;
					if ((num4 >= -90f && num4 < 90f) || (num4 >= 270f && num4 < 450f))
					{
						labelColumnRight.numOfItems++;
					}
					else
					{
						labelColumnLeft.numOfItems++;
					}
					SizeF sizeF = graph.MeasureStringRel(GetLabelText(dataPoint).Replace("\\n", "\n"), dataPoint.Font);
					num = Math.Max(sizeF.Width, num);
					num2 = Math.Max(sizeF.Height, num2);
					num3++;
				}
			}
			float width = pieRectangle.Width;
			float height = pieRectangle.Height;
			pieRectangle.Width = pieRectangle.Width - 2f * num - 2f * pieRectangle.Width * labelLineSize;
			pieRectangle.Height -= pieRectangle.Height * 0.3f;
			if (pieRectangle.Width < width * (float)MinimumRelativePieSize(area))
			{
				pieRectangle.Width = width * (float)MinimumRelativePieSize(area);
			}
			if (pieRectangle.Height < height * (float)MinimumRelativePieSize(area))
			{
				pieRectangle.Height = height * (float)MinimumRelativePieSize(area);
			}
			if (width * 0.8f < pieRectangle.Width)
			{
				pieRectangle.Width *= 0.8f;
			}
			pieRectangle.X += (width - pieRectangle.Width) / 2f;
			pieWidth = pieRectangle.Width / width * pieWidth;
			pieRectangle.Y += (height - pieRectangle.Height) / 2f;
			SizeF size = new SizeF(1.4f * series.Font.Size, 1.4f * series.Font.Size);
			size = graph.GetRelativeSize(size);
			int maxNumOfRows = (int)(pieRectangle.Height / num2);
			labelColumnRight.Initialize(pieRectangle, rightPosition: true, maxNumOfRows, labelLineSize);
			labelColumnLeft.Initialize(pieRectangle, rightPosition: false, maxNumOfRows, labelLineSize);
		}

		private void FillPieLabelOutside(ChartGraphics graph, ChartArea area, RectangleF pieRectangle, float pieWidth, DataPoint point, float startAngle, float sweepAngle, int pointIndx, float doughnutRadius, bool exploded)
		{
			float num = startAngle + sweepAngle / 2f;
			float y = GetPiePoints(graph, area, pieWidth, pieRectangle, startAngle, sweepAngle, relativeCoordinates: false, doughnutRadius, exploded)[11].Y;
			if ((num >= -90f && num < 90f) || (num >= 270f && num < 450f))
			{
				labelColumnRight.InsertLabel(point, y, pointIndx);
			}
			else
			{
				labelColumnLeft.InsertLabel(point, y, pointIndx);
			}
		}

		private void Draw3DOutsideLabels(ChartGraphics graph, ChartArea area, Pen pen, PointF[] points, DataPoint point, float midAngle, int pointIndex)
		{
			string labelText = GetLabelText(point);
			if (labelText.Length == 0)
			{
				return;
			}
			graph.DrawLine(pen, points[10], points[11]);
			StringFormat stringFormat = new StringFormat();
			stringFormat.LineAlignment = StringAlignment.Center;
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(area.Position.ToRectangleF());
			RectangleF empty = RectangleF.Empty;
			PointF absolutePoint;
			if ((midAngle >= -90f && midAngle < 90f) || (midAngle >= 270f && midAngle < 450f))
			{
				LabelColumn labelColumn = labelColumnRight;
				stringFormat.Alignment = StringAlignment.Near;
				float height = graph.GetAbsoluteSize(new SizeF(0f, labelColumnRight.columnHeight)).Height;
				absolutePoint = graph.GetAbsolutePoint(labelColumn.GetLabelPosition(point));
				if (points[11].X > absolutePoint.X)
				{
					absolutePoint.X = points[11].X + 10f;
				}
				empty.X = absolutePoint.X;
				empty.Width = absoluteRectangle.Right - empty.X;
				empty.Y = absolutePoint.Y - height / 2f;
				empty.Height = height;
			}
			else
			{
				LabelColumn labelColumn = labelColumnLeft;
				stringFormat.Alignment = StringAlignment.Far;
				float height2 = graph.GetAbsoluteSize(new SizeF(0f, labelColumnLeft.columnHeight)).Height;
				absolutePoint = graph.GetAbsolutePoint(labelColumn.GetLabelPosition(point));
				if (points[11].X < absolutePoint.X)
				{
					absolutePoint.X = points[11].X - 10f;
				}
				empty.X = absoluteRectangle.X;
				empty.Width = absolutePoint.X - empty.X;
				empty.Y = absolutePoint.Y - height2 / 2f;
				empty.Height = height2;
			}
			stringFormat.FormatFlags = (StringFormatFlags.NoWrap | StringFormatFlags.LineLimit);
			stringFormat.Trimming = StringTrimming.EllipsisWord;
			graph.DrawLine(pen, points[11], absolutePoint);
			empty = graph.GetRelativeRectangle(empty);
			SizeF sizeF = graph.MeasureStringRel(labelText.Replace("\\n", "\n"), point.Font);
			sizeF.Height += sizeF.Height / 8f;
			float num = sizeF.Width / (float)labelText.Length / 2f;
			sizeF.Width += num;
			RectangleF backPosition = new RectangleF(empty.X, empty.Y + empty.Height / 2f - sizeF.Height / 2f, sizeF.Width, sizeF.Height);
			if (stringFormat.Alignment == StringAlignment.Near)
			{
				backPosition.X -= num / 2f;
			}
			else if (stringFormat.Alignment == StringAlignment.Center)
			{
				backPosition.X = empty.X + (empty.Width - sizeF.Width) / 2f;
			}
			else if (stringFormat.Alignment == StringAlignment.Far)
			{
				backPosition.X = empty.Right - sizeF.Width - num / 2f;
			}
			graph.DrawPointLabelStringRel(graph.common, labelText, point.Font, new SolidBrush(point.FontColor), empty, stringFormat, 0, backPosition, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, point.series, point, pointIndex);
		}

		private void Draw3DInsideLabels(ChartGraphics graph, PointF[] points, DataPoint point, int pointIndex)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.LineAlignment = StringAlignment.Center;
			stringFormat.Alignment = StringAlignment.Center;
			string labelText = GetLabelText(point);
			PointF relativePoint = graph.GetRelativePoint(points[12]);
			SizeF relativeSize = graph.GetRelativeSize(graph.MeasureString(labelText.Replace("\\n", "\n"), point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
			RectangleF empty = RectangleF.Empty;
			SizeF sizeF = new SizeF(relativeSize.Width, relativeSize.Height);
			sizeF.Height += relativeSize.Height / 8f;
			sizeF.Width += sizeF.Width / (float)labelText.Length;
			graph.DrawPointLabelStringRel(backPosition: new RectangleF(relativePoint.X - sizeF.Width / 2f, relativePoint.Y - sizeF.Height / 2f - relativeSize.Height / 10f, sizeF.Width, sizeF.Height), common: graph.common, text: labelText, font: point.Font, brush: new SolidBrush(point.FontColor), position: relativePoint, format: stringFormat, angle: 0, backColor: point.LabelBackColor, borderColor: point.LabelBorderColor, borderWidth: point.LabelBorderWidth, borderStyle: point.LabelBorderStyle, series: point.series, point: point, pointIndex: pointIndex);
		}

		private static string GetPointLabel(DataPoint point, bool alwaysIncludeAxisLabel = false)
		{
			string empty = string.Empty;
			if (point.Label.Length == 0)
			{
				empty = point.AxisLabel;
				if (point.series != null && point.series.IsAttributeSet("AutoAxisLabels") && string.Equals(point.series.GetAttribute("AutoAxisLabels"), "false", StringComparison.OrdinalIgnoreCase) && !alwaysIncludeAxisLabel)
				{
					empty = string.Empty;
				}
			}
			else
			{
				empty = point.Label;
			}
			return point.ReplaceKeywords(empty);
		}

		internal static string GetLabelText(DataPoint point, bool alwaysIncludeAxisLabel = false)
		{
			string pointLabel = GetPointLabel(point, alwaysIncludeAxisLabel);
			string text;
			if (point.Label.Length == 0 && point.ShowLabelAsValue)
			{
				text = ValueConverter.FormatValue(point.series.chart, point, point.YValues[0], point.LabelFormat, point.series.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = pointLabel;
				if (point.series.chart != null && point.series.chart.LocalizeTextHandler != null)
				{
					text = point.series.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
				}
			}
			return text;
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
