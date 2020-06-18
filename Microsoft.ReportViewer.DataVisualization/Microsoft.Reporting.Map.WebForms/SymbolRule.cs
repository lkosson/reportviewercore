using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(SymbolRuleConverter))]
	internal class SymbolRule : RuleBase
	{
		private PredefinedSymbolCollection predefinedSymbols;

		private bool showInColorSwatch;

		private string symbolField = "(Name)";

		private string category = string.Empty;

		private string showInLegend = "(none)";

		private string legendText = "#FROMVALUE{N0} - #TOVALUE{N0}";

		private string fromValue = "";

		private string toValue = "";

		private DataGrouping dataGrouping = DataGrouping.Optimal;

		private AffectedSymbolAttributes affectedAttributes;

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolRule_PredefinedSymbols")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PredefinedSymbolCollection PredefinedSymbols => predefinedSymbols;

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeSymbolRule_ShowInColorSwatch")]
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

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolRule_Name")]
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
		[SRDescription("DescriptionAttributeSymbolRule_SymbolField")]
		[TypeConverter(typeof(DesignTimeFieldConverter))]
		[ParenthesizePropertyName(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("(Name)")]
		public string SymbolField
		{
			get
			{
				return symbolField;
			}
			set
			{
				symbolField = value;
				InvalidateRules();
			}
		}

		internal override string Field
		{
			get
			{
				return SymbolField;
			}
			set
			{
				SymbolField = value;
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolRule_Category")]
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
		[SRDescription("DescriptionAttributeSymbolRule_ShowInLegend")]
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

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributeSymbolRule_LegendText")]
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

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeSymbolRule_FromValue")]
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
		[SRDescription("DescriptionAttributeSymbolRule_ToValue")]
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

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolRule_DataGrouping")]
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

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				if (predefinedSymbols != null)
				{
					predefinedSymbols.Common = value;
				}
			}
		}

		[DefaultValue(AffectedSymbolAttributes.All)]
		internal AffectedSymbolAttributes AffectedAttributes
		{
			get
			{
				return affectedAttributes;
			}
			set
			{
				affectedAttributes = value;
				InvalidateRules();
			}
		}

		public SymbolRule()
			: this(null)
		{
		}

		internal SymbolRule(CommonElements common)
			: base(common)
		{
			predefinedSymbols = new PredefinedSymbolCollection(this, common);
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

		internal void UpdateAutoRanges()
		{
			foreach (PredefinedSymbol predefinedSymbol4 in PredefinedSymbols)
			{
				predefinedSymbol4.AffectedSymbols.Clear();
			}
			MapCore mapCore = GetMapCore();
			if (mapCore == null || mapCore.Symbols.Count == 0 || PredefinedSymbols.Count == 0)
			{
				return;
			}
			if (SymbolField == "(Name)")
			{
				int num = 0;
				foreach (Symbol symbol3 in mapCore.Symbols)
				{
					if (num == PredefinedSymbols.Count)
					{
						break;
					}
					PredefinedSymbol predefinedSymbol = PredefinedSymbols[num++];
					predefinedSymbol.FromValueInt = symbol3.Name;
					predefinedSymbol.ToValueInt = symbol3.Name;
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
				int count = PredefinedSymbols.Count;
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
					GetRangeFromSymbols(field, count, ref obj, ref obj2);
				}
				object[] fromValues = null;
				object[] toValues = null;
				if (DataGrouping == DataGrouping.EqualInterval)
				{
					GetEqualIntervals(field, obj, obj2, count, ref fromValues, ref toValues);
				}
				else if (DataGrouping == DataGrouping.EqualDistribution)
				{
					ArrayList sortedValues = GetSortedValues(field, obj, obj2);
					GetEqualDistributionIntervals(field, sortedValues, obj, obj2, count, ref fromValues, ref toValues);
				}
				else if (DataGrouping == DataGrouping.Optimal)
				{
					ArrayList sortedValues2 = GetSortedValues(field, obj, obj2);
					GetOptimalIntervals(field, sortedValues2, obj, obj2, count, ref fromValues, ref toValues);
				}
				int num2 = 0;
				foreach (PredefinedSymbol predefinedSymbol5 in PredefinedSymbols)
				{
					if (num2 < fromValues.Length)
					{
						predefinedSymbol5.FromValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(fromValues[num2]);
						predefinedSymbol5.ToValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(toValues[num2]);
						predefinedSymbol5.VisibleInt = true;
					}
					else
					{
						predefinedSymbol5.FromValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(toValues[toValues.Length - 1]);
						predefinedSymbol5.ToValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(toValues[toValues.Length - 1]);
						predefinedSymbol5.VisibleInt = false;
					}
					num2++;
				}
			}
			else if (field.Type == typeof(string))
			{
				Hashtable hashtable = new Hashtable();
				foreach (Symbol symbol4 in GetMapCore().Symbols)
				{
					if (symbol4.Category == Category)
					{
						string text = (string)symbol4[field.Name];
						if (text != null)
						{
							hashtable[text] = 0;
						}
					}
				}
				ArrayList arrayList = new ArrayList();
				arrayList.AddRange(hashtable.Keys);
				arrayList.Sort();
				int num3 = 0;
				foreach (object item in arrayList)
				{
					if (num3 == PredefinedSymbols.Count)
					{
						break;
					}
					PredefinedSymbol predefinedSymbol3 = PredefinedSymbols[num3++];
					predefinedSymbol3.FromValueInt = (string)item;
					predefinedSymbol3.ToValueInt = (string)item;
				}
			}
			else
			{
				PredefinedSymbols[0].FromValueInt = "False";
				PredefinedSymbols[0].ToValueInt = "False";
				if (PredefinedSymbols.Count > 1)
				{
					PredefinedSymbols[1].FromValueInt = "True";
					PredefinedSymbols[1].ToValueInt = "True";
				}
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
				foreach (PredefinedSymbol predefinedSymbol3 in PredefinedSymbols)
				{
					if (predefinedSymbol3.Visible)
					{
						SwatchColor swatchColor = mapCore.ColorSwatchPanel.Colors.Add("");
						swatchColor.automaticallyAdded = true;
						swatchColor.Color = predefinedSymbol3.Color;
						swatchColor.SecondaryColor = predefinedSymbol3.SecondaryColor;
						swatchColor.GradientType = predefinedSymbol3.GradientType;
						swatchColor.HatchStyle = predefinedSymbol3.HatchStyle;
						if (field != null && field.IsNumeric())
						{
							swatchColor.FromValue = field.ConvertToDouble(field.Parse(predefinedSymbol3.FromValueInt));
							swatchColor.ToValue = field.ConvertToDouble(field.Parse(predefinedSymbol3.ToValueInt));
						}
						else
						{
							swatchColor.TextValue = predefinedSymbol3.FromValueInt;
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
			foreach (PredefinedSymbol predefinedSymbol4 in PredefinedSymbols)
			{
				if (predefinedSymbol4.Visible)
				{
					LegendItem legendItem = legend.Items.Add("");
					legendItem.automaticallyAdded = true;
					legendItem.ShadowOffset = predefinedSymbol4.ShadowOffset;
					legendItem.Text = GetLegendText(field, predefinedSymbol4.FromValueInt, predefinedSymbol4.ToValueInt);
					if (!string.IsNullOrEmpty(predefinedSymbol4.Image))
					{
						LegendCell legendCell = new LegendCell(LegendCellType.Image, predefinedSymbol4.Image);
						legendCell.ImageTranspColor = predefinedSymbol4.ImageTransColor;
						legendCell.Margins.Top = 15;
						legendCell.Margins.Bottom = 15;
						LegendCell cell = new LegendCell(LegendCellType.Text, "#LEGENDTEXT", ContentAlignment.MiddleLeft);
						legendItem.Cells.Add(legendCell);
						legendItem.Cells.Add(cell);
					}
					else
					{
						legendItem.ItemStyle = LegendItemStyle.Symbol;
						legendItem.MarkerStyle = predefinedSymbol4.MarkerStyle;
						legendItem.MarkerColor = predefinedSymbol4.Color;
						legendItem.MarkerWidth = ((predefinedSymbol4.Width < 0.001f) ? 13f : predefinedSymbol4.Width);
						legendItem.MarkerHeight = ((predefinedSymbol4.Height < 0.001f) ? 13f : predefinedSymbol4.Height);
						legendItem.MarkerGradientType = predefinedSymbol4.GradientType;
						legendItem.MarkerHatchStyle = predefinedSymbol4.HatchStyle;
						legendItem.MarkerSecondaryColor = predefinedSymbol4.SecondaryColor;
						legendItem.MarkerBorderColor = predefinedSymbol4.BorderColor;
						legendItem.MarkerBorderWidth = predefinedSymbol4.BorderWidth;
						legendItem.MarkerBorderStyle = predefinedSymbol4.BorderStyle;
					}
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

		internal void Apply(Symbol symbol)
		{
			if (Category != symbol.Category)
			{
				return;
			}
			if (SymbolField == "(Name)")
			{
				foreach (PredefinedSymbol predefinedSymbol3 in PredefinedSymbols)
				{
					if (predefinedSymbol3.FromValueInt == symbol.Name)
					{
						symbol.ApplyPredefinedSymbolAttributes(predefinedSymbol3, AffectedAttributes);
						predefinedSymbol3.AffectedSymbols.Add(symbol);
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
			object testValue = symbol[field.Name];
			foreach (PredefinedSymbol predefinedSymbol4 in PredefinedSymbols)
			{
				object obj = field.Parse(predefinedSymbol4.FromValueInt);
				object obj2 = field.Parse(predefinedSymbol4.ToValueInt);
				if (IsValueInRange(field, testValue, obj, obj2))
				{
					symbol.ApplyPredefinedSymbolAttributes(predefinedSymbol4, AffectedAttributes);
					predefinedSymbol4.AffectedSymbols.Add(symbol);
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
			return (Field)mapCore.SymbolFields.GetByName(SymbolField);
		}

		internal void GetRangeFromSymbols(Field field, int intervalCount, ref object fromValue, ref object toValue)
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
				foreach (Symbol symbol6 in mapCore.Symbols)
				{
					if (!(Category != symbol6.Category))
					{
						object obj = symbol6[field.Name];
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
				foreach (Symbol symbol7 in mapCore.Symbols)
				{
					if (!(Category != symbol7.Category))
					{
						object obj2 = symbol7[field.Name];
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
				foreach (Symbol symbol8 in mapCore.Symbols)
				{
					if (!(Category != symbol8.Category))
					{
						object obj3 = symbol8[field.Name];
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
				foreach (Symbol symbol9 in mapCore.Symbols)
				{
					if (Category != symbol9.Category)
					{
						continue;
					}
					object obj4 = symbol9[field.Name];
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
				foreach (Symbol symbol10 in mapCore.Symbols)
				{
					if (Category != symbol10.Category)
					{
						continue;
					}
					object obj5 = symbol10[field.Name];
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
			foreach (Symbol symbol in mapCore.Symbols)
			{
				if (symbol.Category == Category)
				{
					object obj = symbol[field.Name];
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
