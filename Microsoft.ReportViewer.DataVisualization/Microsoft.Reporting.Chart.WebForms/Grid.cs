using Microsoft.Reporting.Chart.WebForms.ChartTypes;
using Microsoft.Reporting.Chart.WebForms.Design;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[DefaultProperty("Enabled")]
	[SRDescription("DescriptionAttributeGrid_Grid")]
	internal class Grid
	{
		internal Axis axis;

		internal bool intervalOffsetChanged;

		internal bool intervalChanged;

		internal bool intervalTypeChanged;

		internal bool intervalOffsetTypeChanged;

		internal bool enabledChanged;

		internal double intervalOffset;

		internal double interval;

		internal DateTimeIntervalType intervalType;

		internal DateTimeIntervalType intervalOffsetType;

		internal Color borderColor = Color.Black;

		internal int borderWidth = 1;

		internal ChartDashStyle borderStyle = ChartDashStyle.Solid;

		internal bool enabled = true;

		internal bool majorGridTick;

		protected const double NumberOfIntervals = 5.0;

		protected const double NumberOfDateTimeIntervals = 4.0;

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeIntervalOffset3")]
		[TypeConverter(typeof(AxisElementIntervalValueConverter))]
		public double IntervalOffset
		{
			get
			{
				if (majorGridTick && double.IsNaN(intervalOffset) && axis != null)
				{
					if (axis.IsSerializing())
					{
						return double.NaN;
					}
					return axis.IntervalOffset;
				}
				if (!majorGridTick && intervalOffset == 0.0 && axis.IsSerializing())
				{
					return double.NaN;
				}
				return intervalOffset;
			}
			set
			{
				intervalOffset = value;
				intervalOffsetChanged = true;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.NotSet)]
		[SRDescription("DescriptionAttributeIntervalOffsetType6")]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType IntervalOffsetType
		{
			get
			{
				if (majorGridTick && intervalOffsetType == DateTimeIntervalType.NotSet && axis != null)
				{
					if (axis.IsSerializing())
					{
						return DateTimeIntervalType.NotSet;
					}
					return axis.IntervalOffsetType;
				}
				if (!majorGridTick && intervalOffsetType == DateTimeIntervalType.Auto && axis.IsSerializing())
				{
					return DateTimeIntervalType.NotSet;
				}
				return intervalOffsetType;
			}
			set
			{
				intervalOffsetType = value;
				intervalOffsetTypeChanged = true;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeInterval6")]
		[TypeConverter(typeof(AxisElementIntervalValueConverter))]
		[RefreshProperties(RefreshProperties.All)]
		public double Interval
		{
			get
			{
				if (majorGridTick && double.IsNaN(interval) && axis != null)
				{
					if (axis.IsSerializing())
					{
						return double.NaN;
					}
					return axis.Interval;
				}
				if (!majorGridTick && interval == 0.0 && axis.IsSerializing())
				{
					return double.NaN;
				}
				return interval;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(SR.ExceptionTickMarksIntervalIsNegative, "value");
				}
				interval = value;
				intervalChanged = true;
				if (!majorGridTick && value != 0.0 && !double.IsNaN(value) && axis != null && axis.chart != null && axis.chart.serializing)
				{
					Enabled = true;
				}
				if (axis != null)
				{
					if (this is TickMark)
					{
						if (majorGridTick)
						{
							axis.tempMajorTickMarkInterval = interval;
						}
						else
						{
							axis.tempMinorTickMarkInterval = interval;
						}
					}
					else if (majorGridTick)
					{
						axis.tempMajorGridInterval = interval;
					}
					else
					{
						axis.tempMinorGridInterval = interval;
					}
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.NotSet)]
		[SRDescription("DescriptionAttributeIntervalType3")]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType IntervalType
		{
			get
			{
				if (majorGridTick && intervalType == DateTimeIntervalType.NotSet && axis != null)
				{
					if (axis.IsSerializing())
					{
						return DateTimeIntervalType.NotSet;
					}
					return axis.IntervalType;
				}
				if (!majorGridTick && intervalType == DateTimeIntervalType.Auto && axis.IsSerializing())
				{
					return DateTimeIntervalType.NotSet;
				}
				return intervalType;
			}
			set
			{
				intervalType = value;
				intervalTypeChanged = true;
				if (axis != null)
				{
					if (this is TickMark)
					{
						axis.tempTickMarkIntervalType = intervalType;
					}
					else
					{
						axis.tempGridIntervalType = intervalType;
					}
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLineColor5")]
		public Color LineColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLineStyle9")]
		public ChartDashStyle LineStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLineWidth8")]
		public int LineWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				borderWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeEnabled5")]
		public bool Enabled
		{
			get
			{
				if (axis != null && axis.IsSerializing() && !majorGridTick)
				{
					return true;
				}
				return enabled;
			}
			set
			{
				enabled = value;
				enabledChanged = true;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(false)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeDisabled")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool Disabled
		{
			get
			{
				if (axis != null && axis.IsSerializing() && majorGridTick)
				{
					return true;
				}
				return !enabled;
			}
			set
			{
				enabled = !value;
				enabledChanged = true;
				Invalidate();
			}
		}

		public Grid()
		{
		}

		public Grid(Axis axis, bool major)
		{
			Initialize(axis, major);
		}

		internal void Initialize(Axis axis, bool major)
		{
			if (!enabledChanged && this.axis == null && !major)
			{
				enabled = false;
			}
			if (this.axis == null)
			{
				if (interval != 0.0)
				{
					if (this is TickMark)
					{
						if (major)
						{
							axis.tempMajorTickMarkInterval = interval;
						}
						else
						{
							axis.tempMinorTickMarkInterval = interval;
						}
					}
					else if (major)
					{
						axis.tempMajorGridInterval = interval;
					}
					else
					{
						axis.tempMinorGridInterval = interval;
					}
				}
				if (intervalType != 0)
				{
					if (this is TickMark)
					{
						if (major)
						{
							axis.tempTickMarkIntervalType = intervalType;
						}
					}
					else if (major)
					{
						axis.tempGridIntervalType = intervalType;
					}
				}
			}
			this.axis = axis;
			majorGridTick = major;
		}

		internal void Invalidate()
		{
		}

		internal void Paint(ChartGraphics graph)
		{
			if (!enabled)
			{
				return;
			}
			if (axis.IsCustomGridLines())
			{
				PaintCustom(graph);
				return;
			}
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
			double num = interval;
			DateTimeIntervalType dateTimeIntervalType = intervalType;
			double num2 = intervalOffset;
			DateTimeIntervalType dateTimeIntervalType2 = intervalOffsetType;
			if (!majorGridTick && (interval == 0.0 || double.IsNaN(interval)))
			{
				if (axis.majorGrid.IntervalType == DateTimeIntervalType.Auto)
				{
					interval = axis.majorGrid.Interval / 5.0;
				}
				else
				{
					DateTimeIntervalType type = axis.majorGrid.IntervalType;
					interval = axis.CalcInterval(axis.minimum, axis.minimum + (axis.maximum - axis.minimum) / 4.0, date: true, out type, ChartValueTypes.DateTime);
					intervalType = type;
					intervalOffsetType = axis.majorGrid.IntervalOffsetType;
					intervalOffset = axis.majorGrid.IntervalOffset;
				}
			}
			double num3 = axis.GetViewMinimum();
			if (!axis.chartArea.chartAreaIsCurcular || axis.axisType == AxisName.Y || axis.axisType == AxisName.Y2)
			{
				num3 = axis.AlignIntervalStart(num3, Interval, IntervalType, series, majorGridTick);
			}
			DateTimeIntervalType type2 = (IntervalOffsetType == DateTimeIntervalType.Auto) ? IntervalType : IntervalOffsetType;
			if (IntervalOffset != 0.0 && !double.IsNaN(IntervalOffset) && series == null)
			{
				num3 += axis.GetIntervalSize(num3, IntervalOffset, type2, series, 0.0, DateTimeIntervalType.Number, forceIntIndex: true, forceAbsInterval: false);
			}
			if ((axis.GetViewMaximum() - axis.GetViewMinimum()) / axis.GetIntervalSize(num3, Interval, IntervalType, series, 0.0, DateTimeIntervalType.Number, forceIntIndex: true) > 10000.0 || axis.GetViewMaximum() <= axis.GetViewMinimum() || Interval <= 0.0)
			{
				return;
			}
			int index = 0;
			int num4 = 1;
			double num5 = num3;
			decimal d = (decimal)axis.GetViewMaximum();
			while ((decimal)num3 <= d)
			{
				if (majorGridTick || !axis.Logarithmic)
				{
					double num6 = Interval;
					double intervalSize = axis.GetIntervalSize(num3, num6, IntervalType, series, IntervalOffset, type2, forceIntIndex: true);
					if (intervalSize == 0.0)
					{
						throw new InvalidOperationException(SR.ExceptionTickMarksIntervalIsZero);
					}
					if ((decimal)num3 >= (decimal)axis.GetViewMinimum())
					{
						DrawGrid(graph, num3, (int)((axis.GetViewMaximum() - axis.GetViewMinimum()) / intervalSize), index);
					}
					num3 += intervalSize;
				}
				else
				{
					double logMinimum = GetLogMinimum(num3, series);
					if (num5 != logMinimum)
					{
						num5 = logMinimum;
						num4 = 1;
					}
					double num7 = Math.Log(1.0 + interval * (double)num4, axis.logarithmBase);
					num3 = num5;
					num3 += num7;
					num4++;
					if (GetLogMinimum(num3, series) != logMinimum)
					{
						continue;
					}
					if (num7 == 0.0)
					{
						throw new InvalidOperationException(SR.ExceptionTickMarksIntervalIsZero);
					}
					if ((decimal)num3 >= (decimal)axis.GetViewMinimum() && (decimal)num3 <= (decimal)axis.GetViewMaximum())
					{
						DrawGrid(graph, num3, (int)((axis.GetViewMaximum() - axis.GetViewMinimum()) / num7), index);
					}
				}
				if (index++ > 10000)
				{
					break;
				}
			}
			if (!majorGridTick)
			{
				interval = num;
				intervalType = dateTimeIntervalType;
				intervalOffset = num2;
				intervalOffsetType = dateTimeIntervalType2;
			}
		}

		private double GetLogMinimum(double current, Series axisSeries)
		{
			double num = axis.GetViewMinimum();
			DateTimeIntervalType type = (IntervalOffsetType == DateTimeIntervalType.Auto) ? IntervalType : IntervalOffsetType;
			if (IntervalOffset != 0.0 && !double.IsNaN(IntervalOffset) && axisSeries == null)
			{
				num += axis.GetIntervalSize(num, IntervalOffset, type, axisSeries, 0.0, DateTimeIntervalType.Number, forceIntIndex: true, forceAbsInterval: false);
			}
			return num + Math.Floor(current - num);
		}

		private void DrawGrid(ChartGraphics graph, double current, int numberOfElements, int index)
		{
			CommonElements common = axis.Common;
			PointF point = PointF.Empty;
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			RectangleF rectangleF = axis.PlotAreaPosition.ToRectangleF();
			if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
			{
				empty.X = rectangleF.X;
				empty2.X = rectangleF.Right;
				empty.Y = (float)axis.GetLinearPosition(current);
				empty2.Y = empty.Y;
				point = empty;
			}
			if (axis.AxisPosition == AxisPosition.Top || axis.AxisPosition == AxisPosition.Bottom)
			{
				empty.Y = rectangleF.Y;
				empty2.Y = rectangleF.Bottom;
				empty.X = (float)axis.GetLinearPosition(current);
				empty2.X = empty.X;
				point = empty2;
			}
			if (common.ProcessModeRegions)
			{
				if (axis.chartArea.Area3DStyle.Enable3D && !axis.chartArea.chartAreaIsCurcular)
				{
					graph.Draw3DGridLine(axis.chartArea, borderColor, borderWidth, borderStyle, empty, empty2, axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right, common, this);
				}
				else if (!axis.chartArea.chartAreaIsCurcular)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					if (Math.Abs(empty.X - empty2.X) > Math.Abs(empty.Y - empty2.Y))
					{
						graphicsPath.AddLine(empty.X, empty.Y - 1f, empty2.X, empty2.Y - 1f);
						graphicsPath.AddLine(empty2.X, empty2.Y + 1f, empty.X, empty.Y + 1f);
						graphicsPath.CloseAllFigures();
					}
					else
					{
						graphicsPath.AddLine(empty.X - 1f, empty.Y, empty2.X - 1f, empty2.Y);
						graphicsPath.AddLine(empty2.X + 1f, empty2.Y, empty.X + 1f, empty.Y);
						graphicsPath.CloseAllFigures();
					}
					common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: true, graph, ChartElementType.Gridlines, this);
				}
			}
			if (!common.ProcessModePaint)
			{
				return;
			}
			if (axis.chartArea.chartAreaIsCurcular)
			{
				InitAnimation(axis.Common, point, graph, numberOfElements, index);
				graph.StartAnimation();
				if (axis.axisType == AxisName.Y)
				{
					axis.DrawCircularLine(this, graph, borderColor, borderWidth, borderStyle, empty.Y);
				}
				if (axis.axisType == AxisName.X)
				{
					ICircularChartType circularChartType = axis.chartArea.GetCircularChartType();
					if (circularChartType != null && circularChartType.RadialGridLinesSupported())
					{
						axis.DrawRadialLine(this, graph, borderColor, borderWidth, borderStyle, current);
					}
				}
				graph.StopAnimation();
			}
			else if (!axis.chartArea.Area3DStyle.Enable3D || axis.chartArea.chartAreaIsCurcular)
			{
				InitAnimation(axis.Common, point, graph, numberOfElements, index);
				graph.StartAnimation();
				graph.DrawLineRel(borderColor, borderWidth, borderStyle, empty, empty2);
				graph.StopAnimation();
			}
			else
			{
				graph.Draw3DGridLine(axis.chartArea, borderColor, borderWidth, borderStyle, empty, empty2, axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right, axis.Common, this, numberOfElements, index);
			}
		}

		private void InitAnimation(CommonElements common, PointF point, ChartGraphics graph, int numberOfElements, int index)
		{
		}

		internal void PaintCustom(ChartGraphics graph)
		{
			CommonElements common = axis.Common;
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			RectangleF rectangleF = axis.PlotAreaPosition.ToRectangleF();
			foreach (CustomLabel customLabel in axis.CustomLabels)
			{
				if ((customLabel.GridTicks & GridTicks.Gridline) != GridTicks.Gridline)
				{
					continue;
				}
				double num = (customLabel.To + customLabel.From) / 2.0;
				if (!(num >= axis.GetViewMinimum()) || !(num <= axis.GetViewMaximum()))
				{
					continue;
				}
				if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
				{
					empty.X = rectangleF.X;
					empty2.X = rectangleF.Right;
					empty.Y = (float)axis.GetLinearPosition(num);
					empty2.Y = empty.Y;
				}
				if (axis.AxisPosition == AxisPosition.Top || axis.AxisPosition == AxisPosition.Bottom)
				{
					empty.Y = rectangleF.Y;
					empty2.Y = rectangleF.Bottom;
					empty.X = (float)axis.GetLinearPosition(num);
					empty2.X = empty.X;
				}
				if (common.ProcessModeRegions)
				{
					if (!axis.chartArea.Area3DStyle.Enable3D || axis.chartArea.chartAreaIsCurcular)
					{
						GraphicsPath graphicsPath = new GraphicsPath();
						if (Math.Abs(empty.X - empty2.X) > Math.Abs(empty.Y - empty2.Y))
						{
							graphicsPath.AddLine(empty.X, empty.Y - 1f, empty2.X, empty2.Y - 1f);
							graphicsPath.AddLine(empty2.X, empty2.Y + 1f, empty.X, empty.Y + 1f);
							graphicsPath.CloseAllFigures();
						}
						else
						{
							graphicsPath.AddLine(empty.X - 1f, empty.Y, empty2.X - 1f, empty2.Y);
							graphicsPath.AddLine(empty2.X + 1f, empty2.Y, empty.X + 1f, empty.Y);
							graphicsPath.CloseAllFigures();
						}
						common.HotRegionsList.AddHotRegion(graphicsPath, relativePath: true, graph, ChartElementType.Gridlines, this);
					}
					else
					{
						graph.Draw3DGridLine(axis.chartArea, borderColor, borderWidth, borderStyle, empty, empty2, axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right, common, this);
					}
				}
				if (common.ProcessModePaint)
				{
					if (!axis.chartArea.Area3DStyle.Enable3D || axis.chartArea.chartAreaIsCurcular)
					{
						graph.DrawLineRel(borderColor, borderWidth, borderStyle, empty, empty2);
					}
					else
					{
						graph.Draw3DGridLine(axis.chartArea, borderColor, borderWidth, borderStyle, empty, empty2, axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right, axis.Common, this);
					}
				}
			}
		}
	}
}
