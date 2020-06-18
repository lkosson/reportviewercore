using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class BoxPlotChart : IChartType
	{
		protected Axis vAxis;

		protected Axis hAxis;

		protected bool showSideBySide = true;

		public virtual string Name => "BoxPlot";

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

		public virtual int YValuesPerPoint => 6;

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
				bool flag2 = showSideBySide;
				if (item.IsAttributeSet("DrawSideBySide"))
				{
					string strA = item["DrawSideBySide"];
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
				double num2 = seriesFromChartType.Count;
				if (!flag2)
				{
					num2 = 1.0;
				}
				float num3 = (float)(item.GetPointWidth(graph, hAxis, interval, 0.8) / num2);
				if (!selection)
				{
					common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				int num4 = 1;
				foreach (DataPoint point in item.Points)
				{
					if (point.YValues.Length < YValuesPerPoint)
					{
						throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(Name, YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
					}
					point.positionRel = new PointF(float.NaN, float.NaN);
					float num5 = 0f;
					double num6 = point.XValue;
					if (!flag)
					{
						num5 = ((!flag2) ? ((float)hAxis.GetPosition(num6)) : ((float)(hAxis.GetPosition(num6) - (double)num3 * num2 / 2.0 + (double)(num3 / 2f) + (double)((float)num * num3))));
					}
					else
					{
						num6 = num4;
						num5 = (float)(hAxis.GetPosition(num4) - (double)num3 * num2 / 2.0 + (double)(num3 / 2f) + (double)((float)num * num3));
					}
					double logValue = vAxis.GetLogValue(point.YValues[0]);
					double logValue2 = vAxis.GetLogValue(point.YValues[1]);
					num6 = hAxis.GetLogValue(num6);
					if (num6 < hAxis.GetViewMinimum() || num6 > hAxis.GetViewMaximum() || (logValue < vAxis.GetViewMinimum() && logValue2 < vAxis.GetViewMinimum()) || (logValue > vAxis.GetViewMaximum() && logValue2 > vAxis.GetViewMaximum()))
					{
						num4++;
						continue;
					}
					double num7 = vAxis.GetLogValue(point.YValues[0]);
					double num8 = vAxis.GetLogValue(point.YValues[1]);
					if (num8 > vAxis.GetViewMaximum())
					{
						num8 = vAxis.GetViewMaximum();
					}
					if (num8 < vAxis.GetViewMinimum())
					{
						num8 = vAxis.GetViewMinimum();
					}
					num8 = (float)vAxis.GetLinearPosition(num8);
					if (num7 > vAxis.GetViewMaximum())
					{
						num7 = vAxis.GetViewMaximum();
					}
					if (num7 < vAxis.GetViewMinimum())
					{
						num7 = vAxis.GetViewMinimum();
					}
					num7 = vAxis.GetLinearPosition(num7);
					point.positionRel = new PointF(num5, (float)Math.Min(num8, num7));
					if (common.ProcessModePaint)
					{
						bool flag3 = false;
						if (num6 == hAxis.GetViewMinimum() || num6 == hAxis.GetViewMaximum())
						{
							graph.SetClip(area.PlotAreaPosition.ToRectangleF());
							flag3 = true;
						}
						Color color = point.BorderColor;
						if (color == Color.Empty)
						{
							color = point.Color;
						}
						graph.StartHotRegion(point);
						graph.StartAnimation();
						graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, new PointF(num5, (float)num7), new PointF(num5, (float)vAxis.GetPosition(point.YValues[2])), item.ShadowColor, item.ShadowOffset);
						graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, new PointF(num5, (float)num8), new PointF(num5, (float)vAxis.GetPosition(point.YValues[3])), item.ShadowColor, item.ShadowOffset);
						RectangleF empty = RectangleF.Empty;
						empty.X = num5 - num3 / 2f;
						empty.Width = num3;
						empty.Y = (float)vAxis.GetPosition(point.YValues[3]);
						empty.Height = (float)Math.Abs((double)empty.Y - vAxis.GetPosition(point.YValues[2]));
						graph.FillRectangleRel(empty, point.Color, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, point.BackGradientEndColor, point.BorderColor, point.BorderWidth, point.BorderStyle, item.ShadowColor, item.ShadowOffset, PenAlignment.Inset);
						bool flag4 = true;
						if (point.IsAttributeSet("BoxPlotShowAverage") || item.IsAttributeSet("BoxPlotShowAverage"))
						{
							string strA2 = item["BoxPlotShowAverage"];
							if (point.IsAttributeSet("BoxPlotShowAverage"))
							{
								strA2 = point["BoxPlotShowAverage"];
							}
							if (string.Compare(strA2, "True", StringComparison.OrdinalIgnoreCase) != 0)
							{
								if (string.Compare(strA2, "False", StringComparison.OrdinalIgnoreCase) != 0)
								{
									throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(point["BoxPlotShowAverage"], "BoxPlotShowAverage"));
								}
								flag4 = false;
							}
						}
						SizeF relativeSize = graph.GetRelativeSize(new SizeF(point.BorderWidth, point.BorderWidth));
						if (point.BorderColor == Color.Empty)
						{
							relativeSize.Height = 0f;
							relativeSize.Width = 0f;
						}
						Color color2 = color;
						if (color2 == point.Color)
						{
							color2 = ((!(Math.Sqrt(point.Color.R * point.Color.R + point.Color.G * point.Color.G + point.Color.B * point.Color.B) > 220.0)) ? ChartGraphics.GetGradientColor(point.Color, Color.White, 0.4) : ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.4));
						}
						if (!double.IsNaN(point.YValues[4]) && flag4)
						{
							graph.DrawLineRel(color2, 1, ChartDashStyle.Solid, new PointF(empty.Left + relativeSize.Width, (float)vAxis.GetPosition(point.YValues[4])), new PointF(empty.Right - relativeSize.Width, (float)vAxis.GetPosition(point.YValues[4])), Color.Empty, 0);
						}
						bool flag5 = true;
						if (point.IsAttributeSet("BoxPlotShowMedian") || item.IsAttributeSet("BoxPlotShowMedian"))
						{
							string strA3 = item["BoxPlotShowMedian"];
							if (point.IsAttributeSet("BoxPlotShowMedian"))
							{
								strA3 = point["BoxPlotShowMedian"];
							}
							if (string.Compare(strA3, "True", StringComparison.OrdinalIgnoreCase) != 0)
							{
								if (string.Compare(strA3, "False", StringComparison.OrdinalIgnoreCase) != 0)
								{
									throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(point["BoxPlotShowMedian"], "BoxPlotShowMedian"));
								}
								flag5 = false;
							}
						}
						if (!double.IsNaN(point.YValues[5]) && flag5)
						{
							float y = (float)vAxis.GetPosition(point.YValues[5]);
							float val = (empty.Width - relativeSize.Width * 2f) / 9f;
							val = Math.Max(val, graph.GetRelativeSize(new SizeF(2f, 2f)).Width);
							for (float num9 = empty.Left + relativeSize.Width; num9 < empty.Right - relativeSize.Width; num9 += val * 2f)
							{
								graph.DrawLineRel(color2, 1, ChartDashStyle.Solid, new PointF(num9, y), new PointF(Math.Min(empty.Right, num9 + val), y), Color.Empty, 0);
							}
						}
						DrawBoxPlotMarks(graph, area, item, point, num5, num3);
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
						empty2.X = num5 - num3 / 2f;
						empty2.Y = (float)Math.Min(num8, num7);
						empty2.Width = num3;
						empty2.Height = (float)Math.Max(num8, num7) - empty2.Y;
						common.HotRegionsList.AddHotRegion(graph, empty2, point, item.Name, num4 - 1);
					}
					num4++;
				}
				if (!selection)
				{
					num4 = 1;
					foreach (DataPoint point2 in item.Points)
					{
						float num10 = 0f;
						double num11 = point2.XValue;
						if (!flag)
						{
							num10 = ((!flag2) ? ((float)hAxis.GetPosition(num11)) : ((float)(hAxis.GetPosition(num11) - (double)num3 * num2 / 2.0 + (double)(num3 / 2f) + (double)((float)num * num3))));
						}
						else
						{
							num11 = num4;
							num10 = (float)(hAxis.GetPosition(num4) - (double)num3 * num2 / 2.0 + (double)(num3 / 2f) + (double)((float)num * num3));
						}
						double logValue3 = vAxis.GetLogValue(point2.YValues[0]);
						double logValue4 = vAxis.GetLogValue(point2.YValues[1]);
						num11 = hAxis.GetLogValue(num11);
						if (num11 < hAxis.GetViewMinimum() || num11 > hAxis.GetViewMaximum() || (logValue3 < vAxis.GetViewMinimum() && logValue4 < vAxis.GetViewMinimum()) || (logValue3 > vAxis.GetViewMaximum() && logValue4 > vAxis.GetViewMaximum()))
						{
							num4++;
							continue;
						}
						double num12 = double.MaxValue;
						for (int i = 0; i < point2.YValues.Length; i++)
						{
							if (!double.IsNaN(point2.YValues[i]))
							{
								double num13 = vAxis.GetLogValue(point2.YValues[i]);
								if (num13 > vAxis.GetViewMaximum())
								{
									num13 = vAxis.GetViewMaximum();
								}
								if (num13 < vAxis.GetViewMinimum())
								{
									num13 = vAxis.GetViewMinimum();
								}
								num13 = (float)vAxis.GetLinearPosition(num13);
								num12 = Math.Min(num12, num13);
							}
						}
						num12 -= (double)(graph.GetRelativeSize(new SizeF(point2.MarkerSize, point2.MarkerSize)).Height / 2f);
						graph.StartHotRegion(point2, labelRegion: true);
						graph.StartAnimation();
						DrawLabel(common, area, graph, item, point2, new PointF(num10, (float)num12), num4);
						graph.StopAnimation();
						graph.EndHotRegion();
						num4++;
					}
				}
				if (!selection)
				{
					common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				if (flag2)
				{
					num++;
				}
			}
		}

		protected virtual void DrawBoxPlotMarks(ChartGraphics graph, ChartArea area, Series ser, DataPoint point, float xPosition, float width)
		{
			string markerStyle = "LINE";
			if (point.MarkerStyle != 0)
			{
				markerStyle = point.MarkerStyle.ToString();
			}
			double logValue = vAxis.GetLogValue(point.YValues[0]);
			DrawBoxPlotSingleMarker(graph, area, point, markerStyle, xPosition, (float)logValue, 0f, width, draw3D: false);
			logValue = vAxis.GetLogValue(point.YValues[1]);
			DrawBoxPlotSingleMarker(graph, area, point, markerStyle, xPosition, (float)logValue, 0f, width, draw3D: false);
			markerStyle = "CIRCLE";
			if (point.MarkerStyle != 0)
			{
				markerStyle = point.MarkerStyle.ToString();
			}
			for (int i = 6; i < point.YValues.Length; i++)
			{
				if (!double.IsNaN(point.YValues[i]))
				{
					logValue = vAxis.GetLogValue(point.YValues[i]);
					DrawBoxPlotSingleMarker(graph, area, point, markerStyle, xPosition, (float)logValue, 0f, width, draw3D: false);
				}
			}
		}

		private void DrawBoxPlotSingleMarker(ChartGraphics graph, ChartArea area, DataPoint point, string markerStyle, float xPosition, float yPosition, float zPosition, float width, bool draw3D)
		{
			markerStyle = markerStyle.ToUpper(CultureInfo.InvariantCulture);
			if (markerStyle.Length <= 0 || string.Compare(markerStyle, "None", StringComparison.OrdinalIgnoreCase) == 0 || (double)yPosition > vAxis.GetViewMaximum() || (double)yPosition < vAxis.GetViewMinimum())
			{
				return;
			}
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
			Color color = point.BorderColor;
			if (color == Color.Empty)
			{
				color = point.Color;
			}
			if (string.Compare(markerStyle, "Line", StringComparison.OrdinalIgnoreCase) == 0)
			{
				graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, new PointF(xPosition - width / 4f, yPosition), new PointF(xPosition + width / 4f, yPosition), (point.series != null) ? point.series.ShadowColor : Color.Empty, (point.series != null) ? point.series.ShadowOffset : 0);
				return;
			}
			MarkerStyle markerStyle2 = (MarkerStyle)Enum.Parse(typeof(MarkerStyle), markerStyle, ignoreCase: true);
			SizeF markerSize = GetMarkerSize(graph, area.Common, area, point, point.MarkerSize, point.MarkerImage);
			Color color2 = (point.MarkerColor == Color.Empty) ? point.BorderColor : point.MarkerColor;
			if (color2 == Color.Empty)
			{
				color2 = point.Color;
			}
			graph.DrawMarkerRel(new PointF(xPosition, yPosition), markerStyle2, point.MarkerSize, color2, point.MarkerBorderColor, point.MarkerBorderWidth, point.MarkerImage, point.MarkerImageTransparentColor, (point.series != null) ? point.series.ShadowOffset : 0, (point.series != null) ? point.series.ShadowColor : Color.Empty, new RectangleF(xPosition, yPosition, markerSize.Width, markerSize.Height));
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
			int num = 0;
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) != 0 || item.ChartArea != area.Name || !item.IsVisible())
				{
					continue;
				}
				if (item.YValuesPerPoint < 6)
				{
					throw new ArgumentException(SR.ExceptionChartTypeRequiresYValues("BoxPlot", "6"));
				}
				bool flag2 = showSideBySide;
				if (item.IsAttributeSet("DrawSideBySide"))
				{
					string strA = item["DrawSideBySide"];
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
				double num2 = seriesFromChartType.Count;
				if (!flag2)
				{
					num2 = 1.0;
				}
				hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
				vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
				double interval = flag ? 1.0 : area.GetPointsInterval(hAxis.Logarithmic, hAxis.logarithmBase);
				float num3 = (float)(item.GetPointWidth(graph, hAxis, interval, 0.8) / num2);
				if (!selection)
				{
					common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				area.GetSeriesZPositionAndDepth(item, out float depth, out float positionZ);
				int num4 = 1;
				foreach (DataPoint point in item.Points)
				{
					if (point.YValues.Length < YValuesPerPoint)
					{
						throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(Name, YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
					}
					point.positionRel = new PointF(float.NaN, float.NaN);
					float num5 = 0f;
					double num6 = point.XValue;
					if (!flag)
					{
						num5 = ((!flag2) ? ((float)hAxis.GetPosition(num6)) : ((float)(hAxis.GetPosition(num6) - (double)num3 * num2 / 2.0 + (double)(num3 / 2f) + (double)((float)num * num3))));
					}
					else
					{
						num6 = num4;
						num5 = (float)(hAxis.GetPosition(num4) - (double)num3 * num2 / 2.0 + (double)(num3 / 2f) + (double)((float)num * num3));
					}
					double logValue = vAxis.GetLogValue(point.YValues[0]);
					double logValue2 = vAxis.GetLogValue(point.YValues[1]);
					num6 = hAxis.GetLogValue(num6);
					if (num6 < hAxis.GetViewMinimum() || num6 > hAxis.GetViewMaximum() || (logValue < vAxis.GetViewMinimum() && logValue2 < vAxis.GetViewMinimum()) || (logValue > vAxis.GetViewMaximum() && logValue2 > vAxis.GetViewMaximum()))
					{
						num4++;
						continue;
					}
					double num7 = vAxis.GetLogValue(point.YValues[1]);
					double num8 = vAxis.GetLogValue(point.YValues[0]);
					if (num7 > vAxis.GetViewMaximum())
					{
						num7 = vAxis.GetViewMaximum();
					}
					if (num7 < vAxis.GetViewMinimum())
					{
						num7 = vAxis.GetViewMinimum();
					}
					num7 = (float)vAxis.GetLinearPosition(num7);
					if (num8 > vAxis.GetViewMaximum())
					{
						num8 = vAxis.GetViewMaximum();
					}
					if (num8 < vAxis.GetViewMinimum())
					{
						num8 = vAxis.GetViewMinimum();
					}
					num8 = vAxis.GetLinearPosition(num8);
					point.positionRel = new PointF(num5, (float)Math.Min(num7, num8));
					Point3D[] array = new Point3D[6]
					{
						new Point3D(num5, (float)num8, positionZ + depth / 2f),
						new Point3D(num5, (float)num7, positionZ + depth / 2f),
						new Point3D(num5, (float)vAxis.GetPosition(point.YValues[2]), positionZ + depth / 2f),
						new Point3D(num5, (float)vAxis.GetPosition(point.YValues[3]), positionZ + depth / 2f),
						new Point3D(num5, (float)vAxis.GetPosition(point.YValues[4]), positionZ + depth / 2f),
						new Point3D(num5, (float)vAxis.GetPosition(point.YValues[5]), positionZ + depth / 2f)
					};
					area.matrix3D.TransformPoints(array);
					if (common.ProcessModePaint)
					{
						bool flag3 = false;
						if (num6 == hAxis.GetViewMinimum() || num6 == hAxis.GetViewMaximum())
						{
							graph.SetClip(area.PlotAreaPosition.ToRectangleF());
							flag3 = true;
						}
						Color color = point.BorderColor;
						if (color == Color.Empty)
						{
							color = point.Color;
						}
						graph.StartAnimation();
						graph.StartHotRegion(point);
						graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, array[0].PointF, array[2].PointF, item.ShadowColor, item.ShadowOffset);
						graph.DrawLineRel(color, point.BorderWidth, point.BorderStyle, array[1].PointF, array[3].PointF, item.ShadowColor, item.ShadowOffset);
						RectangleF empty = RectangleF.Empty;
						empty.X = array[0].X - num3 / 2f;
						empty.Width = num3;
						empty.Y = array[3].Y;
						empty.Height = Math.Abs(empty.Y - array[2].Y);
						graph.FillRectangleRel(empty, point.Color, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, point.BackGradientEndColor, point.BorderColor, point.BorderWidth, point.BorderStyle, item.ShadowColor, item.ShadowOffset, PenAlignment.Inset);
						bool flag4 = true;
						if (point.IsAttributeSet("BoxPlotShowAverage") || item.IsAttributeSet("BoxPlotShowAverage"))
						{
							string strA2 = item["BoxPlotShowAverage"];
							if (point.IsAttributeSet("BoxPlotShowAverage"))
							{
								strA2 = point["BoxPlotShowAverage"];
							}
							if (string.Compare(strA2, "True", StringComparison.OrdinalIgnoreCase) != 0)
							{
								if (string.Compare(strA2, "False", StringComparison.OrdinalIgnoreCase) != 0)
								{
									throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(point["BoxPlotShowAverage"], "BoxPlotShowAverage"));
								}
								flag4 = false;
							}
						}
						Color color2 = color;
						if (color2 == point.Color)
						{
							color2 = ((!(Math.Sqrt(point.Color.R * point.Color.R + point.Color.G * point.Color.G + point.Color.B * point.Color.B) > 220.0)) ? ChartGraphics.GetGradientColor(point.Color, Color.White, 0.4) : ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.4));
						}
						if (!double.IsNaN(point.YValues[4]) && flag4)
						{
							graph.DrawLineRel(color2, 1, ChartDashStyle.Solid, new PointF(empty.Left, array[4].Y), new PointF(empty.Right, array[4].Y), Color.Empty, 0);
						}
						bool flag5 = true;
						if (point.IsAttributeSet("BoxPlotShowMedian") || item.IsAttributeSet("BoxPlotShowMedian"))
						{
							string strA3 = item["BoxPlotShowMedian"];
							if (point.IsAttributeSet("BoxPlotShowMedian"))
							{
								strA3 = point["BoxPlotShowMedian"];
							}
							if (string.Compare(strA3, "True", StringComparison.OrdinalIgnoreCase) != 0)
							{
								if (string.Compare(strA3, "False", StringComparison.OrdinalIgnoreCase) != 0)
								{
									throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(point["BoxPlotShowMedian"], "BoxPlotShowMedian"));
								}
								flag5 = false;
							}
						}
						if (!double.IsNaN(point.YValues[5]) && flag5)
						{
							float y = array[5].Y;
							float val = empty.Width / 9f;
							val = Math.Max(val, graph.GetRelativeSize(new SizeF(2f, 2f)).Width);
							for (float num9 = empty.Left; num9 < empty.Right; num9 += val * 2f)
							{
								graph.DrawLineRel(color2, 1, ChartDashStyle.Solid, new PointF(num9, y), new PointF(Math.Min(empty.Right, num9 + val), y), Color.Empty, 0);
							}
						}
						DrawBoxPlotMarks3D(graph, area, item, point, num5, num3, positionZ, depth);
						num5 = array[0].X;
						num7 = array[0].Y;
						num8 = array[1].Y;
						graph.StopAnimation();
						graph.EndHotRegion();
						if (flag3)
						{
							graph.ResetClip();
						}
					}
					if (common.ProcessModeRegions)
					{
						num5 = array[0].X;
						num7 = array[0].Y;
						num8 = array[1].Y;
						RectangleF empty2 = RectangleF.Empty;
						empty2.X = num5 - num3 / 2f;
						empty2.Y = (float)Math.Min(num7, num8);
						empty2.Width = num3;
						empty2.Height = (float)Math.Max(num7, num8) - empty2.Y;
						common.HotRegionsList.AddHotRegion(graph, empty2, point, item.Name, num4 - 1);
					}
					num4++;
				}
				if (!selection)
				{
					num4 = 1;
					foreach (DataPoint point2 in item.Points)
					{
						float num10 = 0f;
						double num11 = point2.XValue;
						if (!flag)
						{
							num10 = ((!flag2) ? ((float)hAxis.GetPosition(num11)) : ((float)(hAxis.GetPosition(num11) - (double)num3 * num2 / 2.0 + (double)(num3 / 2f) + (double)((float)num * num3))));
						}
						else
						{
							num11 = num4;
							num10 = (float)(hAxis.GetPosition(num4) - (double)num3 * num2 / 2.0 + (double)(num3 / 2f) + (double)((float)num * num3));
						}
						double logValue3 = vAxis.GetLogValue(point2.YValues[0]);
						double logValue4 = vAxis.GetLogValue(point2.YValues[1]);
						num11 = hAxis.GetLogValue(num11);
						if (num11 < hAxis.GetViewMinimum() || num11 > hAxis.GetViewMaximum() || (logValue3 < vAxis.GetViewMinimum() && logValue4 < vAxis.GetViewMinimum()) || (logValue3 > vAxis.GetViewMaximum() && logValue4 > vAxis.GetViewMaximum()))
						{
							num4++;
							continue;
						}
						double num12 = vAxis.GetLogValue(point2.YValues[1]);
						double num13 = vAxis.GetLogValue(point2.YValues[0]);
						if (num12 > vAxis.GetViewMaximum())
						{
							num12 = vAxis.GetViewMaximum();
						}
						if (num12 < vAxis.GetViewMinimum())
						{
							num12 = vAxis.GetViewMinimum();
						}
						num12 = (float)vAxis.GetLinearPosition(num12);
						if (num13 > vAxis.GetViewMaximum())
						{
							num13 = vAxis.GetViewMaximum();
						}
						if (num13 < vAxis.GetViewMinimum())
						{
							num13 = vAxis.GetViewMinimum();
						}
						num13 = vAxis.GetLinearPosition(num13);
						Point3D[] array2 = new Point3D[2]
						{
							new Point3D(num10, (float)num12, positionZ + depth / 2f),
							new Point3D(num10, (float)num13, positionZ + depth / 2f)
						};
						area.matrix3D.TransformPoints(array2);
						num10 = array2[0].X;
						num12 = array2[0].Y;
						num13 = array2[1].Y;
						graph.StartAnimation();
						DrawLabel(common, area, graph, item, point2, new PointF(num10, (float)Math.Min(num12, num13)), num4);
						graph.StopAnimation();
						num4++;
					}
				}
				if (!selection)
				{
					common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
			}
		}

		protected virtual void DrawBoxPlotMarks3D(ChartGraphics graph, ChartArea area, Series ser, DataPoint point, float xPosition, float width, float zPosition, float depth)
		{
			string markerStyle = "LINE";
			if (point.MarkerStyle != 0)
			{
				markerStyle = point.MarkerStyle.ToString();
			}
			double logValue = vAxis.GetLogValue(point.YValues[0]);
			DrawBoxPlotSingleMarker(graph, area, point, markerStyle, xPosition, (float)logValue, zPosition + depth / 2f, width, draw3D: true);
			logValue = vAxis.GetLogValue(point.YValues[1]);
			DrawBoxPlotSingleMarker(graph, area, point, markerStyle, xPosition, (float)logValue, zPosition + depth / 2f, width, draw3D: true);
			markerStyle = "CIRCLE";
			if (point.MarkerStyle != 0)
			{
				markerStyle = point.MarkerStyle.ToString();
			}
			for (int i = 6; i < point.YValues.Length; i++)
			{
				if (!double.IsNaN(point.YValues[i]))
				{
					logValue = vAxis.GetLogValue(point.YValues[i]);
					DrawBoxPlotSingleMarker(graph, area, point, markerStyle, xPosition, (float)logValue, zPosition + depth / 2f, width, draw3D: true);
				}
			}
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		internal static void CalculateBoxPlotFromLinkedSeries(Series boxPlotSeries, IServiceContainer serviceContainer)
		{
			if (string.Compare(boxPlotSeries.ChartTypeName, "BoxPlot", StringComparison.OrdinalIgnoreCase) != 0 || serviceContainer == null)
			{
				return;
			}
			if (boxPlotSeries.IsAttributeSet("BoxPlotSeries"))
			{
				string[] array = boxPlotSeries["BoxPlotSeries"].Split(';');
				boxPlotSeries.Points.Clear();
				int num = 0;
				string[] array2 = array;
				foreach (string text in array2)
				{
					boxPlotSeries.Points.AddY(0.0);
					boxPlotSeries.Points[num++]["BoxPlotSeries"] = text.Trim();
				}
			}
			int num2 = 0;
			string text2;
			while (true)
			{
				if (num2 >= boxPlotSeries.Points.Count)
				{
					return;
				}
				DataPoint boxPoint = boxPlotSeries.Points[num2];
				if (boxPoint.IsAttributeSet("BoxPlotSeries"))
				{
					text2 = boxPoint["BoxPlotSeries"];
					string valueName = "Y";
					int num3 = text2.IndexOf(":", StringComparison.OrdinalIgnoreCase);
					if (num3 >= 0)
					{
						valueName = text2.Substring(num3 + 1);
						text2 = text2.Substring(0, num3);
					}
					Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
					if (chart != null)
					{
						if (chart.Series.GetIndex(text2) == -1)
						{
							break;
						}
						Series linkedSeries = chart.Series[text2];
						CalculateBoxPlotValues(ref boxPoint, linkedSeries, valueName);
					}
				}
				num2++;
			}
			throw new InvalidOperationException(SR.ExceptionCustomAttributeSeriesNameNotFound("BoxPlotSeries", text2));
		}

		private static void CalculateBoxPlotValues(ref DataPoint boxPoint, Series linkedSeries, string valueName)
		{
			if (linkedSeries.Points.Count == 0)
			{
				return;
			}
			double num = 0.0;
			int num2 = 0;
			foreach (DataPoint point in linkedSeries.Points)
			{
				if (!point.Empty)
				{
					num += point.GetValueByName(valueName);
					num2++;
				}
			}
			num /= (double)num2;
			double[] array = new double[num2];
			int num3 = 0;
			foreach (DataPoint point2 in linkedSeries.Points)
			{
				if (!point2.Empty)
				{
					array[num3++] = (point2.Empty ? double.NaN : point2.GetValueByName(valueName));
				}
			}
			double[] array2 = new double[5]
			{
				10.0,
				90.0,
				25.0,
				75.0,
				50.0
			};
			string text = boxPoint.IsAttributeSet("BoxPlotPercentile") ? boxPoint["BoxPlotPercentile"] : string.Empty;
			if (text.Length == 0 && boxPoint.series != null && boxPoint.series.IsAttributeSet("BoxPlotPercentile"))
			{
				text = boxPoint.series["BoxPlotPercentile"];
			}
			string text2 = boxPoint.IsAttributeSet("BoxPlotWhiskerPercentile") ? boxPoint["BoxPlotWhiskerPercentile"] : string.Empty;
			if (text2.Length == 0 && boxPoint.series != null && boxPoint.series.IsAttributeSet("BoxPlotWhiskerPercentile"))
			{
				text2 = boxPoint.series["BoxPlotWhiskerPercentile"];
			}
			if (text.Length > 0)
			{
				try
				{
					array2[2] = double.Parse(text, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to50("BoxPlotPercentile"));
				}
				if (array2[2] < 0.0 || array2[2] > 50.0)
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to50("BoxPlotPercentile"));
				}
				array2[3] = 100.0 - array2[2];
			}
			if (text2.Length > 0)
			{
				try
				{
					array2[0] = double.Parse(text2, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to50("BoxPlotWhiskerPercentile"));
				}
				if (array2[0] < 0.0 || array2[0] > 50.0)
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to50("BoxPlotPercentile"));
				}
				array2[1] = 100.0 - array2[0];
			}
			double[] array3 = CalculatePercentileValues(array, array2);
			boxPoint.YValues[0] = array3[0];
			boxPoint.YValues[1] = array3[1];
			boxPoint.YValues[2] = array3[2];
			boxPoint.YValues[3] = array3[3];
			boxPoint.YValues[4] = num;
			boxPoint.YValues[5] = array3[4];
			bool flag = false;
			string text3 = boxPoint.IsAttributeSet("BoxPlotShowUnusualValues") ? boxPoint["BoxPlotShowUnusualValues"] : string.Empty;
			if (text3.Length == 0 && boxPoint.series != null && boxPoint.series.IsAttributeSet("BoxPlotShowUnusualValues"))
			{
				text3 = boxPoint.series["BoxPlotShowUnusualValues"];
			}
			if (text3.Length > 0)
			{
				if (string.Compare(text3, "True", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
				}
				else
				{
					if (string.Compare(text3, "False", StringComparison.OrdinalIgnoreCase) != 0)
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("BoxPlotShowUnusualValues"));
					}
					flag = false;
				}
			}
			if (flag)
			{
				BoxPlotAddUnusual(ref boxPoint, array);
			}
		}

		private static void BoxPlotAddUnusual(ref DataPoint boxPoint, double[] yValues)
		{
			ArrayList arrayList = new ArrayList();
			foreach (double num in yValues)
			{
				if (num < boxPoint.YValues[0] || num > boxPoint.YValues[1])
				{
					arrayList.Add(num);
				}
			}
			if (arrayList.Count > 0)
			{
				double[] array = new double[6 + arrayList.Count];
				for (int j = 0; j < 6; j++)
				{
					array[j] = boxPoint.YValues[j];
				}
				for (int k = 0; k < arrayList.Count; k++)
				{
					array[6 + k] = (double)arrayList[k];
				}
				boxPoint.YValues = array;
			}
		}

		private static double[] CalculatePercentileValues(double[] yValues, double[] requiredPercentile)
		{
			double[] array = new double[5];
			Array.Sort(yValues);
			int num = 0;
			foreach (double num2 in requiredPercentile)
			{
				double num3 = ((double)yValues.Length - 1.0) / 100.0 * num2;
				double num4 = Math.Floor(num3);
				double num5 = num3 - num4;
				array[num] = 0.0;
				if ((int)num4 < yValues.Length)
				{
					array[num] += (1.0 - num5) * yValues[(int)num4];
				}
				if ((int)(num4 + 1.0) < yValues.Length)
				{
					array[num] += num5 * yValues[(int)num4 + 1];
				}
				num++;
			}
			return array;
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
