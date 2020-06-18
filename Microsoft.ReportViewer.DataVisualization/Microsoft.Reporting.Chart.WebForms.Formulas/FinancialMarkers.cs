using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Formulas
{
	internal class FinancialMarkers
	{
		private Font textFont;

		private Color textColor;

		private Color lineColor;

		private int lineWidth;

		private ChartDashStyle lineStyle = ChartDashStyle.Solid;

		private bool axesSwitched;

		private bool drawText = true;

		internal void DrawMarkers(ChartGraphics graph, ChartPicture chart, FinancialMarkerType markerName, Series series, int firstPoint, int firstYValue, int secondPoint, int secondYValue, Color lineColor, int lineWidth, ChartDashStyle lineStyle, Color textColor, Font textFont)
		{
			this.lineStyle = lineStyle;
			if (textColor == Color.Empty)
			{
				drawText = false;
			}
			else
			{
				this.textColor = textColor;
				drawText = true;
			}
			if (lineColor == Color.Empty)
			{
				this.lineColor = Color.Gray;
			}
			else
			{
				this.lineColor = lineColor;
			}
			if (lineWidth == 0)
			{
				this.lineWidth = 1;
			}
			else
			{
				this.lineWidth = lineWidth;
			}
			if (textFont == null)
			{
				this.textFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);
			}
			else
			{
				this.textFont = textFont;
			}
			ChartArea chartArea = chart.ChartAreas[series.ChartArea];
			if (chartArea.Area3DStyle.Enable3D || chartArea.chartAreaIsCurcular || !chartArea.requireAxes)
			{
				return;
			}
			Axis axis = chartArea.GetAxis(AxisName.X, series.XAxisType, series.XSubAxisName);
			Axis axis2 = chartArea.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
			double position;
			double position2;
			double position3;
			double position4;
			try
			{
				if (chartArea.Common.ChartTypeRegistry.GetChartType(series.ChartTypeName).SwitchValueAxes)
				{
					axesSwitched = true;
					Axis axis3 = axis2;
					axis2 = axis;
					axis = axis3;
					position = axis.GetPosition(series.Points[firstPoint].YValues[firstYValue]);
					position2 = axis2.GetPosition(series.Points[firstPoint].XValue);
					position3 = axis.GetPosition(series.Points[secondPoint].YValues[secondYValue]);
					position4 = axis2.GetPosition(series.Points[secondPoint].XValue);
				}
				else
				{
					axesSwitched = false;
					position = axis.GetPosition(series.Points[firstPoint].XValue);
					position2 = axis2.GetPosition(series.Points[firstPoint].YValues[firstYValue]);
					position3 = axis.GetPosition(series.Points[secondPoint].XValue);
					position4 = axis2.GetPosition(series.Points[secondPoint].YValues[secondYValue]);
				}
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionFinancialMarkersSeriesPointYValueIndexInvalid);
			}
			PointF relative = new PointF((float)position, (float)position2);
			PointF relative2 = new PointF((float)position3, (float)position4);
			relative = graph.GetAbsolutePoint(relative);
			relative2 = graph.GetAbsolutePoint(relative2);
			bool flag = false;
			foreach (DataPoint point in series.Points)
			{
				flag = true;
				if (point.XValue != 0.0)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				throw new InvalidOperationException(SR.ExceptionFormulaDataZeroIndexedXValuesUnsupported);
			}
			graph.SetClip(chartArea.PlotAreaPosition.ToRectangleF());
			SmoothingMode smoothingMode = graph.SmoothingMode;
			if ((graph.AntiAliasing & AntiAliasingTypes.Graphics) == AntiAliasingTypes.Graphics)
			{
				graph.SmoothingMode = SmoothingMode.AntiAlias;
			}
			switch (markerName)
			{
			case FinancialMarkerType.FibonacciArcs:
				FibonacciArcs(graph, relative, relative2);
				break;
			case FinancialMarkerType.TrendLine:
				TrendLine(graph, relative, relative2);
				break;
			case FinancialMarkerType.FibonacciFans:
				FibonacciFans(graph, relative, relative2, chartArea);
				break;
			case FinancialMarkerType.FibonacciRetracements:
				FibonacciRetracements(graph, relative, relative2, chartArea);
				break;
			case FinancialMarkerType.SpeedResistanceLines:
				SpeedResistanceLines(graph, relative, relative2, chartArea);
				break;
			case FinancialMarkerType.QuadrantLines:
				QuadrantLines(graph, relative, relative2, chartArea);
				break;
			default:
				throw new InvalidOperationException(SR.ExceptionFinancialMarkersFormulaNotFound);
			}
			graph.ResetClip();
			graph.SmoothingMode = smoothingMode;
		}

		private void FibonacciArcs(ChartGraphics graph, PointF firstPoint, PointF secondPoint)
		{
			float[] array = new float[4]
			{
				0.382f,
				0.5f,
				0.618f,
				1f
			};
			float num = (float)Math.Sqrt(Math.Pow(firstPoint.X - secondPoint.X, 2.0) + Math.Pow(firstPoint.Y - secondPoint.Y, 2.0));
			for (int i = 0; i < array.Length; i++)
			{
				using (Pen pen = new Pen(lineColor, lineWidth))
				{
					pen.DashStyle = graph.GetPenStyle(lineStyle);
					graph.DrawEllipse(pen, secondPoint.X - num * array[i], secondPoint.Y - num * array[i], num * array[i] * 2f, num * array[i] * 2f);
				}
				if (drawText)
				{
					string text = (array[i] * 100f).ToString(CultureInfo.InvariantCulture) + " %";
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					graph.DrawStringRel(text, textFont, new SolidBrush(textColor), graph.GetRelativePoint(new PointF(secondPoint.X, secondPoint.Y + num * array[i])), stringFormat, 0);
				}
			}
		}

		private void FibonacciFans(ChartGraphics graph, PointF firstPoint, PointF secondPoint, ChartArea area)
		{
			float[] array = new float[4]
			{
				0.382f,
				0.5f,
				0.618f,
				1f
			};
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Far;
			if (secondPoint.X == firstPoint.X)
			{
				throw new InvalidOperationException(SR.ExceptionFinancialMarkersDataPointsHaveSameXValues);
			}
			float num = (secondPoint.Y - firstPoint.Y) / (secondPoint.X - firstPoint.X);
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(area.PlotAreaPosition.ToRectangleF());
			for (int i = 0; i < array.Length; i++)
			{
				PointF pt = axesSwitched ? ((firstPoint.Y > secondPoint.Y) ? new PointF(firstPoint.X - (firstPoint.Y - absoluteRectangle.Top) / num * array[i], absoluteRectangle.Top) : new PointF((absoluteRectangle.Bottom - firstPoint.Y) * array[i] / num + firstPoint.X, absoluteRectangle.Bottom)) : ((firstPoint.X > secondPoint.X) ? new PointF(absoluteRectangle.Left, firstPoint.Y - num * (firstPoint.X - absoluteRectangle.Left) * array[i]) : new PointF(absoluteRectangle.Right, num * (absoluteRectangle.Right - firstPoint.X) * array[i] + firstPoint.Y));
				using (Pen pen = new Pen(lineColor, lineWidth))
				{
					pen.DashStyle = graph.GetPenStyle(lineStyle);
					graph.DrawLine(pen, firstPoint, pt);
				}
				if (drawText)
				{
					string text = (array[i] * 100f).ToString(CultureInfo.InvariantCulture) + " %";
					if (axesSwitched)
					{
						graph.DrawStringRel(position: graph.GetRelativePoint(new PointF(firstPoint.X - (firstPoint.X - secondPoint.X) * array[i], secondPoint.Y)), text: text, font: textFont, brush: new SolidBrush(textColor), format: stringFormat, angle: 270 - (int)(Math.Atan(array[i] / num) / 3.1400001049041748 * 180.0));
						continue;
					}
					graph.DrawStringRel(position: graph.GetRelativePoint(new PointF(secondPoint.X, firstPoint.Y - (firstPoint.Y - secondPoint.Y) * array[i])), text: text, font: textFont, brush: new SolidBrush(textColor), format: stringFormat, angle: (int)(Math.Atan(num * array[i]) / 3.1400001049041748 * 180.0));
				}
			}
		}

		private void SpeedResistanceLines(ChartGraphics graph, PointF firstPoint, PointF secondPoint, ChartArea area)
		{
			float[] array = new float[3]
			{
				0.333f,
				0.666f,
				1f
			};
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Far;
			if (secondPoint.X == firstPoint.X)
			{
				throw new InvalidOperationException(SR.ExceptionFinancialMarkersDataPointsHaveSameXValues);
			}
			float num = (secondPoint.Y - firstPoint.Y) / (secondPoint.X - firstPoint.X);
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(area.PlotAreaPosition.ToRectangleF());
			for (int i = 0; i < array.Length; i++)
			{
				PointF pt = axesSwitched ? ((firstPoint.Y > secondPoint.Y) ? new PointF(firstPoint.X - (firstPoint.Y - absoluteRectangle.Top) / num * array[i], absoluteRectangle.Top) : new PointF((absoluteRectangle.Bottom - firstPoint.Y) * array[i] / num + firstPoint.X, absoluteRectangle.Bottom)) : ((firstPoint.X > secondPoint.X) ? new PointF(absoluteRectangle.Left, firstPoint.Y - num * (firstPoint.X - absoluteRectangle.Left) * array[i]) : new PointF(absoluteRectangle.Right, num * (absoluteRectangle.Right - firstPoint.X) * array[i] + firstPoint.Y));
				using (Pen pen = new Pen(lineColor, lineWidth))
				{
					pen.DashStyle = graph.GetPenStyle(lineStyle);
					graph.DrawLine(pen, firstPoint, pt);
				}
				if (drawText)
				{
					string text = (array[i] * 100f).ToString(CultureInfo.InvariantCulture) + " %";
					if (axesSwitched)
					{
						graph.DrawStringRel(position: graph.GetRelativePoint(new PointF(firstPoint.X - (firstPoint.X - secondPoint.X) * array[i], secondPoint.Y)), text: text, font: textFont, brush: new SolidBrush(textColor), format: stringFormat, angle: 270 - (int)(Math.Atan(array[i] / num) / 3.1400001049041748 * 180.0));
						continue;
					}
					graph.DrawStringRel(position: graph.GetRelativePoint(new PointF(secondPoint.X, firstPoint.Y - (firstPoint.Y - secondPoint.Y) * array[i])), text: text, font: textFont, brush: new SolidBrush(textColor), format: stringFormat, angle: (int)(Math.Atan(num * array[i]) / 3.1400001049041748 * 180.0));
				}
			}
		}

		private void FibonacciRetracements(ChartGraphics graph, PointF firstPoint, PointF secondPoint, ChartArea area)
		{
			float[] array = new float[9]
			{
				0f,
				0.236f,
				0.382f,
				0.5f,
				0.618f,
				1f,
				1.618f,
				2.618f,
				4.236f
			};
			Pen pen = new Pen(lineColor, lineWidth);
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(area.PlotAreaPosition.ToRectangleF());
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == 0f || (double)array[i] == 1.0)
				{
					pen.DashStyle = graph.GetPenStyle(lineStyle);
				}
				else
				{
					pen.DashStyle = DashStyle.Dash;
				}
				float num;
				PointF pt;
				PointF pt2;
				if (axesSwitched)
				{
					num = (firstPoint.X - secondPoint.X) * array[i] + secondPoint.X;
					pt = new PointF(num, absoluteRectangle.Top);
					pt2 = new PointF(num, absoluteRectangle.Bottom);
				}
				else
				{
					num = (firstPoint.Y - secondPoint.Y) * array[i] + secondPoint.Y;
					pt = new PointF(absoluteRectangle.Left, num);
					pt2 = new PointF(absoluteRectangle.Right, num);
				}
				SmoothingMode smoothingMode = graph.SmoothingMode;
				graph.SmoothingMode = SmoothingMode.None;
				graph.DrawLine(pen, pt, pt2);
				graph.SmoothingMode = smoothingMode;
				if (drawText)
				{
					string text = (array[i] * 100f).ToString(CultureInfo.InvariantCulture) + " %";
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					if (axesSwitched)
					{
						graph.DrawStringRel(text, textFont, new SolidBrush(textColor), graph.GetRelativePoint(new PointF(pt.X, (firstPoint.Y + secondPoint.Y) / 2f)), stringFormat, 90);
					}
					else
					{
						graph.DrawStringRel(text, textFont, new SolidBrush(textColor), graph.GetRelativePoint(new PointF((firstPoint.X + secondPoint.X) / 2f, pt.Y)), stringFormat, 0);
					}
				}
				if (num > absoluteRectangle.Bottom || num < absoluteRectangle.Top)
				{
					break;
				}
			}
			pen?.Dispose();
		}

		private void QuadrantLines(ChartGraphics graph, PointF firstPoint, PointF secondPoint, ChartArea area)
		{
			float[] array = new float[5]
			{
				0f,
				0.25f,
				0.5f,
				0.75f,
				1f
			};
			Pen pen = new Pen(lineColor, lineWidth);
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(area.PlotAreaPosition.ToRectangleF());
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == 0.5f)
				{
					pen.DashStyle = DashStyle.Dash;
				}
				else
				{
					pen.DashStyle = graph.GetPenStyle(lineStyle);
				}
				float num;
				PointF pt;
				PointF pt2;
				if (axesSwitched)
				{
					num = (firstPoint.X - secondPoint.X) * array[i] + secondPoint.X;
					pt = new PointF(num, absoluteRectangle.Top);
					pt2 = new PointF(num, absoluteRectangle.Bottom);
				}
				else
				{
					num = (firstPoint.Y - secondPoint.Y) * array[i] + secondPoint.Y;
					pt = new PointF(absoluteRectangle.Left, num);
					pt2 = new PointF(absoluteRectangle.Right, num);
				}
				SmoothingMode smoothingMode = graph.SmoothingMode;
				graph.SmoothingMode = SmoothingMode.None;
				graph.DrawLine(pen, pt, pt2);
				graph.SmoothingMode = smoothingMode;
				if (drawText)
				{
					string text = (array[i] * 100f).ToString(CultureInfo.InvariantCulture) + " %";
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					if (axesSwitched)
					{
						graph.DrawStringRel(text, textFont, new SolidBrush(textColor), graph.GetRelativePoint(new PointF(pt.X, (firstPoint.Y + secondPoint.Y) / 2f)), stringFormat, 90);
					}
					else
					{
						graph.DrawStringRel(text, textFont, new SolidBrush(textColor), graph.GetRelativePoint(new PointF((firstPoint.X + secondPoint.X) / 2f, pt.Y)), stringFormat, 0);
					}
				}
				if (num > absoluteRectangle.Bottom || num < absoluteRectangle.Top)
				{
					break;
				}
			}
			pen?.Dispose();
		}

		private void TrendLine(ChartGraphics graph, PointF firstPoint, PointF secondPoint)
		{
			using (Pen pen = new Pen(lineColor, lineWidth))
			{
				pen.DashStyle = graph.GetPenStyle(lineStyle);
				graph.DrawLine(pen, firstPoint, secondPoint);
			}
		}
	}
}
