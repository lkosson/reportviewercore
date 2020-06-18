using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DataBindingRuleBase : NamedElement
	{
		private string dataMember = string.Empty;

		private string bindingField = string.Empty;

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeDataBindingRuleBase_DataMember")]
		[DefaultValue("")]
		[TypeConverter(typeof(DataMemberConverter))]
		public string DataMember
		{
			get
			{
				return dataMember;
			}
			set
			{
				if (dataMember != value)
				{
					dataMember = value;
					if (Common != null)
					{
						Common.MapCore.InvalidateDataBinding();
						Common.MapCore.Invalidate();
					}
				}
			}
		}

		[TypeConverter(typeof(BindingFieldRuleConverter))]
		[DefaultValue("")]
		public virtual string BindingField
		{
			get
			{
				return bindingField;
			}
			set
			{
				if (bindingField != value)
				{
					bindingField = value;
					if (Common != null)
					{
						Common.MapCore.InvalidateDataBinding();
						Common.MapCore.Invalidate();
					}
				}
			}
		}

		internal object DataSource
		{
			get
			{
				if (Common != null)
				{
					return Common.MapCore.DataSource;
				}
				return null;
			}
		}

		public DataBindingRuleBase()
			: this(null)
		{
		}

		internal DataBindingRuleBase(CommonElements common)
			: base(common)
		{
		}

		internal virtual void DataBind()
		{
		}

		internal virtual void Reset()
		{
			if (DataSource != null)
			{
				DataMember = DataBindingHelper.GetDataSourceDefaultDataMember(DataSource);
			}
			else
			{
				DataMember = "";
			}
			BindingField = "";
		}

		internal virtual void UpdateDataMember(StringCollection dataMembers)
		{
			if (!string.IsNullOrEmpty(DataMember) && !dataMembers.Contains(DataMember))
			{
				Reset();
			}
		}

		internal virtual void UpdateDataFields(string dataMember, int dataMemberIndex, StringCollection dataFields)
		{
			if ((DataMember == dataMember || (string.IsNullOrEmpty(DataMember) && dataMemberIndex == 0)) && !dataFields.Contains(BindingField))
			{
				BindingField = "";
			}
		}
	}
}
