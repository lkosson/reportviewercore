using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class StackedAreaChart : AreaChart
	{
		private ArrayList stackedData = new ArrayList();

		protected GraphicsPath areaBottomPath = new GraphicsPath();

		protected double prevPosY = double.NaN;

		protected double prevNegY = double.NaN;

		protected double prevPositionX = double.NaN;

		protected bool hundredPercentStacked;

		public override string Name => "StackedArea";

		public override bool Stacked => true;

		public StackedAreaChart()
		{
			multiSeries = true;
			COPCoordinatesToCheck = (COPCoordinates.X | COPCoordinates.Y);
		}

		protected override float GetDefaultTension()
		{
			return 0f;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public override void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			graph.SetClip(area.PlotAreaPosition.ToRectangleF());
			ProcessChartType(selection: false, graph, common, area, seriesToDraw);
			graph.ResetClip();
		}

		protected override void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			ArrayList arrayList = null;
			ArrayList arrayList2 = null;
			if (area.Area3DStyle.Enable3D)
			{
				base.ProcessChartType(selection, graph, common, area, seriesToDraw);
				return;
			}
			bool flag = area.IndexedSeries((string[])area.GetSeriesFromChartType(Name).ToArray(typeof(string)));
			bool flag2 = false;
			bool flag3 = false;
			int num = -1;
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) != 0 || item.ChartArea != area.Name || !item.IsVisible())
				{
					continue;
				}
				if (areaPath != null)
				{
					areaPath.Dispose();
					areaPath = null;
				}
				areaBottomPath.Reset();
				if (num == -1)
				{
					num = item.Points.Count;
				}
				else if (num != item.Points.Count)
				{
					throw new ArgumentException(SR.ExceptionStackedAreaChartSeriesDataPointsNumberMismatch);
				}
				hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
				vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
				hAxisMin = hAxis.GetViewMinimum();
				hAxisMax = hAxis.GetViewMaximum();
				vAxisMin = vAxis.GetViewMinimum();
				vAxisMax = vAxis.GetViewMaximum();
				axisPos.X = (float)vAxis.GetPosition(vAxis.Crossing);
				axisPos.Y = (float)vAxis.GetPosition(vAxis.Crossing);
				axisPos = graph.GetAbsolutePoint(axisPos);
				if (arrayList2 == null)
				{
					arrayList2 = new ArrayList(item.Points.Count);
				}
				else
				{
					arrayList = arrayList2;
					arrayList2 = new ArrayList(item.Points.Count);
				}
				if (!selection)
				{
					common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				int num2 = 0;
				float num3 = axisPos.Y;
				float y = axisPos.Y;
				PointF pointF = PointF.Empty;
				PointF pointF2 = PointF.Empty;
				foreach (DataPoint point in item.Points)
				{
					point.positionRel = new PointF(float.NaN, float.NaN);
					double num4 = point.Empty ? 0.0 : GetYValue(common, area, item, point, num2, 0);
					double num5 = flag ? ((double)num2 + 1.0) : point.XValue;
					if (arrayList != null && num2 < arrayList.Count)
					{
						num4 += (double)arrayList[num2];
					}
					arrayList2.Insert(num2, num4);
					float y2 = (float)vAxis.GetPosition(num4);
					float x = (float)hAxis.GetPosition(num5);
					point.positionRel = new PointF(x, y2);
					num4 = vAxis.GetLogValue(num4);
					num5 = hAxis.GetLogValue(num5);
					if (pointF == PointF.Empty)
					{
						pointF.X = x;
						pointF.Y = y2;
						if (arrayList != null && num2 < arrayList.Count)
						{
							num3 = (float)vAxis.GetPosition((double)arrayList[num2]);
							num3 = graph.GetAbsolutePoint(new PointF(num3, num3)).Y;
						}
						pointF = graph.GetAbsolutePoint(pointF);
						num2++;
						continue;
					}
					pointF2.X = x;
					pointF2.Y = y2;
					if (arrayList != null && num2 < arrayList.Count)
					{
						y = (float)vAxis.GetPosition((double)arrayList[num2]);
						y = graph.GetAbsolutePoint(new PointF(y, y)).Y;
					}
					pointF2 = graph.GetAbsolutePoint(pointF2);
					pointF.X = (float)Math.Round(pointF.X);
					pointF2.X = (float)Math.Round(pointF2.X);
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddLine(pointF.X, pointF.Y, pointF2.X, pointF2.Y);
					graphicsPath.AddLine(pointF2.X, pointF2.Y, pointF2.X, y);
					graphicsPath.AddLine(pointF2.X, y, pointF.X, num3);
					graphicsPath.AddLine(pointF.X, num3, pointF.X, pointF.Y);
					if (common.ProcessModePaint)
					{
						if (!point.Empty)
						{
							GetYValue(common, area, item, item.Points[num2 - 1], num2 - 1, 0);
						}
						double num6 = flag ? ((double)num2) : item.Points[num2 - 1].XValue;
						if ((num5 <= hAxisMin && num6 <= hAxisMin) || (num5 >= hAxisMax && num6 >= hAxisMax))
						{
							pointF = pointF2;
							num3 = y;
							num2++;
							continue;
						}
						Brush brush = null;
						if (point.BackHatchStyle != 0)
						{
							brush = graph.GetHatchBrush(point.BackHatchStyle, point.Color, point.BackGradientEndColor);
						}
						else if (point.BackGradientType == GradientType.None)
						{
							brush = ((point.BackImage.Length > 0 && point.BackImageMode != ChartImageWrapMode.Unscaled && point.BackImageMode != ChartImageWrapMode.Scaled) ? graph.GetTextureBrush(point.BackImage, point.BackImageTransparentColor, point.BackImageMode, point.Color) : ((!point.Empty || !(point.Color == Color.Empty)) ? new SolidBrush(point.Color) : new SolidBrush(item.Color)));
						}
						else
						{
							gradientFill = true;
							series = point.series;
						}
						if (point.BorderColor != Color.Empty && point.BorderWidth > 0)
						{
							flag2 = true;
						}
						if (point.Label.Length > 0 || point.ShowLabelAsValue)
						{
							flag3 = true;
						}
						if (!gradientFill)
						{
							graph.StartAnimation();
							graph.StartHotRegion(point);
							SmoothingMode smoothingMode = graph.SmoothingMode;
							graph.SmoothingMode = SmoothingMode.None;
							graph.FillPath(brush, graphicsPath);
							graph.SmoothingMode = smoothingMode;
							Pen pen = new Pen(brush, 1f);
							if (pointF.X != pointF2.X && pointF.Y != pointF2.Y)
							{
								graph.DrawLine(pen, pointF.X, pointF.Y, pointF2.X, pointF2.Y);
							}
							if (pointF.X != pointF2.X && y != num3)
							{
								graph.DrawLine(pen, pointF2.X, y, pointF.X, num3);
							}
							graph.EndHotRegion();
							graph.StopAnimation();
						}
						if (areaPath == null)
						{
							areaPath = new GraphicsPath();
						}
						areaPath.AddLine(pointF.X, pointF.Y, pointF2.X, pointF2.Y);
						areaBottomPath.AddLine(pointF.X, num3, pointF2.X, y);
					}
					if (common.ProcessModeRegions)
					{
						PointF empty = PointF.Empty;
						float[] array = new float[graphicsPath.PointCount * 2];
						PointF[] pathPoints = graphicsPath.PathPoints;
						for (int i = 0; i < graphicsPath.PointCount; i++)
						{
							empty = graph.GetRelativePoint(pathPoints[i]);
							array[2 * i] = empty.X;
							array[2 * i + 1] = empty.Y;
						}
						common.HotRegionsList.AddHotRegion(graph, graphicsPath, relativePath: false, array, point, item.Name, num2);
						if (point.BorderWidth > 1 && point.BorderStyle != 0 && point.BorderColor != Color.Empty)
						{
							GraphicsPath graphicsPath2 = new GraphicsPath();
							graphicsPath2.AddLine(pointF.X, pointF.Y, pointF2.X, pointF2.Y);
							ChartGraphics.Widen(graphicsPath2, new Pen(point.Color, point.BorderWidth + 2));
							empty = PointF.Empty;
							array = new float[graphicsPath2.PointCount * 2];
							for (int j = 0; j < graphicsPath2.PointCount; j++)
							{
								empty = graph.GetRelativePoint(graphicsPath2.PathPoints[j]);
								array[2 * j] = empty.X;
								array[2 * j + 1] = empty.Y;
							}
							common.HotRegionsList.AddHotRegion(graph, graphicsPath2, relativePath: false, array, point, item.Name, num2);
						}
					}
					pointF = pointF2;
					num3 = y;
					num2++;
				}
				if (gradientFill && areaPath != null)
				{
					GraphicsPath graphicsPath3 = new GraphicsPath();
					graphicsPath3.AddPath(areaPath, connect: true);
					areaBottomPath.Reverse();
					graphicsPath3.AddPath(areaBottomPath, connect: true);
					Brush gradientBrush = graph.GetGradientBrush(graphicsPath3.GetBounds(), series.Color, series.BackGradientEndColor, series.BackGradientType);
					graph.FillPath(gradientBrush, graphicsPath3);
					areaPath.Dispose();
					areaPath = null;
					gradientFill = false;
				}
				areaBottomPath.Reset();
				if (!selection)
				{
					common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
			}
			if (flag2)
			{
				arrayList = null;
				arrayList2 = null;
				foreach (Series item2 in common.DataManager.Series)
				{
					if (string.Compare(item2.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) != 0 || item2.ChartArea != area.Name || !item2.IsVisible())
					{
						continue;
					}
					hAxis = area.GetAxis(AxisName.X, item2.XAxisType, item2.XSubAxisName);
					vAxis = area.GetAxis(AxisName.Y, item2.YAxisType, item2.YSubAxisName);
					axisPos.X = (float)vAxis.GetPosition(vAxis.Crossing);
					axisPos.Y = (float)vAxis.GetPosition(vAxis.Crossing);
					axisPos = graph.GetAbsolutePoint(axisPos);
					if (arrayList2 == null)
					{
						arrayList2 = new ArrayList(item2.Points.Count);
					}
					else
					{
						arrayList = arrayList2;
						arrayList2 = new ArrayList(item2.Points.Count);
					}
					int num7 = 0;
					float num8 = axisPos.Y;
					float num9 = axisPos.Y;
					PointF pointF3 = PointF.Empty;
					PointF pointF4 = PointF.Empty;
					foreach (DataPoint point2 in item2.Points)
					{
						double num10 = point2.Empty ? 0.0 : GetYValue(common, area, item2, point2, num7, 0);
						double axisValue = flag ? ((double)num7 + 1.0) : point2.XValue;
						if (arrayList != null && num7 < arrayList.Count)
						{
							num10 += (double)arrayList[num7];
						}
						arrayList2.Insert(num7, num10);
						float y3 = (float)vAxis.GetPosition(num10);
						float x2 = (float)hAxis.GetPosition(axisValue);
						if (pointF3 == PointF.Empty)
						{
							pointF3.X = x2;
							pointF3.Y = y3;
							if (arrayList != null && num7 < arrayList.Count)
							{
								num8 = (float)vAxis.GetPosition((double)arrayList[num7]);
								num8 = graph.GetAbsolutePoint(new PointF(num8, num8)).Y;
							}
							pointF3 = graph.GetAbsolutePoint(pointF3);
							pointF4 = pointF3;
							num9 = num8;
						}
						else
						{
							pointF4.X = x2;
							pointF4.Y = y3;
							if (arrayList != null && num7 < arrayList.Count)
							{
								num9 = (float)vAxis.GetPosition((double)arrayList[num7]);
								num9 = graph.GetAbsolutePoint(new PointF(num9, num9)).Y;
							}
							pointF4 = graph.GetAbsolutePoint(pointF4);
						}
						if (num7 != 0)
						{
							pointF3.X = (float)Math.Round(pointF3.X);
							pointF4.X = (float)Math.Round(pointF4.X);
							graph.StartAnimation();
							graph.DrawLineRel(point2.BorderColor, point2.BorderWidth, point2.BorderStyle, graph.GetRelativePoint(pointF3), graph.GetRelativePoint(pointF4), point2.series.ShadowColor, point2.series.ShadowOffset);
							graph.StopAnimation();
						}
						pointF3 = pointF4;
						num8 = num9;
						num7++;
					}
				}
			}
			if (!flag3)
			{
				return;
			}
			arrayList = null;
			arrayList2 = null;
			foreach (Series item3 in common.DataManager.Series)
			{
				if (string.Compare(item3.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) != 0 || item3.ChartArea != area.Name || !item3.IsVisible())
				{
					continue;
				}
				hAxis = area.GetAxis(AxisName.X, item3.XAxisType, item3.XSubAxisName);
				vAxis = area.GetAxis(AxisName.Y, item3.YAxisType, item3.YSubAxisName);
				axisPos.X = (float)vAxis.GetPosition(vAxis.Crossing);
				axisPos.Y = (float)vAxis.GetPosition(vAxis.Crossing);
				axisPos = graph.GetAbsolutePoint(axisPos);
				if (arrayList2 == null)
				{
					arrayList2 = new ArrayList(item3.Points.Count);
				}
				else
				{
					arrayList = arrayList2;
					arrayList2 = new ArrayList(item3.Points.Count);
				}
				int num11 = 0;
				float num12 = axisPos.Y;
				float num13 = axisPos.Y;
				PointF pointF5 = PointF.Empty;
				PointF pointF6 = PointF.Empty;
				foreach (DataPoint point3 in item3.Points)
				{
					double num14 = point3.Empty ? 0.0 : GetYValue(common, area, item3, point3, num11, 0);
					double axisValue2 = flag ? ((double)num11 + 1.0) : point3.XValue;
					if (arrayList != null && num11 < arrayList.Count)
					{
						num14 += (double)arrayList[num11];
					}
					arrayList2.Insert(num11, num14);
					float y4 = (float)vAxis.GetPosition(num14);
					float x3 = (float)hAxis.GetPosition(axisValue2);
					if (pointF5 == PointF.Empty)
					{
						pointF5.X = x3;
						pointF5.Y = y4;
						if (arrayList != null && num11 < arrayList.Count)
						{
							num12 = (float)vAxis.GetPosition((double)arrayList[num11]);
							num12 = graph.GetAbsolutePoint(new PointF(num12, num12)).Y;
						}
						pointF5 = graph.GetAbsolutePoint(pointF5);
						pointF6 = pointF5;
						num13 = num12;
					}
					else
					{
						pointF6.X = x3;
						pointF6.Y = y4;
						if (arrayList != null && num11 < arrayList.Count)
						{
							num13 = (float)vAxis.GetPosition((double)arrayList[num11]);
							num13 = graph.GetAbsolutePoint(new PointF(num13, num13)).Y;
						}
						pointF6 = graph.GetAbsolutePoint(pointF6);
					}
					if (!point3.Empty && (item3.ShowLabelAsValue || point3.ShowLabelAsValue || point3.Label.Length > 0))
					{
						StringFormat stringFormat = new StringFormat();
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Center;
						string text;
						if (point3.Label.Length == 0)
						{
							double value = GetYValue(common, area, item3, point3, num11, 0);
							if (hundredPercentStacked && point3.LabelFormat.Length == 0)
							{
								value = Math.Round(value, 2);
							}
							text = ValueConverter.FormatValue(item3.chart, point3, value, point3.LabelFormat, item3.YValueType, ChartElementType.DataPoint);
						}
						else
						{
							text = point3.ReplaceKeywords(point3.Label);
							if (item3.chart != null && item3.chart.LocalizeTextHandler != null)
							{
								text = item3.chart.LocalizeTextHandler(point3, text, point3.ElementId, ChartElementType.DataPoint);
							}
						}
						Region clip = graph.Clip;
						graph.Clip = new Region();
						graph.StartAnimation();
						PointF empty2 = PointF.Empty;
						empty2.X = pointF6.X;
						empty2.Y = pointF6.Y - (pointF6.Y - num13) / 2f;
						empty2 = graph.GetRelativePoint(empty2);
						SizeF relativeSize = graph.GetRelativeSize(graph.MeasureString(text, point3.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
						RectangleF empty3 = RectangleF.Empty;
						SizeF sizeF = new SizeF(relativeSize.Width, relativeSize.Height);
						sizeF.Height += relativeSize.Height / 8f;
						sizeF.Width += sizeF.Width / (float)text.Length;
						graph.DrawPointLabelStringRel(backPosition: new RectangleF(empty2.X - sizeF.Width / 2f, empty2.Y - sizeF.Height / 2f - relativeSize.Height / 10f, sizeF.Width, sizeF.Height), common: common, text: text, font: point3.Font, brush: new SolidBrush(point3.FontColor), position: empty2, format: stringFormat, angle: point3.FontAngle, backColor: point3.LabelBackColor, borderColor: point3.LabelBorderColor, borderWidth: point3.LabelBorderWidth, borderStyle: point3.LabelBorderStyle, series: item3, point: point3, pointIndex: num11);
						graph.StopAnimation();
						graph.Clip = clip;
					}
					pointF5 = pointF6;
					num12 = num13;
					num11++;
				}
			}
		}

		protected override GraphicsPath Draw3DSurface(ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment)
		{
			if (pointLoopIndex != 2)
			{
				return base.Draw3DSurface(area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, topDarkening, bottomDarkening, thirdPointPosition, fourthPointPosition, clippedSegment);
			}
			DataPoint3D dataPoint3D = (DataPoint3D)points[pointIndex];
			if (dataPoint3D.index == 2)
			{
				int neighborPointIndex = 0;
				DataPoint3D pointEx = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, dataPoint3D, ref neighborPointIndex);
				DrawLabels3D(area, graph, area.Common, pointEx, positionZ, depth);
			}
			DrawLabels3D(area, graph, area.Common, dataPoint3D, positionZ, depth);
			return new GraphicsPath();
		}

		protected override void GetTopSurfaceVisibility(ChartArea area, DataPoint3D firstPoint, DataPoint3D secondPoint, bool upSideDown, float positionZ, float depth, Matrix3D matrix, ref SurfaceNames visibleSurfaces)
		{
			base.GetTopSurfaceVisibility(area, firstPoint, secondPoint, upSideDown, positionZ, depth, matrix, ref visibleSurfaces);
			if ((visibleSurfaces & SurfaceNames.Top) == SurfaceNames.Top)
			{
				bool flag = false;
				foreach (Series item in area.Common.DataManager.Series)
				{
					if (string.Compare(item.ChartTypeName, secondPoint.dataPoint.series.ChartTypeName, ignoreCase: true, CultureInfo.CurrentCulture) != 0)
					{
						continue;
					}
					if (flag)
					{
						DataPointAttributes dataPointAttributes = item.Points[secondPoint.index - 1];
						if (item.Points[secondPoint.index - 1].Empty)
						{
							dataPointAttributes = item.EmptyPointStyle;
						}
						if (dataPointAttributes.Color.A == byte.MaxValue)
						{
							visibleSurfaces ^= SurfaceNames.Top;
						}
						break;
					}
					if (string.Compare(item.Name, secondPoint.dataPoint.series.Name, StringComparison.Ordinal) == 0)
					{
						flag = true;
					}
				}
			}
			if ((visibleSurfaces & SurfaceNames.Bottom) == SurfaceNames.Bottom)
			{
				return;
			}
			DataPointAttributes dataPointAttributes2 = null;
			foreach (Series item2 in area.Common.DataManager.Series)
			{
				if (string.Compare(item2.ChartTypeName, secondPoint.dataPoint.series.ChartTypeName, StringComparison.OrdinalIgnoreCase) != 0)
				{
					continue;
				}
				if (dataPointAttributes2 != null && string.Compare(item2.Name, secondPoint.dataPoint.series.Name, StringComparison.Ordinal) == 0)
				{
					if (dataPointAttributes2.Color.A != byte.MaxValue)
					{
						visibleSurfaces |= SurfaceNames.Bottom;
					}
					break;
				}
				dataPointAttributes2 = item2.Points[secondPoint.index - 1];
				if (item2.Points[secondPoint.index - 1].Empty)
				{
					dataPointAttributes2 = item2.EmptyPointStyle;
				}
			}
		}

		protected override void GetBottomPointsPosition(CommonElements common, ChartArea area, float axisPosition, ref DataPoint3D firstPoint, ref DataPoint3D secondPoint, PointF thirdPointPosition, PointF fourthPointPosition, out PointF thirdPoint, out PointF fourthPoint)
		{
			Axis axis = area.GetAxis(AxisName.Y, firstPoint.dataPoint.series.YAxisType, firstPoint.dataPoint.series.YSubAxisName);
			Axis axis2 = area.GetAxis(AxisName.X, firstPoint.dataPoint.series.XAxisType, firstPoint.dataPoint.series.XSubAxisName);
			double yValue = GetYValue(area.Common, area, firstPoint.dataPoint.series, firstPoint.dataPoint, firstPoint.index - 1, 0);
			double num = (float)firstPoint.xPosition;
			if (yValue >= 0.0)
			{
				if (double.IsNaN(prevPosY))
				{
					yValue = axisPosition;
				}
				else
				{
					yValue = axis.GetPosition(prevPosY);
					num = axis2.GetPosition(prevPositionX);
				}
			}
			else if (double.IsNaN(prevNegY))
			{
				yValue = axisPosition;
			}
			else
			{
				yValue = axis.GetPosition(prevNegY);
				num = axis2.GetPosition(prevPositionX);
			}
			thirdPoint = new PointF((float)num, (float)yValue);
			yValue = GetYValue(area.Common, area, secondPoint.dataPoint.series, secondPoint.dataPoint, secondPoint.index - 1, 0);
			num = (float)secondPoint.xPosition;
			if (yValue >= 0.0)
			{
				if (double.IsNaN(prevPosY))
				{
					yValue = axisPosition;
				}
				else
				{
					yValue = axis.GetPosition(prevPosY);
					num = axis2.GetPosition(prevPositionX);
				}
			}
			else if (double.IsNaN(prevNegY))
			{
				yValue = axisPosition;
			}
			else
			{
				yValue = axis.GetPosition(prevNegY);
				num = axis2.GetPosition(prevPositionX);
			}
			fourthPoint = new PointF((float)num, (float)yValue);
			if (!float.IsNaN(thirdPointPosition.X))
			{
				thirdPoint.X = (float)((firstPoint.xCenterVal == 0.0) ? firstPoint.xPosition : firstPoint.xCenterVal);
				thirdPoint.Y = (thirdPointPosition.X - fourthPoint.X) / (thirdPoint.X - fourthPoint.X) * (thirdPoint.Y - fourthPoint.Y) + fourthPoint.Y;
				thirdPoint.X = thirdPointPosition.X;
			}
			if (!float.IsNaN(thirdPointPosition.Y))
			{
				thirdPoint.Y = thirdPointPosition.Y;
			}
			if (!float.IsNaN(fourthPointPosition.X))
			{
				fourthPoint.X = (float)((secondPoint.xCenterVal == 0.0) ? secondPoint.xPosition : secondPoint.xCenterVal);
				fourthPoint.Y = (fourthPointPosition.X - fourthPoint.X) / (thirdPoint.X - fourthPoint.X) * (thirdPoint.Y - fourthPoint.Y) + fourthPoint.Y;
				fourthPoint.X = fourthPointPosition.X;
			}
			if (!float.IsNaN(fourthPointPosition.Y))
			{
				fourthPoint.Y = fourthPointPosition.Y;
			}
		}

		protected override int GetPointLoopNumber(bool selection, ArrayList pointsArray)
		{
			if (selection)
			{
				return 1;
			}
			int result = 1;
			foreach (DataPoint3D item in pointsArray)
			{
				if (item.dataPoint.Color.A != byte.MaxValue)
				{
					result = 2;
				}
				if (item.dataPoint.Label.Length > 0 || item.dataPoint.ShowLabelAsValue || item.dataPoint.series.ShowLabelAsValue)
				{
					return 3;
				}
			}
			return result;
		}

		private void DrawLabels3D(ChartArea area, ChartGraphics graph, CommonElements common, DataPoint3D pointEx, float positionZ, float depth)
		{
			string label = pointEx.dataPoint.Label;
			bool showLabelAsValue = pointEx.dataPoint.ShowLabelAsValue;
			if ((pointEx.dataPoint.Empty || (!(pointEx.dataPoint.series.ShowLabelAsValue || showLabelAsValue) && label.Length <= 0)) && !showLabelAsValue && label.Length <= 0)
			{
				return;
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			string text;
			if (label.Length == 0)
			{
				double value = pointEx.dataPoint.YValues[(labelYValueIndex == -1) ? yValueIndex : labelYValueIndex];
				if (hundredPercentStacked && pointEx.dataPoint.LabelFormat.Length == 0)
				{
					value = Math.Round(value, 2);
				}
				text = ValueConverter.FormatValue(pointEx.dataPoint.series.chart, pointEx.dataPoint, value, pointEx.dataPoint.LabelFormat, pointEx.dataPoint.series.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = pointEx.dataPoint.ReplaceKeywords(label);
				if (pointEx.dataPoint.series.chart != null && pointEx.dataPoint.series.chart.LocalizeTextHandler != null)
				{
					text = pointEx.dataPoint.series.chart.LocalizeTextHandler(pointEx.dataPoint, text, pointEx.dataPoint.ElementId, ChartElementType.DataPoint);
				}
			}
			Point3D[] array = new Point3D[1]
			{
				new Point3D((float)pointEx.xPosition, (float)(pointEx.yPosition + pointEx.height) / 2f, positionZ + depth)
			};
			area.matrix3D.TransformPoints(array);
			SizeF relativeSize = graph.GetRelativeSize(graph.MeasureString(text, pointEx.dataPoint.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
			RectangleF backPosition = RectangleF.Empty;
			SizeF sizeF = new SizeF(relativeSize.Width, relativeSize.Height);
			sizeF.Height += relativeSize.Height / 8f;
			sizeF.Width += sizeF.Width / (float)text.Length;
			backPosition = new RectangleF(array[0].PointF.X - sizeF.Width / 2f, array[0].PointF.Y - sizeF.Height / 2f - relativeSize.Height / 10f, sizeF.Width, sizeF.Height);
			graph.DrawPointLabelStringRel(common, text, pointEx.dataPoint.Font, new SolidBrush(pointEx.dataPoint.FontColor), array[0].PointF, stringFormat, pointEx.dataPoint.FontAngle, backPosition, pointEx.dataPoint.LabelBackColor, pointEx.dataPoint.LabelBorderColor, pointEx.dataPoint.LabelBorderWidth, pointEx.dataPoint.LabelBorderStyle, pointEx.dataPoint.series, pointEx.dataPoint, pointEx.index - 1);
		}

		public override double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			double num = double.NaN;
			if (!area.Area3DStyle.Enable3D)
			{
				return point.YValues[0];
			}
			if (yValueIndex == -1)
			{
				double crossing = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName).Crossing;
				num = GetYValue(common, area, series, point, pointIndex, 0);
				if (area.Area3DStyle.Enable3D && num < 0.0)
				{
					num = 0.0 - num;
				}
				if (num >= 0.0)
				{
					if (!double.IsNaN(prevPosY))
					{
						crossing = prevPosY;
					}
				}
				else if (!double.IsNaN(prevNegY))
				{
					crossing = prevNegY;
				}
				return num - crossing;
			}
			prevPosY = double.NaN;
			prevNegY = double.NaN;
			prevPositionX = double.NaN;
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(series.ChartArea, item.ChartArea, StringComparison.Ordinal) != 0 || string.Compare(series.ChartTypeName, item.ChartTypeName, StringComparison.OrdinalIgnoreCase) != 0 || !item.IsVisible())
				{
					continue;
				}
				num = item.Points[pointIndex].YValues[0];
				if (area.Area3DStyle.Enable3D && num < 0.0)
				{
					num = 0.0 - num;
				}
				if (!double.IsNaN(num))
				{
					if (num >= 0.0 && !double.IsNaN(prevPosY))
					{
						num += prevPosY;
					}
					if (num < 0.0 && !double.IsNaN(prevNegY))
					{
						num += prevNegY;
					}
				}
				if (string.Compare(series.Name, item.Name, StringComparison.Ordinal) != 0)
				{
					if (num >= 0.0)
					{
						prevPosY = num;
					}
					if (num < 0.0)
					{
						prevNegY = num;
					}
					prevPositionX = item.Points[pointIndex].XValue;
					if (prevPositionX == 0.0 && ChartElement.IndexedSeries(series))
					{
						prevPositionX = pointIndex + 1;
					}
					continue;
				}
				return num;
			}
			return num;
		}
	}
}
