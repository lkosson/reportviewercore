using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class RadarChart : IChartType, ICircularChartType
	{
		protected CommonElements common;

		protected ChartArea area;

		protected bool autoLabelPosition = true;

		protected LabelAlignmentTypes labelPosition = LabelAlignmentTypes.Top;

		public virtual string Name => "Radar";

		public virtual bool Stacked => false;

		public virtual bool SupportStackedGroups => false;

		public bool StackSign => false;

		public virtual bool RequireAxes => true;

		public bool SecondYScale => false;

		public bool CircularChartArea => true;

		public virtual bool SupportLogarithmicAxes => false;

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
			if (series != null)
			{
				switch (GetDrawingStyle(series, new DataPoint(series)))
				{
				case RadarDrawingStyle.Line:
					return LegendImageStyle.Line;
				case RadarDrawingStyle.Marker:
					return LegendImageStyle.Marker;
				}
			}
			return LegendImageStyle.Rectangle;
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public virtual bool RequireClosedFigure()
		{
			return true;
		}

		public virtual bool XAxisCrossingSupported()
		{
			return false;
		}

		public virtual bool XAxisLabelsSupported()
		{
			return false;
		}

		public virtual bool RadialGridLinesSupported()
		{
			return false;
		}

		public virtual int GetNumerOfSectors(ChartArea area, SeriesCollection seriesCollection)
		{
			int num = 0;
			foreach (Series item in seriesCollection)
			{
				if (item.IsVisible() && item.ChartArea == area.Name)
				{
					num = Math.Max(item.Points.Count, num);
				}
			}
			return num;
		}

		public virtual float[] GetYAxisLocations(ChartArea area)
		{
			float[] array = new float[area.CircularSectorsNumber];
			float num = 360f / (float)array.Length;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = num * (float)i;
			}
			return array;
		}

		public virtual void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.common = common;
			this.area = area;
			ProcessChartType(selection: false, graph, common, area, seriesToDraw);
		}

		protected virtual void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			foreach (Series item in common.DataManager.Series)
			{
				if (item.ChartArea != area.Name || !item.IsVisible())
				{
					continue;
				}
				if (string.Compare(item.ChartTypeName, Name, ignoreCase: true, CultureInfo.CurrentCulture) != 0)
				{
					throw new InvalidOperationException(SR.ExceptionChartTypeCanNotCombine(item.ChartTypeName, Name));
				}
				if (!selection)
				{
					common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				Axis axis = area.GetAxis(AxisName.Y, AxisType.Primary, item.YSubAxisName);
				double viewMinimum = axis.GetViewMinimum();
				double viewMaximum = axis.GetViewMaximum();
				PointF[] pointsPosition = GetPointsPosition(graph, area, item);
				int num = 0;
				if (item.ShadowOffset != 0 && !selection)
				{
					graph.shadowDrawingMode = true;
					foreach (DataPoint point in item.Points)
					{
						int num2 = num + 1;
						if (num2 >= item.Points.Count)
						{
							num2 = 0;
						}
						DataPointAttributes dataPointAttributes = point;
						if (item.Points[num2].Empty)
						{
							dataPointAttributes = item.Points[num2];
						}
						Color left = dataPointAttributes.Color;
						_ = dataPointAttributes.BorderColor;
						int borderWidth = dataPointAttributes.BorderWidth;
						ChartDashStyle borderStyle = dataPointAttributes.BorderStyle;
						RadarDrawingStyle drawingStyle = GetDrawingStyle(item, point);
						if (axis.GetLogValue(point.YValues[0]) > viewMaximum || axis.GetLogValue(point.YValues[0]) < viewMinimum || axis.GetLogValue(item.Points[num2].YValues[0]) > viewMaximum || axis.GetLogValue(item.Points[num2].YValues[0]) < viewMinimum)
						{
							num++;
							continue;
						}
						switch (drawingStyle)
						{
						case RadarDrawingStyle.Line:
							_ = dataPointAttributes.Color;
							borderWidth = ((borderWidth < 1) ? 1 : borderWidth);
							borderStyle = ((borderStyle == ChartDashStyle.NotSet) ? ChartDashStyle.Solid : borderStyle);
							left = Color.Transparent;
							break;
						case RadarDrawingStyle.Marker:
							left = Color.Transparent;
							break;
						}
						if (num2 == 0 && !RequireClosedFigure() && drawingStyle != 0)
						{
							break;
						}
						if (left != Color.Transparent && left != Color.Empty && item.ShadowOffset != 0)
						{
							GraphicsPath graphicsPath = new GraphicsPath();
							graphicsPath.AddLine(graph.GetAbsolutePoint(area.circularCenter), pointsPosition[num]);
							graphicsPath.AddLine(pointsPosition[num], pointsPosition[num2]);
							graphicsPath.AddLine(pointsPosition[num2], graph.GetAbsolutePoint(area.circularCenter));
							Matrix matrix = new Matrix();
							matrix.Translate(item.ShadowOffset, item.ShadowOffset);
							graphicsPath.Transform(matrix);
							graph.FillPath(new SolidBrush(item.ShadowColor), graphicsPath);
						}
						num++;
					}
					graph.shadowDrawingMode = false;
				}
				num = 0;
				foreach (DataPoint point2 in item.Points)
				{
					point2.positionRel = graph.GetRelativePoint(pointsPosition[num]);
					int num3 = num + 1;
					if (num3 >= item.Points.Count)
					{
						num3 = 0;
					}
					DataPointAttributes dataPointAttributes2 = point2;
					if (item.Points[num3].Empty)
					{
						dataPointAttributes2 = item.Points[num3];
					}
					Color color = dataPointAttributes2.Color;
					Color color2 = dataPointAttributes2.BorderColor;
					int num4 = dataPointAttributes2.BorderWidth;
					ChartDashStyle chartDashStyle = dataPointAttributes2.BorderStyle;
					RadarDrawingStyle drawingStyle2 = GetDrawingStyle(item, point2);
					if (axis.GetLogValue(point2.YValues[0]) > viewMaximum || axis.GetLogValue(point2.YValues[0]) < viewMinimum || axis.GetLogValue(item.Points[num3].YValues[0]) > viewMaximum || axis.GetLogValue(item.Points[num3].YValues[0]) < viewMinimum)
					{
						num++;
						continue;
					}
					switch (drawingStyle2)
					{
					case RadarDrawingStyle.Line:
						color2 = dataPointAttributes2.Color;
						num4 = ((num4 < 1) ? 1 : num4);
						chartDashStyle = ((chartDashStyle == ChartDashStyle.NotSet) ? ChartDashStyle.Solid : chartDashStyle);
						color = Color.Transparent;
						break;
					case RadarDrawingStyle.Marker:
						color = Color.Transparent;
						break;
					}
					GraphicsPath graphicsPath2 = new GraphicsPath();
					if (num3 == 0 && !RequireClosedFigure() && drawingStyle2 != 0)
					{
						if (common.ProcessModeRegions)
						{
							AddSelectionPath(area, graphicsPath2, pointsPosition, num, num3, graph.GetAbsolutePoint(area.circularCenter), 0);
							int insertIndex = common.HotRegionsList.FindInsertIndex();
							common.HotRegionsList.AddHotRegion(insertIndex, graphicsPath2, relativePath: false, graph, point2, item.Name, num);
						}
						break;
					}
					if (color != Color.Transparent && color != Color.Empty)
					{
						GraphicsPath graphicsPath3 = new GraphicsPath();
						graphicsPath3.AddLine(graph.GetAbsolutePoint(area.circularCenter), pointsPosition[num]);
						graphicsPath3.AddLine(pointsPosition[num], pointsPosition[num3]);
						graphicsPath3.AddLine(pointsPosition[num3], graph.GetAbsolutePoint(area.circularCenter));
						if (common.ProcessModePaint)
						{
							Brush brush = graph.CreateBrush(graphicsPath3.GetBounds(), color, dataPointAttributes2.BackHatchStyle, dataPointAttributes2.BackImage, dataPointAttributes2.BackImageMode, dataPointAttributes2.BackImageTransparentColor, dataPointAttributes2.BackImageAlign, dataPointAttributes2.BackGradientType, dataPointAttributes2.BackGradientEndColor);
							graph.StartHotRegion(point2);
							graph.StartAnimation();
							graph.FillPath(brush, graphicsPath3);
							graph.StopAnimation();
							graph.EndHotRegion();
						}
						if (common.ProcessModeRegions)
						{
							AddSelectionPath(area, graphicsPath2, pointsPosition, num, num3, graph.GetAbsolutePoint(area.circularCenter), 0);
						}
					}
					if (color2 != Color.Empty && num4 > 0 && chartDashStyle != 0 && num3 < item.Points.Count)
					{
						if (common.ProcessModePaint)
						{
							graph.StartHotRegion(point2);
							graph.StartAnimation();
							graph.DrawLineAbs(color2, num4, chartDashStyle, pointsPosition[num], pointsPosition[num3], item.ShadowColor, (color == Color.Transparent || color == Color.Empty) ? item.ShadowOffset : 0);
							graph.StopAnimation();
							graph.EndHotRegion();
						}
						if (common.ProcessModeRegions)
						{
							AddSelectionPath(area, graphicsPath2, pointsPosition, num, num3, PointF.Empty, num4);
						}
					}
					if (common.ProcessModeRegions)
					{
						int insertIndex2 = common.HotRegionsList.FindInsertIndex();
						common.HotRegionsList.AddHotRegion(insertIndex2, graphicsPath2, relativePath: false, graph, point2, item.Name, num);
					}
					num++;
				}
				int num5 = 0;
				num = 0;
				foreach (DataPoint point3 in item.Points)
				{
					Color markerColor = point3.MarkerColor;
					MarkerStyle markerStyle = point3.MarkerStyle;
					RadarDrawingStyle drawingStyle3 = GetDrawingStyle(item, point3);
					if (axis.GetLogValue(point3.YValues[0]) > viewMaximum || axis.GetLogValue(point3.YValues[0]) < viewMinimum)
					{
						num++;
						continue;
					}
					if (drawingStyle3 == RadarDrawingStyle.Marker && markerColor.IsEmpty)
					{
						markerColor = point3.Color;
					}
					SizeF markerSize = GetMarkerSize(graph, common, area, point3, point3.MarkerSize, point3.MarkerImage);
					if (common.ProcessModePaint)
					{
						if (markerStyle != 0 || point3.MarkerImage.Length > 0)
						{
							if (markerColor.IsEmpty)
							{
								markerColor = point3.Color;
							}
							if (num5 == 0)
							{
								graph.StartHotRegion(point3);
								graph.StartAnimation();
								graph.DrawMarkerAbs(pointsPosition[num], markerStyle, (int)markerSize.Height, markerColor, point3.MarkerBorderColor, point3.MarkerBorderWidth, point3.MarkerImage, point3.MarkerImageTransparentColor, (point3.series != null) ? point3.series.ShadowOffset : 0, (point3.series != null) ? point3.series.ShadowColor : Color.Empty, new RectangleF(pointsPosition[num].X, pointsPosition[num].Y, markerSize.Width, markerSize.Height), forceAntiAlias: false);
								graph.StopAnimation();
								graph.EndHotRegion();
							}
							num5++;
							if (item.MarkerStep == num5)
							{
								num5 = 0;
							}
						}
						graph.StartAnimation();
						DrawLabels(area, graph, common, pointsPosition[num], (int)markerSize.Height, point3, item, num);
						graph.StopAnimation();
					}
					if (common.ProcessModeRegions)
					{
						SizeF relativeSize = graph.GetRelativeSize(markerSize);
						PointF relativePoint = graph.GetRelativePoint(pointsPosition[num]);
						int insertIndex3 = common.HotRegionsList.FindInsertIndex();
						if (point3.MarkerStyle == MarkerStyle.Circle)
						{
							float[] array = new float[3]
							{
								relativePoint.X,
								relativePoint.Y,
								relativeSize.Width / 2f
							};
							common.HotRegionsList.AddHotRegion(insertIndex3, graph, array[0], array[1], array[2], point3, item.Name, num);
						}
						else
						{
							common.HotRegionsList.AddHotRegion(insertIndex3, graph, new RectangleF(relativePoint.X - relativeSize.Width / 2f, relativePoint.Y - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height), point3, item.Name, num);
						}
					}
					num++;
				}
				if (!selection)
				{
					common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
			}
		}

		internal void AddSelectionPath(ChartArea area, GraphicsPath selectionPath, PointF[] dataPointPos, int firstPointIndex, int secondPointIndex, PointF centerPoint, int borderWidth)
		{
			PointF middlePoint = GetMiddlePoint(dataPointPos[firstPointIndex], dataPointPos[secondPointIndex]);
			PointF pointF = PointF.Empty;
			if (firstPointIndex > 0)
			{
				pointF = GetMiddlePoint(dataPointPos[firstPointIndex], dataPointPos[firstPointIndex - 1]);
			}
			else if (firstPointIndex == 0 && area.CircularSectorsNumber == dataPointPos.Length - 1)
			{
				pointF = GetMiddlePoint(dataPointPos[firstPointIndex], dataPointPos[dataPointPos.Length - 2]);
			}
			if (!centerPoint.IsEmpty)
			{
				selectionPath.AddLine(centerPoint, middlePoint);
				selectionPath.AddLine(middlePoint, dataPointPos[firstPointIndex]);
				if (pointF.IsEmpty)
				{
					selectionPath.AddLine(dataPointPos[firstPointIndex], centerPoint);
					return;
				}
				selectionPath.AddLine(dataPointPos[firstPointIndex], pointF);
				selectionPath.AddLine(pointF, centerPoint);
				return;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			if (!pointF.IsEmpty)
			{
				graphicsPath.AddLine(pointF, dataPointPos[firstPointIndex]);
			}
			graphicsPath.AddLine(dataPointPos[firstPointIndex], middlePoint);
			try
			{
				ChartGraphics.Widen(graphicsPath, new Pen(Color.Black, borderWidth + 2));
				graphicsPath.Flatten();
			}
			catch
			{
			}
			selectionPath.AddPath(graphicsPath, connect: false);
		}

		private PointF GetMiddlePoint(PointF p1, PointF p2)
		{
			PointF empty = PointF.Empty;
			empty.X = (p1.X + p2.X) / 2f;
			empty.Y = (p1.Y + p2.Y) / 2f;
			return empty;
		}

		protected virtual SizeF GetMarkerSize(ChartGraphics graph, CommonElements common, ChartArea area, DataPoint point, int markerSize, string markerImage)
		{
			SizeF size = new SizeF(markerSize, markerSize);
			if (markerImage.Length > 0)
			{
				common.ImageLoader.GetAdjustedImageSize(markerImage, graph.Graphics, ref size);
			}
			return size;
		}

		protected virtual PointF[] GetPointsPosition(ChartGraphics graph, ChartArea area, Series series)
		{
			PointF[] array = new PointF[series.Points.Count + 1];
			int num = 0;
			foreach (DataPoint point in series.Points)
			{
				double yValue = GetYValue(common, area, series, point, num, 0);
				double position = area.AxisY.GetPosition(yValue);
				double num2 = area.circularCenter.X;
				array[num] = graph.GetAbsolutePoint(new PointF((float)num2, (float)position));
				float angle = 360f / (float)area.CircularSectorsNumber * (float)num;
				Matrix matrix = new Matrix();
				matrix.RotateAt(angle, graph.GetAbsolutePoint(area.circularCenter));
				PointF[] array2 = new PointF[1]
				{
					array[num]
				};
				matrix.TransformPoints(array2);
				array[num] = array2[0];
				num++;
			}
			array[num] = graph.GetAbsolutePoint(area.circularCenter);
			return array;
		}

		internal void DrawLabels(ChartArea area, ChartGraphics graph, CommonElements common, PointF markerPosition, int markerSize, DataPoint point, Series ser, int pointIndex)
		{
			string label = point.Label;
			bool showLabelAsValue = point.ShowLabelAsValue;
			if ((point.Empty || (!(ser.ShowLabelAsValue || showLabelAsValue) && label.Length <= 0)) && !showLabelAsValue && label.Length <= 0)
			{
				return;
			}
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Near;
			format.LineAlignment = StringAlignment.Center;
			string text;
			if (label.Length == 0)
			{
				text = ValueConverter.FormatValue(ser.chart, point, point.YValues[0], point.LabelFormat, ser.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = point.ReplaceKeywords(label);
				if (ser.chart != null && ser.chart.LocalizeTextHandler != null)
				{
					text = ser.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
				}
			}
			SizeF size = new SizeF(markerSize, markerSize);
			SizeF size2 = graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic));
			SizeF sizeF = new SizeF(size2.Width, size2.Height);
			sizeF.Height += sizeF.Height / 2f;
			sizeF.Width += sizeF.Width / (float)text.Length;
			autoLabelPosition = true;
			string text2 = point["LabelStyle"];
			if (text2 == null || text2.Length == 0)
			{
				text2 = ser["LabelStyle"];
			}
			if (text2 != null && text2.Length > 0)
			{
				autoLabelPosition = false;
				if (string.Compare(text2, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
				{
					autoLabelPosition = true;
				}
				else if (string.Compare(text2, "Center", StringComparison.OrdinalIgnoreCase) == 0)
				{
					labelPosition = LabelAlignmentTypes.Center;
				}
				else if (string.Compare(text2, "Bottom", StringComparison.OrdinalIgnoreCase) == 0)
				{
					labelPosition = LabelAlignmentTypes.Bottom;
				}
				else if (string.Compare(text2, "TopLeft", StringComparison.OrdinalIgnoreCase) == 0)
				{
					labelPosition = LabelAlignmentTypes.TopLeft;
				}
				else if (string.Compare(text2, "TopRight", StringComparison.OrdinalIgnoreCase) == 0)
				{
					labelPosition = LabelAlignmentTypes.TopRight;
				}
				else if (string.Compare(text2, "BottomLeft", StringComparison.OrdinalIgnoreCase) == 0)
				{
					labelPosition = LabelAlignmentTypes.BottomLeft;
				}
				else if (string.Compare(text2, "BottomRight", StringComparison.OrdinalIgnoreCase) == 0)
				{
					labelPosition = LabelAlignmentTypes.BottomRight;
				}
				else if (string.Compare(text2, "Left", StringComparison.OrdinalIgnoreCase) == 0)
				{
					labelPosition = LabelAlignmentTypes.Left;
				}
				else if (string.Compare(text2, "Right", StringComparison.OrdinalIgnoreCase) == 0)
				{
					labelPosition = LabelAlignmentTypes.Right;
				}
				else
				{
					if (string.Compare(text2, "Top", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new ArgumentException(SR.ExceptionCustomAttributeValueInvalid(text2, "LabelStyle"));
					}
					labelPosition = LabelAlignmentTypes.Top;
				}
			}
			if (autoLabelPosition)
			{
				labelPosition = GetAutoLabelPosition(area, ser, pointIndex);
			}
			PointF pointF = new PointF(markerPosition.X, markerPosition.Y);
			switch (labelPosition)
			{
			case LabelAlignmentTypes.Center:
				format.Alignment = StringAlignment.Center;
				break;
			case LabelAlignmentTypes.Bottom:
				format.Alignment = StringAlignment.Center;
				pointF.Y += size.Height / 1.75f;
				pointF.Y += sizeF.Height / 2f;
				break;
			case LabelAlignmentTypes.Top:
				format.Alignment = StringAlignment.Center;
				pointF.Y -= size.Height / 1.75f;
				pointF.Y -= sizeF.Height / 2f;
				break;
			case LabelAlignmentTypes.Left:
				format.Alignment = StringAlignment.Far;
				pointF.X -= size.Height / 1.75f;
				break;
			case LabelAlignmentTypes.TopLeft:
				format.Alignment = StringAlignment.Far;
				pointF.X -= size.Height / 1.75f;
				pointF.Y -= size.Height / 1.75f;
				pointF.Y -= sizeF.Height / 2f;
				break;
			case LabelAlignmentTypes.BottomLeft:
				format.Alignment = StringAlignment.Far;
				pointF.X -= size.Height / 1.75f;
				pointF.Y += size.Height / 1.75f;
				pointF.Y += sizeF.Height / 2f;
				break;
			case LabelAlignmentTypes.Right:
				pointF.X += size.Height / 1.75f;
				break;
			case LabelAlignmentTypes.TopRight:
				pointF.X += size.Height / 1.75f;
				pointF.Y -= size.Height / 1.75f;
				pointF.Y -= sizeF.Height / 2f;
				break;
			case LabelAlignmentTypes.BottomRight:
				pointF.X += size.Height / 1.75f;
				pointF.Y += size.Height / 1.75f;
				pointF.Y += sizeF.Height / 2f;
				break;
			}
			int angle = point.FontAngle;
			if (text.Trim().Length == 0)
			{
				return;
			}
			if (ser.SmartLabels.Enabled)
			{
				pointF = graph.GetRelativePoint(pointF);
				markerPosition = graph.GetRelativePoint(markerPosition);
				size2 = graph.GetRelativeSize(size2);
				size = graph.GetRelativeSize(size);
				pointF = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, ser.SmartLabels, pointF, size2, ref format, markerPosition, size, labelPosition);
				if (!pointF.IsEmpty)
				{
					pointF = graph.GetAbsolutePoint(pointF);
				}
				size2 = graph.GetAbsoluteSize(size2);
				angle = 0;
			}
			if (!pointF.IsEmpty)
			{
				pointF = graph.GetRelativePoint(pointF);
				RectangleF empty = RectangleF.Empty;
				sizeF = graph.GetRelativeSize(size2);
				sizeF.Height += sizeF.Height / 8f;
				empty = PointChart.GetLabelPosition(graph, pointF, sizeF, format, adjustForDrawing: true);
				graph.DrawPointLabelStringRel(common, text, point.Font, new SolidBrush(point.FontColor), pointF, format, angle, empty, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, ser, point, pointIndex);
			}
		}

		protected virtual LabelAlignmentTypes GetAutoLabelPosition(ChartArea area, Series series, int pointIndex)
		{
			LabelAlignmentTypes result = LabelAlignmentTypes.Top;
			float num = 360f / (float)area.CircularSectorsNumber * (float)pointIndex;
			if (num == 0f)
			{
				result = LabelAlignmentTypes.TopRight;
			}
			else if (num >= 0f && num <= 45f)
			{
				result = LabelAlignmentTypes.Top;
			}
			else if (num >= 45f && num <= 90f)
			{
				result = LabelAlignmentTypes.TopRight;
			}
			else if (num >= 90f && num <= 135f)
			{
				result = LabelAlignmentTypes.BottomRight;
			}
			else if (num >= 135f && num <= 180f)
			{
				result = LabelAlignmentTypes.BottomRight;
			}
			else if (num >= 180f && num <= 225f)
			{
				result = LabelAlignmentTypes.BottomLeft;
			}
			else if (num >= 225f && num <= 270f)
			{
				result = LabelAlignmentTypes.BottomLeft;
			}
			else if (num >= 270f && num <= 315f)
			{
				result = LabelAlignmentTypes.TopLeft;
			}
			else if (num >= 315f && num <= 360f)
			{
				result = LabelAlignmentTypes.TopLeft;
			}
			return result;
		}

		protected virtual RadarDrawingStyle GetDrawingStyle(Series ser, DataPoint point)
		{
			RadarDrawingStyle result = RadarDrawingStyle.Area;
			if (point.IsAttributeSet("RadarDrawingStyle") || ser.IsAttributeSet("RadarDrawingStyle"))
			{
				string text = point.IsAttributeSet("RadarDrawingStyle") ? point["RadarDrawingStyle"] : ser["RadarDrawingStyle"];
				if (string.Compare(text, "Area", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = RadarDrawingStyle.Area;
				}
				else if (string.Compare(text, "Line", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = RadarDrawingStyle.Line;
				}
				else
				{
					if (string.Compare(text, "Marker", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "RadarDrawingStyle"));
					}
					result = RadarDrawingStyle.Marker;
				}
			}
			return result;
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			if (yValueIndex == -1)
			{
				return 0.0;
			}
			if (point.Empty)
			{
				double num = GetEmptyPointValue(point, pointIndex);
				if (num == 0.0)
				{
					Axis axis = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
					double maximum = axis.maximum;
					double minimum = axis.minimum;
					if (num < minimum)
					{
						num = minimum;
					}
					else if (num > maximum)
					{
						num = maximum;
					}
				}
				return num;
			}
			return point.YValues[yValueIndex];
		}

		internal double GetEmptyPointValue(DataPoint point, int pointIndex)
		{
			Series series = point.series;
			double num = 0.0;
			double num2 = 0.0;
			int index = 0;
			int index2 = series.Points.Count - 1;
			string strA = "";
			if (series.EmptyPointStyle.IsAttributeSet("EmptyPointValue"))
			{
				strA = series.EmptyPointStyle["EmptyPointValue"];
			}
			else if (series.IsAttributeSet("EmptyPointValue"))
			{
				strA = series["EmptyPointValue"];
			}
			if (string.Compare(strA, "Zero", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 0.0;
			}
			for (int num3 = pointIndex; num3 >= 0; num3--)
			{
				if (!series.Points[num3].Empty)
				{
					num = series.Points[num3].YValues[0];
					index = num3;
					break;
				}
				num = double.NaN;
			}
			for (int i = pointIndex; i < series.Points.Count; i++)
			{
				if (!series.Points[i].Empty)
				{
					num2 = series.Points[i].YValues[0];
					index2 = i;
					break;
				}
				num2 = double.NaN;
			}
			if (double.IsNaN(num))
			{
				num = ((!double.IsNaN(num2)) ? num2 : 0.0);
			}
			if (double.IsNaN(num2))
			{
				num2 = num;
			}
			if (series.Points[index2].XValue == series.Points[index].XValue)
			{
				return (num + num2) / 2.0;
			}
			return (0.0 - (num - num2) / (series.Points[index2].XValue - series.Points[index].XValue)) * (point.XValue - series.Points[index].XValue) + num;
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
			PointF[] pointsPosition = GetPointsPosition(common.graph, area, series);
			int num = 0;
			int num2 = 0;
			foreach (DataPoint point in series.Points)
			{
				Color color = point.MarkerColor;
				MarkerStyle markerStyle = point.MarkerStyle;
				if (GetDrawingStyle(series, point) == RadarDrawingStyle.Marker)
				{
					color = point.Color;
				}
				SizeF markerSize = GetMarkerSize(common.graph, common, area, point, point.MarkerSize, point.MarkerImage);
				if (markerStyle != 0 || point.MarkerImage.Length > 0)
				{
					if (color.IsEmpty)
					{
						color = point.Color;
					}
					if (num == 0)
					{
						PointF relativePoint = common.graph.GetRelativePoint(pointsPosition[num2]);
						markerSize = common.graph.GetRelativeSize(markerSize);
						RectangleF rectangleF = new RectangleF(relativePoint.X - markerSize.Width / 2f, relativePoint.Y - markerSize.Height / 2f, markerSize.Width, markerSize.Height);
						list.Add(rectangleF);
					}
					num++;
					if (series.MarkerStep == num)
					{
						num = 0;
					}
				}
				num2++;
			}
		}
	}
}
