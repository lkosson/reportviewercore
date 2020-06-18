using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class SpecialPosition : PinMajorTickMark
	{
		private bool enable;

		private float location = 5f;

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSpecialPosition_Enable")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSpecialPosition_Location")]
		[DefaultValue(5f)]
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
