using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class ErrorBarChart : IChartType
	{
		protected Axis vAxis;

		protected Axis hAxis;

		public virtual string Name => "ErrorBar";

		public virtual bool Stacked => false;

		public virtual bool SupportStackedGroups => false;

		public bool StackSign => false;

		public virtual bool RequireAxes => true;

		public bool SecondYScale => false;

		public bool CircularChartArea => false;

		public virtual bool SupportLogarithmicAxes => true;

		public virtual bool SwitchValueAxes => false;

		public bool SideBySideSeries => false;

		public virtual bool DataPointsInLegend => false;

		public virtual bool ZeroCrossing => false;

		public virtual bool ApplyPaletteColorsToPoints => false;

		public virtual bool ExtraYValuesConnectedToYAxis => true;

		public virtual bool HundredPercent => false;

		public virtual bool HundredPercentSupportNegative => false;

		public virtual int YValuesPerPoint => 3;

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
			ProcessChartType(selection: false, graph, common, area, seriesToDraw);
		}

		protected virtual void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			if (area.Area3DStyle.Enable3D)
			{
				ProcessChartType3D(selection, graph, common, area, seriesToDraw);
				return;
			}
			ArrayList seriesFromChartType = area.GetSeriesFromChartType(Name);
			bool flag = area.IndexedSeries((string[])seriesFromChartType.ToArray(typeof(string)));
			int num = 0;
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) != 0 || item.ChartArea != area.Name || !item.IsVisible())
				{
					continue;
				}
				hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
				vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
				double interval = flag ? 1.0 : area.GetPointsInterval(hAxis.Logarithmic, hAxis.logarithmBase);
				float num2 = (float)item.GetPointWidth(graph, hAxis, interval, 0.4);
				float num3 = num2;
				int num4 = 1;
				int num5 = 0;
				bool sameInterval = false;
				string empty = string.Empty;
				bool flag2 = false;
				if (item.IsAttributeSet("ErrorBarSeries"))
				{
					empty = item["ErrorBarSeries"];
					int num6 = empty.IndexOf(":", StringComparison.Ordinal);
					if (num6 >= 0)
					{
						empty = empty.Substring(0, num6);
					}
					string chartTypeName = common.DataManager.Series[empty].ChartTypeName;
					ArrayList seriesFromChartType2 = common.chartAreaCollection[common.DataManager.Series[empty].ChartArea].GetSeriesFromChartType(chartTypeName);
					{
						IEnumerator enumerator2 = seriesFromChartType2.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext() && !((string)enumerator2.Current == empty))
							{
								num5++;
							}
						}
						finally
						{
							IDisposable disposable = enumerator2 as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
					flag2 = false;
					if (string.Compare(chartTypeName, "Column", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(chartTypeName, "RangeColumn", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag2 = true;
					}
					foreach (string item2 in seriesFromChartType2)
					{
						if (common.DataManager.Series[item2].IsAttributeSet("DrawSideBySide"))
						{
							string strA = common.DataManager.Series[item2]["DrawSideBySide"];
							if (string.Compare(strA, "False", StringComparison.OrdinalIgnoreCase) == 0)
							{
								flag2 = false;
							}
							else if (string.Compare(strA, "True", StringComparison.OrdinalIgnoreCase) == 0)
							{
								flag2 = true;
							}
							else if (string.Compare(strA, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
							{
								throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
							}
						}
					}
					if (flag2)
					{
						num4 = seriesFromChartType2.Count;
						num2 /= (float)num4;
						sameInterval = true;
						if (!flag)
						{
							area.GetPointsInterval(seriesFromChartType2, hAxis.Logarithmic, hAxis.logarithmBase, checkSameInterval: true, out sameInterval);
						}
						num3 = (float)common.DataManager.Series[empty].GetPointWidth(graph, hAxis, interval, 0.8) / (float)num4;
					}
				}
				if (!flag2 && item.IsAttributeSet("DrawSideBySide"))
				{
					string strA2 = item["DrawSideBySide"];
					if (string.Compare(strA2, "False", StringComparison.OrdinalIgnoreCase) == 0)
					{
						sameInterval = false;
					}
					else if (string.Compare(strA2, "True", StringComparison.OrdinalIgnoreCase) == 0)
					{
						sameInterval = true;
						num4 = seriesFromChartType.Count;
						num5 = num;
						num2 /= (float)num4;
						num3 = (float)item.GetPointWidth(graph, hAxis, interval, 0.8) / (float)num4;
					}
					else if (string.Compare(strA2, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
					}
				}
				if (!selection)
				{
					common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				int num7 = 1;
				foreach (DataPoint point in item.Points)
				{
					if (point.YValues.Length < YValuesPerPoint)
					{
						throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(Name, YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
					}
					point.positionRel = new PointF(float.NaN, float.NaN);
					float num8 = 0f;
					double num9 = point.XValue;
					if (flag)
					{
						num9 = num7;
					}
					num8 = ((!sameInterval) ? ((float)hAxis.GetPosition(num9)) : ((float)(hAxis.GetPosition(num9) - (double)num3 * (double)num4 / 2.0 + (double)(num3 / 2f) + (double)((float)num5 * num3))));
					double logValue = vAxis.GetLogValue(point.YValues[1]);
					double logValue2 = vAxis.GetLogValue(point.YValues[2]);
					num9 = hAxis.GetLogValue(num9);
					if (num9 < hAxis.GetViewMinimum() || num9 > hAxis.GetViewMaximum() || (logValue < vAxis.GetViewMinimum() && logValue2 < vAxis.GetViewMinimum()) || (logValue > vAxis.GetViewMaximum() && logValue2 > vAxis.GetViewMaximum()))
					{
						num7++;
						continue;
					}
					double num10 = vAxis.GetLogValue(point.YValues[1]);
					double num11 = vAxis.GetLogValue(point.YValues[2]);
					ErrorBarStyle barStyle = ErrorBarStyle.Both;
					if (point.IsAttributeSet("ErrorBarStyle") || item.IsAttributeSet("ErrorBarStyle"))
					{
						string strA3 = item["ErrorBarStyle"];
						if (point.IsAttributeSet("ErrorBarStyle"))
						{
							strA3 = point["ErrorBarStyle"];
						}
						if (string.Compare(strA3, "Both", StringComparison.OrdinalIgnoreCase) != 0)
						{
							if (string.Compare(strA3, "UpperError", StringComparison.OrdinalIgnoreCase) == 0)
							{
								barStyle = ErrorBarStyle.UpperError;
								num10 = vAxis.GetLogValue(point.YValues[0]);
								num11 = vAxis.GetLogValue(point.YValues[2]);
							}
							else
							{
								if (string.Compare(strA3, "LowerError", StringComparison.OrdinalIgnoreCase) != 0)
								{
									throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(point["ErrorBarStyle"], "ErrorBarStyle"));
								}
								barStyle = ErrorBarStyle.LowerError;
								num10 = vAxis.GetLogValue(point.YValues[1]);
								num11 = vAxis.GetLogValue(point.YValues[0]);
							}
						}
					}
					if (num11 > vAxis.GetViewMaximum())
					{
						num11 = vAxis.GetViewMaximum();
					}
					if (num11 < vAxis.GetViewMinimum())
					{
						num11 = vAxis.GetViewMinimum();
					}
					num11 = (float)vAxis.GetLinearPosition(num11);
					if (num10 > vAxis.GetViewMaximum())
					{
						num10 = vAxis.GetViewMaximum();
					}
					if (num10 < vAxis.GetViewMinimum())
					{
						num10 = vAxis.GetViewMinimum();
					}
					num10 = vAxis.GetLinearPosition(num10);
					point.positionRel = new PointF(num8, (float)Math.Min(num11, num10));
					if (common.ProcessModePaint)
					{
						bool flag3 = false;
						if (num9 == hAxis.GetViewMinimum() || num9 == hAxis.GetViewMaximum())
						{
							graph.SetClip(area.PlotAreaPosition.ToRectangleF());
							flag3 = true;
						}
						graph.StartAnimation();
						graph.StartHotRegion(point);
						graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, new PointF(num8, (float)num11), new PointF(num8, (float)num10), item.ShadowColor, item.ShadowOffset);
						DrawErrorBarMarks(graph, barStyle, area, item, point, num8, num2);
						graph.StopAnimation();
						graph.EndHotRegion();
						if (flag3)
						{
							graph.ResetClip();
						}
					}
					if (common.ProcessModeRegions)
					{
						RectangleF empty2 = RectangleF.Empty;
						empty2.X = num8 - num2 / 2f;
						empty2.Y = (float)Math.Min(num11, num10);
						empty2.Width = num2;
						empty2.Height = (float)Math.Max(num11, num10) - empty2.Y;
						common.HotRegionsList.AddHotRegion(graph, empty2, point, item.Name, num7 - 1);
					}
					num7++;
				}
				if (!selection)
				{
					num7 = 1;
					foreach (DataPoint point2 in item.Points)
					{
						float num12 = 0f;
						double num13 = point2.XValue;
						if (!flag)
						{
							num12 = ((!sameInterval) ? ((float)hAxis.GetPosition(num13)) : ((float)(hAxis.GetPosition(num13) - (double)num3 * (double)num4 / 2.0 + (double)(num3 / 2f) + (double)((float)num5 * num3))));
						}
						else
						{
							num13 = num7;
							num12 = (float)(hAxis.GetPosition(num7) - (double)num3 * (double)num4 / 2.0 + (double)(num3 / 2f) + (double)((float)num5 * num3));
						}
						double logValue3 = vAxis.GetLogValue(point2.YValues[1]);
						double logValue4 = vAxis.GetLogValue(point2.YValues[2]);
						num13 = hAxis.GetLogValue(num13);
						if (num13 < hAxis.GetViewMinimum() || num13 > hAxis.GetViewMaximum() || (logValue3 < vAxis.GetViewMinimum() && logValue4 < vAxis.GetViewMinimum()) || (logValue3 > vAxis.GetViewMaximum() && logValue4 > vAxis.GetViewMaximum()))
						{
							num7++;
							continue;
						}
						double num14 = vAxis.GetLogValue(point2.YValues[1]);
						double num15 = vAxis.GetLogValue(point2.YValues[2]);
						if (point2.IsAttributeSet("ErrorBarStyle") || item.IsAttributeSet("ErrorBarStyle"))
						{
							string strA4 = item["ErrorBarStyle"];
							if (point2.IsAttributeSet("ErrorBarStyle"))
							{
								strA4 = point2["ErrorBarStyle"];
							}
							if (string.Compare(strA4, "Both", StringComparison.OrdinalIgnoreCase) != 0)
							{
								if (string.Compare(strA4, "UpperError", StringComparison.OrdinalIgnoreCase) == 0)
								{
									num15 = vAxis.GetLogValue(point2.YValues[0]);
									num14 = vAxis.GetLogValue(point2.YValues[2]);
								}
								else
								{
									if (string.Compare(strA4, "LowerError", StringComparison.OrdinalIgnoreCase) != 0)
									{
										throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(point2["ErrorBarStyle"], "ErrorBarStyle"));
									}
									num15 = vAxis.GetLogValue(point2.YValues[1]);
									num14 = vAxis.GetLogValue(point2.YValues[0]);
								}
							}
						}
						if (num14 > vAxis.GetViewMaximum())
						{
							num14 = vAxis.GetViewMaximum();
						}
						if (num14 < vAxis.GetViewMinimum())
						{
							num14 = vAxis.GetViewMinimum();
						}
						num14 = (float)vAxis.GetLinearPosition(num14);
						if (num15 > vAxis.GetViewMaximum())
						{
							num15 = vAxis.GetViewMaximum();
						}
						if (num15 < vAxis.GetViewMinimum())
						{
							num15 = vAxis.GetViewMinimum();
						}
						num15 = vAxis.GetLinearPosition(num15);
						graph.StartAnimation();
						graph.StartHotRegion(point2, labelRegion: true);
						DrawLabel(common, area, graph, item, point2, new PointF(num12, (float)Math.Min(num14, num15)), num7);
						graph.EndHotRegion();
						graph.StopAnimation();
						num7++;
					}
				}
				if (!selection)
				{
					common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				num++;
			}
		}

		protected virtual void DrawErrorBarMarks(ChartGraphics graph, ErrorBarStyle barStyle, ChartArea area, Series ser, DataPoint point, float xPosition, float width)
		{
			double num = 0.0;
			string empty = string.Empty;
			if (barStyle == ErrorBarStyle.Both || barStyle == ErrorBarStyle.LowerError)
			{
				num = vAxis.GetLogValue(point.YValues[1]);
				empty = "LINE";
				if (point.MarkerStyle != 0)
				{
					empty = point.MarkerStyle.ToString();
				}
				DrawErrorBarSingleMarker(graph, area, point, empty, xPosition, (float)num, 0f, width, draw3D: false);
			}
			if (barStyle == ErrorBarStyle.Both || barStyle == ErrorBarStyle.UpperError)
			{
				num = vAxis.GetLogValue(point.YValues[2]);
				empty = "LINE";
				if (point.MarkerStyle != 0)
				{
					empty = point.MarkerStyle.ToString();
				}
				DrawErrorBarSingleMarker(graph, area, point, empty, xPosition, (float)num, 0f, width, draw3D: false);
			}
			if (point.IsAttributeSet("ErrorBarCenterMarkerStyle") || point.series.IsAttributeSet("ErrorBarCenterMarkerStyle"))
			{
				num = vAxis.GetLogValue(point.YValues[0]);
				empty = point.series["ErrorBarCenterMarkerStyle"];
				if (point.IsAttributeSet("ErrorBarCenterMarkerStyle"))
				{
					empty = point["ErrorBarCenterMarkerStyle"];
				}
				empty = empty.ToUpper(CultureInfo.InvariantCulture);
				DrawErrorBarSingleMarker(graph, area, point, empty, xPosition, (float)num, 0f, width, draw3D: false);
			}
		}

		private void DrawErrorBarSingleMarker(ChartGraphics graph, ChartArea area, DataPoint point, string markerStyle, float xPosition, float yPosition, float zPosition, float width, bool draw3D)
		{
			markerStyle = markerStyle.ToUpper(CultureInfo.InvariantCulture);
			if (markerStyle.Length > 0 && string.Compare(markerStyle, "None", StringComparison.OrdinalIgnoreCase) != 0 && !((double)yPosition > vAxis.GetViewMaximum()) && !((double)yPosition < vAxis.GetViewMinimum()))
			{
				yPosition = (float)vAxis.GetLinearPosition(yPosition);
				if (draw3D)
				{
					Point3D[] array = new Point3D[1]
					{
						new Point3D(xPosition, yPosition, zPosition)
					};
					area.matrix3D.TransformPoints(array);
					xPosition = array[0].X;
					yPosition = array[0].Y;
				}
				if (string.Compare(markerStyle, "Line", StringComparison.OrdinalIgnoreCase) == 0)
				{
					graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, new PointF(xPosition - width / 2f, yPosition), new PointF(xPosition + width / 2f, yPosition), (point.series != null) ? point.series.ShadowColor : Color.Empty, (point.series != null) ? point.series.ShadowOffset : 0);
					return;
				}
				MarkerStyle markerStyle2 = (MarkerStyle)Enum.Parse(typeof(MarkerStyle), markerStyle, ignoreCase: true);
				SizeF markerSize = GetMarkerSize(graph, area.Common, area, point, point.MarkerSize, point.MarkerImage);
				Color markerColor = (point.MarkerColor == Color.Empty) ? point.Color : point.MarkerColor;
				graph.DrawMarkerRel(new PointF(xPosition, yPosition), markerStyle2, point.MarkerSize, markerColor, point.MarkerBorderColor, point.MarkerBorderWidth, point.MarkerImage, point.MarkerImageTransparentColor, (point.series != null) ? point.series.ShadowOffset : 0, (point.series != null) ? point.series.ShadowColor : Color.Empty, new RectangleF(xPosition, yPosition, markerSize.Width, markerSize.Height));
			}
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

		protected virtual void DrawLabel(CommonElements common, ChartArea area, ChartGraphics graph, Series ser, DataPoint point, PointF position, int pointIndex)
		{
			if (!ser.ShowLabelAsValue && !point.ShowLabelAsValue && point.Label.Length <= 0)
			{
				return;
			}
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Near;
			format.LineAlignment = StringAlignment.Center;
			if (point.FontAngle == 0)
			{
				format.Alignment = StringAlignment.Center;
				format.LineAlignment = StringAlignment.Far;
			}
			string text;
			if (point.Label.Length == 0)
			{
				text = ValueConverter.FormatValue(ser.chart, point, point.YValues[0], point.LabelFormat, ser.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = point.ReplaceKeywords(point.Label);
				if (ser.chart != null && ser.chart.LocalizeTextHandler != null)
				{
					text = ser.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
				}
			}
			SizeF markerSize = new SizeF(0f, 0f);
			if (point.MarkerStyle != 0)
			{
				markerSize = graph.GetRelativeSize(new SizeF(point.MarkerSize, point.MarkerSize));
				position.Y -= markerSize.Height / 2f;
			}
			int angle = point.FontAngle;
			if (text.Trim().Length == 0)
			{
				return;
			}
			SizeF labelSize = SizeF.Empty;
			if (ser.SmartLabels.Enabled)
			{
				labelSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
				position = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, ser.SmartLabels, position, labelSize, ref format, position, markerSize, LabelAlignmentTypes.Top);
				angle = 0;
			}
			if (!position.IsEmpty)
			{
				if (labelSize.IsEmpty)
				{
					labelSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
				}
				RectangleF empty = RectangleF.Empty;
				SizeF size = new SizeF(labelSize.Width, labelSize.Height);
				size.Height += labelSize.Height / 8f;
				size.Width += size.Width / (float)text.Length;
				empty = PointChart.GetLabelPosition(graph, position, size, format, adjustForDrawing: true);
				graph.DrawPointLabelStringRel(common, text, point.Font, new SolidBrush(point.FontColor), position, format, angle, empty, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, ser, point, pointIndex - 1);
			}
		}

		protected virtual void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			ArrayList seriesFromChartType = area.GetSeriesFromChartType(Name);
			bool flag = area.IndexedSeries((string[])seriesFromChartType.ToArray(typeof(string)));
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) != 0 || item.ChartArea != area.Name || !item.IsVisible())
				{
					continue;
				}
				if (item.YValuesPerPoint < 3)
				{
					throw new ArgumentException(SR.ExceptionChartTypeRequiresYValues("ErrorBar", 3.ToString(CultureInfo.CurrentCulture)));
				}
				hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
				vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
				double interval = flag ? 1.0 : area.GetPointsInterval(hAxis.Logarithmic, hAxis.logarithmBase);
				float num = (float)item.GetPointWidth(graph, hAxis, interval, 0.4);
				float num2 = num;
				int num3 = 1;
				int num4 = 0;
				bool sameInterval = false;
				if (item.IsAttributeSet("ErrorBarSeries"))
				{
					string text = item["ErrorBarSeries"];
					int num5 = text.IndexOf(":", StringComparison.Ordinal);
					if (num5 >= 0)
					{
						text = text.Substring(0, num5);
					}
					string chartTypeName = common.DataManager.Series[text].ChartTypeName;
					ArrayList seriesFromChartType2 = area.GetSeriesFromChartType(chartTypeName);
					{
						IEnumerator enumerator2 = seriesFromChartType2.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext() && !((string)enumerator2.Current == text))
							{
								num4++;
							}
						}
						finally
						{
							IDisposable disposable = enumerator2 as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
					bool flag2 = false;
					if (string.Compare(chartTypeName, "Column", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(chartTypeName, "RangeColumn", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag2 = true;
					}
					foreach (string item2 in seriesFromChartType2)
					{
						if (common.DataManager.Series[item2].IsAttributeSet("DrawSideBySide"))
						{
							text = common.DataManager.Series[item2]["DrawSideBySide"];
							if (string.Compare(text, "False", StringComparison.OrdinalIgnoreCase) == 0)
							{
								flag2 = false;
							}
							else if (string.Compare(text, "True", StringComparison.OrdinalIgnoreCase) == 0)
							{
								flag2 = true;
							}
							else if (string.Compare(text, "Auto", StringComparison.OrdinalIgnoreCase) != 0)
							{
								throw new InvalidOperationException(SR.ExceptionAttributeDrawSideBySideInvalid);
							}
						}
					}
					if (flag2)
					{
						num3 = seriesFromChartType2.Count;
						num /= (float)num3;
						if (!flag)
						{
							area.GetPointsInterval(seriesFromChartType2, hAxis.Logarithmic, hAxis.logarithmBase, checkSameInterval: true, out sameInterval);
						}
					}
				}
				if (!selection)
				{
					common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				area.GetSeriesZPositionAndDepth(item, out float depth, out float positionZ);
				int num6 = 1;
				foreach (DataPoint point in item.Points)
				{
					if (point.YValues.Length < YValuesPerPoint)
					{
						throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(Name, YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
					}
					point.positionRel = new PointF(float.NaN, float.NaN);
					float num7 = 0f;
					double num8 = point.XValue;
					if (!flag)
					{
						num7 = ((!sameInterval) ? ((float)hAxis.GetPosition(num8)) : ((float)(hAxis.GetPosition(num8) - (double)num2 * (double)num3 / 2.0 + (double)(num2 / 2f) + (double)((float)num4 * num2))));
					}
					else
					{
						num8 = num6;
						num7 = (float)(hAxis.GetPosition(num6) - (double)num2 * (double)num3 / 2.0 + (double)(num2 / 2f) + (double)((float)num4 * num2));
					}
					double logValue = vAxis.GetLogValue(point.YValues[1]);
					double logValue2 = vAxis.GetLogValue(point.YValues[2]);
					num8 = hAxis.GetLogValue(num8);
					if (num8 < hAxis.GetViewMinimum() || num8 > hAxis.GetViewMaximum() || (logValue < vAxis.GetViewMinimum() && logValue2 < vAxis.GetViewMinimum()) || (logValue > vAxis.GetViewMaximum() && logValue2 > vAxis.GetViewMaximum()))
					{
						num6++;
						continue;
					}
					double num9 = vAxis.GetLogValue(point.YValues[2]);
					double num10 = vAxis.GetLogValue(point.YValues[1]);
					ErrorBarStyle barStyle = ErrorBarStyle.Both;
					if (point.IsAttributeSet("ErrorBarStyle") || item.IsAttributeSet("ErrorBarStyle"))
					{
						string strA = item["ErrorBarStyle"];
						if (point.IsAttributeSet("ErrorBarStyle"))
						{
							strA = point["ErrorBarStyle"];
						}
						if (string.Compare(strA, "Both", StringComparison.OrdinalIgnoreCase) != 0)
						{
							if (string.Compare(strA, "UpperError", StringComparison.OrdinalIgnoreCase) == 0)
							{
								barStyle = ErrorBarStyle.UpperError;
								num10 = vAxis.GetLogValue(point.YValues[0]);
								num9 = vAxis.GetLogValue(point.YValues[2]);
							}
							else
							{
								if (string.Compare(strA, "LowerError", StringComparison.OrdinalIgnoreCase) != 0)
								{
									throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(point["ErrorBarStyle"], "ErrorBarStyle"));
								}
								barStyle = ErrorBarStyle.LowerError;
								num10 = vAxis.GetLogValue(point.YValues[1]);
								num9 = vAxis.GetLogValue(point.YValues[0]);
							}
						}
					}
					if (num9 > vAxis.GetViewMaximum())
					{
						num9 = vAxis.GetViewMaximum();
					}
					if (num9 < vAxis.GetViewMinimum())
					{
						num9 = vAxis.GetViewMinimum();
					}
					num9 = (float)vAxis.GetLinearPosition(num9);
					if (num10 > vAxis.GetViewMaximum())
					{
						num10 = vAxis.GetViewMaximum();
					}
					if (num10 < vAxis.GetViewMinimum())
					{
						num10 = vAxis.GetViewMinimum();
					}
					num10 = vAxis.GetLinearPosition(num10);
					point.positionRel = new PointF(num7, (float)Math.Min(num9, num10));
					Point3D[] array = new Point3D[2]
					{
						new Point3D(num7, (float)num9, positionZ + depth / 2f),
						new Point3D(num7, (float)num10, positionZ + depth / 2f)
					};
					area.matrix3D.TransformPoints(array);
					if (common.ProcessModePaint)
					{
						bool flag3 = false;
						if (num8 == hAxis.GetViewMinimum() || num8 == hAxis.GetViewMaximum())
						{
							graph.SetClip(area.PlotAreaPosition.ToRectangleF());
							flag3 = true;
						}
						graph.StartAnimation();
						graph.StartHotRegion(point);
						graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, array[0].PointF, array[1].PointF, item.ShadowColor, item.ShadowOffset);
						DrawErrorBarMarks3D(graph, barStyle, area, item, point, num7, num, positionZ, depth);
						num7 = array[0].X;
						num9 = array[0].Y;
						num10 = array[1].Y;
						graph.EndHotRegion();
						graph.StopAnimation();
						if (flag3)
						{
							graph.ResetClip();
						}
					}
					if (common.ProcessModeRegions)
					{
						num7 = array[0].X;
						num9 = array[0].Y;
						num10 = array[1].Y;
						RectangleF empty = RectangleF.Empty;
						empty.X = num7 - num / 2f;
						empty.Y = (float)Math.Min(num9, num10);
						empty.Width = num;
						empty.Height = (float)Math.Max(num9, num10) - empty.Y;
						common.HotRegionsList.AddHotRegion(graph, empty, point, item.Name, num6 - 1);
					}
					num6++;
				}
				if (!selection)
				{
					num6 = 1;
					foreach (DataPoint point2 in item.Points)
					{
						float num11 = 0f;
						double num12 = point2.XValue;
						if (!flag)
						{
							num11 = ((!sameInterval) ? ((float)hAxis.GetPosition(num12)) : ((float)(hAxis.GetPosition(num12) - (double)num2 * (double)num3 / 2.0 + (double)(num2 / 2f) + (double)((float)num4 * num2))));
						}
						else
						{
							num12 = num6;
							num11 = (float)(hAxis.GetPosition(num6) - (double)num2 * (double)num3 / 2.0 + (double)(num2 / 2f) + (double)((float)num4 * num2));
						}
						double logValue3 = vAxis.GetLogValue(point2.YValues[1]);
						double logValue4 = vAxis.GetLogValue(point2.YValues[2]);
						num12 = hAxis.GetLogValue(num12);
						if (num12 < hAxis.GetViewMinimum() || num12 > hAxis.GetViewMaximum() || (logValue3 < vAxis.GetViewMinimum() && logValue4 < vAxis.GetViewMinimum()) || (logValue3 > vAxis.GetViewMaximum() && logValue4 > vAxis.GetViewMaximum()))
						{
							num6++;
							continue;
						}
						double num13 = vAxis.GetLogValue(point2.YValues[2]);
						double num14 = vAxis.GetLogValue(point2.YValues[1]);
						if (point2.IsAttributeSet("ErrorBarStyle") || item.IsAttributeSet("ErrorBarStyle"))
						{
							string strA2 = item["ErrorBarStyle"];
							if (point2.IsAttributeSet("ErrorBarStyle"))
							{
								strA2 = point2["ErrorBarStyle"];
							}
							if (string.Compare(strA2, "Both", StringComparison.OrdinalIgnoreCase) != 0)
							{
								if (string.Compare(strA2, "UpperError", StringComparison.OrdinalIgnoreCase) == 0)
								{
									num14 = vAxis.GetLogValue(point2.YValues[0]);
									num13 = vAxis.GetLogValue(point2.YValues[2]);
								}
								else
								{
									if (string.Compare(strA2, "LowerError", StringComparison.OrdinalIgnoreCase) != 0)
									{
										throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(point2["ErrorBarStyle"], "ErrorBarStyle"));
									}
									num14 = vAxis.GetLogValue(point2.YValues[1]);
									num13 = vAxis.GetLogValue(point2.YValues[0]);
								}
							}
						}
						if (num13 > vAxis.GetViewMaximum())
						{
							num13 = vAxis.GetViewMaximum();
						}
						if (num13 < vAxis.GetViewMinimum())
						{
							num13 = vAxis.GetViewMinimum();
						}
						num13 = (float)vAxis.GetLinearPosition(num13);
						if (num14 > vAxis.GetViewMaximum())
						{
							num14 = vAxis.GetViewMaximum();
						}
						if (num14 < vAxis.GetViewMinimum())
						{
							num14 = vAxis.GetViewMinimum();
						}
						num14 = vAxis.GetLinearPosition(num14);
						Point3D[] array2 = new Point3D[2]
						{
							new Point3D(num11, (float)num13, positionZ + depth / 2f),
							new Point3D(num11, (float)num14, positionZ + depth / 2f)
						};
						area.matrix3D.TransformPoints(array2);
						num11 = array2[0].X;
						num13 = array2[0].Y;
						num14 = array2[1].Y;
						graph.StartAnimation();
						DrawLabel(common, area, graph, item, point2, new PointF(num11, (float)Math.Min(num13, num14)), num6);
						graph.StopAnimation();
						num6++;
					}
				}
				if (!selection)
				{
					common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
			}
		}

		protected virtual void DrawErrorBarMarks3D(ChartGraphics graph, ErrorBarStyle barStyle, ChartArea area, Series ser, DataPoint point, float xPosition, float width, float zPosition, float depth)
		{
			float num = 0f;
			string empty = string.Empty;
			if (barStyle == ErrorBarStyle.Both || barStyle == ErrorBarStyle.LowerError)
			{
				num = (float)vAxis.GetLogValue(point.YValues[1]);
				empty = "LINE";
				if (point.MarkerStyle != 0)
				{
					empty = point.MarkerStyle.ToString();
				}
				DrawErrorBarSingleMarker(graph, area, point, empty, xPosition, num, zPosition + depth / 2f, width, draw3D: true);
			}
			if (barStyle == ErrorBarStyle.Both || barStyle == ErrorBarStyle.UpperError)
			{
				num = (float)vAxis.GetLogValue(point.YValues[2]);
				empty = "LINE";
				if (point.MarkerStyle != 0)
				{
					empty = point.MarkerStyle.ToString();
				}
				DrawErrorBarSingleMarker(graph, area, point, empty, xPosition, num, zPosition + depth / 2f, width, draw3D: true);
			}
			if (point.IsAttributeSet("ErrorBarCenterMarkerStyle") || point.series.IsAttributeSet("ErrorBarCenterMarkerStyle"))
			{
				num = (float)vAxis.GetLogValue(point.YValues[0]);
				empty = point.series["ErrorBarCenterMarkerStyle"];
				if (point.IsAttributeSet("ErrorBarCenterMarkerStyle"))
				{
					empty = point["ErrorBarCenterMarkerStyle"];
				}
				empty = empty.ToUpper(CultureInfo.InvariantCulture);
				DrawErrorBarSingleMarker(graph, area, point, empty, xPosition, num, zPosition + depth / 2f, width, draw3D: true);
			}
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		internal static void CalculateErrorAmount(Series errorBarSeries)
		{
			if (string.Compare(errorBarSeries.ChartTypeName, "ErrorBar", StringComparison.OrdinalIgnoreCase) != 0 || (!errorBarSeries.IsAttributeSet("ErrorBarType") && !errorBarSeries.IsAttributeSet("ErrorBarSeries")))
			{
				return;
			}
			double num = double.NaN;
			ErrorBarType errorBarType = ErrorBarType.StandardError;
			if (errorBarSeries.IsAttributeSet("ErrorBarType"))
			{
				string text = errorBarSeries["ErrorBarType"];
				if (text.StartsWith("FixedValue", StringComparison.OrdinalIgnoreCase))
				{
					errorBarType = ErrorBarType.FixedValue;
				}
				else if (text.StartsWith("Percentage", StringComparison.OrdinalIgnoreCase))
				{
					errorBarType = ErrorBarType.Percentage;
				}
				else if (text.StartsWith("StandardDeviation", StringComparison.OrdinalIgnoreCase))
				{
					errorBarType = ErrorBarType.StandardDeviation;
				}
				else
				{
					if (!text.StartsWith("StandardError", StringComparison.OrdinalIgnoreCase))
					{
						if (text.StartsWith("None", StringComparison.OrdinalIgnoreCase))
						{
							return;
						}
						throw new InvalidOperationException(SR.ExceptionErrorBarTypeInvalid(errorBarSeries["ErrorBarType"]));
					}
					errorBarType = ErrorBarType.StandardError;
				}
				text = text.Substring(errorBarType.ToString().Length);
				if (text.Length > 0)
				{
					if (!text.StartsWith("(", StringComparison.Ordinal) || !text.EndsWith(")", StringComparison.Ordinal))
					{
						throw new InvalidOperationException(SR.ExceptionErrorBarTypeFormatInvalid(errorBarSeries["ErrorBarType"]));
					}
					text = text.Substring(1, text.Length - 2);
					if (text.Length > 0)
					{
						num = double.Parse(text, CultureInfo.InvariantCulture);
					}
				}
			}
			int count = errorBarSeries.Points.Count;
			int num2 = 0;
			foreach (DataPoint point in errorBarSeries.Points)
			{
				if (point.Empty)
				{
					num2++;
				}
			}
			count -= num2;
			if (double.IsNaN(num))
			{
				num = DefaultErrorBarTypeValue(errorBarType);
			}
			double num3 = 0.0;
			switch (errorBarType)
			{
			case ErrorBarType.FixedValue:
				num3 = num;
				break;
			case ErrorBarType.StandardDeviation:
				if (count > 1)
				{
					double num4 = 0.0;
					foreach (DataPoint point2 in errorBarSeries.Points)
					{
						num4 += point2.YValues[0];
					}
					num4 /= (double)count;
					num3 = 0.0;
					foreach (DataPoint point3 in errorBarSeries.Points)
					{
						if (!point3.Empty)
						{
							num3 += Math.Pow(point3.YValues[0] - num4, 2.0);
						}
					}
					num3 = num * Math.Sqrt(num3 / (double)(count - 1));
				}
				else
				{
					num3 = 0.0;
				}
				break;
			case ErrorBarType.StandardError:
				if (count > 1)
				{
					num3 = 0.0;
					foreach (DataPoint point4 in errorBarSeries.Points)
					{
						if (!point4.Empty)
						{
							num3 += Math.Pow(point4.YValues[0], 2.0);
						}
					}
					num3 = num * Math.Sqrt(num3 / (double)(count * (count - 1))) / 2.0;
				}
				else
				{
					num3 = 0.0;
				}
				break;
			}
			foreach (DataPoint point5 in errorBarSeries.Points)
			{
				if (errorBarType == ErrorBarType.Percentage)
				{
					point5.YValues[1] = point5.YValues[0] - point5.YValues[0] * num / 100.0;
					point5.YValues[2] = point5.YValues[0] + point5.YValues[0] * num / 100.0;
				}
				else
				{
					point5.YValues[1] = point5.YValues[0] - num3;
					point5.YValues[2] = point5.YValues[0] + num3;
				}
			}
		}

		internal static double DefaultErrorBarTypeValue(ErrorBarType errorBarType)
		{
			switch (errorBarType)
			{
			case ErrorBarType.FixedValue:
			case ErrorBarType.Percentage:
				return 10.0;
			case ErrorBarType.StandardDeviation:
			case ErrorBarType.StandardError:
				return 1.0;
			default:
				return 10.0;
			}
		}

		internal static void GetDataFromLinkedSeries(Series errorBarSeries, IServiceContainer serviceContainer)
		{
			if (string.Compare(errorBarSeries.ChartTypeName, "ErrorBar", StringComparison.OrdinalIgnoreCase) != 0 || serviceContainer == null || !errorBarSeries.IsAttributeSet("ErrorBarSeries"))
			{
				return;
			}
			string text = errorBarSeries["ErrorBarSeries"];
			string valueName = "Y";
			int num = text.IndexOf(":", StringComparison.Ordinal);
			if (num >= 0)
			{
				valueName = text.Substring(num + 1);
				text = text.Substring(0, num);
			}
			Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
			if (chart == null)
			{
				return;
			}
			if (chart.Series.GetIndex(text) == -1)
			{
				throw new InvalidOperationException(SR.ExceptionDataSeriesNameNotFound(text));
			}
			Series series = chart.Series[text];
			errorBarSeries.XAxisType = series.XAxisType;
			errorBarSeries.YAxisType = series.YAxisType;
			errorBarSeries.Points.Clear();
			foreach (DataPoint point in series.Points)
			{
				errorBarSeries.Points.AddXY(point.XValue, point.GetValueByName(valueName));
			}
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
