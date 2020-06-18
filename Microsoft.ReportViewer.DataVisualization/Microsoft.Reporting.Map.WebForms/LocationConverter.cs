using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class LocationConverter : ExpandableObjectConverter
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
			if (destinationType == typeof(MapLocation))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				MapLocation mapLocation = (MapLocation)value;
				if (mapLocation.Docked)
				{
					return "Docked";
				}
				return mapLocation.ToString();
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
					return new MapLocation(null, float.Parse(array[0], CultureInfo.CurrentCulture), float.Parse(array[1], CultureInfo.CurrentCulture));
				}
				throw new ArgumentException("ElementPositionConverter - Incorrect string format. The X, Y of the location must be specified e.g. \"0,0\"");
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (context != null && ((MapLocation)value).Docked)
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
