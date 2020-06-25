using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class FrameAttributesConverter : NoNameExpandableObjectConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (context != null)
			{
				Frame frame = value as Frame;
				if (frame != null && frame.ShouldRenderReadOnly())
				{
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, new Attribute[1]
					{
						new BrowsableAttribute(browsable: true)
					});
					PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
					{
						foreach (PropertyDescriptor item in properties)
						{
							if (item.Name != "FrameStyle" && (item.Name != "PageColor" || frame.FrameStyle == FrameStyle.None))
							{
								if (item.Name != "BackImage")
								{
									propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), item, new ReadOnlyAttribute(isReadOnly: true)));
								}
								else
								{
									propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), item, new ReadOnlyAttribute(isReadOnly: true)));
								}
							}
							else
							{
								propertyDescriptorCollection.Add(item);
							}
						}
						return propertyDescriptorCollection;
					}
				}
			}
			return base.GetProperties(context, value, attributes);
		}
	}
}
