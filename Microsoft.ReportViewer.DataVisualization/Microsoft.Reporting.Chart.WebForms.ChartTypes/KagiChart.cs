using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class KagiChart : StepLineChart
	{
		internal bool kagiChart;

		internal Color kagiUpColor = Color.Empty;

		internal int currentKagiDirection;

		public override string Name => "Kagi";

		internal static void PrepareData(Series series, IServiceContainer serviceContainer)
		{
			if (string.Compare(series.ChartTypeName, "Kagi", StringComparison.OrdinalIgnoreCase) != 0 || !series.IsVisible())
			{
				return;
			}
			Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
			if (chart == null)
			{
				throw new InvalidOperationException(SR.ExceptionKagiNullReference);
			}
			ChartArea chartArea = chart.ChartAreas[series.ChartArea];
			foreach (Series item in chart.Series)
			{
				if (item.IsVisible() && item != series && chartArea == chart.ChartAreas[item.ChartArea])
				{
					throw new InvalidOperationException(SR.ExceptionKagiCanNotCombine);
				}
			}
			string name = "KAGI_ORIGINAL_DATA_" + series.Name;
			if (chart.Series.GetIndex(name) != -1)
			{
				return;
			}
			Series series3 = new Series(name, series.YValuesPerPoint);
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
			series.XValueIndexed = true;
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
			FillKagiData(series, series3);
		}

		internal static bool UnPrepareData(Series series, IServiceContainer serviceContainer)
		{
			if (series.Name.StartsWith("KAGI_ORIGINAL_DATA_", StringComparison.Ordinal))
			{
				Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
				if (chart == null)
				{
					throw new InvalidOperationException(SR.ExceptionKagiNullReference);
				}
				Series series2 = chart.Series[series.Name.Substring(19)];
				series2.Points.Clear();
				if (!series.IsAttributeSet("TempDesignData"))
				{
					foreach (DataPoint point in series.Points)
					{
						series2.Points.Add(point);
					}
				}
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

		private static double GetReversalAmount(Series series, Series originalData, int yValueIndex, out double percentOfPrice)
		{
			double result = 1.0;
			percentOfPrice = 3.0;
			if (series.IsAttributeSet("ReversalAmount"))
			{
				string text = series["ReversalAmount"].Trim();
				bool flag = text.EndsWith("%", StringComparison.Ordinal);
				if (flag)
				{
					text = text.Substring(0, text.Length - 1);
				}
				try
				{
					if (flag)
					{
						percentOfPrice = double.Parse(text, CultureInfo.InvariantCulture);
						return result;
					}
					result = double.Parse(text, CultureInfo.InvariantCulture);
					percentOfPrice = 0.0;
					return result;
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionKagiAttributeFormatInvalid("ReversalAmount"));
				}
			}
			return result;
		}

		private static void FillKagiData(Series series, Series originalData)
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
					throw new InvalidOperationException(SR.ExceptionKagiAttributeFormatInvalid("UsedYValue"));
				}
				if (num >= series.YValuesPerPoint)
				{
					throw new InvalidOperationException(SR.ExceptionKagiAttributeOutOfRange("UsedYValue"));
				}
			}
			double percentOfPrice = 0.0;
			double num2 = GetReversalAmount(series, originalData, num, out percentOfPrice);
			double num3 = double.NaN;
			int num4 = 0;
			int num5 = 0;
			foreach (DataPoint point in originalData.Points)
			{
				if (double.IsNaN(num3))
				{
					num3 = point.YValues[num];
					DataPoint dataPoint2 = point.Clone();
					dataPoint2.series = series;
					dataPoint2.XValue = point.XValue;
					dataPoint2.YValues[0] = point.YValues[num];
					series.Points.Add(dataPoint2);
					num5++;
					continue;
				}
				if (percentOfPrice != 0.0)
				{
					num2 = num3 / 100.0 * percentOfPrice;
				}
				int num6 = 0;
				num6 = ((point.YValues[num] > num3) ? 1 : ((point.YValues[num] < num3) ? (-1) : 0));
				if (num6 != 0)
				{
					if (num6 == num4)
					{
						series.Points[series.Points.Count - 1].YValues[0] = point.YValues[num];
						series.Points[series.Points.Count - 1]["OriginalPointIndex"] = num5.ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						if (Math.Abs(point.YValues[num] - num3) < num2)
						{
							num5++;
							continue;
						}
						DataPoint dataPoint3 = point.Clone();
						dataPoint3["OriginalPointIndex"] = num5.ToString(CultureInfo.InvariantCulture);
						dataPoint3.series = series;
						dataPoint3.XValue = point.XValue;
						dataPoint3.YValues[0] = point.YValues[num];
						series.Points.Add(dataPoint3);
					}
					num3 = point.YValues[num];
					num4 = num6;
				}
				num5++;
			}
		}

		protected override void DrawLine(ChartGraphics graph, CommonElements common, DataPoint point, Series series, PointF[] points, int pointIndex, float tension)
		{
			if (pointIndex <= 0)
			{
				return;
			}
			if (currentKagiDirection == 0)
			{
				kagiUpColor = ChartGraphics.GetGradientColor(series.Color, Color.Black, 0.5);
				string text = series["PriceUpColor"];
				ColorConverter colorConverter = new ColorConverter();
				if (text != null)
				{
					try
					{
						kagiUpColor = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, text);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionKagiAttributeFormatInvalid("Up Brick color"));
					}
				}
				currentKagiDirection = ((points[pointIndex - 1].Y > points[pointIndex].Y) ? 1 : (-1));
			}
			Color color = (currentKagiDirection == 1) ? kagiUpColor : point.Color;
			PointF pointF = points[pointIndex - 1];
			PointF pointF2 = new PointF(points[pointIndex].X, points[pointIndex - 1].Y);
			PointF pointF3 = points[pointIndex];
			PointF empty = PointF.Empty;
			if (pointIndex >= 2 && ((points[pointIndex - 1].Y > points[pointIndex].Y) ? 1 : (-1)) != currentKagiDirection)
			{
				PointF pointF4 = points[pointIndex - 2];
				bool flag = false;
				if (pointF.Y > pointF4.Y && pointF.Y > pointF3.Y && pointF4.Y > pointF3.Y)
				{
					flag = true;
				}
				else if (pointF.Y < pointF4.Y && pointF.Y < pointF3.Y && pointF4.Y < pointF3.Y)
				{
					flag = true;
				}
				if (flag)
				{
					empty.Y = pointF4.Y;
					empty.X = pointF2.X;
				}
			}
			pointF.X = (float)Math.Round(pointF.X);
			pointF.Y = (float)Math.Round(pointF.Y);
			pointF2.X = (float)Math.Round(pointF2.X);
			pointF2.Y = (float)Math.Round(pointF2.Y);
			pointF3.X = (float)Math.Round(pointF3.X);
			pointF3.Y = (float)Math.Round(pointF3.Y);
			if (!empty.IsEmpty)
			{
				empty.X = (float)Math.Round(empty.X);
				empty.Y = (float)Math.Round(empty.Y);
			}
			graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, graph.GetRelativePoint(pointF), graph.GetRelativePoint(pointF2), series.ShadowColor, series.ShadowOffset);
			if (empty.IsEmpty)
			{
				graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, graph.GetRelativePoint(pointF2), graph.GetRelativePoint(pointF3), series.ShadowColor, series.ShadowOffset);
			}
			else
			{
				graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, graph.GetRelativePoint(pointF2), graph.GetRelativePoint(empty), series.ShadowColor, series.ShadowOffset);
				currentKagiDirection = ((currentKagiDirection != 1) ? 1 : (-1));
				color = ((currentKagiDirection == 1) ? kagiUpColor : point.Color);
				graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, graph.GetRelativePoint(empty), graph.GetRelativePoint(pointF3), series.ShadowColor, series.ShadowOffset);
			}
			if (common.ProcessModeRegions)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddLine(pointF, pointF2);
				graphicsPath.AddLine(pointF2, pointF3);
				ChartGraphics.Widen(graphicsPath, new Pen(point.Color, point.BorderWidth + 2));
				PointF empty2 = PointF.Empty;
				float[] array = new float[graphicsPath.PointCount * 2];
				PointF[] pathPoints = graphicsPath.PathPoints;
				for (int i = 0; i < graphicsPath.PointCount; i++)
				{
					empty2 = graph.GetRelativePoint(pathPoints[i]);
					array[2 * i] = empty2.X;
					array[2 * i + 1] = empty2.Y;
				}
				common.HotRegionsList.AddHotRegion(graph, graphicsPath, relativePath: false, array, point, series.Name, pointIndex);
			}
		}

		protected override PointF[] GetPointsPosition(ChartGraphics graph, Series series, bool indexedSeries)
		{
			PointF[] array = new PointF[series.Points.Count];
			int num = 0;
			foreach (DataPoint point in series.Points)
			{
				double yValue = GetYValue(common, area, series, point, num, yValueIndex);
				double position = vAxis.GetPosition(yValue);
				double position2 = hAxis.GetPosition(point.XValue);
				if (indexedSeries)
				{
					position2 = hAxis.GetPosition(num + 1);
				}
				array[num] = new PointF((float)(position2 * (double)(graph.common.ChartPicture.Width - 1) / 100.0), (float)(position * (double)(graph.common.ChartPicture.Height - 1) / 100.0));
				num++;
			}
			return array;
		}

		protected override GraphicsPath Draw3DSurface(ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment)
		{
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			if (centerPointIndex == int.MaxValue)
			{
				centerPointIndex = GetCenterPointIndex(points);
			}
			DataPoint3D dataPoint3D = (DataPoint3D)points[pointIndex];
			int neighborPointIndex = pointIndex;
			DataPoint3D dataPoint3D2 = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, multiSeries ? dataPoint3D : null, ref neighborPointIndex);
			DataPoint3D dataPoint3D3 = dataPoint3D;
			if (prevDataPointEx.dataPoint.Empty)
			{
				dataPoint3D3 = prevDataPointEx;
			}
			else if (dataPoint3D2.index > dataPoint3D.index)
			{
				dataPoint3D3 = dataPoint3D2;
			}
			Color color = useBorderColor ? dataPoint3D3.dataPoint.BorderColor : dataPoint3D3.dataPoint.Color;
			ChartDashStyle borderStyle = dataPoint3D3.dataPoint.BorderStyle;
			if (dataPoint3D3.dataPoint.Empty && dataPoint3D3.dataPoint.Color == Color.Empty)
			{
				color = Color.Gray;
			}
			if (dataPoint3D3.dataPoint.Empty && dataPoint3D3.dataPoint.BorderStyle == ChartDashStyle.NotSet)
			{
				borderStyle = ChartDashStyle.Solid;
			}
			if (currentKagiDirection == 0)
			{
				kagiUpColor = dataPoint3D.dataPoint.series.Color;
				string text = dataPoint3D.dataPoint.series["PriceUpColor"];
				ColorConverter colorConverter = new ColorConverter();
				if (text != null)
				{
					try
					{
						kagiUpColor = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, text);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionKagiAttributeFormatInvalid("Up Brick color"));
					}
				}
				currentKagiDirection = ((dataPoint3D2.yPosition > dataPoint3D.yPosition) ? 1 : (-1));
			}
			Color backColor = (currentKagiDirection == 1) ? kagiUpColor : color;
			DataPoint3D dataPoint3D4 = new DataPoint3D();
			dataPoint3D4.xPosition = dataPoint3D.xPosition;
			dataPoint3D4.yPosition = dataPoint3D2.yPosition;
			bool flag = true;
			if (pointIndex + 1 < points.Count && ((DataPoint3D)points[pointIndex + 1]).index == dataPoint3D2.index)
			{
				flag = false;
			}
			if (centerPointIndex != int.MaxValue && pointIndex >= centerPointIndex)
			{
				flag = false;
			}
			DataPoint3D dataPoint3D5 = null;
			if (dataPoint3D.index >= 2 && ((dataPoint3D2.yPosition > dataPoint3D.yPosition) ? 1 : (-1)) != currentKagiDirection)
			{
				DataPoint3D dataPoint3D6 = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 2, multiSeries ? dataPoint3D : null, ref neighborPointIndex);
				bool flag2 = false;
				if (dataPoint3D2.yPosition > dataPoint3D6.yPosition && dataPoint3D2.yPosition > dataPoint3D.yPosition && dataPoint3D6.yPosition > dataPoint3D.yPosition)
				{
					flag2 = true;
				}
				else if (dataPoint3D2.yPosition < dataPoint3D6.yPosition && dataPoint3D2.yPosition < dataPoint3D.yPosition && dataPoint3D6.yPosition < dataPoint3D.yPosition)
				{
					flag2 = true;
				}
				if (flag2)
				{
					dataPoint3D5 = new DataPoint3D();
					dataPoint3D5.xPosition = dataPoint3D.xPosition;
					dataPoint3D5.yPosition = dataPoint3D6.yPosition;
					dataPoint3D5.dataPoint = dataPoint3D.dataPoint;
				}
			}
			GraphicsPath[] array = new GraphicsPath[3];
			for (int i = 0; i < 2; i++)
			{
				DataPoint3D firstPoint = dataPoint3D2;
				DataPoint3D secondPoint = dataPoint3D;
				LineSegmentType lineSegmentType = LineSegmentType.First;
				switch (i)
				{
				case 0:
					lineSegmentType = (flag ? LineSegmentType.First : LineSegmentType.Last);
					dataPoint3D4.dataPoint = (flag ? dataPoint3D.dataPoint : dataPoint3D2.dataPoint);
					firstPoint = (flag ? dataPoint3D2 : dataPoint3D4);
					secondPoint = (flag ? dataPoint3D4 : dataPoint3D);
					break;
				case 1:
					lineSegmentType = ((!flag) ? LineSegmentType.First : LineSegmentType.Last);
					dataPoint3D4.dataPoint = ((!flag) ? dataPoint3D.dataPoint : dataPoint3D.dataPoint);
					firstPoint = ((!flag) ? dataPoint3D2 : dataPoint3D4);
					secondPoint = ((!flag) ? dataPoint3D4 : dataPoint3D);
					break;
				}
				if (lineSegmentType == LineSegmentType.First || dataPoint3D5 == null)
				{
					array[i] = new GraphicsPath();
					array[i] = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, points, pointIndex, 0f, operationType, lineSegmentType, showPointLines ? true : false, forceThickBorder: false, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
				}
				else
				{
					if (!flag)
					{
						backColor = ((currentKagiDirection == -1) ? kagiUpColor : color);
					}
					array[i] = new GraphicsPath();
					array[i] = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, firstPoint, dataPoint3D5, points, pointIndex, 0f, operationType, LineSegmentType.Middle, showPointLines ? true : false, forceThickBorder: false, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
					graph.frontLinePen = null;
					currentKagiDirection = ((currentKagiDirection != 1) ? 1 : (-1));
					backColor = ((!flag) ? ((currentKagiDirection == -1) ? kagiUpColor : color) : ((currentKagiDirection == 1) ? kagiUpColor : color));
					array[2] = new GraphicsPath();
					array[2] = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, dataPoint3D5, secondPoint, points, pointIndex, 0f, operationType, lineSegmentType, showPointLines ? true : false, forceThickBorder: false, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
					if (!flag)
					{
						backColor = ((currentKagiDirection == 1) ? kagiUpColor : color);
					}
				}
				graph.frontLinePen = null;
			}
			if (graphicsPath != null)
			{
				if (array[0] != null)
				{
					graphicsPath.AddPath(array[0], connect: true);
				}
				if (array[1] != null)
				{
					graphicsPath.AddPath(array[1], connect: true);
				}
				if (array[2] != null)
				{
					graphicsPath.AddPath(array[2], connect: true);
				}
			}
			return graphicsPath;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public override void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			currentKagiDirection = 0;
			base.Paint(graph, common, area, seriesToDraw);
		}
	}
}
