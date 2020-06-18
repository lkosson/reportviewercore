using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class LinearMajorTickMark : TickMark
	{
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearMajorTickMark_Shape")]
		[NotifyParentProperty(true)]
		[DefaultValue(MarkerStyle.Rectangle)]
		public override MarkerStyle Shape
		{
			get
			{
				return base.Shape;
			}
			set
			{
				base.Shape = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearMajorTickMark_Length")]
		[NotifyParentProperty(true)]
		[DefaultValue(15f)]
		public override float Length
		{
			get
			{
				return base.Length;
			}
			set
			{
				base.Length = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearMajorTickMark_Width")]
		[NotifyParentProperty(true)]
		[DefaultValue(4f)]
		public override float Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				base.Width = value;
			}
		}

		public LinearMajorTickMark()
			: this(null)
		{
		}

		public LinearMajorTickMark(object parent)
			: base(parent)
		{
			Shape = MarkerStyle.Rectangle;
			Width = 4f;
			Length = 15f;
		}
	}
}
