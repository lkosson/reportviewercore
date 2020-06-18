using Microsoft.Reporting.Chart.WebForms.Design;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[DefaultProperty("Enabled")]
	[SRDescription("DescriptionAttributeSmartLabelsStyle_SmartLabelsStyle")]
	[TypeConverter(typeof(NoNameExpandableObjectConverter))]
	internal class SmartLabelsStyle
	{
		internal object chartElement;

		private bool enabled;

		private bool markerOverlapping;

		private bool hideOverlapped = true;

		private LabelAlignmentTypes movingDirection = LabelAlignmentTypes.Top | LabelAlignmentTypes.Bottom | LabelAlignmentTypes.Right | LabelAlignmentTypes.Left | LabelAlignmentTypes.TopLeft | LabelAlignmentTypes.TopRight | LabelAlignmentTypes.BottomLeft | LabelAlignmentTypes.BottomRight;

		private double minMovingDistance;

		private double maxMovingDistance = 30.0;

		private LabelOutsidePlotAreaStyle allowOutsidePlotArea = LabelOutsidePlotAreaStyle.Partial;

		private LabelCalloutStyle calloutStyle = LabelCalloutStyle.Underlined;

		private Color calloutLineColor = Color.Black;

		private ChartDashStyle calloutLineStyle = ChartDashStyle.Solid;

		private Color calloutBackColor = Color.Transparent;

		private int calloutLineWidth = 1;

		private LineAnchorCap calloutLineAnchorCap = LineAnchorCap.Arrow;

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeEnabled13")]
		[ParenthesizePropertyName(true)]
		public virtual bool Enabled
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

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeMarkerOverlapping")]
		public virtual bool MarkerOverlapping
		{
			get
			{
				return markerOverlapping;
			}
			set
			{
				markerOverlapping = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeHideOverlapped")]
		public virtual bool HideOverlapped
		{
			get
			{
				return hideOverlapped;
			}
			set
			{
				hideOverlapped = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(typeof(LabelAlignmentTypes), "Top, Bottom, Right, Left, TopLeft, TopRight, BottomLeft, BottomRight")]
		[SRDescription("DescriptionAttributeMovingDirection")]
		public virtual LabelAlignmentTypes MovingDirection
		{
			get
			{
				return movingDirection;
			}
			set
			{
				if (value == (LabelAlignmentTypes)0)
				{
					Series series = chartElement as Series;
					if (series == null || series.chart == null || !series.chart.SuppressExceptions)
					{
						throw new InvalidOperationException(SR.ExceptionSmartLabelsDirectionUndefined);
					}
				}
				movingDirection = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeMinMovingDistance")]
		public virtual double MinMovingDistance
		{
			get
			{
				return minMovingDistance;
			}
			set
			{
				if (value < 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionSmartLabelsMinMovingDistanceIsNegative);
				}
				minMovingDistance = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(30.0)]
		[SRDescription("DescriptionAttributeMaxMovingDistance")]
		public virtual double MaxMovingDistance
		{
			get
			{
				return maxMovingDistance;
			}
			set
			{
				if (value < 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionSmartLabelsMaxMovingDistanceIsNegative);
				}
				maxMovingDistance = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(LabelOutsidePlotAreaStyle.Partial)]
		[SRDescription("DescriptionAttributeAllowOutsidePlotArea")]
		public virtual LabelOutsidePlotAreaStyle AllowOutsidePlotArea
		{
			get
			{
				return allowOutsidePlotArea;
			}
			set
			{
				allowOutsidePlotArea = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue(LabelCalloutStyle.Underlined)]
		[SRDescription("DescriptionAttributeCalloutStyle3")]
		public virtual LabelCalloutStyle CalloutStyle
		{
			get
			{
				return calloutStyle;
			}
			set
			{
				calloutStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeCalloutLineColor")]
		public virtual Color CalloutLineColor
		{
			get
			{
				return calloutLineColor;
			}
			set
			{
				calloutLineColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeCalloutLineStyle")]
		public virtual ChartDashStyle CalloutLineStyle
		{
			get
			{
				return calloutLineStyle;
			}
			set
			{
				calloutLineStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Transparent")]
		[SRDescription("DescriptionAttributeCalloutBackColor")]
		public virtual Color CalloutBackColor
		{
			get
			{
				return calloutBackColor;
			}
			set
			{
				calloutBackColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeCalloutLineWidth")]
		public virtual int CalloutLineWidth
		{
			get
			{
				return calloutLineWidth;
			}
			set
			{
				calloutLineWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(LineAnchorCap.Arrow)]
		[SRDescription("DescriptionAttributeCalloutLineAnchorCap")]
		public virtual LineAnchorCap CalloutLineAnchorCap
		{
			get
			{
				return calloutLineAnchorCap;
			}
			set
			{
				calloutLineAnchorCap = value;
				Invalidate();
			}
		}

		public SmartLabelsStyle()
		{
			chartElement = null;
		}

		public SmartLabelsStyle(object chartElement)
		{
			this.chartElement = chartElement;
		}

		private void Invalidate()
		{
			if (chartElement != null)
			{
				if (chartElement is Series)
				{
					((Series)chartElement).Invalidate(invalidateAreaOnly: false, invalidateLegend: false);
				}
				else if (chartElement is Annotation)
				{
					((Annotation)chartElement).Invalidate();
				}
			}
		}
	}
}
