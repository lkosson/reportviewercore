using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class CustomTickMark : MapObject
	{
		private bool visible = true;

		private Placement placement = Placement.Cross;

		private Color borderColor = Color.DarkGray;

		private int borderWidth = 1;

		private Color fillColor = Color.WhiteSmoke;

		private bool enableGradient = true;

		private float gradientDensity = 30f;

		private float offset;

		private MarkerStyle shape = MarkerStyle.Trapezoid;

		private float length = 5f;

		private float width = 3f;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_Visible")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_Placement")]
		[NotifyParentProperty(true)]
		[DefaultValue(Placement.Cross)]
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
		[SRDescription("DescriptionAttributeCustomTickMark_BorderColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_BorderWidth")]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		public int BorderWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				if (value < 0 || value > 25)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 25.0));
				}
				borderWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_FillColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "WhiteSmoke")]
		public Color FillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_EnableGradient")]
		[NotifyParentProperty(true)]
		[DefaultValue(true)]
		public bool EnableGradient
		{
			get
			{
				return enableGradient;
			}
			set
			{
				enableGradient = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_GradientDensity")]
		[NotifyParentProperty(true)]
		[DefaultValue(30f)]
		public float GradientDensity
		{
			get
			{
				return gradientDensity;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				gradientDensity = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_DistanceFromScale")]
		[NotifyParentProperty(true)]
		[DefaultValue(0f)]
		public float DistanceFromScale
		{
			get
			{
				return offset;
			}
			set
			{
				if (value < -100f || value > 100f)
				{
					throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
				}
				offset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_Shape")]
		[NotifyParentProperty(true)]
		public virtual MarkerStyle Shape
		{
			get
			{
				return shape;
			}
			set
			{
				shape = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_Length")]
		[NotifyParentProperty(true)]
		public virtual float Length
		{
			get
			{
				return length;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				length = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_Width")]
		[NotifyParentProperty(true)]
		[DefaultValue(3f)]
		public virtual float Width
		{
			get
			{
				return width;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				width = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_Image")]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		public string Image
		{
			get
			{
				return image;
			}
			set
			{
				image = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_ImageTransColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		public Color ImageTransColor
		{
			get
			{
				return imageTransColor;
			}
			set
			{
				imageTransColor = value;
				Invalidate();
			}
		}

		public CustomTickMark()
			: this(null)
		{
		}

		public CustomTickMark(object parent)
			: base(parent)
		{
		}

		public CustomTickMark(object parent, MarkerStyle shape, float length, float width)
			: this(parent)
		{
			Shape = shape;
			this.length = length;
			this.width = width;
		}
	}
}
