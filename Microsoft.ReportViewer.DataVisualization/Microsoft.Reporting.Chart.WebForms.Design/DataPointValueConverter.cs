using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class DataPointValueConverter : DoubleConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (context != null && context.Instance != null)
			{
				DataPoint dataPoint = (DataPoint)context.Instance;
				if (destinationType == typeof(string) && dataPoint.series.IsXValueDateTime())
				{
					return DateTime.FromOADate((double)value).ToString("g", CultureInfo.CurrentCulture);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (context != null && context.Instance != null)
			{
				DataPoint dataPoint = (DataPoint)context.Instance;
				if (value is string && dataPoint.series.IsXValueDateTime())
				{
					return DateTime.Parse((string)value, CultureInfo.CurrentCulture).ToOADate();
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
