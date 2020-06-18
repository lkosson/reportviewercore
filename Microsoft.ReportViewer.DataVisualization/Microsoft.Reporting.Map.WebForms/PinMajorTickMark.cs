using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PinMajorTickMark : CustomTickMark
	{
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePinMajorTickMark_Shape")]
		[NotifyParentProperty(true)]
		[DefaultValue(MarkerStyle.Circle)]
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
		[SRDescription("DescriptionAttributePinMajorTickMark_Length")]
		[NotifyParentProperty(true)]
		[DefaultValue(6f)]
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
		[SRDescription("DescriptionAttributePinMajorTickMark_Width")]
		[NotifyParentProperty(true)]
		[DefaultValue(6f)]
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

		public PinMajorTickMark()
			: this(null)
		{
		}

		public PinMajorTickMark(object parent)
			: base(parent)
		{
			Shape = MarkerStyle.Circle;
			Width = 6f;
			Length = 6f;
		}

		public PinMajorTickMark(object parent, MarkerStyle shape, float length, float width)
			: base(parent, shape, length, width)
		{
		}
	}
}
