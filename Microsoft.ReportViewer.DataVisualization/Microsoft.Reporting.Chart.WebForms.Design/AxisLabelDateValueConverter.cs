using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class AxisLabelDateValueConverter : DoubleConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (context != null && context.Instance != null && destinationType == typeof(string))
			{
				DateTimeIntervalType dateTimeIntervalType = DateTimeIntervalType.Auto;
				double num = 0.0;
				PropertyInfo property = context.Instance.GetType().GetProperty("IntervalType");
				if (property != null)
				{
					dateTimeIntervalType = (DateTimeIntervalType)property.GetValue(context.Instance, null);
				}
				property = context.Instance.GetType().GetProperty("Interval");
				if (property != null)
				{
					num = (double)property.GetValue(context.Instance, null);
				}
				if (dateTimeIntervalType == DateTimeIntervalType.Auto)
				{
					Axis axis = null;
					if (context.Instance is Axis)
					{
						axis = (Axis)context.Instance;
					}
					else
					{
						MethodInfo method = context.Instance.GetType().GetMethod("GetAxis");
						if (method != null)
						{
							axis = (Axis)method.Invoke(context.Instance, null);
						}
					}
					if (axis != null)
					{
						dateTimeIntervalType = axis.GetAxisIntervalType();
					}
				}
				if ((context.Instance.GetType() != typeof(StripLine) || num == 0.0) && dateTimeIntervalType != DateTimeIntervalType.Number && dateTimeIntervalType != 0)
				{
					if (dateTimeIntervalType < DateTimeIntervalType.Hours)
					{
						return DateTime.FromOADate((double)value).ToShortDateString();
					}
					return DateTime.FromOADate((double)value).ToString("g", CultureInfo.CurrentCulture);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			object obj = null;
			bool flag = false;
			if (context != null && context.Instance != null)
			{
				DateTimeIntervalType dateTimeIntervalType = DateTimeIntervalType.Auto;
				PropertyInfo property = context.Instance.GetType().GetProperty("intervalType");
				if (property != null)
				{
					dateTimeIntervalType = (DateTimeIntervalType)property.GetValue(context.Instance, null);
				}
				if (dateTimeIntervalType == DateTimeIntervalType.Auto)
				{
					Axis axis = null;
					if (context.Instance is Axis)
					{
						axis = (Axis)context.Instance;
					}
					else
					{
						MethodInfo method = context.Instance.GetType().GetMethod("GetAxis");
						if (method != null)
						{
							axis = (Axis)method.Invoke(context.Instance, null);
						}
					}
					if (axis != null)
					{
						dateTimeIntervalType = axis.GetAxisIntervalType();
					}
				}
				if (value is string && dateTimeIntervalType != DateTimeIntervalType.Number && dateTimeIntervalType != 0)
				{
					flag = true;
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
