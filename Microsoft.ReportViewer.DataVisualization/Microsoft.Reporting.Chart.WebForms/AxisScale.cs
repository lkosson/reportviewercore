using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class AxisScale : ChartElement
	{
		internal double margin = 100.0;

		internal double marginView;

		internal bool offsetTempSet;

		internal double marginTemp;

		private ArrayList stripLineOffsets = new ArrayList();

		private bool logarithmic;

		internal double logarithmBase = 10.0;

		internal bool reverse;

		internal bool startFromZero = true;

		internal TickMark minorTickMark;

		internal TickMark majorTickMark;

		internal Grid minorGrid;

		internal Grid majorGrid;

		internal bool enabled;

		internal bool autoEnabled = true;

		internal Label labelStyle;

		private DateTimeIntervalType intervalType;

		internal double maximum = double.NaN;

		internal double crossing = double.NaN;

		internal double minimum = double.NaN;

		internal double tempMaximum = double.NaN;

		internal double tempMinimum = double.NaN;

		internal double tempCrossing = double.NaN;

		internal CustomLabelsCollection tempLabels;

		internal bool tempAutoMaximum = true;

		internal bool tempAutoMinimum = true;

		internal double tempMajorGridInterval = double.NaN;

		internal double tempMinorGridInterval = double.NaN;

		internal double tempMajorTickMarkInterval = double.NaN;

		internal double tempMinorTickMarkInterval = double.NaN;

		internal double tempLabelInterval = double.NaN;

		internal DateTimeIntervalType tempGridIntervalType = DateTimeIntervalType.NotSet;

		internal DateTimeIntervalType tempTickMarkIntervalType = DateTimeIntervalType.NotSet;

		internal DateTimeIntervalType tempLabelIntervalType = DateTimeIntervalType.NotSet;

		internal bool paintMode;

		internal AxisName axisType;

		internal ChartArea chartArea;

		internal bool autoMaximum = true;

		internal bool autoMinimum = true;

		private AxisPosition axisPosition;

		internal Axis oppositeAxis;

		private AxisDataView view;

		internal AxisScrollBar scrollBar;

		internal bool roundedXValues;

		internal bool logarithmicConvertedToLinear;

		internal double logarithmicMinimum;

		internal double logarithmicMaximum;

		internal double logarithmicCrossing;

		internal double interval3DCorrection = double.NaN;

		internal bool optimizedGetPosition;

		internal double paintViewMax;

		internal double paintViewMin;

		internal double paintRange;

		internal double valueMultiplier;

		internal RectangleF paintAreaPosition = RectangleF.Empty;

		internal double paintAreaPositionBottom;

		internal double paintAreaPositionRight;

		internal double paintChartAreaSize;

		private IntervalAutoMode intervalAutoMode;

		internal bool scaleSegmentsUsed;

		internal int prefferedNumberofIntervals = 5;

		private Stack<double> intervalsStore = new Stack<double>();

		internal AxisScaleBreakStyle axisScaleBreakStyle;

		internal AxisScaleSegmentCollection scaleSegments;

		[Bindable(true)]
		[DefaultValue(AxisPosition.Left)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeReverse")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal virtual AxisPosition AxisPosition
		{
			get
			{
				return axisPosition;
			}
			set
			{
				axisPosition = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeInterval")]
		[DefaultValue(IntervalAutoMode.FixedCount)]
		[SRDescription("DescriptionAttributeIntervalAutoMode")]
		public IntervalAutoMode IntervalAutoMode
		{
			get
			{
				return intervalAutoMode;
			}
			set
			{
				intervalAutoMode = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[DefaultValue(false)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeReverse")]
		public bool Reverse
		{
			get
			{
				return reverse;
			}
			set
			{
				reverse = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[DefaultValue(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeStartFromZero3")]
		public bool StartFromZero
		{
			get
			{
				return startFromZero;
			}
			set
			{
				startFromZero = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[DefaultValue(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMargin")]
		public bool Margin
		{
			get
			{
				if (margin > 0.0)
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					margin = 100.0;
				}
				else
				{
					margin = 0.0;
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeInternalIntervalType")]
		[RefreshProperties(RefreshProperties.All)]
		internal DateTimeIntervalType InternalIntervalType
		{
			get
			{
				return intervalType;
			}
			set
			{
				if (tempMajorGridInterval <= 0.0 || (double.IsNaN(tempMajorGridInterval) && ((Axis)this).Interval <= 0.0))
				{
					majorGrid.intervalType = value;
				}
				if (tempMajorTickMarkInterval <= 0.0 || (double.IsNaN(tempMajorTickMarkInterval) && ((Axis)this).Interval <= 0.0))
				{
					majorTickMark.intervalType = value;
				}
				if (tempLabelInterval <= 0.0 || (double.IsNaN(tempLabelInterval) && ((Axis)this).Interval <= 0.0))
				{
					labelStyle.intervalType = value;
				}
				intervalType = value;
				Invalidate();
			}
		}

		internal double SetInterval
		{
			set
			{
				if (tempMajorGridInterval <= 0.0 || (double.IsNaN(tempMajorGridInterval) && ((Axis)this).Interval <= 0.0))
				{
					majorGrid.interval = value;
				}
				if (tempMajorTickMarkInterval <= 0.0 || (double.IsNaN(tempMajorTickMarkInterval) && ((Axis)this).Interval <= 0.0))
				{
					majorTickMark.interval = value;
				}
				if (tempLabelInterval <= 0.0 || (double.IsNaN(tempLabelInterval) && ((Axis)this).Interval <= 0.0))
				{
					labelStyle.interval = value;
				}
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMaximum")]
		[TypeConverter(typeof(AxisMinMaxAutoValueConverter))]
		public double Maximum
		{
			get
			{
				if (logarithmic && logarithmicConvertedToLinear && !double.IsNaN(maximum))
				{
					return logarithmicMaximum;
				}
				return maximum;
			}
			set
			{
				if (double.IsNaN(value))
				{
					autoMaximum = true;
					maximum = double.NaN;
				}
				else
				{
					maximum = value;
					logarithmicMaximum = value;
					autoMaximum = false;
				}
				((Axis)this).tempMaximum = maximum;
				((Axis)this).tempAutoMaximum = autoMaximum;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMinimum")]
		[TypeConverter(typeof(AxisMinMaxAutoValueConverter))]
		public double Minimum
		{
			get
			{
				if (logarithmic && logarithmicConvertedToLinear && !double.IsNaN(maximum))
				{
					return logarithmicMinimum;
				}
				return minimum;
			}
			set
			{
				if (double.IsNaN(value))
				{
					autoMinimum = true;
					minimum = double.NaN;
				}
				else
				{
					minimum = value;
					autoMinimum = false;
					logarithmicMinimum = value;
				}
				((Axis)this).tempMinimum = minimum;
				((Axis)this).tempAutoMinimum = autoMinimum;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeCrossing")]
		[TypeConverter(typeof(AxisCrossingValueConverter))]
		public virtual double Crossing
		{
			get
			{
				if (paintMode)
				{
					if (logarithmic)
					{
						return Math.Pow(logarithmBase, GetCrossing());
					}
					return GetCrossing();
				}
				return crossing;
			}
			set
			{
				crossing = value;
				((Axis)this).tempCrossing = crossing;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(typeof(AxisEnabled), "Auto")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeEnabled7")]
		public AxisEnabled Enabled
		{
			get
			{
				if (autoEnabled)
				{
					return AxisEnabled.Auto;
				}
				if (enabled)
				{
					return AxisEnabled.True;
				}
				return AxisEnabled.False;
			}
			set
			{
				switch (value)
				{
				case AxisEnabled.Auto:
					autoEnabled = true;
					break;
				case AxisEnabled.True:
					enabled = true;
					autoEnabled = false;
					break;
				default:
					enabled = false;
					autoEnabled = false;
					break;
				}
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[DefaultValue(false)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLogarithmic")]
		public bool Logarithmic
		{
			get
			{
				return logarithmic;
			}
			set
			{
				logarithmic = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeScale")]
		[Bindable(true)]
		[DefaultValue(10.0)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLogarithmBase")]
		public double LogarithmBase
		{
			get
			{
				return logarithmBase;
			}
			set
			{
				if (value < 2.0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleLogarithmBaseInvalid);
				}
				logarithmBase = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeScale")]
		[SRDescription("DescriptionAttributeScaleBreakStyle")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[NotifyParentProperty(true)]
		public virtual AxisScaleBreakStyle ScaleBreakStyle
		{
			get
			{
				return axisScaleBreakStyle;
			}
			set
			{
				axisScaleBreakStyle = value;
				axisScaleBreakStyle.axis = (Axis)this;
			}
		}

		[SRCategory("CategoryAttributeScale")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeAxisScaleSegmentCollection_AxisScaleSegmentCollection")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AxisScaleSegmentCollection ScaleSegments => scaleSegments;

		[SRCategory("CategoryAttributeDataView")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeView")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		internal AxisDataView View
		{
			get
			{
				return view;
			}
			set
			{
				view = value;
				view.axis = (Axis)this;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeDataView")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeScrollBar")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		internal AxisScrollBar ScrollBar
		{
			get
			{
				return scrollBar;
			}
			set
			{
				scrollBar = value;
				scrollBar.axis = (Axis)this;
				Invalidate();
			}
		}

		internal void SetIntervalAndType(double newInterval, DateTimeIntervalType newIntervalType)
		{
			if (tempMajorGridInterval <= 0.0 || (double.IsNaN(tempMajorGridInterval) && ((Axis)this).Interval <= 0.0))
			{
				majorGrid.interval = newInterval;
				majorGrid.intervalType = newIntervalType;
			}
			if (tempMajorTickMarkInterval <= 0.0 || (double.IsNaN(tempMajorTickMarkInterval) && ((Axis)this).Interval <= 0.0))
			{
				majorTickMark.interval = newInterval;
				majorTickMark.intervalType = newIntervalType;
			}
			if (tempLabelInterval <= 0.0 || (double.IsNaN(tempLabelInterval) && ((Axis)this).Interval <= 0.0))
			{
				labelStyle.interval = newInterval;
				labelStyle.intervalType = newIntervalType;
			}
			Invalidate();
		}

		internal double GetViewMinimum()
		{
			return view.GetViewMinimum();
		}

		internal double GetViewMaximum()
		{
			return view.GetViewMaximum();
		}

		public double GetPosition(double axisValue)
		{
			if (logarithmic && axisValue != 0.0)
			{
				axisValue = Math.Log(axisValue, logarithmBase);
			}
			return GetLinearPosition(axisValue);
		}

		public double ValueToPosition(double axisValue)
		{
			return GetPosition(axisValue);
		}

		public double ValueToPixelPosition(double axisValue)
		{
			double num = ValueToPosition(axisValue);
			if (AxisPosition == AxisPosition.Top || AxisPosition == AxisPosition.Bottom)
			{
				return num * (double)((float)(base.Common.ChartPicture.Width - 1) / 100f);
			}
			return num * (double)((float)(base.Common.ChartPicture.Height - 1) / 100f);
		}

		public double PositionToValue(double position)
		{
			return PositionToValue(position, validateInput: true);
		}

		internal double PositionToValue(double position, bool validateInput)
		{
			if (validateInput && (position < 0.0 || position > 100.0))
			{
				throw new ArgumentException(SR.ExceptionAxisScalePositionInvalid, "position");
			}
			if (base.PlotAreaPosition == null)
			{
				throw new InvalidOperationException(SR.ExceptionAxisScalePositionToValueCallFailed);
			}
			position = ((AxisPosition != AxisPosition.Top && AxisPosition != AxisPosition.Bottom) ? ((double)base.PlotAreaPosition.Bottom() - position) : (position - (double)base.PlotAreaPosition.X));
			double num = (AxisPosition != AxisPosition.Top && AxisPosition != AxisPosition.Bottom) ? ((double)base.PlotAreaPosition.Height) : ((double)base.PlotAreaPosition.Width);
			double viewMaximum = GetViewMaximum();
			double viewMinimum = GetViewMinimum();
			double num2 = viewMaximum - viewMinimum;
			double num3 = 0.0;
			if (num2 != 0.0)
			{
				num3 = num2 / num * position;
			}
			if (reverse)
			{
				return viewMaximum - num3;
			}
			return viewMinimum + num3;
		}

		public double PixelPositionToValue(double position)
		{
			double num = position;
			num = ((AxisPosition != AxisPosition.Top && AxisPosition != AxisPosition.Bottom) ? (num * (double)(100f / (float)(base.Common.ChartPicture.Height - 1))) : (num * (double)(100f / (float)(base.Common.ChartPicture.Width - 1))));
			return PositionToValue(num);
		}

		internal double PixelPositionToValue(double position, bool validate)
		{
			double num = position;
			num = ((AxisPosition != AxisPosition.Top && AxisPosition != AxisPosition.Bottom) ? (num * (double)(100f / (float)(base.Common.ChartPicture.Height - 1))) : (num * (double)(100f / (float)(base.Common.ChartPicture.Width - 1))));
			return PositionToValue(num, validate);
		}

		public AxisScale()
		{
			view = new AxisDataView((Axis)this);
			scrollBar = new AxisScrollBar((Axis)this);
		}

		internal void SetAxisPosition()
		{
			if (GetOppositeAxis().reverse)
			{
				if (AxisPosition == AxisPosition.Left)
				{
					AxisPosition = AxisPosition.Right;
				}
				else if (AxisPosition == AxisPosition.Right)
				{
					AxisPosition = AxisPosition.Left;
				}
				else if (AxisPosition == AxisPosition.Top)
				{
					AxisPosition = AxisPosition.Bottom;
				}
				else if (AxisPosition == AxisPosition.Bottom)
				{
					AxisPosition = AxisPosition.Top;
				}
			}
		}

		internal void SetTempAxisOffset()
		{
			if (chartArea.Series.Count == 0)
			{
				return;
			}
			Series firstSeries = chartArea.GetFirstSeries();
			if ((firstSeries.ChartType != SeriesChartType.Column && firstSeries.ChartType != SeriesChartType.StackedColumn && firstSeries.ChartType != SeriesChartType.StackedColumn100 && firstSeries.ChartType != SeriesChartType.Bar && firstSeries.ChartType != SeriesChartType.Gantt && firstSeries.ChartType != SeriesChartType.RangeColumn && firstSeries.ChartType != SeriesChartType.StackedBar && firstSeries.ChartType != SeriesChartType.StackedBar100) || margin == 100.0 || offsetTempSet)
			{
				return;
			}
			marginTemp = margin;
			string text = firstSeries["PointWidth"];
			double num = (text == null) ? 0.8 : CommonElements.ParseDouble(text);
			margin = num / 2.0 * 100.0;
			double num2 = margin / 100.0;
			double num3 = (100.0 - margin) / 100.0;
			if (intervalsStore.Count == 0)
			{
				intervalsStore.Push(labelStyle.intervalOffset);
				intervalsStore.Push(majorGrid.intervalOffset);
				intervalsStore.Push(majorTickMark.intervalOffset);
				intervalsStore.Push(minorGrid.intervalOffset);
				intervalsStore.Push(minorTickMark.intervalOffset);
			}
			labelStyle.intervalOffset = (double.IsNaN(labelStyle.intervalOffset) ? num2 : (labelStyle.intervalOffset + num2));
			majorGrid.intervalOffset = (double.IsNaN(majorGrid.intervalOffset) ? num2 : (majorGrid.intervalOffset + num2));
			majorTickMark.intervalOffset = (double.IsNaN(majorTickMark.intervalOffset) ? num2 : (majorTickMark.intervalOffset + num2));
			minorGrid.intervalOffset = (double.IsNaN(minorGrid.intervalOffset) ? num2 : (minorGrid.intervalOffset + num2));
			minorTickMark.intervalOffset = (double.IsNaN(minorTickMark.intervalOffset) ? num2 : (minorTickMark.intervalOffset + num2));
			foreach (StripLine stripLine in ((Axis)this).StripLines)
			{
				stripLineOffsets.Add(stripLine.IntervalOffset);
				stripLine.IntervalOffset -= num3;
			}
			offsetTempSet = true;
		}

		internal void ResetTempAxisOffset()
		{
			if (!offsetTempSet)
			{
				return;
			}
			minorTickMark.intervalOffset = intervalsStore.Pop();
			minorGrid.intervalOffset = intervalsStore.Pop();
			majorTickMark.intervalOffset = intervalsStore.Pop();
			majorGrid.intervalOffset = intervalsStore.Pop();
			labelStyle.intervalOffset = intervalsStore.Pop();
			int num = 0;
			foreach (StripLine stripLine in ((Axis)this).StripLines)
			{
				if (stripLineOffsets.Count > num)
				{
					stripLine.IntervalOffset = (double)stripLineOffsets[num];
				}
				num++;
			}
			stripLineOffsets.Clear();
			offsetTempSet = false;
			margin = marginTemp;
		}

		internal double RoundedValues(double inter, bool shouldStartFromZero, bool autoMax, bool autoMin, ref double min, ref double max)
		{
			if (axisType == AxisName.X || axisType == AxisName.X2)
			{
				if (margin == 0.0 && !roundedXValues)
				{
					return inter;
				}
			}
			else if (margin == 0.0)
			{
				return inter;
			}
			if (autoMin)
			{
				if (min < 0.0 || (!shouldStartFromZero && !chartArea.stacked))
				{
					min = (Axis.RemoveNoiseFromDoubleMath(Math.Ceiling(min / inter)) - 1.0) * Axis.RemoveNoiseFromDoubleMath(inter);
				}
				else
				{
					min = 0.0;
				}
			}
			if (autoMax)
			{
				if (max <= 0.0 && shouldStartFromZero)
				{
					max = 0.0;
				}
				else
				{
					max = (Axis.RemoveNoiseFromDoubleMath(Math.Floor(max / inter)) + 1.0) * Axis.RemoveNoiseFromDoubleMath(inter);
				}
			}
			return inter;
		}

		internal double CalcInterval(double diff)
		{
			if (diff == 0.0)
			{
				throw new ArgumentOutOfRangeException("diff", SR.ExceptionAxisScaleIntervalIsZero);
			}
			double num = -1.0;
			double num2 = diff;
			while (num2 > 1.0)
			{
				num += 1.0;
				num2 /= 10.0;
				if (num > 1000.0)
				{
					throw new InvalidOperationException(SR.ExceptionAxisScaleMinimumMaximumInvalid);
				}
			}
			num2 = diff;
			if (num2 < 1.0)
			{
				num = 0.0;
			}
			while (num2 < 1.0)
			{
				num -= 1.0;
				num2 *= 10.0;
				if (num < -1000.0)
				{
					throw new InvalidOperationException(SR.ExceptionAxisScaleMinimumMaximumInvalid);
				}
			}
			double x = Logarithmic ? logarithmBase : 10.0;
			double num3 = diff / Math.Pow(x, num);
			num3 = ((num3 < 3.0) ? 2.0 : ((!(num3 < 7.0)) ? 10.0 : 5.0));
			return num3 * Math.Pow(x, num);
		}

		private double CalcInterval(double min, double max)
		{
			return CalcInterval((max - min) / 5.0);
		}

		internal double CalcInterval(double min, double max, bool date, out DateTimeIntervalType type, ChartValueTypes valuesType)
		{
			if (date)
			{
				DateTime value = DateTime.FromOADate(min);
				TimeSpan timeSpan = DateTime.FromOADate(max).Subtract(value);
				double totalMinutes = timeSpan.TotalMinutes;
				if (totalMinutes <= 1.0 && valuesType != ChartValueTypes.Date)
				{
					double totalMilliseconds = timeSpan.TotalMilliseconds;
					if (totalMilliseconds <= 10.0)
					{
						type = DateTimeIntervalType.Milliseconds;
						return 1.0;
					}
					if (totalMilliseconds <= 50.0)
					{
						type = DateTimeIntervalType.Milliseconds;
						return 4.0;
					}
					if (totalMilliseconds <= 200.0)
					{
						type = DateTimeIntervalType.Milliseconds;
						return 20.0;
					}
					if (totalMilliseconds <= 500.0)
					{
						type = DateTimeIntervalType.Milliseconds;
						return 50.0;
					}
					double totalSeconds = timeSpan.TotalSeconds;
					if (totalSeconds <= 7.0)
					{
						type = DateTimeIntervalType.Seconds;
						return 1.0;
					}
					if (totalSeconds <= 15.0)
					{
						type = DateTimeIntervalType.Seconds;
						return 2.0;
					}
					if (totalSeconds <= 30.0)
					{
						type = DateTimeIntervalType.Seconds;
						return 5.0;
					}
					if (totalSeconds <= 60.0)
					{
						type = DateTimeIntervalType.Seconds;
						return 10.0;
					}
				}
				else
				{
					if (totalMinutes <= 2.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Seconds;
						return 20.0;
					}
					if (totalMinutes <= 3.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Seconds;
						return 30.0;
					}
					if (totalMinutes <= 10.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Minutes;
						return 1.0;
					}
					if (totalMinutes <= 20.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Minutes;
						return 2.0;
					}
					if (totalMinutes <= 60.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Minutes;
						return 5.0;
					}
					if (totalMinutes <= 120.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Minutes;
						return 10.0;
					}
					if (totalMinutes <= 180.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Minutes;
						return 30.0;
					}
					if (totalMinutes <= 720.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Hours;
						return 1.0;
					}
					if (totalMinutes <= 1440.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Hours;
						return 4.0;
					}
					if (totalMinutes <= 2880.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Hours;
						return 6.0;
					}
					if (totalMinutes <= 4320.0 && valuesType != ChartValueTypes.Date)
					{
						type = DateTimeIntervalType.Hours;
						return 12.0;
					}
					if (totalMinutes <= 14400.0)
					{
						type = DateTimeIntervalType.Days;
						return 1.0;
					}
					if (totalMinutes <= 28800.0)
					{
						type = DateTimeIntervalType.Days;
						return 2.0;
					}
					if (totalMinutes <= 43200.0)
					{
						type = DateTimeIntervalType.Days;
						return 3.0;
					}
					if (totalMinutes <= 87840.0)
					{
						type = DateTimeIntervalType.Weeks;
						return 1.0;
					}
					if (totalMinutes <= 219600.0)
					{
						type = DateTimeIntervalType.Weeks;
						return 2.0;
					}
					if (totalMinutes <= 527040.0)
					{
						type = DateTimeIntervalType.Months;
						return 1.0;
					}
					if (totalMinutes <= 1054080.0)
					{
						type = DateTimeIntervalType.Months;
						return 3.0;
					}
					if (totalMinutes <= 2108160.0)
					{
						type = DateTimeIntervalType.Months;
						return 6.0;
					}
					if (totalMinutes >= 2108160.0)
					{
						type = DateTimeIntervalType.Years;
						return CalcYearInterval(totalMinutes / 60.0 / 24.0 / 365.0);
					}
				}
			}
			type = DateTimeIntervalType.Number;
			return CalcInterval(min, max);
		}

		private double CalcYearInterval(double years)
		{
			if (years <= 1.0)
			{
				throw new ArgumentOutOfRangeException("years", SR.ExceptionAxisScaleIntervalIsLessThen1Year);
			}
			if (years < 5.0)
			{
				return 1.0;
			}
			if (years < 10.0)
			{
				return 2.0;
			}
			return Math.Floor(years / 5.0);
		}

		private int GetNumOfUnits(double min, double max, DateTimeIntervalType type)
		{
			double intervalSize = GetIntervalSize(min, 1.0, type);
			return (int)Math.Round((max - min) / intervalSize);
		}

		internal ChartValueTypes GetDateTimeType()
		{
			ArrayList arrayList = null;
			ChartValueTypes result = ChartValueTypes.Auto;
			if (axisType == AxisName.X)
			{
				arrayList = chartArea.GetXAxesSeries(AxisType.Primary, ((Axis)this).SubAxisName);
				if (arrayList.Count == 0)
				{
					return ChartValueTypes.Auto;
				}
				if (base.Common.DataManager.Series[arrayList[0]].IsXValueDateTime())
				{
					result = base.Common.DataManager.Series[arrayList[0]].XValueType;
				}
			}
			else if (axisType == AxisName.X2)
			{
				arrayList = chartArea.GetXAxesSeries(AxisType.Secondary, ((Axis)this).SubAxisName);
				if (arrayList.Count == 0)
				{
					return ChartValueTypes.Auto;
				}
				if (base.Common.DataManager.Series[arrayList[0]].IsXValueDateTime())
				{
					result = base.Common.DataManager.Series[arrayList[0]].XValueType;
				}
			}
			else if (axisType == AxisName.Y)
			{
				arrayList = chartArea.GetYAxesSeries(AxisType.Primary, ((Axis)this).SubAxisName);
				if (arrayList.Count == 0)
				{
					return ChartValueTypes.Auto;
				}
				if (base.Common.DataManager.Series[arrayList[0]].IsYValueDateTime())
				{
					result = base.Common.DataManager.Series[arrayList[0]].YValueType;
				}
			}
			else if (axisType == AxisName.Y2)
			{
				arrayList = chartArea.GetYAxesSeries(AxisType.Secondary, ((Axis)this).SubAxisName);
				if (arrayList.Count == 0)
				{
					return ChartValueTypes.Auto;
				}
				if (base.Common.DataManager.Series[arrayList[0]].IsYValueDateTime())
				{
					result = base.Common.DataManager.Series[arrayList[0]].YValueType;
				}
			}
			return result;
		}

		private double GetCrossing()
		{
			if (double.IsNaN(crossing))
			{
				if (base.Common.ChartTypeRegistry.GetChartType((string)chartArea.ChartTypes[0]).ZeroCrossing)
				{
					if (GetViewMinimum() > 0.0)
					{
						return GetViewMinimum();
					}
					if (GetViewMaximum() < 0.0)
					{
						return GetViewMaximum();
					}
					return 0.0;
				}
				return GetViewMinimum();
			}
			if (crossing == double.MaxValue)
			{
				return GetViewMaximum();
			}
			if (crossing == double.MinValue)
			{
				return GetViewMinimum();
			}
			return crossing;
		}

		internal void SetAutoMinimum(double min)
		{
			if (autoMinimum)
			{
				minimum = min;
			}
		}

		internal void SetAutoMaximum(double max)
		{
			if (autoMaximum)
			{
				maximum = max;
			}
		}

		internal Axis GetOppositeAxis()
		{
			if (oppositeAxis != null)
			{
				return oppositeAxis;
			}
			switch (axisType)
			{
			case AxisName.X:
			{
				ArrayList yAxesSeries = chartArea.GetXAxesSeries(AxisType.Primary, ((Axis)this).SubAxisName);
				if (yAxesSeries.Count == 0)
				{
					oppositeAxis = chartArea.AxisY;
				}
				else if (base.Common.DataManager.Series[yAxesSeries[0]].YAxisType == AxisType.Primary)
				{
					oppositeAxis = chartArea.AxisY.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].YSubAxisName);
				}
				else
				{
					oppositeAxis = chartArea.AxisY2.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].YSubAxisName);
				}
				break;
			}
			case AxisName.X2:
			{
				ArrayList yAxesSeries = chartArea.GetXAxesSeries(AxisType.Secondary, ((Axis)this).SubAxisName);
				if (yAxesSeries.Count == 0)
				{
					oppositeAxis = chartArea.AxisY2;
				}
				else if (base.Common.DataManager.Series[yAxesSeries[0]].YAxisType == AxisType.Primary)
				{
					oppositeAxis = chartArea.AxisY.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].YSubAxisName);
				}
				else
				{
					oppositeAxis = chartArea.AxisY2.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].YSubAxisName);
				}
				break;
			}
			case AxisName.Y:
			{
				ArrayList yAxesSeries = chartArea.GetYAxesSeries(AxisType.Primary, ((Axis)this).SubAxisName);
				if (yAxesSeries.Count == 0)
				{
					oppositeAxis = chartArea.AxisX;
				}
				else if (base.Common.DataManager.Series[yAxesSeries[0]].XAxisType == AxisType.Primary)
				{
					oppositeAxis = chartArea.AxisX.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].XSubAxisName);
				}
				else
				{
					oppositeAxis = chartArea.AxisX2.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].XSubAxisName);
				}
				break;
			}
			case AxisName.Y2:
			{
				ArrayList yAxesSeries = chartArea.GetYAxesSeries(AxisType.Secondary, ((Axis)this).SubAxisName);
				if (yAxesSeries.Count == 0)
				{
					oppositeAxis = chartArea.AxisX2;
				}
				else if (base.Common.DataManager.Series[yAxesSeries[0]].XAxisType == AxisType.Primary)
				{
					oppositeAxis = chartArea.AxisX.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].XSubAxisName);
				}
				else
				{
					oppositeAxis = chartArea.AxisX2.GetSubAxis(base.Common.DataManager.Series[yAxesSeries[0]].XSubAxisName);
				}
				break;
			}
			}
			return oppositeAxis;
		}

		internal void Invalidate()
		{
		}

		internal double GetLinearPosition(double axisValue)
		{
			bool flag = (chartArea != null && chartArea.chartAreaIsCurcular) ? true : false;
			if (!optimizedGetPosition)
			{
				paintViewMax = GetViewMaximum();
				paintViewMin = GetViewMinimum();
				paintRange = paintViewMax - paintViewMin;
				paintAreaPosition = base.PlotAreaPosition.ToRectangleF();
				if (flag)
				{
					paintAreaPosition.Width /= 2f;
					paintAreaPosition.Height /= 2f;
				}
				paintAreaPositionBottom = paintAreaPosition.Y + paintAreaPosition.Height;
				paintAreaPositionRight = paintAreaPosition.X + paintAreaPosition.Width;
				if (AxisPosition == AxisPosition.Top || AxisPosition == AxisPosition.Bottom)
				{
					paintChartAreaSize = paintAreaPosition.Width;
				}
				else
				{
					paintChartAreaSize = paintAreaPosition.Height;
				}
				valueMultiplier = 0.0;
				if (paintRange != 0.0)
				{
					valueMultiplier = paintChartAreaSize / paintRange;
				}
			}
			double num = valueMultiplier * (axisValue - paintViewMin);
			if (scaleSegmentsUsed)
			{
				AxisScaleSegment axisScaleSegment = ScaleSegments.FindScaleSegmentForAxisValue(axisValue);
				if (axisScaleSegment != null)
				{
					double scaleSize = 0.0;
					double scalePosition = 0.0;
					axisScaleSegment.GetScalePositionAndSize(paintChartAreaSize, out scalePosition, out scaleSize);
					if (!ScaleSegments.AllowOutOfScaleValues)
					{
						if (axisValue > axisScaleSegment.ScaleMaximum)
						{
							axisValue = axisScaleSegment.ScaleMaximum;
						}
						else if (axisValue < axisScaleSegment.ScaleMinimum)
						{
							axisValue = axisScaleSegment.ScaleMinimum;
						}
					}
					double num2 = axisScaleSegment.ScaleMaximum - axisScaleSegment.ScaleMinimum;
					num = scaleSize / num2 * (axisValue - axisScaleSegment.ScaleMinimum);
					num += scalePosition;
				}
			}
			if (reverse)
			{
				if (AxisPosition == AxisPosition.Top || AxisPosition == AxisPosition.Bottom)
				{
					return paintAreaPositionRight - num;
				}
				return (double)paintAreaPosition.Y + num;
			}
			if (AxisPosition == AxisPosition.Top || AxisPosition == AxisPosition.Bottom)
			{
				return (double)paintAreaPosition.X + num;
			}
			return paintAreaPositionBottom - num;
		}

		internal void EstimateAxis()
		{
			if (!double.IsNaN(View.Size) && double.IsNaN(View.Position))
			{
				View.Position = Minimum;
			}
			double num;
			if (!double.IsNaN(view.Position) && !double.IsNaN(view.Size))
			{
				double y = GetViewMaximum();
				double y2 = GetViewMinimum();
				if (logarithmic)
				{
					y = Math.Pow(logarithmBase, y);
					y2 = Math.Pow(logarithmBase, y2);
				}
				else
				{
					EstimateAxis(ref minimum, ref maximum, autoMaximum, autoMinimum);
				}
				num = EstimateAxis(ref y2, ref y, autoMaximum: true, autoMinimum: true);
			}
			else
			{
				num = EstimateAxis(ref minimum, ref maximum, autoMaximum, autoMinimum);
			}
			if (num <= 0.0)
			{
				throw new InvalidOperationException(SR.ExceptionAxisScaleAutoIntervalInvalid);
			}
			if (chartArea.SeriesIntegerType(axisType, string.Empty))
			{
				num = Math.Round(num);
				if (num == 0.0)
				{
					num = 1.0;
				}
				minimum = Math.Floor(minimum);
			}
			SetInterval = num;
		}

		internal double EstimateAxis(ref double minimum, ref double maximum, bool autoMaximum, bool autoMinimum)
		{
			if (maximum < minimum)
			{
				if (!base.Common.ChartPicture.SuppressExceptions)
				{
					throw new InvalidOperationException(SR.ExceptionAxisScaleMinimumValueIsGreaterThenMaximumDataPoint);
				}
				double num = maximum;
				maximum = minimum;
				minimum = num;
			}
			ChartValueTypes dateTimeType = GetDateTimeType();
			double num2 = logarithmic ? EstimateLogarithmicAxis(ref minimum, ref maximum, crossing, autoMaximum, autoMinimum) : ((dateTimeType == ChartValueTypes.Auto) ? EstimateNumberAxis(ref minimum, ref maximum, StartFromZero, prefferedNumberofIntervals, crossing, autoMaximum, autoMinimum) : EstimateDateAxis(ref minimum, ref maximum, crossing, autoMaximum, autoMinimum, dateTimeType));
			if (num2 <= 0.0)
			{
				throw new InvalidOperationException(SR.ExceptionAxisScaleAutoIntervalInvalid);
			}
			SetInterval = num2;
			return num2;
		}

		private double EstimateLogarithmicAxis(ref double minimum, ref double maximum, double crossing, bool autoMaximum, bool autoMinimum)
		{
			if (!logarithmicConvertedToLinear)
			{
				logarithmicMinimum = this.minimum;
				logarithmicMaximum = this.maximum;
				logarithmicCrossing = this.crossing;
			}
			margin = 100.0;
			if (base.Common != null && base.Common.Chart != null && base.Common.Chart.chartPicture.SuppressExceptions)
			{
				if (minimum <= 0.0)
				{
					minimum = 1.0;
				}
				if (maximum <= 0.0)
				{
					maximum = 1.0;
				}
				if (crossing <= 0.0 && crossing != double.MinValue)
				{
					crossing = 1.0;
				}
			}
			if (minimum <= 0.0 || maximum <= 0.0 || crossing <= 0.0)
			{
				if (minimum <= 0.0)
				{
					throw new ArgumentOutOfRangeException("minimum", SR.ExceptionAxisScaleLogarithmicNegativeValues);
				}
				if (maximum <= 0.0)
				{
					throw new ArgumentOutOfRangeException("maximum", SR.ExceptionAxisScaleLogarithmicNegativeValues);
				}
			}
			crossing = Math.Log(crossing, logarithmBase);
			minimum = Math.Log(minimum, logarithmBase);
			maximum = Math.Log(maximum, logarithmBase);
			logarithmicConvertedToLinear = true;
			double num = Math.Floor((maximum - minimum) / 5.0);
			if (num == 0.0)
			{
				num = 1.0;
			}
			if (autoMinimum && autoMaximum)
			{
				RoundedValues(num, StartFromZero, autoMaximum, autoMinimum, ref minimum, ref maximum);
			}
			if (chartArea.hundredPercent)
			{
				if (autoMinimum && minimum < 0.0)
				{
					minimum = 0.0;
				}
				if (autoMaximum && maximum > 2.0)
				{
					maximum = 2.0;
				}
			}
			return num;
		}

		private double EstimateDateAxis(ref double minimum, ref double maximum, double crossing, bool autoMaximum, bool autoMinimum, ChartValueTypes valuesType)
		{
			double num = minimum;
			double num2 = maximum;
			double num3 = CalcInterval(num, num2, date: true, out intervalType, valuesType);
			if (!double.IsNaN(interval3DCorrection) && chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular)
			{
				num3 = Math.Floor(num3 / interval3DCorrection);
				interval3DCorrection = double.NaN;
			}
			int numOfUnits = GetNumOfUnits(num, num2, intervalType);
			if (axisType == AxisName.Y || axisType == AxisName.Y2)
			{
				if (autoMinimum && minimum > GetIntervalSize(num, num3, intervalType))
				{
					minimum += GetIntervalSize(num, (0.0 - num3 / 2.0) * margin / 100.0, intervalType, null, 0.0, DateTimeIntervalType.Number, forceIntIndex: false, forceAbsInterval: false);
					minimum = AlignIntervalStart(minimum, num3 * margin / 100.0, intervalType);
				}
				if (autoMaximum && num2 > 0.0 && margin != 0.0)
				{
					maximum = minimum + GetIntervalSize(minimum, (Math.Floor((double)numOfUnits / num3 / margin * 100.0) + 2.0) * num3 * margin / 100.0, intervalType);
				}
			}
			InternalIntervalType = intervalType;
			return num3;
		}

		internal double EstimateNumberAxis(ref double minimum, ref double maximum, bool shouldStartFromZero, int preferredNumberOfIntervals, double crossing, bool autoMaximum, bool autoMinimum)
		{
			double num = minimum;
			double num2 = maximum;
			double num3;
			if (!roundedXValues && (axisType == AxisName.X || axisType == AxisName.X2))
			{
				num3 = chartArea.GetPointsInterval(logarithmic: false, 10.0);
				if (num3 == 0.0 || (num2 - num) / num3 > 20.0)
				{
					num3 = (num2 - num) / (double)preferredNumberOfIntervals;
				}
			}
			else
			{
				num3 = (num2 - num) / (double)preferredNumberOfIntervals;
			}
			if (!double.IsNaN(interval3DCorrection) && chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular)
			{
				num3 /= interval3DCorrection;
				if (num2 - num < num3)
				{
					num3 = num2 - num;
				}
				interval3DCorrection = double.NaN;
				if (num3 != 0.0)
				{
					num3 = CalcInterval(num3);
				}
			}
			double num4;
			if (autoMaximum || autoMinimum)
			{
				if (num3 == 0.0)
				{
					num2 = num + 1.0;
					num3 = 0.2;
					num4 = 0.2;
				}
				else
				{
					num4 = CalcInterval(num3);
				}
			}
			else
			{
				num4 = num3;
			}
			if (((Axis)this).interval != 0.0 && ((Axis)this).interval > num4 && minimum + ((Axis)this).interval > maximum)
			{
				num4 = ((Axis)this).interval;
				if (autoMaximum)
				{
					maximum = minimum + num4;
				}
				if (autoMinimum)
				{
					minimum = maximum - num4;
				}
			}
			if (axisType == AxisName.Y || axisType == AxisName.Y2 || (roundedXValues && (axisType == AxisName.X || axisType == AxisName.X2)))
			{
				bool flag = false;
				bool flag2 = false;
				if (chartArea.hundredPercent)
				{
					flag = (minimum == 0.0);
					flag2 = (maximum == 0.0);
				}
				RoundedValues(num4, shouldStartFromZero, autoMaximum, autoMinimum, ref minimum, ref maximum);
				if (chartArea.hundredPercent)
				{
					if (autoMinimum)
					{
						if (minimum < -100.0)
						{
							minimum = -100.0;
						}
						if (flag)
						{
							minimum = 0.0;
						}
					}
					if (autoMaximum)
					{
						if (maximum > 100.0)
						{
							maximum = 100.0;
						}
						if (flag2)
						{
							maximum = 0.0;
						}
					}
				}
			}
			return num4;
		}
	}
}
