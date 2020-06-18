using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapCoordinateConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string) || sourceType == typeof(double))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				return new MapCoordinate((string)value);
			}
			if (value is double)
			{
				return new MapCoordinate((double)value);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == typeof(string) && value is MapCoordinate)
			{
				return ((MapCoordinate)value).ToString();
			}
			if (destinationType == typeof(InstanceDescriptor) && value is MapCoordinate)
			{
				MapCoordinate mapCoordinate = (MapCoordinate)value;
				ConstructorInfo constructor = typeof(MapCoordinate).GetConstructor(new Type[1]
				{
					typeof(double)
				});
				if (constructor != null)
				{
					double num = mapCoordinate.ToDouble();
					return new InstanceDescriptor(constructor, new object[1]
					{
						num
					});
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
