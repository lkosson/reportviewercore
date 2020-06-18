using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class ElementPositionConverter : ExpandableObjectConverter
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
			if (destinationType == typeof(ElementPosition))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return ((ElementPosition)value).ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				if (string.Compare((string)value, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return new ElementPosition();
				}
				string[] array = ((string)value).Split(',');
				if (array.Length == 4)
				{
					return new ElementPosition(float.Parse(array[0], CultureInfo.CurrentCulture), float.Parse(array[1], CultureInfo.CurrentCulture), float.Parse(array[2], CultureInfo.CurrentCulture), float.Parse(array[3], CultureInfo.CurrentCulture));
				}
				throw new ArgumentException(SR.ExceptionElementPositionConverter);
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
