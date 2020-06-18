using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class StackedBarChart : IChartType
	{
		protected double prevPosY = double.NaN;

		protected double prevNegY = double.NaN;

		protected bool hundredPercentStacked;

		internal bool stackGroupNameUsed;

		internal ArrayList stackGroupNames;

		internal string currentStackGroup = string.Empty;

		public virtual string Name => "StackedBar";

		public bool Stacked => true;

		public virtual bool SupportStackedGroups => true;

		public bool StackSign => true;

		public bool RequireAxes => true;

		public bool SecondYScale => false;

		public bool CircularChartArea => false;

		public bool SupportLogarithmicAxes => true;

		public bool SwitchValueAxes => true;

		public bool SideBySideSeries => false;

		public bool ZeroCrossing => true;

		public bool DataPointsInLegend => false;

		public virtual bool ExtraYValuesConnectedToYAxis => false;

		public virtual bool HundredPercent => false;

		public virtual bool HundredPercentSupportNegative => false;

		public bool ApplyPaletteColorsToPoints => false;

		public int YValuesPerPoint => 1;

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(Name + "ChartType");
		}

		public LegendImageStyle GetLegendImageStyle(Series series)
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
						string seriesStackGroupName = StackedColumnChart.GetSeriesStackGroupName(series2);
						seriesStackGroupName = (series2["StackedGroupName"] = "_X_" + series2.XAxisType.ToString() + series2.XSubAxisName + "_Y_" + series2.YAxisType.ToString() + series2.YSubAxisName + "__");
					}
				}
			}
			stackGroupNames = new ArrayList();
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) == 0 && !(item.ChartArea != area.Name) && item.IsVisible())
				{
					string seriesStackGroupName2 = StackedColumnChart.GetSeriesStackGroupName(item);
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
						if (string.Compare(item2.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) != 0 || item2.ChartArea != area.Name || !item2.IsVisible() || k >= item2.Points.Count || StackedColumnChart.GetSeriesStackGroupName(item2) != currentStackGroup)
						{
							continue;
						}
						DataPoint dataPoint = item2.Points[k];
						dataPoint.positionRel = new PointF(float.NaN, float.NaN);
						Axis axis = area.GetAxis(AxisName.X, item2.XAxisType, item2.XSubAxisName);
						Axis axis2 = area.GetAxis(AxisName.Y, item2.YAxisType, item2.YSubAxisName);
						double interval = 1.0;
						if (!flag2)
						{
							if (item2.Points.Count == 1 && (item2.XValueType == ChartValueTypes.Date || item2.XValueType == ChartValueTypes.DateTime || item2.XValueType == ChartValueTypes.Time || item2.XValueType == ChartValueTypes.DateTimeOffset))
							{
								bool sameInterval = false;
								ArrayList seriesFromChartType = area.GetSeriesFromChartType(Name);
								area.GetPointsInterval(seriesFromChartType, axis.Logarithmic, axis.logarithmBase, checkSameInterval: true, out sameInterval);
								interval = ((double.IsNaN(axis.majorGrid.Interval) || axis.majorGrid.IntervalType == DateTimeIntervalType.NotSet) ? axis.GetIntervalSize(axis.minimum, axis.Interval, axis.IntervalType) : axis.GetIntervalSize(axis.minimum, axis.majorGrid.Interval, axis.majorGrid.IntervalType));
							}
							else
							{
								interval = area.GetPointsInterval(axis.Logarithmic, axis.logarithmBase);
							}
						}
						double pointWidth = item2.GetPointWidth(graph, axis, interval, 0.8);
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
						if (axis2.Logarithmic)
						{
							num4 = Math.Log(num4, axis2.logarithmBase);
						}
						double linearPosition = axis2.GetLinearPosition(num4);
						double num6 = dataPoint.XValue;
						if (flag2)
						{
							num6 = (double)k + 1.0;
						}
						double num7 = axis.GetPosition(num6);
						if (stackGroupNames.Count > 1)
						{
							num7 = num7 - pointWidth * (double)stackGroupNames.Count / 2.0 + pointWidth / 2.0 + (double)l * pointWidth;
						}
						num6 = axis.GetLogValue(num6);
						double num8 = (num == 0) ? ((!(flag3 && labels)) ? axis2.Crossing : 0.0) : ((!(GetYValue(common, area, item2, dataPoint, k, 0) >= 0.0)) ? num3 : num2);
						double position = axis2.GetPosition(num8);
						RectangleF empty = RectangleF.Empty;
						try
						{
							empty.Y = (float)(num7 - pointWidth / 2.0);
							empty.Height = (float)pointWidth;
							if (position < linearPosition)
							{
								empty.X = (float)position;
								empty.Width = (float)linearPosition - empty.X;
							}
							else
							{
								empty.X = (float)linearPosition;
								empty.Width = (float)position - empty.X;
							}
						}
						catch (Exception)
						{
							continue;
						}
						dataPoint.positionRel = new PointF(empty.Right, (float)num7);
						if (dataPoint.Empty)
						{
							continue;
						}
						if (axis2.Logarithmic)
						{
							num8 = Math.Log(num8, axis2.logarithmBase);
						}
						bool flag4 = false;
						if (num6 < axis.GetViewMinimum() || num6 > axis.GetViewMaximum() || (num4 < axis2.GetViewMinimum() && num8 < axis2.GetViewMinimum()) || (num4 > axis2.GetViewMaximum() && num8 > axis2.GetViewMaximum()))
						{
							flag4 = true;
						}
						if (!flag4)
						{
							if (common.ProcessModePaint)
							{
								bool flag5 = false;
								if (empty.Y < area.PlotAreaPosition.Y || empty.Bottom > area.PlotAreaPosition.Bottom() || empty.X < area.PlotAreaPosition.X || empty.Right > area.PlotAreaPosition.Right())
								{
									graph.SetClip(area.PlotAreaPosition.ToRectangleF());
									flag5 = true;
								}
								int shadowOffset = 0;
								if (shadow)
								{
									shadowOffset = item2.ShadowOffset;
								}
								if (!labels)
								{
									graph.StartHotRegion(dataPoint);
									graph.StartAnimation();
									graph.FillRectangleRel(empty, (!shadow) ? dataPoint.Color : Color.Transparent, dataPoint.BackHatchStyle, dataPoint.BackImage, dataPoint.BackImageMode, dataPoint.BackImageTransparentColor, dataPoint.BackImageAlign, dataPoint.BackGradientType, (!shadow) ? dataPoint.BackGradientEndColor : Color.Transparent, dataPoint.BorderColor, dataPoint.BorderWidth, dataPoint.BorderStyle, item2.ShadowColor, shadowOffset, PenAlignment.Inset, (!shadow) ? ChartGraphics.GetBarDrawingStyle(dataPoint) : BarDrawingStyle.Default, isVertical: false);
									graph.StopAnimation();
									graph.EndHotRegion();
								}
								else
								{
									graph.StartAnimation();
									RectangleF rectangle = new RectangleF(empty.Location, empty.Size);
									if (flag5 && !flag3)
									{
										rectangle.Intersect(area.PlotAreaPosition.ToRectangleF());
									}
									DrawLabels(common, graph, area, dataPoint, k, item2, rectangle);
									graph.StopAnimation();
								}
								if (flag5)
								{
									graph.ResetClip();
								}
							}
							if (common.ProcessModeRegions && !shadow && !labels)
							{
								common.HotRegionsList.AddHotRegion(graph, empty, dataPoint, item2.Name, k);
								if (labels && !common.ProcessModePaint)
								{
									DrawLabels(common, graph, area, dataPoint, k, item2, empty);
								}
							}
							if (!selection)
							{
								common.EventsManager.OnPaint(item2, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
							}
						}
						if (axis2.Logarithmic)
						{
							num4 = Math.Pow(axis2.logarithmBase, num4);
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
					string text2 = StackedColumnChart.GetSeriesStackGroupName(series6);
					int num9 = text2.IndexOf("__", StringComparison.Ordinal);
					if (num9 >= 0)
					{
						text2 = text2.Substring(num9 + 2);
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

		public void DrawLabels(CommonElements common, ChartGraphics graph, ChartArea area, DataPoint point, int pointIndex, Series series, RectangleF rectangle)
		{
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			Region clip = graph.Clip;
			graph.Clip = new Region();
			if (point.ShowLabelAsValue || point.Label.Length > 0)
			{
				double value = GetYValue(common, area, series, point, pointIndex, 0);
				if (hundredPercentStacked && point.LabelFormat.Length == 0)
				{
					value = Math.Round(value, 2);
				}
				string text;
				if (point.Label.Length == 0)
				{
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
					SizeF relativeSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
					BarValueLabelDrawingStyle barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Center;
					string text2 = "";
					if (point.IsAttributeSet("BarLabelStyle"))
					{
						text2 = point["BarLabelStyle"];
					}
					else if (series.IsAttributeSet("BarLabelStyle"))
					{
						text2 = series["BarLabelStyle"];
					}
					if (text2 != null && text2.Length > 0)
					{
						if (string.Compare(text2, "Left", StringComparison.OrdinalIgnoreCase) == 0)
						{
							barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Left;
						}
						else if (string.Compare(text2, "Right", StringComparison.OrdinalIgnoreCase) == 0)
						{
							barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Right;
						}
						else if (string.Compare(text2, "Center", StringComparison.OrdinalIgnoreCase) == 0)
						{
							barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Center;
						}
						else if (string.Compare(text2, "Outside", StringComparison.OrdinalIgnoreCase) == 0)
						{
							barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Outside;
						}
					}
					switch (barValueLabelDrawingStyle)
					{
					case BarValueLabelDrawingStyle.Left:
						pointF.X = rectangle.X + relativeSize.Width / 2f;
						break;
					case BarValueLabelDrawingStyle.Right:
						pointF.X = rectangle.Right - relativeSize.Width / 2f;
						break;
					case BarValueLabelDrawingStyle.Outside:
						pointF.X = rectangle.Right + relativeSize.Width / 2f;
						break;
					}
					if (series.SmartLabels.Enabled)
					{
						bool markerOverlapping = series.SmartLabels.MarkerOverlapping;
						LabelAlignmentTypes movingDirection = series.SmartLabels.MovingDirection;
						series.SmartLabels.MarkerOverlapping = true;
						if (series.SmartLabels.MovingDirection == (LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom | LabelAlignmentTypes.Right | LabelAlignmentTypes.Left | LabelAlignmentTypes.TopLeft | LabelAlignmentTypes.TopRight | LabelAlignmentTypes.BottomLeft | LabelAlignmentTypes.BottomRight))
						{
							series.SmartLabels.MovingDirection = (LabelAlignmentTypes.Right | LabelAlignmentTypes.Left);
						}
						pointF = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, series.SmartLabels, pointF, relativeSize, ref format, pointF, new SizeF(0f, 0f), LabelAlignmentTypes.Center);
						series.SmartLabels.MarkerOverlapping = markerOverlapping;
						series.SmartLabels.MovingDirection = movingDirection;
						angle = 0;
					}
					if (!pointF.IsEmpty)
					{
						RectangleF empty = RectangleF.Empty;
						SizeF size = new SizeF(relativeSize.Width, relativeSize.Height);
						size.Height += relativeSize.Height / 8f;
						size.Width += size.Width / (float)text.Length;
						empty = new RectangleF(pointF.X - size.Width / 2f, pointF.Y - size.Height / 2f - relativeSize.Height / 10f, size.Width, size.Height);
						empty = area.smartLabels.GetLabelPosition(graph, pointF, size, format, adjustForDrawing: true);
						graph.DrawPointLabelStringRel(common, text, point.Font, new SolidBrush(point.FontColor), pointF, format, angle, empty, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, series, point, pointIndex);
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
							string seriesStackGroupName = StackedColumnChart.GetSeriesStackGroupName(item);
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

		private void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, bool drawLabels, Series seriesToDraw)
		{
			ArrayList arrayList = null;
			arrayList = area.GetClusterSeriesNames(seriesToDraw.Name);
			common.DataManager.GetNumberOfPoints((string[])arrayList.ToArray(typeof(string)));
			ArrayList dataPointDrawingOrder = area.GetDataPointDrawingOrder(arrayList, this, selection, COPCoordinates.X | COPCoordinates.Y, new BarPointsDrawingOrderComparer(area, selection, COPCoordinates.X | COPCoordinates.Y), 0, sideBySide: false);
			if (!drawLabels)
			{
				foreach (DataPoint3D item in dataPointDrawingOrder)
				{
					DataPoint dataPoint = item.dataPoint;
					Series series = dataPoint.series;
					currentStackGroup = StackedColumnChart.GetSeriesStackGroupName(series);
					dataPoint.positionRel = new PointF(float.NaN, float.NaN);
					Axis axis = area.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
					Axis axis2 = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
					BarDrawingStyle barDrawingStyle = ChartGraphics.GetBarDrawingStyle(dataPoint);
					float num = 0.5f;
					float num2 = 0.5f;
					bool flag = true;
					bool flag2 = false;
					for (int i = 0; i < arrayList.Count; i++)
					{
						Series series2 = common.DataManager.Series[i];
						if (flag && item.index <= series2.Points.Count && series2.Points[item.index - 1].YValues[0] != 0.0)
						{
							flag = false;
							if (series2.Name == series.Name)
							{
								num = 0f;
							}
						}
						if (series2.Name == series.Name)
						{
							flag2 = true;
						}
						else if (item.index <= series2.Points.Count && series2.Points[item.index - 1].YValues[0] != 0.0)
						{
							flag2 = false;
						}
					}
					if (flag2)
					{
						num2 = 0f;
					}
					if (area.stackGroupNames != null && area.stackGroupNames.Count > 1 && area.Area3DStyle.Clustered)
					{
						string seriesStackGroupName = StackedColumnChart.GetSeriesStackGroupName(series);
						bool flag3 = true;
						bool flag4 = false;
						foreach (string item2 in arrayList)
						{
							Series series3 = common.DataManager.Series[item2];
							if (!(StackedColumnChart.GetSeriesStackGroupName(series3) == seriesStackGroupName))
							{
								continue;
							}
							if (flag3 && item.index < series3.Points.Count && series3.Points[item.index - 1].YValues[0] != 0.0)
							{
								flag3 = false;
								if (item2 == series.Name)
								{
									num = 0f;
								}
							}
							if (item2 == series.Name)
							{
								flag4 = true;
							}
							else if (item.index < series3.Points.Count && series3.Points[item.index - 1].YValues[0] != 0.0)
							{
								flag4 = false;
							}
						}
						if (flag4)
						{
							num2 = 0f;
						}
					}
					double yValue = GetYValue(common, area, series, item.dataPoint, item.index - 1, 0);
					double yValue2 = yValue - GetYValue(common, area, series, item.dataPoint, item.index - 1, -1);
					yValue = axis2.GetLogValue(yValue);
					yValue2 = axis2.GetLogValue(yValue2);
					if (yValue2 > axis2.GetViewMaximum())
					{
						num2 = 0.5f;
						yValue2 = axis2.GetViewMaximum();
					}
					else if (yValue2 < axis2.GetViewMinimum())
					{
						num = 0.5f;
						yValue2 = axis2.GetViewMinimum();
					}
					if (yValue > axis2.GetViewMaximum())
					{
						num2 = 0.5f;
						yValue = axis2.GetViewMaximum();
					}
					else if (yValue < axis2.GetViewMinimum())
					{
						num = 0.5f;
						yValue = axis2.GetViewMinimum();
					}
					double linearPosition = axis2.GetLinearPosition(yValue);
					double linearPosition2 = axis2.GetLinearPosition(yValue2);
					double yValue3 = item.indexedSeries ? ((double)item.index) : dataPoint.XValue;
					yValue3 = axis.GetLogValue(yValue3);
					RectangleF empty = RectangleF.Empty;
					try
					{
						empty.Y = (float)(item.xPosition - item.width / 2.0);
						empty.Height = (float)item.width;
						if (linearPosition2 < linearPosition)
						{
							float num3 = num2;
							num2 = num;
							num = num3;
							empty.X = (float)linearPosition2;
							empty.Width = (float)linearPosition - empty.X;
						}
						else
						{
							empty.X = (float)linearPosition;
							empty.Width = (float)linearPosition2 - empty.X;
						}
					}
					catch (Exception)
					{
						continue;
					}
					dataPoint.positionRel = new PointF(empty.Right, (float)item.xPosition);
					if (dataPoint.Empty)
					{
						continue;
					}
					GraphicsPath graphicsPath = null;
					if (yValue3 < axis.GetViewMinimum() || yValue3 > axis.GetViewMaximum() || (yValue < axis2.GetViewMinimum() && yValue2 < axis2.GetViewMinimum()) || (yValue > axis2.GetViewMaximum() && yValue2 > axis2.GetViewMaximum()))
					{
						continue;
					}
					bool flag5 = false;
					if (empty.Bottom <= area.PlotAreaPosition.Y || empty.Y >= area.PlotAreaPosition.Bottom())
					{
						continue;
					}
					if (empty.Y < area.PlotAreaPosition.Y)
					{
						empty.Height -= area.PlotAreaPosition.Y - empty.Y;
						empty.Y = area.PlotAreaPosition.Y;
					}
					if (empty.Bottom > area.PlotAreaPosition.Bottom())
					{
						empty.Height -= empty.Bottom - area.PlotAreaPosition.Bottom();
					}
					if (empty.Height < 0f)
					{
						empty.Height = 0f;
					}
					if (empty.Height != 0f && empty.Width != 0f)
					{
						DrawingOperationTypes drawingOperationTypes = DrawingOperationTypes.DrawElement;
						if (common.ProcessModeRegions)
						{
							drawingOperationTypes |= DrawingOperationTypes.CalcElementPath;
						}
						graph.StartHotRegion(dataPoint);
						graph.StartAnimation();
						graphicsPath = graph.Fill3DRectangle(empty, item.zPosition, item.depth, area.matrix3D, area.Area3DStyle.Light, dataPoint.Color, num, num2, dataPoint.BackHatchStyle, dataPoint.BackImage, dataPoint.BackImageMode, dataPoint.BackImageTransparentColor, dataPoint.BackImageAlign, dataPoint.BackGradientType, dataPoint.BackGradientEndColor, dataPoint.BorderColor, dataPoint.BorderWidth, dataPoint.BorderStyle, PenAlignment.Inset, barDrawingStyle, veticalOrientation: false, drawingOperationTypes);
						graph.StopAnimation();
						graph.EndHotRegion();
						if (flag5)
						{
							graph.ResetClip();
						}
						if (common.ProcessModeRegions && !drawLabels)
						{
							common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: false, graph, dataPoint, series.Name, item.index - 1);
						}
					}
				}
			}
			if (!drawLabels)
			{
				return;
			}
			foreach (DataPoint3D item3 in dataPointDrawingOrder)
			{
				DataPoint dataPoint2 = item3.dataPoint;
				Series series4 = dataPoint2.series;
				Axis axis3 = area.GetAxis(AxisName.X, series4.XAxisType, series4.XSubAxisName);
				Axis axis4 = area.GetAxis(AxisName.Y, series4.YAxisType, series4.YSubAxisName);
				double num4 = GetYValue(common, area, series4, item3.dataPoint, item3.index - 1, 0);
				if (axis4.Logarithmic)
				{
					num4 = Math.Log(num4, axis4.logarithmBase);
				}
				double yPosition = item3.yPosition;
				double num5 = item3.indexedSeries ? ((double)item3.index) : dataPoint2.XValue;
				double num6 = num4 - GetYValue(common, area, series4, item3.dataPoint, item3.index - 1, -1);
				double height = item3.height;
				RectangleF empty2 = RectangleF.Empty;
				try
				{
					empty2.Y = (float)(item3.xPosition - item3.width / 2.0);
					empty2.Height = (float)item3.width;
					if (height < yPosition)
					{
						empty2.X = (float)height;
						empty2.Width = (float)yPosition - empty2.X;
					}
					else
					{
						empty2.X = (float)yPosition;
						empty2.Width = (float)height - empty2.X;
					}
				}
				catch (Exception)
				{
					continue;
				}
				if (!dataPoint2.Empty)
				{
					if (axis4.Logarithmic)
					{
						num6 = Math.Log(num6, axis4.logarithmBase);
					}
					if (!(num5 < axis3.GetViewMinimum()) && !(num5 > axis3.GetViewMaximum()) && (!(num4 < axis4.GetViewMinimum()) || !(num6 < axis4.GetViewMinimum())) && (!(num4 > axis4.GetViewMaximum()) || !(num6 > axis4.GetViewMaximum())))
					{
						graph.StartAnimation();
						DrawLabels3D(area, axis4, graph, common, empty2, item3, series4, num6, yPosition, item3.width, item3.index - 1);
						graph.StopAnimation();
					}
				}
			}
		}

		private void DrawLabels3D(ChartArea area, Axis hAxis, ChartGraphics graph, CommonElements common, RectangleF rectSize, DataPoint3D pointEx, Series ser, double barStartPosition, double barSize, double width, int pointIndex)
		{
			DataPoint dataPoint = pointEx.dataPoint;
			if (!ser.ShowLabelAsValue && !dataPoint.ShowLabelAsValue && dataPoint.Label.Length <= 0)
			{
				return;
			}
			RectangleF rectangleF = RectangleF.Empty;
			StringFormat format = new StringFormat();
			string text;
			if (dataPoint.Label.Length == 0)
			{
				double value = GetYValue(common, area, ser, dataPoint, pointIndex, -2);
				if (hundredPercentStacked && dataPoint.LabelFormat.Length == 0)
				{
					value = Math.Round(value, 2);
				}
				text = ValueConverter.FormatValue(ser.chart, dataPoint, value, dataPoint.LabelFormat, ser.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = dataPoint.ReplaceKeywords(dataPoint.Label);
				if (ser.chart != null && ser.chart.LocalizeTextHandler != null)
				{
					text = ser.chart.LocalizeTextHandler(dataPoint, text, dataPoint.ElementId, ChartElementType.DataPoint);
				}
			}
			SizeF size = SizeF.Empty;
			if ((dataPoint.MarkerStyle != 0 || dataPoint.MarkerImage.Length > 0) && pointEx.index % ser.MarkerStep == 0)
			{
				if (dataPoint.MarkerImage.Length == 0)
				{
					size.Width = dataPoint.MarkerSize;
					size.Height = dataPoint.MarkerSize;
				}
				else
				{
					Image image = common.ImageLoader.LoadImage(dataPoint.MarkerImage);
					size.Width = image.Width;
					size.Height = image.Height;
				}
				size = graph.GetRelativeSize(size);
			}
			BarValueLabelDrawingStyle barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Center;
			string text2 = "";
			if (dataPoint.IsAttributeSet("BarLabelStyle"))
			{
				text2 = dataPoint["BarLabelStyle"];
			}
			else if (ser.IsAttributeSet("BarLabelStyle"))
			{
				text2 = ser["BarLabelStyle"];
			}
			if (text2 != null && text2.Length > 0)
			{
				if (string.Compare(text2, "Left", StringComparison.OrdinalIgnoreCase) == 0)
				{
					barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Left;
				}
				else if (string.Compare(text2, "Right", StringComparison.OrdinalIgnoreCase) == 0)
				{
					barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Right;
				}
				else if (string.Compare(text2, "Center", StringComparison.OrdinalIgnoreCase) == 0)
				{
					barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Center;
				}
				else if (string.Compare(text2, "Outside", StringComparison.OrdinalIgnoreCase) == 0)
				{
					barValueLabelDrawingStyle = BarValueLabelDrawingStyle.Outside;
				}
			}
			bool flag = false;
			while (!flag)
			{
				format.Alignment = StringAlignment.Near;
				format.LineAlignment = StringAlignment.Center;
				if (barStartPosition < barSize)
				{
					rectangleF.X = rectSize.Right;
					rectangleF.Width = area.PlotAreaPosition.Right() - rectSize.Right;
				}
				else
				{
					rectangleF.X = area.PlotAreaPosition.X;
					rectangleF.Width = rectSize.X - area.PlotAreaPosition.X;
				}
				rectangleF.Y = rectSize.Y - (float)width / 2f;
				rectangleF.Height = rectSize.Height + (float)width;
				switch (barValueLabelDrawingStyle)
				{
				case BarValueLabelDrawingStyle.Outside:
					if (!size.IsEmpty)
					{
						rectangleF.Width -= Math.Min(rectangleF.Width, size.Width / 2f);
						if (barStartPosition < barSize)
						{
							rectangleF.X += Math.Min(rectangleF.Width, size.Width / 2f);
						}
					}
					break;
				case BarValueLabelDrawingStyle.Left:
					rectangleF = rectSize;
					format.Alignment = StringAlignment.Near;
					break;
				case BarValueLabelDrawingStyle.Center:
					rectangleF = rectSize;
					format.Alignment = StringAlignment.Center;
					break;
				case BarValueLabelDrawingStyle.Right:
					rectangleF = rectSize;
					format.Alignment = StringAlignment.Far;
					if (!size.IsEmpty)
					{
						rectangleF.Width -= Math.Min(rectangleF.Width, size.Width / 2f);
						if (barStartPosition >= barSize)
						{
							rectangleF.X += Math.Min(rectangleF.Width, size.Width / 2f);
						}
					}
					break;
				}
				if (barStartPosition >= barSize)
				{
					if (format.Alignment == StringAlignment.Far)
					{
						format.Alignment = StringAlignment.Near;
					}
					else if (format.Alignment == StringAlignment.Near)
					{
						format.Alignment = StringAlignment.Far;
					}
				}
				flag = true;
			}
			SizeF sizeF = graph.MeasureStringRel(text, dataPoint.Font, new SizeF(rectangleF.Width, rectangleF.Height), format);
			PointF empty = PointF.Empty;
			if (format.Alignment == StringAlignment.Near)
			{
				empty.X = rectangleF.X + sizeF.Width / 2f;
			}
			else if (format.Alignment == StringAlignment.Far)
			{
				empty.X = rectangleF.Right - sizeF.Width / 2f;
			}
			else
			{
				empty.X = (rectangleF.Left + rectangleF.Right) / 2f;
			}
			if (format.LineAlignment == StringAlignment.Near)
			{
				empty.Y = rectangleF.Top + sizeF.Height / 2f;
			}
			else if (format.LineAlignment == StringAlignment.Far)
			{
				empty.Y = rectangleF.Bottom - sizeF.Height / 2f;
			}
			else
			{
				empty.Y = (rectangleF.Bottom + rectangleF.Top) / 2f;
			}
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			int num = dataPoint.FontAngle;
			Point3D[] array = new Point3D[2]
			{
				new Point3D(empty.X, empty.Y, pointEx.zPosition + pointEx.depth),
				new Point3D(empty.X - 20f, empty.Y, pointEx.zPosition + pointEx.depth)
			};
			area.matrix3D.TransformPoints(array);
			empty = array[0].PointF;
			if (num == 0 || num == 180)
			{
				array[0].PointF = graph.GetAbsolutePoint(array[0].PointF);
				array[1].PointF = graph.GetAbsolutePoint(array[1].PointF);
				float num2 = (float)Math.Atan((array[1].Y - array[0].Y) / (array[1].X - array[0].X));
				num2 = (float)Math.Round(num2 * 180f / (float)Math.PI);
				num += (int)num2;
			}
			SizeF labelSize = SizeF.Empty;
			if (ser.SmartLabels.Enabled)
			{
				labelSize = graph.GetRelativeSize(graph.MeasureString(text, dataPoint.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
				bool markerOverlapping = ser.SmartLabels.MarkerOverlapping;
				LabelAlignmentTypes movingDirection = ser.SmartLabels.MovingDirection;
				ser.SmartLabels.MarkerOverlapping = true;
				if (ser.SmartLabels.MovingDirection == (LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom | LabelAlignmentTypes.Right | LabelAlignmentTypes.Left | LabelAlignmentTypes.TopLeft | LabelAlignmentTypes.TopRight | LabelAlignmentTypes.BottomLeft | LabelAlignmentTypes.BottomRight))
				{
					ser.SmartLabels.MovingDirection = (LabelAlignmentTypes.Right | LabelAlignmentTypes.Left);
				}
				empty = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, ser.SmartLabels, empty, labelSize, ref format, empty, new SizeF(0f, 0f), LabelAlignmentTypes.Center);
				ser.SmartLabels.MarkerOverlapping = markerOverlapping;
				ser.SmartLabels.MovingDirection = movingDirection;
				num = 0;
			}
			if (!empty.IsEmpty)
			{
				if (labelSize.IsEmpty)
				{
					labelSize = graph.GetRelativeSize(graph.MeasureString(text, dataPoint.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
				}
				RectangleF empty2 = RectangleF.Empty;
				SizeF sizeF2 = new SizeF(labelSize.Width, labelSize.Height);
				sizeF2.Height += labelSize.Height / 8f;
				sizeF2.Width += sizeF2.Width / (float)text.Length;
				graph.DrawPointLabelStringRel(backPosition: new RectangleF(empty.X - sizeF2.Width / 2f, empty.Y - sizeF2.Height / 2f - labelSize.Height / 10f, sizeF2.Width, sizeF2.Height), common: common, text: text, font: dataPoint.Font, brush: new SolidBrush(dataPoint.FontColor), position: empty, format: format, angle: num, backColor: dataPoint.LabelBackColor, borderColor: dataPoint.LabelBorderColor, borderWidth: dataPoint.LabelBorderWidth, borderStyle: dataPoint.LabelBorderStyle, series: ser, point: dataPoint, pointIndex: pointIndex);
			}
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
