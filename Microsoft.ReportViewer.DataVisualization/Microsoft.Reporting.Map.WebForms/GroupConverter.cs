using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class GroupConverter : CollectionItemTypeConverter
	{
		public GroupConverter()
		{
			simpleType = typeof(Group);
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
			MapCore mapCore = ((Group)value).GetMapCore();
			if (mapCore != null)
			{
				foreach (Field groupField in mapCore.GroupFields)
				{
					ArrayList arrayList = new ArrayList();
					arrayList.Add(new CategoryAttribute(SR.CategoryAttribute_GroupFields));
					arrayList.Add(new DescriptionAttribute(SR.DescriptionAttributeGroup_Fields(groupField.Name)));
					GroupFieldPropertyDescriptor value2 = new GroupFieldPropertyDescriptor(groupField, (Attribute[])arrayList.ToArray(typeof(Attribute)));
					propertyDescriptorCollection.Add(value2);
				}
				return propertyDescriptorCollection;
			}
			return propertyDescriptorCollection;
		}
	}
}
