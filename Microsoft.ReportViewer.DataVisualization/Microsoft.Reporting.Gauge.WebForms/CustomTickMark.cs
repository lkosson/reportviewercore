using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CustomTickMark : GaugeObject
	{
		private bool visible = true;

		private Placement placement = Placement.Cross;

		private Color borderColor = Color.DimGray;

		private GaugeDashStyle borderStyle = GaugeDashStyle.Solid;

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

		private Color imageHueColor = Color.Empty;

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeVisible12")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributePlacement6")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBorderColor3")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DimGray")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBorderStyle3")]
		[DefaultValue(GaugeDashStyle.Solid)]
		public GaugeDashStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBorderWidth")]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 25));
				}
				borderWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillColor4")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeEnableGradient")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeGradientDensity")]
		[NotifyParentProperty(true)]
		[ValidateBound(0.0, 100.0)]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				gradientDensity = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeDistanceFromScale4")]
		[NotifyParentProperty(true)]
		[ValidateBound(-30.0, 30.0)]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
				}
				offset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeShape3")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeLength")]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				length = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeWidth7")]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				width = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeImage5")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeImageTransColor4")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeImageHueColor6")]
		[DefaultValue(typeof(Color), "")]
		public Color ImageHueColor
		{
			get
			{
				return imageHueColor;
			}
			set
			{
				imageHueColor = value;
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
			this.shape = shape;
			this.length = length;
			this.width = width;
		}
	}
}
