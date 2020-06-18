using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class AreaChart : SplineChart
	{
		protected bool gradientFill;

		protected GraphicsPath areaPath;

		protected Series series;

		protected PointF axisPos = PointF.Empty;

		public override string Name => "Area";

		public override bool ZeroCrossing => true;

		public AreaChart()
		{
			drawOutsideLines = true;
			lineTension = 0f;
			axisPos = PointF.Empty;
		}

		protected override float GetDefaultTension()
		{
			return 0f;
		}

		public override LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		protected override void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			gradientFill = false;
			axisPos = PointF.Empty;
			base.ProcessChartType(selection, graph, common, area, seriesToDraw);
			if (!area.Area3DStyle.Enable3D)
			{
				FillLastSeriesGradient(graph);
			}
		}

		protected override void DrawLine(ChartGraphics graph, CommonElements common, DataPoint point, Series series, PointF[] points, int pointIndex, float tension)
		{
			if (pointIndex <= 0)
			{
				return;
			}
			if (this.series != null)
			{
				if (this.series.Name != series.Name)
				{
					FillLastSeriesGradient(graph);
					this.series = series;
				}
			}
			else
			{
				this.series = series;
			}
			PointF pointF = points[pointIndex - 1];
			PointF pointF2 = points[pointIndex];
			pointF.X = (float)Math.Round(pointF.X);
			pointF.Y = (float)Math.Round(pointF.Y);
			pointF2.X = (float)Math.Round(pointF2.X);
			pointF2.Y = (float)Math.Round(pointF2.Y);
			if (axisPos == PointF.Empty)
			{
				axisPos.X = (float)vAxis.GetPosition(vAxis.Crossing);
				axisPos.Y = (float)vAxis.GetPosition(vAxis.Crossing);
				axisPos = graph.GetAbsolutePoint(axisPos);
				axisPos.X = (float)Math.Round(axisPos.X);
				axisPos.Y = (float)Math.Round(axisPos.Y);
			}
			Color color = point.Color;
			Color borderColor = point.BorderColor;
			int borderWidth = point.BorderWidth;
			ChartDashStyle borderStyle = point.BorderStyle;
			Brush brush = null;
			if (point.BackHatchStyle != 0)
			{
				brush = graph.GetHatchBrush(point.BackHatchStyle, color, point.BackGradientEndColor);
			}
			else if (point.BackGradientType == GradientType.None)
			{
				brush = ((point.BackImage.Length <= 0 || point.BackImageMode == ChartImageWrapMode.Unscaled || point.BackImageMode == ChartImageWrapMode.Scaled) ? new SolidBrush(color) : graph.GetTextureBrush(point.BackImage, point.BackImageTransparentColor, point.BackImageMode, point.Color));
			}
			else
			{
				gradientFill = true;
				this.series = point.series;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddLine(pointF.X, axisPos.Y, pointF.X, pointF.Y);
			if (lineTension == 0f)
			{
				graphicsPath.AddLine(points[pointIndex - 1], points[pointIndex]);
			}
			else
			{
				graphicsPath.AddCurve(points, pointIndex - 1, 1, lineTension);
			}
			graphicsPath.AddLine(pointF2.X, pointF2.Y, pointF2.X, axisPos.Y);
			if (series.ShadowColor != Color.Empty && series.ShadowOffset != 0)
			{
				graph.shadowDrawingMode = true;
				if (color != Color.Empty && color != Color.Transparent)
				{
					Region region = new Region(graphicsPath);
					Brush brush2 = new SolidBrush((series.ShadowColor.A != byte.MaxValue) ? series.ShadowColor : Color.FromArgb((int)color.A / 2, series.ShadowColor));
					GraphicsState gstate = graph.Save();
					Region region2 = null;
					Region region3 = null;
					if (!graph.IsClipEmpty && !graph.Clip.IsInfinite(graph.Graphics))
					{
						region3 = graph.Clip.Clone();
						region2 = graph.Clip;
						region2.Translate(series.ShadowOffset, series.ShadowOffset);
						graph.Clip = region2;
					}
					graph.TranslateTransform(series.ShadowOffset, series.ShadowOffset);
					if (graph.SmoothingMode != SmoothingMode.None)
					{
						Pen pen = new Pen(brush2, 1f);
						if (lineTension == 0f)
						{
							graph.DrawLine(pen, points[pointIndex - 1], points[pointIndex]);
						}
						else
						{
							graph.DrawCurve(pen, points, pointIndex - 1, 1, lineTension);
						}
					}
					graph.FillRegion(brush2, region);
					graph.Restore(gstate);
					if (region2 != null && region3 != null)
					{
						graph.Clip = region3;
					}
				}
				graph.shadowDrawingMode = false;
			}
			if (!gradientFill)
			{
				SmoothingMode smoothingMode = graph.SmoothingMode;
				graph.SmoothingMode = SmoothingMode.None;
				graph.FillPath(brush, graphicsPath);
				graph.SmoothingMode = smoothingMode;
				if (graph.SmoothingMode != SmoothingMode.None)
				{
					graph.StartHotRegion(point);
					Pen pen2 = new Pen(brush, 1f);
					if (lineTension == 0f)
					{
						if (points[pointIndex - 1].X != points[pointIndex].X && points[pointIndex - 1].Y != points[pointIndex].Y)
						{
							graph.DrawLine(pen2, points[pointIndex - 1], points[pointIndex]);
						}
					}
					else
					{
						graph.DrawCurve(pen2, points, pointIndex - 1, 1, lineTension);
					}
					graph.EndHotRegion();
				}
			}
			if (areaPath == null)
			{
				areaPath = new GraphicsPath();
				areaPath.AddLine(pointF.X, axisPos.Y, pointF.X, pointF.Y);
			}
			if (lineTension == 0f)
			{
				areaPath.AddLine(points[pointIndex - 1], points[pointIndex]);
			}
			else
			{
				areaPath.AddCurve(points, pointIndex - 1, 1, lineTension);
			}
			if (borderWidth > 0 && borderColor != Color.Empty)
			{
				Pen pen3 = new Pen((borderColor != Color.Empty) ? borderColor : color, borderWidth);
				pen3.DashStyle = graph.GetPenStyle(borderStyle);
				pen3.StartCap = LineCap.Round;
				pen3.EndCap = LineCap.Round;
				if (lineTension == 0f)
				{
					graph.DrawLine(pen3, points[pointIndex - 1], points[pointIndex]);
				}
				else
				{
					graph.DrawCurve(pen3, points, pointIndex - 1, 1, lineTension);
				}
			}
			if (!common.ProcessModeRegions)
			{
				return;
			}
			GraphicsPath graphicsPath2 = new GraphicsPath();
			graphicsPath2.AddLine(pointF.X, axisPos.Y, pointF.X, pointF.Y);
			if (lineTension == 0f)
			{
				graphicsPath2.AddLine(points[pointIndex - 1], points[pointIndex]);
			}
			else
			{
				graphicsPath2.AddCurve(points, pointIndex - 1, 1, lineTension);
				graphicsPath2.Flatten();
			}
			graphicsPath2.AddLine(pointF2.X, pointF2.Y, pointF2.X, axisPos.Y);
			graphicsPath2.AddLine(pointF2.X, axisPos.Y, pointF.X, axisPos.Y);
			PointF empty = PointF.Empty;
			float[] array = new float[graphicsPath2.PointCount * 2];
			PointF[] pathPoints = graphicsPath2.PathPoints;
			for (int i = 0; i < graphicsPath2.PointCount; i++)
			{
				empty = graph.GetRelativePoint(pathPoints[i]);
				array[2 * i] = empty.X;
				array[2 * i + 1] = empty.Y;
			}
			common.HotRegionsList.AddHotRegion(graph, graphicsPath2, relativePath: false, array, point, series.Name, pointIndex);
			if (borderWidth <= 1 || borderStyle == ChartDashStyle.NotSet || !(borderColor != Color.Empty))
			{
				return;
			}
			try
			{
				graphicsPath2 = new GraphicsPath();
				if (lineTension == 0f)
				{
					graphicsPath2.AddLine(points[pointIndex - 1], points[pointIndex]);
				}
				else
				{
					graphicsPath2.AddCurve(points, pointIndex - 1, 1, lineTension);
					graphicsPath2.Flatten();
				}
				ChartGraphics.Widen(graphicsPath2, new Pen(color, borderWidth + 2));
			}
			catch (Exception)
			{
			}
			empty = PointF.Empty;
			array = new float[graphicsPath2.PointCount * 2];
			PointF[] pathPoints2 = graphicsPath2.PathPoints;
			for (int j = 0; j < pathPoints2.Length; j++)
			{
				empty = graph.GetRelativePoint(pathPoints2[j]);
				array[2 * j] = empty.X;
				array[2 * j + 1] = empty.Y;
			}
			common.HotRegionsList.AddHotRegion(graph, graphicsPath2, relativePath: false, array, point, series.Name, pointIndex);
		}

		private void FillLastSeriesGradient(ChartGraphics graph)
		{
			if (areaPath != null)
			{
				areaPath.AddLine(areaPath.GetLastPoint().X, areaPath.GetLastPoint().Y, areaPath.GetLastPoint().X, axisPos.Y);
			}
			if (gradientFill && areaPath != null)
			{
				graph.SetClip(area.PlotAreaPosition.ToRectangleF());
				Brush gradientBrush = graph.GetGradientBrush(areaPath.GetBounds(), series.Color, series.BackGradientEndColor, series.BackGradientType);
				graph.FillPath(gradientBrush, areaPath);
				gradientFill = false;
				graph.ResetClip();
			}
			if (areaPath != null)
			{
				areaPath.Dispose();
				areaPath = null;
			}
		}

		protected override bool IsLineTensionSupported()
		{
			return false;
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
			bool flag = false;
			if (dataPoint3D2.index > dataPoint3D.index)
			{
				DataPoint3D dataPoint3D3 = dataPoint3D2;
				dataPoint3D2 = dataPoint3D;
				dataPoint3D = dataPoint3D3;
				flag = true;
			}
			if (matrix.perspective != 0f && centerPointIndex != int.MaxValue)
			{
				neighborPointIndex = pointIndex;
				if (pointIndex != centerPointIndex + 1)
				{
					dataPoint3D2 = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, multiSeries ? dataPoint3D : null, ref neighborPointIndex);
				}
				else if (!area.reverseSeriesOrder)
				{
					dataPoint3D = ChartGraphics3D.FindPointByIndex(points, dataPoint3D2.index + 1, multiSeries ? dataPoint3D : null, ref neighborPointIndex);
				}
				else
				{
					dataPoint3D2 = dataPoint3D;
					dataPoint3D = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, multiSeries ? dataPoint3D : null, ref neighborPointIndex);
				}
			}
			if (dataPoint3D2 == null || dataPoint3D == null)
			{
				return graphicsPath;
			}
			DataPoint3D dataPoint3D4 = dataPoint3D;
			if (prevDataPointEx.dataPoint.Empty)
			{
				dataPoint3D4 = prevDataPointEx;
			}
			else if (dataPoint3D2.index > dataPoint3D.index)
			{
				dataPoint3D4 = dataPoint3D2;
			}
			if (!useBorderColor)
			{
				_ = dataPoint3D4.dataPoint.Color;
			}
			else
			{
				_ = dataPoint3D4.dataPoint.BorderColor;
			}
			_ = dataPoint3D4.dataPoint.BorderStyle;
			if (dataPoint3D4.dataPoint.Empty && dataPoint3D4.dataPoint.Color == Color.Empty)
			{
				_ = Color.Gray;
			}
			if (dataPoint3D4.dataPoint.Empty)
			{
				_ = dataPoint3D4.dataPoint.BorderStyle;
			}
			flag = false;
			for (int i = 1; pointIndex + i < points.Count; i++)
			{
				DataPoint3D dataPoint3D5 = (DataPoint3D)points[pointIndex + i];
				if (dataPoint3D5.dataPoint.series.Name == dataPoint3D2.dataPoint.series.Name)
				{
					if (dataPoint3D5.index == dataPoint3D2.index)
					{
						flag = true;
					}
					break;
				}
			}
			if (tension != 0f)
			{
				GraphicsPath splineFlattenPath = graph.GetSplineFlattenPath(area, matrix, positionZ, depth, dataPoint3D2, dataPoint3D, points, pointIndex, tension, flatten: true, translateCoordinates: false, 0);
				PointF[] array = null;
				flag = (pointIndex < neighborPointIndex);
				if (flag)
				{
					splineFlattenPath.Reverse();
				}
				array = splineFlattenPath.PathPoints;
				DataPoint3D dataPoint3D6 = new DataPoint3D();
				DataPoint3D dataPoint3D7 = new DataPoint3D();
				LineSegmentType lineSegmentType = LineSegmentType.Middle;
				for (int j = 1; j < array.Length; j++)
				{
					if (!flag)
					{
						dataPoint3D6.dataPoint = dataPoint3D2.dataPoint;
						dataPoint3D6.index = dataPoint3D2.index;
						dataPoint3D6.xPosition = array[j - 1].X;
						dataPoint3D6.yPosition = array[j - 1].Y;
						dataPoint3D7.dataPoint = dataPoint3D.dataPoint;
						dataPoint3D7.index = dataPoint3D.index;
						dataPoint3D7.xPosition = array[j].X;
						dataPoint3D7.yPosition = array[j].Y;
					}
					else
					{
						dataPoint3D7.dataPoint = dataPoint3D2.dataPoint;
						dataPoint3D7.index = dataPoint3D2.index;
						dataPoint3D7.xPosition = array[j - 1].X;
						dataPoint3D7.yPosition = array[j - 1].Y;
						dataPoint3D6.dataPoint = dataPoint3D.dataPoint;
						dataPoint3D6.index = dataPoint3D.index;
						dataPoint3D6.xPosition = array[j].X;
						dataPoint3D6.yPosition = array[j].Y;
					}
					lineSegmentType = LineSegmentType.Middle;
					if (j == 1)
					{
						lineSegmentType = ((!flag) ? LineSegmentType.First : LineSegmentType.Last);
					}
					else if (j == array.Length - 1)
					{
						lineSegmentType = (flag ? LineSegmentType.First : LineSegmentType.Last);
					}
					area.IterationCounter = 0;
					GraphicsPath graphicsPath2 = Draw3DSurface(dataPoint3D6, dataPoint3D7, flag, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, 0f, operationType, lineSegmentType, topDarkening, bottomDarkening, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment, clipOnTop: true, clipOnBottom: true);
					if (graphicsPath != null && graphicsPath2 != null && graphicsPath2.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath2, connect: true);
					}
				}
				return graphicsPath;
			}
			return Draw3DSurface(dataPoint3D2, dataPoint3D, flag, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, LineSegmentType.Single, topDarkening, bottomDarkening, thirdPointPosition, fourthPointPosition, clippedSegment, clipOnTop: true, clipOnBottom: true);
		}

		protected override GraphicsPath Draw3DSurface(DataPoint3D firstPoint, DataPoint3D secondPoint, bool reversed, ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, LineSegmentType surfaceSegmentType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment, bool clipOnTop, bool clipOnBottom)
		{
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			if (Math.Round(firstPoint.xPosition, 3) == Math.Round(secondPoint.xPosition, 3) && Math.Round(firstPoint.yPosition, 3) == Math.Round(secondPoint.yPosition, 3))
			{
				return graphicsPath;
			}
			DataPoint3D dataPoint3D = secondPoint;
			if (prevDataPointEx.dataPoint.Empty)
			{
				dataPoint3D = prevDataPointEx;
			}
			else if (firstPoint.index > secondPoint.index)
			{
				dataPoint3D = firstPoint;
			}
			Color color = useBorderColor ? dataPoint3D.dataPoint.BorderColor : dataPoint3D.dataPoint.Color;
			ChartDashStyle borderStyle = dataPoint3D.dataPoint.BorderStyle;
			if (dataPoint3D.dataPoint.Empty && dataPoint3D.dataPoint.Color == Color.Empty)
			{
				color = Color.Gray;
			}
			if (dataPoint3D.dataPoint.Empty && dataPoint3D.dataPoint.BorderStyle == ChartDashStyle.NotSet)
			{
				borderStyle = ChartDashStyle.Solid;
			}
			float num = (float)Math.Round(vAxis.GetPosition(vAxis.Crossing), 3);
			float num2 = (float)Math.Min(firstPoint.xPosition, secondPoint.xPosition);
			float val = (float)Math.Min(firstPoint.yPosition, secondPoint.yPosition);
			val = Math.Min(val, num);
			float num3 = (float)Math.Max(firstPoint.xPosition, secondPoint.xPosition);
			float val2 = (float)Math.Max(firstPoint.yPosition, secondPoint.yPosition);
			val2 = Math.Max(val2, num);
			RectangleF position = new RectangleF(num2, val, num3 - num2, val2 - val);
			SurfaceNames visibleSurfaces = graph.GetVisibleSurfaces(position, positionZ, depth, matrix);
			bool upSideDown = false;
			if ((decimal)firstPoint.yPosition >= (decimal)num && (decimal)secondPoint.yPosition >= (decimal)num)
			{
				upSideDown = true;
				bool num4 = (visibleSurfaces & SurfaceNames.Top) == SurfaceNames.Top;
				bool flag = (visibleSurfaces & SurfaceNames.Bottom) == SurfaceNames.Bottom;
				visibleSurfaces ^= SurfaceNames.Bottom;
				visibleSurfaces ^= SurfaceNames.Top;
				if (num4)
				{
					visibleSurfaces |= SurfaceNames.Bottom;
				}
				if (flag)
				{
					visibleSurfaces |= SurfaceNames.Top;
				}
			}
			GetTopSurfaceVisibility(area, firstPoint, secondPoint, upSideDown, positionZ, depth, matrix, ref visibleSurfaces);
			GetBottomPointsPosition(common, area, num, ref firstPoint, ref secondPoint, thirdPointPosition, fourthPointPosition, out PointF thirdPoint, out PointF fourthPoint);
			if (!float.IsNaN(thirdPointPosition.Y))
			{
				thirdPoint.Y = thirdPointPosition.Y;
			}
			if (!float.IsNaN(fourthPointPosition.Y))
			{
				fourthPoint.Y = fourthPointPosition.Y;
			}
			if (float.IsNaN(thirdPoint.X) || float.IsNaN(thirdPoint.Y) || float.IsNaN(fourthPoint.X) || float.IsNaN(fourthPoint.Y))
			{
				return graphicsPath;
			}
			if (clipOnTop && ClipTopPoints(graphicsPath, ref firstPoint, ref secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening))
			{
				return graphicsPath;
			}
			if (clipOnBottom && ClipBottomPoints(graphicsPath, ref firstPoint, ref secondPoint, ref thirdPoint, ref fourthPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening))
			{
				return graphicsPath;
			}
			if ((Math.Round((decimal)firstPoint.yPosition, 3) > (decimal)num + 0.001m && Math.Round((decimal)secondPoint.yPosition, 3) < (decimal)num - 0.001m) || (Math.Round((decimal)firstPoint.yPosition, 3) < (decimal)num - 0.001m && Math.Round((decimal)secondPoint.yPosition, 3) > (decimal)num + 0.001m))
			{
				DataPoint3D axisIntersection = GetAxisIntersection(firstPoint, secondPoint, num);
				for (int i = 0; i <= 1; i++)
				{
					GraphicsPath graphicsPath2 = null;
					if ((i == 0 && !reversed) || (i == 1 && reversed))
					{
						axisIntersection.dataPoint = secondPoint.dataPoint;
						axisIntersection.index = secondPoint.index;
						graphicsPath2 = Draw3DSurface(firstPoint, axisIntersection, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment, clipOnTop, clipOnBottom);
					}
					if ((i == 1 && !reversed) || (i == 0 && reversed))
					{
						axisIntersection.dataPoint = firstPoint.dataPoint;
						axisIntersection.index = firstPoint.index;
						graphicsPath2 = Draw3DSurface(axisIntersection, secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment, clipOnTop, clipOnBottom);
					}
					if (graphicsPath != null && graphicsPath2 != null && graphicsPath2.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath2, connect: true);
					}
				}
				return graphicsPath;
			}
			if (Math.Round(firstPoint.xPosition, 3) == Math.Round(secondPoint.xPosition, 3) && Math.Round(firstPoint.yPosition, 3) == Math.Round(secondPoint.yPosition, 3))
			{
				return graphicsPath;
			}
			for (int j = 1; j <= 2; j++)
			{
				SurfaceNames[] obj = new SurfaceNames[6]
				{
					SurfaceNames.Back,
					SurfaceNames.Bottom,
					SurfaceNames.Top,
					SurfaceNames.Left,
					SurfaceNames.Right,
					SurfaceNames.Front
				};
				LineSegmentType lineSegmentType = LineSegmentType.Middle;
				SurfaceNames[] array = obj;
				foreach (SurfaceNames surfaceNames in array)
				{
					if (ChartGraphics3D.ShouldDrawLineChartSurface(area, area.reverseSeriesOrder, surfaceNames, visibleSurfaces, color, points, firstPoint, secondPoint, multiSeries, reversed, ref lineSegmentType) != j || (allPointsLoopsNumber == 2 && (operationType & DrawingOperationTypes.DrawElement) == DrawingOperationTypes.DrawElement && ((pointLoopIndex == 0 && (surfaceNames == SurfaceNames.Front || (j == 2 && (surfaceNames == SurfaceNames.Left || surfaceNames == SurfaceNames.Right)))) || (pointLoopIndex == 1 && (surfaceNames == SurfaceNames.Back || surfaceNames != SurfaceNames.Front) && (j == 1 || (surfaceNames != SurfaceNames.Left && surfaceNames != SurfaceNames.Right))))))
					{
						continue;
					}
					Color color2 = color;
					Color color3 = dataPoint3D.dataPoint.BorderColor;
					if (j == 1)
					{
						if (color2.A == byte.MaxValue)
						{
							continue;
						}
						color2 = Color.Transparent;
						if (color3 == Color.Empty)
						{
							color3 = ChartGraphics.GetGradientColor(color, Color.Black, 0.2);
						}
					}
					bool flag2 = showPointLines;
					if (surfaceSegmentType == LineSegmentType.Middle)
					{
						flag2 = false;
					}
					if (clippedSegment && surfaceNames != SurfaceNames.Top && surfaceNames != SurfaceNames.Bottom)
					{
						continue;
					}
					GraphicsPath graphicsPath3 = null;
					switch (surfaceNames)
					{
					case SurfaceNames.Top:
					{
						Color backColor = (topDarkening == 0f) ? color2 : ChartGraphics.GetGradientColor(color2, Color.Black, topDarkening);
						Color borderColor = (topDarkening == 0f) ? color3 : ChartGraphics.GetGradientColor(color3, Color.Black, topDarkening);
						graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames, positionZ, depth, backColor, borderColor, dataPoint3D.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, points, pointIndex, 0f, operationType, surfaceSegmentType, flag2, forceThickBorder: false, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
						break;
					}
					case SurfaceNames.Bottom:
					{
						DataPoint3D dataPoint3D12 = new DataPoint3D();
						dataPoint3D12.index = firstPoint.index;
						dataPoint3D12.dataPoint = firstPoint.dataPoint;
						dataPoint3D12.xPosition = firstPoint.xPosition;
						dataPoint3D12.yPosition = thirdPoint.Y;
						DataPoint3D dataPoint3D13 = new DataPoint3D();
						dataPoint3D13.index = secondPoint.index;
						dataPoint3D13.dataPoint = secondPoint.dataPoint;
						dataPoint3D13.xPosition = secondPoint.xPosition;
						dataPoint3D13.yPosition = fourthPoint.Y;
						Color backColor2 = (bottomDarkening == 0f) ? color2 : ChartGraphics.GetGradientColor(color2, Color.Black, topDarkening);
						Color borderColor2 = (bottomDarkening == 0f) ? color3 : ChartGraphics.GetGradientColor(color3, Color.Black, topDarkening);
						graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames, positionZ, depth, backColor2, borderColor2, dataPoint3D.dataPoint.BorderWidth, borderStyle, dataPoint3D12, dataPoint3D13, points, pointIndex, 0f, operationType, surfaceSegmentType, flag2, forceThickBorder: false, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
						break;
					}
					case SurfaceNames.Left:
						if (surfaceSegmentType == LineSegmentType.Single || (!area.reverseSeriesOrder && surfaceSegmentType == LineSegmentType.First) || (area.reverseSeriesOrder && surfaceSegmentType == LineSegmentType.Last))
						{
							DataPoint3D dataPoint3D4 = (firstPoint.xPosition <= secondPoint.xPosition) ? firstPoint : secondPoint;
							DataPoint3D dataPoint3D5 = new DataPoint3D();
							dataPoint3D5.index = dataPoint3D4.index;
							dataPoint3D5.dataPoint = dataPoint3D4.dataPoint;
							dataPoint3D5.xPosition = dataPoint3D4.xPosition;
							dataPoint3D5.yPosition = ((firstPoint.xPosition <= secondPoint.xPosition) ? thirdPoint.Y : fourthPoint.Y);
							DataPoint3D dataPoint3D6 = new DataPoint3D();
							dataPoint3D6.index = dataPoint3D4.index;
							dataPoint3D6.dataPoint = dataPoint3D4.dataPoint;
							dataPoint3D6.xPosition = dataPoint3D4.xPosition;
							dataPoint3D6.yPosition = dataPoint3D4.yPosition;
							graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames, positionZ, depth, color2, color3, dataPoint3D.dataPoint.BorderWidth, borderStyle, dataPoint3D5, dataPoint3D6, points, pointIndex, 0f, operationType, LineSegmentType.Single, forceThinBorder: true, forceThickBorder: true, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
						}
						break;
					case SurfaceNames.Right:
						if (surfaceSegmentType == LineSegmentType.Single || (!area.reverseSeriesOrder && surfaceSegmentType == LineSegmentType.Last) || (area.reverseSeriesOrder && surfaceSegmentType == LineSegmentType.First))
						{
							DataPoint3D dataPoint3D9 = (secondPoint.xPosition >= firstPoint.xPosition) ? secondPoint : firstPoint;
							DataPoint3D dataPoint3D10 = new DataPoint3D();
							dataPoint3D10.index = dataPoint3D9.index;
							dataPoint3D10.dataPoint = dataPoint3D9.dataPoint;
							dataPoint3D10.xPosition = dataPoint3D9.xPosition;
							dataPoint3D10.yPosition = ((secondPoint.xPosition >= firstPoint.xPosition) ? fourthPoint.Y : thirdPoint.Y);
							DataPoint3D dataPoint3D11 = new DataPoint3D();
							dataPoint3D11.index = dataPoint3D9.index;
							dataPoint3D11.dataPoint = dataPoint3D9.dataPoint;
							dataPoint3D11.xPosition = dataPoint3D9.xPosition;
							dataPoint3D11.yPosition = dataPoint3D9.yPosition;
							graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames, positionZ, depth, color2, color3, dataPoint3D.dataPoint.BorderWidth, borderStyle, dataPoint3D10, dataPoint3D11, points, pointIndex, 0f, operationType, LineSegmentType.Single, forceThinBorder: true, forceThickBorder: true, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
						}
						break;
					case SurfaceNames.Back:
					{
						DataPoint3D dataPoint3D7 = new DataPoint3D();
						dataPoint3D7.index = firstPoint.index;
						dataPoint3D7.dataPoint = firstPoint.dataPoint;
						dataPoint3D7.xPosition = firstPoint.xPosition;
						dataPoint3D7.yPosition = thirdPoint.Y;
						DataPoint3D dataPoint3D8 = new DataPoint3D();
						dataPoint3D8.index = secondPoint.index;
						dataPoint3D8.dataPoint = secondPoint.dataPoint;
						dataPoint3D8.xPosition = secondPoint.xPosition;
						dataPoint3D8.yPosition = fourthPoint.Y;
						SurfaceNames thinBorders2 = (SurfaceNames)0;
						if (flag2)
						{
							switch (surfaceSegmentType)
							{
							case LineSegmentType.Single:
								thinBorders2 = (SurfaceNames.Left | SurfaceNames.Right);
								break;
							case LineSegmentType.First:
								thinBorders2 = SurfaceNames.Left;
								break;
							case LineSegmentType.Last:
								thinBorders2 = SurfaceNames.Right;
								break;
							}
						}
						graphicsPath3 = graph.Draw3DPolygon(area, matrix, lightStyle, surfaceNames, positionZ, color2, color3, dataPoint3D.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, dataPoint3D8, dataPoint3D7, points, pointIndex, 0f, operationType, lineSegmentType, thinBorders2);
						break;
					}
					case SurfaceNames.Front:
					{
						DataPoint3D dataPoint3D2 = new DataPoint3D();
						dataPoint3D2.index = firstPoint.index;
						dataPoint3D2.dataPoint = firstPoint.dataPoint;
						dataPoint3D2.xPosition = firstPoint.xPosition;
						dataPoint3D2.yPosition = thirdPoint.Y;
						DataPoint3D dataPoint3D3 = new DataPoint3D();
						dataPoint3D3.index = secondPoint.index;
						dataPoint3D3.dataPoint = secondPoint.dataPoint;
						dataPoint3D3.xPosition = secondPoint.xPosition;
						dataPoint3D3.yPosition = fourthPoint.Y;
						if (area.reverseSeriesOrder)
						{
							switch (lineSegmentType)
							{
							case LineSegmentType.First:
								lineSegmentType = LineSegmentType.Last;
								break;
							case LineSegmentType.Last:
								lineSegmentType = LineSegmentType.First;
								break;
							}
						}
						if (surfaceSegmentType != 0 && (surfaceSegmentType == LineSegmentType.Middle || (surfaceSegmentType == LineSegmentType.First && lineSegmentType != LineSegmentType.First) || (surfaceSegmentType == LineSegmentType.Last && lineSegmentType != LineSegmentType.Last)))
						{
							lineSegmentType = LineSegmentType.Middle;
						}
						SurfaceNames thinBorders = (SurfaceNames)0;
						if (flag2)
						{
							switch (surfaceSegmentType)
							{
							case LineSegmentType.Single:
								thinBorders = (SurfaceNames.Left | SurfaceNames.Right);
								break;
							case LineSegmentType.First:
								thinBorders = SurfaceNames.Left;
								break;
							case LineSegmentType.Last:
								thinBorders = SurfaceNames.Right;
								break;
							}
						}
						graphicsPath3 = graph.Draw3DPolygon(area, matrix, lightStyle, surfaceNames, positionZ + depth, color2, color3, dataPoint3D.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, dataPoint3D3, dataPoint3D2, points, pointIndex, 0f, operationType, lineSegmentType, thinBorders);
						break;
					}
					}
					if (j == 2 && graphicsPath != null && graphicsPath3 != null && graphicsPath3.PointCount > 0)
					{
						graphicsPath.CloseFigure();
						graphicsPath.SetMarkers();
						graphicsPath.AddPath(graphicsPath3, connect: true);
					}
				}
			}
			return graphicsPath;
		}

		protected virtual void GetTopSurfaceVisibility(ChartArea area, DataPoint3D firstPoint, DataPoint3D secondPoint, bool upSideDown, float positionZ, float depth, Matrix3D matrix, ref SurfaceNames visibleSurfaces)
		{
			if ((visibleSurfaces & SurfaceNames.Top) == SurfaceNames.Top)
			{
				visibleSurfaces ^= SurfaceNames.Top;
			}
			Point3D[] array = new Point3D[3];
			if (!area.reverseSeriesOrder)
			{
				if ((!upSideDown && firstPoint.xPosition <= secondPoint.xPosition) || (upSideDown && firstPoint.xPosition >= secondPoint.xPosition))
				{
					array[0] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ + depth);
					array[1] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ);
					array[2] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ);
				}
				else
				{
					array[0] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ + depth);
					array[1] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ);
					array[2] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ);
				}
			}
			else if ((!upSideDown && secondPoint.xPosition <= firstPoint.xPosition) || (upSideDown && secondPoint.xPosition >= firstPoint.xPosition))
			{
				array[0] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ + depth);
				array[1] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ);
				array[2] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ);
			}
			else
			{
				array[0] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ + depth);
				array[1] = new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ);
				array[2] = new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ);
			}
			matrix.TransformPoints(array);
			if (ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]))
			{
				visibleSurfaces |= SurfaceNames.Top;
			}
		}

		internal DataPoint3D GetAxisIntersection(DataPoint3D firstPoint, DataPoint3D secondPoint, float axisPosition)
		{
			return new DataPoint3D
			{
				yPosition = axisPosition,
				xPosition = ((double)axisPosition - firstPoint.yPosition) * (secondPoint.xPosition - firstPoint.xPosition) / (secondPoint.yPosition - firstPoint.yPosition) + firstPoint.xPosition
			};
		}

		protected virtual void GetBottomPointsPosition(CommonElements common, ChartArea area, float axisPosition, ref DataPoint3D firstPoint, ref DataPoint3D secondPoint, PointF thirdPointPosition, PointF fourthPointPosition, out PointF thirdPoint, out PointF fourthPoint)
		{
			thirdPoint = new PointF((float)firstPoint.xPosition, axisPosition);
			fourthPoint = new PointF((float)secondPoint.xPosition, axisPosition);
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
			}
			return result;
		}
	}
}
