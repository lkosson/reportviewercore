using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class CoordinateFieldConverter : StringConverter
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
			StringCollection values = null;
			if (context != null && context.Instance != null && context.Instance is DataBindingRuleBase)
			{
				try
				{
					object obj = null;
					DataBindingRuleBase dataBindingRuleBase = (DataBindingRuleBase)context.Instance;
					obj = dataBindingRuleBase.DataSource;
					if (obj != null)
					{
						values = DataBindingHelper.GetDataSourceCoordinateFields(obj, dataBindingRuleBase.DataMember, dataBindingRuleBase.BindingField);
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
