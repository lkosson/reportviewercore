using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(PathWidthRuleConverter))]
	internal class PathWidthRule : PathRuleBase
	{
		private CustomWidthCollection customWidths;

		private int widthCount = 5;

		private float fromWidth = 1f;

		private float toWidth = 30f;

		private string fromValue = "";

		private string toValue = "";

		private bool useCustomWidths;

		private Color borderColorInLegend = Color.DarkGray;

		private Color colorInLegend = Color.LightSalmon;

		private Color secondaryColorInLegend = Color.Empty;

		private GradientType gradientTypeInLegend;

		private MapHatchStyle hatchStyleInLegend;

		[SRCategory("CategoryAttribute_CustomWidths")]
		[SRDescription("DescriptionAttributePathWidthRule_CustomWidths")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CustomWidthCollection CustomWidths => customWidths;

		[SRCategory("CategoryAttribute_Colors")]
		[SRDescription("DescriptionAttributePathWidthRule_WidthCount")]
		[DefaultValue(5)]
		public int WidthCount
		{
			get
			{
				return widthCount;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException(SR.ExceptionPropertyValueCannotbeLessThanOne);
				}
				widthCount = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Widths")]
		[SRDescription("DescriptionAttributePathWidthRule_FromWidth")]
		[DefaultValue(1f)]
		public float FromWidth
		{
			get
			{
				return fromWidth;
			}
			set
			{
				fromWidth = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Widths")]
		[SRDescription("DescriptionAttributePathWidthRule_ToWidth")]
		[DefaultValue(30f)]
		public float ToWidth
		{
			get
			{
				return toWidth;
			}
			set
			{
				toWidth = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributePathWidthRule_FromValue")]
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
		[SRDescription("DescriptionAttributePathWidthRule_ToValue")]
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

		[SRCategory("CategoryAttribute_CustomWidths")]
		[SRDescription("DescriptionAttributePathWidthRule_UseCustomWidths")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(false)]
		public bool UseCustomWidths
		{
			get
			{
				return useCustomWidths;
			}
			set
			{
				useCustomWidths = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributePathWidthRule_BorderColorInLegend")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color BorderColorInLegend
		{
			get
			{
				return borderColorInLegend;
			}
			set
			{
				borderColorInLegend = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributePathWidthRule_ColorInLegend")]
		[DefaultValue(typeof(Color), "LightSalmon")]
		public Color ColorInLegend
		{
			get
			{
				return colorInLegend;
			}
			set
			{
				colorInLegend = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributePathWidthRule_SecondaryColorInLegend")]
		[DefaultValue(typeof(Color), "")]
		public Color SecondaryColorInLegend
		{
			get
			{
				return secondaryColorInLegend;
			}
			set
			{
				secondaryColorInLegend = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributePathWidthRule_GradientTypeInLegend")]
		[DefaultValue(GradientType.None)]
		public GradientType GradientTypeInLegend
		{
			get
			{
				return gradientTypeInLegend;
			}
			set
			{
				gradientTypeInLegend = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Legend")]
		[SRDescription("DescriptionAttributePathWidthRule_HatchStyleInLegend")]
		[DefaultValue(MapHatchStyle.None)]
		public MapHatchStyle HatchStyleInLegend
		{
			get
			{
				return hatchStyleInLegend;
			}
			set
			{
				hatchStyleInLegend = value;
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
				if (customWidths != null)
				{
					customWidths.Common = value;
				}
			}
		}

		public PathWidthRule()
			: this(null)
		{
		}

		internal PathWidthRule(CommonElements common)
			: base(common)
		{
			customWidths = new CustomWidthCollection(this, common);
		}

		internal override void RegenerateRanges()
		{
			if (UseCustomWidths)
			{
				foreach (CustomWidth customWidth9 in CustomWidths)
				{
					customWidth9.AffectedElements.Clear();
				}
			}
			MapCore mapCore = GetMapCore();
			if (mapCore == null || mapCore.Paths.Count == 0 || (UseCustomWidths && CustomWidths.Count == 0))
			{
				return;
			}
			if (!UseCustomWidths)
			{
				CustomWidths.Clear();
			}
			if (base.PathField == "(Name)")
			{
				if (UseCustomWidths)
				{
					int num = 0;
					foreach (Path path4 in mapCore.Paths)
					{
						if (num == CustomWidths.Count)
						{
							break;
						}
						CustomWidth customWidth = CustomWidths[num++];
						customWidth.FromValueInt = path4.Name;
						customWidth.ToValueInt = path4.Name;
					}
				}
				else
				{
					float[] widths = GetWidths(FromWidth, ToWidth, mapCore.Paths.Count);
					int num2 = 0;
					foreach (Path path5 in mapCore.Paths)
					{
						if (path5.Category == Category)
						{
							CustomWidth customWidth2 = CustomWidths.Add(string.Empty);
							customWidth2.Width = widths[num2++];
							customWidth2.FromValueInt = path5.Name;
							customWidth2.ToValueInt = path5.Name;
							customWidth2.Text = base.Text;
							customWidth2.ToolTip = base.ToolTip;
						}
					}
				}
				UpdateLegend();
				return;
			}
			Field field = GetField();
			if (field == null)
			{
				return;
			}
			if (field.IsNumeric())
			{
				int intervalCount = (!UseCustomWidths) ? WidthCount : CustomWidths.Count;
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
				if (UseCustomWidths)
				{
					int num3 = 0;
					foreach (CustomWidth customWidth10 in CustomWidths)
					{
						if (num3 < fromValues.Length)
						{
							customWidth10.FromValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(fromValues[num3]);
							customWidth10.ToValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(toValues[num3]);
							customWidth10.VisibleInt = true;
						}
						else
						{
							customWidth10.FromValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(toValues[toValues.Length - 1]);
							customWidth10.ToValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(toValues[toValues.Length - 1]);
							customWidth10.VisibleInt = false;
						}
						num3++;
					}
				}
				else
				{
					float[] widths2 = GetWidths(FromWidth, ToWidth, fromValues.Length);
					for (int i = 0; i < fromValues.Length; i++)
					{
						CustomWidth customWidth4 = CustomWidths.Add(string.Empty);
						customWidth4.Width = widths2[i];
						customWidth4.FromValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(fromValues[i]);
						customWidth4.ToValueInt = Microsoft.Reporting.Map.WebForms.Field.ToStringInvariant(toValues[i]);
						customWidth4.Text = base.Text;
						customWidth4.ToolTip = base.ToolTip;
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
				if (UseCustomWidths)
				{
					ArrayList arrayList = new ArrayList();
					arrayList.AddRange(hashtable.Keys);
					arrayList.Sort();
					int num4 = 0;
					foreach (object item in arrayList)
					{
						if (num4 == CustomWidths.Count)
						{
							break;
						}
						CustomWidth customWidth5 = CustomWidths[num4++];
						customWidth5.FromValueInt = (string)item;
						customWidth5.ToValueInt = (string)item;
					}
				}
				else
				{
					float[] widths3 = GetWidths(FromWidth, ToWidth, hashtable.Keys.Count);
					int num5 = 0;
					foreach (object key in hashtable.Keys)
					{
						CustomWidth customWidth6 = CustomWidths.Add(string.Empty);
						customWidth6.Width = widths3[num5++];
						customWidth6.FromValueInt = (string)key;
						customWidth6.ToValueInt = (string)key;
						customWidth6.Text = base.Text;
						customWidth6.ToolTip = base.ToolTip;
					}
				}
			}
			else if (UseCustomWidths)
			{
				CustomWidths[0].FromValueInt = "False";
				CustomWidths[0].ToValueInt = "False";
				if (CustomWidths.Count > 1)
				{
					CustomWidths[1].FromValueInt = "True";
					CustomWidths[1].ToValueInt = "True";
				}
			}
			else
			{
				CustomWidth customWidth7 = CustomWidths.Add(string.Empty);
				customWidth7.Width = FromWidth;
				customWidth7.FromValueInt = "False";
				customWidth7.ToValueInt = "False";
				customWidth7.Text = base.Text;
				customWidth7.ToolTip = base.ToolTip;
				CustomWidth customWidth8 = CustomWidths.Add(string.Empty);
				customWidth8.Width = ToWidth;
				customWidth8.FromValueInt = "True";
				customWidth8.ToValueInt = "True";
				customWidth8.Text = base.Text;
				customWidth8.ToolTip = base.ToolTip;
			}
			UpdateLegend();
		}

		internal void UpdateLegend()
		{
			MapCore mapCore = GetMapCore();
			if (mapCore == null)
			{
				return;
			}
			Field field = GetField();
			if (!(ShowInLegend != string.Empty) || !(ShowInLegend != "(none)"))
			{
				return;
			}
			Legend legend = (Legend)mapCore.Legends.GetByName(ShowInLegend);
			if (legend == null)
			{
				return;
			}
			foreach (CustomWidth customWidth in CustomWidths)
			{
				if (customWidth.VisibleInt)
				{
					LegendItem legendItem = legend.Items.Add("");
					legendItem.automaticallyAdded = true;
					legendItem.PathWidth = (int)Math.Round(customWidth.Width);
					legendItem.ItemStyle = LegendItemStyle.Path;
					legendItem.Text = GetLegendText(field, customWidth.FromValueInt, customWidth.ToValueInt);
					legendItem.PathLineStyle = base.LineStyleInLegend;
					legendItem.BorderColor = BorderColorInLegend;
					legendItem.BorderWidth = base.BorderWidthInLegend;
					legendItem.Color = ColorInLegend;
					legendItem.SecondaryColor = SecondaryColorInLegend;
					legendItem.GradientType = GradientTypeInLegend;
					legendItem.HatchStyle = HatchStyleInLegend;
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
				foreach (CustomWidth customWidth3 in CustomWidths)
				{
					if (customWidth3.FromValueInt != null && customWidth3.ToValueInt != null && customWidth3.FromValueInt == path.Name)
					{
						path.ApplyCustomWidthAttributes(customWidth3);
						customWidth3.AffectedElements.Add(path);
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
			foreach (CustomWidth customWidth4 in CustomWidths)
			{
				object obj = field.Parse(customWidth4.FromValueInt);
				object obj2 = field.Parse(customWidth4.ToValueInt);
				if (IsValueInRange(field, testValue, obj, obj2))
				{
					path.ApplyCustomWidthAttributes(customWidth4);
					customWidth4.AffectedElements.Add(path);
					break;
				}
			}
		}
	}
}
