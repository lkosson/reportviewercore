using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class FastPointChart : IChartType
	{
		internal bool chartArea3DEnabled;

		internal ChartGraphics graph;

		internal float seriesZCoordinate;

		internal Matrix3D matrix3D;

		internal CommonElements common;

		public virtual string Name => "FastPoint";

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
			return LegendImageStyle.Marker;
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public virtual void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.common = common;
			this.graph = graph;
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
				float num = (float)item.MarkerSize / 3f;
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
					if (num < 0f || num > 50f)
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to50("PermittedPixelError"));
					}
				}
				SizeF relativeSize = graph.GetRelativeSize(new SizeF(num, num));
				SizeF relativeSize2 = graph.GetRelativeSize(new SizeF((float)viewMinimum, (float)viewMinimum2));
				double num2 = Math.Abs(axis.PositionToValue(relativeSize2.Width + relativeSize.Width, validateInput: false) - axis.PositionToValue(relativeSize2.Width, validateInput: false));
				double num3 = Math.Abs(axis2.PositionToValue(relativeSize2.Height + relativeSize.Height, validateInput: false) - axis2.PositionToValue(relativeSize2.Height, validateInput: false));
				SolidBrush solidBrush = new SolidBrush(item.MarkerColor.IsEmpty ? item.Color : item.MarkerColor);
				SolidBrush solidBrush2 = new SolidBrush(item.EmptyPointStyle.MarkerColor.IsEmpty ? item.EmptyPointStyle.Color : item.EmptyPointStyle.MarkerColor);
				Pen pen = null;
				Pen pen2 = null;
				if (!item.MarkerBorderColor.IsEmpty && item.MarkerBorderWidth > 0)
				{
					pen = new Pen(item.MarkerBorderColor, item.MarkerBorderWidth);
				}
				if (!item.EmptyPointStyle.MarkerBorderColor.IsEmpty && item.EmptyPointStyle.MarkerBorderWidth > 0)
				{
					pen2 = new Pen(item.EmptyPointStyle.MarkerBorderColor, item.EmptyPointStyle.MarkerBorderWidth);
				}
				bool flag = area.IndexedSeries(item.Name);
				int num4 = 0;
				double num5 = 0.0;
				double num6 = 0.0;
				double num7 = 0.0;
				double num8 = 0.0;
				PointF empty = PointF.Empty;
				bool flag2 = false;
				double num9 = ((double)graph.common.ChartPicture.Width - 1.0) / 100.0;
				double num10 = ((double)graph.common.ChartPicture.Height - 1.0) / 100.0;
				int markerSize = item.MarkerSize;
				MarkerStyle markerStyle = item.MarkerStyle;
				MarkerStyle markerStyle2 = item.EmptyPointStyle.MarkerStyle;
				foreach (DataPoint point in item.Points)
				{
					num5 = (flag ? ((double)(num4 + 1)) : point.XValue);
					num5 = axis.GetLogValue(num5);
					num6 = axis2.GetLogValue(point.YValues[0]);
					flag2 = point.Empty;
					if (num5 < viewMinimum || num5 > viewMaximum || num6 < viewMinimum2 || num6 > viewMaximum2)
					{
						num7 = num5;
						num8 = num6;
						num4++;
						continue;
					}
					if (num4 > 0 && Math.Abs(num5 - num7) < num2 && Math.Abs(num6 - num8) < num3)
					{
						num4++;
						continue;
					}
					empty.X = (float)(axis.GetLinearPosition(num5) * num9);
					empty.Y = (float)(axis2.GetLinearPosition(num6) * num10);
					DrawMarker(graph, point, num4, empty, flag2 ? markerStyle2 : markerStyle, markerSize, flag2 ? solidBrush2 : solidBrush, flag2 ? pen2 : pen);
					num7 = num5;
					num8 = num6;
					num4++;
				}
				solidBrush.Dispose();
				solidBrush2.Dispose();
				pen?.Dispose();
				pen2?.Dispose();
			}
		}

		protected virtual void DrawMarker(ChartGraphics graph, DataPoint point, int pointIndex, PointF location, MarkerStyle markerStyle, int markerSize, Brush brush, Pen borderPen)
		{
			if (chartArea3DEnabled)
			{
				Point3D[] array = new Point3D[1];
				location = graph.GetRelativePoint(location);
				array[0] = new Point3D(location.X, location.Y, seriesZCoordinate);
				matrix3D.TransformPoints(array);
				location.X = array[0].X;
				location.Y = array[0].Y;
				location = graph.GetAbsolutePoint(location);
			}
			RectangleF rectangleF = new RectangleF(location.X - (float)markerSize / 2f, location.Y - (float)markerSize / 2f, markerSize, markerSize);
			switch (markerStyle)
			{
			case MarkerStyle.Star4:
			case MarkerStyle.Star5:
			case MarkerStyle.Star6:
			case MarkerStyle.Star10:
			{
				int numberOfCorners = 4;
				switch (markerStyle)
				{
				case MarkerStyle.Star5:
					numberOfCorners = 5;
					break;
				case MarkerStyle.Star6:
					numberOfCorners = 6;
					break;
				case MarkerStyle.Star10:
					numberOfCorners = 10;
					break;
				}
				PointF[] points = graph.CreateStarPolygon(rectangleF, numberOfCorners);
				graph.FillPolygon(brush, points);
				if (borderPen != null)
				{
					graph.DrawPolygon(borderPen, points);
				}
				break;
			}
			case MarkerStyle.Circle:
				graph.FillEllipse(brush, rectangleF);
				if (borderPen != null)
				{
					graph.DrawEllipse(borderPen, rectangleF);
				}
				break;
			case MarkerStyle.Square:
				graph.FillRectangle(brush, rectangleF);
				if (borderPen != null)
				{
					graph.DrawRectangle(borderPen, (int)Math.Round(rectangleF.X, 0), (int)Math.Round(rectangleF.Y, 0), (int)Math.Round(rectangleF.Width, 0), (int)Math.Round(rectangleF.Height, 0));
				}
				break;
			case MarkerStyle.Cross:
			{
				float num = (float)Math.Ceiling((float)markerSize / 4f);
				float num2 = markerSize;
				PointF[] array4 = new PointF[12];
				array4[0].X = location.X - num2 / 2f;
				array4[0].Y = location.Y + num / 2f;
				array4[1].X = location.X - num2 / 2f;
				array4[1].Y = location.Y - num / 2f;
				array4[2].X = location.X - num / 2f;
				array4[2].Y = location.Y - num / 2f;
				array4[3].X = location.X - num / 2f;
				array4[3].Y = location.Y - num2 / 2f;
				array4[4].X = location.X + num / 2f;
				array4[4].Y = location.Y - num2 / 2f;
				array4[5].X = location.X + num / 2f;
				array4[5].Y = location.Y - num / 2f;
				array4[6].X = location.X + num2 / 2f;
				array4[6].Y = location.Y - num / 2f;
				array4[7].X = location.X + num2 / 2f;
				array4[7].Y = location.Y + num / 2f;
				array4[8].X = location.X + num / 2f;
				array4[8].Y = location.Y + num / 2f;
				array4[9].X = location.X + num / 2f;
				array4[9].Y = location.Y + num2 / 2f;
				array4[10].X = location.X - num / 2f;
				array4[10].Y = location.Y + num2 / 2f;
				array4[11].X = location.X - num / 2f;
				array4[11].Y = location.Y + num / 2f;
				Matrix matrix = new Matrix();
				matrix.RotateAt(45f, location);
				matrix.TransformPoints(array4);
				matrix.Dispose();
				graph.FillPolygon(brush, array4);
				if (borderPen != null)
				{
					graph.DrawPolygon(borderPen, array4);
				}
				break;
			}
			case MarkerStyle.Diamond:
			{
				PointF[] array3 = new PointF[4];
				array3[0].X = rectangleF.X;
				array3[0].Y = rectangleF.Y + rectangleF.Height / 2f;
				array3[1].X = rectangleF.X + rectangleF.Width / 2f;
				array3[1].Y = rectangleF.Top;
				array3[2].X = rectangleF.Right;
				array3[2].Y = rectangleF.Y + rectangleF.Height / 2f;
				array3[3].X = rectangleF.X + rectangleF.Width / 2f;
				array3[3].Y = rectangleF.Bottom;
				graph.FillPolygon(brush, array3);
				if (borderPen != null)
				{
					graph.DrawPolygon(borderPen, array3);
				}
				break;
			}
			case MarkerStyle.Triangle:
			{
				PointF[] array2 = new PointF[3];
				array2[0].X = rectangleF.X;
				array2[0].Y = rectangleF.Bottom;
				array2[1].X = rectangleF.X + rectangleF.Width / 2f;
				array2[1].Y = rectangleF.Top;
				array2[2].X = rectangleF.Right;
				array2[2].Y = rectangleF.Bottom;
				graph.FillPolygon(brush, array2);
				if (borderPen != null)
				{
					graph.DrawPolygon(borderPen, array2);
				}
				break;
			}
			default:
				throw new InvalidOperationException(SR.ExceptionFastPointMarkerStyleUnknown);
			}
			if (common.ProcessModeRegions)
			{
				common.HotRegionsList.AddHotRegion(graph, graph.GetRelativeRectangle(rectangleF), point, point.series.Name, pointIndex);
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
