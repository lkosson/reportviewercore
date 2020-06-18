using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(SymbolDataBindingRuleConverter))]
	internal class SymbolDataBindingRule : DataBindingRuleBase
	{
		private string category = "";

		private string xCoordinateField;

		private string yCoordinateField;

		private string parentShapeField;

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolDataBindingRule_BindingField")]
		public override string BindingField
		{
			get
			{
				return base.BindingField;
			}
			set
			{
				base.BindingField = value;
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolDataBindingRule_Category")]
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
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolDataBindingRule_XCoordinateField")]
		[DefaultValue("")]
		[TypeConverter(typeof(CoordinateFieldConverter))]
		public string XCoordinateField
		{
			get
			{
				return xCoordinateField;
			}
			set
			{
				xCoordinateField = value;
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolDataBindingRule_YCoordinateField")]
		[DefaultValue("")]
		[TypeConverter(typeof(CoordinateFieldConverter))]
		public string YCoordinateField
		{
			get
			{
				return yCoordinateField;
			}
			set
			{
				yCoordinateField = value;
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeSymbolDataBindingRule_ParentShapeField")]
		[DefaultValue("")]
		[TypeConverter(typeof(CoordinateFieldConverter))]
		public string ParentShapeField
		{
			get
			{
				return parentShapeField;
			}
			set
			{
				parentShapeField = value;
			}
		}

		public SymbolDataBindingRule()
			: this(null)
		{
		}

		internal SymbolDataBindingRule(CommonElements common)
			: base(common)
		{
		}

		internal override void DataBind()
		{
			if (Common != null)
			{
				Common.MapCore.ExecuteDataBind(BindingType.Symbols, this, base.DataSource, base.DataMember, BindingField, Category, ParentShapeField, XCoordinateField, YCoordinateField);
			}
		}

		internal override void UpdateDataFields(string dataMember, int dataMemberIndex, StringCollection dataFields)
		{
			base.UpdateDataFields(dataMember, dataMemberIndex, dataFields);
			if (base.DataMember == dataMember || (string.IsNullOrEmpty(base.DataMember) && dataMemberIndex == 0))
			{
				if (!dataFields.Contains(XCoordinateField))
				{
					XCoordinateField = "";
				}
				if (!dataFields.Contains(YCoordinateField))
				{
					YCoordinateField = "";
				}
				if (!dataFields.Contains(ParentShapeField))
				{
					ParentShapeField = "";
				}
			}
		}
	}
}
