using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class BackFrameConverter : NoNameExpandableObjectConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, noCustomTypeDesc: false);
			BackFrame backFrame = (BackFrame)value;
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			for (int i = 0; i < properties.Count; i++)
			{
				if (properties[i].IsBrowsable)
				{
					if (backFrame.IsCustomXamlFrame() && (properties[i].Name == "FrameWidth" || properties[i].Name == "FrameGradientEndColor" || properties[i].Name == "FrameGradientType" || properties[i].Name == "FrameHatchStyle" || properties[i].Name == "BackGradientEndColor" || properties[i].Name == "BackGradientType" || properties[i].Name == "BackHatchStyle"))
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
