using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class LineChart : PointChart
	{
		protected float lineTension;

		protected int centerPointIndex = int.MaxValue;

		protected bool useBorderColor;

		protected bool disableShadow;

		protected bool drawShadowOnly;

		private Pen linePen = new Pen(Color.Black);

		protected double hAxisMin;

		protected double hAxisMax;

		protected double vAxisMin;

		protected double vAxisMax;

		protected bool clipRegionSet;

		protected bool multiSeries;

		protected COPCoordinates COPCoordinatesToCheck = COPCoordinates.X;

		protected int allPointsLoopsNumber = 1;

		protected bool showPointLines;

		protected bool drawOutsideLines;

		private bool processBaseChart;

		public override string Name => "Line";

		public override bool Stacked => false;

		public override bool RequireAxes => true;

		public override bool SupportLogarithmicAxes => true;

		public override bool SwitchValueAxes => false;

		public override bool SideBySideSeries => false;

		public override bool ZeroCrossing => true;

		public override bool DataPointsInLegend => false;

		public override bool ApplyPaletteColorsToPoints => false;

		public override int YValuesPerPoint => 1;

		public LineChart()
			: base(alwaysDrawMarkers: false)
		{
			middleMarker = false;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public override LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Line;
		}

		public override void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			base.area = area;
			processBaseChart = false;
			ProcessChartType(selection: false, graph, common, area, seriesToDraw);
			if (processBaseChart)
			{
				base.ProcessChartType(selection: false, graph, common, area, seriesToDraw);
			}
		}

		protected override void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			if (area.Area3DStyle.Enable3D)
			{
				processBaseChart = true;
				ProcessLineChartType3D(selection, graph, common, area, seriesToDraw);
				return;
			}
			ArrayList seriesFromChartType = area.GetSeriesFromChartType(Name);
			bool flag = area.IndexedSeries((string[])seriesFromChartType.ToArray(typeof(string)));
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, Name, ignoreCase: true, CultureInfo.CurrentCulture) != 0 || item.ChartArea != area.Name || !item.IsVisible() || (seriesToDraw != null && seriesToDraw.Name != item.Name))
				{
					continue;
				}
				hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
				vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
				hAxisMin = hAxis.GetViewMinimum();
				hAxisMax = hAxis.GetViewMaximum();
				vAxisMin = vAxis.GetViewMinimum();
				vAxisMax = vAxis.GetViewMaximum();
				float num = (float)(graph.common.ChartPicture.Width - 1) / 100f;
				float num2 = (float)(graph.common.ChartPicture.Height - 1) / 100f;
				if (!selection)
				{
					common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				bool flag2 = false;
				PointF[] array = null;
				if (lineTension == 0f && !common.ProcessModeRegions)
				{
					array = new PointF[item.Points.Count];
				}
				else
				{
					flag2 = true;
					array = GetPointsPosition(graph, item, flag);
					if (lineTension != 0f)
					{
						float num3 = 0.1f;
						for (int i = 1; i < array.Length; i++)
						{
							if (Math.Abs(array[i - 1].X - array[i].X) < num3)
							{
								if (array[i].X > array[i - 1].X)
								{
									array[i].X = array[i - 1].X + num3;
								}
								else
								{
									array[i].X = array[i - 1].X - num3;
								}
							}
							if (Math.Abs(array[i - 1].Y - array[i].Y) < num3)
							{
								if (array[i].Y > array[i - 1].Y)
								{
									array[i].Y = array[i - 1].Y + num3;
								}
								else
								{
									array[i].Y = array[i - 1].Y - num3;
								}
							}
						}
					}
				}
				if (array.Length > 1)
				{
					int num4 = 0;
					DataPoint dataPoint = null;
					double num5 = 0.0;
					double num6 = 0.0;
					bool showLabelAsValue = item.ShowLabelAsValue;
					bool flag3 = false;
					foreach (DataPoint point in item.Points)
					{
						flag3 = false;
						point.positionRel = new PointF(float.NaN, float.NaN);
						if (!processBaseChart)
						{
							_ = point.MarkerSize;
							string markerImage = point.MarkerImage;
							MarkerStyle markerStyle = point.MarkerStyle;
							if (alwaysDrawMarkers || markerStyle != 0 || markerImage.Length > 0 || showLabelAsValue || point.ShowLabelAsValue || point.Label.Length > 0)
							{
								processBaseChart = true;
							}
						}
						double yValue = GetYValue(common, area, item, point, num4, yValueIndex);
						double yValue2 = flag ? ((double)(num4 + 1)) : point.XValue;
						if (num4 != 0)
						{
							yValue = vAxis.GetLogValue(yValue);
							yValue2 = hAxis.GetLogValue(yValue2);
							if (((yValue2 <= hAxisMin && num6 < hAxisMin) || (yValue2 >= hAxisMax && num6 > hAxisMax) || (yValue <= vAxisMin && num5 < vAxisMin) || (yValue >= vAxisMax && num5 > vAxisMax)) && !drawOutsideLines)
							{
								bool flag4 = true;
								if (common.ProcessModeRegions && num4 + 1 < item.Points.Count)
								{
									DataPoint dataPoint3 = item.Points[num4 + 1];
									double num7 = flag ? ((double)(num4 + 2)) : dataPoint3.XValue;
									if ((yValue2 < hAxisMin && num7 > hAxisMin) || (yValue2 > hAxisMax && num7 < hAxisMax))
									{
										flag4 = false;
									}
									if (flag4)
									{
										GetYValue(common, area, item, dataPoint3, num4 + 1, yValueIndex);
										if ((yValue < vAxisMin && num7 > vAxisMin) || (yValue > vAxisMax && num7 < vAxisMax))
										{
											flag4 = false;
										}
									}
								}
								if (flag4)
								{
									num4++;
									dataPoint = point;
									num5 = yValue;
									num6 = yValue2;
									continue;
								}
							}
							clipRegionSet = false;
							if ((double)lineTension != 0.0 || num6 < hAxisMin || num6 > hAxisMax || yValue2 > hAxisMax || yValue2 < hAxisMin || num5 < vAxisMin || num5 > vAxisMax || yValue < vAxisMin || yValue > vAxisMax)
							{
								graph.SetClip(area.PlotAreaPosition.ToRectangleF());
								clipRegionSet = true;
							}
							if (lineTension == 0f && !flag2)
							{
								float num8 = 0f;
								float num9 = 0f;
								if (!flag3)
								{
									num8 = (float)vAxis.GetLinearPosition(num5);
									num9 = (float)hAxis.GetLinearPosition(num6);
									array[num4 - 1] = new PointF(num9 * num, num8 * num2);
								}
								num8 = (float)vAxis.GetLinearPosition(yValue);
								num9 = (float)hAxis.GetLinearPosition(yValue2);
								array[num4] = new PointF(num9 * num, num8 * num2);
								flag3 = true;
							}
							point.positionRel = graph.GetRelativePoint(array[num4]);
							Init2DAnimation(common, point, num4, yValue2, num6, graph, array);
							graph.StartHotRegion(point);
							graph.StartAnimation();
							if (num4 != 0 && dataPoint.Empty)
							{
								DrawLine(graph, common, dataPoint, item, array, num4, lineTension);
							}
							else
							{
								DrawLine(graph, common, point, item, array, num4, lineTension);
							}
							graph.StopAnimation();
							graph.EndHotRegion();
							if (clipRegionSet)
							{
								graph.ResetClip();
							}
							dataPoint = point;
							num5 = yValue;
							num6 = yValue2;
						}
						else
						{
							dataPoint = point;
							num5 = GetYValue(common, area, item, point, num4, 0);
							num6 = (flag ? ((double)(num4 + 1)) : point.XValue);
							num5 = vAxis.GetLogValue(num5);
							num6 = hAxis.GetLogValue(num6);
							point.positionRel = new PointF((float)hAxis.GetPosition(num6), (float)vAxis.GetPosition(num5));
						}
						if (num4 == 0)
						{
							DrawLine(graph, common, point, item, array, num4, lineTension);
						}
						num4++;
					}
				}
				else if (array.Length == 1 && item.Points.Count == 1 && !processBaseChart && (alwaysDrawMarkers || item.Points[0].MarkerStyle != 0 || item.Points[0].MarkerImage.Length > 0 || item.ShowLabelAsValue || item.Points[0].ShowLabelAsValue || item.Points[0].Label.Length > 0))
				{
					processBaseChart = true;
				}
				array = null;
				if (!selection)
				{
					common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
			}
		}

		protected virtual void DrawLine(ChartGraphics graph, CommonElements common, DataPoint point, Series series, PointF[] points, int pointIndex, float tension)
		{
			int borderWidth = point.BorderWidth;
			if (common.ProcessModePaint && pointIndex > 0)
			{
				Color color = useBorderColor ? point.BorderColor : point.Color;
				ChartDashStyle borderStyle = point.BorderStyle;
				if (!disableShadow && series.ShadowOffset != 0 && series.ShadowColor != Color.Empty)
				{
					graph.shadowDrawingMode = true;
					if (color != Color.Empty && color != Color.Transparent && borderWidth > 0 && borderStyle != 0)
					{
						Pen pen = new Pen((series.ShadowColor.A != byte.MaxValue) ? series.ShadowColor : Color.FromArgb(useBorderColor ? ((int)point.BorderColor.A / 2) : ((int)point.Color.A / 2), series.ShadowColor), borderWidth);
						pen.DashStyle = graph.GetPenStyle(point.BorderStyle);
						pen.StartCap = LineCap.Round;
						pen.EndCap = LineCap.Round;
						GraphicsState gstate = graph.Save();
						Matrix matrix = graph.Transform.Clone();
						matrix.Translate(series.ShadowOffset, series.ShadowOffset);
						graph.Transform = matrix;
						if (lineTension == 0f)
						{
							try
							{
								graph.DrawLine(pen, points[pointIndex - 1], points[pointIndex]);
							}
							catch (OverflowException)
							{
								DrawTruncatedLine(graph, pen, points[pointIndex - 1], points[pointIndex]);
							}
						}
						else
						{
							graph.DrawCurve(pen, points, pointIndex - 1, 1, tension);
						}
						graph.Restore(gstate);
					}
					graph.shadowDrawingMode = false;
				}
				if (drawShadowOnly)
				{
					return;
				}
				if (color != Color.Empty && borderWidth > 0 && borderStyle != 0)
				{
					if (linePen.Color != color)
					{
						linePen.Color = color;
					}
					if (linePen.Width != (float)borderWidth)
					{
						linePen.Width = borderWidth;
					}
					if (linePen.DashStyle != graph.GetPenStyle(borderStyle))
					{
						linePen.DashStyle = graph.GetPenStyle(borderStyle);
					}
					if (linePen.StartCap != LineCap.Round)
					{
						linePen.StartCap = LineCap.Round;
					}
					if (linePen.EndCap != LineCap.Round)
					{
						linePen.EndCap = LineCap.Round;
					}
					if (tension == 0f)
					{
						try
						{
							graph.DrawLine(linePen, points[pointIndex - 1], points[pointIndex]);
						}
						catch (OverflowException)
						{
							DrawTruncatedLine(graph, linePen, points[pointIndex - 1], points[pointIndex]);
						}
					}
					else
					{
						graph.DrawCurve(linePen, points, pointIndex - 1, 1, tension);
					}
				}
			}
			if (!common.ProcessModeRegions)
			{
				return;
			}
			int num = borderWidth + 2;
			GraphicsPath graphicsPath = new GraphicsPath();
			if (lineTension == 0f)
			{
				if (pointIndex > 0)
				{
					PointF pointF = points[pointIndex - 1];
					PointF pointF2 = points[pointIndex];
					pointF.X = (pointF.X + pointF2.X) / 2f;
					pointF.Y = (pointF.Y + pointF2.Y) / 2f;
					if (Math.Abs(pointF.X - pointF2.X) > Math.Abs(pointF.Y - pointF2.Y))
					{
						graphicsPath.AddLine(pointF.X, pointF.Y - (float)num, pointF2.X, pointF2.Y - (float)num);
						graphicsPath.AddLine(pointF2.X, pointF2.Y + (float)num, pointF.X, pointF.Y + (float)num);
						graphicsPath.CloseAllFigures();
					}
					else
					{
						graphicsPath.AddLine(pointF.X - (float)num, pointF.Y, pointF2.X - (float)num, pointF2.Y);
						graphicsPath.AddLine(pointF2.X + (float)num, pointF2.Y, pointF.X + (float)num, pointF.Y);
						graphicsPath.CloseAllFigures();
					}
				}
				if (pointIndex + 1 < points.Length)
				{
					PointF pointF3 = points[pointIndex];
					PointF pointF4 = points[pointIndex + 1];
					pointF4.X = (pointF3.X + pointF4.X) / 2f;
					pointF4.Y = (pointF3.Y + pointF4.Y) / 2f;
					if (pointIndex > 0)
					{
						graphicsPath.SetMarkers();
					}
					if (Math.Abs(pointF3.X - pointF4.X) > Math.Abs(pointF3.Y - pointF4.Y))
					{
						graphicsPath.AddLine(pointF3.X, pointF3.Y - (float)num, pointF4.X, pointF4.Y - (float)num);
						graphicsPath.AddLine(pointF4.X, pointF4.Y + (float)num, pointF3.X, pointF3.Y + (float)num);
						graphicsPath.CloseAllFigures();
					}
					else
					{
						graphicsPath.AddLine(pointF3.X - (float)num, pointF3.Y, pointF4.X - (float)num, pointF4.Y);
						graphicsPath.AddLine(pointF4.X + (float)num, pointF4.Y, pointF3.X + (float)num, pointF3.Y);
						graphicsPath.CloseAllFigures();
					}
				}
			}
			else if (pointIndex > 0)
			{
				try
				{
					graphicsPath.AddCurve(points, pointIndex - 1, 1, lineTension);
					ChartGraphics.Widen(graphicsPath, new Pen(point.Color, borderWidth + 2));
					graphicsPath.Flatten();
				}
				catch
				{
				}
			}
			if (graphicsPath.PointCount != 0)
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
				common.HotRegionsList.AddHotRegion(graph, graphicsPath, relativePath: false, array, point, series.Name, pointIndex);
			}
		}

		private void DrawTruncatedLine(ChartGraphics graph, Pen pen, PointF pt1, PointF pt2)
		{
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			if (Math.Abs(pt2.Y - pt1.Y) > Math.Abs(pt2.X - pt1.X))
			{
				empty = GetIntersectionY(pt1, pt2, 0f);
				empty2 = GetIntersectionY(pt1, pt2, graph.common.ChartPicture.Height);
			}
			else
			{
				empty = GetIntersectionX(pt1, pt2, 0f);
				empty2 = GetIntersectionX(pt1, pt2, graph.common.ChartPicture.Width);
			}
			graph.DrawLine(pen, empty, empty2);
		}

		internal static PointF GetIntersectionY(PointF firstPoint, PointF secondPoint, float pointY)
		{
			PointF result = default(PointF);
			result.Y = pointY;
			result.X = (pointY - firstPoint.Y) * (secondPoint.X - firstPoint.X) / (secondPoint.Y - firstPoint.Y) + firstPoint.X;
			return result;
		}

		internal static PointF GetIntersectionX(PointF firstPoint, PointF secondPoint, float pointX)
		{
			PointF result = default(PointF);
			result.X = pointX;
			result.Y = (pointX - firstPoint.X) * (secondPoint.Y - firstPoint.Y) / (secondPoint.X - firstPoint.X) + firstPoint.Y;
			return result;
		}

		protected void DrawLine(ChartGraphics graph, DataPoint point, Series series, PointF firstPoint, PointF secondPoint)
		{
			graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, firstPoint, secondPoint, series.ShadowColor, series.ShadowOffset);
		}

		protected virtual bool IsLineTensionSupported()
		{
			return false;
		}

		private void Init2DAnimation(CommonElements common, DataPoint point, int index, double xValue, double xValuePrev, ChartGraphics graph, PointF[] dataPointPos)
		{
		}

		protected virtual float GetDefaultTension()
		{
			return 0f;
		}

		protected override LabelAlignmentTypes GetAutoLabelPosition(Series series, int pointIndex)
		{
			int count = series.Points.Count;
			if (count == 1)
			{
				return LabelAlignmentTypes.Top;
			}
			double yValue = GetYValue(common, area, series, series.Points[pointIndex], pointIndex, 0);
			if (pointIndex < count - 1 && pointIndex > 0)
			{
				double yValue2 = GetYValue(common, area, series, series.Points[pointIndex - 1], pointIndex - 1, 0);
				double yValue3 = GetYValue(common, area, series, series.Points[pointIndex + 1], pointIndex + 1, 0);
				if (yValue2 > yValue && yValue3 > yValue)
				{
					return LabelAlignmentTypes.Bottom;
				}
			}
			if (pointIndex == count - 1 && GetYValue(common, area, series, series.Points[pointIndex - 1], pointIndex - 1, 0) > yValue)
			{
				return LabelAlignmentTypes.Bottom;
			}
			if (pointIndex == 0)
			{
				double yValue3 = GetYValue(common, area, series, series.Points[pointIndex + 1], pointIndex + 1, 0);
				if (yValue3 > yValue)
				{
					return LabelAlignmentTypes.Bottom;
				}
			}
			return LabelAlignmentTypes.Top;
		}

		protected virtual PointF[] GetPointsPosition(ChartGraphics graph, Series series, bool indexedSeries)
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
				array[num] = new PointF((float)position2 * (float)(graph.common.ChartPicture.Width - 1) / 100f, (float)position * (float)(graph.common.ChartPicture.Height - 1) / 100f);
				num++;
			}
			return array;
		}

		protected void ProcessLineChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			graph.frontLinePen = null;
			graph.frontLinePoint1 = PointF.Empty;
			graph.frontLinePoint2 = PointF.Empty;
			ArrayList arrayList = null;
			if ((area.Area3DStyle.Clustered && SideBySideSeries) || Stacked)
			{
				arrayList = area.GetSeriesFromChartType(Name);
			}
			else
			{
				arrayList = new ArrayList();
				arrayList.Add(seriesToDraw.Name);
			}
			foreach (string item in arrayList)
			{
				Series series = common.DataManager.Series[item];
				if (series.XValueIndexed)
				{
					continue;
				}
				bool flag = true;
				int num = int.MaxValue;
				double num2 = double.NaN;
				foreach (DataPoint point in series.Points)
				{
					if (flag && point.XValue == 0.0)
					{
						continue;
					}
					flag = false;
					bool flag2 = true;
					if (!double.IsNaN(num2) && point.XValue != num2)
					{
						if (num == int.MaxValue)
						{
							num = ((!(point.XValue > num2)) ? 1 : 0);
						}
						if (point.XValue > num2 && num == 1)
						{
							flag2 = false;
						}
						if (point.XValue < num2 && num == 0)
						{
							flag2 = false;
						}
					}
					if (!flag2)
					{
						throw new InvalidOperationException(SR.Exception3DChartPointsXValuesUnsorted);
					}
					num2 = point.XValue;
				}
			}
			ArrayList dataPointDrawingOrder = area.GetDataPointDrawingOrder(arrayList, this, selection, COPCoordinatesToCheck, null, 0, sideBySide: false);
			lineTension = GetDefaultTension();
			if (dataPointDrawingOrder.Count > 0)
			{
				Series series2 = series2 = ((DataPoint3D)dataPointDrawingOrder[0]).dataPoint.series;
				if (IsLineTensionSupported() && series2.IsAttributeSet("LineTension"))
				{
					lineTension = CommonElements.ParseFloat(series2["LineTension"]);
				}
			}
			allPointsLoopsNumber = GetPointLoopNumber(selection, dataPointDrawingOrder);
			for (int i = 0; i < allPointsLoopsNumber; i++)
			{
				int num3 = 0;
				centerPointIndex = int.MaxValue;
				foreach (DataPoint3D item2 in dataPointDrawingOrder)
				{
					DataPoint dataPoint2 = item2.dataPoint;
					Series series3 = dataPoint2.series;
					hAxis = area.GetAxis(AxisName.X, series3.XAxisType, series3.XSubAxisName);
					vAxis = area.GetAxis(AxisName.Y, series3.YAxisType, series3.YSubAxisName);
					hAxisMin = hAxis.GetViewMinimum();
					hAxisMax = hAxis.GetViewMaximum();
					vAxisMin = vAxis.GetViewMinimum();
					vAxisMax = vAxis.GetViewMaximum();
					if (item2.index > 1)
					{
						int neighborPointIndex = num3;
						DataPoint3D dataPoint3D2 = ChartGraphics3D.FindPointByIndex(dataPointDrawingOrder, item2.index - 1, multiSeries ? item2 : null, ref neighborPointIndex);
						GraphicsPath graphicsPath = null;
						double yValue = GetYValue(common, area, series3, item2.dataPoint, item2.index - 1, 0);
						double yValue2 = GetYValue(common, area, series3, dataPoint3D2.dataPoint, dataPoint3D2.index - 1, 0);
						double yValue3 = item2.indexedSeries ? ((double)item2.index) : item2.dataPoint.XValue;
						double yValue4 = dataPoint3D2.indexedSeries ? ((double)dataPoint3D2.index) : dataPoint3D2.dataPoint.XValue;
						yValue = vAxis.GetLogValue(yValue);
						yValue2 = vAxis.GetLogValue(yValue2);
						yValue3 = hAxis.GetLogValue(yValue3);
						yValue4 = hAxis.GetLogValue(yValue4);
						DataPoint3D dataPoint3D3 = dataPoint3D2.dataPoint.Empty ? dataPoint3D2 : item2;
						if (dataPoint3D3.dataPoint.Color != Color.Empty)
						{
							DrawingOperationTypes drawingOperationTypes = DrawingOperationTypes.DrawElement;
							if (common.ProcessModeRegions)
							{
								drawingOperationTypes |= DrawingOperationTypes.CalcElementPath;
							}
							showPointLines = false;
							if (dataPoint3D3.dataPoint.IsAttributeSet("ShowMarkerLines"))
							{
								if (string.Compare(dataPoint3D3.dataPoint["ShowMarkerLines"], "TRUE", StringComparison.OrdinalIgnoreCase) == 0)
								{
									showPointLines = true;
								}
							}
							else if (dataPoint3D3.dataPoint.series.IsAttributeSet("ShowMarkerLines") && string.Compare(dataPoint3D3.dataPoint.series["ShowMarkerLines"], "TRUE", StringComparison.OrdinalIgnoreCase) == 0)
							{
								showPointLines = true;
							}
							graph.StartHotRegion(dataPoint2);
							Init3DAnimation(common, yValue4, yValue3, yValue2, yValue, vAxis, hAxis, dataPoint3D3, graph, dataPoint3D2.dataPoint, seriesToDraw);
							graph.StartAnimation();
							area.IterationCounter = 0;
							graphicsPath = Draw3DSurface(area, graph, area.matrix3D, area.Area3DStyle.Light, dataPoint3D2, dataPoint3D3.zPosition, dataPoint3D3.depth, dataPointDrawingOrder, num3, i, lineTension, drawingOperationTypes, 0f, 0f, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment: false);
							graph.StopAnimation();
							graph.EndHotRegion();
						}
						if (common.ProcessModeRegions && graphicsPath != null)
						{
							common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: false, graph, dataPoint2, series3.Name, item2.index - 1);
						}
					}
					num3++;
				}
			}
		}

		private void Init3DAnimation(CommonElements common, double xValuePrev, double xValue, double yValuePrev, double yValue, Axis vAxis, Axis hAxis, DataPoint3D pointAttr, ChartGraphics graph, DataPoint point, Series series)
		{
		}

		protected virtual GraphicsPath Draw3DSurface(ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment)
		{
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
			Color backColor = useBorderColor ? dataPoint3D3.dataPoint.BorderColor : dataPoint3D3.dataPoint.Color;
			ChartDashStyle borderStyle = dataPoint3D3.dataPoint.BorderStyle;
			if (dataPoint3D3.dataPoint.Empty && dataPoint3D3.dataPoint.Color == Color.Empty)
			{
				backColor = Color.Gray;
			}
			if (dataPoint3D3.dataPoint.Empty && dataPoint3D3.dataPoint.BorderStyle == ChartDashStyle.NotSet)
			{
				borderStyle = ChartDashStyle.Solid;
			}
			return graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, dataPoint3D2, dataPoint3D, points, pointIndex, tension, operationType, LineSegmentType.Single, showPointLines ? true : false, forceThickBorder: false, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
		}

		protected int GetCenterPointIndex(ArrayList points)
		{
			for (int i = 1; i < points.Count; i++)
			{
				DataPoint3D dataPoint3D = (DataPoint3D)points[i - 1];
				if (Math.Abs(((DataPoint3D)points[i]).index - dataPoint3D.index) != 1)
				{
					return i - 1;
				}
			}
			return int.MaxValue;
		}

		protected virtual int GetPointLoopNumber(bool selection, ArrayList pointsArray)
		{
			return 1;
		}

		protected bool ClipTopPoints(GraphicsPath resultPath, ref DataPoint3D firstPoint, ref DataPoint3D secondPoint, bool reversed, ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, LineSegmentType surfaceSegmentType, float topDarkening, float bottomDarkening)
		{
			area.IterationCounter++;
			if (area.IterationCounter > 20)
			{
				area.IterationCounter = 0;
				return true;
			}
			if (double.IsNaN(firstPoint.xPosition) || double.IsNaN(firstPoint.yPosition) || double.IsNaN(secondPoint.xPosition) || double.IsNaN(secondPoint.yPosition))
			{
				return true;
			}
			int num = 3;
			decimal num2 = Math.Round((decimal)area.PlotAreaPosition.X, num);
			decimal num3 = Math.Round((decimal)area.PlotAreaPosition.Y, num);
			decimal num4 = Math.Round((decimal)area.PlotAreaPosition.Right(), num);
			decimal num5 = Math.Round((decimal)area.PlotAreaPosition.Bottom(), num);
			num2 -= 0.001m;
			num3 -= 0.001m;
			num4 += 0.001m;
			num5 += 0.001m;
			firstPoint.xPosition = Math.Round(firstPoint.xPosition, num);
			firstPoint.yPosition = Math.Round(firstPoint.yPosition, num);
			secondPoint.xPosition = Math.Round(secondPoint.xPosition, num);
			secondPoint.yPosition = Math.Round(secondPoint.yPosition, num);
			if ((decimal)firstPoint.xPosition < num2 || (decimal)firstPoint.xPosition > num4 || (decimal)secondPoint.xPosition < num2 || (decimal)secondPoint.xPosition > num4)
			{
				if ((decimal)firstPoint.xPosition < num2 && (decimal)secondPoint.xPosition < num2)
				{
					return true;
				}
				if ((decimal)firstPoint.xPosition > num4 && (decimal)secondPoint.xPosition > num4)
				{
					return true;
				}
				if ((decimal)firstPoint.xPosition < num2)
				{
					firstPoint.yPosition = ((double)num2 - secondPoint.xPosition) / (firstPoint.xPosition - secondPoint.xPosition) * (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.yPosition;
					firstPoint.xPosition = (double)num2;
				}
				else if ((decimal)firstPoint.xPosition > num4)
				{
					firstPoint.yPosition = ((double)num4 - secondPoint.xPosition) / (firstPoint.xPosition - secondPoint.xPosition) * (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.yPosition;
					firstPoint.xPosition = (double)num4;
				}
				if ((decimal)secondPoint.xPosition < num2)
				{
					secondPoint.yPosition = ((double)num2 - secondPoint.xPosition) / (firstPoint.xPosition - secondPoint.xPosition) * (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.yPosition;
					secondPoint.xPosition = (double)num2;
				}
				else if ((decimal)secondPoint.xPosition > num4)
				{
					secondPoint.yPosition = ((double)num4 - secondPoint.xPosition) / (firstPoint.xPosition - secondPoint.xPosition) * (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.yPosition;
					secondPoint.xPosition = (double)num4;
				}
			}
			if ((decimal)firstPoint.yPosition < num3 || (decimal)firstPoint.yPosition > num5 || (decimal)secondPoint.yPosition < num3 || (decimal)secondPoint.yPosition > num5)
			{
				double yPosition = firstPoint.yPosition;
				double yPosition2 = secondPoint.yPosition;
				bool flag = false;
				bool clippedSegment = false;
				if ((decimal)firstPoint.yPosition < num3 && (decimal)secondPoint.yPosition < num3)
				{
					flag = true;
					firstPoint.yPosition = (double)num3;
					secondPoint.yPosition = (double)num3;
				}
				if ((decimal)firstPoint.yPosition > num5 && (decimal)secondPoint.yPosition > num5)
				{
					flag = true;
					clippedSegment = true;
					firstPoint.yPosition = (double)num5;
					secondPoint.yPosition = (double)num5;
				}
				if (flag)
				{
					resultPath = Draw3DSurface(firstPoint, secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, 0.5f, 0f, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment, clipOnTop: false, clipOnBottom: true);
					firstPoint.yPosition = yPosition;
					secondPoint.yPosition = yPosition2;
					return true;
				}
				DataPoint3D dataPoint3D = new DataPoint3D();
				dataPoint3D.yPosition = (double)num3;
				if ((decimal)firstPoint.yPosition > num5 || (decimal)secondPoint.yPosition > num5)
				{
					dataPoint3D.yPosition = (double)num5;
				}
				dataPoint3D.xPosition = (dataPoint3D.yPosition - secondPoint.yPosition) * (firstPoint.xPosition - secondPoint.xPosition) / (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.xPosition;
				if (double.IsNaN(dataPoint3D.xPosition) || double.IsInfinity(dataPoint3D.xPosition) || double.IsNaN(dataPoint3D.yPosition) || double.IsInfinity(dataPoint3D.yPosition))
				{
					return true;
				}
				int num6 = 2;
				DataPoint3D dataPoint3D2 = null;
				if (((decimal)firstPoint.yPosition < num3 && (decimal)secondPoint.yPosition > num5) || ((decimal)firstPoint.yPosition > num5 && (decimal)secondPoint.yPosition < num3))
				{
					num6 = 3;
					dataPoint3D2 = new DataPoint3D();
					if ((decimal)dataPoint3D.yPosition == num3)
					{
						dataPoint3D2.yPosition = (double)num5;
					}
					else
					{
						dataPoint3D2.yPosition = (double)num3;
					}
					dataPoint3D2.xPosition = (dataPoint3D2.yPosition - secondPoint.yPosition) * (firstPoint.xPosition - secondPoint.xPosition) / (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.xPosition;
					if (double.IsNaN(dataPoint3D2.xPosition) || double.IsInfinity(dataPoint3D2.xPosition) || double.IsNaN(dataPoint3D2.yPosition) || double.IsInfinity(dataPoint3D2.yPosition))
					{
						return true;
					}
					if ((decimal)firstPoint.yPosition > num5)
					{
						DataPoint3D dataPoint3D3 = new DataPoint3D();
						dataPoint3D3.xPosition = dataPoint3D.xPosition;
						dataPoint3D3.yPosition = dataPoint3D.yPosition;
						dataPoint3D.xPosition = dataPoint3D2.xPosition;
						dataPoint3D.yPosition = dataPoint3D2.yPosition;
						dataPoint3D2.xPosition = dataPoint3D3.xPosition;
						dataPoint3D2.yPosition = dataPoint3D3.yPosition;
					}
				}
				bool flag2 = true;
				bool clippedSegment2 = false;
				bool clippedSegment3 = false;
				if ((decimal)firstPoint.yPosition < num3)
				{
					flag2 = false;
					firstPoint.yPosition = (double)num3;
				}
				else if ((decimal)firstPoint.yPosition > num5)
				{
					clippedSegment2 = true;
					flag2 = false;
					firstPoint.yPosition = (double)num5;
				}
				if ((decimal)secondPoint.yPosition < num3)
				{
					secondPoint.yPosition = (double)num3;
				}
				else if ((decimal)secondPoint.yPosition > num5)
				{
					clippedSegment3 = true;
					secondPoint.yPosition = (double)num5;
				}
				for (int i = 0; i < 3; i++)
				{
					GraphicsPath graphicsPath = null;
					if ((i == 0 && !reversed) || (i == 2 && reversed))
					{
						if (dataPoint3D2 == null)
						{
							dataPoint3D2 = dataPoint3D;
						}
						dataPoint3D2.dataPoint = secondPoint.dataPoint;
						dataPoint3D2.index = secondPoint.index;
						dataPoint3D2.xCenterVal = secondPoint.xCenterVal;
						graphicsPath = Draw3DSurface(firstPoint, dataPoint3D2, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, (surfaceSegmentType != LineSegmentType.Middle) ? LineSegmentType.First : LineSegmentType.Middle, (flag2 && num6 != 3) ? 0f : 0.5f, 0f, new PointF(float.NaN, float.NaN), new PointF((float)dataPoint3D2.xPosition, float.NaN), clippedSegment2, clipOnTop: false, clipOnBottom: true);
					}
					if (i == 1 && dataPoint3D2 != null && num6 == 3)
					{
						dataPoint3D2.dataPoint = secondPoint.dataPoint;
						dataPoint3D2.index = secondPoint.index;
						dataPoint3D2.xCenterVal = secondPoint.xCenterVal;
						dataPoint3D.xCenterVal = firstPoint.xCenterVal;
						dataPoint3D.index = firstPoint.index;
						dataPoint3D.dataPoint = firstPoint.dataPoint;
						graphicsPath = Draw3DSurface(dataPoint3D, dataPoint3D2, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, LineSegmentType.Middle, topDarkening, bottomDarkening, new PointF((float)dataPoint3D.xPosition, float.NaN), new PointF((float)dataPoint3D2.xPosition, float.NaN), clippedSegment: false, clipOnTop: false, clipOnBottom: true);
					}
					if ((i == 2 && !reversed) || (i == 0 && reversed))
					{
						dataPoint3D.dataPoint = firstPoint.dataPoint;
						dataPoint3D.index = firstPoint.index;
						dataPoint3D.xCenterVal = firstPoint.xCenterVal;
						graphicsPath = Draw3DSurface(dataPoint3D, secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, (surfaceSegmentType == LineSegmentType.Middle) ? LineSegmentType.Middle : LineSegmentType.Last, (!flag2 && num6 != 3) ? 0f : 0.5f, 0f, new PointF((float)dataPoint3D.xPosition, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment3, clipOnTop: false, clipOnBottom: true);
					}
					if (resultPath != null && graphicsPath != null && graphicsPath.PointCount > 0)
					{
						resultPath.AddPath(graphicsPath, connect: true);
					}
				}
				firstPoint.yPosition = yPosition;
				secondPoint.yPosition = yPosition2;
				return true;
			}
			return false;
		}

		protected bool ClipBottomPoints(GraphicsPath resultPath, ref DataPoint3D firstPoint, ref DataPoint3D secondPoint, ref PointF thirdPoint, ref PointF fourthPoint, bool reversed, ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, LineSegmentType surfaceSegmentType, float topDarkening, float bottomDarkening)
		{
			area.IterationCounter++;
			if (area.IterationCounter > 20)
			{
				area.IterationCounter = 0;
				return true;
			}
			int num = 3;
			decimal num2 = Math.Round((decimal)area.PlotAreaPosition.X, num);
			decimal num3 = Math.Round((decimal)area.PlotAreaPosition.Y, num);
			decimal d = Math.Round((decimal)area.PlotAreaPosition.Right(), num);
			decimal d2 = Math.Round((decimal)area.PlotAreaPosition.Bottom(), num);
			num2 -= 0.001m;
			num3 -= 0.001m;
			_ = d + 0.001m;
			d2 += 0.001m;
			firstPoint.xPosition = Math.Round(firstPoint.xPosition, num);
			firstPoint.yPosition = Math.Round(firstPoint.yPosition, num);
			secondPoint.xPosition = Math.Round(secondPoint.xPosition, num);
			secondPoint.yPosition = Math.Round(secondPoint.yPosition, num);
			thirdPoint.X = (float)Math.Round(thirdPoint.X, num);
			thirdPoint.Y = (float)Math.Round(thirdPoint.Y, num);
			fourthPoint.X = (float)Math.Round(fourthPoint.X, num);
			fourthPoint.Y = (float)Math.Round(fourthPoint.Y, num);
			if ((decimal)thirdPoint.Y < num3 || (decimal)thirdPoint.Y > d2 || (decimal)fourthPoint.Y < num3 || (decimal)fourthPoint.Y > d2)
			{
				PointF pointF = new PointF(thirdPoint.X, thirdPoint.Y);
				PointF pointF2 = new PointF(fourthPoint.X, fourthPoint.Y);
				bool flag = false;
				bool clippedSegment = false;
				if ((decimal)thirdPoint.Y < num3 && (decimal)fourthPoint.Y < num3)
				{
					clippedSegment = true;
					flag = true;
					thirdPoint.Y = area.PlotAreaPosition.Y;
					fourthPoint.Y = area.PlotAreaPosition.Y;
				}
				if ((decimal)thirdPoint.Y > d2 && (decimal)fourthPoint.Y > d2)
				{
					flag = true;
					thirdPoint.Y = area.PlotAreaPosition.Bottom();
					fourthPoint.Y = area.PlotAreaPosition.Bottom();
				}
				if (flag)
				{
					resultPath = Draw3DSurface(firstPoint, secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, 0.5f, new PointF(thirdPoint.X, thirdPoint.Y), new PointF(fourthPoint.X, fourthPoint.Y), clippedSegment, clipOnTop: false, clipOnBottom: false);
					thirdPoint = new PointF(pointF.X, pointF.Y);
					fourthPoint = new PointF(pointF2.X, pointF2.Y);
					return true;
				}
				DataPoint3D dataPoint3D = new DataPoint3D();
				bool flag2 = false;
				dataPoint3D.yPosition = (double)num3;
				if ((decimal)thirdPoint.Y > d2 || (decimal)fourthPoint.Y > d2)
				{
					dataPoint3D.yPosition = area.PlotAreaPosition.Bottom();
					flag2 = true;
				}
				dataPoint3D.xPosition = (dataPoint3D.yPosition - (double)fourthPoint.Y) * (double)(thirdPoint.X - fourthPoint.X) / (double)(thirdPoint.Y - fourthPoint.Y) + (double)fourthPoint.X;
				dataPoint3D.yPosition = (dataPoint3D.xPosition - secondPoint.xPosition) / (firstPoint.xPosition - secondPoint.xPosition) * (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.yPosition;
				if (double.IsNaN(dataPoint3D.xPosition) || double.IsInfinity(dataPoint3D.xPosition) || double.IsNaN(dataPoint3D.yPosition) || double.IsInfinity(dataPoint3D.yPosition))
				{
					return true;
				}
				int num4 = 2;
				DataPoint3D dataPoint3D2 = null;
				bool flag3 = false;
				if (((decimal)thirdPoint.Y < num3 && (decimal)fourthPoint.Y > d2) || ((decimal)thirdPoint.Y > d2 && (decimal)fourthPoint.Y < num3))
				{
					num4 = 3;
					dataPoint3D2 = new DataPoint3D();
					if (!flag2)
					{
						dataPoint3D2.yPosition = area.PlotAreaPosition.Bottom();
					}
					else
					{
						dataPoint3D2.yPosition = area.PlotAreaPosition.Y;
					}
					dataPoint3D2.xPosition = (dataPoint3D2.yPosition - (double)fourthPoint.Y) * (double)(thirdPoint.X - fourthPoint.X) / (double)(thirdPoint.Y - fourthPoint.Y) + (double)fourthPoint.X;
					dataPoint3D2.yPosition = (dataPoint3D2.xPosition - secondPoint.xPosition) / (firstPoint.xPosition - secondPoint.xPosition) * (firstPoint.yPosition - secondPoint.yPosition) + secondPoint.yPosition;
					if (double.IsNaN(dataPoint3D2.xPosition) || double.IsInfinity(dataPoint3D2.xPosition) || double.IsNaN(dataPoint3D2.yPosition) || double.IsInfinity(dataPoint3D2.yPosition))
					{
						return true;
					}
					if ((decimal)thirdPoint.Y > d2)
					{
						flag3 = true;
					}
				}
				bool flag4 = true;
				float bottomDarkening2 = bottomDarkening;
				bool clippedSegment2 = false;
				bool flag5 = false;
				if ((decimal)thirdPoint.Y < num3)
				{
					clippedSegment2 = true;
					flag4 = false;
					thirdPoint.Y = area.PlotAreaPosition.Y;
					bottomDarkening2 = 0.5f;
				}
				else if ((decimal)thirdPoint.Y > d2)
				{
					flag4 = false;
					thirdPoint.Y = area.PlotAreaPosition.Bottom();
					if (firstPoint.yPosition >= (double)thirdPoint.Y)
					{
						bottomDarkening2 = 0.5f;
					}
				}
				if ((decimal)fourthPoint.Y < num3)
				{
					flag5 = true;
					fourthPoint.Y = area.PlotAreaPosition.Y;
					bottomDarkening2 = 0.5f;
				}
				else if ((decimal)fourthPoint.Y > d2)
				{
					fourthPoint.Y = area.PlotAreaPosition.Bottom();
					if ((double)fourthPoint.Y <= secondPoint.yPosition)
					{
						bottomDarkening2 = 0.5f;
					}
				}
				for (int i = 0; i < 3; i++)
				{
					GraphicsPath graphicsPath = null;
					if ((i == 0 && !reversed) || (i == 2 && reversed))
					{
						if (dataPoint3D2 == null)
						{
							dataPoint3D2 = dataPoint3D;
						}
						if (flag3)
						{
							DataPoint3D dataPoint3D3 = new DataPoint3D();
							dataPoint3D3.xPosition = dataPoint3D.xPosition;
							dataPoint3D3.yPosition = dataPoint3D.yPosition;
							dataPoint3D.xPosition = dataPoint3D2.xPosition;
							dataPoint3D.yPosition = dataPoint3D2.yPosition;
							dataPoint3D2.xPosition = dataPoint3D3.xPosition;
							dataPoint3D2.yPosition = dataPoint3D3.yPosition;
						}
						dataPoint3D2.dataPoint = secondPoint.dataPoint;
						dataPoint3D2.index = secondPoint.index;
						dataPoint3D2.xCenterVal = secondPoint.xCenterVal;
						graphicsPath = Draw3DSurface(firstPoint, dataPoint3D2, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, (surfaceSegmentType != LineSegmentType.Middle) ? LineSegmentType.First : LineSegmentType.Middle, topDarkening, bottomDarkening2, new PointF(float.NaN, thirdPoint.Y), new PointF((float)dataPoint3D2.xPosition, (!flag4 || num4 == 3) ? thirdPoint.Y : fourthPoint.Y), clippedSegment2, clipOnTop: false, clipOnBottom: false);
						if (flag3)
						{
							DataPoint3D dataPoint3D4 = new DataPoint3D();
							dataPoint3D4.xPosition = dataPoint3D.xPosition;
							dataPoint3D4.yPosition = dataPoint3D.yPosition;
							dataPoint3D.xPosition = dataPoint3D2.xPosition;
							dataPoint3D.yPosition = dataPoint3D2.yPosition;
							dataPoint3D2.xPosition = dataPoint3D4.xPosition;
							dataPoint3D2.yPosition = dataPoint3D4.yPosition;
						}
					}
					if (i == 1 && dataPoint3D2 != null && num4 == 3)
					{
						if (!flag3)
						{
							DataPoint3D dataPoint3D5 = new DataPoint3D();
							dataPoint3D5.xPosition = dataPoint3D.xPosition;
							dataPoint3D5.yPosition = dataPoint3D.yPosition;
							dataPoint3D.xPosition = dataPoint3D2.xPosition;
							dataPoint3D.yPosition = dataPoint3D2.yPosition;
							dataPoint3D2.xPosition = dataPoint3D5.xPosition;
							dataPoint3D2.yPosition = dataPoint3D5.yPosition;
						}
						dataPoint3D2.dataPoint = secondPoint.dataPoint;
						dataPoint3D2.index = secondPoint.index;
						dataPoint3D2.xCenterVal = secondPoint.xCenterVal;
						dataPoint3D.xCenterVal = firstPoint.xCenterVal;
						dataPoint3D.index = firstPoint.index;
						dataPoint3D.dataPoint = firstPoint.dataPoint;
						graphicsPath = Draw3DSurface(dataPoint3D, dataPoint3D2, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, LineSegmentType.Middle, topDarkening, bottomDarkening, new PointF((float)dataPoint3D.xPosition, thirdPoint.Y), new PointF((float)dataPoint3D2.xPosition, fourthPoint.Y), clippedSegment: false, clipOnTop: false, clipOnBottom: false);
						if (!flag3)
						{
							DataPoint3D dataPoint3D6 = new DataPoint3D();
							dataPoint3D6.xPosition = dataPoint3D.xPosition;
							dataPoint3D6.yPosition = dataPoint3D.yPosition;
							dataPoint3D.xPosition = dataPoint3D2.xPosition;
							dataPoint3D.yPosition = dataPoint3D2.yPosition;
							dataPoint3D2.xPosition = dataPoint3D6.xPosition;
							dataPoint3D2.yPosition = dataPoint3D6.yPosition;
						}
					}
					if ((i == 2 && !reversed) || (i == 0 && reversed))
					{
						if (flag3)
						{
							DataPoint3D dataPoint3D7 = new DataPoint3D();
							dataPoint3D7.xPosition = dataPoint3D.xPosition;
							dataPoint3D7.yPosition = dataPoint3D.yPosition;
							dataPoint3D.xPosition = dataPoint3D2.xPosition;
							dataPoint3D.yPosition = dataPoint3D2.yPosition;
							dataPoint3D2.xPosition = dataPoint3D7.xPosition;
							dataPoint3D2.yPosition = dataPoint3D7.yPosition;
						}
						dataPoint3D.dataPoint = firstPoint.dataPoint;
						dataPoint3D.index = firstPoint.index;
						dataPoint3D.xCenterVal = firstPoint.xCenterVal;
						float y = (!flag4 || num4 == 3) ? thirdPoint.Y : fourthPoint.Y;
						if (num4 == 3)
						{
							y = (flag5 ? thirdPoint.Y : fourthPoint.Y);
						}
						graphicsPath = Draw3DSurface(dataPoint3D, secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, (surfaceSegmentType == LineSegmentType.Middle) ? LineSegmentType.Middle : LineSegmentType.Last, topDarkening, bottomDarkening2, new PointF((float)dataPoint3D.xPosition, y), new PointF(float.NaN, fourthPoint.Y), flag5, clipOnTop: false, clipOnBottom: false);
						if (flag3)
						{
							DataPoint3D dataPoint3D8 = new DataPoint3D();
							dataPoint3D8.xPosition = dataPoint3D.xPosition;
							dataPoint3D8.yPosition = dataPoint3D.yPosition;
							dataPoint3D.xPosition = dataPoint3D2.xPosition;
							dataPoint3D.yPosition = dataPoint3D2.yPosition;
							dataPoint3D2.xPosition = dataPoint3D8.xPosition;
							dataPoint3D2.yPosition = dataPoint3D8.yPosition;
						}
					}
					if (resultPath != null && graphicsPath != null && graphicsPath.PointCount > 0)
					{
						resultPath.AddPath(graphicsPath, connect: true);
					}
				}
				thirdPoint = new PointF(pointF.X, pointF.Y);
				fourthPoint = new PointF(pointF2.X, pointF2.Y);
				return true;
			}
			return false;
		}

		protected virtual GraphicsPath Draw3DSurface(DataPoint3D firstPoint, DataPoint3D secondPoint, bool reversed, ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, LineSegmentType surfaceSegmentType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment, bool clipOnTop, bool clipOnBottom)
		{
			return null;
		}
	}
}
