using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PathConverter : CollectionItemTypeConverter
	{
		public PathConverter()
		{
			simpleType = typeof(Path);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, noCustomTypeDesc: false);
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			for (int i = 0; i < properties.Count; i++)
			{
				if (properties[i].IsBrowsable)
				{
					propertyDescriptorCollection.Add(properties[i]);
				}
			}
			MapCore mapCore = ((Path)value).GetMapCore();
			if (mapCore != null)
			{
				foreach (Field pathField in mapCore.PathFields)
				{
					ArrayList arrayList = new ArrayList();
					arrayList.Add(new CategoryAttribute(SR.CategoryAttribute_PathFields));
					arrayList.Add(new DescriptionAttribute(SR.DescriptionAttributePath_Fields(pathField.Name)));
					PathFieldPropertyDescriptor value2 = new PathFieldPropertyDescriptor(pathField, (Attribute[])arrayList.ToArray(typeof(Attribute)));
					propertyDescriptorCollection.Add(value2);
				}
				return propertyDescriptorCollection;
			}
			return propertyDescriptorCollection;
		}
	}
}
