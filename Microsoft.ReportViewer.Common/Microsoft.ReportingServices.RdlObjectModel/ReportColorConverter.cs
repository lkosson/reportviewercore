using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal sealed class ReportColorConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string) || sourceType == typeof(Color))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				return new ReportColor(((string)value).Trim());
			}
			if (value is Color)
			{
				return new ReportColor((Color)value);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (!(destinationType == typeof(string)))
			{
				return destinationType == typeof(Color);
			}
			return true;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is ReportColor)
			{
				if (destinationType == typeof(string))
				{
					return ((ReportColor)value).ToString();
				}
				if (destinationType == typeof(Color) && value is ReportColor)
				{
					return ((ReportColor)value).Color;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
