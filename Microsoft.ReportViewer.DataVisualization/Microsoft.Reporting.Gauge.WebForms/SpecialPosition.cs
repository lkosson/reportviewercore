using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class SpecialPosition : PinMajorTickMark
	{
		private bool enable;

		private float location = 5f;

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeEnable")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		[DefaultValue(false)]
		public bool Enable
		{
			get
			{
				return enable;
			}
			set
			{
				enable = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeLocation6")]
		[DefaultValue(5f)]
		[ValidateBound(-50.0, 50.0)]
		public virtual float Location
		{
			get
			{
				return location;
			}
			set
			{
				location = value;
				Invalidate();
			}
		}

		public SpecialPosition()
			: this(null)
		{
		}

		public SpecialPosition(object parent)
			: base(parent)
		{
		}
	}
}
