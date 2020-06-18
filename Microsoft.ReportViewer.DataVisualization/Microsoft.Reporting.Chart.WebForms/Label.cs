using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Design;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLabel_Label")]
	[DefaultProperty("Enabled")]
	internal class Label : ChartElement
	{
		internal Axis axis;

		private bool enabled = true;

		internal double intervalOffset = double.NaN;

		internal double interval = double.NaN;

		internal DateTimeIntervalType intervalType = DateTimeIntervalType.NotSet;

		internal DateTimeIntervalType intervalOffsetType = DateTimeIntervalType.NotSet;

		internal Font font = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private Color fontColor = Color.Black;

		internal int fontAngle;

		internal bool offsetLabels;

		private bool showEndLabels = true;

		private bool truncatedLabels;

		private string format = "";

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeLabel_IntervalOffset")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(AxisElementIntervalValueConverter))]
		public double IntervalOffset
		{
			get
			{
				if (double.IsNaN(intervalOffset) && axis != null)
				{
					if (axis.IsSerializing())
					{
						return double.NaN;
					}
					return axis.IntervalOffset;
				}
				return intervalOffset;
			}
			set
			{
				intervalOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.NotSet)]
		[SRDescription("DescriptionAttributeLabel_IntervalOffsetType")]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType IntervalOffsetType
		{
			get
			{
				if (intervalOffsetType == DateTimeIntervalType.NotSet && axis != null)
				{
					if (axis.IsSerializing())
					{
						return DateTimeIntervalType.NotSet;
					}
					return axis.IntervalOffsetType;
				}
				return intervalOffsetType;
			}
			set
			{
				intervalOffsetType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeLabel_Interval")]
		[TypeConverter(typeof(AxisElementIntervalValueConverter))]
		public double Interval
		{
			get
			{
				if (double.IsNaN(interval) && axis != null)
				{
					if (axis.IsSerializing())
					{
						return double.NaN;
					}
					return axis.Interval;
				}
				return interval;
			}
			set
			{
				interval = value;
				if (axis != null)
				{
					axis.tempLabelInterval = interval;
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.NotSet)]
		[SRDescription("DescriptionAttributeLabel_IntervalType")]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType IntervalType
		{
			get
			{
				if (intervalType == DateTimeIntervalType.NotSet && axis != null)
				{
					if (axis.IsSerializing())
					{
						return DateTimeIntervalType.NotSet;
					}
					return axis.IntervalType;
				}
				return intervalType;
			}
			set
			{
				intervalType = value;
				if (axis != null)
				{
					axis.tempLabelIntervalType = intervalType;
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeLabel_Font")]
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				if (axis != null && axis.chart != null && !axis.chart.serializing)
				{
					axis.LabelsAutoFit = false;
				}
				font = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLabel_FontColor")]
		[NotifyParentProperty(true)]
		public Color FontColor
		{
			get
			{
				return fontColor;
			}
			set
			{
				fontColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeLabel_FontAngle")]
		[RefreshProperties(RefreshProperties.All)]
		public int FontAngle
		{
			get
			{
				return fontAngle;
			}
			set
			{
				if (value < -90 || value > 90)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisLabelFontAngleInvalid);
				}
				if (OffsetLabels && value != 0 && value != -90 && value != 90)
				{
					OffsetLabels = false;
				}
				if (axis != null && axis.chart != null && !axis.chart.serializing)
				{
					axis.LabelsAutoFit = false;
				}
				fontAngle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeLabel_OffsetLabels")]
		[RefreshProperties(RefreshProperties.All)]
		public bool OffsetLabels
		{
			get
			{
				return offsetLabels;
			}
			set
			{
				if (value && (FontAngle != 0 || FontAngle != -90 || FontAngle != 90))
				{
					FontAngle = 0;
				}
				if (axis != null && axis.chart != null && !axis.chart.serializing)
				{
					axis.LabelsAutoFit = false;
				}
				offsetLabels = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLabel_ShowEndLabels")]
		public bool ShowEndLabels
		{
			get
			{
				return showEndLabels;
			}
			set
			{
				showEndLabels = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeLabel_TruncatedLabels")]
		public bool TruncatedLabels
		{
			get
			{
				return truncatedLabels;
			}
			set
			{
				truncatedLabels = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLabel_Format")]
		public string Format
		{
			get
			{
				return format;
			}
			set
			{
				format = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLabel_Enabled")]
		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				Invalidate();
			}
		}

		public Label()
		{
			axis = null;
		}

		public Label(Axis axis)
		{
			this.axis = axis;
		}

		internal void PaintCircular(ChartGraphics graph)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			if (!axis.LabelStyle.Enabled)
			{
				return;
			}
			CircularAxisLabelsStyle circularAxisLabelsStyle = axis.chartArea.GetCircularAxisLabelsStyle();
			ArrayList circularAxisList = axis.chartArea.GetCircularAxisList();
			int num = 0;
			foreach (CircularChartAreaAxis item in circularAxisList)
			{
				if (item.Title.Length > 0)
				{
					PointF relative = new PointF(axis.chartArea.circularCenter.X, axis.chartArea.PlotAreaPosition.Y);
					relative.Y -= axis.markSize + 1f;
					PointF[] array = new PointF[1]
					{
						graph.GetAbsolutePoint(relative)
					};
					float num2 = item.AxisPosition;
					ICircularChartType circularChartType = axis.chartArea.GetCircularChartType();
					if (circularChartType != null && circularChartType.XAxisCrossingSupported() && !double.IsNaN(axis.chartArea.AxisX.Crossing))
					{
						num2 += (float)axis.chartArea.AxisX.Crossing;
					}
					while (num2 < 0f)
					{
						num2 = 360f + num2;
					}
					Matrix matrix = new Matrix();
					matrix.RotateAt(num2, graph.GetAbsolutePoint(axis.chartArea.circularCenter));
					matrix.TransformPoints(array);
					stringFormat.LineAlignment = StringAlignment.Center;
					stringFormat.Alignment = StringAlignment.Near;
					if (circularAxisLabelsStyle != CircularAxisLabelsStyle.Radial)
					{
						if (num2 < 5f || num2 > 355f)
						{
							stringFormat.Alignment = StringAlignment.Center;
							stringFormat.LineAlignment = StringAlignment.Far;
						}
						if (num2 < 185f && num2 > 175f)
						{
							stringFormat.Alignment = StringAlignment.Center;
							stringFormat.LineAlignment = StringAlignment.Near;
						}
						if (num2 > 185f && num2 < 355f)
						{
							stringFormat.Alignment = StringAlignment.Far;
						}
					}
					else if (num2 > 180f)
					{
						stringFormat.Alignment = StringAlignment.Far;
					}
					float num3 = num2;
					switch (circularAxisLabelsStyle)
					{
					case CircularAxisLabelsStyle.Radial:
						num3 = ((!(num2 > 180f)) ? (num3 - 90f) : (num3 + 90f));
						break;
					case CircularAxisLabelsStyle.Circular:
						stringFormat.Alignment = StringAlignment.Center;
						stringFormat.LineAlignment = StringAlignment.Far;
						break;
					}
					Matrix transform = graph.Transform;
					if (circularAxisLabelsStyle == CircularAxisLabelsStyle.Radial || circularAxisLabelsStyle == CircularAxisLabelsStyle.Circular)
					{
						Matrix matrix2 = transform.Clone();
						matrix2.RotateAt(num3, array[0]);
						graph.Transform = matrix2;
					}
					InitAnimation(graph, circularAxisList.Count, num);
					graph.StartAnimation();
					Color titleColor = fontColor;
					if (!item.TitleColor.IsEmpty)
					{
						titleColor = item.TitleColor;
					}
					graph.DrawString(item.Title.Replace("\\n", "\n"), (axis.autoLabelFont == null) ? font : axis.autoLabelFont, new SolidBrush(titleColor), array[0], stringFormat);
					graph.StopAnimation();
					if (axis.Common.ProcessModeRegions)
					{
						SizeF size = graph.MeasureString(item.Title.Replace("\\n", "\n"), (axis.autoLabelFont == null) ? font : axis.autoLabelFont);
						RectangleF labelPosition = GetLabelPosition(graph, array[0], size, stringFormat);
						PointF[] points = new PointF[4]
						{
							labelPosition.Location,
							new PointF(labelPosition.Right, labelPosition.Y),
							new PointF(labelPosition.Right, labelPosition.Bottom),
							new PointF(labelPosition.X, labelPosition.Bottom)
						};
						GraphicsPath graphicsPath = new GraphicsPath();
						graphicsPath.AddPolygon(points);
						graphicsPath.CloseAllFigures();
						graphicsPath.Transform(graph.Transform);
						try
						{
							axis.Common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: false, graph, ChartElementType.AxisLabels, item.Title);
						}
						catch
						{
						}
					}
					if (circularAxisLabelsStyle == CircularAxisLabelsStyle.Radial || circularAxisLabelsStyle == CircularAxisLabelsStyle.Circular)
					{
						graph.Transform = transform;
					}
				}
				num++;
			}
		}

		internal static RectangleF GetLabelPosition(ChartGraphics graph, PointF position, SizeF size, StringFormat format)
		{
			RectangleF empty = RectangleF.Empty;
			empty.Width = size.Width;
			empty.Height = size.Height;
			if (format.Alignment == StringAlignment.Far)
			{
				empty.X = position.X - size.Width;
			}
			else if (format.Alignment == StringAlignment.Near)
			{
				empty.X = position.X;
			}
			else if (format.Alignment == StringAlignment.Center)
			{
				empty.X = position.X - size.Width / 2f;
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
			return empty;
		}

		internal void Paint(ChartGraphics graph, bool backElements)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			if (!axis.LabelStyle.Enabled || double.IsNaN(axis.GetViewMinimum()) || double.IsNaN(axis.GetViewMaximum()))
			{
				return;
			}
			if (axis.chartArea.Area3DStyle.Enable3D && !axis.chartArea.chartAreaIsCurcular)
			{
				Paint3D(graph, backElements);
				return;
			}
			axis.PlotAreaPosition.ToRectangleF();
			RectangleF rectangleF = axis.chartArea.Position.ToRectangleF();
			float labelSize = axis.labelSize;
			if (axis.AxisPosition == AxisPosition.Left)
			{
				rectangleF.Width = labelSize;
				if (axis.IsMarksNextToAxis())
				{
					rectangleF.X = (float)axis.GetAxisPosition();
				}
				else
				{
					rectangleF.X = axis.PlotAreaPosition.X;
				}
				rectangleF.X -= labelSize + axis.markSize;
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (axis.AxisPosition == AxisPosition.Right)
			{
				rectangleF.Width = labelSize;
				if (axis.IsMarksNextToAxis())
				{
					rectangleF.X = (float)axis.GetAxisPosition();
				}
				else
				{
					rectangleF.X = axis.PlotAreaPosition.Right();
				}
				rectangleF.X += axis.markSize;
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Center;
			}
			else if (axis.AxisPosition == AxisPosition.Top)
			{
				rectangleF.Height = labelSize;
				if (axis.IsMarksNextToAxis())
				{
					rectangleF.Y = (float)axis.GetAxisPosition();
				}
				else
				{
					rectangleF.Y = axis.PlotAreaPosition.Y;
				}
				rectangleF.Y -= labelSize + axis.markSize;
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Far;
			}
			else if (axis.AxisPosition == AxisPosition.Bottom)
			{
				rectangleF.Height = labelSize;
				if (axis.IsMarksNextToAxis())
				{
					rectangleF.Y = (float)axis.GetAxisPosition();
				}
				else
				{
					rectangleF.Y = axis.PlotAreaPosition.Bottom();
				}
				rectangleF.Y += axis.markSize;
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
			}
			RectangleF rectangleF2 = rectangleF;
			if (rectangleF2 != RectangleF.Empty && axis.totlaGroupingLabelsSize > 0f)
			{
				if (axis.AxisPosition == AxisPosition.Left)
				{
					rectangleF2.X += axis.totlaGroupingLabelsSize;
					rectangleF2.Width -= axis.totlaGroupingLabelsSize;
				}
				else if (axis.AxisPosition == AxisPosition.Right)
				{
					rectangleF2.Width -= axis.totlaGroupingLabelsSize;
				}
				else if (axis.AxisPosition == AxisPosition.Top)
				{
					rectangleF2.Y += axis.totlaGroupingLabelsSize;
					rectangleF2.Height -= axis.totlaGroupingLabelsSize;
				}
				else if (axis.AxisPosition == AxisPosition.Bottom)
				{
					rectangleF2.Height -= axis.totlaGroupingLabelsSize;
				}
			}
			bool flag = false;
			bool flag2 = true;
			bool flag3 = true;
			int num = 0;
			foreach (CustomLabel customLabel in axis.CustomLabels)
			{
				bool truncatedLeft = false;
				bool truncatedRight = false;
				double axisValue = customLabel.From;
				double axisValue2 = customLabel.To;
				bool flag4 = false;
				double num2 = double.NaN;
				double num3 = double.NaN;
				if (customLabel.RowIndex == 0)
				{
					double num4 = (customLabel.From + customLabel.To) / 2.0;
					decimal d = (decimal)axis.GetViewMinimum();
					decimal d2 = (decimal)axis.GetViewMaximum();
					if (flag)
					{
						if ((flag2 && (decimal)customLabel.From < (decimal)axis.Minimum) || (flag3 && (decimal)customLabel.To > (decimal)axis.Maximum) || (decimal)customLabel.To < d || (decimal)customLabel.From > d2)
						{
							continue;
						}
						if ((axis.autoLabelOffset == -1) ? OffsetLabels : (axis.autoLabelOffset == 1))
						{
							num = 0;
							Series series = null;
							if (axis.axisType == AxisName.X || axis.axisType == AxisName.X2)
							{
								ArrayList xAxesSeries = axis.chartArea.GetXAxesSeries((axis.axisType != 0) ? AxisType.Secondary : AxisType.Primary, axis.SubAxisName);
								if (xAxesSeries.Count > 0)
								{
									series = axis.Common.DataManager.Series[xAxesSeries[0]];
									if (series != null && !series.XValueIndexed)
									{
										series = null;
									}
								}
							}
							double num5 = axis.Minimum;
							while (num5 < axis.Maximum && !(num5 >= num4))
							{
								num5 += GetIntervalSize(num5, axis.LabelStyle.Interval, axis.LabelStyle.IntervalType, series, 0.0, DateTimeIntervalType.Number, forceIntIndex: true);
								num++;
							}
						}
					}
					else if ((decimal)num4 < d || (decimal)num4 > d2)
					{
						continue;
					}
					if (axis.ScaleSegments.Count > 0)
					{
						AxisScaleSegment segment = axis.ScaleSegments.FindScaleSegmentForAxisValue(num4);
						axis.ScaleSegments.AllowOutOfScaleValues = true;
						axis.ScaleSegments.EnforceSegment(segment);
					}
					if ((decimal)customLabel.From < d && (decimal)customLabel.To > d2)
					{
						flag4 = true;
						num2 = axis.GetLinearPosition(num4) - 50.0;
						num3 = num2 + 100.0;
					}
				}
				else
				{
					if (customLabel.To <= axis.GetViewMinimum() || customLabel.From >= axis.GetViewMaximum())
					{
						continue;
					}
					if (!flag && axis.View.IsZoomed)
					{
						if (customLabel.From < axis.GetViewMinimum())
						{
							truncatedLeft = true;
							axisValue = axis.GetViewMinimum();
						}
						if (customLabel.To > axis.GetViewMaximum())
						{
							truncatedRight = true;
							axisValue2 = axis.GetViewMaximum();
						}
					}
				}
				RectangleF position = rectangleF;
				if (customLabel.RowIndex == 0)
				{
					if (axis.AxisPosition == AxisPosition.Left)
					{
						position.X = rectangleF.Right - axis.unRotatedLabelSize;
						position.Width = axis.unRotatedLabelSize;
						if ((axis.autoLabelOffset == -1) ? OffsetLabels : (axis.autoLabelOffset == 1))
						{
							position.Width /= 2f;
							if ((float)(num % 2) != 0f)
							{
								position.X += position.Width;
							}
						}
					}
					else if (axis.AxisPosition == AxisPosition.Right)
					{
						position.Width = axis.unRotatedLabelSize;
						if ((axis.autoLabelOffset == -1) ? OffsetLabels : (axis.autoLabelOffset == 1))
						{
							position.Width /= 2f;
							if ((float)(num % 2) != 0f)
							{
								position.X += position.Width;
							}
						}
					}
					else if (axis.AxisPosition == AxisPosition.Top)
					{
						position.Y = rectangleF.Bottom - axis.unRotatedLabelSize;
						position.Height = axis.unRotatedLabelSize;
						if ((axis.autoLabelOffset == -1) ? OffsetLabels : (axis.autoLabelOffset == 1))
						{
							position.Height /= 2f;
							if ((float)(num % 2) != 0f)
							{
								position.Y += position.Height;
							}
						}
					}
					else if (axis.AxisPosition == AxisPosition.Bottom)
					{
						position.Height = axis.unRotatedLabelSize;
						if ((axis.autoLabelOffset == -1) ? OffsetLabels : (axis.autoLabelOffset == 1))
						{
							position.Height /= 2f;
							if ((float)(num % 2) != 0f)
							{
								position.Y += position.Height;
							}
						}
					}
					num++;
				}
				else
				{
					if (customLabel.RowIndex <= 0)
					{
						throw new InvalidOperationException(SR.ExceptionAxisLabelIndexIsNegative);
					}
					if (axis.AxisPosition == AxisPosition.Left)
					{
						position.X += axis.totlaGroupingLabelsSizeAdjustment;
						for (int num6 = axis.groupingLabelSizes.Length; num6 > customLabel.RowIndex; num6--)
						{
							position.X += axis.groupingLabelSizes[num6 - 1];
						}
						position.Width = axis.groupingLabelSizes[customLabel.RowIndex - 1];
					}
					else if (axis.AxisPosition == AxisPosition.Right)
					{
						position.X = position.Right - axis.totlaGroupingLabelsSize - axis.totlaGroupingLabelsSizeAdjustment;
						for (int i = 1; i < customLabel.RowIndex; i++)
						{
							position.X += axis.groupingLabelSizes[i - 1];
						}
						position.Width = axis.groupingLabelSizes[customLabel.RowIndex - 1];
					}
					else if (axis.AxisPosition == AxisPosition.Top)
					{
						position.Y += axis.totlaGroupingLabelsSizeAdjustment;
						for (int num7 = axis.groupingLabelSizes.Length; num7 > customLabel.RowIndex; num7--)
						{
							position.Y += axis.groupingLabelSizes[num7 - 1];
						}
						position.Height = axis.groupingLabelSizes[customLabel.RowIndex - 1];
					}
					if (axis.AxisPosition == AxisPosition.Bottom)
					{
						position.Y = position.Bottom - axis.totlaGroupingLabelsSize - axis.totlaGroupingLabelsSizeAdjustment;
						for (int j = 1; j < customLabel.RowIndex; j++)
						{
							position.Y += axis.groupingLabelSizes[j - 1];
						}
						position.Height = axis.groupingLabelSizes[customLabel.RowIndex - 1];
					}
				}
				double val = axis.GetLinearPosition(axisValue);
				double val2 = axis.GetLinearPosition(axisValue2);
				if (flag4)
				{
					flag4 = false;
					val = num2;
					val2 = num3;
				}
				if (axis.AxisPosition == AxisPosition.Top || axis.AxisPosition == AxisPosition.Bottom)
				{
					position.X = (float)Math.Min(val, val2);
					position.Width = (float)Math.Max(val, val2) - position.X;
					if (customLabel.RowIndex == 0 && ((axis.autoLabelOffset == -1) ? OffsetLabels : (axis.autoLabelOffset == 1)))
					{
						position.X -= position.Width / 2f;
						position.Width *= 2f;
					}
				}
				else
				{
					position.Y = (float)Math.Min(val, val2);
					position.Height = (float)Math.Max(val, val2) - position.Y;
					if (customLabel.RowIndex == 0 && ((axis.autoLabelOffset == -1) ? OffsetLabels : (axis.autoLabelOffset == 1)))
					{
						position.Y -= position.Height / 2f;
						position.Height *= 2f;
					}
				}
				InitAnimation(graph, axis.CustomLabels.Count, num);
				graph.StartAnimation();
				graph.DrawLabelStringRel(axis, customLabel.RowIndex, customLabel.LabelMark, customLabel.MarkColor, customLabel.Text, customLabel.Image, customLabel.ImageTransparentColor, (axis.autoLabelFont == null) ? font : axis.autoLabelFont, new SolidBrush(customLabel.TextColor.IsEmpty ? fontColor : customLabel.TextColor), position, stringFormat, (axis.autoLabelAngle < -90) ? fontAngle : axis.autoLabelAngle, (!TruncatedLabels || customLabel.RowIndex > 0) ? RectangleF.Empty : rectangleF2, customLabel, truncatedLeft, truncatedRight);
				graph.StopAnimation();
				axis.ScaleSegments.EnforceSegment(null);
				axis.ScaleSegments.AllowOutOfScaleValues = false;
			}
		}

		private void InitAnimation(ChartGraphics graph, int numberOfElements, int index)
		{
		}

		private RectangleF GetAllLabelsRect(ChartArea area, AxisPosition position, ref StringFormat stringFormat)
		{
			Axis axis = null;
			Axis[] axes = area.Axes;
			foreach (Axis axis2 in axes)
			{
				if (axis2.AxisPosition == position)
				{
					axis = axis2;
					break;
				}
			}
			if (axis == null)
			{
				return RectangleF.Empty;
			}
			RectangleF result = area.Position.ToRectangleF();
			switch (position)
			{
			case AxisPosition.Left:
				result.Width = axis.labelSize;
				if (axis.IsMarksNextToAxis())
				{
					result.X = (float)axis.GetAxisPosition();
					result.Width = Math.Max(result.Width, result.X - axis.PlotAreaPosition.X);
				}
				else
				{
					result.X = axis.PlotAreaPosition.X;
				}
				result.X -= result.Width;
				if (area.IsSideSceneWallOnLeft() || area.Area3DStyle.WallWidth == 0)
				{
					result.X -= axis.markSize;
				}
				stringFormat.Alignment = StringAlignment.Far;
				stringFormat.LineAlignment = StringAlignment.Center;
				break;
			case AxisPosition.Right:
				result.Width = axis.labelSize;
				if (axis.IsMarksNextToAxis())
				{
					result.X = (float)axis.GetAxisPosition();
					result.Width = Math.Max(result.Width, axis.PlotAreaPosition.Right() - result.X);
				}
				else
				{
					result.X = axis.PlotAreaPosition.Right();
				}
				if (!area.IsSideSceneWallOnLeft() || area.Area3DStyle.WallWidth == 0)
				{
					result.X += axis.markSize;
				}
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Center;
				break;
			case AxisPosition.Top:
				result.Height = axis.labelSize;
				if (axis.IsMarksNextToAxis())
				{
					result.Y = (float)axis.GetAxisPosition();
					result.Height = Math.Max(result.Height, result.Y - axis.PlotAreaPosition.Y);
				}
				else
				{
					result.Y = axis.PlotAreaPosition.Y;
				}
				result.Y -= result.Height;
				if (area.Area3DStyle.WallWidth == 0)
				{
					result.Y -= axis.markSize;
				}
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Far;
				break;
			case AxisPosition.Bottom:
				result.Height = axis.labelSize;
				if (axis.IsMarksNextToAxis())
				{
					result.Y = (float)axis.GetAxisPosition();
					result.Height = Math.Max(result.Height, axis.PlotAreaPosition.Bottom() - result.Y);
				}
				else
				{
					result.Y = axis.PlotAreaPosition.Bottom();
				}
				result.Y += axis.markSize;
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Near;
				break;
			}
			return result;
		}

		private AxisPosition GetLabelsPosition(ChartArea area, Axis axis)
		{
			double axisProjectionAngle = axis.GetAxisProjectionAngle();
			if (axis.AxisPosition == AxisPosition.Bottom)
			{
				if (axisProjectionAngle <= -25.0)
				{
					return AxisPosition.Right;
				}
				if (axisProjectionAngle >= 25.0)
				{
					return AxisPosition.Left;
				}
			}
			else if (axis.AxisPosition == AxisPosition.Top)
			{
				if (axisProjectionAngle <= -25.0)
				{
					return AxisPosition.Left;
				}
				if (axisProjectionAngle >= 25.0)
				{
					return AxisPosition.Right;
				}
			}
			return axis.AxisPosition;
		}

		internal void Paint3D(ChartGraphics graph, bool backElements)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			this.axis.PlotAreaPosition.ToRectangleF();
			SizeF relativeSize = graph.GetRelativeSize(new SizeF(1f, 1f));
			AxisPosition labelsPosition = GetLabelsPosition(this.axis.chartArea, this.axis);
			bool axisOnEdge;
			float num = this.axis.GetMarksZPosition(out axisOnEdge);
			bool flag = false;
			if (this.axis.AxisPosition == AxisPosition.Top && !this.axis.chartArea.ShouldDrawOnSurface(SurfaceNames.Top, backElements, onEdge: false))
			{
				flag = true;
			}
			if (this.axis.AxisPosition == AxisPosition.Left && !this.axis.chartArea.ShouldDrawOnSurface(SurfaceNames.Left, backElements, onEdge: false))
			{
				flag = true;
			}
			if (this.axis.AxisPosition == AxisPosition.Right && !this.axis.chartArea.ShouldDrawOnSurface(SurfaceNames.Right, backElements, onEdge: false))
			{
				flag = true;
			}
			if (flag && this.axis.chartArea.Area3DStyle.WallWidth > 0)
			{
				if (this.axis.MajorTickMark.Style == TickMarkStyle.Inside)
				{
					num -= this.axis.chartArea.areaSceneWallWidth.Width;
				}
				else if (this.axis.MajorTickMark.Style == TickMarkStyle.Outside)
				{
					num -= this.axis.MajorTickMark.Size + this.axis.chartArea.areaSceneWallWidth.Width;
				}
				else if (this.axis.MajorTickMark.Style == TickMarkStyle.Cross)
				{
					num -= this.axis.MajorTickMark.Size / 2f + this.axis.chartArea.areaSceneWallWidth.Width;
				}
			}
			bool flag2 = this.axis.IsMarksNextToAxis() && !axisOnEdge;
			if (backElements == flag2)
			{
				return;
			}
			RectangleF allLabelsRect = GetAllLabelsRect(this.axis.chartArea, this.axis.AxisPosition, ref stringFormat);
			RectangleF rectangleF = allLabelsRect;
			if (rectangleF != RectangleF.Empty && this.axis.totlaGroupingLabelsSize > 0f)
			{
				if (this.axis.AxisPosition == AxisPosition.Left)
				{
					rectangleF.X += this.axis.totlaGroupingLabelsSize;
					rectangleF.Width -= this.axis.totlaGroupingLabelsSize;
				}
				else if (this.axis.AxisPosition == AxisPosition.Right)
				{
					rectangleF.Width -= this.axis.totlaGroupingLabelsSize;
				}
				else if (this.axis.AxisPosition == AxisPosition.Top)
				{
					rectangleF.Y += this.axis.totlaGroupingLabelsSize;
					rectangleF.Height -= this.axis.totlaGroupingLabelsSize;
				}
				else if (this.axis.AxisPosition == AxisPosition.Bottom)
				{
					rectangleF.Height -= this.axis.totlaGroupingLabelsSize;
				}
			}
			float num2 = -1f;
			for (int i = 0; i <= this.axis.GetGroupLabelLevelCount(); i++)
			{
				int num3 = 0;
				foreach (CustomLabel customLabel in this.axis.CustomLabels)
				{
					bool truncatedLeft = false;
					bool truncatedRight = false;
					double axisValue = customLabel.From;
					double axisValue2 = customLabel.To;
					if (customLabel.RowIndex != i)
					{
						continue;
					}
					if (customLabel.RowIndex == 0)
					{
						double value = (customLabel.From + customLabel.To) / 2.0;
						if ((decimal)value < (decimal)this.axis.GetViewMinimum() || (decimal)value > (decimal)this.axis.GetViewMaximum())
						{
							continue;
						}
					}
					else
					{
						if (customLabel.To <= this.axis.GetViewMinimum() || customLabel.From >= this.axis.GetViewMaximum())
						{
							continue;
						}
						if (this.axis.View.IsZoomed)
						{
							if (customLabel.From < this.axis.GetViewMinimum())
							{
								truncatedLeft = true;
								axisValue = this.axis.GetViewMinimum();
							}
							if (customLabel.To > this.axis.GetViewMaximum())
							{
								truncatedRight = true;
								axisValue2 = this.axis.GetViewMaximum();
							}
						}
					}
					RectangleF position = allLabelsRect;
					if (customLabel.RowIndex == 0)
					{
						if (this.axis.AxisPosition == AxisPosition.Left)
						{
							if (!this.axis.IsMarksNextToAxis())
							{
								position.X = allLabelsRect.Right - this.axis.unRotatedLabelSize;
								position.Width = this.axis.unRotatedLabelSize;
							}
							if ((this.axis.autoLabelOffset == -1) ? OffsetLabels : (this.axis.autoLabelOffset == 1))
							{
								position.Width /= 2f;
								if ((float)(num3 % 2) != 0f)
								{
									position.X += position.Width;
								}
							}
						}
						else if (this.axis.AxisPosition == AxisPosition.Right)
						{
							if (!this.axis.IsMarksNextToAxis())
							{
								position.Width = this.axis.unRotatedLabelSize;
							}
							if ((this.axis.autoLabelOffset == -1) ? OffsetLabels : (this.axis.autoLabelOffset == 1))
							{
								position.Width /= 2f;
								if ((float)(num3 % 2) != 0f)
								{
									position.X += position.Width;
								}
							}
						}
						else if (this.axis.AxisPosition == AxisPosition.Top)
						{
							if (!this.axis.IsMarksNextToAxis())
							{
								position.Y = allLabelsRect.Bottom - this.axis.unRotatedLabelSize;
								position.Height = this.axis.unRotatedLabelSize;
							}
							if ((this.axis.autoLabelOffset == -1) ? OffsetLabels : (this.axis.autoLabelOffset == 1))
							{
								position.Height /= 2f;
								if ((float)(num3 % 2) != 0f)
								{
									position.Y += position.Height;
								}
							}
						}
						else if (this.axis.AxisPosition == AxisPosition.Bottom)
						{
							if (!this.axis.IsMarksNextToAxis())
							{
								position.Height = this.axis.unRotatedLabelSize;
							}
							if ((this.axis.autoLabelOffset == -1) ? OffsetLabels : (this.axis.autoLabelOffset == 1))
							{
								position.Height /= 2f;
								if ((float)(num3 % 2) != 0f)
								{
									position.Y += position.Height;
								}
							}
						}
						num3++;
					}
					else
					{
						if (customLabel.RowIndex <= 0)
						{
							throw new InvalidOperationException(SR.ExceptionAxisLabelRowIndexMustBe1Or2);
						}
						if (labelsPosition != this.axis.AxisPosition)
						{
							continue;
						}
						if (this.axis.AxisPosition == AxisPosition.Left)
						{
							position.X += this.axis.totlaGroupingLabelsSizeAdjustment;
							for (int num4 = this.axis.groupingLabelSizes.Length; num4 > customLabel.RowIndex; num4--)
							{
								position.X += this.axis.groupingLabelSizes[num4 - 1];
							}
							position.Width = this.axis.groupingLabelSizes[customLabel.RowIndex - 1];
						}
						else if (this.axis.AxisPosition == AxisPosition.Right)
						{
							position.X = position.Right - this.axis.totlaGroupingLabelsSize - this.axis.totlaGroupingLabelsSizeAdjustment;
							for (int j = 1; j < customLabel.RowIndex; j++)
							{
								position.X += this.axis.groupingLabelSizes[j - 1];
							}
							position.Width = this.axis.groupingLabelSizes[customLabel.RowIndex - 1];
						}
						else if (this.axis.AxisPosition == AxisPosition.Top)
						{
							position.Y += this.axis.totlaGroupingLabelsSizeAdjustment;
							for (int num5 = this.axis.groupingLabelSizes.Length; num5 > customLabel.RowIndex; num5--)
							{
								position.Y += this.axis.groupingLabelSizes[num5 - 1];
							}
							position.Height = this.axis.groupingLabelSizes[customLabel.RowIndex - 1];
						}
						if (this.axis.AxisPosition == AxisPosition.Bottom)
						{
							position.Y = position.Bottom - this.axis.totlaGroupingLabelsSize - this.axis.totlaGroupingLabelsSizeAdjustment;
							for (int k = 1; k < customLabel.RowIndex; k++)
							{
								position.Y += this.axis.groupingLabelSizes[k - 1];
							}
							position.Height = this.axis.groupingLabelSizes[customLabel.RowIndex - 1];
						}
					}
					double linearPosition = this.axis.GetLinearPosition(axisValue);
					double linearPosition2 = this.axis.GetLinearPosition(axisValue2);
					if (this.axis.AxisPosition == AxisPosition.Top || this.axis.AxisPosition == AxisPosition.Bottom)
					{
						position.X = (float)Math.Min(linearPosition, linearPosition2);
						position.Width = (float)Math.Max(linearPosition, linearPosition2) - position.X;
						if (position.Width < relativeSize.Width)
						{
							position.Width = relativeSize.Width;
						}
						if (customLabel.RowIndex == 0 && ((this.axis.autoLabelOffset == -1) ? OffsetLabels : (this.axis.autoLabelOffset == 1)))
						{
							position.X -= position.Width / 2f;
							position.Width *= 2f;
						}
					}
					else
					{
						position.Y = (float)Math.Min(linearPosition, linearPosition2);
						position.Height = (float)Math.Max(linearPosition, linearPosition2) - position.Y;
						if (position.Height < relativeSize.Height)
						{
							position.Height = relativeSize.Height;
						}
						if (customLabel.RowIndex == 0 && ((this.axis.autoLabelOffset == -1) ? OffsetLabels : (this.axis.autoLabelOffset == 1)))
						{
							position.Y -= position.Height / 2f;
							position.Height *= 2f;
						}
					}
					RectangleF rectangleF2 = new RectangleF(position.Location, position.Size);
					Point3D[] array = new Point3D[3];
					if (this.axis.AxisPosition == AxisPosition.Left)
					{
						array[0] = new Point3D(position.Right, position.Y, num);
						array[1] = new Point3D(position.Right, position.Y + position.Height / 2f, num);
						array[2] = new Point3D(position.Right, position.Bottom, num);
						this.axis.chartArea.matrix3D.TransformPoints(array);
						position.Y = array[0].Y;
						position.Height = array[2].Y - position.Y;
						position.Width = array[1].X - position.X;
					}
					else if (this.axis.AxisPosition == AxisPosition.Right)
					{
						array[0] = new Point3D(position.X, position.Y, num);
						array[1] = new Point3D(position.X, position.Y + position.Height / 2f, num);
						array[2] = new Point3D(position.X, position.Bottom, num);
						this.axis.chartArea.matrix3D.TransformPoints(array);
						position.Y = array[0].Y;
						position.Height = array[2].Y - position.Y;
						position.Width = position.Right - array[1].X;
						position.X = array[1].X;
					}
					else if (this.axis.AxisPosition == AxisPosition.Top)
					{
						array[0] = new Point3D(position.X, position.Bottom, num);
						array[1] = new Point3D(position.X + position.Width / 2f, position.Bottom, num);
						array[2] = new Point3D(position.Right, position.Bottom, num);
						this.axis.chartArea.matrix3D.TransformPoints(array);
						switch (labelsPosition)
						{
						case AxisPosition.Top:
							position.X = array[0].X;
							position.Width = array[2].X - position.X;
							position.Height = array[1].Y - position.Y;
							break;
						case AxisPosition.Right:
						{
							RectangleF allLabelsRect3 = GetAllLabelsRect(this.axis.chartArea, labelsPosition, ref stringFormat);
							position.Y = array[0].Y;
							position.Height = array[2].Y - position.Y;
							position.X = array[1].X;
							position.Width = allLabelsRect3.Right - position.X;
							break;
						}
						case AxisPosition.Left:
						{
							RectangleF allLabelsRect2 = GetAllLabelsRect(this.axis.chartArea, labelsPosition, ref stringFormat);
							position.Y = array[2].Y;
							position.Height = array[0].Y - position.Y;
							position.X = allLabelsRect2.X;
							position.Width = array[1].X - allLabelsRect2.X;
							break;
						}
						}
					}
					else if (this.axis.AxisPosition == AxisPosition.Bottom)
					{
						array[0] = new Point3D(position.X, position.Y, num);
						array[1] = new Point3D(position.X + position.Width / 2f, position.Y, num);
						array[2] = new Point3D(position.Right, position.Y, num);
						this.axis.chartArea.matrix3D.TransformPoints(array);
						switch (labelsPosition)
						{
						case AxisPosition.Bottom:
							position.X = array[0].X;
							position.Width = array[2].X - position.X;
							position.Height = position.Bottom - array[1].Y;
							position.Y = array[1].Y;
							break;
						case AxisPosition.Right:
						{
							RectangleF allLabelsRect5 = GetAllLabelsRect(this.axis.chartArea, labelsPosition, ref stringFormat);
							position.Y = array[2].Y;
							position.Height = array[0].Y - position.Y;
							position.X = array[1].X;
							position.Width = allLabelsRect5.Right - position.X;
							if (this.axis.autoLabelAngle == 0)
							{
								position.Y += this.axis.markSize / 4f;
							}
							break;
						}
						case AxisPosition.Left:
						{
							RectangleF allLabelsRect4 = GetAllLabelsRect(this.axis.chartArea, labelsPosition, ref stringFormat);
							position.Y = array[0].Y;
							position.Height = array[2].Y - position.Y;
							position.X = allLabelsRect4.X;
							position.Width = array[1].X - allLabelsRect4.X;
							if (this.axis.autoLabelAngle == 0)
							{
								position.Y += this.axis.markSize / 4f;
							}
							break;
						}
						}
					}
					Axis axis = null;
					Axis[] axes = this.axis.chartArea.Axes;
					foreach (Axis axis2 in axes)
					{
						if (axis2.AxisPosition == labelsPosition)
						{
							axis = axis2;
							break;
						}
					}
					int num6 = (this.axis.autoLabelAngle < -90) ? fontAngle : this.axis.autoLabelAngle;
					if (labelsPosition != this.axis.AxisPosition)
					{
						if ((this.axis.AxisPosition == AxisPosition.Top || this.axis.AxisPosition == AxisPosition.Bottom) && (num6 == 90 || num6 == -90))
						{
							num6 = 0;
						}
						else if (this.axis.AxisPosition == AxisPosition.Bottom)
						{
							if (labelsPosition == AxisPosition.Left && num6 > 0)
							{
								num6 = -num6;
							}
							else if (labelsPosition == AxisPosition.Right && num6 < 0)
							{
								num6 = -num6;
							}
						}
						else if (this.axis.AxisPosition == AxisPosition.Top)
						{
							if (labelsPosition == AxisPosition.Left && num6 < 0)
							{
								num6 = -num6;
							}
							else if (labelsPosition == AxisPosition.Right && num6 > 0)
							{
								num6 = -num6;
							}
						}
					}
					StringFormat stringFormat2 = null;
					if (customLabel.RowIndex == 0 && num6 == 0 && this.axis.groupingLabelSizes != null && this.axis.groupingLabelSizes.Length != 0 && this.axis.AxisPosition == AxisPosition.Bottom && labelsPosition == AxisPosition.Bottom && !((this.axis.autoLabelOffset == -1) ? OffsetLabels : (this.axis.autoLabelOffset == 1)))
					{
						if (num2 == -1f)
						{
							Point3D[] array2 = new Point3D[1]
							{
								new Point3D(rectangleF2.X, rectangleF2.Bottom - this.axis.totlaGroupingLabelsSize - this.axis.totlaGroupingLabelsSizeAdjustment, num)
							};
							this.axis.chartArea.matrix3D.TransformPoints(array2);
							float num7 = array2[0].Y - position.Y;
							num2 = ((num7 > 0f) ? num7 : position.Height);
						}
						position.Height = num2;
						stringFormat2 = (StringFormat)stringFormat.Clone();
						if ((stringFormat.FormatFlags & StringFormatFlags.LineLimit) == 0)
						{
							stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
						}
					}
					InitAnimation(graph, this.axis.CustomLabels.Count, num3);
					graph.StartAnimation();
					graph.DrawLabelStringRel(axis, customLabel.RowIndex, customLabel.LabelMark, customLabel.MarkColor, customLabel.Text, customLabel.Image, customLabel.ImageTransparentColor, (this.axis.autoLabelFont == null) ? font : this.axis.autoLabelFont, new SolidBrush(customLabel.TextColor.IsEmpty ? fontColor : customLabel.TextColor), position, stringFormat, num6, (!TruncatedLabels || customLabel.Row > LabelRow.First) ? RectangleF.Empty : rectangleF, customLabel, truncatedLeft, truncatedRight);
					graph.StopAnimation();
					if (stringFormat2 != null)
					{
						stringFormat?.Dispose();
						stringFormat = stringFormat2;
						stringFormat2 = null;
					}
				}
			}
		}

		internal Axis GetAxis()
		{
			return axis;
		}

		internal void Invalidate()
		{
		}
	}
}
