using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class LinearPinLabel : MapObject
	{
		private string text = "";

		private Font font = new Font("Microsoft Sans Serif", 12f);

		private FontUnit fontUnit;

		private Color textColor = Color.Black;

		private Placement placement;

		private float fontAngle;

		private float scaleOffset = 2f;

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearPinLabel_Text")]
		[Localizable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPinLabel_Font")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 12pt")]
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPinLabel_FontUnit")]
		[DefaultValue(FontUnit.Percent)]
		public FontUnit FontUnit
		{
			get
			{
				return fontUnit;
			}
			set
			{
				fontUnit = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPinLabel_TextColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Black")]
		public Color TextColor
		{
			get
			{
				return textColor;
			}
			set
			{
				textColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPinLabel_Placement")]
		[DefaultValue(Placement.Inside)]
		public Placement Placement
		{
			get
			{
				return placement;
			}
			set
			{
				placement = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPinLabel_FontAngle")]
		[DefaultValue(0f)]
		public float FontAngle
		{
			get
			{
				return fontAngle;
			}
			set
			{
				if (value > 360f || value < 0f)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 360.0));
				}
				fontAngle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPinLabel_DistanceFromScale")]
		[DefaultValue(2f)]
		public float DistanceFromScale
		{
			get
			{
				return scaleOffset;
			}
			set
			{
				if (value < -100f || value > 100f)
				{
					throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
				}
				scaleOffset = value;
				Invalidate();
			}
		}

		public LinearPinLabel()
			: this(null)
		{
		}

		public LinearPinLabel(object parent)
			: base(parent)
		{
		}
	}
}
