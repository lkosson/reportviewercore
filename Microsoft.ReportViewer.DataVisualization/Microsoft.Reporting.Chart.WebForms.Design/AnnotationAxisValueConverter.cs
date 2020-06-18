using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class AnnotationAxisValueConverter : TypeConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value == null)
				{
					return "NotSet";
				}
				if (value is Axis)
				{
					Axis axis = (Axis)value;
					if (axis.chartArea != null)
					{
						return axis.chartArea.Name + " - " + axis.Name;
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
