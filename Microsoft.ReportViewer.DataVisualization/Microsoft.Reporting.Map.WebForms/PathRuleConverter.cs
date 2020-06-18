using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PathRuleConverter : CollectionItemTypeConverter
	{
		public PathRuleConverter()
		{
			simpleType = typeof(PathRule);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, noCustomTypeDesc: false);
			PathRule pathRule = (PathRule)value;
			Field field = pathRule.GetField();
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			for (int i = 0; i < properties.Count; i++)
			{
				if (!properties[i].IsBrowsable)
				{
					continue;
				}
				if (pathRule.UseCustomColors && (properties[i].Name == "ColorCount" || properties[i].Name == "ColoringMode" || properties[i].Name == "ColorPalette" || properties[i].Name == "FromColor" || properties[i].Name == "MiddleColor" || properties[i].Name == "ToColor" || properties[i].Name == "BorderColor" || properties[i].Name == "GradientType" || properties[i].Name == "HatchStyle" || properties[i].Name == "Text" || properties[i].Name == "ToolTip" || properties[i].Name == "SecondaryColor"))
				{
					propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), properties[i], new ReadOnlyAttribute(isReadOnly: true)));
				}
				else if (properties[i].Name == "ColorPalette" && pathRule.ColoringMode != ColoringMode.DistinctColors)
				{
					propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), properties[i], new ReadOnlyAttribute(isReadOnly: true)));
				}
				else if ((properties[i].Name == "FromColor" || properties[i].Name == "MiddleColor" || properties[i].Name == "ToColor") && pathRule.ColoringMode == ColoringMode.DistinctColors)
				{
					propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), properties[i], new ReadOnlyAttribute(isReadOnly: true)));
				}
				else if (properties[i].Name == "CustomColors" && !pathRule.UseCustomColors)
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
						PathRulePropertyDescriptor value2 = new PathRulePropertyDescriptor(field, properties[i].Name, array);
						propertyDescriptorCollection.Add(value2);
					}
					else
					{
						propertyDescriptorCollection.Add(TypeDescriptor.CreateProperty(value.GetType(), properties[i], new ReadOnlyAttribute(isReadOnly: true)));
					}
				}
				else if (pathRule.ShowInLegend == "(none)" && properties[i].Name != "ShowInLegend" && (properties[i].Name == "LegendText" || properties[i].Name.EndsWith("InLegend", StringComparison.Ordinal)))
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
