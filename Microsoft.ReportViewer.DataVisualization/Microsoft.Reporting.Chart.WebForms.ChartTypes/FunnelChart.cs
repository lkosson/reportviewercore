using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class FunnelChart : IChartType
	{
		protected ArrayList segmentList;

		protected ArrayList labelInfoList;

		protected ChartGraphics graph;

		protected ChartArea area;

		protected CommonElements common;

		protected RectangleF plotAreaSpacing = new RectangleF(3f, 3f, 3f, 3f);

		private Series chartTypeSeries;

		protected double yValueTotal;

		private double yValueMax;

		private double xValueTotal;

		protected int pointNumber;

		protected RectangleF plotAreaPosition = RectangleF.Empty;

		private FunnelStyle funnelStyle;

		private SizeF funnelNeckSize = new SizeF(50f, 30f);

		protected float funnelSegmentGap;

		private int rotation3D = 5;

		protected bool round3DShape = true;

		protected bool isPyramid;

		private float funnelMinPointHeight;

		protected string funnelPointGapAttributeName = "FunnelPointGap";

		protected string funnelRotationAngleAttributeName = "Funnel3DRotationAngle";

		protected string funnelPointMinHeight = "FunnelMinPointHeight";

		protected string funnel3DDrawingStyleAttributeName = "Funnel3DDrawingStyle";

		protected string funnelInsideLabelAlignmentAttributeName = "FunnelInsideLabelAlignment";

		protected string funnelOutsideLabelPlacementAttributeName = "FunnelOutsideLabelPlacement";

		protected string funnelLabelStyleAttributeName = "FunnelLabelStyle";

		private double[] valuePercentages;

		public virtual string Name => "Funnel";

		public virtual bool Stacked => false;

		public virtual bool SupportStackedGroups => false;

		public bool StackSign => false;

		public virtual bool RequireAxes => false;

		public virtual bool SecondYScale => false;

		public bool CircularChartArea => false;

		public virtual bool SupportLogarithmicAxes => true;

		public virtual bool SwitchValueAxes => false;

		public virtual bool SideBySideSeries => false;

		public virtual bool DataPointsInLegend => true;

		public virtual bool ZeroCrossing => false;

		public virtual bool ApplyPaletteColorsToPoints => true;

		public virtual bool ExtraYValuesConnectedToYAxis => false;

		public virtual bool HundredPercent => false;

		public virtual bool HundredPercentSupportNegative => false;

		public virtual int YValuesPerPoint => 1;

		public virtual LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public virtual void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			chartTypeSeries = null;
			funnelMinPointHeight = 0f;
			this.graph = graph;
			this.common = common;
			this.area = area;
			GetDataPointValuesStatistic();
			if (yValueTotal != 0.0 && pointNumber != 0)
			{
				funnelStyle = GetFunnelStyle(GetDataSeries());
				if (funnelStyle != FunnelStyle.YIsWidth || pointNumber != 1)
				{
					GetFunnelMinPointHeight(GetDataSeries());
					labelInfoList = CreateLabelsInfoList();
					GetPlotAreaSpacing();
					ProcessChartType();
					DrawLabels();
				}
			}
		}

		private void ProcessChartType()
		{
			if (area.Area3DStyle.Enable3D && ((rotation3D > 0 && !isPyramid) || (rotation3D < 0 && isPyramid)))
			{
				segmentList.Reverse();
			}
			bool flag = true;
			bool flag2 = (!area.Area3DStyle.Enable3D) ? true : false;
			Series dataSeries = GetDataSeries();
			if (flag2 && flag && dataSeries != null && dataSeries.ShadowOffset != 0)
			{
				foreach (FunnelSegmentInfo segment in segmentList)
				{
					DrawFunnelCircularSegment(segment.Point, segment.PointIndex, segment.StartWidth, segment.EndWidth, segment.Location, segment.Height, segment.NothingOnTop, segment.NothingOnBottom, drawSegment: false, drawSegmentShadow: true);
				}
				flag2 = false;
			}
			foreach (FunnelSegmentInfo segment2 in segmentList)
			{
				DrawFunnelCircularSegment(segment2.Point, segment2.PointIndex, segment2.StartWidth, segment2.EndWidth, segment2.Location, segment2.Height, segment2.NothingOnTop, segment2.NothingOnBottom, drawSegment: true, flag2);
			}
		}

		protected virtual void GetPointWidthAndHeight(Series series, int pointIndex, float location, out float height, out float startWidth, out float endWidth)
		{
			PointF empty = PointF.Empty;
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(plotAreaPosition);
			float num = absoluteRectangle.Height - funnelSegmentGap * (float)(pointNumber - (ShouldDrawFirstPoint() ? 1 : 2));
			if (num < 0f)
			{
				num = 0f;
			}
			if (funnelStyle == FunnelStyle.YIsWidth)
			{
				if (xValueTotal == 0.0)
				{
					height = num / (float)(pointNumber - 1);
				}
				else
				{
					height = (float)((double)num * (GetXValue(series.Points[pointIndex]) / xValueTotal));
				}
				height = CheckMinHeight(height);
				startWidth = (float)((double)absoluteRectangle.Width * (GetYValue(series.Points[pointIndex - 1], pointIndex - 1) / yValueMax));
				endWidth = (float)((double)absoluteRectangle.Width * (GetYValue(series.Points[pointIndex], pointIndex) / yValueMax));
				empty = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, location + height);
			}
			else
			{
				if (funnelStyle != 0)
				{
					throw new InvalidOperationException(SR.ExceptionFunnelStyleUnknown(funnelStyle.ToString()));
				}
				height = (float)((double)num * (GetYValue(series.Points[pointIndex], pointIndex) / yValueTotal));
				height = CheckMinHeight(height);
				PointF linesIntersection = ChartGraphics3D.GetLinesIntersection(absoluteRectangle.X, location, absoluteRectangle.Right, location, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.X + absoluteRectangle.Width / 2f - funnelNeckSize.Width / 2f, absoluteRectangle.Bottom - funnelNeckSize.Height);
				PointF linesIntersection2 = ChartGraphics3D.GetLinesIntersection(absoluteRectangle.X, location + height, absoluteRectangle.Right, location + height, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.X + absoluteRectangle.Width / 2f - funnelNeckSize.Width / 2f, absoluteRectangle.Bottom - funnelNeckSize.Height);
				startWidth = (absoluteRectangle.X + absoluteRectangle.Width / 2f - linesIntersection.X) * 2f;
				endWidth = (absoluteRectangle.X + absoluteRectangle.Width / 2f - linesIntersection2.X) * 2f;
				empty = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, location + height / 2f);
			}
			series.Points[pointIndex].positionRel = graph.GetRelativePoint(empty);
		}

		protected virtual bool ShouldDrawFirstPoint()
		{
			if (funnelStyle != 0)
			{
				return isPyramid;
			}
			return true;
		}

		private void DrawFunnel3DSquareSegment(DataPoint point, int pointIndex, float startWidth, float endWidth, float location, float height, bool nothingOnTop, bool nothingOnBottom, bool drawSegment, bool drawSegmentShadow)
		{
			if (!nothingOnBottom)
			{
				height += 0.3f;
			}
			Color gradientColor = ChartGraphics.GetGradientColor(point.Color, Color.White, 0.3);
			Color gradientColor2 = ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.3);
			if (funnelStyle == FunnelStyle.YIsHeight && !isPyramid)
			{
				if (startWidth < funnelNeckSize.Width)
				{
					startWidth = funnelNeckSize.Width;
				}
				if (endWidth < funnelNeckSize.Width)
				{
					endWidth = funnelNeckSize.Width;
				}
			}
			float num = (float)((double)(startWidth / 2f) * Math.Sin((double)((float)rotation3D / 180f) * Math.PI));
			float num2 = (float)((double)(endWidth / 2f) * Math.Sin((double)((float)rotation3D / 180f) * Math.PI));
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(plotAreaPosition);
			float num3 = absoluteRectangle.X + absoluteRectangle.Width / 2f;
			graph.StartHotRegion(point);
			graph.StartAnimation();
			GraphicsPath graphicsPath = new GraphicsPath();
			if (startWidth > 0f)
			{
				graphicsPath.AddLine(num3 - startWidth / 2f, location, num3, location + num);
			}
			graphicsPath.AddLine(num3, location + num, num3, location + height + num2);
			if (endWidth > 0f)
			{
				graphicsPath.AddLine(num3, location + height + num2, num3 - endWidth / 2f, location + height);
			}
			graphicsPath.AddLine(num3 - endWidth / 2f, location + height, num3 - startWidth / 2f, location);
			if (common.ProcessModePaint)
			{
				graph.DrawPathAbs(graphicsPath, drawSegment ? gradientColor : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
			}
			if (common.ProcessModeRegions)
			{
				common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: false, graph, point, point.series.Name, pointIndex);
			}
			else
			{
				graphicsPath.Dispose();
			}
			graphicsPath = new GraphicsPath();
			if (startWidth > 0f)
			{
				graphicsPath.AddLine(num3 + startWidth / 2f, location, num3, location + num);
			}
			graphicsPath.AddLine(num3, location + num, num3, location + height + num2);
			if (endWidth > 0f)
			{
				graphicsPath.AddLine(num3, location + height + num2, num3 + endWidth / 2f, location + height);
			}
			graphicsPath.AddLine(num3 + endWidth / 2f, location + height, num3 + startWidth / 2f, location);
			if (common.ProcessModePaint)
			{
				graph.DrawPathAbs(graphicsPath, drawSegment ? gradientColor2 : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
			}
			if (common.ProcessModeRegions)
			{
				common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: false, graph, point, point.series.Name, pointIndex);
			}
			else
			{
				graphicsPath.Dispose();
			}
			if ((float)rotation3D > 0f && startWidth > 0f && nothingOnTop && area.Area3DStyle.Enable3D)
			{
				PointF[] points = new PointF[4]
				{
					new PointF(num3 + startWidth / 2f, location),
					new PointF(num3, location + num),
					new PointF(num3 - startWidth / 2f, location),
					new PointF(num3, location - num)
				};
				GraphicsPath graphicsPath2 = new GraphicsPath();
				graphicsPath2.AddLines(points);
				graphicsPath2.CloseAllFigures();
				if (common.ProcessModePaint)
				{
					graph.DrawPathAbs(graphicsPath2, drawSegment ? ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.4) : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
				}
				if (common.ProcessModeRegions)
				{
					common.HotRegionsList.AddHotRegion(graphicsPath2, relativePath: false, graph, point, point.series.Name, pointIndex);
				}
				else
				{
					graphicsPath2.Dispose();
				}
			}
			if ((float)rotation3D < 0f && startWidth > 0f && nothingOnBottom && area.Area3DStyle.Enable3D)
			{
				PointF[] points2 = new PointF[4]
				{
					new PointF(num3 + endWidth / 2f, location + height),
					new PointF(num3, location + height + num2),
					new PointF(num3 - endWidth / 2f, location + height),
					new PointF(num3, location + height - num2)
				};
				GraphicsPath graphicsPath3 = new GraphicsPath();
				graphicsPath3.AddLines(points2);
				graphicsPath3.CloseAllFigures();
				if (common.ProcessModePaint)
				{
					graph.DrawPathAbs(graphicsPath3, drawSegment ? ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.4) : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
				}
				if (common.ProcessModeRegions)
				{
					common.HotRegionsList.AddHotRegion(graphicsPath3, relativePath: false, graph, point, point.series.Name, pointIndex);
				}
				else
				{
					graphicsPath3.Dispose();
				}
			}
			graph.StopAnimation();
			graph.EndHotRegion();
		}

		private void DrawFunnelCircularSegment(DataPoint point, int pointIndex, float startWidth, float endWidth, float location, float height, bool nothingOnTop, bool nothingOnBottom, bool drawSegment, bool drawSegmentShadow)
		{
			PointF leftSideLinePoint = PointF.Empty;
			PointF rightSideLinePoint = PointF.Empty;
			if (area.Area3DStyle.Enable3D && !round3DShape)
			{
				DrawFunnel3DSquareSegment(point, pointIndex, startWidth, endWidth, location, height, nothingOnTop, nothingOnBottom, drawSegment, drawSegmentShadow);
				return;
			}
			if (!nothingOnBottom)
			{
				height += 0.3f;
			}
			float num = startWidth;
			float num2 = endWidth;
			if (funnelStyle == FunnelStyle.YIsHeight && !isPyramid)
			{
				if (startWidth < funnelNeckSize.Width)
				{
					startWidth = funnelNeckSize.Width;
				}
				if (endWidth < funnelNeckSize.Width)
				{
					endWidth = funnelNeckSize.Width;
				}
			}
			float tension = 0.8f;
			float num3 = (float)((double)(startWidth / 2f) * Math.Sin((double)((float)rotation3D / 180f) * Math.PI));
			float num4 = (float)((double)(endWidth / 2f) * Math.Sin((double)((float)rotation3D / 180f) * Math.PI));
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(plotAreaPosition);
			float num5 = absoluteRectangle.X + absoluteRectangle.Width / 2f;
			graph.StartHotRegion(point);
			graph.StartAnimation();
			GraphicsPath graphicsPath = new GraphicsPath();
			if (startWidth > 0f)
			{
				if (area.Area3DStyle.Enable3D)
				{
					PointF[] points = new PointF[4]
					{
						new PointF(num5 + startWidth / 2f, location),
						new PointF(num5, location + num3),
						new PointF(num5 - startWidth / 2f, location),
						new PointF(num5, location - num3)
					};
					GraphicsPath graphicsPath2 = new GraphicsPath();
					graphicsPath2.AddClosedCurve(points, tension);
					graphicsPath2.Flatten();
					graphicsPath2.Reverse();
					graph.AddEllipseSegment(graphicsPath, graphicsPath2, null, veticalOrientation: true, 0f, out leftSideLinePoint, out rightSideLinePoint);
				}
				else
				{
					graphicsPath.AddLine(num5 - startWidth / 2f, location, num5 + startWidth / 2f, location);
				}
			}
			if (funnelStyle == FunnelStyle.YIsHeight && !isPyramid && startWidth > funnelNeckSize.Width && endWidth <= funnelNeckSize.Width)
			{
				PointF linesIntersection = ChartGraphics3D.GetLinesIntersection(num5 + funnelNeckSize.Width / 2f, absoluteRectangle.Top, num5 + funnelNeckSize.Width / 2f, absoluteRectangle.Bottom, num5 + num / 2f, location, num5 + num2 / 2f, location + height);
				linesIntersection.Y = absoluteRectangle.Bottom - funnelNeckSize.Height;
				graphicsPath.AddLine(num5 + startWidth / 2f, location, linesIntersection.X, linesIntersection.Y);
				graphicsPath.AddLine(linesIntersection.X, linesIntersection.Y, linesIntersection.X, location + height);
			}
			else
			{
				graphicsPath.AddLine(num5 + startWidth / 2f, location, num5 + endWidth / 2f, location + height);
			}
			if (endWidth > 0f)
			{
				if (area.Area3DStyle.Enable3D)
				{
					PointF[] points2 = new PointF[4]
					{
						new PointF(num5 + endWidth / 2f, location + height),
						new PointF(num5, location + height + num4),
						new PointF(num5 - endWidth / 2f, location + height),
						new PointF(num5, location + height - num4)
					};
					GraphicsPath graphicsPath3 = new GraphicsPath();
					graphicsPath3.AddClosedCurve(points2, tension);
					graphicsPath3.Flatten();
					graphicsPath3.Reverse();
					GraphicsPath graphicsPath4 = new GraphicsPath();
					graph.AddEllipseSegment(graphicsPath4, graphicsPath3, null, veticalOrientation: true, 0f, out leftSideLinePoint, out rightSideLinePoint);
					graphicsPath4.Reverse();
					if (graphicsPath4.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath4, connect: false);
					}
				}
				else
				{
					graphicsPath.AddLine(num5 + endWidth / 2f, location + height, num5 - endWidth / 2f, location + height);
				}
			}
			if (funnelStyle == FunnelStyle.YIsHeight && !isPyramid && startWidth > funnelNeckSize.Width && endWidth <= funnelNeckSize.Width)
			{
				PointF linesIntersection2 = ChartGraphics3D.GetLinesIntersection(num5 - funnelNeckSize.Width / 2f, absoluteRectangle.Top, num5 - funnelNeckSize.Width / 2f, absoluteRectangle.Bottom, num5 - num / 2f, location, num5 - num2 / 2f, location + height);
				linesIntersection2.Y = absoluteRectangle.Bottom - funnelNeckSize.Height;
				graphicsPath.AddLine(linesIntersection2.X, location + height, linesIntersection2.X, linesIntersection2.Y);
				graphicsPath.AddLine(linesIntersection2.X, linesIntersection2.Y, num5 - startWidth / 2f, location);
			}
			else
			{
				graphicsPath.AddLine(num5 - endWidth / 2f, location + height, num5 - startWidth / 2f, location);
			}
			if (common.ProcessModePaint)
			{
				if (area.Area3DStyle.Enable3D && graph.ActiveRenderingType == RenderingType.Gdi)
				{
					Color gradientColor = ChartGraphics.GetGradientColor(point.Color, Color.White, 0.3);
					Color gradientColor2 = ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.3);
					RectangleF bounds = graphicsPath.GetBounds();
					if (bounds.Width == 0f)
					{
						bounds.Width = 1f;
					}
					if (bounds.Height == 0f)
					{
						bounds.Height = 1f;
					}
					using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(bounds, gradientColor, gradientColor2, 0f))
					{
						ColorBlend colorBlend = new ColorBlend(5);
						colorBlend.Colors[0] = gradientColor2;
						colorBlend.Colors[1] = gradientColor2;
						colorBlend.Colors[2] = gradientColor;
						colorBlend.Colors[3] = gradientColor2;
						colorBlend.Colors[4] = gradientColor2;
						colorBlend.Positions[0] = 0f;
						colorBlend.Positions[1] = 0f;
						colorBlend.Positions[2] = 0.5f;
						colorBlend.Positions[3] = 1f;
						colorBlend.Positions[4] = 1f;
						linearGradientBrush.InterpolationColors = colorBlend;
						graph.Graphics.FillPath(linearGradientBrush, graphicsPath);
						Pen pen = new Pen(point.BorderColor, point.BorderWidth);
						pen.DashStyle = graph.GetPenStyle(point.BorderStyle);
						if (point.BorderWidth == 0 || point.BorderStyle == ChartDashStyle.NotSet || point.BorderColor == Color.Empty)
						{
							pen = new Pen(ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.3), 1f);
							pen.Alignment = PenAlignment.Inset;
						}
						pen.StartCap = LineCap.Round;
						pen.EndCap = LineCap.Round;
						pen.LineJoin = LineJoin.Bevel;
						graph.DrawPath(pen, graphicsPath);
						pen.Dispose();
					}
				}
				else
				{
					graph.DrawPathAbs(graphicsPath, drawSegment ? point.Color : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
				}
			}
			if (common.ProcessModeRegions)
			{
				common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: false, graph, point, point.series.Name, pointIndex);
			}
			else
			{
				graphicsPath.Dispose();
			}
			if ((float)rotation3D > 0f && startWidth > 0f && nothingOnTop && area.Area3DStyle.Enable3D)
			{
				PointF[] points3 = new PointF[4]
				{
					new PointF(num5 + startWidth / 2f, location),
					new PointF(num5, location + num3),
					new PointF(num5 - startWidth / 2f, location),
					new PointF(num5, location - num3)
				};
				GraphicsPath graphicsPath5 = new GraphicsPath();
				graphicsPath5.AddClosedCurve(points3, tension);
				if (common.ProcessModePaint)
				{
					graph.DrawPathAbs(graphicsPath5, drawSegment ? ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.4) : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
				}
				if (common.ProcessModeRegions)
				{
					common.HotRegionsList.AddHotRegion(graphicsPath5, relativePath: false, graph, point, point.series.Name, pointIndex);
				}
				else
				{
					graphicsPath5.Dispose();
				}
			}
			if ((float)rotation3D < 0f && startWidth > 0f && nothingOnBottom && area.Area3DStyle.Enable3D)
			{
				PointF[] points4 = new PointF[4]
				{
					new PointF(num5 + endWidth / 2f, location + height),
					new PointF(num5, location + height + num4),
					new PointF(num5 - endWidth / 2f, location + height),
					new PointF(num5, location + height - num4)
				};
				GraphicsPath graphicsPath6 = new GraphicsPath();
				graphicsPath6.AddClosedCurve(points4, tension);
				if (common.ProcessModePaint)
				{
					graph.DrawPathAbs(graphicsPath6, drawSegment ? ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.4) : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
				}
				if (common.ProcessModeRegions)
				{
					common.HotRegionsList.AddHotRegion(graphicsPath6, relativePath: false, graph, point, point.series.Name, pointIndex);
				}
				else
				{
					graphicsPath6.Dispose();
				}
			}
			graph.StopAnimation();
			graph.EndHotRegion();
		}

		private ArrayList GetFunnelSegmentPositions()
		{
			ArrayList arrayList = new ArrayList();
			Series dataSeries = GetDataSeries();
			if (dataSeries != null)
			{
				funnelStyle = GetFunnelStyle(dataSeries);
				round3DShape = (GetFunnel3DDrawingStyle(dataSeries) == Funnel3DDrawingStyle.CircularBase);
				funnelSegmentGap = GetFunnelPointGap(dataSeries);
				funnelNeckSize = GetFunnelNeckSize(dataSeries);
				float num = graph.GetAbsolutePoint(plotAreaPosition.Location).Y;
				if (isPyramid)
				{
					num = graph.GetAbsoluteRectangle(plotAreaPosition).Bottom;
				}
				for (int i = 0; i >= 0 && i < dataSeries.Points.Count; i++)
				{
					DataPoint point = dataSeries.Points[i];
					if (i <= 0 && !ShouldDrawFirstPoint())
					{
						continue;
					}
					float startWidth = 0f;
					float endWidth = 0f;
					float height = 0f;
					GetPointWidthAndHeight(dataSeries, i, num, out height, out startWidth, out endWidth);
					bool nothingOnTop = false;
					bool nothingOnBottom = false;
					if (funnelSegmentGap > 0f)
					{
						nothingOnTop = true;
						nothingOnBottom = true;
					}
					else
					{
						if (ShouldDrawFirstPoint())
						{
							if (i == 0 || dataSeries.Points[i - 1].Color.A != byte.MaxValue)
							{
								if (isPyramid)
								{
									nothingOnBottom = true;
								}
								else
								{
									nothingOnTop = true;
								}
							}
						}
						else if (i == 1 || dataSeries.Points[i - 1].Color.A != byte.MaxValue)
						{
							if (isPyramid)
							{
								nothingOnBottom = true;
							}
							else
							{
								nothingOnTop = true;
							}
						}
						if (i == dataSeries.Points.Count - 1)
						{
							if (isPyramid)
							{
								nothingOnTop = true;
							}
							else
							{
								nothingOnBottom = true;
							}
						}
						else if (dataSeries.Points[i + 1].Color.A != byte.MaxValue)
						{
							if (isPyramid)
							{
								nothingOnTop = true;
							}
							else
							{
								nothingOnBottom = true;
							}
						}
					}
					FunnelSegmentInfo funnelSegmentInfo = new FunnelSegmentInfo();
					funnelSegmentInfo.Point = point;
					funnelSegmentInfo.PointIndex = i;
					funnelSegmentInfo.StartWidth = startWidth;
					funnelSegmentInfo.EndWidth = endWidth;
					funnelSegmentInfo.Location = (isPyramid ? (num - height) : num);
					funnelSegmentInfo.Height = height;
					funnelSegmentInfo.NothingOnTop = nothingOnTop;
					funnelSegmentInfo.NothingOnBottom = nothingOnBottom;
					arrayList.Add(funnelSegmentInfo);
					num = ((!isPyramid) ? (num + (height + funnelSegmentGap)) : (num - (height + funnelSegmentGap)));
				}
			}
			return arrayList;
		}

		private void DrawLabels()
		{
			foreach (FunnelPointLabelInfo labelInfo in labelInfoList)
			{
				if (labelInfo.Position.IsEmpty || float.IsNaN(labelInfo.Position.X) || float.IsNaN(labelInfo.Position.Y) || float.IsNaN(labelInfo.Position.Width) || float.IsNaN(labelInfo.Position.Height))
				{
					continue;
				}
				graph.StartHotRegion(labelInfo.Point);
				graph.StartAnimation();
				SizeF sizeF = graph.MeasureString("W", labelInfo.Point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic));
				if (!labelInfo.CalloutPoint1.IsEmpty && !labelInfo.CalloutPoint2.IsEmpty && !float.IsNaN(labelInfo.CalloutPoint1.X) && !float.IsNaN(labelInfo.CalloutPoint1.Y) && !float.IsNaN(labelInfo.CalloutPoint2.X) && !float.IsNaN(labelInfo.CalloutPoint2.Y))
				{
					if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
					{
						labelInfo.CalloutPoint2.X -= sizeF.Width / 2f;
						labelInfo.CalloutPoint1.X += 2f;
					}
					else
					{
						labelInfo.CalloutPoint2.X += sizeF.Width / 2f;
						labelInfo.CalloutPoint1.X += 2f;
					}
					Color calloutLineColor = GetCalloutLineColor(labelInfo.Point);
					graph.DrawLineAbs(calloutLineColor, 1, ChartDashStyle.Solid, labelInfo.CalloutPoint1, labelInfo.CalloutPoint2);
				}
				RectangleF position = labelInfo.Position;
				position.Inflate(sizeF.Width / 2f, sizeF.Height / 8f);
				position = graph.GetRelativeRectangle(position);
				StringFormat stringFormat = (StringFormat)labelInfo.Format.Clone();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				graph.DrawPointLabelStringRel(common, labelInfo.Text, labelInfo.Point.Font, new SolidBrush(labelInfo.Point.FontColor), position, stringFormat, labelInfo.Point.FontAngle, position, labelInfo.Point.LabelBackColor, labelInfo.Point.LabelBorderColor, labelInfo.Point.LabelBorderWidth, labelInfo.Point.LabelBorderStyle, labelInfo.Point.series, labelInfo.Point, labelInfo.PointIndex);
				graph.StopAnimation();
				graph.EndHotRegion();
			}
		}

		private ArrayList CreateLabelsInfoList()
		{
			ArrayList arrayList = new ArrayList();
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(area.Position.ToRectangleF());
			Series dataSeries = GetDataSeries();
			if (dataSeries != null)
			{
				int num = 0;
				{
					foreach (DataPoint point in dataSeries.Points)
					{
						if (!point.Empty)
						{
							string label = point.Label;
							if (point.ShowLabelAsValue || label.Length > 0)
							{
								FunnelPointLabelInfo funnelPointLabelInfo = new FunnelPointLabelInfo();
								funnelPointLabelInfo.Point = point;
								funnelPointLabelInfo.PointIndex = num;
								if (label.Length == 0)
								{
									funnelPointLabelInfo.Text = ValueConverter.FormatValue(point.series.chart, point, point.YValues[0], point.LabelFormat, point.series.YValueType, ChartElementType.DataPoint);
								}
								else
								{
									funnelPointLabelInfo.Text = point.ReplaceKeywords(label);
								}
								funnelPointLabelInfo.Style = GetLabelStyle(point);
								if (funnelPointLabelInfo.Style == FunnelLabelStyle.Inside)
								{
									funnelPointLabelInfo.VerticalAlignment = GetInsideLabelAlignment(point);
								}
								if (funnelPointLabelInfo.Style != 0)
								{
									funnelPointLabelInfo.OutsidePlacement = GetOutsideLabelPlacement(point);
								}
								funnelPointLabelInfo.Size = graph.MeasureString(funnelPointLabelInfo.Text, point.Font, absoluteRectangle.Size, new StringFormat(StringFormat.GenericTypographic));
								if (funnelPointLabelInfo.Text.Length > 0 && funnelPointLabelInfo.Style != FunnelLabelStyle.Disabled)
								{
									arrayList.Add(funnelPointLabelInfo);
								}
							}
						}
						num++;
					}
					return arrayList;
				}
			}
			return arrayList;
		}

		private bool FitPointLabels()
		{
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(plotAreaPosition);
			absoluteRectangle.Inflate(-4f, -4f);
			GetLabelsPosition();
			RectangleF absoluteRectangle2 = graph.GetAbsoluteRectangle(new RectangleF(1f, 1f, 1f, 1f));
			foreach (FunnelPointLabelInfo labelInfo in labelInfoList)
			{
				RectangleF position = labelInfo.Position;
				if (labelInfo.Style == FunnelLabelStyle.Outside || labelInfo.Style == FunnelLabelStyle.OutsideInColumn)
				{
					float num = 10f;
					if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
					{
						position.Width += num;
					}
					else if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Left)
					{
						position.X -= num;
						position.Width += num;
					}
				}
				if (labelInfo.Style != 0)
				{
					if (absoluteRectangle.X - position.X > absoluteRectangle2.X)
					{
						absoluteRectangle2.X = absoluteRectangle.X - position.X;
					}
					if (position.Right - absoluteRectangle.Right > absoluteRectangle2.Width)
					{
						absoluteRectangle2.Width = position.Right - absoluteRectangle.Right;
					}
				}
				if (absoluteRectangle.Y - position.Y > absoluteRectangle2.Y)
				{
					absoluteRectangle2.Y = absoluteRectangle.Y - position.Y;
				}
				if (position.Bottom - absoluteRectangle.Bottom > absoluteRectangle2.Height)
				{
					absoluteRectangle2.Height = position.Bottom - absoluteRectangle.Bottom;
				}
			}
			absoluteRectangle2 = graph.GetRelativeRectangle(absoluteRectangle2);
			if (absoluteRectangle2.X > 1f || absoluteRectangle2.Y > 1f || absoluteRectangle2.Width > 1f || absoluteRectangle2.Height > 1f)
			{
				plotAreaSpacing = absoluteRectangle2;
				plotAreaPosition = GetPlotAreaPosition();
				segmentList = GetFunnelSegmentPositions();
				GetLabelsPosition();
				return false;
			}
			return true;
		}

		private void GetLabelsPosition()
		{
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(plotAreaPosition);
			float num = absoluteRectangle.X + absoluteRectangle.Width / 2f;
			SizeF sizeF = new SizeF(3f, 3f);
			foreach (FunnelPointLabelInfo labelInfo in labelInfoList)
			{
				bool flag = false;
				int num2 = labelInfo.PointIndex + ((!ShouldDrawFirstPoint()) ? 1 : 0);
				if (num2 > segmentList.Count && !ShouldDrawFirstPoint())
				{
					num2 = segmentList.Count;
					flag = true;
				}
				FunnelSegmentInfo funnelSegmentInfo = null;
				foreach (FunnelSegmentInfo segment in segmentList)
				{
					if (segment.PointIndex == num2)
					{
						funnelSegmentInfo = segment;
						break;
					}
				}
				if (funnelSegmentInfo == null)
				{
					continue;
				}
				labelInfo.Position.Width = labelInfo.Size.Width;
				labelInfo.Position.Height = labelInfo.Size.Height;
				if (labelInfo.Style == FunnelLabelStyle.Outside || labelInfo.Style == FunnelLabelStyle.OutsideInColumn)
				{
					if (funnelStyle == FunnelStyle.YIsHeight)
					{
						float num3 = funnelSegmentInfo.StartWidth;
						float num4 = funnelSegmentInfo.EndWidth;
						if (!isPyramid)
						{
							if (num3 < funnelNeckSize.Width)
							{
								num3 = funnelNeckSize.Width;
							}
							if (num4 < funnelNeckSize.Width)
							{
								num4 = funnelNeckSize.Width;
							}
							if (funnelSegmentInfo.StartWidth >= funnelNeckSize.Width && funnelSegmentInfo.EndWidth < funnelNeckSize.Width)
							{
								num4 = funnelSegmentInfo.EndWidth;
							}
						}
						labelInfo.Position.Y = funnelSegmentInfo.Location + funnelSegmentInfo.Height / 2f - labelInfo.Size.Height / 2f;
						if (labelInfo.Style == FunnelLabelStyle.OutsideInColumn)
						{
							if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
							{
								labelInfo.Position.X = absoluteRectangle.Right + 4f * sizeF.Width;
								if (!isPyramid)
								{
									labelInfo.CalloutPoint1.X = num + Math.Max(funnelNeckSize.Width / 2f, (num3 + num4) / 4f);
								}
								else
								{
									labelInfo.CalloutPoint1.X = num + (num3 + num4) / 4f;
								}
								labelInfo.CalloutPoint2.X = labelInfo.Position.X;
							}
							else
							{
								labelInfo.Position.X = absoluteRectangle.X - labelInfo.Size.Width - 4f * sizeF.Width;
								if (!isPyramid)
								{
									labelInfo.CalloutPoint1.X = num - Math.Max(funnelNeckSize.Width / 2f, (num3 + num4) / 4f);
								}
								else
								{
									labelInfo.CalloutPoint1.X = num - (num3 + num4) / 4f;
								}
								labelInfo.CalloutPoint2.X = labelInfo.Position.Right;
							}
							labelInfo.CalloutPoint1.Y = funnelSegmentInfo.Location + funnelSegmentInfo.Height / 2f;
							labelInfo.CalloutPoint2.Y = labelInfo.CalloutPoint1.Y;
						}
						else if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
						{
							labelInfo.Position.X = num + (num3 + num4) / 4f + 4f * sizeF.Width;
						}
						else
						{
							labelInfo.Position.X = num - labelInfo.Size.Width - (num3 + num4) / 4f - 4f * sizeF.Width;
						}
					}
					else
					{
						if (flag)
						{
							if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
							{
								labelInfo.Position.X = num + funnelSegmentInfo.EndWidth / 2f + 4f * sizeF.Width;
							}
							else
							{
								labelInfo.Position.X = num - labelInfo.Size.Width - funnelSegmentInfo.EndWidth / 2f - 4f * sizeF.Width;
							}
							labelInfo.Position.Y = funnelSegmentInfo.Location + funnelSegmentInfo.Height - labelInfo.Size.Height / 2f;
						}
						else
						{
							if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
							{
								labelInfo.Position.X = num + funnelSegmentInfo.StartWidth / 2f + 4f * sizeF.Width;
							}
							else
							{
								labelInfo.Position.X = num - labelInfo.Size.Width - funnelSegmentInfo.StartWidth / 2f - 4f * sizeF.Width;
							}
							labelInfo.Position.Y = funnelSegmentInfo.Location - labelInfo.Size.Height / 2f;
						}
						if (labelInfo.Style == FunnelLabelStyle.OutsideInColumn)
						{
							if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
							{
								labelInfo.Position.X = absoluteRectangle.Right + 4f * sizeF.Width;
								labelInfo.CalloutPoint1.X = num + (flag ? funnelSegmentInfo.EndWidth : funnelSegmentInfo.StartWidth) / 2f;
								labelInfo.CalloutPoint2.X = labelInfo.Position.X;
							}
							else
							{
								labelInfo.Position.X = absoluteRectangle.X - labelInfo.Size.Width - 4f * sizeF.Width;
								labelInfo.CalloutPoint1.X = num - (flag ? funnelSegmentInfo.EndWidth : funnelSegmentInfo.StartWidth) / 2f;
								labelInfo.CalloutPoint2.X = labelInfo.Position.Right;
							}
							labelInfo.CalloutPoint1.Y = funnelSegmentInfo.Location;
							if (flag)
							{
								labelInfo.CalloutPoint1.Y += funnelSegmentInfo.Height;
							}
							labelInfo.CalloutPoint2.Y = labelInfo.CalloutPoint1.Y;
						}
					}
				}
				else if (labelInfo.Style == FunnelLabelStyle.Inside)
				{
					labelInfo.Position.X = num - labelInfo.Size.Width / 2f;
					if (funnelStyle == FunnelStyle.YIsHeight)
					{
						labelInfo.Position.Y = funnelSegmentInfo.Location + funnelSegmentInfo.Height / 2f - labelInfo.Size.Height / 2f;
						if (labelInfo.VerticalAlignment == FunnelLabelVerticalAlignment.Top)
						{
							labelInfo.Position.Y -= funnelSegmentInfo.Height / 2f - labelInfo.Size.Height / 2f - sizeF.Height;
						}
						else if (labelInfo.VerticalAlignment == FunnelLabelVerticalAlignment.Bottom)
						{
							labelInfo.Position.Y += funnelSegmentInfo.Height / 2f - labelInfo.Size.Height / 2f - sizeF.Height;
						}
					}
					else
					{
						labelInfo.Position.Y = funnelSegmentInfo.Location - labelInfo.Size.Height / 2f;
						if (labelInfo.VerticalAlignment == FunnelLabelVerticalAlignment.Top)
						{
							labelInfo.Position.Y -= labelInfo.Size.Height / 2f + sizeF.Height;
						}
						else if (labelInfo.VerticalAlignment == FunnelLabelVerticalAlignment.Bottom)
						{
							labelInfo.Position.Y += labelInfo.Size.Height / 2f + sizeF.Height;
						}
						if (flag)
						{
							labelInfo.Position.Y += funnelSegmentInfo.Height;
						}
					}
					if (area.Area3DStyle.Enable3D)
					{
						labelInfo.Position.Y += (float)((double)((funnelSegmentInfo.EndWidth + funnelSegmentInfo.StartWidth) / 4f) * Math.Sin((double)((float)rotation3D / 180f) * Math.PI));
					}
				}
				int num5 = 0;
				while (IsLabelsOverlap(labelInfo) && num5 < 1000)
				{
					float num6 = isPyramid ? (-3f) : 3f;
					labelInfo.Position.Y += num6;
					if (!labelInfo.CalloutPoint2.IsEmpty)
					{
						labelInfo.CalloutPoint2.Y += num6;
					}
					num5++;
				}
			}
		}

		private bool IsLabelsOverlap(FunnelPointLabelInfo testLabelInfo)
		{
			RectangleF position = testLabelInfo.Position;
			position.Inflate(1f, 1f);
			if (!testLabelInfo.Point.LabelBackColor.IsEmpty || (testLabelInfo.Point.LabelBorderWidth > 0 && !testLabelInfo.Point.LabelBorderColor.IsEmpty && testLabelInfo.Point.LabelBorderStyle != 0))
			{
				position.Inflate(4f, 4f);
			}
			foreach (FunnelPointLabelInfo labelInfo in labelInfoList)
			{
				if (labelInfo.PointIndex == testLabelInfo.PointIndex)
				{
					break;
				}
				if (!labelInfo.Position.IsEmpty && labelInfo.Position.IntersectsWith(position))
				{
					return true;
				}
			}
			return false;
		}

		private FunnelLabelStyle GetLabelStyle(DataPointAttributes attributes)
		{
			FunnelLabelStyle result = FunnelLabelStyle.OutsideInColumn;
			string text = attributes[funnelLabelStyleAttributeName];
			if (text != null && text.Length > 0)
			{
				try
				{
					result = (FunnelLabelStyle)Enum.Parse(typeof(FunnelLabelStyle), text, ignoreCase: true);
					return result;
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(result.ToString(), funnelLabelStyleAttributeName));
				}
			}
			return result;
		}

		private void GetPlotAreaSpacing()
		{
			plotAreaSpacing = new RectangleF(1f, 1f, 1f, 1f);
			plotAreaPosition = GetPlotAreaPosition();
			segmentList = GetFunnelSegmentPositions();
			if (area.InnerPlotPosition.Auto)
			{
				int num = 0;
				while (!FitPointLabels() && num < 5)
				{
					num++;
				}
			}
			else
			{
				GetLabelsPosition();
			}
		}

		private RectangleF GetPlotAreaPosition()
		{
			RectangleF rectangleF = area.InnerPlotPosition.Auto ? area.Position.ToRectangleF() : area.PlotAreaPosition.ToRectangleF();
			if (plotAreaSpacing.Y > rectangleF.Height / 2f)
			{
				plotAreaSpacing.Y = rectangleF.Height / 2f;
			}
			if (plotAreaSpacing.Height > rectangleF.Height / 2f)
			{
				plotAreaSpacing.Height = rectangleF.Height / 2f;
			}
			rectangleF.X += plotAreaSpacing.X;
			rectangleF.Y += plotAreaSpacing.Y;
			rectangleF.Width -= plotAreaSpacing.X + plotAreaSpacing.Width;
			rectangleF.Height -= plotAreaSpacing.Y + plotAreaSpacing.Height;
			if (area.Area3DStyle.Enable3D)
			{
				RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(rectangleF);
				Series dataSeries = GetDataSeries();
				if (dataSeries != null)
				{
					rotation3D = GetFunnelRotation(dataSeries);
				}
				float num = (float)Math.Abs((double)(absoluteRectangle.Width / 2f) * Math.Sin((double)((float)rotation3D / 180f) * Math.PI));
				float num2 = (float)Math.Abs((double)(absoluteRectangle.Width / 2f) * Math.Sin((double)((float)rotation3D / 180f) * Math.PI));
				if (isPyramid)
				{
					absoluteRectangle.Height -= num2;
				}
				else
				{
					absoluteRectangle.Y += num;
					absoluteRectangle.Height -= num + num2;
				}
				rectangleF = graph.GetRelativeRectangle(absoluteRectangle);
			}
			return rectangleF;
		}

		protected float CheckMinHeight(float height)
		{
			float num = Math.Min(2f, funnelSegmentGap / 2f);
			if (funnelSegmentGap > 0f && height < num)
			{
				return num;
			}
			return height;
		}

		private void GetFunnelMinPointHeight(DataPointAttributes attributes)
		{
			funnelMinPointHeight = 0f;
			string text = attributes[funnelPointMinHeight];
			if (text != null && text.Length > 0)
			{
				try
				{
					funnelMinPointHeight = float.Parse(text, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, funnelPointMinHeight));
				}
				if (funnelMinPointHeight < 0f || funnelMinPointHeight > 100f)
				{
					throw new InvalidOperationException(SR.ExceptionFunnelMinimumPointHeightAttributeInvalid);
				}
				funnelMinPointHeight = (float)(yValueTotal * (double)funnelMinPointHeight / 100.0);
				GetDataPointValuesStatistic();
			}
		}

		private int GetFunnelRotation(DataPointAttributes attributes)
		{
			int num = 5;
			string text = attributes[funnelRotationAngleAttributeName];
			if (text != null && text.Length > 0)
			{
				try
				{
					num = int.Parse(text, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, funnelRotationAngleAttributeName));
				}
				if (num < -10 || num > 10)
				{
					throw new InvalidOperationException(SR.ExceptionFunnelAngleRangeInvalid);
				}
			}
			return num;
		}

		private Color GetCalloutLineColor(DataPointAttributes attributes)
		{
			Color result = Color.Black;
			string text = attributes["CalloutLineColor"];
			if (text != null && text.Length > 0)
			{
				bool flag = false;
				ColorConverter colorConverter = new ColorConverter();
				try
				{
					result = (Color)colorConverter.ConvertFromInvariantString(text);
				}
				catch
				{
					flag = true;
				}
				if (flag)
				{
					try
					{
						return (Color)colorConverter.ConvertFromString(text);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "CalloutLineColor"));
					}
				}
			}
			return result;
		}

		private SizeF GetFunnelNeckSize(DataPointAttributes attributes)
		{
			SizeF relative = new SizeF(5f, 5f);
			string text = attributes["FunnelNeckWidth"];
			if (text != null && text.Length > 0)
			{
				try
				{
					relative.Width = float.Parse(text, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "FunnelNeckWidth"));
				}
				if (relative.Width < 0f || relative.Width > 100f)
				{
					throw new InvalidOperationException(SR.ExceptionFunnelNeckWidthInvalid);
				}
			}
			text = attributes["FunnelNeckHeight"];
			if (text != null && text.Length > 0)
			{
				try
				{
					relative.Height = float.Parse(text, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "FunnelNeckHeight"));
				}
				if (relative.Height < 0f || relative.Height > 100f)
				{
					throw new InvalidOperationException(SR.ExceptionFunnelNeckHeightInvalid);
				}
			}
			if (relative.Height > plotAreaPosition.Height / 2f)
			{
				relative.Height = plotAreaPosition.Height / 2f;
			}
			if (relative.Width > plotAreaPosition.Width / 2f)
			{
				relative.Width = plotAreaPosition.Width / 2f;
			}
			return graph.GetAbsoluteSize(relative);
		}

		private float GetFunnelPointGap(DataPointAttributes attributes)
		{
			float result = 0f;
			string text = attributes[funnelPointGapAttributeName];
			if (text != null && text.Length > 0)
			{
				try
				{
					result = float.Parse(text, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, funnelPointGapAttributeName));
				}
				float num = plotAreaPosition.Height / (float)(pointNumber - (ShouldDrawFirstPoint() ? 1 : 2));
				if (result > num)
				{
					result = num;
				}
				if (result < 0f)
				{
					result = 0f;
				}
				result = graph.GetAbsoluteSize(new SizeF(result, result)).Height;
			}
			return result;
		}

		private FunnelStyle GetFunnelStyle(DataPointAttributes attributes)
		{
			FunnelStyle result = FunnelStyle.YIsHeight;
			if (!isPyramid)
			{
				string text = attributes["FunnelStyle"];
				if (text != null && text.Length > 0)
				{
					try
					{
						return (FunnelStyle)Enum.Parse(typeof(FunnelStyle), text, ignoreCase: true);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "FunnelStyle"));
					}
				}
			}
			return result;
		}

		private FunnelLabelPlacement GetOutsideLabelPlacement(DataPointAttributes attributes)
		{
			FunnelLabelPlacement result = FunnelLabelPlacement.Right;
			string text = attributes[funnelOutsideLabelPlacementAttributeName];
			if (text != null && text.Length > 0)
			{
				try
				{
					return (FunnelLabelPlacement)Enum.Parse(typeof(FunnelLabelPlacement), text, ignoreCase: true);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, funnelOutsideLabelPlacementAttributeName));
				}
			}
			return result;
		}

		private FunnelLabelVerticalAlignment GetInsideLabelAlignment(DataPointAttributes attributes)
		{
			FunnelLabelVerticalAlignment result = FunnelLabelVerticalAlignment.Center;
			string text = attributes[funnelInsideLabelAlignmentAttributeName];
			if (text != null && text.Length > 0)
			{
				try
				{
					return (FunnelLabelVerticalAlignment)Enum.Parse(typeof(FunnelLabelVerticalAlignment), text, ignoreCase: true);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, funnelInsideLabelAlignmentAttributeName));
				}
			}
			return result;
		}

		private Funnel3DDrawingStyle GetFunnel3DDrawingStyle(DataPointAttributes attributes)
		{
			Funnel3DDrawingStyle result = isPyramid ? Funnel3DDrawingStyle.SquareBase : Funnel3DDrawingStyle.CircularBase;
			string text = attributes[funnel3DDrawingStyleAttributeName];
			if (text != null && text.Length > 0)
			{
				try
				{
					return (Funnel3DDrawingStyle)Enum.Parse(typeof(Funnel3DDrawingStyle), text, ignoreCase: true);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, funnel3DDrawingStyleAttributeName));
				}
			}
			return result;
		}

		private void GetDataPointValuesStatistic()
		{
			Series dataSeries = GetDataSeries();
			if (dataSeries == null)
			{
				return;
			}
			yValueTotal = 0.0;
			xValueTotal = 0.0;
			yValueMax = 0.0;
			pointNumber = 0;
			valuePercentages = null;
			if (GetPyramidValueType(dataSeries) == PyramidValueType.Surface)
			{
				double num = 0.0;
				int num2 = 0;
				foreach (DataPoint point in dataSeries.Points)
				{
					if (!point.Empty)
					{
						num += GetYValue(point, num2);
					}
					num2++;
				}
				double num3 = 100.0;
				double num4 = 2.0 * num / num3 / num3;
				double[] array = new double[dataSeries.Points.Count];
				double num5 = 0.0;
				for (int i = 0; i < array.Length; i++)
				{
					double yValue = GetYValue(dataSeries.Points[i], i);
					num5 += yValue;
					array[i] = Math.Sqrt(2.0 * num5 / num4);
				}
				valuePercentages = array;
			}
			foreach (DataPoint point2 in dataSeries.Points)
			{
				if (!point2.Empty)
				{
					double yValue2 = GetYValue(point2, pointNumber);
					yValueTotal += yValue2;
					yValueMax = Math.Max(yValueMax, yValue2);
					xValueTotal += GetXValue(point2);
				}
				pointNumber++;
			}
		}

		private Series GetDataSeries()
		{
			if (chartTypeSeries == null)
			{
				Series series = null;
				foreach (Series item in common.DataManager.Series)
				{
					if (!item.IsVisible() || !(item.ChartArea == area.Name))
					{
						continue;
					}
					if (string.Compare(item.ChartTypeName, Name, ignoreCase: true, CultureInfo.CurrentCulture) == 0)
					{
						if (series == null)
						{
							series = item;
						}
					}
					else if (!common.ChartPicture.SuppressExceptions)
					{
						throw new InvalidOperationException(SR.ExceptionFunnelCanNotCombine);
					}
				}
				chartTypeSeries = series;
			}
			return chartTypeSeries;
		}

		private PyramidValueType GetPyramidValueType(DataPointAttributes attributes)
		{
			PyramidValueType result = PyramidValueType.Linear;
			if (isPyramid)
			{
				string text = attributes["PyramidValueType"];
				if (text != null && text.Length > 0)
				{
					try
					{
						return (PyramidValueType)Enum.Parse(typeof(PyramidValueType), text, ignoreCase: true);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "PyramidValueType"));
					}
				}
			}
			return result;
		}

		public virtual double GetYValue(DataPoint point, int pointIndex)
		{
			double num = 0.0;
			if (!point.Empty)
			{
				num = point.YValues[0];
				if (valuePercentages != null && valuePercentages.Length > pointIndex)
				{
					num = num / 100.0 * valuePercentages[pointIndex];
				}
				if (area.AxisY.Logarithmic)
				{
					num = Math.Abs(Math.Log(num, area.AxisY.LogarithmBase));
				}
				else
				{
					num = Math.Abs(num);
					if (num < (double)funnelMinPointHeight)
					{
						num = funnelMinPointHeight;
					}
				}
			}
			return num;
		}

		public virtual double GetXValue(DataPoint point)
		{
			if (area.AxisX.Logarithmic)
			{
				return Math.Abs(Math.Log(point.XValue, area.AxisX.LogarithmBase));
			}
			return Math.Abs(point.XValue);
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
