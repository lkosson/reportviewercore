using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class AxisMinMaxValueConverter : DoubleConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (context != null && context.Instance != null && context.Instance is Axis)
			{
				Axis axis = (Axis)context.Instance;
				if (destinationType == typeof(string))
				{
					string text = DoubleDateNanValueConverter.ConvertDateTimeToString((double)value, axis.GetAxisValuesType(), axis.InternalIntervalType);
					if (text != null)
					{
						return text;
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			object obj = null;
			bool flag = false;
			if (context != null && context.Instance != null && context.Instance is Axis)
			{
				Axis axis = (Axis)context.Instance;
				if (value is string)
				{
					if (axis.InternalIntervalType == DateTimeIntervalType.Auto)
					{
						if (axis.GetAxisValuesType() == ChartValueTypes.DateTime || axis.GetAxisValuesType() == ChartValueTypes.Date || axis.GetAxisValuesType() == ChartValueTypes.Time || axis.GetAxisValuesType() == ChartValueTypes.DateTimeOffset)
						{
							flag = true;
						}
					}
					else if (axis.InternalIntervalType != DateTimeIntervalType.Number)
					{
						flag = true;
					}
				}
			}
			try
			{
				obj = base.ConvertFrom(context, culture, value);
			}
			catch (Exception)
			{
				obj = null;
			}
			if (value is string && (flag || obj == null))
			{
				try
				{
					obj = DateTime.Parse((string)value, CultureInfo.CurrentCulture).ToOADate();
					if (obj != null)
					{
						return obj;
					}
				}
				catch (Exception)
				{
					obj = null;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
