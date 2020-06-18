using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class StackedColumnChart : IChartType
	{
		protected double prevPosY = double.NaN;

		protected double prevNegY = double.NaN;

		protected bool hundredPercentStacked;

		internal bool stackGroupNameUsed;

		internal ArrayList stackGroupNames;

		internal string currentStackGroup = string.Empty;

		public virtual string Name => "StackedColumn";

		public virtual bool Stacked => true;

		public virtual bool SupportStackedGroups => true;

		public bool StackSign => true;

		public virtual bool RequireAxes => true;

		public virtual bool SecondYScale => false;

		public bool CircularChartArea => false;

		public virtual bool SupportLogarithmicAxes => true;

		public virtual bool SwitchValueAxes => false;

		public bool SideBySideSeries => false;

		public virtual bool DataPointsInLegend => false;

		public virtual bool ExtraYValuesConnectedToYAxis => false;

		public virtual bool HundredPercent => false;

		public virtual bool HundredPercentSupportNegative => false;

		public virtual bool ApplyPaletteColorsToPoints => false;

		public virtual int YValuesPerPoint => 1;

		public virtual bool ZeroCrossing => true;

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public virtual LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		public virtual void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			stackGroupNameUsed = true;
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(area.PlotAreaPosition.ToRectangleF());
			float num = (float)Math.Ceiling(absoluteRectangle.Right);
			float num2 = (float)Math.Ceiling(absoluteRectangle.Bottom);
			absoluteRectangle.X = (float)Math.Floor(absoluteRectangle.X);
			absoluteRectangle.Width = num - absoluteRectangle.X;
			absoluteRectangle.Y = (float)Math.Floor(absoluteRectangle.Y);
			absoluteRectangle.Height = num2 - absoluteRectangle.Y;
			graph.SetClipAbs(absoluteRectangle);
			ProcessChartType(selection: false, graph, common, area, shadow: true, labels: false, seriesToDraw);
			ProcessChartType(selection: false, graph, common, area, shadow: false, labels: false, seriesToDraw);
			ProcessChartType(selection: false, graph, common, area, shadow: false, labels: true, seriesToDraw);
			graph.ResetClip();
		}

		private void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, bool shadow, bool labels, Series seriesToDraw)
		{
			bool flag = false;
			AxisType axisType = AxisType.Primary;
			AxisType axisType2 = AxisType.Primary;
			string a = string.Empty;
			string a2 = string.Empty;
			for (int i = 0; i < common.DataManager.Series.Count; i++)
			{
				Series series = common.DataManager.Series[i];
				if (string.Compare(series.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) == 0 && !(series.ChartArea != area.Name) && series.IsVisible())
				{
					if (i == 0)
					{
						axisType = series.XAxisType;
						axisType2 = series.YAxisType;
						a = series.XSubAxisName;
						a2 = series.YSubAxisName;
					}
					else if (axisType != series.XAxisType || axisType2 != series.YAxisType || a != series.XSubAxisName || a2 != series.YSubAxisName)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				for (int j = 0; j < common.DataManager.Series.Count; j++)
				{
					Series series2 = common.DataManager.Series[j];
					if (string.Compare(series2.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) == 0 && !(series2.ChartArea != area.Name) && series2.IsVisible())
					{
						string seriesStackGroupName = GetSeriesStackGroupName(series2);
						seriesStackGroupName = (series2["StackedGroupName"] = "_X_" + series2.XAxisType.ToString() + series2.XSubAxisName + "_Y_" + series2.YAxisType.ToString() + series2.YSubAxisName + "__");
					}
				}
			}
			stackGroupNames = new ArrayList();
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) == 0 && !(item.ChartArea != area.Name) && item.IsVisible())
				{
					string seriesStackGroupName2 = GetSeriesStackGroupName(item);
					if (!stackGroupNames.Contains(seriesStackGroupName2))
					{
						stackGroupNames.Add(seriesStackGroupName2);
					}
				}
			}
			if (area.Area3DStyle.Enable3D)
			{
				if (!shadow)
				{
					ProcessChartType3D(selection, graph, common, area, labels, seriesToDraw);
				}
				return;
			}
			string[] series4 = (string[])area.GetSeriesFromChartType(Name).ToArray(typeof(string));
			int numberOfPoints = common.DataManager.GetNumberOfPoints(series4);
			bool flag2 = area.IndexedSeries(series4);
			for (int k = 0; k < numberOfPoints; k++)
			{
				for (int l = 0; l < stackGroupNames.Count; l++)
				{
					currentStackGroup = (string)stackGroupNames[l];
					int num = 0;
					double num2 = 0.0;
					double num3 = 0.0;
					foreach (Series item2 in common.DataManager.Series)
					{
						if (string.Compare(item2.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) != 0 || item2.ChartArea != area.Name || !item2.IsVisible() || k >= item2.Points.Count || GetSeriesStackGroupName(item2) != currentStackGroup)
						{
							continue;
						}
						DataPoint dataPoint = item2.Points[k];
						dataPoint.positionRel = new PointF(float.NaN, float.NaN);
						Axis axis = area.GetAxis(AxisName.Y, item2.YAxisType, item2.YSubAxisName);
						Axis axis2 = area.GetAxis(AxisName.X, item2.XAxisType, item2.XSubAxisName);
						bool sameInterval = false;
						double interval = 1.0;
						if (!flag2)
						{
							if (item2.Points.Count == 1 && (item2.XValueType == ChartValueTypes.Date || item2.XValueType == ChartValueTypes.DateTime || item2.XValueType == ChartValueTypes.Time || item2.XValueType == ChartValueTypes.DateTimeOffset))
							{
								ArrayList seriesFromChartType = area.GetSeriesFromChartType(Name);
								area.GetPointsInterval(seriesFromChartType, axis2.Logarithmic, axis2.logarithmBase, checkSameInterval: true, out sameInterval);
								interval = ((double.IsNaN(axis2.majorGrid.Interval) || axis2.majorGrid.IntervalType == DateTimeIntervalType.NotSet) ? axis2.GetIntervalSize(axis2.minimum, axis2.Interval, axis2.IntervalType) : axis2.GetIntervalSize(axis2.minimum, axis2.majorGrid.Interval, axis2.majorGrid.IntervalType));
							}
							else
							{
								interval = area.GetPointsInterval(axis2.Logarithmic, axis2.logarithmBase);
							}
						}
						double pointWidth = item2.GetPointWidth(graph, axis2, interval, 0.8);
						pointWidth /= (double)stackGroupNames.Count;
						if (!selection)
						{
							common.EventsManager.OnBackPaint(item2, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						double num4 = GetYValue(common, area, item2, dataPoint, k, 0);
						if (num != 0)
						{
							num4 = ((!(num4 >= 0.0)) ? (num4 + num3) : (num4 + num2));
						}
						bool flag3 = false;
						double num5 = num4;
						num4 = axis.GetLogValue(num4);
						if (!(flag3 && labels))
						{
							if (num4 > axis.GetViewMaximum())
							{
								num4 = axis.GetViewMaximum();
							}
							if (num4 < axis.GetViewMinimum())
							{
								num4 = axis.GetViewMinimum();
							}
						}
						double linearPosition = axis.GetLinearPosition(num4);
						double num6 = 0.0;
						num6 = ((num == 0) ? ((!(flag3 && labels)) ? axis.Crossing : 0.0) : ((!(GetYValue(common, area, item2, dataPoint, k, 0) >= 0.0)) ? num3 : num2));
						double position = axis.GetPosition(num6);
						double num7 = dataPoint.XValue;
						if (flag2)
						{
							num7 = (double)k + 1.0;
						}
						double num8 = axis2.GetPosition(num7);
						if (stackGroupNames.Count > 1)
						{
							num8 = num8 - pointWidth * (double)stackGroupNames.Count / 2.0 + pointWidth / 2.0 + (double)l * pointWidth;
						}
						num7 = axis2.GetLogValue(num7);
						RectangleF empty = RectangleF.Empty;
						try
						{
							empty.X = (float)(num8 - pointWidth / 2.0);
							empty.Width = (float)pointWidth;
							if (position < linearPosition)
							{
								empty.Y = (float)position;
								empty.Height = (float)linearPosition - empty.Y;
							}
							else
							{
								empty.Y = (float)linearPosition;
								empty.Height = (float)position - empty.Y;
							}
						}
						catch (Exception)
						{
							num++;
							continue;
						}
						dataPoint.positionRel = new PointF((float)num8, empty.Top);
						if (dataPoint.Empty)
						{
							num++;
							continue;
						}
						if (common.ProcessModePaint)
						{
							bool flag4 = false;
							if (num7 < axis2.GetViewMinimum() || num7 > axis2.GetViewMaximum() || (num4 < axis.GetViewMinimum() && num6 < axis.GetViewMinimum()) || (num4 > axis.GetViewMaximum() && num6 > axis.GetViewMaximum()))
							{
								flag4 = true;
							}
							if (!flag4)
							{
								int num9 = 0;
								if (shadow)
								{
									num9 = item2.ShadowOffset;
								}
								if (!labels)
								{
									bool flag5 = false;
									if (empty.X < area.PlotAreaPosition.X || empty.Right > area.PlotAreaPosition.Right() || empty.Y < area.PlotAreaPosition.Y || empty.Bottom > area.PlotAreaPosition.Bottom())
									{
										graph.SetClip(area.PlotAreaPosition.ToRectangleF());
										flag5 = true;
									}
									graph.StartHotRegion(dataPoint);
									graph.StartAnimation();
									if (!shadow || num9 != 0)
									{
										graph.FillRectangleRel(empty, (!shadow) ? dataPoint.Color : Color.Transparent, dataPoint.BackHatchStyle, dataPoint.BackImage, dataPoint.BackImageMode, dataPoint.BackImageTransparentColor, dataPoint.BackImageAlign, dataPoint.BackGradientType, (!shadow) ? dataPoint.BackGradientEndColor : Color.Transparent, dataPoint.BorderColor, dataPoint.BorderWidth, dataPoint.BorderStyle, item2.ShadowColor, num9, PenAlignment.Inset, (!shadow) ? ChartGraphics.GetBarDrawingStyle(dataPoint) : BarDrawingStyle.Default, isVertical: true);
									}
									graph.StopAnimation();
									graph.EndHotRegion();
									if (flag5)
									{
										graph.ResetClip();
									}
								}
								else
								{
									graph.StartAnimation();
									DrawLabels(common, graph, area, dataPoint, k, item2, empty);
									graph.StopAnimation();
								}
							}
						}
						if (common.ProcessModeRegions && !shadow && !labels)
						{
							common.HotRegionsList.AddHotRegion(graph, empty, dataPoint, item2.Name, k);
						}
						if (!selection)
						{
							common.EventsManager.OnPaint(item2, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
						}
						if (axis.Logarithmic)
						{
							num4 = Math.Pow(axis.logarithmBase, num4);
						}
						num++;
						if (GetYValue(common, area, item2, dataPoint, k, 0) >= 0.0)
						{
							num2 = num5;
						}
						else
						{
							num3 = num5;
						}
					}
				}
			}
			if (!flag)
			{
				return;
			}
			for (int m = 0; m < common.DataManager.Series.Count; m++)
			{
				Series series6 = common.DataManager.Series[m];
				if (string.Compare(series6.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) == 0 && !(series6.ChartArea != area.Name) && series6.IsVisible())
				{
					string text2 = GetSeriesStackGroupName(series6);
					int num10 = text2.IndexOf("__", StringComparison.Ordinal);
					if (num10 >= 0)
					{
						text2 = text2.Substring(num10 + 2);
					}
					if (text2.Length > 0)
					{
						series6["StackedGroupName"] = text2;
					}
					else
					{
						series6.DeleteAttribute("StackedGroupName");
					}
				}
			}
		}

		internal static Series[] GetSeriesByStackedGroupName(CommonElements common, string groupName, string chartTypeName, string chartAreaName)
		{
			ArrayList arrayList = new ArrayList();
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, chartTypeName, StringComparison.OrdinalIgnoreCase) == 0 && chartAreaName == item.ChartArea && item.IsVisible() && GetSeriesStackGroupName(item) == groupName)
				{
					arrayList.Add(item);
				}
			}
			int num = 0;
			Series[] array = new Series[arrayList.Count];
			foreach (Series item2 in arrayList)
			{
				array[num++] = item2;
			}
			return array;
		}

		internal static string GetSeriesStackGroupName(Series series)
		{
			string result = string.Empty;
			if (series.IsAttributeSet("StackedGroupName"))
			{
				result = series["StackedGroupName"];
			}
			return result;
		}

		internal static bool IsSeriesStackGroupNameSupported(Series series)
		{
			if (series.ChartType == SeriesChartType.StackedColumn || series.ChartType == SeriesChartType.StackedColumn100 || series.ChartType == SeriesChartType.StackedBar || series.ChartType == SeriesChartType.StackedBar100)
			{
				return true;
			}
			return false;
		}

		public void DrawLabels(CommonElements common, ChartGraphics graph, ChartArea area, DataPoint point, int pointIndex, Series series, RectangleF rectangle)
		{
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			Region clip = graph.Clip;
			graph.Clip = new Region();
			if (point.ShowLabelAsValue || point.Label.Length > 0)
			{
				string text;
				if (point.Label.Length == 0)
				{
					double value = GetYValue(common, area, series, point, pointIndex, 0);
					if (hundredPercentStacked && point.LabelFormat.Length == 0)
					{
						value = Math.Round(value, 2);
					}
					text = ValueConverter.FormatValue(series.chart, point, value, point.LabelFormat, series.YValueType, ChartElementType.DataPoint);
				}
				else
				{
					text = point.ReplaceKeywords(point.Label);
					if (series.chart != null && series.chart.LocalizeTextHandler != null)
					{
						text = series.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
					}
				}
				PointF pointF = PointF.Empty;
				pointF.X = rectangle.X + rectangle.Width / 2f;
				pointF.Y = rectangle.Y + rectangle.Height / 2f;
				int angle = point.FontAngle;
				if (text.Trim().Length != 0)
				{
					SizeF labelSize = SizeF.Empty;
					if (series.SmartLabels.Enabled)
					{
						labelSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
						bool markerOverlapping = series.SmartLabels.MarkerOverlapping;
						LabelAlignmentTypes movingDirection = series.SmartLabels.MovingDirection;
						series.SmartLabels.MarkerOverlapping = true;
						if (series.SmartLabels.MovingDirection == (LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom | LabelAlignmentTypes.Right | LabelAlignmentTypes.Left | LabelAlignmentTypes.TopLeft | LabelAlignmentTypes.TopRight | LabelAlignmentTypes.BottomLeft | LabelAlignmentTypes.BottomRight))
						{
							series.SmartLabels.MovingDirection = (LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom);
						}
						pointF = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, series.SmartLabels, pointF, labelSize, ref format, pointF, new SizeF(0f, 0f), LabelAlignmentTypes.Center);
						series.SmartLabels.MarkerOverlapping = markerOverlapping;
						series.SmartLabels.MovingDirection = movingDirection;
						angle = 0;
					}
					if (!pointF.IsEmpty)
					{
						PointF absolutePoint = graph.GetAbsolutePoint(pointF);
						if (graph.TextRenderingHint != TextRenderingHint.AntiAlias)
						{
							absolutePoint.X = (float)Math.Ceiling(absolutePoint.X) + 1f;
							pointF = graph.GetRelativePoint(absolutePoint);
						}
						if (labelSize.IsEmpty)
						{
							labelSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
						}
						RectangleF empty = RectangleF.Empty;
						SizeF sizeF = new SizeF(labelSize.Width, labelSize.Height);
						sizeF.Height += labelSize.Height / 8f;
						sizeF.Width += sizeF.Width / (float)text.Length;
						graph.DrawPointLabelStringRel(backPosition: new RectangleF(pointF.X - sizeF.Width / 2f, pointF.Y - sizeF.Height / 2f - labelSize.Height / 10f, sizeF.Width, sizeF.Height), common: common, text: text, font: point.Font, brush: new SolidBrush(point.FontColor), position: pointF, format: format, angle: angle, backColor: point.LabelBackColor, borderColor: point.LabelBorderColor, borderWidth: point.LabelBorderWidth, borderStyle: point.LabelBorderStyle, series: series, point: point, pointIndex: pointIndex);
					}
				}
			}
			graph.Clip = clip;
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			double num = double.NaN;
			if (area.Area3DStyle.Enable3D)
			{
				switch (yValueIndex)
				{
				case -2:
					break;
				case -1:
				{
					double crossing = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName).Crossing;
					num = GetYValue(common, area, series, point, pointIndex, 0);
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
				default:
					prevPosY = double.NaN;
					prevNegY = double.NaN;
					{
						foreach (Series item in common.DataManager.Series)
						{
							if (string.Compare(series.ChartArea, item.ChartArea, StringComparison.Ordinal) != 0 || string.Compare(series.ChartTypeName, item.ChartTypeName, StringComparison.OrdinalIgnoreCase) != 0 || !item.IsVisible())
							{
								continue;
							}
							string seriesStackGroupName = GetSeriesStackGroupName(item);
							if (stackGroupNameUsed && seriesStackGroupName != currentStackGroup)
							{
								continue;
							}
							if (double.IsNaN(num))
							{
								num = item.Points[pointIndex].YValues[0];
							}
							else
							{
								num = item.Points[pointIndex].YValues[0];
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
								continue;
							}
							return num;
						}
						return num;
					}
				}
			}
			return point.YValues[0];
		}

		private void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, bool labels, Series seriesToDraw)
		{
			if (labels && !selection)
			{
				return;
			}
			ArrayList arrayList = null;
			arrayList = area.GetClusterSeriesNames(seriesToDraw.Name);
			common.DataManager.GetNumberOfPoints((string[])arrayList.ToArray(typeof(string)));
			ArrayList dataPointDrawingOrder = area.GetDataPointDrawingOrder(arrayList, this, selection, COPCoordinates.X | COPCoordinates.Y, null, 0, sideBySide: false);
			bool flag = false;
			foreach (DataPoint3D item in dataPointDrawingOrder)
			{
				DataPoint dataPoint = item.dataPoint;
				Series series = dataPoint.series;
				currentStackGroup = GetSeriesStackGroupName(series);
				dataPoint.positionRel = new PointF(float.NaN, float.NaN);
				Axis axis = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
				Axis axis2 = area.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
				BarDrawingStyle barDrawingStyle = ChartGraphics.GetBarDrawingStyle(dataPoint);
				float num = 0.5f;
				float num2 = 0.5f;
				bool flag2 = true;
				bool flag3 = false;
				for (int i = 0; i < arrayList.Count; i++)
				{
					Series series2 = common.DataManager.Series[i];
					if (flag2 && item.index <= series2.Points.Count && series2.Points[item.index - 1].YValues[0] != 0.0)
					{
						flag2 = false;
						if (series2.Name == series.Name)
						{
							num2 = 0f;
						}
					}
					if (series2.Name == series.Name)
					{
						flag3 = true;
					}
					else if (item.index <= series2.Points.Count && series2.Points[item.index - 1].YValues[0] != 0.0)
					{
						flag3 = false;
					}
				}
				if (flag3)
				{
					num = 0f;
				}
				if (area.stackGroupNames != null && area.stackGroupNames.Count > 1 && area.Area3DStyle.Clustered)
				{
					string seriesStackGroupName = GetSeriesStackGroupName(series);
					bool flag4 = true;
					bool flag5 = false;
					foreach (string item2 in arrayList)
					{
						Series series3 = common.DataManager.Series[item2];
						if (!(GetSeriesStackGroupName(series3) == seriesStackGroupName))
						{
							continue;
						}
						if (flag4 && item.index < series3.Points.Count && series3.Points[item.index - 1].YValues[0] != 0.0)
						{
							flag4 = false;
							if (item2 == series.Name)
							{
								num2 = 0f;
							}
						}
						if (item2 == series.Name)
						{
							flag5 = true;
						}
						else if (item.index < series3.Points.Count && series3.Points[item.index - 1].YValues[0] != 0.0)
						{
							flag5 = false;
						}
					}
					if (flag5)
					{
						num = 0f;
					}
				}
				double yValue = GetYValue(common, area, series, item.dataPoint, item.index - 1, 0);
				double yValue2 = yValue - GetYValue(common, area, series, item.dataPoint, item.index - 1, -1);
				yValue = axis.GetLogValue(yValue);
				yValue2 = axis.GetLogValue(yValue2);
				if (yValue2 > axis.GetViewMaximum())
				{
					num = 0.5f;
					yValue2 = axis.GetViewMaximum();
				}
				else if (yValue2 < axis.GetViewMinimum())
				{
					num2 = 0.5f;
					yValue2 = axis.GetViewMinimum();
				}
				if (yValue > axis.GetViewMaximum())
				{
					num = 0.5f;
					yValue = axis.GetViewMaximum();
				}
				else if (yValue < axis.GetViewMinimum())
				{
					num2 = 0.5f;
					yValue = axis.GetViewMinimum();
				}
				double linearPosition = axis.GetLinearPosition(yValue);
				double linearPosition2 = axis.GetLinearPosition(yValue2);
				RectangleF empty = RectangleF.Empty;
				try
				{
					empty.X = (float)(item.xPosition - item.width / 2.0);
					empty.Width = (float)item.width;
					if (linearPosition2 < linearPosition)
					{
						float num3 = num2;
						num2 = num;
						num = num3;
						empty.Y = (float)linearPosition2;
						empty.Height = (float)linearPosition - empty.Y;
					}
					else
					{
						empty.Y = (float)linearPosition;
						empty.Height = (float)linearPosition2 - empty.Y;
					}
				}
				catch (Exception)
				{
					continue;
				}
				dataPoint.positionRel = new PointF((float)item.xPosition, empty.Top);
				if (dataPoint.Empty)
				{
					continue;
				}
				double yValue3 = item.indexedSeries ? ((double)item.index) : dataPoint.XValue;
				yValue3 = axis2.GetLogValue(yValue3);
				if (yValue3 < axis2.GetViewMinimum() || yValue3 > axis2.GetViewMaximum() || (yValue < axis.GetViewMinimum() && yValue2 < axis.GetViewMinimum()) || (yValue > axis.GetViewMaximum() && yValue2 > axis.GetViewMaximum()))
				{
					continue;
				}
				bool flag6 = false;
				if (empty.Right <= area.PlotAreaPosition.X || empty.X >= area.PlotAreaPosition.Right())
				{
					continue;
				}
				if (empty.X < area.PlotAreaPosition.X)
				{
					empty.Width -= area.PlotAreaPosition.X - empty.X;
					empty.X = area.PlotAreaPosition.X;
				}
				if (empty.Right > area.PlotAreaPosition.Right())
				{
					empty.Width -= empty.Right - area.PlotAreaPosition.Right();
				}
				if (empty.Width < 0f)
				{
					empty.Width = 0f;
				}
				if (empty.Height != 0f && empty.Width != 0f)
				{
					DrawingOperationTypes drawingOperationTypes = DrawingOperationTypes.DrawElement;
					if (common.ProcessModeRegions)
					{
						drawingOperationTypes |= DrawingOperationTypes.CalcElementPath;
					}
					graph.StartHotRegion(dataPoint);
					GraphicsPath path = graph.Fill3DRectangle(empty, item.zPosition, item.depth, area.matrix3D, area.Area3DStyle.Light, dataPoint.Color, num, num2, dataPoint.BackHatchStyle, dataPoint.BackImage, dataPoint.BackImageMode, dataPoint.BackImageTransparentColor, dataPoint.BackImageAlign, dataPoint.BackGradientType, dataPoint.BackGradientEndColor, dataPoint.BorderColor, dataPoint.BorderWidth, dataPoint.BorderStyle, PenAlignment.Inset, barDrawingStyle, veticalOrientation: true, drawingOperationTypes);
					graph.StopAnimation();
					graph.EndHotRegion();
					if (flag6)
					{
						graph.ResetClip();
					}
					if (common.ProcessModeRegions && !labels)
					{
						common.HotRegionsList.AddHotRegion(path, relativePath: false, graph, dataPoint, series.Name, item.index - 1);
					}
					if (dataPoint.ShowLabelAsValue || dataPoint.Label.Length > 0)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			foreach (DataPoint3D item3 in dataPointDrawingOrder)
			{
				DataPoint dataPoint2 = item3.dataPoint;
				Series series4 = dataPoint2.series;
				Axis axis3 = area.GetAxis(AxisName.Y, series4.YAxisType, series4.YSubAxisName);
				Axis axis4 = area.GetAxis(AxisName.X, series4.XAxisType, series4.XSubAxisName);
				double num4 = GetYValue(common, area, series4, item3.dataPoint, item3.index - 1, 0);
				if (num4 > axis3.GetViewMaximum())
				{
					num4 = axis3.GetViewMaximum();
				}
				if (num4 < axis3.GetViewMinimum())
				{
					num4 = axis3.GetViewMinimum();
				}
				num4 = axis3.GetLogValue(num4);
				double yPosition = item3.yPosition;
				double num5 = num4 - axis3.GetLogValue(GetYValue(common, area, series4, item3.dataPoint, item3.index - 1, -1));
				double height = item3.height;
				RectangleF empty2 = RectangleF.Empty;
				try
				{
					empty2.X = (float)(item3.xPosition - item3.width / 2.0);
					empty2.Width = (float)item3.width;
					if (height < yPosition)
					{
						empty2.Y = (float)height;
						empty2.Height = (float)yPosition - empty2.Y;
					}
					else
					{
						empty2.Y = (float)yPosition;
						empty2.Height = (float)height - empty2.Y;
					}
				}
				catch (Exception)
				{
					continue;
				}
				if (!dataPoint2.Empty && !selection)
				{
					double yValue4 = item3.indexedSeries ? ((double)item3.index) : dataPoint2.XValue;
					yValue4 = axis4.GetLogValue(yValue4);
					if (!(yValue4 < axis4.GetViewMinimum()) && !(yValue4 > axis4.GetViewMaximum()) && (!(num4 < axis3.GetViewMinimum()) || !(num5 < axis3.GetViewMinimum())) && (!(num4 > axis3.GetViewMaximum()) || !(num5 > axis3.GetViewMaximum())))
					{
						graph.StartAnimation();
						DrawLabels3D(common, graph, area, item3, item3.index - 1, series4, empty2);
						graph.StopAnimation();
					}
				}
			}
		}

		internal void DrawLabels3D(CommonElements common, ChartGraphics graph, ChartArea area, DataPoint3D pointEx, int pointIndex, Series series, RectangleF rectangle)
		{
			DataPoint dataPoint = pointEx.dataPoint;
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			Region clip = graph.Clip;
			graph.Clip = new Region();
			if (dataPoint.ShowLabelAsValue || dataPoint.Label.Length > 0)
			{
				string text;
				if (dataPoint.Label.Length == 0)
				{
					double value = GetYValue(common, area, series, dataPoint, pointIndex, -2);
					if (hundredPercentStacked && dataPoint.LabelFormat.Length == 0)
					{
						value = Math.Round(value, 2);
					}
					text = ValueConverter.FormatValue(series.chart, dataPoint, value, dataPoint.LabelFormat, series.YValueType, ChartElementType.DataPoint);
				}
				else
				{
					text = dataPoint.ReplaceKeywords(dataPoint.Label);
					if (series.chart != null && series.chart.LocalizeTextHandler != null)
					{
						text = series.chart.LocalizeTextHandler(dataPoint, text, dataPoint.ElementId, ChartElementType.DataPoint);
					}
				}
				PointF pointF = PointF.Empty;
				pointF.X = rectangle.X + rectangle.Width / 2f;
				pointF.Y = rectangle.Y + rectangle.Height / 2f;
				Point3D[] array = new Point3D[1]
				{
					new Point3D(pointF.X, pointF.Y, pointEx.zPosition + pointEx.depth)
				};
				area.matrix3D.TransformPoints(array);
				pointF.X = array[0].X;
				pointF.Y = array[0].Y;
				int angle = dataPoint.FontAngle;
				SizeF labelSize = SizeF.Empty;
				if (series.SmartLabels.Enabled)
				{
					labelSize = graph.GetRelativeSize(graph.MeasureString(text, dataPoint.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
					bool markerOverlapping = series.SmartLabels.MarkerOverlapping;
					LabelAlignmentTypes movingDirection = series.SmartLabels.MovingDirection;
					series.SmartLabels.MarkerOverlapping = true;
					if (series.SmartLabels.MovingDirection == (LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom | LabelAlignmentTypes.Right | LabelAlignmentTypes.Left | LabelAlignmentTypes.TopLeft | LabelAlignmentTypes.TopRight | LabelAlignmentTypes.BottomLeft | LabelAlignmentTypes.BottomRight))
					{
						series.SmartLabels.MovingDirection = (LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom);
					}
					pointF = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, series.SmartLabels, pointF, labelSize, ref format, pointF, new SizeF(0f, 0f), LabelAlignmentTypes.Center);
					series.SmartLabels.MarkerOverlapping = markerOverlapping;
					series.SmartLabels.MovingDirection = movingDirection;
					angle = 0;
				}
				if (!pointF.IsEmpty)
				{
					if (labelSize.IsEmpty)
					{
						labelSize = graph.GetRelativeSize(graph.MeasureString(text, dataPoint.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
					}
					RectangleF empty = RectangleF.Empty;
					SizeF sizeF = new SizeF(labelSize.Width, labelSize.Height);
					sizeF.Height += labelSize.Height / 8f;
					sizeF.Width += sizeF.Width / (float)text.Length;
					graph.DrawPointLabelStringRel(backPosition: new RectangleF(pointF.X - sizeF.Width / 2f, pointF.Y - sizeF.Height / 2f - labelSize.Height / 10f, sizeF.Width, sizeF.Height), common: common, text: text, font: dataPoint.Font, brush: new SolidBrush(dataPoint.FontColor), position: pointF, format: format, angle: angle, backColor: dataPoint.LabelBackColor, borderColor: dataPoint.LabelBorderColor, borderWidth: dataPoint.LabelBorderWidth, borderStyle: dataPoint.LabelBorderStyle, series: series, point: dataPoint, pointIndex: pointIndex);
				}
			}
			graph.Clip = clip;
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
