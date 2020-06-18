using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(PathDataBindingRuleConverter))]
	internal class PathDataBindingRule : DataBindingRuleBase
	{
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributePathDataBindingRule_BindingField")]
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

		public PathDataBindingRule()
			: this(null)
		{
		}

		internal PathDataBindingRule(CommonElements common)
			: base(common)
		{
		}

		internal override void DataBind()
		{
			if (Common != null)
			{
				Common.MapCore.ExecuteDataBind(BindingType.Paths, this, base.DataSource, base.DataMember, BindingField);
			}
		}
	}
}
