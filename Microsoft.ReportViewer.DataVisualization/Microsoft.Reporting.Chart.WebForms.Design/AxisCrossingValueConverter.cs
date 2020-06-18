using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class AxisCrossingValueConverter : AxisMinMaxValueConverter
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
				double.NaN,
				double.MinValue,
				double.MaxValue
			});
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			double num = (double)value;
			if (destinationType == typeof(string))
			{
				if (double.IsNaN(num))
				{
					return "Auto";
				}
				if (num == double.MinValue)
				{
					return "Min";
				}
				if (num == double.MaxValue)
				{
					return "Max";
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string strA = (string)value;
				if (string.Compare(strA, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return double.NaN;
				}
				if (string.Compare(strA, "Min", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return double.MinValue;
				}
				if (string.Compare(strA, "Max", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return double.MaxValue;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
