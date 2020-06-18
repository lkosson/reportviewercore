using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapAreaCoordinatesConverter : ArrayConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string[] array = ((string)value).Split(',');
				int[] array2 = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = int.Parse(array[i], CultureInfo.CurrentCulture);
				}
				return array2;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				int[] obj = (int[])value;
				string text = "";
				int[] array = obj;
				foreach (int num in array)
				{
					text = text + num.ToString(CultureInfo.CurrentCulture) + ",";
				}
				return text.TrimEnd(',');
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
