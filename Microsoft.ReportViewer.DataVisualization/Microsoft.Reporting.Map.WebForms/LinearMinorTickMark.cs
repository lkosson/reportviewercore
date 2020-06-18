using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class LinearMinorTickMark : TickMark
	{
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearMinorTickMark_Shape")]
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
		[SRDescription("DescriptionAttributeLinearMinorTickMark_Length")]
		[NotifyParentProperty(true)]
		[DefaultValue(9f)]
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
		[SRDescription("DescriptionAttributeLinearMinorTickMark_Width")]
		[NotifyParentProperty(true)]
		[DefaultValue(3f)]
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

		public LinearMinorTickMark()
			: this(null)
		{
		}

		public LinearMinorTickMark(object parent)
			: base(parent)
		{
			Shape = MarkerStyle.Rectangle;
			Width = 3f;
			Length = 9f;
		}
	}
}
