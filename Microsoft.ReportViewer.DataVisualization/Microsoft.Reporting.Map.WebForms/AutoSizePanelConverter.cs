using System;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class AutoSizePanelConverter : DockablePanelConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (context != null && value is AutoSizePanel && ((AutoSizePanel)value).AutoSize)
			{
				PropertyDescriptorCollection properties = base.GetProperties(context, value, attributes);
				PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
				{
					foreach (PropertyDescriptor item in properties)
					{
						if (item.Name == "Size")
						{
							propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), item, new ReadOnlyAttribute(isReadOnly: true)));
						}
						else
						{
							propertyDescriptorCollection.Add(item);
						}
					}
					return propertyDescriptorCollection;
				}
			}
			return base.GetProperties(context, value, attributes);
		}
	}
}
