using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class DoubleInfinityConverter : DoubleConverter
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
			ArrayList arrayList = new ArrayList();
			if (context != null)
			{
				foreach (Attribute attribute in context.PropertyDescriptor.Attributes)
				{
					if (attribute is DoubleConverterHint)
					{
						arrayList.Add(((DoubleConverterHint)attribute).Bound);
					}
				}
			}
			return new StandardValuesCollection(arrayList);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			double d = (double)value;
			if (destinationType == typeof(string) && double.IsNaN(d))
			{
				return "Auto";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string && ((string)value).Equals("Auto", StringComparison.OrdinalIgnoreCase))
			{
				return double.NaN;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
