using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CircularPinLabel : LinearPinLabel
	{
		private bool rotateLabels;

		private bool allowUpsideDown;

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCircularPinLabel_RotateLabel")]
		[DefaultValue(false)]
		public bool RotateLabel
		{
			get
			{
				return rotateLabels;
			}
			set
			{
				rotateLabels = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCircularPinLabel_AllowUpsideDown")]
		[DefaultValue(false)]
		public bool AllowUpsideDown
		{
			get
			{
				return allowUpsideDown;
			}
			set
			{
				allowUpsideDown = value;
				Invalidate();
			}
		}

		public CircularPinLabel()
			: this(null)
		{
		}

		public CircularPinLabel(object parent)
			: base(parent)
		{
		}
	}
}
