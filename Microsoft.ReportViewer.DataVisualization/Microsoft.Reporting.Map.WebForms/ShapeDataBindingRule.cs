using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(ShapeDataBindingRuleConverter))]
	internal class ShapeDataBindingRule : DataBindingRuleBase
	{
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeShapeDataBindingRule_BindingField")]
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

		public ShapeDataBindingRule()
			: this(null)
		{
		}

		internal ShapeDataBindingRule(CommonElements common)
			: base(common)
		{
		}

		internal override void DataBind()
		{
			if (Common != null)
			{
				Common.MapCore.ExecuteDataBind(BindingType.Shapes, this, base.DataSource, base.DataMember, BindingField);
			}
		}
	}
}
