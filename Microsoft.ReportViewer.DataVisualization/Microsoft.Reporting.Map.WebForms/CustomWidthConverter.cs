using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class CustomWidthConverter : CollectionItemTypeConverter
	{
		public CustomWidthConverter()
		{
			simpleType = typeof(CustomWidth);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		private Field GetField(CustomWidth customWidth)
		{
			return customWidth?.GetRule()?.GetField();
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, noCustomTypeDesc: false);
			PathRule pathRule = (PathRule)value;
			Field field = GetField((CustomWidth)value);
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			for (int i = 0; i < properties.Count; i++)
			{
				if (properties[i].IsBrowsable)
				{
					if (field != null && (properties[i].Name == "FromValue" || properties[i].Name == "ToValue"))
					{
						Attribute[] array = new Attribute[properties[i].Attributes.Count];
						properties[i].Attributes.CopyTo(array, 0);
						CustomWidthPropertyDescriptor value2 = new CustomWidthPropertyDescriptor(field, properties[i].Name, array);
						propertyDescriptorCollection.Add(value2);
					}
					else if (properties[i].Name == "LegendText" || (properties[i].Name.EndsWith("InLegend", StringComparison.Ordinal) && pathRule.ShowInLegend == "(none)"))
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
