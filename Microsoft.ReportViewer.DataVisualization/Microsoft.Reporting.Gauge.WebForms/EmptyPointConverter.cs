using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class EmptyPointConverter : PointConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (context != null && context.Instance != null && destinationType == typeof(string) && ((Point)value).IsEmpty)
			{
				return "Not set";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string && string.Compare((string)value, "Not set", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return Point.Empty;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null && context.Instance is GaugeContainer)
			{
				GaugeContainer.controlCurrentContext = context;
			}
			return base.GetPropertiesSupported(context);
		}
	}
}
