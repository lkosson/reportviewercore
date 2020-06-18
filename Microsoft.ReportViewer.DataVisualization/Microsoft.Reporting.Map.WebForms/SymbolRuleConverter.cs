using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class SymbolRuleConverter : CollectionItemTypeConverter
	{
		public SymbolRuleConverter()
		{
			simpleType = typeof(SymbolRule);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, noCustomTypeDesc: false);
			SymbolRule symbolRule = (SymbolRule)value;
			symbolRule.GetField();
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			for (int i = 0; i < properties.Count; i++)
			{
				if (properties[i].IsBrowsable)
				{
					if (properties[i].Name == "LegendText" && symbolRule.ShowInLegend == "(none)")
					{
						propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), properties[i], new ReadOnlyAttribute(isReadOnly: true)));
					}
					else
					{
						propertyDescriptorCollection.Add(properties[i]);
					}
				}
			}
			return propertyDescriptorCollection;
		}
	}
}
