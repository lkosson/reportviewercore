using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class FastLineChart : IChartType
	{
		internal bool chartArea3DEnabled;

		internal ChartGraphics graph;

		internal float seriesZCoordinate;

		internal Matrix3D matrix3D;

		internal CommonElements common;

		public virtual string Name => "FastLine";

		public virtual bool Stacked => false;

		public virtual bool SupportStackedGroups => false;

		public bool StackSign => false;

		public virtual bool RequireAxes => true;

		public virtual bool SecondYScale => false;

		public bool CircularChartArea => false;

		public virtual bool SupportLogarithmicAxes => true;

		public virtual bool SwitchValueAxes => false;

		public virtual bool SideBySideSeries => false;

		public virtual bool DataPointsInLegend => false;

		public virtual bool ZeroCrossing => false;

		public virtual bool ApplyPaletteColorsToPoints => false;

		public virtual bool ExtraYValuesConnectedToYAxis => false;

		public virtual bool HundredPercent => false;

		public virtual bool HundredPercentSupportNegative => false;

		public virtual int YValuesPerPoint => 1;

		public virtual LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Line;
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public virtual void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.common = common;
			this.graph = graph;
			bool flag = false;
			if (area.Area3DStyle.Enable3D)
			{
				chartArea3DEnabled = true;
				matrix3D = area.matrix3D;
			}
			else
			{
				chartArea3DEnabled = false;
			}
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, Name, ignoreCase: true, CultureInfo.CurrentCulture) != 0 || item.ChartArea != area.Name || !item.IsVisible())
				{
					continue;
				}
				if (chartArea3DEnabled)
				{
					area.GetSeriesZPositionAndDepth(item, out float depth, out seriesZCoordinate);
					seriesZCoordinate += depth / 2f;
				}
				Axis axis = area.GetAxis(AxisName.X, item.XAxisType, area.Area3DStyle.Enable3D ? string.Empty : item.XSubAxisName);
				Axis axis2 = area.GetAxis(AxisName.Y, item.YAxisType, area.Area3DStyle.Enable3D ? string.Empty : item.YSubAxisName);
				double viewMinimum = axis.GetViewMinimum();
				double viewMaximum = axis.GetViewMaximum();
				double viewMinimum2 = axis2.GetViewMinimum();
				double viewMaximum2 = axis2.GetViewMaximum();
				float num = 1f;
				if (item.IsAttributeSet("PixelPointGapDepth"))
				{
					string s = item["PixelPointGapDepth"];
					try
					{
						num = float.Parse(s, CultureInfo.CurrentCulture);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("PermittedPixelError"));
					}
					if (num < 0f || num > 1f)
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to1("PermittedPixelError"));
					}
				}
				SizeF relativeSize = graph.GetRelativeSize(new SizeF(num, num));
				SizeF relativeSize2 = graph.GetRelativeSize(new SizeF((float)viewMinimum, (float)viewMinimum2));
				double num2 = Math.Abs(axis.PositionToValue(relativeSize2.Width + relativeSize.Width, validateInput: false) - axis.PositionToValue(relativeSize2.Width, validateInput: false));
				Math.Abs(axis2.PositionToValue(relativeSize2.Height + relativeSize.Height, validateInput: false) - axis2.PositionToValue(relativeSize2.Height, validateInput: false));
				Pen pen = new Pen(item.Color, item.BorderWidth);
				pen.DashStyle = graph.GetPenStyle(item.BorderStyle);
				pen.StartCap = LineCap.Round;
				pen.EndCap = LineCap.Round;
				Pen pen2 = new Pen(item.EmptyPointStyle.Color, item.EmptyPointStyle.BorderWidth);
				pen2.DashStyle = graph.GetPenStyle(item.EmptyPointStyle.BorderStyle);
				pen2.StartCap = LineCap.Round;
				pen2.EndCap = LineCap.Round;
				bool flag2 = area.IndexedSeries(item.Name);
				int num3 = 0;
				double num4 = double.NaN;
				double num5 = double.NaN;
				DataPoint pointMin = null;
				DataPoint pointMax = null;
				double num6 = 0.0;
				double num7 = 0.0;
				double num8 = 0.0;
				double num9 = 0.0;
				DataPoint dataPoint = null;
				PointF empty = PointF.Empty;
				PointF pointF = PointF.Empty;
				PointF empty2 = PointF.Empty;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				bool flag7 = false;
				double num10 = ((double)graph.common.ChartPicture.Width - 1.0) / 100.0;
				double num11 = ((double)graph.common.ChartPicture.Height - 1.0) / 100.0;
				foreach (DataPoint point in item.Points)
				{
					num6 = (flag2 ? ((double)(num3 + 1)) : point.XValue);
					num6 = axis.GetLogValue(num6);
					num7 = axis2.GetLogValue(point.YValues[0]);
					flag6 = point.Empty;
					if (flag5 && !flag6 && !flag7)
					{
						flag7 = true;
						flag6 = true;
					}
					else
					{
						flag7 = false;
					}
					if (!flag4 && ((num6 < viewMinimum && num8 < viewMinimum) || (num6 > viewMaximum && num8 > viewMaximum) || (num7 < viewMinimum2 && num9 < viewMinimum2) || (num7 > viewMaximum2 && num9 > viewMaximum2)))
					{
						num8 = num6;
						num9 = num7;
						flag3 = true;
						num3++;
						continue;
					}
					if (!flag && (num8 < viewMinimum || num8 > viewMaximum || num6 > viewMaximum || num6 < viewMinimum || num9 < viewMinimum2 || num9 > viewMaximum2 || num7 < viewMinimum2 || num7 > viewMaximum2))
					{
						graph.SetClip(area.PlotAreaPosition.ToRectangleF());
						flag = true;
					}
					if (num3 > 0 && flag6 == flag5 && Math.Abs(num6 - num8) < num2)
					{
						if (!flag4)
						{
							flag4 = true;
							if (num7 > num9)
							{
								num5 = num7;
								num4 = num9;
								pointMax = point;
								pointMin = dataPoint;
							}
							else
							{
								num5 = num9;
								num4 = num7;
								pointMax = dataPoint;
								pointMin = point;
							}
						}
						else if (num7 > num5)
						{
							num5 = num7;
							pointMax = point;
						}
						else if (num7 < num4)
						{
							num4 = num7;
							pointMin = point;
						}
						dataPoint = point;
						empty.Y = (float)num7;
						num3++;
						continue;
					}
					empty2.X = (float)(axis.GetLinearPosition(num6) * num10);
					empty2.Y = (float)(axis2.GetLinearPosition(num7) * num11);
					if (flag3)
					{
						pointF.X = (float)(axis.GetLinearPosition(num8) * num10);
						pointF.Y = (float)(axis2.GetLinearPosition(num9) * num11);
					}
					if (flag4)
					{
						num4 = axis2.GetLinearPosition(num4) * num11;
						num5 = axis2.GetLinearPosition(num5) * num11;
						DrawLine(item, dataPoint, pointMin, pointMax, num3, flag5 ? pen2 : pen, pointF.X, (float)num4, pointF.X, (float)num5);
						flag4 = false;
						pointF.Y = (float)(axis2.GetLinearPosition(empty.Y) * num11);
					}
					if (num3 > 0)
					{
						DrawLine(item, point, pointMin, pointMax, num3, flag6 ? pen2 : pen, pointF.X, pointF.Y, empty2.X, empty2.Y);
					}
					num8 = num6;
					num9 = num7;
					dataPoint = point;
					pointF = empty2;
					flag3 = false;
					flag5 = flag6;
					num3++;
				}
				if (flag4)
				{
					if (flag3)
					{
						pointF.X = (float)(axis.GetLinearPosition(num8) * num10);
						pointF.Y = (float)(axis2.GetLinearPosition(num9) * num11);
					}
					num4 = axis2.GetLinearPosition(num4) * num11;
					num5 = axis2.GetLinearPosition(num5) * num11;
					DrawLine(item, dataPoint, pointMin, pointMax, num3 - 1, flag5 ? pen2 : pen, pointF.X, (float)num4, pointF.X, (float)num5);
					flag4 = false;
					num4 = double.NaN;
					num5 = double.NaN;
					pointMin = null;
					pointMax = null;
				}
			}
			if (flag)
			{
				graph.ResetClip();
			}
		}

		public virtual void DrawLine(Series series, DataPoint point, DataPoint pointMin, DataPoint pointMax, int pointIndex, Pen pen, float firstPointX, float firstPointY, float secondPointX, float secondPointY)
		{
			if (chartArea3DEnabled)
			{
				Point3D[] array = new Point3D[2];
				PointF relativePoint = graph.GetRelativePoint(new PointF(firstPointX, firstPointY));
				PointF relativePoint2 = graph.GetRelativePoint(new PointF(secondPointX, secondPointY));
				array[0] = new Point3D(relativePoint.X, relativePoint.Y, seriesZCoordinate);
				array[1] = new Point3D(relativePoint2.X, relativePoint2.Y, seriesZCoordinate);
				matrix3D.TransformPoints(array);
				array[0].PointF = graph.GetAbsolutePoint(array[0].PointF);
				array[1].PointF = graph.GetAbsolutePoint(array[1].PointF);
				firstPointX = array[0].X;
				firstPointY = array[0].Y;
				secondPointX = array[1].X;
				secondPointY = array[1].Y;
			}
			graph.DrawLine(pen, firstPointX, firstPointY, secondPointX, secondPointY);
			if (common.ProcessModeRegions)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				float num = pen.Width + 2f;
				if (Math.Abs(firstPointX - secondPointX) > Math.Abs(firstPointY - secondPointY))
				{
					graphicsPath.AddLine(firstPointX, firstPointY - num, secondPointX, secondPointY - num);
					graphicsPath.AddLine(secondPointX, secondPointY + num, firstPointX, firstPointY + num);
					graphicsPath.CloseAllFigures();
				}
				else
				{
					graphicsPath.AddLine(firstPointX - num, firstPointY, secondPointX - num, secondPointY);
					graphicsPath.AddLine(secondPointX + num, secondPointY, firstPointX + num, firstPointY);
					graphicsPath.CloseAllFigures();
				}
				RectangleF bounds = graphicsPath.GetBounds();
				if ((double)bounds.Width <= 2.0 || (double)bounds.Height <= 2.0)
				{
					bounds.Inflate(pen.Width, pen.Width);
					common.HotRegionsList.AddHotRegion(graph, graph.GetRelativeRectangle(bounds), point, point.series.Name, pointIndex);
				}
				else
				{
					common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: false, graph, point, point.series.Name, pointIndex);
				}
			}
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
