using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class ColorArrayConverter : ExpandableObjectConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return ColorArrayToString(value as Color[]);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				return StringToColorArray((string)value);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public static string ColorArrayToString(Color[] colors)
		{
			if (colors != null && colors.GetLength(0) > 0)
			{
				ColorConverter colorConverter = new ColorConverter();
				string text = string.Empty;
				foreach (Color color in colors)
				{
					if (text.Length > 0)
					{
						text += "; ";
					}
					text += colorConverter.ConvertToInvariantString(color);
				}
				return text;
			}
			return string.Empty;
		}

		public static Color[] StringToColorArray(string colorNames)
		{
			ColorConverter colorConverter = new ColorConverter();
			Color[] array = new Color[0];
			if (colorNames.Length > 0)
			{
				string[] array2 = colorNames.Split(';');
				array = new Color[array2.Length];
				int num = 0;
				string[] array3 = array2;
				foreach (string text in array3)
				{
					array[num++] = (Color)colorConverter.ConvertFromInvariantString(text);
				}
			}
			return array;
		}
	}
}
