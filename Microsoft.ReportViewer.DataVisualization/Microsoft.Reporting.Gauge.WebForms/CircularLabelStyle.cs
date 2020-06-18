using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CircularLabelStyle : LinearLabelStyle
	{
		private bool rotateLabels = true;

		private bool allowUpsideDown;

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCircularLabelStyle_RotateLabels")]
		[DefaultValue(true)]
		public bool RotateLabels
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
		[SRDescription("DescriptionAttributeCircularLabelStyle_AllowUpsideDown")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFont3")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 14pt")]
		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
			}
		}

		public CircularLabelStyle()
			: this(null)
		{
		}

		public CircularLabelStyle(object parent)
			: base(parent, new Font("Microsoft Sans Serif", 14f))
		{
		}
	}
}
