using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class LinearPointerConverter : CollectionItemTypeConverter
	{
		public LinearPointerConverter()
		{
			simpleType = typeof(LinearPointer);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, noCustomTypeDesc: false);
			LinearPointer linearPointer = (LinearPointer)value;
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			for (int i = 0; i < properties.Count; i++)
			{
				if (properties[i].IsBrowsable)
				{
					if (linearPointer.Type == LinearPointerType.Thermometer && (properties[i].Name == "MarkerLength" || properties[i].Name == "MarkerStyle" || properties[i].Name == "BarStart"))
					{
						propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), properties[i], new ReadOnlyAttribute(isReadOnly: true)));
					}
					else if (linearPointer.Type == LinearPointerType.Marker && (properties[i].Name == "BarStart" || properties[i].Name == "NeedleStyle" || properties[i].Name.StartsWith("Thermometer", StringComparison.Ordinal)))
					{
						propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), properties[i], new ReadOnlyAttribute(isReadOnly: true)));
					}
					else if (linearPointer.Type == LinearPointerType.Bar && (properties[i].Name == "MarkerLength" || properties[i].Name == "MarkerStyle" || properties[i].Name == "NeedleStyle" || properties[i].Name.StartsWith("Thermometer", StringComparison.Ordinal)))
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
