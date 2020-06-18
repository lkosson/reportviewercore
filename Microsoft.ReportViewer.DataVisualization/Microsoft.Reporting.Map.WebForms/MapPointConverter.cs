using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapPointConverter : ExpandableObjectConverter
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
				throw new ArgumentNullException("Destination type required.");
			}
			if (value is MapPoint && destinationType == typeof(string))
			{
				MapPoint mapPoint = (MapPoint)value;
				if (culture == null)
				{
					culture = CultureInfo.CurrentCulture;
				}
				string separator = culture.TextInfo.ListSeparator + " ";
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(double));
				string[] array = new string[2];
				int num = 0;
				array[num++] = converter.ConvertToString(context, culture, mapPoint.X);
				array[num++] = converter.ConvertToString(context, culture, mapPoint.Y);
				return string.Join(separator, array);
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				MapPoint mapPoint2 = (MapPoint)value;
				ConstructorInfo constructor = typeof(MapPoint).GetConstructor(new Type[2]
				{
					typeof(double),
					typeof(double)
				});
				if (constructor != null)
				{
					return new InstanceDescriptor(constructor, new object[2]
					{
						mapPoint2.X,
						mapPoint2.Y
					});
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text == null)
			{
				return base.ConvertFrom(context, culture, value);
			}
			string text2 = text.Trim();
			if (text2.Length == 0)
			{
				return null;
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			char c = culture.TextInfo.ListSeparator[0];
			string[] array = text2.Split(c);
			double[] array2 = new double[array.Length];
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = (double)converter.ConvertFromString(context, culture, array[i]);
			}
			if (array2.Length != 2)
			{
				throw new ArgumentException("Incorrect string format. The X, Y of the location must be specified e.g. \"0,0\"");
			}
			return new MapPoint(array2[0], array2[1]);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(MapPoint), attributes).Sort(new string[2]
			{
				"X",
				"Y"
			});
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues");
			}
			object obj = propertyValues["X"];
			object obj2 = propertyValues["Y"];
			if (obj == null || obj2 == null || !(obj is double) || !(obj2 is double))
			{
				throw new ArgumentException("Incorrect string format. The X, Y of the location must be specified e.g. \"0,0\"");
			}
			return new MapPoint((double)obj, (double)obj2);
		}
	}
}
