using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class SymbolConverter : CollectionItemTypeConverter
	{
		public SymbolConverter()
		{
			simpleType = typeof(Symbol);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, noCustomTypeDesc: false);
			Symbol symbol = (Symbol)value;
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			for (int i = 0; i < properties.Count; i++)
			{
				if (properties[i].IsBrowsable)
				{
					if (symbol.ParentShape != "(none)" && (properties[i].Name == "X" || properties[i].Name == "Y"))
					{
						propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), properties[i], new ReadOnlyAttribute(isReadOnly: true)));
					}
					else
					{
						propertyDescriptorCollection.Add(properties[i]);
					}
				}
			}
			MapCore mapCore = symbol.GetMapCore();
			if (mapCore != null)
			{
				foreach (Field symbolField in mapCore.SymbolFields)
				{
					SymbolFieldPropertyDescriptor value2 = new SymbolFieldPropertyDescriptor(symbolField);
					propertyDescriptorCollection.Add(value2);
				}
				return propertyDescriptorCollection;
			}
			return propertyDescriptorCollection;
		}
	}
}
