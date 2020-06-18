using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PathWidthRuleConverter : CollectionItemTypeConverter
	{
		public PathWidthRuleConverter()
		{
			simpleType = typeof(PathWidthRule);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, noCustomTypeDesc: false);
			PathWidthRule pathWidthRule = (PathWidthRule)value;
			Field field = pathWidthRule.GetField();
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			for (int i = 0; i < properties.Count; i++)
			{
				if (!properties[i].IsBrowsable)
				{
					continue;
				}
				if (pathWidthRule.UseCustomWidths && (properties[i].Name == "WidthCount" || properties[i].Name == "FromWidth" || properties[i].Name == "ToWidth" || properties[i].Name == "Text" || properties[i].Name == "ToolTip"))
				{
					propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), properties[i], new ReadOnlyAttribute(isReadOnly: true)));
				}
				else if (properties[i].Name == "CustomWidths" && !pathWidthRule.UseCustomWidths)
				{
					ArrayList arrayList = new ArrayList();
					foreach (Attribute attribute in properties[i].Attributes)
					{
						if (attribute is CategoryAttribute || attribute is DescriptionAttribute)
						{
							arrayList.Add(attribute);
						}
					}
					propertyDescriptorCollection.Add(new ReadOnlyCollectionDescriptor(properties[i].Name, (Attribute[])arrayList.ToArray(typeof(Attribute))));
				}
				else if (field != null && (properties[i].Name == "FromValue" || properties[i].Name == "ToValue"))
				{
					if (field.IsNumeric())
					{
						Attribute[] array = new Attribute[properties[i].Attributes.Count];
						properties[i].Attributes.CopyTo(array, 0);
						PathWidthRulePropertyDescriptor value2 = new PathWidthRulePropertyDescriptor(field, properties[i].Name, array);
						propertyDescriptorCollection.Add(value2);
					}
					else
					{
						propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), properties[i], new ReadOnlyAttribute(isReadOnly: true)));
					}
				}
				else if (pathWidthRule.ShowInLegend == "(none)" && properties[i].Name != "ShowInLegend" && (properties[i].Name == "LegendText" || properties[i].Name.EndsWith("InLegend", StringComparison.Ordinal)))
				{
					propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), properties[i], new ReadOnlyAttribute(isReadOnly: true)));
				}
				else
				{
					propertyDescriptorCollection.Add(properties[i]);
				}
			}
			return propertyDescriptorCollection;
		}
	}
}
