using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class FloatAutoValueConverter : SingleConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new ArrayList
			{
				float.NaN
			});
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			float f = (float)value;
			if (destinationType == typeof(string) && float.IsNaN(f))
			{
				return "Auto";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string && string.Compare((string)value, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return float.NaN;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
