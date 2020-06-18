using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CircularMinorTickMark : TickMark
	{
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeShape3")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeLength3")]
		[NotifyParentProperty(true)]
		[ValidateBound(0.0, 50.0)]
		[DefaultValue(8f)]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeWidth7")]
		[NotifyParentProperty(true)]
		[ValidateBound(0.0, 30.0)]
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

		public CircularMinorTickMark()
			: this(null)
		{
		}

		public CircularMinorTickMark(object parent)
			: base(parent, MarkerStyle.Rectangle, 8f, 3f)
		{
		}
	}
}
