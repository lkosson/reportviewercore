using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class SwatchColor : NamedElement
	{
		private Color color = Color.White;

		private GradientType gradientType;

		private Color secondaryColor = Color.Empty;

		private MapHatchStyle hatchStyle;

		private bool noData;

		private double fromValue;

		private double toValue;

		private string textValue = "";

		internal bool automaticallyAdded;

		private ColorSwatchPanel owner;

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSwatchColor_Color")]
		[DefaultValue(typeof(Color), "White")]
		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
				Invalidate(layout: false);
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSwatchColor_GradientType")]
		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.None)]
		public GradientType GradientType
		{
			get
			{
				return gradientType;
			}
			set
			{
				gradientType = value;
				Invalidate(layout: false);
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSwatchColor_SecondaryColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		public Color SecondaryColor
		{
			get
			{
				return secondaryColor;
			}
			set
			{
				secondaryColor = value;
				Invalidate(layout: false);
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeSwatchColor_HatchStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(MapHatchStyle.None)]
		public MapHatchStyle HatchStyle
		{
			get
			{
				return hatchStyle;
			}
			set
			{
				hatchStyle = value;
				Invalidate(layout: false);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSwatchColor_NoData")]
		[DefaultValue(false)]
		public bool NoData
		{
			get
			{
				return noData;
			}
			set
			{
				noData = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSwatchColor_FromValue")]
		[DefaultValue(0.0)]
		public double FromValue
		{
			get
			{
				return fromValue;
			}
			set
			{
				fromValue = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSwatchColor_ToValue")]
		[DefaultValue(0.0)]
		public double ToValue
		{
			get
			{
				return toValue;
			}
			set
			{
				toValue = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSwatchColor_TextValue")]
		[DefaultValue("")]
		public string TextValue
		{
			get
			{
				return textValue;
			}
			set
			{
				textValue = value;
				Invalidate(layout: true);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasTextValue => !string.IsNullOrEmpty(TextValue);

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeSwatchColor_AutomaticallyAdded")]
		[DefaultValue(false)]
		public bool AutomaticallyAdded
		{
			get
			{
				return automaticallyAdded;
			}
			set
			{
				automaticallyAdded = value;
			}
		}

		internal ColorSwatchPanel Owner
		{
			get
			{
				return owner;
			}
			set
			{
				owner = value;
			}
		}

		public SwatchColor()
			: this(null, "", 0.0, 0.0, "")
		{
		}

		public SwatchColor(string name)
			: this(null, name, 0.0, 0.0, "")
		{
		}

		public SwatchColor(string name, double fromValue, double toValue)
			: this(null, name, fromValue, toValue, "")
		{
		}

		public SwatchColor(string name, string textValue)
			: this(null, name, 0.0, 0.0, textValue)
		{
		}

		public SwatchColor(string name, double fromValue, double toValue, string textValue)
			: this(null, name, fromValue, toValue, textValue)
		{
		}

		internal SwatchColor(CommonElements common, string name, double fromValue, double toValue, string textValue)
			: base(common)
		{
			Name = name;
			this.fromValue = fromValue;
			this.toValue = toValue;
			this.textValue = textValue;
		}

		private void Invalidate(bool layout)
		{
			if (layout)
			{
				base.InvalidateAndLayout();
			}
			else
			{
				base.Invalidate();
			}
		}
	}
}
