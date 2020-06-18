using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class RangeChart : SplineChart
	{
		protected bool gradientFill;

		protected GraphicsPath areaBottomPath = new GraphicsPath();

		protected GraphicsPath areaPath;

		protected Series series;

		protected PointF[] lowPoints;

		protected bool indexedBasedX;

		private float thirdPointY2Value = float.NaN;

		private float fourthPointY2Value = float.NaN;

		public override string Name => "Range";

		public override int YValuesPerPoint => 2;

		public override bool ExtraYValuesConnectedToYAxis => true;

		public RangeChart()
		{
			drawOutsideLines = true;
		}

		public override LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		protected override float GetDefaultTension()
		{
			return 0f;
		}

		protected override bool IsLineTensionSupported()
		{
			return false;
		}

		private void FillLastSeriesGradient(ChartGraphics graph)
		{
			if (areaPath != null)
			{
				areaPath.AddLine(areaPath.GetLastPoint().X, areaPath.GetLastPoint().Y, areaPath.GetLastPoint().X, areaBottomPath.GetLastPoint().Y);
			}
			if (gradientFill && areaPath != null)
			{
				graph.SetClip(area.PlotAreaPosition.ToRectangleF());
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddPath(areaPath, connect: true);
				areaBottomPath.Reverse();
				graphicsPath.AddPath(areaBottomPath, connect: true);
				Brush gradientBrush = graph.GetGradientBrush(graphicsPath.GetBounds(), series.Color, series.BackGradientEndColor, series.BackGradientType);
				graph.FillPath(gradientBrush, graphicsPath);
				gradientFill = false;
				graph.ResetClip();
			}
			if (areaPath != null)
			{
				areaPath.Dispose();
				areaPath = null;
			}
			areaBottomPath.Reset();
		}

		protected override void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			gradientFill = false;
			lowPoints = null;
			indexedBasedX = area.IndexedSeries((string[])area.GetSeriesFromChartType(Name).ToArray(typeof(string)));
			base.ProcessChartType(selection, graph, common, area, seriesToDraw);
			FillLastSeriesGradient(graph);
		}

		protected override void DrawLine(ChartGraphics graph, CommonElements common, DataPoint point, Series series, PointF[] points, int pointIndex, float tension)
		{
			if (point.YValues.Length < 2)
			{
				throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(Name, "2"));
			}
			if (pointIndex <= 0 || yValueIndex == 1)
			{
				return;
			}
			if (this.series != null)
			{
				if (this.series.Name != series.Name)
				{
					FillLastSeriesGradient(graph);
					this.series = series;
					lowPoints = null;
					areaBottomPath.Reset();
				}
			}
			else
			{
				this.series = series;
			}
			if (lowPoints == null)
			{
				yValueIndex = 1;
				lowPoints = GetPointsPosition(graph, series, indexedBasedX);
				yValueIndex = 0;
			}
			PointF pointF = points[pointIndex - 1];
			PointF pointF2 = points[pointIndex];
			PointF pointF3 = lowPoints[pointIndex - 1];
			PointF pointF4 = lowPoints[pointIndex];
			Brush brush = null;
			if (point.BackHatchStyle != 0)
			{
				brush = graph.GetHatchBrush(point.BackHatchStyle, point.Color, point.BackGradientEndColor);
			}
			else if (point.BackGradientType == GradientType.None)
			{
				brush = ((point.BackImage.Length <= 0 || point.BackImageMode == ChartImageWrapMode.Unscaled || point.BackImageMode == ChartImageWrapMode.Scaled) ? new SolidBrush(point.Color) : graph.GetTextureBrush(point.BackImage, point.BackImageTransparentColor, point.BackImageMode, point.Color));
			}
			else
			{
				gradientFill = true;
				this.series = point.series;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddLine(pointF.X, pointF3.Y, pointF.X, pointF.Y);
			if (lineTension == 0f)
			{
				graphicsPath.AddLine(points[pointIndex - 1], points[pointIndex]);
			}
			else
			{
				graphicsPath.AddCurve(points, pointIndex - 1, 1, lineTension);
			}
			graphicsPath.AddLine(pointF2.X, pointF2.Y, pointF2.X, pointF4.Y);
			if (graph.ActiveRenderingType == RenderingType.Svg)
			{
				GraphicsPath graphicsPath2 = new GraphicsPath();
				if (lineTension == 0f)
				{
					graphicsPath.AddLine(lowPoints[pointIndex - 1], lowPoints[pointIndex]);
				}
				else
				{
					graphicsPath2.AddCurve(lowPoints, pointIndex - 1, 1, lineTension);
					graphicsPath2.Flatten();
					PointF[] pathPoints = graphicsPath2.PathPoints;
					PointF[] array = new PointF[pathPoints.Length];
					int num = pathPoints.Length - 1;
					PointF[] array2 = pathPoints;
					for (int i = 0; i < array2.Length; i++)
					{
						PointF pointF5 = array[num] = array2[i];
						num--;
					}
					if (array.Length == 2)
					{
						array = new PointF[3]
						{
							array[0],
							array[1],
							array[1]
						};
					}
					graphicsPath.AddPolygon(array);
				}
			}
			else if (lineTension == 0f)
			{
				graphicsPath.AddLine(lowPoints[pointIndex - 1], lowPoints[pointIndex]);
			}
			else
			{
				graphicsPath.AddCurve(lowPoints, pointIndex - 1, 1, lineTension);
			}
			if (!clipRegionSet)
			{
				double num2 = indexedSeries ? ((double)(pointIndex + 1)) : series.Points[pointIndex].XValue;
				double num3 = indexedSeries ? ((double)pointIndex) : series.Points[pointIndex - 1].XValue;
				if (num3 < hAxisMin || num3 > hAxisMax || num2 > hAxisMax || num2 < hAxisMin || series.Points[pointIndex - 1].YValues[1] < vAxisMin || series.Points[pointIndex - 1].YValues[1] > vAxisMax || series.Points[pointIndex].YValues[1] < vAxisMin || series.Points[pointIndex].YValues[1] > vAxisMax)
				{
					graph.SetClip(area.PlotAreaPosition.ToRectangleF());
					clipRegionSet = true;
				}
			}
			if (series.ShadowColor != Color.Empty && series.ShadowOffset != 0 && point.Color != Color.Empty && point.Color != Color.Transparent)
			{
				Matrix matrix = graph.Transform.Clone();
				matrix.Translate(series.ShadowOffset, series.ShadowOffset);
				Matrix transform = graph.Transform;
				graph.Transform = matrix;
				Region region = new Region(graphicsPath);
				Brush brush2 = new SolidBrush((series.ShadowColor.A != byte.MaxValue) ? series.ShadowColor : Color.FromArgb((int)point.Color.A / 2, series.ShadowColor));
				Region region2 = null;
				if (!graph.IsClipEmpty && !graph.Clip.IsInfinite(graph.Graphics))
				{
					region2 = graph.Clip;
					region2.Translate(series.ShadowOffset + 1, series.ShadowOffset + 1);
					graph.Clip = region2;
				}
				graph.FillRegion(brush2, region);
				Pen pen = new Pen(brush2, 1f);
				if (pointIndex == 0)
				{
					graph.DrawLine(pen, pointF.X, pointF3.Y, pointF.X, pointF.Y);
				}
				if (pointIndex == series.Points.Count - 1)
				{
					graph.DrawLine(pen, pointF2.X, pointF2.Y, pointF2.X, pointF4.Y);
				}
				graph.Transform = transform;
				graph.shadowDrawingMode = true;
				drawShadowOnly = true;
				base.DrawLine(graph, common, point, series, points, pointIndex, tension);
				yValueIndex = 1;
				base.DrawLine(graph, common, point, series, lowPoints, pointIndex, tension);
				yValueIndex = 0;
				drawShadowOnly = false;
				graph.shadowDrawingMode = false;
				if (region2 != null)
				{
					region2 = graph.Clip;
					region2.Translate(-(series.ShadowOffset + 1), -(series.ShadowOffset + 1));
					graph.Clip = region2;
				}
			}
			if (!gradientFill)
			{
				SmoothingMode smoothingMode = graph.SmoothingMode;
				graph.SmoothingMode = SmoothingMode.None;
				graphicsPath.CloseAllFigures();
				graph.FillPath(brush, graphicsPath);
				graph.SmoothingMode = smoothingMode;
				if (graph.SmoothingMode != SmoothingMode.None)
				{
					Pen pen2 = new Pen(brush, 1f);
					if (brush is HatchBrush)
					{
						pen2.Color = ((HatchBrush)brush).ForegroundColor;
					}
					if (pointIndex == 0)
					{
						graph.DrawLine(pen2, pointF.X, pointF3.Y, pointF.X, pointF.Y);
					}
					if (pointIndex == series.Points.Count - 1)
					{
						graph.DrawLine(pen2, pointF2.X, pointF2.Y, pointF2.X, pointF4.Y);
					}
					if (lineTension == 0f)
					{
						graph.DrawLine(pen2, points[pointIndex - 1], points[pointIndex]);
					}
					else
					{
						graph.DrawCurve(pen2, points, pointIndex - 1, 1, lineTension);
					}
					if (lineTension == 0f)
					{
						graph.DrawLine(pen2, lowPoints[pointIndex - 1], lowPoints[pointIndex]);
					}
					else
					{
						graph.DrawCurve(pen2, lowPoints, pointIndex - 1, 1, lineTension);
					}
				}
			}
			if (areaPath == null)
			{
				areaPath = new GraphicsPath();
				areaPath.AddLine(pointF.X, pointF3.Y, pointF.X, pointF.Y);
			}
			if (lineTension == 0f)
			{
				areaPath.AddLine(points[pointIndex - 1], points[pointIndex]);
			}
			else
			{
				areaPath.AddCurve(points, pointIndex - 1, 1, lineTension);
			}
			if (lineTension == 0f)
			{
				areaBottomPath.AddLine(lowPoints[pointIndex - 1], lowPoints[pointIndex]);
			}
			else
			{
				areaBottomPath.AddCurve(lowPoints, pointIndex - 1, 1, lineTension);
			}
			if ((point.BorderWidth > 0 && point.BorderStyle != 0 && point.BorderColor != Color.Empty) || brush is SolidBrush)
			{
				useBorderColor = true;
				disableShadow = true;
				base.DrawLine(graph, common, point, series, points, pointIndex, tension);
				yValueIndex = 1;
				base.DrawLine(graph, common, point, series, lowPoints, pointIndex, tension);
				yValueIndex = 0;
				useBorderColor = false;
				disableShadow = false;
			}
			if (common.ProcessModeRegions)
			{
				graphicsPath.AddLine(pointF.X, pointF3.Y, pointF.X, pointF.Y);
				if (lineTension == 0f)
				{
					graphicsPath.AddLine(points[pointIndex - 1], points[pointIndex]);
				}
				else
				{
					graphicsPath.AddCurve(points, pointIndex - 1, 1, lineTension);
				}
				graphicsPath.AddLine(pointF2.X, pointF2.Y, pointF2.X, pointF4.Y);
				if (lineTension == 0f)
				{
					graphicsPath.AddLine(lowPoints[pointIndex - 1], lowPoints[pointIndex]);
				}
				else
				{
					graphicsPath.AddCurve(lowPoints, pointIndex - 1, 1, lineTension);
				}
				GraphicsPath graphicsPath3 = new GraphicsPath();
				graphicsPath3.AddLine(pointF.X, pointF3.Y, pointF.X, pointF.Y);
				if (lineTension == 0f)
				{
					graphicsPath3.AddLine(points[pointIndex - 1], points[pointIndex]);
				}
				else
				{
					graphicsPath3.AddCurve(points, pointIndex - 1, 1, lineTension);
					graphicsPath3.Flatten();
				}
				graphicsPath3.AddLine(pointF2.X, pointF2.Y, pointF2.X, pointF4.Y);
				if (lineTension == 0f)
				{
					graphicsPath3.AddLine(lowPoints[pointIndex - 1], lowPoints[pointIndex]);
				}
				else
				{
					graphicsPath3.AddCurve(lowPoints, pointIndex - 1, 1, lineTension);
					graphicsPath3.Flatten();
				}
				PointF empty = PointF.Empty;
				float[] array3 = new float[graphicsPath3.PointCount * 2];
				PointF[] pathPoints2 = graphicsPath3.PathPoints;
				for (int j = 0; j < graphicsPath3.PointCount; j++)
				{
					empty = graph.GetRelativePoint(pathPoints2[j]);
					array3[2 * j] = empty.X;
					array3[2 * j + 1] = empty.Y;
				}
				common.HotRegionsList.AddHotRegion(graph, graphicsPath3, relativePath: false, array3, point, series.Name, pointIndex);
			}
		}

		protected override GraphicsPath Draw3DSurface(ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment)
		{
			GraphicsPath result = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			if (centerPointIndex == int.MaxValue)
			{
				centerPointIndex = GetCenterPointIndex(points);
			}
			DataPoint3D dataPoint3D = (DataPoint3D)points[pointIndex];
			int neighborPointIndex = pointIndex;
			DataPoint3D dataPoint3D2 = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, multiSeries ? dataPoint3D : null, ref neighborPointIndex);
			bool reversed = false;
			if (dataPoint3D2.index > dataPoint3D.index)
			{
				DataPoint3D dataPoint3D3 = dataPoint3D2;
				dataPoint3D2 = dataPoint3D;
				dataPoint3D = dataPoint3D3;
				reversed = true;
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
				return result;
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
			return Draw3DSurface(dataPoint3D2, dataPoint3D, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, LineSegmentType.Single, topDarkening, bottomDarkening, thirdPointPosition, fourthPointPosition, clippedSegment, clipOnTop: true, clipOnBottom: true);
		}

		protected override GraphicsPath Draw3DSurface(DataPoint3D firstPoint, DataPoint3D secondPoint, bool reversed, ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, LineSegmentType surfaceSegmentType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment, bool clipOnTop, bool clipOnBottom)
		{
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
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
			float num = (float)vAxis.GetPosition(vAxis.Crossing);
			GetBottomPointsPosition(common, area, num, ref firstPoint, ref secondPoint, out PointF thirdPoint, out PointF fourthPoint);
			if (!float.IsNaN(thirdPointPosition.Y))
			{
				thirdPoint.Y = thirdPointPosition.Y;
			}
			if (!float.IsNaN(fourthPointPosition.Y))
			{
				fourthPoint.Y = fourthPointPosition.Y;
			}
			if ((firstPoint.yPosition > (double)thirdPoint.Y && secondPoint.yPosition < (double)fourthPoint.Y) || (firstPoint.yPosition < (double)thirdPoint.Y && secondPoint.yPosition > (double)fourthPoint.Y))
			{
				if (tension != 0f)
				{
					throw new InvalidOperationException(SR.Exception3DSplineY1ValueIsLessThenY2);
				}
				PointF linesIntersection = ChartGraphics3D.GetLinesIntersection((float)firstPoint.xPosition, (float)firstPoint.yPosition, (float)secondPoint.xPosition, (float)secondPoint.yPosition, thirdPoint.X, thirdPoint.Y, fourthPoint.X, fourthPoint.Y);
				DataPoint3D dataPoint3D2 = new DataPoint3D();
				dataPoint3D2.xPosition = linesIntersection.X;
				dataPoint3D2.yPosition = linesIntersection.Y;
				bool flag = true;
				if (double.IsNaN(linesIntersection.X) || double.IsNaN(linesIntersection.Y))
				{
					flag = false;
				}
				else
				{
					if ((decimal)linesIntersection.X == (decimal)firstPoint.xPosition && (decimal)linesIntersection.Y == (decimal)firstPoint.yPosition)
					{
						flag = false;
					}
					if ((decimal)linesIntersection.X == (decimal)secondPoint.xPosition && (decimal)linesIntersection.Y == (decimal)secondPoint.yPosition)
					{
						flag = false;
					}
				}
				if (flag)
				{
					reversed = false;
					if (pointIndex + 1 < points.Count && ((DataPoint3D)points[pointIndex + 1]).index == firstPoint.index)
					{
						reversed = true;
					}
					for (int i = 0; i <= 1; i++)
					{
						GraphicsPath graphicsPath2 = null;
						if ((i == 0 && !reversed) || (i == 1 && reversed))
						{
							fourthPointY2Value = (float)dataPoint3D2.yPosition;
							dataPoint3D2.dataPoint = secondPoint.dataPoint;
							dataPoint3D2.index = secondPoint.index;
							graphicsPath2 = Draw3DSurface(firstPoint, dataPoint3D2, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment, clipOnTop: true, clipOnBottom: true);
						}
						if ((i == 1 && !reversed) || (i == 0 && reversed))
						{
							thirdPointY2Value = (float)dataPoint3D2.yPosition;
							dataPoint3D2.dataPoint = firstPoint.dataPoint;
							dataPoint3D2.index = firstPoint.index;
							graphicsPath2 = Draw3DSurface(dataPoint3D2, secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening, new PointF(float.NaN, float.NaN), new PointF(float.NaN, float.NaN), clippedSegment, clipOnTop: true, clipOnBottom: true);
						}
						if (graphicsPath != null && graphicsPath2 != null && graphicsPath2.PointCount > 0)
						{
							graphicsPath.AddPath(graphicsPath2, connect: true);
						}
						thirdPointY2Value = float.NaN;
						fourthPointY2Value = float.NaN;
					}
					return graphicsPath;
				}
			}
			float num2 = (float)Math.Min(firstPoint.xPosition, secondPoint.xPosition);
			float val = (float)Math.Min(firstPoint.yPosition, secondPoint.yPosition);
			val = Math.Min(val, num);
			float num3 = (float)Math.Max(firstPoint.xPosition, secondPoint.xPosition);
			float val2 = (float)Math.Max(firstPoint.yPosition, secondPoint.yPosition);
			val2 = Math.Max(val2, num);
			RectangleF position = new RectangleF(num2, val, num3 - num2, val2 - val);
			SurfaceNames visibleSurfaces = graph.GetVisibleSurfaces(position, positionZ, depth, matrix);
			bool upSideDown = false;
			if (firstPoint.yPosition >= (double)thirdPoint.Y && secondPoint.yPosition >= (double)fourthPoint.Y)
			{
				upSideDown = true;
				bool num4 = (visibleSurfaces & SurfaceNames.Top) == SurfaceNames.Top;
				bool flag2 = (visibleSurfaces & SurfaceNames.Bottom) == SurfaceNames.Bottom;
				visibleSurfaces ^= SurfaceNames.Bottom;
				visibleSurfaces ^= SurfaceNames.Top;
				if (num4)
				{
					visibleSurfaces |= SurfaceNames.Bottom;
				}
				if (flag2)
				{
					visibleSurfaces |= SurfaceNames.Top;
				}
			}
			GetTopSurfaceVisibility(area, firstPoint, secondPoint, upSideDown, positionZ, depth, matrix, ref visibleSurfaces);
			bool flag3 = true;
			if (tension != 0f)
			{
				if ((visibleSurfaces & SurfaceNames.Bottom) == SurfaceNames.Bottom)
				{
					flag3 = false;
				}
				if ((visibleSurfaces & SurfaceNames.Bottom) == 0 && (visibleSurfaces & SurfaceNames.Top) == 0)
				{
					flag3 = false;
				}
				visibleSurfaces |= SurfaceNames.Bottom;
				visibleSurfaces |= SurfaceNames.Top;
			}
			firstPoint.xPosition = Math.Round(firstPoint.xPosition, 5);
			firstPoint.yPosition = Math.Round(firstPoint.yPosition, 5);
			secondPoint.xPosition = Math.Round(secondPoint.xPosition, 5);
			secondPoint.yPosition = Math.Round(secondPoint.yPosition, 5);
			if (ClipTopPoints(graphicsPath, ref firstPoint, ref secondPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening))
			{
				return graphicsPath;
			}
			if (ClipBottomPoints(graphicsPath, ref firstPoint, ref secondPoint, ref thirdPoint, ref fourthPoint, reversed, area, graph, matrix, lightStyle, prevDataPointEx, positionZ, depth, points, pointIndex, pointLoopIndex, tension, operationType, surfaceSegmentType, topDarkening, bottomDarkening))
			{
				return graphicsPath;
			}
			for (int j = 1; j <= 2; j++)
			{
				SurfaceNames[] array = null;
				array = ((!flag3) ? new SurfaceNames[6]
				{
					SurfaceNames.Back,
					SurfaceNames.Top,
					SurfaceNames.Bottom,
					SurfaceNames.Left,
					SurfaceNames.Right,
					SurfaceNames.Front
				} : new SurfaceNames[6]
				{
					SurfaceNames.Back,
					SurfaceNames.Bottom,
					SurfaceNames.Top,
					SurfaceNames.Left,
					SurfaceNames.Right,
					SurfaceNames.Front
				});
				LineSegmentType lineSegmentType = LineSegmentType.Middle;
				SurfaceNames[] array2 = array;
				foreach (SurfaceNames surfaceNames in array2)
				{
					if (ChartGraphics3D.ShouldDrawLineChartSurface(area, area.reverseSeriesOrder, surfaceNames, visibleSurfaces, color, points, firstPoint, secondPoint, multiSeries, reversed, ref lineSegmentType) != j)
					{
						continue;
					}
					Color backColor = color;
					Color color2 = dataPoint3D.dataPoint.BorderColor;
					if (j == 1)
					{
						if (backColor.A == byte.MaxValue)
						{
							continue;
						}
						backColor = Color.Transparent;
						if (color2 == Color.Empty)
						{
							color2 = ChartGraphics.GetGradientColor(color, Color.Black, 0.2);
						}
					}
					GraphicsPath graphicsPath3 = null;
					switch (surfaceNames)
					{
					case SurfaceNames.Top:
						graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames, positionZ, depth, backColor, color2, dataPoint3D.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, points, pointIndex, tension, operationType, LineSegmentType.Middle, showPointLines ? true : false, forceThickBorder: false, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
						break;
					case SurfaceNames.Bottom:
					{
						DataPoint3D dataPoint3D13 = new DataPoint3D();
						dataPoint3D13.dataPoint = firstPoint.dataPoint;
						dataPoint3D13.index = firstPoint.index;
						dataPoint3D13.xPosition = firstPoint.xPosition;
						dataPoint3D13.yPosition = thirdPoint.Y;
						DataPoint3D dataPoint3D14 = new DataPoint3D();
						dataPoint3D14.dataPoint = secondPoint.dataPoint;
						dataPoint3D14.index = secondPoint.index;
						dataPoint3D14.xPosition = secondPoint.xPosition;
						dataPoint3D14.yPosition = fourthPoint.Y;
						graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames, positionZ, depth, backColor, color2, dataPoint3D.dataPoint.BorderWidth, borderStyle, dataPoint3D13, dataPoint3D14, points, pointIndex, tension, operationType, LineSegmentType.Middle, showPointLines ? true : false, forceThickBorder: false, area.reverseSeriesOrder, multiSeries, 1, clipInsideArea: true);
						break;
					}
					case SurfaceNames.Left:
						if (surfaceSegmentType == LineSegmentType.Single || (!area.reverseSeriesOrder && surfaceSegmentType == LineSegmentType.First) || (area.reverseSeriesOrder && surfaceSegmentType == LineSegmentType.Last))
						{
							DataPoint3D dataPoint3D10 = (firstPoint.xPosition <= secondPoint.xPosition) ? firstPoint : secondPoint;
							DataPoint3D dataPoint3D11 = new DataPoint3D();
							dataPoint3D11.xPosition = dataPoint3D10.xPosition;
							dataPoint3D11.yPosition = ((firstPoint.xPosition <= secondPoint.xPosition) ? thirdPoint.Y : fourthPoint.Y);
							DataPoint3D dataPoint3D12 = new DataPoint3D();
							dataPoint3D12.xPosition = dataPoint3D10.xPosition;
							dataPoint3D12.yPosition = dataPoint3D10.yPosition;
							graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames, positionZ, depth, backColor, color2, dataPoint3D.dataPoint.BorderWidth, borderStyle, dataPoint3D11, dataPoint3D12, points, pointIndex, 0f, operationType, LineSegmentType.Single, forceThinBorder: false, forceThickBorder: true, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
						}
						break;
					case SurfaceNames.Right:
						if (surfaceSegmentType == LineSegmentType.Single || (!area.reverseSeriesOrder && surfaceSegmentType == LineSegmentType.Last) || (area.reverseSeriesOrder && surfaceSegmentType == LineSegmentType.First))
						{
							DataPoint3D dataPoint3D7 = (secondPoint.xPosition >= firstPoint.xPosition) ? secondPoint : firstPoint;
							DataPoint3D dataPoint3D8 = new DataPoint3D();
							dataPoint3D8.xPosition = dataPoint3D7.xPosition;
							dataPoint3D8.yPosition = ((secondPoint.xPosition >= firstPoint.xPosition) ? fourthPoint.Y : thirdPoint.Y);
							DataPoint3D dataPoint3D9 = new DataPoint3D();
							dataPoint3D9.xPosition = dataPoint3D7.xPosition;
							dataPoint3D9.yPosition = dataPoint3D7.yPosition;
							graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, surfaceNames, positionZ, depth, backColor, color2, dataPoint3D.dataPoint.BorderWidth, borderStyle, dataPoint3D8, dataPoint3D9, points, pointIndex, 0f, operationType, LineSegmentType.Single, forceThinBorder: false, forceThickBorder: true, area.reverseSeriesOrder, multiSeries, 0, clipInsideArea: true);
						}
						break;
					case SurfaceNames.Back:
					{
						DataPoint3D dataPoint3D5 = new DataPoint3D();
						dataPoint3D5.dataPoint = firstPoint.dataPoint;
						dataPoint3D5.index = firstPoint.index;
						dataPoint3D5.xPosition = firstPoint.xPosition;
						dataPoint3D5.yPosition = thirdPoint.Y;
						DataPoint3D dataPoint3D6 = new DataPoint3D();
						dataPoint3D6.dataPoint = secondPoint.dataPoint;
						dataPoint3D6.index = secondPoint.index;
						dataPoint3D6.xPosition = secondPoint.xPosition;
						dataPoint3D6.yPosition = fourthPoint.Y;
						graphicsPath3 = Draw3DSplinePolygon(graph, area, positionZ, backColor, color2, dataPoint3D.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, dataPoint3D6, dataPoint3D5, points, pointIndex, tension, operationType, lineSegmentType, showPointLines ? true : false);
						break;
					}
					case SurfaceNames.Front:
					{
						DataPoint3D dataPoint3D3 = new DataPoint3D();
						dataPoint3D3.dataPoint = firstPoint.dataPoint;
						dataPoint3D3.index = firstPoint.index;
						dataPoint3D3.xPosition = firstPoint.xPosition;
						dataPoint3D3.yPosition = thirdPoint.Y;
						DataPoint3D dataPoint3D4 = new DataPoint3D();
						dataPoint3D4.dataPoint = secondPoint.dataPoint;
						dataPoint3D4.index = secondPoint.index;
						dataPoint3D4.xPosition = secondPoint.xPosition;
						dataPoint3D4.yPosition = fourthPoint.Y;
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
						if (surfaceSegmentType != 0)
						{
							if (surfaceSegmentType == LineSegmentType.Middle || (surfaceSegmentType == LineSegmentType.First && lineSegmentType != LineSegmentType.First) || (surfaceSegmentType == LineSegmentType.Last && lineSegmentType != LineSegmentType.Last))
							{
								lineSegmentType = LineSegmentType.Middle;
							}
							if (reversed)
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
						}
						graphicsPath3 = Draw3DSplinePolygon(graph, area, positionZ + depth, backColor, color2, dataPoint3D.dataPoint.BorderWidth, borderStyle, firstPoint, secondPoint, dataPoint3D4, dataPoint3D3, points, pointIndex, tension, operationType, lineSegmentType, showPointLines ? true : false);
						break;
					}
					}
					if (j == 2 && graphicsPath != null && graphicsPath3 != null && graphicsPath3.PointCount > 0)
					{
						graphicsPath.CloseFigure();
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
				if ((!upSideDown && firstPoint.xPosition < secondPoint.xPosition) || (upSideDown && firstPoint.xPosition > secondPoint.xPosition))
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
			else if ((!upSideDown && secondPoint.xPosition < firstPoint.xPosition) || (upSideDown && secondPoint.xPosition > firstPoint.xPosition))
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
			GetBottomPointsPosition(area.Common, area, 0f, ref firstPoint, ref secondPoint, out PointF thirdPoint, out PointF fourthPoint);
			if ((visibleSurfaces & SurfaceNames.Bottom) == SurfaceNames.Bottom)
			{
				visibleSurfaces ^= SurfaceNames.Bottom;
			}
			array = new Point3D[3];
			if (!area.reverseSeriesOrder)
			{
				if ((!upSideDown && firstPoint.xPosition < secondPoint.xPosition) || (upSideDown && firstPoint.xPosition > secondPoint.xPosition))
				{
					array[0] = new Point3D((float)firstPoint.xPosition, thirdPoint.Y, positionZ + depth);
					array[1] = new Point3D((float)firstPoint.xPosition, thirdPoint.Y, positionZ);
					array[2] = new Point3D((float)secondPoint.xPosition, fourthPoint.Y, positionZ);
				}
				else
				{
					array[0] = new Point3D((float)secondPoint.xPosition, fourthPoint.Y, positionZ + depth);
					array[1] = new Point3D((float)secondPoint.xPosition, fourthPoint.Y, positionZ);
					array[2] = new Point3D((float)firstPoint.xPosition, thirdPoint.Y, positionZ);
				}
			}
			else if ((!upSideDown && secondPoint.xPosition < firstPoint.xPosition) || (upSideDown && secondPoint.xPosition > firstPoint.xPosition))
			{
				array[0] = new Point3D((float)secondPoint.xPosition, fourthPoint.Y, positionZ + depth);
				array[1] = new Point3D((float)secondPoint.xPosition, fourthPoint.Y, positionZ);
				array[2] = new Point3D((float)firstPoint.xPosition, thirdPoint.Y, positionZ);
			}
			else
			{
				array[0] = new Point3D((float)firstPoint.xPosition, thirdPoint.Y, positionZ + depth);
				array[1] = new Point3D((float)firstPoint.xPosition, thirdPoint.Y, positionZ);
				array[2] = new Point3D((float)secondPoint.xPosition, fourthPoint.Y, positionZ);
			}
			matrix.TransformPoints(array);
			if (ChartGraphics3D.IsSurfaceVisible(array[2], array[1], array[0]))
			{
				visibleSurfaces |= SurfaceNames.Bottom;
			}
		}

		protected virtual void GetBottomPointsPosition(CommonElements common, ChartArea area, float axisPosition, ref DataPoint3D firstPoint, ref DataPoint3D secondPoint, out PointF thirdPoint, out PointF fourthPoint)
		{
			Axis axis = area.GetAxis(AxisName.Y, firstPoint.dataPoint.series.YAxisType, firstPoint.dataPoint.series.YSubAxisName);
			thirdPoint = new PointF(y: (float)axis.GetPosition(firstPoint.dataPoint.YValues[1]), x: (float)firstPoint.xPosition);
			float y2 = (float)axis.GetPosition(secondPoint.dataPoint.YValues[1]);
			fourthPoint = new PointF((float)secondPoint.xPosition, y2);
			if (!float.IsNaN(thirdPointY2Value))
			{
				thirdPoint.Y = thirdPointY2Value;
			}
			if (!float.IsNaN(fourthPointY2Value))
			{
				fourthPoint.Y = fourthPointY2Value;
			}
		}

		internal GraphicsPath Draw3DSplinePolygon(ChartGraphics graph, ChartArea area, float positionZ, Color backColor, Color borderColor, int borderWidth, ChartDashStyle borderStyle, DataPoint3D firstPoint, DataPoint3D secondPoint, DataPoint3D thirdPoint, DataPoint3D fourthPoint, ArrayList points, int pointIndex, float tension, DrawingOperationTypes operationType, LineSegmentType lineSegmentType, bool forceThinBorder)
		{
			if (tension == 0f)
			{
				SurfaceNames thinBorders = (SurfaceNames)0;
				if (forceThinBorder)
				{
					thinBorders = (SurfaceNames.Left | SurfaceNames.Right);
				}
				return graph.Draw3DPolygon(area, area.matrix3D, area.Area3DStyle.Light, SurfaceNames.Front, positionZ, backColor, borderColor, borderWidth, borderStyle, firstPoint, secondPoint, thirdPoint, fourthPoint, points, pointIndex, tension, operationType, lineSegmentType, thinBorders);
			}
			bool num = (operationType & DrawingOperationTypes.DrawElement) == DrawingOperationTypes.DrawElement;
			GraphicsPath graphicsPath = new GraphicsPath();
			GraphicsPath splineFlattenPath = graph.GetSplineFlattenPath(area, area.matrix3D, positionZ, 0f, firstPoint, secondPoint, points, pointIndex, tension, flatten: false, translateCoordinates: true, 0);
			GraphicsPath splineFlattenPath2 = graph.GetSplineFlattenPath(area, area.matrix3D, positionZ, 0f, thirdPoint, fourthPoint, points, pointIndex, tension, flatten: false, translateCoordinates: true, 1);
			graphicsPath.AddPath(splineFlattenPath, connect: true);
			graphicsPath.AddPath(splineFlattenPath2, connect: true);
			graphicsPath.CloseAllFigures();
			Point3D[] array = new Point3D[3]
			{
				new Point3D((float)firstPoint.xPosition, (float)firstPoint.yPosition, positionZ),
				new Point3D((float)secondPoint.xPosition, (float)secondPoint.yPosition, positionZ),
				new Point3D((float)thirdPoint.xPosition, (float)thirdPoint.yPosition, positionZ)
			};
			area.matrix3D.TransformPoints(array);
			bool visiblePolygon = ChartGraphics3D.IsSurfaceVisible(array[0], array[1], array[2]);
			Color polygonLight = area.matrix3D.GetPolygonLight(array, backColor, visiblePolygon, area.Area3DStyle.YAngle, SurfaceNames.Front, area.reverseSeriesOrder);
			Color color = borderColor;
			if (color == Color.Empty)
			{
				color = ChartGraphics.GetGradientColor(backColor, Color.Black, 0.2);
			}
			Pen pen = null;
			if (num)
			{
				SmoothingMode smoothingMode = graph.SmoothingMode;
				graph.SmoothingMode = SmoothingMode.Default;
				graph.FillPath(new SolidBrush(polygonLight), graphicsPath);
				graph.SmoothingMode = smoothingMode;
				if (forceThinBorder)
				{
					graph.DrawPath(new Pen(color, 1f), graphicsPath);
				}
				else if (polygonLight.A == byte.MaxValue)
				{
					graph.DrawPath(new Pen(polygonLight, 1f), graphicsPath);
				}
				pen = new Pen(color, borderWidth);
				pen.StartCap = LineCap.Round;
				pen.EndCap = LineCap.Round;
				graph.DrawPath(pen, splineFlattenPath);
				graph.DrawPath(pen, splineFlattenPath2);
				switch (lineSegmentType)
				{
				case LineSegmentType.First:
					graph.DrawLine(pen, splineFlattenPath.PathPoints[0], splineFlattenPath2.GetLastPoint());
					break;
				case LineSegmentType.Last:
					graph.DrawLine(pen, splineFlattenPath.GetLastPoint(), splineFlattenPath2.PathPoints[0]);
					break;
				}
			}
			if (graphicsPath != null && pen != null)
			{
				ChartGraphics.Widen(graphicsPath, pen);
			}
			return graphicsPath;
		}
	}
}
