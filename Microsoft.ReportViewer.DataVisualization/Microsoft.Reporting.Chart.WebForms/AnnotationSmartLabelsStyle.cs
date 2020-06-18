using Microsoft.Reporting.Chart.WebForms.Design;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[DefaultProperty("Enabled")]
	[SRDescription("DescriptionAttributeAnnotationSmartLabelsStyle_AnnotationSmartLabelsStyle")]
	[TypeConverter(typeof(NoNameExpandableObjectConverter))]
	internal class AnnotationSmartLabelsStyle : SmartLabelsStyle
	{
		[SRCategory("CategoryAttributeMisc")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(LabelCalloutStyle.Underlined)]
		[SRDescription("DescriptionAttributeCalloutStyle3")]
		public override LabelCalloutStyle CalloutStyle
		{
			get
			{
				return base.CalloutStyle;
			}
			set
			{
				base.CalloutStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeCalloutLineColor")]
		public override Color CalloutLineColor
		{
			get
			{
				return base.CalloutLineColor;
			}
			set
			{
				base.CalloutLineColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeCalloutLineStyle")]
		public override ChartDashStyle CalloutLineStyle
		{
			get
			{
				return base.CalloutLineStyle;
			}
			set
			{
				base.CalloutLineStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(Color), "Transparent")]
		[SRDescription("DescriptionAttributeCalloutBackColor")]
		public override Color CalloutBackColor
		{
			get
			{
				return base.CalloutBackColor;
			}
			set
			{
				base.CalloutBackColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeCalloutLineWidth")]
		public override int CalloutLineWidth
		{
			get
			{
				return base.CalloutLineWidth;
			}
			set
			{
				base.CalloutLineWidth = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(LineAnchorCap.Arrow)]
		[SRDescription("DescriptionAttributeCalloutLineAnchorCap")]
		public override LineAnchorCap CalloutLineAnchorCap
		{
			get
			{
				return base.CalloutLineAnchorCap;
			}
			set
			{
				base.CalloutLineAnchorCap = value;
			}
		}

		public AnnotationSmartLabelsStyle()
		{
			chartElement = null;
		}

		public AnnotationSmartLabelsStyle(object chartElement)
			: base(chartElement)
		{
		}
	}
}
