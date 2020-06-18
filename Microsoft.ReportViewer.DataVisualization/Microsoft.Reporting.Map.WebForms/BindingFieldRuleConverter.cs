using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class BindingFieldRuleConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList values = new ArrayList();
			if (context != null && context.Instance != null && context.Instance is DataBindingRuleBase)
			{
				try
				{
					object obj = null;
					DataBindingRuleBase dataBindingRuleBase = (DataBindingRuleBase)context.Instance;
					obj = dataBindingRuleBase.DataSource;
					if (obj != null)
					{
						values = DataBindingHelper.GetDataSourceDataFields(obj, dataBindingRuleBase.DataMember, string.Empty);
					}
				}
				catch
				{
				}
			}
			return new StandardValuesCollection(values);
		}
	}
}
