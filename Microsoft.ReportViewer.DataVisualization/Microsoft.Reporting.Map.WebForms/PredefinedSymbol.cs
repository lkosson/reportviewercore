using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(PredefinedSymbolConverter))]
	internal class PredefinedSymbol : NamedElement
	{
		private TextAlignment textAlignment = TextAlignment.Bottom;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.DarkGray;

		private MapDashStyle borderStyle = MapDashStyle.Solid;

		private int borderWidth = 1;

		private Color color = Color.Red;

		private Color textColor = Color.Black;

		private Color secondaryColor = Color.Empty;

		private GradientType gradientType;

		private MapHatchStyle hatchStyle;

		private string fromValue = string.Empty;

		private string toValue = "";

		private string legendText = "";

		private string text = "";

		private int shadowOffset;

		private int textShadowOffset;

		private MarkerStyle markerStyle = MarkerStyle.Circle;

		private float width = 7f;

		private float height = 7f;

		private ResizeMode imageResizeMode = ResizeMode.AutoFit;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private string category = string.Empty;

		private string toolTip = "";

		private bool visible = true;

		private ArrayList affectedSymbols;

		private string fromValueInt = string.Empty;

		private string toValueInt = "";

		private bool visibleInt = true;

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Name")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override object Tag
		{
			get
			{
				return base.Tag;
			}
			set
			{
				base.Tag = value;
			}
		}

		[SRCategory("CategoryAttribute_SymbolText")]
		[SRDescription("DescriptionAttributePredefinedSymbol_TextAlignment")]
		[DefaultValue(TextAlignment.Bottom)]
		public TextAlignment TextAlignment
		{
			get
			{
				return textAlignment;
			}
			set
			{
				textAlignment = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_SymbolText")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_BorderColor")]
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
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_BorderStyle")]
		[DefaultValue(MapDashStyle.Solid)]
		public MapDashStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{
				borderStyle = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_BorderWidth")]
		[DefaultValue(1)]
		public int BorderWidth
		{
			get
			{
				return borderWidth;
			}
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				borderWidth = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Color")]
		[DefaultValue(typeof(Color), "Red")]
		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_SymbolText")]
		[SRDescription("DescriptionAttributePredefinedSymbol_TextColor")]
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
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_SecondaryColor")]
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
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_GradientType")]
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
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_HatchStyle")]
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
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributePredefinedSymbol_FromValue")]
		[TypeConverter(typeof(StringConverter))]
		[DefaultValue("")]
		public string FromValue
		{
			get
			{
				return fromValue;
			}
			set
			{
				fromValue = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributePredefinedSymbol_ToValue")]
		[TypeConverter(typeof(StringConverter))]
		[DefaultValue("")]
		public string ToValue
		{
			get
			{
				return toValue;
			}
			set
			{
				toValue = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_LegendText")]
		[DefaultValue("")]
		public string LegendText
		{
			get
			{
				return legendText;
			}
			set
			{
				legendText = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Text")]
		[Localizable(true)]
		[TypeConverter(typeof(KeywordConverter))]
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
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_ShadowOffset")]
		[DefaultValue(0)]
		public int ShadowOffset
		{
			get
			{
				return shadowOffset;
			}
			set
			{
				if (value < -100 || value > 100)
				{
					throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
				}
				shadowOffset = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_SymbolText")]
		[SRDescription("DescriptionAttributePredefinedSymbol_TextShadowOffset")]
		[DefaultValue(0)]
		public int TextShadowOffset
		{
			get
			{
				return textShadowOffset;
			}
			set
			{
				if (value < -100 || value > 100)
				{
					throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
				}
				textShadowOffset = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_MarkerStyle")]
		[DefaultValue(MarkerStyle.Circle)]
		public MarkerStyle MarkerStyle
		{
			get
			{
				return markerStyle;
			}
			set
			{
				markerStyle = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Size")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Width")]
		[DefaultValue(7f)]
		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 1000.0));
				}
				width = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Size")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Height")]
		[DefaultValue(7f)]
		public float Height
		{
			get
			{
				return height;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 1000.0));
				}
				height = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributePredefinedSymbol_ImageResizeMode")]
		[DefaultValue(ResizeMode.AutoFit)]
		public ResizeMode ImageResizeMode
		{
			get
			{
				return imageResizeMode;
			}
			set
			{
				imageResizeMode = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Image")]
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
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributePredefinedSymbol_ImageTransColor")]
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
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Category")]
		[DefaultValue("")]
		public string Category
		{
			get
			{
				return category;
			}
			set
			{
				category = value;
				InvalidateRules();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributePredefinedSymbol_ToolTip")]
		[Localizable(true)]
		[TypeConverter(typeof(KeywordConverter))]
		[DefaultValue("")]
		public string ToolTip
		{
			get
			{
				return toolTip;
			}
			set
			{
				toolTip = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributePredefinedSymbol_Visible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public bool Visible
		{
			get
			{
				if (visible)
				{
					return VisibleInt;
				}
				return false;
			}
			set
			{
				visible = value;
				InvalidateRules();
			}
		}

		internal ArrayList AffectedSymbols
		{
			get
			{
				return affectedSymbols;
			}
			set
			{
				affectedSymbols = value;
			}
		}

		internal string FromValueInt
		{
			get
			{
				if (string.IsNullOrEmpty(fromValue))
				{
					return fromValueInt;
				}
				return fromValue;
			}
			set
			{
				fromValueInt = value;
			}
		}

		internal string ToValueInt
		{
			get
			{
				if (string.IsNullOrEmpty(toValue))
				{
					return toValueInt;
				}
				return toValue;
			}
			set
			{
				toValueInt = value;
			}
		}

		internal bool VisibleInt
		{
			get
			{
				return visibleInt;
			}
			set
			{
				visibleInt = value;
			}
		}

		public PredefinedSymbol()
			: this(null)
		{
		}

		internal PredefinedSymbol(CommonElements common)
			: base(common)
		{
			affectedSymbols = new ArrayList();
		}

		public override string ToString()
		{
			return Name;
		}

		public ArrayList GetAffectedSymbols()
		{
			return AffectedSymbols;
		}

		internal RuleBase GetRule()
		{
			return (RuleBase)ParentElement;
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			InvalidateRules();
		}

		internal override void OnRemove()
		{
			base.OnRemove();
			InvalidateRules();
		}

		internal void InvalidateRules()
		{
			MapCore mapCore = GetMapCore();
			if (mapCore != null)
			{
				mapCore.InvalidateRules();
				mapCore.Invalidate();
			}
		}

		internal MapCore GetMapCore()
		{
			return GetRule()?.GetMapCore();
		}
	}
}
