using Microsoft.Reporting.Chart.WebForms.Design;
using Microsoft.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAxisDataView_AxisDataView")]
	[DefaultProperty("Position")]
	internal class AxisDataView
	{
		internal Axis axis;

		private double position = double.NaN;

		private double size = double.NaN;

		private DateTimeIntervalType sizeType;

		private double minSize = double.NaN;

		private DateTimeIntervalType minSizeType;

		private bool zoomable = true;

		private double smallScrollSize = double.NaN;

		private DateTimeIntervalType smallScrollSizeType;

		private double smallScrollMinSize = 1.0;

		private DateTimeIntervalType smallScrollMinSizeType;

		private bool ignoreValidation;

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAxisDataView_Position")]
		[TypeConverter(typeof(DoubleDateNanValueConverter))]
		[ParenthesizePropertyName(true)]
		public double Position
		{
			get
			{
				if (axis != null && axis.chartArea != null && axis.chartArea.chartAreaIsCurcular)
				{
					return double.NaN;
				}
				return position;
			}
			set
			{
				if ((axis == null || axis.chartArea == null || !axis.chartArea.chartAreaIsCurcular) && position != value)
				{
					position = value;
					if (axis != null && axis.chartArea != null && axis.Common != null && axis.Common.ChartPicture != null && !axis.chartArea.alignmentInProcess)
					{
						AreaAlignOrientations orientation = (axis.axisType == AxisName.X || axis.axisType == AxisName.X2) ? AreaAlignOrientations.Vertical : AreaAlignOrientations.Horizontal;
						axis.Common.ChartPicture.AlignChartAreasAxesView(axis.chartArea, orientation);
					}
					if (!ignoreValidation && axis != null)
					{
						axis.Invalidate();
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAxisDataView_Size")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[ParenthesizePropertyName(true)]
		public double Size
		{
			get
			{
				if (axis != null && axis.chartArea != null && axis.chartArea.chartAreaIsCurcular)
				{
					return double.NaN;
				}
				return size;
			}
			set
			{
				if ((axis == null || axis.chartArea == null || !axis.chartArea.chartAreaIsCurcular) && size != value)
				{
					size = value;
					if (axis != null && axis.chartArea != null && axis.Common != null && axis.Common.ChartPicture != null && !axis.chartArea.alignmentInProcess)
					{
						AreaAlignOrientations orientation = (axis.axisType == AxisName.X || axis.axisType == AxisName.X2) ? AreaAlignOrientations.Vertical : AreaAlignOrientations.Horizontal;
						axis.Common.ChartPicture.AlignChartAreasAxesView(axis.chartArea, orientation);
					}
					if (!ignoreValidation && axis != null)
					{
						axis.Invalidate();
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeAxisDataView_SizeType")]
		[ParenthesizePropertyName(true)]
		public DateTimeIntervalType SizeType
		{
			get
			{
				return sizeType;
			}
			set
			{
				if (sizeType != value)
				{
					sizeType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
					if (axis != null && axis.chartArea != null && axis.Common != null && axis.Common.ChartPicture != null && !axis.chartArea.alignmentInProcess)
					{
						AreaAlignOrientations orientation = (axis.axisType == AxisName.X || axis.axisType == AxisName.X2) ? AreaAlignOrientations.Vertical : AreaAlignOrientations.Horizontal;
						axis.Common.ChartPicture.AlignChartAreasAxesView(axis.chartArea, orientation);
					}
					if (!ignoreValidation && axis != null)
					{
						axis.Invalidate();
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAxisDataView_MinSize")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public double MinSize
		{
			get
			{
				return minSize;
			}
			set
			{
				minSize = value;
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeAxisDataView_MinSizeType")]
		public DateTimeIntervalType MinSizeType
		{
			get
			{
				return minSizeType;
			}
			set
			{
				minSizeType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAxisDataView_Zoomable")]
		public bool Zoomable
		{
			get
			{
				return zoomable;
			}
			set
			{
				zoomable = value;
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(false)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeAxisDataView_IsZoomed")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public bool IsZoomed
		{
			get
			{
				if (!double.IsNaN(Size) && Size != 0.0)
				{
					return !double.IsNaN(Position);
				}
				return false;
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAxisDataView_SmallScrollSize")]
		[TypeConverter(typeof(AxisMinMaxAutoValueConverter))]
		public double SmallScrollSize
		{
			get
			{
				return smallScrollSize;
			}
			set
			{
				if (smallScrollSize != value)
				{
					smallScrollSize = value;
					if (!ignoreValidation && axis != null)
					{
						axis.Invalidate();
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeAxisDataView_SmallScrollSizeType")]
		public DateTimeIntervalType SmallScrollSizeType
		{
			get
			{
				return smallScrollSizeType;
			}
			set
			{
				if (smallScrollSizeType != value)
				{
					smallScrollSizeType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
					if (!ignoreValidation && axis != null)
					{
						axis.Invalidate();
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(1.0)]
		[SRDescription("DescriptionAttributeAxisDataView_SmallScrollMinSize")]
		public double SmallScrollMinSize
		{
			get
			{
				return smallScrollMinSize;
			}
			set
			{
				if (smallScrollMinSize != value)
				{
					smallScrollMinSize = value;
					if (!ignoreValidation && axis != null)
					{
						axis.Invalidate();
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeAxisDataView_SmallScrollMinSizeType")]
		public DateTimeIntervalType SmallScrollMinSizeType
		{
			get
			{
				return smallScrollMinSizeType;
			}
			set
			{
				if (smallScrollMinSizeType != value)
				{
					smallScrollMinSizeType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
					if (!ignoreValidation && axis != null)
					{
						axis.Invalidate();
					}
				}
			}
		}

		public AxisDataView()
		{
			axis = null;
		}

		public AxisDataView(Axis axis)
		{
			this.axis = axis;
		}

		public double GetViewMinimum()
		{
			if (!double.IsNaN(Size))
			{
				if (!double.IsNaN(Position))
				{
					if (Position <= axis.minimum)
					{
						return Position;
					}
					return Position - axis.marginView;
				}
				Position = axis.Minimum;
			}
			return axis.minimum;
		}

		public double GetViewMaximum()
		{
			if (!double.IsNaN(Size))
			{
				if (!double.IsNaN(Position))
				{
					double intervalSize = axis.GetIntervalSize(Position, Size, SizeType);
					if (Position + intervalSize >= axis.maximum)
					{
						return Position + intervalSize;
					}
					return Position + intervalSize + axis.marginView;
				}
				Position = axis.Minimum;
			}
			return axis.maximum;
		}

		internal Chart GetChartObject()
		{
			if (axis != null)
			{
				if (axis.chart != null)
				{
					return axis.chart;
				}
				if (axis.Common != null && axis.Common.container != null)
				{
					axis.chart = (Chart)axis.Common.container.GetService(typeof(Chart));
				}
				return axis.chart;
			}
			return null;
		}
	}
}
