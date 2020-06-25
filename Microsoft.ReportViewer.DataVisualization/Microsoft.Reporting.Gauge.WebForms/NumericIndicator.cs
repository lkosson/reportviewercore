using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Timers;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(NumericIndicatorConverter))]
	internal class NumericIndicator : NamedElement, IRenderable, IToolTipProvider, IPointerProvider, ISelectable, IImageMapProvider
	{
		private double numberPosition = double.NaN;

		private DataAttributes data;

		private bool refreshPending;

		private double pendingNumberPosition;

		private SegmentsCache segmentsCache = new SegmentsCache();

		private NumericRangeCollection ranges;

		private NamedElement parentSystem;

		private string parent = string.Empty;

		private int zOrder;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private NumericIndicatorStyle style = NumericIndicatorStyle.Mechanical;

		private int digits = 6;

		private int decimals = 1;

		private FontUnit fontUnit;

		private ResizeMode resizeMode = ResizeMode.AutoFit;

		private float shadowOffset;

		private string formatString = string.Empty;

		private bool showDecimal;

		private bool showLeadingZeros = true;

		private float refreshRate = 10f;

		private string offString = "-";

		private string outOfRangeString = "Error";

		private ShowSign showSign = ShowSign.NegativeOnly;

		private double minimum = double.NegativeInfinity;

		private double maximum = double.PositiveInfinity;

		private double multiplier = 1.0;

		private bool snappingEnabled;

		private double snappingInterval;

		private bool dampeningEnabled;

		private double dampeningSweepTime = 1.0;

		private GaugeLocation location;

		private GaugeSize size;

		private bool visible = true;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.DimGray;

		private GaugeDashStyle borderStyle = GaugeDashStyle.Solid;

		private int borderWidth = 1;

		private Color separatorColor = Color.DimGray;

		private float separatorWidth = 1f;

		private Color backColor = Color.DimGray;

		private GradientType backGradientType = GradientType.HorizontalCenter;

		private Color backGradientEndColor = Color.White;

		private GaugeHatchStyle backHatchStyle;

		private Color digitColor = Color.SteelBlue;

		private Color decimalColor = Color.Firebrick;

		private Color ledDimColor = Color.Empty;

		private bool selected;

		private bool defaultParent = true;

		private object imageMapProviderTag;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NumericRangeCollection Ranges => ranges;

		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeParentObject3")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public NamedElement ParentObject => parentSystem;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeNumericIndicator_Parent")]
		[TypeConverter(typeof(ParentSourceConverter))]
		[NotifyParentProperty(true)]
		public string Parent
		{
			get
			{
				return parent;
			}
			set
			{
				string text = parent;
				if (value == "(none)")
				{
					value = string.Empty;
				}
				parent = value;
				try
				{
					ConnectToParent(exact: true);
				}
				catch
				{
					parent = text;
					throw;
				}
				DefaultParent = false;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeNumericIndicator_ZOrder")]
		[DefaultValue(0)]
		public int ZOrder
		{
			get
			{
				return zOrder;
			}
			set
			{
				zOrder = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_ToolTip")]
		[Localizable(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
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
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_Href")]
		[Localizable(true)]
		[Browsable(false)]
		[DefaultValue("")]
		public string Href
		{
			get
			{
				return href;
			}
			set
			{
				href = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_MapAreaAttributes")]
		[DefaultValue("")]
		public string MapAreaAttributes
		{
			get
			{
				return mapAreaAttributes;
			}
			set
			{
				mapAreaAttributes = value;
			}
		}

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeNumericIndicator_Name")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryStyleSpecific")]
		[SRDescription("DescriptionAttributeNumericIndicator_Style")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ParenthesizePropertyName(true)]
		[DefaultValue(NumericIndicatorStyle.Mechanical)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Obsolete("This property is obsolete in Dundas Gauge 2.0. IndicatorStyle is supposed to be used instead.")]
		public NumericIndicatorStyle Style
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryStyleSpecific")]
		[SRDescription("DescriptionAttributeNumericIndicator_Style")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(NumericIndicatorStyle.Mechanical)]
		public NumericIndicatorStyle IndicatorStyle
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_Digits")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(6)]
		public int Digits
		{
			get
			{
				return digits;
			}
			set
			{
				if (value < Decimals && Common != null && initialized)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionDigitsDecimals"));
				}
				if (value < 0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfRangeMinClose", 0));
				}
				digits = value;
				RefreshIndicator();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_Decimals")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(1)]
		public int Decimals
		{
			get
			{
				return decimals;
			}
			set
			{
				if (value > Digits && Common != null && initialized)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionDecimalsDigitsDrror"));
				}
				if (value < 0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionPropertyNegative", "Decimals"));
				}
				decimals = value;
				RefreshIndicator();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_FontUnit")]
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

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeResizeMode")]
		[DefaultValue(ResizeMode.AutoFit)]
		public ResizeMode ResizeMode
		{
			get
			{
				return resizeMode;
			}
			set
			{
				resizeMode = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeShadowOffset")]
		[ValidateBound(-5.0, 5.0)]
		[DefaultValue(0f)]
		public float ShadowOffset
		{
			get
			{
				return shadowOffset;
			}
			set
			{
				if (value < -100f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
				}
				shadowOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_FormatString")]
		[Localizable(true)]
		[DefaultValue("")]
		public string FormatString
		{
			get
			{
				return formatString;
			}
			set
			{
				formatString = value;
				RefreshIndicator();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_ShowDecimalPoint")]
		[DefaultValue(false)]
		public bool ShowDecimalPoint
		{
			get
			{
				return showDecimal;
			}
			set
			{
				showDecimal = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_ShowLeadingZeros")]
		[DefaultValue(true)]
		public bool ShowLeadingZeros
		{
			get
			{
				return showLeadingZeros;
			}
			set
			{
				showLeadingZeros = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_RefreshRate")]
		[DefaultValue(10f)]
		public float RefreshRate
		{
			get
			{
				return refreshRate;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
				}
				refreshRate = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_OffString")]
		[Localizable(true)]
		[DefaultValue("-")]
		public string OffString
		{
			get
			{
				return offString;
			}
			set
			{
				offString = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_OutOfRangeString")]
		[Localizable(true)]
		[DefaultValue("Error")]
		public string OutOfRangeString
		{
			get
			{
				return outOfRangeString;
			}
			set
			{
				outOfRangeString = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_ShowSign")]
		[DefaultValue(ShowSign.NegativeOnly)]
		public ShowSign ShowSign
		{
			get
			{
				return showSign;
			}
			set
			{
				showSign = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_Minimum")]
		[DoubleConverterHint(double.NegativeInfinity)]
		[TypeConverter(typeof(DoubleInfinityConverter))]
		[DefaultValue(double.NegativeInfinity)]
		public double Minimum
		{
			get
			{
				return minimum;
			}
			set
			{
				minimum = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_Maximum")]
		[DoubleConverterHint(double.PositiveInfinity)]
		[TypeConverter(typeof(DoubleInfinityConverter))]
		[DefaultValue(double.PositiveInfinity)]
		public double Maximum
		{
			get
			{
				return maximum;
			}
			set
			{
				maximum = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_Multiplier")]
		[DefaultValue(1.0)]
		public double Multiplier
		{
			get
			{
				return multiplier;
			}
			set
			{
				multiplier = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_SnappingEnabled")]
		[DefaultValue(false)]
		public bool SnappingEnabled
		{
			get
			{
				return snappingEnabled;
			}
			set
			{
				snappingEnabled = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_SnappingInterval")]
		[DefaultValue(0.0)]
		public double SnappingInterval
		{
			get
			{
				return snappingInterval;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionPropertyNegative", "SnappingInterval"));
				}
				snappingInterval = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeNumericIndicator_DampeningEnabled")]
		[DefaultValue(false)]
		public bool DampeningEnabled
		{
			get
			{
				return dampeningEnabled;
			}
			set
			{
				dampeningEnabled = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeDampeningSweepTime")]
		[DefaultValue(1.0)]
		public double DampeningSweepTime
		{
			get
			{
				return dampeningSweepTime;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionPropertyNegative", "DampeningSweepTime"));
				}
				dampeningSweepTime = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeNumericIndicator_Location")]
		[TypeConverter(typeof(LocationConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ValidateBound(100.0, 100.0)]
		public GaugeLocation Location
		{
			get
			{
				return location;
			}
			set
			{
				location = value;
				location.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryPosition")]
		[SRDescription("DescriptionAttributeNumericIndicator_Size")]
		[TypeConverter(typeof(SizeConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ValidateBound(100.0, 100.0)]
		public GaugeSize Size
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
				size.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_Visible")]
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
		[SRDescription("DescriptionAttributeNumericIndicator_Font")]
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
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_BorderColor")]
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
		[SRDescription("DescriptionAttributeNumericIndicator_BorderStyle")]
		[NotifyParentProperty(true)]
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
		[SRDescription("DescriptionAttributeNumericIndicator_BorderWidth")]
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
				if (value < 0 || value > 100)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				borderWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryStyleSpecific")]
		[SRDescription("DescriptionAttributeNumericIndicator_SeparatorColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DimGray")]
		public Color SeparatorColor
		{
			get
			{
				return separatorColor;
			}
			set
			{
				separatorColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryStyleSpecific")]
		[SRDescription("DescriptionAttributeNumericIndicator_SeparatorWidth")]
		[NotifyParentProperty(true)]
		[DefaultValue(1f)]
		public float SeparatorWidth
		{
			get
			{
				return separatorWidth;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				separatorWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_BackColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DimGray")]
		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_BackGradientType")]
		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.HorizontalCenter)]
		public GradientType BackGradientType
		{
			get
			{
				return backGradientType;
			}
			set
			{
				backGradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_BackGradientEndColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "White")]
		public Color BackGradientEndColor
		{
			get
			{
				return backGradientEndColor;
			}
			set
			{
				backGradientEndColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_BackHatchStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(GaugeHatchStyle.None)]
		public GaugeHatchStyle BackHatchStyle
		{
			get
			{
				return backHatchStyle;
			}
			set
			{
				backHatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_DigitColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "SteelBlue")]
		public Color DigitColor
		{
			get
			{
				return digitColor;
			}
			set
			{
				digitColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_DecimalColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Firebrick")]
		public Color DecimalColor
		{
			get
			{
				return decimalColor;
			}
			set
			{
				decimalColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryStyleSpecific")]
		[SRDescription("DescriptionAttributeNumericIndicator_LedDimColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Empty")]
		public Color LedDimColor
		{
			get
			{
				return ledDimColor;
			}
			set
			{
				ledDimColor = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeNumericIndicator_Value")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(double.NaN)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public double Value
		{
			get
			{
				return data.Value;
			}
			set
			{
				if (data.Value != value)
				{
					data.Value = value;
					data.ValueSource = string.Empty;
				}
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeValueSource")]
		[TypeConverter(typeof(ValueSourceConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		public string ValueSource
		{
			get
			{
				return data.ValueSource;
			}
			set
			{
				data.ValueSource = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeNumericIndicator_Selected")]
		[Browsable(false)]
		[DefaultValue(false)]
		public bool Selected
		{
			get
			{
				return selected;
			}
			set
			{
				selected = value;
				Invalidate();
			}
		}

		internal Position Position => new Position(Location, Size, ContentAlignment.TopLeft);

		internal double NumberPosition
		{
			get
			{
				return numberPosition;
			}
			set
			{
				numberPosition = value;
				pendingNumberPosition = value;
				Refresh();
			}
		}

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				data.Common = value;
				ranges.Common = value;
			}
		}

		internal DataAttributes Data => data;

		internal bool DefaultParent
		{
			get
			{
				return defaultParent;
			}
			set
			{
				defaultParent = value;
			}
		}

		object IImageMapProvider.Tag
		{
			get
			{
				return imageMapProviderTag;
			}
			set
			{
				imageMapProviderTag = value;
			}
		}

		double IPointerProvider.Position
		{
			get
			{
				return NumberPosition;
			}
			set
			{
				NumberPosition = value;
			}
		}

		public NumericIndicator()
		{
			location = new GaugeLocation(this, 35f, 30f);
			size = new GaugeSize(this, 30f, 10f);
			location.DefaultValues = true;
			size.DefaultValues = true;
			ranges = new NumericRangeCollection(this, common);
			data = new DataAttributes(this);
		}

		protected bool ShouldSerializeStyle()
		{
			return false;
		}

		public override string ToString()
		{
			return Name;
		}

		private Color GetRangeColor(Color color, bool decimalColor)
		{
			foreach (NumericRange range in ranges)
			{
				double num = Math.Min(range.StartValue, range.EndValue);
				double num2 = Math.Max(range.StartValue, range.EndValue);
				if (numberPosition >= num && numberPosition <= num2)
				{
					return decimalColor ? range.DecimalColor : range.DigitColor;
				}
			}
			return color;
		}

		private Brush GetFontBrush(GaugeGraphics g, Color color)
		{
			if (BackGradientType != GradientType.HorizontalCenter || IndicatorStyle != NumericIndicatorStyle.Mechanical)
			{
				return new SolidBrush(color);
			}
			HSV hSV = ColorHandler.ColorToHSV(backGradientEndColor);
			HSV hSV2 = ColorHandler.ColorToHSV(backColor);
			HSV hsv = ColorHandler.ColorToHSV(color);
			HSV hsv2 = ColorHandler.ColorToHSV(color);
			hsv.value = Math.Min(Math.Max(hsv.value - (hSV.value - hSV2.value), 0), 255);
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			absoluteRectangle.Inflate(2f, 2f);
			Color color2 = ColorHandler.HSVtoColor(hsv);
			Color color3 = ColorHandler.HSVtoColor(hsv2);
			color2 = Color.FromArgb(color.A, color2.R, color2.G, color2.B);
			color3 = Color.FromArgb(color.A, color3.R, color3.G, color3.B);
			return g.GetGradientBrush(absoluteRectangle, color2, color3, backGradientType);
		}

		private double GetNumber()
		{
			return numberPosition * Multiplier;
		}

		private double GetNumber(double number)
		{
			return number * Multiplier;
		}

		private string GetDefaultFormat()
		{
			string text = "F0";
			if (formatString != string.Empty)
			{
				text = formatString;
				if (formatString.IndexOf(";", StringComparison.Ordinal) == -1 && ShowSign == ShowSign.None && !IsStandardFormat())
				{
					text = text + ";" + text;
				}
			}
			else
			{
				int num = Digits - Decimals;
				string text2 = "000000000000000000000000000000000000000".Substring(0, num % 40);
				for (int i = 0; i < num / 40; i++)
				{
					text2 += "000000000000000000000000000000000000000";
				}
				if (Decimals > 0)
				{
					text2 += ".";
					text2 += "000000000000000000000000000000000000000".Substring(0, Decimals % 40);
					for (int j = 0; j < Decimals / 40; j++)
					{
						text2 += "000000000000000000000000000000000000000";
					}
				}
				string text3 = text2;
				if (ShowSign == ShowSign.Both)
				{
					text2 = ((CultureInfo.CurrentCulture.NumberFormat.NumberNegativePattern <= 2) ? (CultureInfo.CurrentCulture.NumberFormat.PositiveSign + text2) : (text2 + CultureInfo.CurrentCulture.NumberFormat.PositiveSign));
				}
				if (ShowSign != 0)
				{
					text3 = ((CultureInfo.CurrentCulture.NumberFormat.NumberNegativePattern <= 2) ? (CultureInfo.CurrentCulture.NumberFormat.NegativeSign + text3) : (text3 + CultureInfo.CurrentCulture.NumberFormat.NegativeSign));
				}
				text = text2 + ";" + text3;
			}
			return text;
		}

		private string GetLabel(ref bool digitsPrinted)
		{
			digitsPrinted = false;
			string text = "";
			if (double.IsNaN(numberPosition))
			{
				text = offString;
			}
			else if (numberPosition > maximum || numberPosition < minimum)
			{
				text = outOfRangeString;
			}
			else
			{
				string defaultFormat = GetDefaultFormat();
				text = GetNumber().ToString(defaultFormat, CultureInfo.CurrentCulture);
				int num = Digits;
				string numberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
				if (text.IndexOf(numberDecimalSeparator, StringComparison.CurrentCulture) != -1)
				{
					num++;
				}
				switch (ShowSign)
				{
				case ShowSign.NegativeOnly:
					if (GetNumber() < 0.0)
					{
						num++;
					}
					break;
				case ShowSign.Both:
					num++;
					break;
				}
				if (text.Length > num && formatString == string.Empty)
				{
					text = outOfRangeString;
				}
				else
				{
					digitsPrinted = true;
				}
			}
			return text;
		}

		private void DrawSymbol(GaugeGraphics g, string symbol, RectangleF rect, Font font, Brush brush, StringFormat format, bool decDot, bool comma)
		{
			PointF pointF = rect.Location;
			pointF.X += rect.Width / 2f;
			pointF.Y += rect.Height / 2f;
			if (style == NumericIndicatorStyle.Mechanical)
			{
				pointF.Y += rect.Height / 20f;
				if (g.TextRenderingHint == TextRenderingHint.ClearTypeGridFit)
				{
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddString(symbol, font.FontFamily, (int)font.Style, font.SizeInPoints * 1.4f, pointF, format);
						g.FillPath(brush, graphicsPath);
					}
				}
				else
				{
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Far;
					stringFormat.LineAlignment = StringAlignment.Far;
					g.DrawString(symbol, font, brush, rect, stringFormat);
				}
			}
			else if (style == NumericIndicatorStyle.Digital7Segment)
			{
				float num = (float)((double)rect.Height * 0.65);
				if (symbol == string.Empty)
				{
					if (!(LedDimColor != Color.Empty))
					{
						return;
					}
					using (Brush brush2 = GetFontBrush(g, LedDimColor))
					{
						using (GraphicsPath path = DigitalSegment.GetOrientedSegments(LEDSegment7.All, pointF, num, segmentsCache))
						{
							g.FillPath(brush2, path);
						}
					}
				}
				else
				{
					using (GraphicsPath path2 = DigitalSegment.GetSymbol7(symbol[0], pointF, num, decDot, comma, sepDots: false, segmentsCache))
					{
						g.FillPath(brush, path2);
					}
				}
			}
			else
			{
				if (style != NumericIndicatorStyle.Digital14Segment)
				{
					return;
				}
				float num2 = (float)((double)rect.Height * 0.65);
				symbol = symbol.ToUpper(CultureInfo.InvariantCulture);
				if (symbol == string.Empty)
				{
					if (!(LedDimColor != Color.Empty))
					{
						return;
					}
					using (Brush brush3 = GetFontBrush(g, LedDimColor))
					{
						using (GraphicsPath path3 = DigitalSegment.GetOrientedSegments(LEDSegment14.All, pointF, num2, segmentsCache))
						{
							g.FillPath(brush3, path3);
						}
					}
				}
				else
				{
					using (GraphicsPath path4 = DigitalSegment.GetSymbol14(symbol[0], pointF, num2, decDot, comma, sepDots: false, segmentsCache))
					{
						g.FillPath(brush, path4);
					}
				}
			}
		}

		private void DrawSeparator(GaugeGraphics g, Brush brush, float digitsCount, float rectPosition, RectangleF gaugeRect, float separatorWidth)
		{
			float x = gaugeRect.X + gaugeRect.Width / digitsCount * rectPosition;
			RectangleF rect = new RectangleF(x, gaugeRect.Y, 0f, gaugeRect.Height);
			rect.Inflate(separatorWidth / 2f, 0f);
			using (new GraphicsPath())
			{
				g.FillRectangle(brush, rect);
			}
		}

		private bool IsStandardFormat()
		{
			if (formatString != string.Empty && formatString.Length < 4)
			{
				if (FormatString.Length == 1)
				{
					return true;
				}
				if ("CDEFGNPRX".IndexOf(formatString.ToUpper(CultureInfo.InvariantCulture)[0]) != -1)
				{
					bool result = false;
					for (int i = 1; i < formatString.Length; i++)
					{
						if (!char.IsDigit(formatString[i]))
						{
							return false;
						}
						result = true;
					}
					return result;
				}
			}
			return false;
		}

		private void RenderIndicator(GaugeGraphics g)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			bool digitsPrinted = false;
			string label = GetLabel(ref digitsPrinted);
			string format = GetDefaultFormat().Replace("#", "0");
			string text;
			string text2;
			if (IsStandardFormat())
			{
				double num = Math.Pow(10.0, Digits - Decimals - 1);
				text = GetNumber(Math.Min(num, Maximum)).ToString(format, CultureInfo.CurrentCulture);
				text2 = ((ShowSign == ShowSign.None) ? text : GetNumber(Math.Max(0.0 - num, Minimum)).ToString(format, CultureInfo.CurrentCulture));
			}
			else
			{
				text2 = GetNumber(1.0).ToString(format, CultureInfo.CurrentCulture);
				text = GetNumber(-1.0).ToString(format, CultureInfo.CurrentCulture);
			}
			int num2 = Math.Max(text2.Length, text.Length);
			GetNumber();
			string numberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			bool flag = showDecimal & (style == NumericIndicatorStyle.Mechanical);
			if (!flag && (text2.IndexOf(numberDecimalSeparator, StringComparison.CurrentCulture) != -1 || text.IndexOf(numberDecimalSeparator, StringComparison.CurrentCulture) != -1))
			{
				num2--;
			}
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			SizeF insetSize = g.MeasureString(SR.DefaultMeazureStringSimbol, this.font);
			RectangleF rectangleF = absoluteRectangle;
			rectangleF.Width /= num2;
			RectangleF boundRect = rectangleF;
			float num3 = g.GetAbsoluteSize(new SizeF(SeparatorWidth, 0f)).Width;
			if (num3 > boundRect.Width - boundRect.Width / 10f)
			{
				num3 = boundRect.Width - boundRect.Width / 10f;
			}
			boundRect.Width -= num3;
			RectangleF rectangleF2 = Utils.NormalizeRectangle(boundRect, insetSize, ResizeMode == ResizeMode.AutoFit);
			Font font;
			if (ResizeMode != ResizeMode.AutoFit)
			{
				font = ((FontUnit != FontUnit.Default) ? ((Font)this.font.Clone()) : ((Font)this.font.Clone()));
			}
			else
			{
				float num4 = this.font.Size * (rectangleF2.Height / insetSize.Height);
				if (num4 <= 0f)
				{
					return;
				}
				font = new Font(this.font.Name, num4, this.font.Style, this.font.Unit);
			}
			Brush fontBrush = GetFontBrush(g, GetRangeColor(digitColor, decimalColor: false));
			Brush fontBrush2 = GetFontBrush(g, GetRangeColor(decimalColor, decimalColor: true));
			Brush fontBrush3 = GetFontBrush(g, separatorColor);
			float num5 = num2;
			try
			{
				string[] array = new string[2]
				{
					string.Empty,
					string.Empty
				};
				if (digitsPrinted && label.IndexOf(numberDecimalSeparator, StringComparison.CurrentCulture) != -1)
				{
					int num6 = label.IndexOf(numberDecimalSeparator, StringComparison.CurrentCulture);
					array[1] = label.Substring(num6 + numberDecimalSeparator.Length);
					if (!flag)
					{
						array[0] = label.Substring(0, num6);
					}
					else
					{
						array[0] = label.Substring(0, num6 + numberDecimalSeparator.Length);
					}
				}
				else
				{
					array[0] = label.Substring(0, Math.Min(label.Length, num2));
				}
				if (array[0].Length + array[1].Length > num2)
				{
					array[0] = outOfRangeString.Substring(0, Math.Min(outOfRangeString.Length, num2));
					array[1] = "";
				}
				if (style == NumericIndicatorStyle.Mechanical)
				{
					if (separatorWidth > 0f)
					{
						for (int i = 1; i < num2; i++)
						{
							DrawSeparator(g, fontBrush3, num2, i, absoluteRectangle, num3);
						}
					}
				}
				else if (style == NumericIndicatorStyle.Digital7Segment || style == NumericIndicatorStyle.Digital14Segment)
				{
					num5 = num2;
					for (int j = 0; j < num2; j++)
					{
						rectangleF.X = absoluteRectangle.X + (num5 -= 1f) * rectangleF.Width;
						rectangleF2 = Utils.NormalizeRectangle(rectangleF, insetSize, ResizeMode == ResizeMode.AutoFit);
						DrawSymbol(g, string.Empty, rectangleF2, font, fontBrush2, stringFormat, decDot: false, comma: false);
					}
				}
				num5 = num2;
				if (array[1].Length > 0)
				{
					for (int num7 = array[1].Length - 1; num7 >= 0; num7--)
					{
						rectangleF.X = absoluteRectangle.X + (num5 -= 1f) * rectangleF.Width;
						rectangleF2 = Utils.NormalizeRectangle(rectangleF, insetSize, ResizeMode == ResizeMode.AutoFit);
						DrawSymbol(g, array[1].Substring(num7, 1), rectangleF2, font, fontBrush2, stringFormat, decDot: false, comma: false);
					}
				}
				if (array[0].Length <= 0)
				{
					return;
				}
				bool decDot = array[1].Length > 0 && ShowDecimalPoint;
				bool comma = false;
				string text3 = "";
				if (!ShowLeadingZeros)
				{
					bool flag2 = true;
					for (int k = 0; k < array[0].Length; k++)
					{
						char c = array[0][k];
						if (char.IsDigit(c) && flag2)
						{
							if (c == '0' && k != array[0].Length - 1)
							{
								c = ' ';
							}
							else
							{
								flag2 = false;
							}
						}
						text3 += c;
					}
				}
				else
				{
					text3 = array[0];
				}
				for (int num8 = text3.Length - 1; num8 >= 0; num8--)
				{
					string a = text3.Substring(num8, 1);
					if (IndicatorStyle != NumericIndicatorStyle.Mechanical && (a == "." || a == ",") && num8 > 0)
					{
						num8--;
						decDot = (a == ".");
						comma = (a == ",");
						a = text3.Substring(num8, 1);
					}
					rectangleF.X = absoluteRectangle.X + (num5 -= 1f) * rectangleF.Width;
					rectangleF2 = Utils.NormalizeRectangle(rectangleF, insetSize, ResizeMode == ResizeMode.AutoFit);
					DrawSymbol(g, text3.Substring(num8, 1), rectangleF2, font, fontBrush, stringFormat, decDot, comma);
					decDot = false;
					comma = false;
				}
				for (int num9 = text3.Length - 1; num9 >= 0; num9--)
				{
				}
			}
			finally
			{
				font.Dispose();
				fontBrush.Dispose();
				fontBrush2.Dispose();
			}
		}

		private void RenderBackground(GaugeGraphics g)
		{
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			if (ShadowOffset != 0f)
			{
				RectangleF rect = absoluteRectangle;
				if (BorderWidth > 0)
				{
					rect.Inflate(BorderWidth, BorderWidth);
				}
				rect.Offset(ShadowOffset, ShadowOffset);
				using (Brush brush = g.GetShadowBrush())
				{
					g.FillRectangle(brush, rect);
				}
			}
			if (BackColor != Color.Empty)
			{
				using (Brush brush2 = g.CreateBrush(absoluteRectangle, BackColor, BackHatchStyle, "", GaugeImageWrapMode.Unscaled, Color.Empty, GaugeImageAlign.Center, BackGradientType, BackGradientEndColor))
				{
					g.FillRectangle(brush2, absoluteRectangle);
				}
			}
			if (BorderWidth <= 0)
			{
				return;
			}
			using (Pen pen = new Pen(BorderColor, BorderWidth))
			{
				absoluteRectangle.Inflate(BorderWidth / 2, BorderWidth / 2);
				if (BorderStyle != 0)
				{
					pen.DashStyle = g.GetPenStyle(BorderStyle);
				}
				g.DrawRectangle(pen, absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height);
			}
		}

		private void ConnectToParent(bool exact)
		{
			if (Common != null && !Common.GaugeCore.isInitializing)
			{
				if (parent == string.Empty)
				{
					parentSystem = null;
					return;
				}
				Common.ObjectLinker.IsParentElementValid(this, this, exact);
				parentSystem = Common.ObjectLinker.GetElement(parent);
			}
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			switch (msg)
			{
			case MessageType.NamedElementRemove:
				if (parentSystem == element)
				{
					Parent = string.Empty;
				}
				break;
			case MessageType.NamedElementRename:
				if (parentSystem == element)
				{
					parent = element.GetNameAsParent((string)param);
				}
				break;
			case MessageType.PrepareSnapShot:
				refreshTimer_Elapsed(this, null);
				break;
			}
			Data.Notify(msg, element, param);
			ranges.Notify(msg, element, param);
		}

		internal override void OnAdded()
		{
			RefreshRate = RefreshRate;
			base.OnAdded();
			ConnectToParent(exact: true);
			data.ReconnectData(exact: true);
			refreshTimer_Elapsed(this, null);
			Decimals = Decimals;
			Digits = Digits;
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			data.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			ConnectToParent(exact: true);
			data.EndInit();
			refreshTimer_Elapsed(this, null);
			Decimals = Decimals;
			Digits = Digits;
		}

		internal override void ReconnectData(bool exact)
		{
			base.ReconnectData(exact);
			data.ReconnectData(exact);
		}

		protected override void OnDispose()
		{
			ranges.Dispose();
			data.Dispose();
			segmentsCache.Reset();
			base.OnDispose();
		}

		internal override void Invalidate()
		{
			segmentsCache.Reset();
			base.Invalidate();
		}

		internal virtual void PointerValueChanged()
		{
			if (Common == null || double.IsNaN(Data.OldValue))
			{
				return;
			}
			bool playbackMode = false;
			if (((IValueConsumer)Data).GetProvider() != null)
			{
				playbackMode = ((IValueConsumer)Data).GetProvider().GetPlayBackMode();
			}
			if ((Data.OldValue >= minimum && Data.Value < minimum) || (Data.OldValue <= maximum && Data.Value > maximum))
			{
				Common.GaugeContainer.OnValueScaleLeave(this, new ValueRangeEventArgs(Data.Value, Data.DateValueStamp, Name, playbackMode, this));
			}
			if ((Data.OldValue < minimum && Data.Value >= minimum) || (Data.OldValue > maximum && Data.Value <= maximum))
			{
				Common.GaugeContainer.OnValueScaleEnter(this, new ValueRangeEventArgs(Data.Value, Data.DateValueStamp, Name, playbackMode, this));
			}
			foreach (Range range in Ranges)
			{
				range.PointerValueChanged(Data);
			}
		}

		private void refreshTimer_Elapsed(object source, ElapsedEventArgs e)
		{
			if (refreshPending)
			{
				refreshPending = false;
				numberPosition = pendingNumberPosition;
				base.Refresh();
			}
		}

		private void RefreshIndicator()
		{
			segmentsCache.Reset();
			Refresh();
		}

		private double GetSnapValue(double value)
		{
			if (snappingEnabled && snappingInterval > 0.0)
			{
				return snappingInterval * Math.Round(value / snappingInterval);
			}
			return value;
		}

		void IRenderable.RenderStaticElements(GaugeGraphics g)
		{
			if (Visible)
			{
				_ = Position.Rectangle;
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddRectangle(g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)));
				Common.GaugeCore.HotRegionList.SetHotRegion(this, graphicsPath);
			}
		}

		void IRenderable.RenderDynamicElements(GaugeGraphics g)
		{
			if (Visible)
			{
				g.StartHotRegion(this);
				RenderBackground(g);
				RenderIndicator(g);
				g.EndHotRegion();
			}
		}

		int IRenderable.GetZOrder()
		{
			return ZOrder;
		}

		RectangleF IRenderable.GetBoundRect(GaugeGraphics g)
		{
			return Position.Rectangle;
		}

		object IRenderable.GetParentRenderable()
		{
			return ParentObject;
		}

		string IRenderable.GetParentRenderableName()
		{
			return parent;
		}

		string IToolTipProvider.GetToolTip(HitTestResult ht)
		{
			if (Common != null && Common.GaugeCore != null)
			{
				return Common.GaugeCore.ResolveAllKeywords(ToolTip, this);
			}
			return ToolTip;
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip((HitTestResult)null);
		}

		string IImageMapProvider.GetHref()
		{
			if (Common != null && Common.GaugeCore != null)
			{
				return Common.GaugeCore.ResolveAllKeywords(Href, this);
			}
			return Href;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (Common != null && Common.GaugeCore != null)
			{
				return Common.GaugeCore.ResolveAllKeywords(MapAreaAttributes, this);
			}
			return MapAreaAttributes;
		}

		void IPointerProvider.DataValueChanged(bool initialize)
		{
			PointerValueChanged();
			double num = NumberPosition = GetSnapValue(Data.Value);
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			Stack stack = new Stack();
			for (NamedElement namedElement = ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
			{
				stack.Push(namedElement);
			}
			foreach (IRenderable item in stack)
			{
				g.CreateDrawRegion(item.GetBoundRect(g));
			}
			g.CreateDrawRegion(((IRenderable)this).GetBoundRect(g));
			g.DrawSelection(g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)), designTimeSelection, Common.GaugeCore.SelectionBorderColor, Common.GaugeCore.SelectionMarkerColor);
			g.RestoreDrawRegion();
			foreach (IRenderable item2 in stack)
			{
				_ = item2;
				g.RestoreDrawRegion();
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			NumericIndicator numericIndicator = new NumericIndicator();
			binaryFormatSerializer.Deserialize(numericIndicator, stream);
			return numericIndicator;
		}
	}
}
