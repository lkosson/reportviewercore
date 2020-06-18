using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(CustomWidthConverter))]
	internal class CustomWidth : NamedElement
	{
		private float width = 5f;

		private string fromValue = string.Empty;

		private string toValue = "";

		private string legendText = "";

		private string text = "#NAME";

		private string toolTip = "";

		private ArrayList affectedElements;

		private string fromValueInt = string.Empty;

		private string toValueInt = "";

		private bool visibleInt = true;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomWidth_Width")]
		[DefaultValue(5f)]
		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
				InvalidateRules();
			}
		}

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeCustomWidth_FromValue")]
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
		[SRDescription("DescriptionAttributeCustomWidth_ToValue")]
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
		[SRDescription("DescriptionAttributeCustomWidth_LegendText")]
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

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeCustomWidth_Text")]
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

		[SRCategory("CategoryAttribute_Misc")]
		[SRDescription("DescriptionAttributeCustomWidth_ToolTip")]
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

		internal ArrayList AffectedElements
		{
			get
			{
				return affectedElements;
			}
			set
			{
				affectedElements = value;
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

		public bool VisibleInt
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

		public CustomWidth()
			: this(null)
		{
		}

		internal CustomWidth(CommonElements common)
			: base(common)
		{
			affectedElements = new ArrayList();
		}

		public override string ToString()
		{
			return Name;
		}

		public ArrayList GetAffectedElements()
		{
			return AffectedElements;
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
