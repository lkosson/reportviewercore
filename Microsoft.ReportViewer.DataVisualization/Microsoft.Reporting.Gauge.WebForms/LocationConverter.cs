using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
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
			if (destinationType == typeof(GaugeLocation))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return ((GaugeLocation)value).ToString();
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
					return new GaugeLocation(null, float.Parse(array[0], CultureInfo.CurrentCulture), float.Parse(array[1], CultureInfo.CurrentCulture));
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionLocationFormat"));
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
