using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class NoNameExpandableObjectConverter : ExpandableObjectConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (context != null && context.Instance != null && destinationType == typeof(string))
			{
				return "";
			}
			return base.ConvertTo(context, culture, value, destinationType);
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
