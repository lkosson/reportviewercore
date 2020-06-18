using Microsoft.Reporting.Chart.WebForms.Design;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[DefaultProperty("Enabled")]
	[SRDescription("DescriptionAttributeCursor_Cursor")]
	internal class Cursor
	{
		private ChartArea chartArea;

		private AxisName attachedToXAxis;

		private bool userEnabled;

		private bool userSelection;

		private bool autoScroll = true;

		private Color lineColor = Color.Red;

		private int lineWidth = 1;

		private ChartDashStyle lineStyle = ChartDashStyle.Solid;

		private Color selectionColor = Color.LightGray;

		private AxisType axisType;

		private double position = double.NaN;

		private double selectionStart = double.NaN;

		private double selectionEnd = double.NaN;

		private double interval = 1.0;

		private DateTimeIntervalType intervalType;

		private double intervalOffset;

		private DateTimeIntervalType intervalOffsetType;

		private Axis axis;

		private PointF userSelectionStart = PointF.Empty;

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeCursor_Position")]
		[ParenthesizePropertyName(true)]
		[TypeConverter(typeof(DoubleDateNanValueConverter))]
		public double Position
		{
			get
			{
				return position;
			}
			set
			{
				if (position != value)
				{
					position = value;
					if (chartArea != null && chartArea.Common != null && chartArea.Common.ChartPicture != null && !chartArea.alignmentInProcess)
					{
						AreaAlignOrientations orientation = (attachedToXAxis == AxisName.X || attachedToXAxis == AxisName.X2) ? AreaAlignOrientations.Vertical : AreaAlignOrientations.Horizontal;
						chartArea.Common.ChartPicture.AlignChartAreasCursor(chartArea, orientation, selectionChanged: false);
					}
					if (chartArea != null && !chartArea.alignmentInProcess)
					{
						Invalidate(invalidateArea: false);
					}
				}
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeCursor_SelectionStart")]
		[TypeConverter(typeof(DoubleDateNanValueConverter))]
		public double SelectionStart
		{
			get
			{
				return selectionStart;
			}
			set
			{
				if (selectionStart != value)
				{
					selectionStart = value;
					if (chartArea != null && chartArea.Common != null && chartArea.Common.ChartPicture != null && !chartArea.alignmentInProcess)
					{
						AreaAlignOrientations orientation = (attachedToXAxis == AxisName.X || attachedToXAxis == AxisName.X2) ? AreaAlignOrientations.Vertical : AreaAlignOrientations.Horizontal;
						chartArea.Common.ChartPicture.AlignChartAreasCursor(chartArea, orientation, selectionChanged: false);
					}
					if (chartArea != null && !chartArea.alignmentInProcess)
					{
						Invalidate(invalidateArea: false);
					}
				}
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeCursor_SelectionEnd")]
		[TypeConverter(typeof(DoubleDateNanValueConverter))]
		public double SelectionEnd
		{
			get
			{
				return selectionEnd;
			}
			set
			{
				if (selectionEnd != value)
				{
					selectionEnd = value;
					if (chartArea != null && chartArea.Common != null && chartArea.Common.ChartPicture != null && !chartArea.alignmentInProcess)
					{
						AreaAlignOrientations orientation = (attachedToXAxis == AxisName.X || attachedToXAxis == AxisName.X2) ? AreaAlignOrientations.Vertical : AreaAlignOrientations.Horizontal;
						chartArea.Common.ChartPicture.AlignChartAreasCursor(chartArea, orientation, selectionChanged: false);
					}
					if (chartArea != null && !chartArea.alignmentInProcess)
					{
						Invalidate(invalidateArea: false);
					}
				}
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeCursor_UserEnabled")]
		public bool UserEnabled
		{
			get
			{
				return userEnabled;
			}
			set
			{
				userEnabled = value;
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeCursor_UserSelection")]
		public bool UserSelection
		{
			get
			{
				return userSelection;
			}
			set
			{
				userSelection = value;
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeCursor_AutoScroll")]
		public bool AutoScroll
		{
			get
			{
				return autoScroll;
			}
			set
			{
				autoScroll = value;
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCursor_AxisType")]
		[DefaultValue(AxisType.Primary)]
		public AxisType AxisType
		{
			get
			{
				return axisType;
			}
			set
			{
				axisType = value;
				axis = null;
				Invalidate(invalidateArea: true);
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(1.0)]
		[SRDescription("DescriptionAttributeCursor_Interval")]
		public double Interval
		{
			get
			{
				return interval;
			}
			set
			{
				interval = value;
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeCursor_IntervalType")]
		public DateTimeIntervalType IntervalType
		{
			get
			{
				return intervalType;
			}
			set
			{
				intervalType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeCursor_IntervalOffset")]
		public double IntervalOffset
		{
			get
			{
				return intervalOffset;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(SR.ExceptionCursorIntervalOffsetIsNegative, "value");
				}
				intervalOffset = value;
			}
		}

		[SRCategory("CategoryAttributeBehavior")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeCursor_IntervalOffsetType")]
		public DateTimeIntervalType IntervalOffsetType
		{
			get
			{
				return intervalOffsetType;
			}
			set
			{
				intervalOffsetType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Red")]
		[SRDescription("DescriptionAttributeCursor_LineColor")]
		public Color LineColor
		{
			get
			{
				return lineColor;
			}
			set
			{
				lineColor = value;
				Invalidate(invalidateArea: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeCursor_LineStyle")]
		public ChartDashStyle LineStyle
		{
			get
			{
				return lineStyle;
			}
			set
			{
				lineStyle = value;
				Invalidate(invalidateArea: false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeCursor_LineWidth")]
		public int LineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionCursorLineWidthIsNegative);
				}
				lineWidth = value;
				Invalidate(invalidateArea: true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "LightGray")]
		[SRDescription("DescriptionAttributeCursor_SelectionColor")]
		public Color SelectionColor
		{
			get
			{
				return selectionColor;
			}
			set
			{
				selectionColor = value;
				Invalidate(invalidateArea: false);
			}
		}

		internal void Initialize(ChartArea chartArea, AxisName attachedToXAxis)
		{
			this.chartArea = chartArea;
			this.attachedToXAxis = attachedToXAxis;
		}

		private void Invalidate(bool invalidateArea)
		{
		}

		internal Axis GetAxis()
		{
			if (axis == null && chartArea != null)
			{
				if (attachedToXAxis == AxisName.X)
				{
					axis = ((axisType == AxisType.Primary) ? chartArea.AxisX : chartArea.AxisX2);
				}
				else
				{
					axis = ((axisType == AxisType.Primary) ? chartArea.AxisY : chartArea.AxisY2);
				}
			}
			return axis;
		}
	}
}
