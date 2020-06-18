using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class RectangleFConverter : TypeConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (context != null && context.Instance != null)
			{
				PropertyDescriptorCollection properties = base.GetProperties(context, value, attributes);
				for (int i = 0; i < properties.Count; i++)
				{
					if (properties[i].Name != "X" && properties[i].Name != "Y" && properties[i].Name != "Width" && properties[i].Name != "Height")
					{
						properties.RemoveAt(i);
						i--;
					}
				}
				return properties;
			}
			return base.GetProperties(context, value, attributes);
		}

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
			if (destinationType == typeof(RectangleF))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (context != null && context.Instance != null && destinationType == typeof(string))
			{
				RectangleF rectangleF = (RectangleF)value;
				return rectangleF.X.ToString(CultureInfo.InvariantCulture) + "," + rectangleF.Y.ToString(CultureInfo.InvariantCulture) + "," + rectangleF.Width.ToString(CultureInfo.InvariantCulture) + "," + rectangleF.Height.ToString(CultureInfo.InvariantCulture);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (context != null && context.Instance != null && value is string)
			{
				string[] array = ((string)value).Split(',');
				if (array.Length == 4)
				{
					return new RectangleF(float.Parse(array[0], CultureInfo.CurrentCulture), float.Parse(array[1], CultureInfo.CurrentCulture), float.Parse(array[2], CultureInfo.CurrentCulture), float.Parse(array[3], CultureInfo.CurrentCulture));
				}
				throw new ArgumentException(SR.ExceptionRectangleConverterStringFormatInvalid);
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
