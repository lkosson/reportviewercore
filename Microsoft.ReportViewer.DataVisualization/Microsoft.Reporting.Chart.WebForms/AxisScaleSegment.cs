using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAxisScaleSegment_AxisScaleSegment")]
	internal class AxisScaleSegment
	{
		internal Axis axis;

		private double position;

		private double size;

		private double spacing;

		private double scaleMinimum;

		private double scaleMaximum;

		private double intervalOffset;

		private double interval;

		private DateTimeIntervalType intervalType;

		private DateTimeIntervalType intervalOffsetType;

		private object tag;

		private Stack oldAxisSettings = new Stack();

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_Position")]
		public double Position
		{
			get
			{
				return position;
			}
			set
			{
				if (value < 0.0 || value > 100.0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleSegmentsPositionInvalid);
				}
				position = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_Size")]
		public double Size
		{
			get
			{
				return size;
			}
			set
			{
				if (value < 0.0 || value > 100.0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleSegmentsSizeInvalid);
				}
				size = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_Spacing")]
		public double Spacing
		{
			get
			{
				return spacing;
			}
			set
			{
				if (value < 0.0 || value > 100.0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleSegmentsSpacingInvalid);
				}
				spacing = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_ScaleMaximum")]
		public double ScaleMaximum
		{
			get
			{
				return scaleMaximum;
			}
			set
			{
				scaleMaximum = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_ScaleMinimum")]
		public double ScaleMinimum
		{
			get
			{
				return scaleMinimum;
			}
			set
			{
				scaleMinimum = value;
			}
		}

		[SRCategory("CategoryAttributeInterval")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_Interval")]
		[TypeConverter(typeof(AxisIntervalValueConverter))]
		public double Interval
		{
			get
			{
				return interval;
			}
			set
			{
				if (double.IsNaN(value))
				{
					interval = 0.0;
				}
				else
				{
					interval = value;
				}
			}
		}

		[SRCategory("CategoryAttributeInterval")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_IntervalOffset")]
		[TypeConverter(typeof(AxisIntervalValueConverter))]
		public double IntervalOffset
		{
			get
			{
				return intervalOffset;
			}
			set
			{
				if (double.IsNaN(value))
				{
					intervalOffset = 0.0;
				}
				else
				{
					intervalOffset = value;
				}
			}
		}

		[SRCategory("CategoryAttributeInterval")]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_IntervalType")]
		public DateTimeIntervalType IntervalType
		{
			get
			{
				return intervalType;
			}
			set
			{
				if (value == DateTimeIntervalType.NotSet)
				{
					intervalType = DateTimeIntervalType.Auto;
				}
				else
				{
					intervalType = value;
				}
			}
		}

		[SRCategory("CategoryAttributeInterval")]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_IntervalOffsetType")]
		public DateTimeIntervalType IntervalOffsetType
		{
			get
			{
				return intervalOffsetType;
			}
			set
			{
				if (value == DateTimeIntervalType.NotSet)
				{
					intervalOffsetType = DateTimeIntervalType.Auto;
				}
				else
				{
					intervalOffsetType = value;
				}
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeAxisScaleSegment_Tag")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public object Tag
		{
			get
			{
				return tag;
			}
			set
			{
				tag = value;
			}
		}

		internal void PaintBreakLine(ChartGraphics graph, AxisScaleSegment nextSegment)
		{
			RectangleF breakLinePosition = GetBreakLinePosition(graph, nextSegment);
			GraphicsPath breakLinePath = GetBreakLinePath(breakLinePosition, top: true);
			GraphicsPath graphicsPath = null;
			if (breakLinePosition.Width > 0f && breakLinePosition.Height > 0f)
			{
				graphicsPath = GetBreakLinePath(breakLinePosition, top: false);
				using (GraphicsPath graphicsPath2 = new GraphicsPath())
				{
					graphicsPath2.AddPath(breakLinePath, connect: true);
					graphicsPath2.Reverse();
					graphicsPath2.AddPath(graphicsPath, connect: true);
					graphicsPath2.CloseAllFigures();
					using (Brush brush = GetChartFillBrush(graph))
					{
						graph.FillPath(brush, graphicsPath2);
						if (axis.chartArea.ShadowOffset != 0 && !axis.chartArea.ShadowColor.IsEmpty)
						{
							RectangleF rect = breakLinePosition;
							if (axis.AxisPosition == AxisPosition.Right || axis.AxisPosition == AxisPosition.Left)
							{
								rect.Y += axis.chartArea.ShadowOffset;
								rect.Height -= axis.chartArea.ShadowOffset;
								rect.X = rect.Right - 1f;
								rect.Width = axis.chartArea.ShadowOffset + 2;
							}
							else
							{
								rect.X += axis.chartArea.ShadowOffset;
								rect.Width -= axis.chartArea.ShadowOffset;
								rect.Y = rect.Bottom - 1f;
								rect.Height = axis.chartArea.ShadowOffset + 2;
							}
							graph.FillRectangle(brush, rect);
							using (GraphicsPath graphicsPath3 = new GraphicsPath())
							{
								graphicsPath3.AddPath(breakLinePath, connect: false);
								float val = axis.chartArea.ShadowOffset;
								val = ((axis.AxisPosition != AxisPosition.Right && axis.AxisPosition != 0) ? Math.Min(val, breakLinePosition.Width) : Math.Min(val, breakLinePosition.Height));
								int num = (int)((float)(int)axis.chartArea.ShadowColor.A / val);
								RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(axis.PlotAreaPosition.ToRectangleF());
								if (axis.AxisPosition == AxisPosition.Right || axis.AxisPosition == AxisPosition.Left)
								{
									absoluteRectangle.X += axis.chartArea.ShadowOffset;
									absoluteRectangle.Width += axis.chartArea.ShadowOffset;
								}
								else
								{
									absoluteRectangle.Y += axis.chartArea.ShadowOffset;
									absoluteRectangle.Height += axis.chartArea.ShadowOffset;
								}
								graph.SetClip(graph.GetRelativeRectangle(absoluteRectangle));
								for (int i = 0; (float)i < val; i++)
								{
									using (Matrix matrix = new Matrix())
									{
										if (axis.AxisPosition == AxisPosition.Right || axis.AxisPosition == AxisPosition.Left)
										{
											matrix.Translate(0f, 1f);
										}
										else
										{
											matrix.Translate(1f, 0f);
										}
										graphicsPath3.Transform(matrix);
									}
									using (Pen pen = new Pen(Color.FromArgb(axis.chartArea.ShadowColor.A - num * i, axis.chartArea.ShadowColor), 1f))
									{
										graph.DrawPath(pen, graphicsPath3);
									}
								}
								graph.ResetClip();
							}
						}
					}
				}
			}
			if (axis.ScaleBreakStyle.BreakLineType != 0)
			{
				using (Pen pen2 = new Pen(axis.ScaleBreakStyle.LineColor, axis.ScaleBreakStyle.LineWidth))
				{
					pen2.DashStyle = graph.GetPenStyle(axis.ScaleBreakStyle.LineStyle);
					graph.DrawPath(pen2, breakLinePath);
					if (breakLinePosition.Width > 0f && breakLinePosition.Height > 0f)
					{
						graph.DrawPath(pen2, graphicsPath);
					}
				}
			}
			breakLinePath.Dispose();
			breakLinePath = null;
			if (graphicsPath != null)
			{
				graphicsPath.Dispose();
				graphicsPath = null;
			}
		}

		private Brush GetChartFillBrush(ChartGraphics graph)
		{
			Chart chart = axis.chartArea.Common.Chart;
			Brush brush = null;
			brush = ((chart.BackGradientType != 0) ? graph.GetGradientBrush(new RectangleF(0f, 0f, chart.chartPicture.Width - 1, chart.chartPicture.Height - 1), chart.BackColor, chart.BackGradientEndColor, chart.BackGradientType) : new SolidBrush(chart.BackColor));
			if (chart.BackHatchStyle != 0)
			{
				brush = graph.GetHatchBrush(chart.BackHatchStyle, chart.BackColor, chart.BackGradientEndColor);
			}
			if (chart.BackImage.Length > 0 && chart.BackImageMode != ChartImageWrapMode.Unscaled && chart.BackImageMode != ChartImageWrapMode.Scaled)
			{
				brush = graph.GetTextureBrush(chart.BackImage, chart.BackImageTransparentColor, chart.BackImageMode, chart.BackColor);
			}
			return brush;
		}

		private GraphicsPath GetBreakLinePath(RectangleF breakLinePosition, bool top)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			if (axis.ScaleBreakStyle.BreakLineType == BreakLineType.Wave)
			{
				PointF[] array = null;
				int num = 0;
				if (axis.AxisPosition == AxisPosition.Right || axis.AxisPosition == AxisPosition.Left)
				{
					float x = breakLinePosition.X;
					float right = breakLinePosition.Right;
					float num2 = top ? breakLinePosition.Y : breakLinePosition.Bottom;
					num = (int)(right - x) / 40 * 2;
					if (num < 2)
					{
						num = 2;
					}
					float num3 = (right - x) / (float)num;
					array = new PointF[num + 1];
					for (int i = 1; i < num + 1; i++)
					{
						array[i] = new PointF(x + (float)i * num3, num2 + ((i % 2 == 0) ? (-2f) : 2f));
					}
					array[0] = new PointF(x, num2);
					array[array.Length - 1] = new PointF(right, num2);
				}
				else
				{
					float y = breakLinePosition.Y;
					float bottom = breakLinePosition.Bottom;
					float num4 = top ? breakLinePosition.X : breakLinePosition.Right;
					num = (int)(bottom - y) / 40 * 2;
					if (num < 2)
					{
						num = 2;
					}
					float num5 = (bottom - y) / (float)num;
					array = new PointF[num + 1];
					for (int j = 1; j < num + 1; j++)
					{
						array[j] = new PointF(num4 + ((j % 2 == 0) ? (-2f) : 2f), y + (float)j * num5);
					}
					array[0] = new PointF(num4, y);
					array[array.Length - 1] = new PointF(num4, bottom);
				}
				graphicsPath.AddCurve(array, 0, num, 0.8f);
			}
			else if (axis.ScaleBreakStyle.BreakLineType == BreakLineType.Ragged)
			{
				PointF[] array2 = null;
				Random random = new Random(435657);
				if (axis.AxisPosition == AxisPosition.Right || axis.AxisPosition == AxisPosition.Left)
				{
					float x2 = breakLinePosition.X;
					float right2 = breakLinePosition.Right;
					float num6 = top ? breakLinePosition.Y : breakLinePosition.Bottom;
					float num7 = 10f;
					int num8 = (int)((right2 - x2) / num7);
					if (num8 < 2)
					{
						num8 = 2;
					}
					array2 = new PointF[num8];
					for (int k = 1; k < num8 - 1; k++)
					{
						array2[k] = new PointF(x2 + (float)k * num7, num6 + (float)random.Next(-3, 3));
					}
					array2[0] = new PointF(x2, num6);
					array2[array2.Length - 1] = new PointF(right2, num6);
				}
				else
				{
					float y2 = breakLinePosition.Y;
					float bottom2 = breakLinePosition.Bottom;
					float num9 = top ? breakLinePosition.X : breakLinePosition.Right;
					float num10 = 10f;
					int num11 = (int)((bottom2 - y2) / num10);
					if (num11 < 2)
					{
						num11 = 2;
					}
					array2 = new PointF[num11];
					for (int l = 1; l < num11 - 1; l++)
					{
						array2[l] = new PointF(num9 + (float)random.Next(-3, 3), y2 + (float)l * num10);
					}
					array2[0] = new PointF(num9, y2);
					array2[array2.Length - 1] = new PointF(num9, bottom2);
				}
				graphicsPath.AddLines(array2);
			}
			else if (axis.AxisPosition == AxisPosition.Right || axis.AxisPosition == AxisPosition.Left)
			{
				if (top)
				{
					graphicsPath.AddLine(breakLinePosition.X, breakLinePosition.Y, breakLinePosition.Right, breakLinePosition.Y);
				}
				else
				{
					graphicsPath.AddLine(breakLinePosition.X, breakLinePosition.Bottom, breakLinePosition.Right, breakLinePosition.Bottom);
				}
			}
			else if (top)
			{
				graphicsPath.AddLine(breakLinePosition.X, breakLinePosition.Y, breakLinePosition.X, breakLinePosition.Bottom);
			}
			else
			{
				graphicsPath.AddLine(breakLinePosition.Right, breakLinePosition.Y, breakLinePosition.Right, breakLinePosition.Bottom);
			}
			return graphicsPath;
		}

		internal RectangleF GetBreakLinePosition(ChartGraphics graph, AxisScaleSegment nextSegment)
		{
			RectangleF relative = axis.PlotAreaPosition.ToRectangleF();
			double linearPosition = axis.GetLinearPosition(nextSegment.ScaleMinimum);
			double linearPosition2 = axis.GetLinearPosition(ScaleMaximum);
			if (axis.AxisPosition == AxisPosition.Right || axis.AxisPosition == AxisPosition.Left)
			{
				relative.Y = (float)Math.Min(linearPosition, linearPosition2);
				relative.Height = (float)Math.Max(linearPosition, linearPosition2);
			}
			else
			{
				relative.X = (float)Math.Min(linearPosition, linearPosition2);
				relative.Width = (float)Math.Max(linearPosition, linearPosition2);
			}
			relative = Rectangle.Round(graph.GetAbsoluteRectangle(relative));
			if (axis.AxisPosition == AxisPosition.Right || axis.AxisPosition == AxisPosition.Left)
			{
				relative.Height = Math.Abs(relative.Y - relative.Height);
				relative.X -= axis.chartArea.BorderWidth;
				relative.Width += 2 * axis.chartArea.BorderWidth;
			}
			else
			{
				relative.Width = Math.Abs(relative.X - relative.Width);
				relative.Y -= axis.chartArea.BorderWidth;
				relative.Height += 2 * axis.chartArea.BorderWidth;
			}
			return relative;
		}

		internal void GetScalePositionAndSize(double plotAreaSize, out double scalePosition, out double scaleSize)
		{
			scaleSize = (Size - Spacing) * (plotAreaSize / 100.0);
			scalePosition = Position * (plotAreaSize / 100.0);
		}

		internal void SetTempAxisScaleAndInterval()
		{
			if (oldAxisSettings.Count == 0)
			{
				oldAxisSettings.Push(axis.maximum);
				oldAxisSettings.Push(axis.minimum);
				oldAxisSettings.Push(axis.majorGrid.interval);
				oldAxisSettings.Push(axis.majorGrid.intervalType);
				oldAxisSettings.Push(axis.majorGrid.intervalOffset);
				oldAxisSettings.Push(axis.majorGrid.intervalOffsetType);
				oldAxisSettings.Push(axis.majorTickMark.interval);
				oldAxisSettings.Push(axis.majorTickMark.intervalType);
				oldAxisSettings.Push(axis.majorTickMark.intervalOffset);
				oldAxisSettings.Push(axis.majorTickMark.intervalOffsetType);
				oldAxisSettings.Push(axis.LabelStyle.interval);
				oldAxisSettings.Push(axis.LabelStyle.intervalType);
				oldAxisSettings.Push(axis.LabelStyle.intervalOffset);
				oldAxisSettings.Push(axis.LabelStyle.intervalOffsetType);
			}
			axis.maximum = ScaleMaximum;
			axis.minimum = ScaleMinimum;
			axis.majorGrid.interval = Interval;
			axis.majorGrid.intervalType = IntervalType;
			axis.majorGrid.intervalOffset = IntervalOffset;
			axis.majorGrid.intervalOffsetType = IntervalOffsetType;
			axis.majorTickMark.interval = Interval;
			axis.majorTickMark.intervalType = IntervalType;
			axis.majorTickMark.intervalOffset = IntervalOffset;
			axis.majorTickMark.intervalOffsetType = IntervalOffsetType;
			axis.LabelStyle.interval = Interval;
			axis.LabelStyle.intervalType = IntervalType;
			axis.LabelStyle.intervalOffset = IntervalOffset;
			axis.LabelStyle.intervalOffsetType = IntervalOffsetType;
		}

		internal void RestoreAxisScaleAndInterval()
		{
			if (oldAxisSettings.Count > 0)
			{
				axis.LabelStyle.intervalOffsetType = (DateTimeIntervalType)oldAxisSettings.Pop();
				axis.LabelStyle.intervalOffset = (double)oldAxisSettings.Pop();
				axis.LabelStyle.intervalType = (DateTimeIntervalType)oldAxisSettings.Pop();
				axis.LabelStyle.interval = (double)oldAxisSettings.Pop();
				axis.majorTickMark.intervalOffsetType = (DateTimeIntervalType)oldAxisSettings.Pop();
				axis.majorTickMark.intervalOffset = (double)oldAxisSettings.Pop();
				axis.majorTickMark.intervalType = (DateTimeIntervalType)oldAxisSettings.Pop();
				axis.majorTickMark.interval = (double)oldAxisSettings.Pop();
				axis.majorGrid.intervalOffsetType = (DateTimeIntervalType)oldAxisSettings.Pop();
				axis.majorGrid.intervalOffset = (double)oldAxisSettings.Pop();
				axis.majorGrid.intervalType = (DateTimeIntervalType)oldAxisSettings.Pop();
				axis.majorGrid.interval = (double)oldAxisSettings.Pop();
				axis.minimum = (double)oldAxisSettings.Pop();
				axis.maximum = (double)oldAxisSettings.Pop();
			}
		}
	}
}
