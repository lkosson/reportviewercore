using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class LinearPinLabel : GaugeObject
	{
		private string text = "";

		private Font font = new Font("Microsoft Sans Serif", 12f);

		private FontUnit fontUnit;

		private Color textColor = Color.Black;

		private Placement placement;

		private float fontAngle;

		private float scaleOffset = 2f;

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeText5")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFont4")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFontUnit")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeTextColor")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributePlacement3")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFontAngle")]
		[ValidateBound(0.0, 360.0)]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 360));
				}
				fontAngle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeDistanceFromScale6")]
		[ValidateBound(-30.0, 30.0)]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
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
