using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(PathRuleConverter))]
	internal class PathRule : PathRuleBase
	{
		private CustomColorCollection customColors;

		private bool showInColorSwatch;

		private Color fromColor = Color.Green;

		private Color middleColor = Color.Yellow;

		private Color toColor = Color.Red;

		private string fromValue = "";

		private string toValue = "";

		private bool useCustomColors;

		private MapColorPalette colorPalette;

		private Color borderColor = Color.DarkGray;

		private Color secondaryColor = Color.Empty;

		private GradientType gradientType;

		private MapHatchStyle hatchStyle;

		private int widthInLegend = 5;

		[SRCategory("CategoryAttribute_CustomColors")]
		[SRDescription("DescriptionAttributePathRule_CustomColors")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CustomColorCollection CustomColors => customColors;

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributePathRule_ShowInColorSwatch")]
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

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributePathRule_ColorCount")]
		[DefaultValue(5)]
		public override int ColorCount
		{
			get
			{
				return base.ColorCount;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException(SR.ExceptionPropertyValueCannotbeLessThanOne);
				}
				base.ColorCount = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributePathRule_FromColor")]
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
		[SRDescription("DescriptionAttributePathRule_MiddleColor")]
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
		[SRDescription("DescriptionAttributePathRule_ToColor")]
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
		[SRDescription("DescriptionAttributePathRule_FromValue")]
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
		[SRDescription("DescriptionAttributePathRule_ToValue")]
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
		[SRDescription("DescriptionAttributePathRule_UseCustomColors")]
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
		[SRDescription("DescriptionAttributePathRule_ColoringMode")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(ColoringMode.DistinctColors)]
		internal override ColoringMode ColoringMode
		{
			get
			{
				return base.ColoringMode;
			}
			set
			{
				base.ColoringMode = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributePathRule_ColorPalette")]
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
		[SRDescription("DescriptionAttributePathRule_BorderColor")]
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
		[SRDescription("DescriptionAttributePathRule_SecondaryColor")]
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
		[SRDescription("DescriptionAttributePathRule_GradientType")]
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
		[SRDescription("DescriptionAttributePathRule_HatchStyle")]
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

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributePathRule_WidthInLegend")]
		[NotifyParentProperty(true)]
		[DefaultValue(5)]
		public int WidthInLegend
		{
			get
			{
				return widthInLegend;
			}
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				widthInLegend = value;
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

		public PathRule()
			: this(null)
		{
		}

		internal PathRule(CommonElements common)
			: base(common)
		{
			customColors = new CustomColorCollection(this, common);
		}

		internal override void RegenerateRanges()
		{
			if (UseCustomColors)
			{
				foreach (CustomColor customColor9 in CustomColors)
				{
					customColor9.AffectedElements.Clear();
				}
			}
			MapCore mapCore = GetMapCore();
			if (mapCore == null || mapCore.Paths.Count == 0 || (UseCustomColors && CustomColors.Count == 0))
			{
				return;
			}
			if (!UseCustomColors)
			{
				CustomColors.Clear();
			}
			if (base.PathField == "(Name)")
			{
				if (UseCustomColors)
				{
					int num = 0;
					foreach (Path path4 in mapCore.Paths)
					{
						if (num == CustomColors.Count)
						{
							break;
						}
						CustomColor customColor = CustomColors[num++];
						customColor.FromValueInt = path4.Name;
						customColor.ToValueInt = path4.Name;
					}
				}
				else
				{
					Color[] colors = GetColors(ColoringMode, ColorPalette, FromColor, MiddleColor, ToColor, mapCore.Paths.Count);
					int num2 = 0;
					foreach (Path path5 in mapCore.Paths)
					{
						if (path5.Category == Category)
						{
							CustomColor customColor2 = CustomColors.Add(string.Empty);
							customColor2.Color = colors[num2++];
							customColor2.SecondaryColor = SecondaryColor;
							customColor2.BorderColor = BorderColor;
							customColor2.GradientType = GradientType;
							customColor2.HatchStyle = HatchStyle;
							customColor2.FromValueInt = path5.Name;
							customColor2.ToValueInt = path5.Name;
							customColor2.Text = base.Text;
							customColor2.ToolTip = base.ToolTip;
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
					GetRangeFromPaths(field, intervalCount, ref obj, ref obj2);
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
						customColor4.Text = base.Text;
						customColor4.ToolTip = base.ToolTip;
					}
				}
			}
			else if (field.Type == typeof(string))
			{
				Hashtable hashtable = new Hashtable();
				foreach (Path path6 in GetMapCore().Paths)
				{
					if (path6.Category == Category)
					{
						string text = (string)path6[field.Name];
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
						customColor6.Text = base.Text;
						customColor6.ToolTip = base.ToolTip;
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
				customColor7.Text = base.Text;
				customColor7.ToolTip = base.ToolTip;
				CustomColor customColor8 = CustomColors.Add(string.Empty);
				customColor8.Color = ToColor;
				customColor8.SecondaryColor = SecondaryColor;
				customColor8.BorderColor = BorderColor;
				customColor8.GradientType = GradientType;
				customColor8.HatchStyle = HatchStyle;
				customColor8.FromValueInt = "True";
				customColor8.ToValueInt = "True";
				customColor8.Text = base.Text;
				customColor8.ToolTip = base.ToolTip;
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
					legendItem.ItemStyle = LegendItemStyle.Path;
					legendItem.BorderColor = customColor4.BorderColor;
					legendItem.Color = customColor4.Color;
					legendItem.SecondaryColor = customColor4.SecondaryColor;
					legendItem.GradientType = customColor4.GradientType;
					legendItem.HatchStyle = customColor4.HatchStyle;
					legendItem.Text = GetLegendText(field, customColor4.FromValueInt, customColor4.ToValueInt);
					legendItem.PathWidth = WidthInLegend;
					legendItem.PathLineStyle = base.LineStyleInLegend;
					legendItem.BorderWidth = base.BorderWidthInLegend;
				}
			}
		}

		internal override void Apply(Path path)
		{
			if (Category != path.Category)
			{
				return;
			}
			if (base.PathField == "(Name)")
			{
				foreach (CustomColor customColor3 in CustomColors)
				{
					if (customColor3.FromValueInt == path.Name)
					{
						path.ApplyCustomColorAttributes(customColor3);
						customColor3.AffectedElements.Add(path);
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
			object testValue = path[field.Name];
			foreach (CustomColor customColor4 in CustomColors)
			{
				object obj = field.Parse(customColor4.FromValueInt);
				object obj2 = field.Parse(customColor4.ToValueInt);
				if (IsValueInRange(field, testValue, obj, obj2))
				{
					path.ApplyCustomColorAttributes(customColor4);
					customColor4.AffectedElements.Add(path);
					break;
				}
			}
		}
	}
}
