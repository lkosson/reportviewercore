using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class AxisIntervalValueConverter : DoubleConverter
	{
		internal bool hideNotSet = true;

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
			if (!hideNotSet)
			{
				arrayList.Add(double.NaN);
			}
			arrayList.Add(0.0);
			return new StandardValuesCollection(arrayList);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			double num = (double)value;
			if (destinationType == typeof(string))
			{
				if (double.IsNaN(num))
				{
					return "NotSet";
				}
				if (num == 0.0)
				{
					return "Auto";
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
					return 0.0;
				}
				if (string.Compare(strA, "NotSet", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return double.NaN;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
