using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal abstract class PathRuleBase : RuleBase
	{
		private string pathField = "(Name)";

		private string showInLegend = "(none)";

		private string legendText = "#FROMVALUE{N0} - #TOVALUE{N0}";

		private string category = string.Empty;

		private DataGrouping dataGrouping = DataGrouping.Optimal;

		private string text = "#NAME";

		private string toolTip = "";

		private ColoringMode coloringMode = ColoringMode.DistinctColors;

		private int colorCount = 5;

		private MapDashStyle lineStyleInLegend = MapDashStyle.Solid;

		private int borderWidthInLegend = 1;

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributePathRule_Name")]
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
		[SRDescription("DescriptionAttributePathRule_PathField")]
		[TypeConverter(typeof(DesignTimeFieldConverter))]
		[RefreshProperties(RefreshProperties.All)]
		[ParenthesizePropertyName(true)]
		[DefaultValue("(Name)")]
		public string PathField
		{
			get
			{
				return pathField;
			}
			set
			{
				pathField = value;
				InvalidateRules();
			}
		}

		internal override string Field
		{
			get
			{
				return PathField;
			}
			set
			{
				PathField = value;
			}
		}

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributePathRule_ShowInLegend")]
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
		[SRDescription("DescriptionAttributePathRule_LegendText")]
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

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributePathRule_Category")]
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

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributePathRule_DataGrouping")]
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

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributePathRule_Text")]
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
		[SRDescription("DescriptionAttributePathRule_ToolTip")]
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal virtual ColoringMode ColoringMode
		{
			get
			{
				return coloringMode;
			}
			set
			{
				coloringMode = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public virtual int ColorCount
		{
			get
			{
				return colorCount;
			}
			set
			{
				colorCount = value;
			}
		}

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributePathRule_LineStyleInLegend")]
		[NotifyParentProperty(true)]
		[DefaultValue(MapDashStyle.Solid)]
		public MapDashStyle LineStyleInLegend
		{
			get
			{
				return lineStyleInLegend;
			}
			set
			{
				lineStyleInLegend = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributePathRule_BorderWidthInLegend")]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		public int BorderWidthInLegend
		{
			get
			{
				return borderWidthInLegend;
			}
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentException(SR.must_in_range(0.0, 100.0));
				}
				borderWidthInLegend = value;
				InvalidateRules();
			}
		}

		internal PathRuleBase(CommonElements common)
			: base(common)
		{
		}

		public override string ToString()
		{
			return Name;
		}

		internal abstract void Apply(Path path);

		internal abstract void RegenerateRanges();

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

		internal void GetRangeFromPaths(Field field, int intervalCount, ref object fromValue, ref object toValue)
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
				foreach (Path path6 in mapCore.Paths)
				{
					if (!(Category != path6.Category))
					{
						object obj = path6[field.Name];
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
				foreach (Path path7 in mapCore.Paths)
				{
					if (!(Category != path7.Category))
					{
						object obj2 = path7[field.Name];
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
				foreach (Path path8 in mapCore.Paths)
				{
					if (!(Category != path8.Category))
					{
						object obj3 = path8[field.Name];
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
				foreach (Path path9 in mapCore.Paths)
				{
					if (Category != path9.Category)
					{
						continue;
					}
					object obj4 = path9[field.Name];
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
				foreach (Path path10 in mapCore.Paths)
				{
					if (Category != path10.Category)
					{
						continue;
					}
					object obj5 = path10[field.Name];
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
			foreach (Path path in mapCore.Paths)
			{
				if (path.Category == Category)
				{
					object obj = path[field.Name];
					if (obj != null && IsValueInRange(field, obj, fromValue, toValue))
					{
						arrayList.Add(obj);
					}
				}
			}
			arrayList.Sort();
			return arrayList;
		}

		internal override Field GetField()
		{
			MapCore mapCore = GetMapCore();
			if (mapCore == null)
			{
				return null;
			}
			return (Field)mapCore.PathFields.GetByName(PathField);
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
	}
}
