using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class FieldConverter : CollectionItemTypeConverter
	{
		public FieldConverter()
		{
			simpleType = typeof(Field);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (context != null && ((Field)value).IsTemporary)
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, new Attribute[1]
				{
					new BrowsableAttribute(browsable: true)
				});
				PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
				{
					foreach (PropertyDescriptor item in properties)
					{
						propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), item, new ReadOnlyAttribute(isReadOnly: true)));
					}
					return propertyDescriptorCollection;
				}
			}
			return TypeDescriptor.GetProperties(value, new Attribute[1]
			{
				new BrowsableAttribute(browsable: true)
			});
		}
	}
}
