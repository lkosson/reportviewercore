using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class PointChart : IChartType
	{
		internal class Label3DInfo
		{
			internal DataPoint3D PointEx;

			internal PointF MarkerPosition = PointF.Empty;

			internal SizeF MarkerSize = SizeF.Empty;

			internal PointF AnimatedPoint = PointF.Empty;
		}

		internal bool alwaysDrawMarkers = true;

		internal int yValueIndex;

		internal int labelYValueIndex = -1;

		internal bool autoLabelPosition = true;

		internal LabelAlignmentTypes labelPosition = LabelAlignmentTypes.Top;

		internal Axis vAxis;

		internal Axis hAxis;

		internal bool indexedSeries;

		internal CommonElements common;

		internal ChartArea area;

		internal bool middleMarker = true;

		internal ArrayList label3DInfoList;

		public virtual string Name => "Point";

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

		public virtual double ShiftedX
		{
			get
			{
				return 0.0;
			}
			set
			{
			}
		}

		public virtual string ShiftedSerName
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public PointChart()
		{
		}

		public PointChart(bool alwaysDrawMarkers)
		{
			this.alwaysDrawMarkers = alwaysDrawMarkers;
		}

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
			this.area = area;
			ProcessChartType(selection: false, graph, common, area, seriesToDraw);
		}

		protected virtual void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			if (area.Area3DStyle.Enable3D)
			{
				ProcessChartType3D(selection, graph, common, area, seriesToDraw);
				return;
			}
			if (ShiftedSerName.Length == 0)
			{
				indexedSeries = area.IndexedSeries((string[])area.GetSeriesFromChartType(Name).ToArray(typeof(string)));
			}
			else
			{
				indexedSeries = ChartElement.IndexedSeries(common.DataManager.Series[ShiftedSerName]);
			}
			foreach (Series item in common.DataManager.Series)
			{
				bool flag = false;
				if (ShiftedSerName.Length > 0)
				{
					if (ShiftedSerName != item.Name)
					{
						continue;
					}
					flag = true;
				}
				if (string.Compare(item.ChartTypeName, Name, ignoreCase: true, CultureInfo.CurrentCulture) != 0 || item.ChartArea != area.Name || !item.IsVisible() || (seriesToDraw != null && seriesToDraw.Name != item.Name))
				{
					continue;
				}
				hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
				vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
				double viewMaximum = hAxis.GetViewMaximum();
				double viewMinimum = hAxis.GetViewMinimum();
				double viewMaximum2 = vAxis.GetViewMaximum();
				double viewMinimum2 = vAxis.GetViewMinimum();
				if (!selection)
				{
					common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				int num = 0;
				int num2 = 1;
				foreach (DataPoint point in item.Points)
				{
					point.positionRel = new PointF(float.NaN, float.NaN);
					double yValue = indexedSeries ? ((double)num2) : point.XValue;
					yValue = hAxis.GetLogValue(yValue);
					if (yValue > viewMaximum || yValue < viewMinimum)
					{
						num2++;
						continue;
					}
					double yValue2 = GetYValue(common, area, item, point, num2 - 1, yValueIndex);
					yValue2 = vAxis.GetLogValue(yValue2);
					if (yValue2 > viewMaximum2 || yValue2 < viewMinimum2)
					{
						num2++;
						continue;
					}
					bool flag2 = false;
					if (!ShouldDrawMarkerOnViewEdgeX())
					{
						if (yValue == viewMaximum && ShiftedX >= 0.0)
						{
							flag2 = true;
						}
						if (yValue == viewMinimum && ShiftedX <= 0.0)
						{
							flag2 = true;
						}
					}
					int markerSize = point.MarkerSize;
					string markerImage = point.MarkerImage;
					MarkerStyle markerStyle = point.MarkerStyle;
					PointF empty = PointF.Empty;
					empty.Y = (float)vAxis.GetLinearPosition(yValue2);
					if (indexedSeries)
					{
						empty.X = (float)hAxis.GetPosition(num2);
					}
					else
					{
						empty.X = (float)hAxis.GetPosition(point.XValue);
					}
					empty.X += (float)ShiftedX;
					point.positionRel = new PointF(empty.X, empty.Y);
					SizeF markerSize2 = GetMarkerSize(graph, common, area, point, markerSize, markerImage);
					if (flag2)
					{
						num2++;
						continue;
					}
					if (alwaysDrawMarkers || markerStyle != 0 || markerImage.Length > 0)
					{
						if (common.ProcessModePaint)
						{
							if (num == 0)
							{
								graph.StartHotRegion(point);
								graph.StartAnimation();
								DrawPointMarker(graph, point.series, point, empty, (markerStyle == MarkerStyle.None) ? MarkerStyle.Circle : markerStyle, (int)markerSize2.Height, (point.MarkerColor == Color.Empty) ? point.Color : point.MarkerColor, (point.MarkerBorderColor == Color.Empty) ? point.BorderColor : point.MarkerBorderColor, GetMarkerBorderSize(point), markerImage, point.MarkerImageTransparentColor, (point.series != null) ? point.series.ShadowOffset : 0, (point.series != null) ? point.series.ShadowColor : Color.Empty, new RectangleF(empty.X, empty.Y, markerSize2.Width, markerSize2.Height));
								graph.StopAnimation();
								graph.EndHotRegion();
							}
							if (common.ProcessModeRegions)
							{
								SetHotRegions(common, graph, point, markerSize2, point.series.Name, num2 - 1, markerStyle, empty);
							}
						}
						num++;
						if (item.MarkerStep == num)
						{
							num = 0;
						}
					}
					graph.StartHotRegion(point, labelRegion: true);
					graph.StartAnimation();
					DrawLabels(area, graph, common, empty, (int)markerSize2.Height, point, item, num2 - 1);
					graph.StopAnimation();
					graph.EndHotRegion();
					num2++;
				}
				if (!selection)
				{
					common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				if (flag)
				{
					break;
				}
			}
		}

		protected virtual void DrawPointMarker(ChartGraphics graph, Series series, DataPoint dataPoint, PointF point, MarkerStyle markerStyle, int markerSize, Color markerColor, Color markerBorderColor, int markerBorderSize, string markerImage, Color markerImageTransparentColor, int shadowSize, Color shadowColor, RectangleF imageScaleRect)
		{
			graph.DrawMarkerRel(point, markerStyle, markerSize, markerColor, markerBorderColor, markerBorderSize, markerImage, markerImageTransparentColor, shadowSize, shadowColor, imageScaleRect);
		}

		private void SetHotRegions(CommonElements common, ChartGraphics graph, DataPoint point, SizeF markerSize, string seriesName, int pointIndex, MarkerStyle pointMarkerStyle, PointF markerPosition)
		{
			SizeF relativeSize = graph.GetRelativeSize(markerSize);
			int insertIndex = common.HotRegionsList.FindInsertIndex();
			if (pointMarkerStyle == MarkerStyle.Circle)
			{
				common.HotRegionsList.AddHotRegion(insertIndex, graph, markerPosition.X, markerPosition.Y, relativeSize.Width / 2f, point, seriesName, pointIndex);
			}
			else
			{
				common.HotRegionsList.AddHotRegion(graph, new RectangleF(markerPosition.X - relativeSize.Width / 2f, markerPosition.Y - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height), point, seriesName, pointIndex);
			}
		}

		private void DrawLabels(ChartArea area, ChartGraphics graph, CommonElements common, PointF markerPosition, int markerSize, DataPoint point, Series ser, int pointIndex)
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
				text = ValueConverter.FormatValue(ser.chart, point, point.YValues[(labelYValueIndex == -1) ? yValueIndex : labelYValueIndex], point.LabelFormat, ser.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = point.ReplaceKeywords(label);
				if (ser.chart != null && ser.chart.LocalizeTextHandler != null)
				{
					text = ser.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
				}
			}
			SizeF relativeSize = graph.GetRelativeSize(new SizeF(markerSize, markerSize));
			SizeF relativeSize2 = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
			SizeF relativeSize3 = graph.GetRelativeSize(graph.MeasureString("W", point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
			SizeF size = new SizeF(relativeSize2.Width, relativeSize2.Height);
			float num = size.Width / (float)text.Length;
			size.Height += relativeSize3.Height / 2f;
			size.Width += num;
			string text2 = point["LabelStyle"];
			if (text2 == null || text2.Length == 0)
			{
				text2 = ser["LabelStyle"];
			}
			autoLabelPosition = true;
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
				labelPosition = GetAutoLabelPosition(ser, pointIndex);
			}
			PointF position = new PointF(markerPosition.X, markerPosition.Y);
			switch (labelPosition)
			{
			case LabelAlignmentTypes.Center:
				format.Alignment = StringAlignment.Center;
				break;
			case LabelAlignmentTypes.Bottom:
				format.Alignment = StringAlignment.Center;
				position.Y += relativeSize.Height / 1.75f;
				position.Y += size.Height / 2f;
				break;
			case LabelAlignmentTypes.Top:
				format.Alignment = StringAlignment.Center;
				position.Y -= relativeSize.Height / 1.75f;
				position.Y -= size.Height / 2f;
				break;
			case LabelAlignmentTypes.Left:
				format.Alignment = StringAlignment.Far;
				position.X -= relativeSize.Height / 1.75f + num / 2f;
				break;
			case LabelAlignmentTypes.TopLeft:
				format.Alignment = StringAlignment.Far;
				position.X -= relativeSize.Height / 1.75f + num / 2f;
				position.Y -= relativeSize.Height / 1.75f;
				position.Y -= size.Height / 2f;
				break;
			case LabelAlignmentTypes.BottomLeft:
				format.Alignment = StringAlignment.Far;
				position.X -= relativeSize.Height / 1.75f + num / 2f;
				position.Y += relativeSize.Height / 1.75f;
				position.Y += size.Height / 2f;
				break;
			case LabelAlignmentTypes.Right:
				position.X += relativeSize.Height / 1.75f + num / 2f;
				break;
			case LabelAlignmentTypes.TopRight:
				position.X += relativeSize.Height / 1.75f + num / 2f;
				position.Y -= relativeSize.Height / 1.75f;
				position.Y -= size.Height / 2f;
				break;
			case LabelAlignmentTypes.BottomRight:
				position.X += relativeSize.Height / 1.75f + num / 2f;
				position.Y += relativeSize.Height / 1.75f;
				position.Y += size.Height / 2f;
				break;
			}
			int num2 = point.FontAngle;
			if (text.Trim().Length == 0)
			{
				return;
			}
			if (ser.SmartLabels.Enabled)
			{
				position = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, ser.SmartLabels, position, relativeSize2, ref format, markerPosition, relativeSize, labelPosition);
				num2 = 0;
			}
			if (num2 == 90 || num2 == -90)
			{
				switch (labelPosition)
				{
				case LabelAlignmentTypes.Top:
					format.Alignment = StringAlignment.Near;
					position.Y += size.Height / 2f;
					break;
				case LabelAlignmentTypes.Bottom:
					format.Alignment = StringAlignment.Far;
					position.Y -= size.Height / 2f;
					break;
				case LabelAlignmentTypes.Right:
					format.Alignment = StringAlignment.Center;
					format.LineAlignment = StringAlignment.Near;
					break;
				case LabelAlignmentTypes.Left:
					format.Alignment = StringAlignment.Center;
					format.LineAlignment = StringAlignment.Center;
					break;
				case LabelAlignmentTypes.TopLeft:
					format.Alignment = StringAlignment.Near;
					break;
				case LabelAlignmentTypes.BottomRight:
					format.Alignment = StringAlignment.Far;
					break;
				}
			}
			if (!position.IsEmpty)
			{
				RectangleF empty = RectangleF.Empty;
				size.Height -= relativeSize2.Height / 2f;
				size.Height += relativeSize2.Height / 8f;
				empty = GetLabelPosition(graph, position, size, format, adjustForDrawing: true);
				switch (labelPosition)
				{
				case LabelAlignmentTypes.Left:
					empty.X += num / 2f;
					break;
				case LabelAlignmentTypes.TopLeft:
					empty.X += num / 2f;
					break;
				case LabelAlignmentTypes.BottomLeft:
					empty.X += num / 2f;
					break;
				case LabelAlignmentTypes.Right:
					empty.X -= num / 2f;
					break;
				case LabelAlignmentTypes.TopRight:
					empty.X -= num / 2f;
					break;
				case LabelAlignmentTypes.BottomRight:
					empty.X -= num / 2f;
					break;
				}
				graph.DrawPointLabelStringRel(common, text, point.Font, new SolidBrush(point.FontColor), position, format, num2, empty, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, ser, point, pointIndex);
			}
		}

		internal static RectangleF GetLabelPosition(ChartGraphics graph, PointF position, SizeF size, StringFormat format, bool adjustForDrawing)
		{
			RectangleF empty = RectangleF.Empty;
			empty.Width = size.Width;
			empty.Height = size.Height;
			SizeF sizeF = SizeF.Empty;
			if (graph != null)
			{
				sizeF = graph.GetRelativeSize(new SizeF(1f, 1f));
			}
			if (format.Alignment == StringAlignment.Far)
			{
				empty.X = position.X - size.Width;
				if (adjustForDrawing && !sizeF.IsEmpty)
				{
					empty.X -= 4f * sizeF.Width;
					empty.Width += 4f * sizeF.Width;
				}
			}
			else if (format.Alignment == StringAlignment.Near)
			{
				empty.X = position.X;
				if (adjustForDrawing && !sizeF.IsEmpty)
				{
					empty.Width += 4f * sizeF.Width;
				}
			}
			else if (format.Alignment == StringAlignment.Center)
			{
				empty.X = position.X - size.Width / 2f;
				if (adjustForDrawing && !sizeF.IsEmpty)
				{
					empty.X -= 2f * sizeF.Width;
					empty.Width += 4f * sizeF.Width;
				}
			}
			if (format.LineAlignment == StringAlignment.Far)
			{
				empty.Y = position.Y - size.Height;
			}
			else if (format.LineAlignment == StringAlignment.Near)
			{
				empty.Y = position.Y;
			}
			else if (format.LineAlignment == StringAlignment.Center)
			{
				empty.Y = position.Y - size.Height / 2f;
			}
			empty.Y -= 1f * sizeF.Height;
			return empty;
		}

		protected virtual void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
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
			foreach (object item in area.GetDataPointDrawingOrder(arrayList, this, selection, COPCoordinates.X, null, yValueIndex, sideBySide: false))
			{
				ProcessSinglePoint3D((DataPoint3D)item, selection, graph, common, area);
			}
			DrawAccumulated3DLabels(graph, common, area);
		}

		internal void ProcessSinglePoint3D(DataPoint3D pointEx, bool selection, ChartGraphics graph, CommonElements common, ChartArea area)
		{
			DataPoint dataPoint = pointEx.dataPoint;
			Series series = dataPoint.series;
			dataPoint.positionRel = new PointF(float.NaN, float.NaN);
			hAxis = area.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
			vAxis = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
			double yValue = GetYValue(common, area, series, pointEx.dataPoint, pointEx.index - 1, yValueIndex);
			yValue = vAxis.GetLogValue(yValue);
			if (yValue > vAxis.GetViewMaximum() || yValue < vAxis.GetViewMinimum())
			{
				return;
			}
			double yValue2 = pointEx.indexedSeries ? ((double)pointEx.index) : dataPoint.XValue;
			yValue2 = hAxis.GetLogValue(yValue2);
			if (yValue2 > hAxis.GetViewMaximum() || yValue2 < hAxis.GetViewMinimum() || (!ShouldDrawMarkerOnViewEdgeX() && ((yValue2 == hAxis.GetViewMaximum() && ShiftedX >= 0.0) || (yValue2 == hAxis.GetViewMinimum() && ShiftedX <= 0.0))))
			{
				return;
			}
			PointF empty = PointF.Empty;
			empty.Y = (float)pointEx.yPosition;
			empty.X = (float)hAxis.GetLinearPosition(yValue2);
			empty.X += (float)ShiftedX;
			dataPoint.positionRel = new PointF(empty.X, empty.Y);
			int markerSize = dataPoint.MarkerSize;
			string markerImage = dataPoint.MarkerImage;
			MarkerStyle markerStyle = dataPoint.MarkerStyle;
			SizeF markerSize2 = GetMarkerSize(graph, common, area, dataPoint, markerSize, markerImage);
			Point3D[] array = new Point3D[1]
			{
				new Point3D(empty.X, empty.Y, pointEx.zPosition + (middleMarker ? (pointEx.depth / 2f) : pointEx.depth))
			};
			area.matrix3D.TransformPoints(array);
			PointF pointF = array[0].PointF;
			GraphicsPath path = null;
			if ((alwaysDrawMarkers || markerStyle != 0 || markerImage.Length > 0) && pointEx.index % series.MarkerStep == 0)
			{
				DrawingOperationTypes drawingOperationTypes = DrawingOperationTypes.DrawElement;
				if (common.ProcessModeRegions)
				{
					drawingOperationTypes |= DrawingOperationTypes.CalcElementPath;
				}
				graph.StartHotRegion(dataPoint);
				graph.StartAnimation();
				path = graph.DrawMarker3D(area.matrix3D, area.Area3DStyle.Light, pointEx.zPosition + (middleMarker ? (pointEx.depth / 2f) : pointEx.depth), empty, (markerStyle == MarkerStyle.None) ? MarkerStyle.Circle : markerStyle, (int)markerSize2.Height, (dataPoint.MarkerColor == Color.Empty) ? dataPoint.Color : dataPoint.MarkerColor, (dataPoint.MarkerBorderColor == Color.Empty) ? dataPoint.BorderColor : dataPoint.MarkerBorderColor, GetMarkerBorderSize(dataPoint), markerImage, dataPoint.MarkerImageTransparentColor, (dataPoint.series != null) ? dataPoint.series.ShadowOffset : 0, (dataPoint.series != null) ? dataPoint.series.ShadowColor : Color.Empty, new RectangleF(pointF.X, pointF.Y, markerSize2.Width, markerSize2.Height), drawingOperationTypes);
				graph.StopAnimation();
				graph.EndHotRegion();
			}
			PointF empty2 = PointF.Empty;
			if (label3DInfoList != null && label3DInfoList.Count > 0 && ((Label3DInfo)label3DInfoList[label3DInfoList.Count - 1]).PointEx.zPosition != pointEx.zPosition)
			{
				DrawAccumulated3DLabels(graph, common, area);
			}
			if (label3DInfoList == null)
			{
				label3DInfoList = new ArrayList();
			}
			Label3DInfo label3DInfo = new Label3DInfo();
			label3DInfo.PointEx = pointEx;
			label3DInfo.MarkerPosition = pointF;
			label3DInfo.MarkerSize = markerSize2;
			label3DInfo.AnimatedPoint = empty2;
			label3DInfoList.Add(label3DInfo);
			if (common.ProcessModeRegions)
			{
				SizeF relativeSize = graph.GetRelativeSize(markerSize2);
				int insertIndex = common.HotRegionsList.FindInsertIndex();
				if (markerStyle == MarkerStyle.Circle)
				{
					float[] array2 = new float[3]
					{
						pointF.X,
						pointF.Y,
						relativeSize.Width / 2f
					};
					common.HotRegionsList.AddHotRegion(insertIndex, graph, array2[0], array2[1], array2[2], dataPoint, series.Name, pointEx.index - 1);
				}
				if (markerStyle == MarkerStyle.Square)
				{
					common.HotRegionsList.AddHotRegion(path, relativePath: false, graph, dataPoint, series.Name, pointEx.index - 1);
				}
				else
				{
					common.HotRegionsList.AddHotRegion(graph, new RectangleF(pointF.X - relativeSize.Width / 2f, pointF.Y - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height), dataPoint, series.Name, pointEx.index - 1);
				}
			}
		}

		internal void DrawAccumulated3DLabels(ChartGraphics graph, CommonElements common, ChartArea area)
		{
			if (label3DInfoList == null)
			{
				return;
			}
			foreach (Label3DInfo label3DInfo in label3DInfoList)
			{
				graph.StartAnimation();
				DrawLabels(area, graph, common, label3DInfo.MarkerPosition, (int)label3DInfo.MarkerSize.Height, label3DInfo.PointEx.dataPoint, label3DInfo.PointEx.dataPoint.series, label3DInfo.PointEx.index - 1);
				graph.StopAnimation();
			}
			label3DInfoList.Clear();
		}

		protected virtual bool ShouldDrawMarkerOnViewEdgeX()
		{
			return true;
		}

		protected virtual int GetMarkerBorderSize(DataPointAttributes point)
		{
			return point.MarkerBorderWidth;
		}

		protected virtual LabelAlignmentTypes GetAutoLabelPosition(Series series, int pointIndex)
		{
			return LabelAlignmentTypes.Top;
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

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			if (yValueIndex == -1)
			{
				return 0.0;
			}
			if (point.YValues.Length <= yValueIndex)
			{
				throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(Name, YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
			}
			if (point.Empty || double.IsNaN(point.YValues[yValueIndex]))
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
					num = series.Points[num3].YValues[yValueIndex];
					index = num3;
					break;
				}
				num = double.NaN;
			}
			for (int i = pointIndex; i < series.Points.Count; i++)
			{
				if (!series.Points[i].Empty)
				{
					num2 = series.Points[i].YValues[yValueIndex];
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
			indexedSeries = area.IndexedSeries((string[])area.GetSeriesFromChartType(Name).ToArray(typeof(string)));
			Axis axis = area.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
			Axis axis2 = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
			int num = 0;
			int num2 = 1;
			foreach (DataPoint point in series.Points)
			{
				double yValue = GetYValue(common, area, series, point, num2 - 1, yValueIndex);
				yValue = axis2.GetLogValue(yValue);
				if (yValue > axis2.GetViewMaximum() || yValue < axis2.GetViewMinimum())
				{
					num2++;
					continue;
				}
				double yValue2 = indexedSeries ? ((double)num2) : point.XValue;
				yValue2 = axis.GetLogValue(yValue2);
				if (yValue2 > axis.GetViewMaximum() || yValue2 < axis.GetViewMinimum())
				{
					num2++;
					continue;
				}
				if (!ShouldDrawMarkerOnViewEdgeX())
				{
					if (yValue2 == axis.GetViewMaximum() && ShiftedX >= 0.0)
					{
						num2++;
						continue;
					}
					if (yValue2 == axis.GetViewMinimum() && ShiftedX <= 0.0)
					{
						num2++;
						continue;
					}
				}
				PointF pointF = PointF.Empty;
				pointF.Y = (float)axis2.GetLinearPosition(yValue);
				if (indexedSeries)
				{
					pointF.X = (float)axis.GetPosition(num2);
				}
				else
				{
					pointF.X = (float)axis.GetPosition(point.XValue);
				}
				pointF.X += (float)ShiftedX;
				int markerSize = point.MarkerSize;
				string markerImage = point.MarkerImage;
				MarkerStyle markerStyle = point.MarkerStyle;
				SizeF markerSize2 = GetMarkerSize(common.graph, common, area, point, markerSize, markerImage);
				if (area.Area3DStyle.Enable3D)
				{
					area.GetSeriesZPositionAndDepth(series, out float depth, out float positionZ);
					Point3D[] array = new Point3D[1]
					{
						new Point3D(pointF.X, pointF.Y, positionZ + (middleMarker ? (depth / 2f) : depth))
					};
					area.matrix3D.TransformPoints(array);
					pointF = array[0].PointF;
				}
				if (alwaysDrawMarkers || markerStyle != 0 || markerImage.Length > 0)
				{
					if (num == 0)
					{
						markerSize2 = common.graph.GetRelativeSize(markerSize2);
						RectangleF rectangleF = new RectangleF(pointF.X - markerSize2.Width / 2f, pointF.Y - markerSize2.Height / 2f, markerSize2.Width, markerSize2.Height);
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
