using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(ShapeRuleConverter))]
	internal class ShapeRule : RuleBase
	{
		private CustomColorCollection customColors;

		private string shapeField = "(Name)";

		private string category = string.Empty;

		private string showInLegend = "(none)";

		private bool showInColorSwatch;

		private string legendText = "#FROMVALUE{N0} - #TOVALUE{N0}";

		private int colorCount = 5;

		private Color fromColor = Color.Green;

		private Color middleColor = Color.Yellow;

		private Color toColor = Color.Red;

		private string fromValue = "";

		private string toValue = "";

		private bool useCustomColors;

		private ColoringMode coloringMode = ColoringMode.DistinctColors;

		private DataGrouping dataGrouping = DataGrouping.Optimal;

		private MapColorPalette colorPalette;

		private Color borderColor = Color.DarkGray;

		private Color secondaryColor = Color.Empty;

		private GradientType gradientType;

		private MapHatchStyle hatchStyle;

		private string text = "#NAME";

		private string toolTip = "";

		[SRCategory("CategoryAttribute_CustomColors")]
		[SRDescription("DescriptionAttributeShapeRule_CustomColors")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CustomColorCollection CustomColors => customColors;

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeShapeRule_Name")]
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

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeShapeRule_ShapeField")]
		[TypeConverter(typeof(DesignTimeFieldConverter))]
		[ParenthesizePropertyName(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("(Name)")]
		public string ShapeField
		{
			get
			{
				return shapeField;
			}
			set
			{
				shapeField = value;
				InvalidateRules();
			}
		}

		internal override string Field
		{
			get
			{
				return ShapeField;
			}
			set
			{
				ShapeField = value;
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeShapeRule_Category")]
		[DefaultValue("")]
		public override string Category
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

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributeShapeRule_ShowInLegend")]
		[TypeConverter(typeof(DesignTimeLegendConverter))]
		[RefreshProperties(RefreshProperties.All)]
		[ParenthesizePropertyName(true)]
		[DefaultValue("(none)")]
		public override string ShowInLegend
		{
			get
			{
				return showInLegend;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					showInLegend = "(none)";
				}
				else
				{
					showInLegend = value;
				}
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeShapeRule_ShowInColorSwatch")]
		[DefaultValue(false)]
		public bool ShowInColorSwatch
		{
			get
			{
				return showInColorSwatch;
			}
			set
			{
				showInColorSwatch = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributeShapeRule_LegendText")]
		[DefaultValue("#FROMVALUE{N0} - #TOVALUE{N0}")]
		public override string LegendText
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

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributeShapeRule_ColorCount")]
		[DefaultValue(5)]
		public int ColorCount
		{
			get
			{
				return colorCount;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException(SR.ExceptionValueCannotBeLessThanOne);
				}
				colorCount = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributeShapeRule_FromColor")]
		[DefaultValue(typeof(Color), "Green")]
		public Color FromColor
		{
			get
			{
				return fromColor;
			}
			set
			{
				fromColor = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributeShapeRule_MiddleColor")]
		[DefaultValue(typeof(Color), "Yellow")]
		public Color MiddleColor
		{
			get
			{
				return middleColor;
			}
			set
			{
				middleColor = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributeShapeRule_ToColor")]
		[DefaultValue(typeof(Color), "Red")]
		public Color ToColor
		{
			get
			{
				return toColor;
			}
			set
			{
				toColor = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeShapeRule_FromValue")]
		[TypeConverter(typeof(StringConverter))]
		[DefaultValue("")]
		public override string FromValue
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
		[SRDescription("DescriptionAttributeShapeRule_ToValue")]
		[TypeConverter(typeof(StringConverter))]
		[DefaultValue("")]
		public override string ToValue
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

		[SRCategory("CategoryAttribute_CustomColors")]
		[SRDescription("DescriptionAttributeShapeRule_UseCustomColors")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(false)]
		public bool UseCustomColors
		{
			get
			{
				return useCustomColors;
			}
			set
			{
				useCustomColors = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributeShapeRule_ColoringMode")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(ColoringMode.DistinctColors)]
		public ColoringMode ColoringMode
		{
			get
			{
				return coloringMode;
			}
			set
			{
				coloringMode = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeShapeRule_DataGrouping")]
		[DefaultValue(DataGrouping.Optimal)]
		internal override DataGrouping DataGrouping
		{
			get
			{
				return dataGrouping;
			}
			set
			{
				dataGrouping = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributeShapeRule_ColorPalette")]
		[DefaultValue(MapColorPalette.Random)]
		public MapColorPalette ColorPalette
		{
			get
			{
				return colorPalette;
			}
			set
			{
				colorPalette = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeShapeRule_BorderColor")]
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

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeShapeRule_SecondaryColor")]
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

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeShapeRule_GradientType")]
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

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeShapeRule_HatchStyle")]
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

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeShapeRule_Text")]
		[Localizable(true)]
		[TypeConverter(typeof(KeywordConverter))]
		[DefaultValue("#NAME")]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeShapeRule_ToolTip")]
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

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				if (customColors != null)
				{
					customColors.Common = value;
				}
			}
		}

		public ShapeRule()
			: this(null)
		{
		}

		internal ShapeRule(CommonElements common)
			: base(common)
		{
			customColors = new CustomColorCollection(this, common);
		}

		public override string ToString()
		{
			return Name;
		}

		internal override void OnAdded()
		{
			base.OnAdded();
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

		internal override void OnRemove()
		{
			base.OnRemove();
			InvalidateRules();
		}

		internal void RegenerateColorRanges()
		{
			if (UseCustomColors)
			{
				foreach (CustomColor customColor9 in CustomColors)
				{
					customColor9.AffectedElements.Clear();
				}
			}
			MapCore mapCore = GetMapCore();
			if (mapCore == null || mapCore.Shapes.Count == 0 || (UseCustomColors && CustomColors.Count == 0))
			{
				return;
			}
			if (!UseCustomColors)
			{
				CustomColors.Clear();
			}
			if (ShapeField == "(Name)")
			{
				if (UseCustomColors)
				{
					int num = 0;
					foreach (Shape shape4 in mapCore.Shapes)
					{
						if (num == CustomColors.Count)
						{
							break;
						}
						CustomColor customColor = CustomColors[num++];
						customColor.FromValueInt = shape4.Name;
						customColor.ToValueInt = shape4.Name;
					}
				}
				else
				{
					Color[] colors = GetColors(ColoringMode, ColorPalette, FromColor, MiddleColor, ToColor, mapCore.Shapes.Count);
					int num2 = 0;
					foreach (Shape shape5 in mapCore.Shapes)
					{
						if (shape5.Category == Category)
						{
							CustomColor customColor2 = CustomColors.Add(string.Empty);
							customColor2.Color = colors[num2++];
							customColor2.SecondaryColor = SecondaryColor;
							customColor2.BorderColor = BorderColor;
							customColor2.GradientType = GradientType;
							customColor2.HatchStyle = HatchStyle;
							customColor2.FromValueInt = shape5.Name;
							customColor2.ToValueInt = shape5.Name;
							customColor2.Text = Text;
							customColor2.ToolTip = ToolTip;
						}
					}
				}
				UpdateColorSwatchAndLegend();
				return;
			}
			Field field = GetField();
			if (field == null)
			{
				return;
			}
			if (field.IsNumeric())
			{
				int intervalCount = (!UseCustomColors) ? ColorCount : CustomColors.Count;
				object obj = null;
				object obj2 = null;
				if (FromValue != string.Empty)
				{
					obj = field.Parse(FromValue);
				}
				if (ToValue != string.Empty)
				{
					obj2 = field.Parse(ToValue);
				}
				if (obj == null || obj2 == null)
				{
					GetRangeFromShapes(field, intervalCount, ref obj, ref obj2);
				}
				object[] fromValues = null;
				object[] toValues = null;
				if (DataGrouping == DataGrouping.EqualInterval)
				{
					GetEqualIntervals(field, obj, obj2, intervalCount, ref fromValues, ref toValues);
				}
				else if (DataGrouping == DataGrouping.EqualDistribution)
				{
					ArrayList sortedValues = GetSortedValues(field, obj, obj2);
					GetEqualDistributionIntervals(field, sortedValues, obj, obj2, intervalCount, ref fromValues, ref toValues);
				}
				else if (DataGrouping == DataGrouping.Optimal)
				{
					ArrayList sortedValues2 = GetSortedValues(field, obj, obj2);
					GetOptimalIntervals(field, sortedValues2, obj, obj2, intervalCount, ref fromValues, ref toValues);
				}
				if (UseCustomColors)
				{
					int num3 = 0;
					foreach (CustomColor customColor10 in CustomColors)
					{
						if (num3 < fromValues.Length)
						{
							customColor10.FromValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(fromValues[num3]);
							customColor10.ToValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(toValues[num3]);
							customColor10.VisibleInt = true;
						}
						else
						{
							customColor10.FromValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(toValues[toValues.Length - 1]);
							customColor10.ToValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(toValues[toValues.Length - 1]);
							customColor10.VisibleInt = false;
						}
						num3++;
					}
				}
				else
				{
					Color[] colors2 = GetColors(ColoringMode, ColorPalette, FromColor, MiddleColor, ToColor, fromValues.Length);
					for (int i = 0; i < fromValues.Length; i++)
					{
						CustomColor customColor4 = CustomColors.Add(string.Empty);
						customColor4.Color = colors2[i];
						customColor4.SecondaryColor = SecondaryColor;
						customColor4.BorderColor = BorderColor;
						customColor4.GradientType = GradientType;
						customColor4.HatchStyle = HatchStyle;
						customColor4.FromValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(fromValues[i]);
						customColor4.ToValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(toValues[i]);
						customColor4.Text = Text;
						customColor4.ToolTip = ToolTip;
					}
				}
			}
			else if (field.Type == typeof(string))
			{
				Hashtable hashtable = new Hashtable();
				foreach (Shape shape6 in GetMapCore().Shapes)
				{
					if (shape6.Category == Category)
					{
						string text = (string)shape6[field.Name];
						if (text != null)
						{
							hashtable[text] = 0;
						}
					}
				}
				if (UseCustomColors)
				{
					ArrayList arrayList = new ArrayList();
					arrayList.AddRange(hashtable.Keys);
					arrayList.Sort();
					int num4 = 0;
					foreach (object item in arrayList)
					{
						if (num4 == CustomColors.Count)
						{
							break;
						}
						CustomColor customColor5 = CustomColors[num4++];
						customColor5.FromValueInt = (string)item;
						customColor5.ToValueInt = (string)item;
					}
				}
				else
				{
					Color[] colors3 = GetColors(ColoringMode, ColorPalette, FromColor, MiddleColor, ToColor, hashtable.Keys.Count);
					int num5 = 0;
					foreach (object key in hashtable.Keys)
					{
						CustomColor customColor6 = CustomColors.Add(string.Empty);
						customColor6.Color = colors3[num5++];
						customColor6.SecondaryColor = SecondaryColor;
						customColor6.BorderColor = BorderColor;
						customColor6.GradientType = GradientType;
						customColor6.HatchStyle = HatchStyle;
						customColor6.FromValueInt = (string)key;
						customColor6.ToValueInt = (string)key;
						customColor6.Text = Text;
						customColor6.ToolTip = ToolTip;
					}
				}
			}
			else if (UseCustomColors)
			{
				CustomColors[0].FromValueInt = "False";
				CustomColors[0].ToValueInt = "False";
				if (CustomColors.Count > 1)
				{
					CustomColors[1].FromValueInt = "True";
					CustomColors[1].ToValueInt = "True";
				}
			}
			else
			{
				CustomColor customColor7 = CustomColors.Add(string.Empty);
				customColor7.Color = FromColor;
				customColor7.SecondaryColor = SecondaryColor;
				customColor7.BorderColor = BorderColor;
				customColor7.GradientType = GradientType;
				customColor7.HatchStyle = HatchStyle;
				customColor7.FromValueInt = "False";
				customColor7.ToValueInt = "False";
				customColor7.Text = Text;
				customColor7.ToolTip = ToolTip;
				CustomColor customColor8 = CustomColors.Add(string.Empty);
				customColor8.Color = ToColor;
				customColor8.SecondaryColor = SecondaryColor;
				customColor8.BorderColor = BorderColor;
				customColor8.GradientType = GradientType;
				customColor8.HatchStyle = HatchStyle;
				customColor8.FromValueInt = "True";
				customColor8.ToValueInt = "True";
				customColor8.Text = Text;
				customColor8.ToolTip = ToolTip;
			}
			UpdateColorSwatchAndLegend();
		}

		internal void UpdateColorSwatchAndLegend()
		{
			MapCore mapCore = GetMapCore();
			if (mapCore == null)
			{
				return;
			}
			Field field = GetField();
			if (ShowInColorSwatch)
			{
				foreach (CustomColor customColor3 in CustomColors)
				{
					if (customColor3.VisibleInt)
					{
						SwatchColor swatchColor = mapCore.ColorSwatchPanel.Colors.Add("");
						swatchColor.automaticallyAdded = true;
						swatchColor.Color = customColor3.Color;
						swatchColor.SecondaryColor = customColor3.SecondaryColor;
						swatchColor.GradientType = customColor3.GradientType;
						swatchColor.HatchStyle = customColor3.HatchStyle;
						if (field != null && field.IsNumeric())
						{
							swatchColor.FromValue = field.ConvertToDouble(field.Parse(customColor3.FromValueInt));
							swatchColor.ToValue = field.ConvertToDouble(field.Parse(customColor3.ToValueInt));
						}
						else
						{
							swatchColor.TextValue = customColor3.FromValueInt;
						}
					}
				}
			}
			if (!(ShowInLegend != string.Empty) || !(ShowInLegend != "(none)"))
			{
				return;
			}
			Legend legend = (Legend)mapCore.Legends.GetByName(ShowInLegend);
			if (legend == null)
			{
				return;
			}
			foreach (CustomColor customColor4 in CustomColors)
			{
				if (customColor4.VisibleInt)
				{
					LegendItem legendItem = legend.Items.Add("");
					legendItem.automaticallyAdded = true;
					legendItem.ItemStyle = LegendItemStyle.Shape;
					legendItem.Color = customColor4.Color;
					legendItem.SecondaryColor = customColor4.SecondaryColor;
					legendItem.GradientType = customColor4.GradientType;
					legendItem.HatchStyle = customColor4.HatchStyle;
					legendItem.Text = GetLegendText(field, customColor4.FromValueInt, customColor4.ToValueInt);
				}
			}
		}

		internal string GetLegendText(Field field, string fromValue, string toValue)
		{
			object obj = null;
			object obj2 = null;
			if (field != null)
			{
				obj = field.Parse(fromValue);
				obj2 = field.Parse(toValue);
			}
			else
			{
				obj = fromValue;
				obj2 = toValue;
			}
			string empty = string.Empty;
			empty = ((!(LegendText == "#FROMVALUE{N0} - #TOVALUE{N0}") || (field != null && field.IsNumeric())) ? LegendText : "#VALUE");
			MapCore mapCore = GetMapCore();
			empty = mapCore.ResolveKeyword(empty, "#FROMVALUE", obj);
			empty = mapCore.ResolveKeyword(empty, "#TOVALUE", obj2);
			return mapCore.ResolveKeyword(empty, "#VALUE", obj);
		}

		internal void Apply(Shape shape)
		{
			if (Category != shape.Category)
			{
				return;
			}
			if (ShapeField == "(Name)")
			{
				foreach (CustomColor customColor3 in CustomColors)
				{
					if (customColor3.FromValueInt == shape.Name)
					{
						shape.ApplyCustomColorAttributes(customColor3);
						customColor3.AffectedElements.Add(shape);
						break;
					}
				}
				return;
			}
			Field field = GetField();
			if (field == null)
			{
				return;
			}
			object testValue = shape[field.Name];
			foreach (CustomColor customColor4 in CustomColors)
			{
				object obj = field.Parse(customColor4.FromValueInt);
				object obj2 = field.Parse(customColor4.ToValueInt);
				if (IsValueInRange(field, testValue, obj, obj2))
				{
					shape.ApplyCustomColorAttributes(customColor4);
					customColor4.AffectedElements.Add(shape);
					break;
				}
			}
		}

		internal override Field GetField()
		{
			MapCore mapCore = GetMapCore();
			if (mapCore == null)
			{
				return null;
			}
			return (Field)mapCore.ShapeFields.GetByName(ShapeField);
		}

		internal void GetRangeFromShapes(Field field, int intervalCount, ref object fromValue, ref object toValue)
		{
			MapCore mapCore = GetMapCore();
			if (mapCore == null)
			{
				return;
			}
			if (field.Type == typeof(int))
			{
				int num = int.MaxValue;
				int num2 = int.MinValue;
				foreach (Shape shape6 in mapCore.Shapes)
				{
					if (!(Category != shape6.Category))
					{
						object obj = shape6[field.Name];
						if (obj != null)
						{
							int val = Convert.ToInt32(obj, CultureInfo.InvariantCulture);
							num = Math.Min(num, val);
							num2 = Math.Max(num2, val);
						}
					}
				}
				if (num == int.MaxValue)
				{
					num = 0;
				}
				if (num2 == int.MinValue)
				{
					num2 = 10;
				}
				if (fromValue == null)
				{
					fromValue = num;
				}
				if (toValue == null)
				{
					toValue = num2;
				}
			}
			else if (field.Type == typeof(double))
			{
				double num3 = double.MaxValue;
				double num4 = double.MinValue;
				foreach (Shape shape7 in mapCore.Shapes)
				{
					if (!(Category != shape7.Category))
					{
						object obj2 = shape7[field.Name];
						if (obj2 != null)
						{
							double val2 = Convert.ToDouble(obj2, CultureInfo.InvariantCulture);
							num3 = Math.Min(num3, val2);
							num4 = Math.Max(num4, val2);
						}
					}
				}
				if (num3 == double.MaxValue)
				{
					num3 = 0.0;
				}
				if (num4 == double.MinValue)
				{
					num4 = 10.0;
				}
				if (num3 != num4 && DataGrouping == DataGrouping.EqualInterval)
				{
					int num5 = (int)Math.Log10(Math.Max(Math.Abs(num3), Math.Abs(num4)));
					double num6 = Math.Pow(10.0, num5 - 1);
					num3 /= num6;
					num3 = Math.Floor(num3);
					int num7 = (int)num3 * 10;
					num3 *= num6;
					num4 /= num6;
					num4 = Math.Ceiling(num4);
					int num8 = ((int)num4 * 10 - num7) % intervalCount;
					num4 += (double)(intervalCount - num8) / 10.0;
					num4 *= num6;
				}
				if (fromValue == null)
				{
					fromValue = num3;
				}
				if (toValue == null)
				{
					toValue = num4;
				}
			}
			else if (field.Type == typeof(decimal))
			{
				decimal num9 = decimal.MaxValue;
				decimal num10 = decimal.MinValue;
				foreach (Shape shape8 in mapCore.Shapes)
				{
					if (!(Category != shape8.Category))
					{
						object obj3 = shape8[field.Name];
						if (obj3 != null)
						{
							decimal val3 = Convert.ToDecimal(obj3, CultureInfo.InvariantCulture);
							num9 = Math.Min(num9, val3);
							num10 = Math.Max(num10, val3);
						}
					}
				}
				if (num9 == decimal.MaxValue)
				{
					num9 = default(decimal);
				}
				if (num10 == decimal.MinValue)
				{
					num10 = 10m;
				}
				if (num9 != num10 && DataGrouping == DataGrouping.EqualInterval)
				{
					int num11 = (int)Math.Log10((double)Math.Max(Math.Abs(num9), Math.Abs(num10)));
					decimal num12 = (decimal)Math.Pow(10.0, num11 - 1);
					num9 /= num12;
					num9 = decimal.Floor(num9);
					int num13 = (int)num9 * 10;
					num9 *= num12;
					num10 /= num12;
					num10 = Math.Ceiling(num10);
					int num14 = ((int)num10 * 10 - num13) % intervalCount;
					num10 += (decimal)(intervalCount - num14) / 10m;
					num10 *= num12;
				}
				if (fromValue == null)
				{
					fromValue = num9;
				}
				if (toValue == null)
				{
					toValue = num10;
				}
			}
			else if (field.Type == typeof(DateTime))
			{
				DateTime dateTime = DateTime.MaxValue;
				DateTime dateTime2 = DateTime.MinValue;
				foreach (Shape shape9 in mapCore.Shapes)
				{
					if (Category != shape9.Category)
					{
						continue;
					}
					object obj4 = shape9[field.Name];
					if (obj4 != null)
					{
						DateTime dateTime3 = Convert.ToDateTime(obj4, CultureInfo.InvariantCulture);
						if (dateTime > dateTime3)
						{
							dateTime = dateTime3;
						}
						if (dateTime2 < dateTime3)
						{
							dateTime2 = dateTime3;
						}
					}
				}
				if (dateTime == DateTime.MaxValue)
				{
					dateTime = DateTime.MinValue;
				}
				if (dateTime2 == DateTime.MinValue)
				{
					dateTime2 = DateTime.MaxValue;
				}
				if (fromValue == null)
				{
					fromValue = dateTime;
				}
				if (toValue == null)
				{
					toValue = dateTime2;
				}
			}
			else
			{
				if (!(field.Type == typeof(TimeSpan)))
				{
					return;
				}
				TimeSpan timeSpan = TimeSpan.MaxValue;
				TimeSpan timeSpan2 = TimeSpan.MinValue;
				foreach (Shape shape10 in mapCore.Shapes)
				{
					if (Category != shape10.Category)
					{
						continue;
					}
					object obj5 = shape10[field.Name];
					if (obj5 != null)
					{
						TimeSpan timeSpan3 = (TimeSpan)obj5;
						if (timeSpan > timeSpan3)
						{
							timeSpan = timeSpan3;
						}
						if (timeSpan2 < timeSpan3)
						{
							timeSpan2 = timeSpan3;
						}
					}
				}
				if (timeSpan == TimeSpan.MaxValue)
				{
					timeSpan = TimeSpan.MinValue;
				}
				if (timeSpan2 == TimeSpan.MinValue)
				{
					timeSpan2 = TimeSpan.MaxValue;
				}
				if (fromValue == null)
				{
					fromValue = timeSpan;
				}
				if (toValue == null)
				{
					toValue = timeSpan2;
				}
			}
		}

		internal override ArrayList GetSortedValues(Field field, object fromValue, object toValue)
		{
			MapCore mapCore = GetMapCore();
			if (mapCore == null)
			{
				return null;
			}
			ArrayList arrayList = new ArrayList();
			foreach (Shape shape in mapCore.Shapes)
			{
				if (shape.Category == Category)
				{
					object obj = shape[field.Name];
					if (obj != null && IsValueInRange(field, obj, fromValue, toValue))
					{
						arrayList.Add(obj);
					}
				}
			}
			arrayList.Sort();
			return arrayList;
		}
	}
}
