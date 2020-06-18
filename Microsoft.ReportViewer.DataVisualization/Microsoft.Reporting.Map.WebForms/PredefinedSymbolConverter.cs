using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PredefinedSymbolConverter : CollectionItemTypeConverter
	{
		public PredefinedSymbolConverter()
		{
			simpleType = typeof(PredefinedSymbol);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		private Field GetField(PredefinedSymbol predefinedSymbol)
		{
			return predefinedSymbol?.GetRule()?.GetField();
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, noCustomTypeDesc: false);
			Field field = GetField((PredefinedSymbol)value);
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			for (int i = 0; i < properties.Count; i++)
			{
				if (properties[i].IsBrowsable)
				{
					if (field != null && (properties[i].Name == "FromValue" || properties[i].Name == "ToValue"))
					{
						Attribute[] array = new Attribute[properties[i].Attributes.Count];
						properties[i].Attributes.CopyTo(array, 0);
						PredefinedSymbolPropertyDescriptor value2 = new PredefinedSymbolPropertyDescriptor(field, properties[i].Name, array);
						propertyDescriptorCollection.Add(value2);
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
