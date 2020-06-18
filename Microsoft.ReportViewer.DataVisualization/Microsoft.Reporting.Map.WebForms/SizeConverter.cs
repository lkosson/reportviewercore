using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class SizeConverter : ExpandableObjectConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(MapSize))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				MapSize mapSize = (MapSize)value;
				if (mapSize.AutoSize)
				{
					return "Auto";
				}
				return mapSize.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				_ = (string)value;
				string[] array = ((string)value).Split(',');
				if (array.Length == 2)
				{
					return new MapSize(null, float.Parse(array[0], CultureInfo.CurrentCulture), float.Parse(array[1], CultureInfo.CurrentCulture));
				}
				throw new ArgumentException("Element SizeConverter - Incorrect string format. The Width, Height of the size must be specified e.g. \"100,100\"");
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (context != null && ((MapSize)value).AutoSize)
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
			return base.GetProperties(context, value, attributes);
		}
	}
}
