using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal class StockChart : IChartType
	{
		protected Axis vAxis;

		protected Axis hAxis;

		protected StockOpenCloseMarkStyle openCloseStyle;

		protected bool forceCandleStick;

		public virtual string Name => "Stock";

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

		public virtual int YValuesPerPoint => 4;

		public StockChart()
		{
		}

		public StockChart(StockOpenCloseMarkStyle style)
		{
			openCloseStyle = style;
		}

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
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) != 0 || item.ChartArea != area.Name || !item.IsVisible())
				{
					continue;
				}
				if (item.YValuesPerPoint < 4)
				{
					throw new ArgumentException(SR.ExceptionChartTypeRequiresYValues("StockChart", "4"));
				}
				hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
				vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
				double interval = flag ? 1.0 : area.GetPointsInterval(hAxis.Logarithmic, hAxis.logarithmBase);
				float num = (float)item.GetPointWidth(graph, hAxis, interval, 0.8);
				if (!selection)
				{
					common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				int num2 = 1;
				foreach (DataPoint point in item.Points)
				{
					point.positionRel = new PointF(float.NaN, float.NaN);
					double num3 = point.XValue;
					if (flag)
					{
						num3 = num2;
					}
					float num4 = (float)hAxis.GetPosition(num3);
					double logValue = vAxis.GetLogValue(point.YValues[0]);
					double logValue2 = vAxis.GetLogValue(point.YValues[1]);
					num3 = hAxis.GetLogValue(num3);
					if (num3 < hAxis.GetViewMinimum() || num3 > hAxis.GetViewMaximum() || (logValue < vAxis.GetViewMinimum() && logValue2 < vAxis.GetViewMinimum()) || (logValue > vAxis.GetViewMaximum() && logValue2 > vAxis.GetViewMaximum()))
					{
						num2++;
						continue;
					}
					double num5 = vAxis.GetLogValue(point.YValues[0]);
					double num6 = vAxis.GetLogValue(point.YValues[1]);
					if (num5 > vAxis.GetViewMaximum())
					{
						num5 = vAxis.GetViewMaximum();
					}
					if (num5 < vAxis.GetViewMinimum())
					{
						num5 = vAxis.GetViewMinimum();
					}
					num5 = (float)vAxis.GetLinearPosition(num5);
					if (num6 > vAxis.GetViewMaximum())
					{
						num6 = vAxis.GetViewMaximum();
					}
					if (num6 < vAxis.GetViewMinimum())
					{
						num6 = vAxis.GetViewMinimum();
					}
					num6 = vAxis.GetLinearPosition(num6);
					point.positionRel = new PointF(num4, (float)num5);
					if (common.ProcessModePaint)
					{
						bool flag2 = false;
						if (num3 == hAxis.GetViewMinimum() || num3 == hAxis.GetViewMaximum())
						{
							graph.SetClip(area.PlotAreaPosition.ToRectangleF());
							flag2 = true;
						}
						graph.StartHotRegion(point);
						graph.StartAnimation();
						graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, new PointF(num4, (float)num5), new PointF(num4, (float)num6), item.ShadowColor, item.ShadowOffset);
						DrawOpenCloseMarks(graph, area, item, point, num4, num);
						graph.StopAnimation();
						graph.EndHotRegion();
						if (flag2)
						{
							graph.ResetClip();
						}
					}
					if (common.ProcessModeRegions)
					{
						RectangleF empty = RectangleF.Empty;
						empty.X = num4 - num / 2f;
						empty.Y = (float)Math.Min(num5, num6);
						empty.Width = num;
						empty.Height = (float)Math.Max(num5, num6) - empty.Y;
						common.HotRegionsList.AddHotRegion(graph, empty, point, item.Name, num2 - 1);
					}
					num2++;
				}
				int num7 = 0;
				num2 = 1;
				foreach (DataPoint point2 in item.Points)
				{
					double num8 = point2.XValue;
					if (flag)
					{
						num8 = num2;
					}
					float x = (float)hAxis.GetPosition(num8);
					double logValue3 = vAxis.GetLogValue(point2.YValues[0]);
					double logValue4 = vAxis.GetLogValue(point2.YValues[1]);
					num8 = hAxis.GetLogValue(num8);
					if (num8 < hAxis.GetViewMinimum() || num8 > hAxis.GetViewMaximum() || (logValue3 < vAxis.GetViewMinimum() && logValue4 < vAxis.GetViewMinimum()) || (logValue3 > vAxis.GetViewMaximum() && logValue4 > vAxis.GetViewMaximum()))
					{
						num2++;
						continue;
					}
					double num9 = vAxis.GetLogValue(point2.YValues[0]);
					double num10 = vAxis.GetLogValue(point2.YValues[1]);
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
					if (point2.MarkerStyle != 0 || point2.MarkerImage.Length > 0)
					{
						SizeF size = SizeF.Empty;
						size.Width = point2.MarkerSize;
						size.Height = point2.MarkerSize;
						if (point2.MarkerImage.Length > 0)
						{
							common.ImageLoader.GetAdjustedImageSize(point2.MarkerImage, graph.Graphics, ref size);
						}
						PointF empty2 = PointF.Empty;
						empty2.X = x;
						empty2.Y = (float)num9 - graph.GetRelativeSize(size).Height / 2f;
						if (num7 == 0)
						{
							graph.StartAnimation();
							graph.DrawMarkerRel(empty2, point2.MarkerStyle, (int)size.Height, (point2.MarkerColor == Color.Empty) ? point2.Color : point2.MarkerColor, (point2.MarkerBorderColor == Color.Empty) ? point2.BorderColor : point2.MarkerBorderColor, point2.MarkerBorderWidth, point2.MarkerImage, point2.MarkerImageTransparentColor, (point2.series != null) ? point2.series.ShadowOffset : 0, (point2.series != null) ? point2.series.ShadowColor : Color.Empty, new RectangleF(empty2.X, empty2.Y, size.Width, size.Height));
							graph.StopAnimation();
							if (common.ProcessModeRegions)
							{
								SizeF relativeSize = graph.GetRelativeSize(size);
								int insertIndex = common.HotRegionsList.FindInsertIndex();
								common.HotRegionsList.FindInsertIndex();
								if (point2.MarkerStyle == MarkerStyle.Circle)
								{
									float[] array = new float[3]
									{
										empty2.X,
										empty2.Y,
										relativeSize.Width / 2f
									};
									common.HotRegionsList.AddHotRegion(insertIndex, graph, array[0], array[1], array[2], point2, item.Name, num2 - 1);
								}
								else
								{
									common.HotRegionsList.AddHotRegion(graph, new RectangleF(empty2.X - relativeSize.Width / 2f, empty2.Y - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height), point2, item.Name, num2 - 1);
								}
							}
						}
						num7++;
						if (item.MarkerStep == num7)
						{
							num7 = 0;
						}
					}
					graph.StartAnimation();
					DrawLabel(common, area, graph, item, point2, new PointF(x, (float)Math.Min(num9, num10)), num2);
					graph.StopAnimation();
					num2++;
				}
				if (!selection)
				{
					common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
			}
		}

		protected virtual void DrawOpenCloseMarks(ChartGraphics graph, ChartArea area, Series ser, DataPoint point, float xPosition, float width)
		{
			double logValue = vAxis.GetLogValue(point.YValues[2]);
			double logValue2 = vAxis.GetLogValue(point.YValues[3]);
			if ((logValue > vAxis.GetViewMaximum() || logValue < vAxis.GetViewMinimum()) && !(logValue2 > vAxis.GetViewMaximum()))
			{
				vAxis.GetViewMinimum();
			}
			float num = (float)vAxis.GetLinearPosition(logValue);
			float num2 = (float)vAxis.GetLinearPosition(logValue2);
			SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF(width, width));
			float height = graph.GetRelativeSize(absoluteSize).Height;
			StockOpenCloseMarkStyle stockOpenCloseMarkStyle = openCloseStyle;
			string text = "";
			if (point.IsAttributeSet("OpenCloseStyle"))
			{
				text = point["OpenCloseStyle"];
			}
			else if (ser.IsAttributeSet("OpenCloseStyle"))
			{
				text = ser["OpenCloseStyle"];
			}
			if (text != null && text.Length > 0)
			{
				if (string.Compare(text, "Candlestick", StringComparison.OrdinalIgnoreCase) == 0)
				{
					stockOpenCloseMarkStyle = StockOpenCloseMarkStyle.Candlestick;
				}
				else if (string.Compare(text, "Triangle", StringComparison.OrdinalIgnoreCase) == 0)
				{
					stockOpenCloseMarkStyle = StockOpenCloseMarkStyle.Triangle;
				}
				else if (string.Compare(text, "Line", StringComparison.OrdinalIgnoreCase) == 0)
				{
					stockOpenCloseMarkStyle = StockOpenCloseMarkStyle.Line;
				}
			}
			bool flag = true;
			bool flag2 = true;
			string text2 = "";
			if (point.IsAttributeSet("ShowOpenClose"))
			{
				text2 = point["ShowOpenClose"];
			}
			else if (ser.IsAttributeSet("ShowOpenClose"))
			{
				text2 = ser["ShowOpenClose"];
			}
			if (text2 != null && text2.Length > 0)
			{
				if (string.Compare(text2, "Both", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
					flag2 = true;
				}
				else if (string.Compare(text2, "Open", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
					flag2 = false;
				}
				else if (string.Compare(text2, "Close", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = false;
					flag2 = true;
				}
			}
			bool flag3 = false;
			if (stockOpenCloseMarkStyle == StockOpenCloseMarkStyle.Candlestick || xPosition - width / 2f < area.PlotAreaPosition.X || xPosition + width / 2f > area.PlotAreaPosition.Right())
			{
				graph.SetClip(area.PlotAreaPosition.ToRectangleF());
				flag3 = true;
			}
			if (forceCandleStick || stockOpenCloseMarkStyle == StockOpenCloseMarkStyle.Candlestick)
			{
				ColorConverter colorConverter = new ColorConverter();
				Color color = point.Color;
				Color color2 = point.BackGradientEndColor;
				string text3 = point["PriceUpColor"];
				if (text3 != null && text3.Length > 0)
				{
					try
					{
						color = (Color)colorConverter.ConvertFromString(text3);
					}
					catch
					{
						color = (Color)colorConverter.ConvertFromInvariantString(text3);
					}
				}
				text3 = point["PriceDownColor"];
				if (text3 != null && text3.Length > 0)
				{
					try
					{
						color2 = (Color)colorConverter.ConvertFromString(text3);
					}
					catch
					{
						color2 = (Color)colorConverter.ConvertFromInvariantString(text3);
					}
				}
				RectangleF empty = RectangleF.Empty;
				empty.Y = Math.Min(num, num2);
				empty.X = xPosition - width / 2f;
				empty.Height = Math.Max(num, num2) - empty.Y;
				empty.Width = width;
				Color color3 = (num > num2) ? color : color2;
				Color color4 = (!(point.BorderColor == Color.Empty)) ? point.BorderColor : ((color3 == Color.Empty) ? point.Color : color3);
				SizeF relative = new SizeF(empty.Height, empty.Height);
				if (graph.GetAbsoluteSize(relative).Height > 1f)
				{
					graph.FillRectangleRel(empty, color3, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, point.BackGradientEndColor, color4, point.BorderWidth, point.BorderStyle, ser.ShadowColor, ser.ShadowOffset, PenAlignment.Inset);
				}
				else
				{
					graph.DrawLineRel(color4, point.BorderWidth, point.BorderStyle, new PointF(empty.X, empty.Y), new PointF(empty.Right, empty.Y), ser.ShadowColor, ser.ShadowOffset);
				}
			}
			else if (stockOpenCloseMarkStyle == StockOpenCloseMarkStyle.Triangle)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				PointF absolutePoint = graph.GetAbsolutePoint(new PointF(xPosition, num));
				PointF absolutePoint2 = graph.GetAbsolutePoint(new PointF(xPosition - width / 2f, num + height / 2f));
				PointF absolutePoint3 = graph.GetAbsolutePoint(new PointF(xPosition - width / 2f, num - height / 2f));
				if (flag && logValue <= vAxis.GetViewMaximum() && logValue >= vAxis.GetViewMinimum())
				{
					graphicsPath.AddLine(absolutePoint2, absolutePoint);
					graphicsPath.AddLine(absolutePoint, absolutePoint3);
					graphicsPath.AddLine(absolutePoint3, absolutePoint3);
					graph.FillPath(new SolidBrush(point.Color), graphicsPath);
				}
				if (flag2 && logValue2 <= vAxis.GetViewMaximum() && logValue2 >= vAxis.GetViewMinimum())
				{
					graphicsPath.Reset();
					absolutePoint = graph.GetAbsolutePoint(new PointF(xPosition, num2));
					absolutePoint2 = graph.GetAbsolutePoint(new PointF(xPosition + width / 2f, num2 + height / 2f));
					absolutePoint3 = graph.GetAbsolutePoint(new PointF(xPosition + width / 2f, num2 - height / 2f));
					graphicsPath.AddLine(absolutePoint2, absolutePoint);
					graphicsPath.AddLine(absolutePoint, absolutePoint3);
					graphicsPath.AddLine(absolutePoint3, absolutePoint3);
					graph.FillPath(new SolidBrush(point.Color), graphicsPath);
				}
				graphicsPath?.Dispose();
			}
			else
			{
				if (flag && logValue <= vAxis.GetViewMaximum() && logValue >= vAxis.GetViewMinimum())
				{
					graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, new PointF(xPosition - width / 2f, num), new PointF(xPosition, num), ser.ShadowColor, ser.ShadowOffset);
				}
				if (flag2 && logValue2 <= vAxis.GetViewMaximum() && logValue2 >= vAxis.GetViewMinimum())
				{
					graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, new PointF(xPosition, num2), new PointF(xPosition + width / 2f, num2), ser.ShadowColor, ser.ShadowOffset);
				}
			}
			if (flag3)
			{
				graph.ResetClip();
			}
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
				int num = 3;
				string strA = "";
				if (point.IsAttributeSet("LabelValueType"))
				{
					strA = point["LabelValueType"];
				}
				else if (ser.IsAttributeSet("LabelValueType"))
				{
					strA = ser["LabelValueType"];
				}
				if (string.Compare(strA, "High", StringComparison.OrdinalIgnoreCase) == 0)
				{
					num = 0;
				}
				else if (string.Compare(strA, "Low", StringComparison.OrdinalIgnoreCase) == 0)
				{
					num = 1;
				}
				else if (string.Compare(strA, "Open", StringComparison.OrdinalIgnoreCase) == 0)
				{
					num = 2;
				}
				text = ValueConverter.FormatValue(ser.chart, point, point.YValues[num], point.LabelFormat, ser.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = point.ReplaceKeywords(point.Label);
				if (ser.chart != null && ser.chart.LocalizeTextHandler != null)
				{
					text = ser.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
				}
			}
			int angle = point.FontAngle;
			if (text.Trim().Length == 0)
			{
				return;
			}
			SizeF labelSize = SizeF.Empty;
			if (ser.SmartLabels.Enabled)
			{
				SizeF size = SizeF.Empty;
				size.Width = point.MarkerSize;
				size.Height = point.MarkerSize;
				if (point.MarkerImage.Length > 0)
				{
					common.ImageLoader.GetAdjustedImageSize(point.MarkerImage, graph.Graphics, ref size);
				}
				size = graph.GetRelativeSize(size);
				labelSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
				position = area.smartLabels.AdjustSmartLabelPosition(common, graph, area, ser.SmartLabels, position, labelSize, ref format, position, size, LabelAlignmentTypes.Top);
				angle = 0;
			}
			if (position.IsEmpty)
			{
				return;
			}
			RectangleF backPosition = RectangleF.Empty;
			if (!point.LabelBackColor.IsEmpty || point.LabelBorderWidth > 0 || !point.LabelBorderColor.IsEmpty)
			{
				if (labelSize.IsEmpty)
				{
					labelSize = graph.GetRelativeSize(graph.MeasureString(text, point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
				}
				position.Y -= labelSize.Height / 8f;
				SizeF size2 = new SizeF(labelSize.Width, labelSize.Height);
				size2.Height += labelSize.Height / 8f;
				size2.Width += size2.Width / (float)text.Length;
				backPosition = PointChart.GetLabelPosition(graph, position, size2, format, adjustForDrawing: true);
			}
			graph.DrawPointLabelStringRel(common, text, point.Font, new SolidBrush(point.FontColor), position, format, angle, backPosition, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, ser, point, pointIndex - 1);
		}

		protected virtual void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			ArrayList seriesFromChartType = area.GetSeriesFromChartType(Name);
			bool flag = area.IndexedSeries((string[])seriesFromChartType.ToArray(typeof(string)));
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, Name, StringComparison.OrdinalIgnoreCase) != 0 || item.ChartArea != area.Name || !item.IsVisible() || (seriesToDraw != null && seriesToDraw.Name != item.Name))
				{
					continue;
				}
				if (item.YValuesPerPoint < 4)
				{
					throw new ArgumentException(SR.ExceptionChartTypeRequiresYValues("StockChart", "4"));
				}
				hAxis = area.GetAxis(AxisName.X, item.XAxisType, item.XSubAxisName);
				vAxis = area.GetAxis(AxisName.Y, item.YAxisType, item.YSubAxisName);
				double interval = flag ? 1.0 : area.GetPointsInterval(hAxis.Logarithmic, hAxis.logarithmBase);
				float num = (float)item.GetPointWidth(graph, hAxis, interval, 0.8);
				if (!selection)
				{
					common.EventsManager.OnBackPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
				area.GetSeriesZPositionAndDepth(item, out float depth, out float positionZ);
				int num2 = 1;
				foreach (DataPoint point in item.Points)
				{
					point.positionRel = new PointF(float.NaN, float.NaN);
					double num3 = point.XValue;
					if (flag)
					{
						num3 = num2;
					}
					float num4 = (float)hAxis.GetPosition(num3);
					double logValue = vAxis.GetLogValue(point.YValues[0]);
					double logValue2 = vAxis.GetLogValue(point.YValues[1]);
					num3 = hAxis.GetLogValue(num3);
					if (num3 < hAxis.GetViewMinimum() || num3 > hAxis.GetViewMaximum() || (logValue < vAxis.GetViewMinimum() && logValue2 < vAxis.GetViewMinimum()) || (logValue > vAxis.GetViewMaximum() && logValue2 > vAxis.GetViewMaximum()))
					{
						num2++;
						continue;
					}
					bool flag2 = false;
					if (num3 == hAxis.GetViewMinimum() || num3 == hAxis.GetViewMaximum())
					{
						graph.SetClip(area.PlotAreaPosition.ToRectangleF());
						flag2 = true;
					}
					double num5 = vAxis.GetLogValue(point.YValues[0]);
					double num6 = vAxis.GetLogValue(point.YValues[1]);
					if (num5 > vAxis.GetViewMaximum())
					{
						num5 = vAxis.GetViewMaximum();
					}
					if (num5 < vAxis.GetViewMinimum())
					{
						num5 = vAxis.GetViewMinimum();
					}
					num5 = (float)vAxis.GetLinearPosition(num5);
					if (num6 > vAxis.GetViewMaximum())
					{
						num6 = vAxis.GetViewMaximum();
					}
					if (num6 < vAxis.GetViewMinimum())
					{
						num6 = vAxis.GetViewMinimum();
					}
					num6 = vAxis.GetLinearPosition(num6);
					point.positionRel = new PointF(num4, (float)num5);
					Point3D[] array = new Point3D[2]
					{
						new Point3D(num4, (float)num5, positionZ + depth / 2f),
						new Point3D(num4, (float)num6, positionZ + depth / 2f)
					};
					area.matrix3D.TransformPoints(array);
					graph.StartHotRegion(point);
					graph.StartAnimation();
					graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, array[0].PointF, array[1].PointF, item.ShadowColor, item.ShadowOffset);
					DrawOpenCloseMarks3D(graph, area, item, point, num4, num, positionZ, depth);
					num4 = array[0].X;
					num5 = array[0].Y;
					num6 = array[1].Y;
					graph.StopAnimation();
					graph.EndHotRegion();
					if (flag2)
					{
						graph.ResetClip();
					}
					if (common.ProcessModeRegions)
					{
						RectangleF empty = RectangleF.Empty;
						empty.X = num4 - num / 2f;
						empty.Y = (float)Math.Min(num5, num6);
						empty.Width = num;
						empty.Height = (float)Math.Max(num5, num6) - empty.Y;
						common.HotRegionsList.AddHotRegion(graph, empty, point, item.Name, num2 - 1);
					}
					num2++;
				}
				int num7 = 0;
				num2 = 1;
				foreach (DataPoint point2 in item.Points)
				{
					double num8 = point2.XValue;
					if (flag)
					{
						num8 = num2;
					}
					float x = (float)hAxis.GetPosition(num8);
					double logValue3 = vAxis.GetLogValue(point2.YValues[0]);
					double logValue4 = vAxis.GetLogValue(point2.YValues[1]);
					num8 = hAxis.GetLogValue(num8);
					if (num8 < hAxis.GetViewMinimum() || num8 > hAxis.GetViewMaximum() || (logValue3 < vAxis.GetViewMinimum() && logValue4 < vAxis.GetViewMinimum()) || (logValue3 > vAxis.GetViewMaximum() && logValue4 > vAxis.GetViewMaximum()))
					{
						num2++;
						continue;
					}
					double num9 = vAxis.GetLogValue(point2.YValues[0]);
					double num10 = vAxis.GetLogValue(point2.YValues[1]);
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
					Point3D[] array2 = new Point3D[2]
					{
						new Point3D(x, (float)num9, positionZ + depth / 2f),
						new Point3D(x, (float)num10, positionZ + depth / 2f)
					};
					area.matrix3D.TransformPoints(array2);
					x = array2[0].X;
					num9 = array2[0].Y;
					num10 = array2[1].Y;
					graph.StartAnimation();
					DrawLabel(common, area, graph, item, point2, new PointF(x, (float)Math.Min(num9, num10)), num2);
					graph.StopAnimation();
					if (point2.MarkerStyle != 0 || point2.MarkerImage.Length > 0)
					{
						SizeF size = SizeF.Empty;
						size.Width = point2.MarkerSize;
						size.Height = point2.MarkerSize;
						if (point2.MarkerImage.Length > 0)
						{
							common.ImageLoader.GetAdjustedImageSize(point2.MarkerImage, graph.Graphics, ref size);
						}
						PointF empty2 = PointF.Empty;
						empty2.X = x;
						empty2.Y = (float)num9 - graph.GetRelativeSize(size).Height / 2f;
						if (num7 == 0)
						{
							graph.StartAnimation();
							graph.DrawMarkerRel(empty2, point2.MarkerStyle, (int)size.Height, (point2.MarkerColor == Color.Empty) ? point2.Color : point2.MarkerColor, (point2.MarkerBorderColor == Color.Empty) ? point2.BorderColor : point2.MarkerBorderColor, point2.MarkerBorderWidth, point2.MarkerImage, point2.MarkerImageTransparentColor, (point2.series != null) ? point2.series.ShadowOffset : 0, (point2.series != null) ? point2.series.ShadowColor : Color.Empty, new RectangleF(empty2.X, empty2.Y, size.Width, size.Height));
							graph.StopAnimation();
							if (common.ProcessModeRegions)
							{
								SizeF relativeSize = graph.GetRelativeSize(size);
								int insertIndex = common.HotRegionsList.FindInsertIndex();
								common.HotRegionsList.FindInsertIndex();
								if (point2.MarkerStyle == MarkerStyle.Circle)
								{
									float[] array3 = new float[3]
									{
										empty2.X,
										empty2.Y,
										relativeSize.Width / 2f
									};
									common.HotRegionsList.AddHotRegion(insertIndex, graph, array3[0], array3[1], array3[2], point2, item.Name, num2 - 1);
								}
								else
								{
									common.HotRegionsList.AddHotRegion(graph, new RectangleF(empty2.X - relativeSize.Width / 2f, empty2.Y - relativeSize.Height / 2f, relativeSize.Width, relativeSize.Height), point2, item.Name, num2 - 1);
								}
							}
						}
						num7++;
						if (item.MarkerStep == num7)
						{
							num7 = 0;
						}
					}
					num2++;
				}
				if (!selection)
				{
					common.EventsManager.OnPaint(item, new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
			}
		}

		protected virtual void DrawOpenCloseMarks3D(ChartGraphics graph, ChartArea area, Series ser, DataPoint point, float xPosition, float width, float zPosition, float depth)
		{
			double logValue = vAxis.GetLogValue(point.YValues[2]);
			double logValue2 = vAxis.GetLogValue(point.YValues[3]);
			if ((logValue > vAxis.GetViewMaximum() || logValue < vAxis.GetViewMinimum()) && !(logValue2 > vAxis.GetViewMaximum()))
			{
				vAxis.GetViewMinimum();
			}
			float num = (float)vAxis.GetLinearPosition(logValue);
			float num2 = (float)vAxis.GetLinearPosition(logValue2);
			SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF(width, width));
			float height = graph.GetRelativeSize(absoluteSize).Height;
			StockOpenCloseMarkStyle stockOpenCloseMarkStyle = openCloseStyle;
			string text = "";
			if (point.IsAttributeSet("OpenCloseStyle"))
			{
				text = point["OpenCloseStyle"];
			}
			else if (ser.IsAttributeSet("OpenCloseStyle"))
			{
				text = ser["OpenCloseStyle"];
			}
			if (text != null && text.Length > 0)
			{
				if (string.Compare(text, "Candlestick", StringComparison.OrdinalIgnoreCase) == 0)
				{
					stockOpenCloseMarkStyle = StockOpenCloseMarkStyle.Candlestick;
				}
				else if (string.Compare(text, "Triangle", StringComparison.OrdinalIgnoreCase) == 0)
				{
					stockOpenCloseMarkStyle = StockOpenCloseMarkStyle.Triangle;
				}
				else if (string.Compare(text, "Line", StringComparison.OrdinalIgnoreCase) == 0)
				{
					stockOpenCloseMarkStyle = StockOpenCloseMarkStyle.Line;
				}
			}
			bool flag = true;
			bool flag2 = true;
			string text2 = "";
			if (point.IsAttributeSet("ShowOpenClose"))
			{
				text2 = point["ShowOpenClose"];
			}
			else if (ser.IsAttributeSet("ShowOpenClose"))
			{
				text2 = ser["ShowOpenClose"];
			}
			if (text2 != null && text2.Length > 0)
			{
				if (string.Compare(text2, "Both", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
					flag2 = true;
				}
				else if (string.Compare(text2, "Open", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
					flag2 = false;
				}
				else if (string.Compare(text2, "Close", StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = false;
					flag2 = true;
				}
			}
			bool flag3 = false;
			if (xPosition - width / 2f < area.PlotAreaPosition.X || xPosition + width / 2f > area.PlotAreaPosition.Right())
			{
				graph.SetClip(area.PlotAreaPosition.ToRectangleF());
				flag3 = true;
			}
			if (forceCandleStick || stockOpenCloseMarkStyle == StockOpenCloseMarkStyle.Candlestick)
			{
				ColorConverter colorConverter = new ColorConverter();
				Color color = point.Color;
				Color color2 = point.BackGradientEndColor;
				string text3 = point["PriceUpColor"];
				if (text3 != null && text3.Length > 0)
				{
					try
					{
						color = (Color)colorConverter.ConvertFromString(text3);
					}
					catch
					{
						color = (Color)colorConverter.ConvertFromInvariantString(text3);
					}
				}
				text3 = point["PriceDownColor"];
				if (text3 != null && text3.Length > 0)
				{
					try
					{
						color2 = (Color)colorConverter.ConvertFromString(text3);
					}
					catch
					{
						color2 = (Color)colorConverter.ConvertFromInvariantString(text3);
					}
				}
				RectangleF empty = RectangleF.Empty;
				empty.Y = Math.Min(num, num2);
				empty.X = xPosition - width / 2f;
				empty.Height = Math.Max(num, num2) - empty.Y;
				empty.Width = width;
				Color color3 = (num > num2) ? color : color2;
				Color color4 = (!(point.BorderColor == Color.Empty)) ? point.BorderColor : ((color3 == Color.Empty) ? point.Color : color3);
				Point3D[] array = new Point3D[2]
				{
					new Point3D(empty.X, empty.Y, zPosition + depth / 2f),
					new Point3D(empty.Right, empty.Bottom, zPosition + depth / 2f)
				};
				area.matrix3D.TransformPoints(array);
				empty.Location = array[0].PointF;
				empty.Width = Math.Abs(array[1].X - array[0].X);
				empty.Height = Math.Abs(array[1].Y - array[0].Y);
				if (empty.Height > 1f)
				{
					graph.FillRectangleRel(empty, color3, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, point.BackGradientEndColor, color4, point.BorderWidth, point.BorderStyle, ser.ShadowColor, ser.ShadowOffset, PenAlignment.Inset);
				}
				else
				{
					graph.DrawLineRel(color4, point.BorderWidth, point.BorderStyle, new PointF(empty.X, empty.Y), new PointF(empty.Right, empty.Y), ser.ShadowColor, ser.ShadowOffset);
				}
			}
			else if (stockOpenCloseMarkStyle == StockOpenCloseMarkStyle.Triangle)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				Point3D[] array2 = new Point3D[3]
				{
					new Point3D(xPosition, num, zPosition + depth / 2f),
					new Point3D(xPosition - width / 2f, num + height / 2f, zPosition + depth / 2f),
					new Point3D(xPosition - width / 2f, num - height / 2f, zPosition + depth / 2f)
				};
				area.matrix3D.TransformPoints(array2);
				array2[0].PointF = graph.GetAbsolutePoint(array2[0].PointF);
				array2[1].PointF = graph.GetAbsolutePoint(array2[1].PointF);
				array2[2].PointF = graph.GetAbsolutePoint(array2[2].PointF);
				if (flag && logValue <= vAxis.GetViewMaximum() && logValue >= vAxis.GetViewMinimum())
				{
					graphicsPath.AddLine(array2[1].PointF, array2[0].PointF);
					graphicsPath.AddLine(array2[0].PointF, array2[2].PointF);
					graphicsPath.AddLine(array2[2].PointF, array2[2].PointF);
					graph.FillPath(new SolidBrush(point.Color), graphicsPath);
				}
				if (flag2 && logValue2 <= vAxis.GetViewMaximum() && logValue2 >= vAxis.GetViewMinimum())
				{
					array2[0] = new Point3D(xPosition, num2, zPosition + depth / 2f);
					array2[1] = new Point3D(xPosition + width / 2f, num2 + height / 2f, zPosition + depth / 2f);
					array2[2] = new Point3D(xPosition + width / 2f, num2 - height / 2f, zPosition + depth / 2f);
					area.matrix3D.TransformPoints(array2);
					array2[0].PointF = graph.GetAbsolutePoint(array2[0].PointF);
					array2[1].PointF = graph.GetAbsolutePoint(array2[1].PointF);
					array2[2].PointF = graph.GetAbsolutePoint(array2[2].PointF);
					graphicsPath.Reset();
					graphicsPath.AddLine(array2[1].PointF, array2[0].PointF);
					graphicsPath.AddLine(array2[0].PointF, array2[2].PointF);
					graphicsPath.AddLine(array2[2].PointF, array2[2].PointF);
					graph.FillPath(new SolidBrush(point.Color), graphicsPath);
				}
				graphicsPath?.Dispose();
			}
			else
			{
				if (flag && logValue <= vAxis.GetViewMaximum() && logValue >= vAxis.GetViewMinimum())
				{
					Point3D[] array3 = new Point3D[2]
					{
						new Point3D(xPosition - width / 2f, num, zPosition + depth / 2f),
						new Point3D(xPosition, num, zPosition + depth / 2f)
					};
					area.matrix3D.TransformPoints(array3);
					graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, array3[0].PointF, array3[1].PointF, ser.ShadowColor, ser.ShadowOffset);
				}
				if (flag2 && logValue2 <= vAxis.GetViewMaximum() && logValue2 >= vAxis.GetViewMinimum())
				{
					Point3D[] array4 = new Point3D[2]
					{
						new Point3D(xPosition, num2, zPosition + depth / 2f),
						new Point3D(xPosition + width / 2f, num2, zPosition + depth / 2f)
					};
					area.matrix3D.TransformPoints(array4);
					graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, array4[0].PointF, array4[1].PointF, ser.ShadowColor, ser.ShadowOffset);
				}
			}
			if (flag3)
			{
				graph.ResetClip();
			}
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
			bool flag = area.IndexedSeries((string[])area.GetSeriesFromChartType(Name).ToArray(typeof(string)));
			Axis axis = area.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
			Axis axis2 = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
			int num = 0;
			int num2 = 1;
			foreach (DataPoint point in series.Points)
			{
				double yValue = GetYValue(common, area, series, point, num2 - 1, 0);
				yValue = axis2.GetLogValue(yValue);
				if (yValue > axis2.GetViewMaximum() || yValue < axis2.GetViewMinimum())
				{
					num2++;
					continue;
				}
				double yValue2 = flag ? ((double)num2) : point.XValue;
				yValue2 = axis.GetLogValue(yValue2);
				if (yValue2 > axis.GetViewMaximum() || yValue2 < axis.GetViewMinimum())
				{
					num2++;
					continue;
				}
				PointF pointF = PointF.Empty;
				pointF.Y = (float)axis2.GetLinearPosition(yValue);
				if (flag)
				{
					pointF.X = (float)axis.GetPosition(num2);
				}
				else
				{
					pointF.X = (float)axis.GetPosition(point.XValue);
				}
				_ = point.MarkerSize;
				string markerImage = point.MarkerImage;
				MarkerStyle markerStyle = point.MarkerStyle;
				SizeF size = SizeF.Empty;
				size.Width = point.MarkerSize;
				size.Height = point.MarkerSize;
				if (point.MarkerImage.Length > 0 && common.graph != null)
				{
					common.ImageLoader.GetAdjustedImageSize(point.MarkerImage, common.graph.Graphics, ref size);
				}
				if (area.Area3DStyle.Enable3D)
				{
					area.GetSeriesZPositionAndDepth(series, out float depth, out float positionZ);
					Point3D[] array = new Point3D[1]
					{
						new Point3D(pointF.X, pointF.Y, positionZ + depth / 2f)
					};
					area.matrix3D.TransformPoints(array);
					pointF = array[0].PointF;
				}
				if (markerStyle != 0 || markerImage.Length > 0)
				{
					if (num == 0)
					{
						size = common.graph.GetRelativeSize(size);
						RectangleF rectangleF = new RectangleF(pointF.X - size.Width / 2f, pointF.Y - size.Height, size.Width, size.Height);
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
