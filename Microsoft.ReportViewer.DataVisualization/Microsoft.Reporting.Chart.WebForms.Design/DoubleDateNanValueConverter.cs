using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class DoubleDateNanValueConverter : DoubleConverter
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
				double.NaN
			});
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && double.IsNaN((double)value))
			{
				return "NotSet";
			}
			if (context != null && context.Instance != null)
			{
				Axis axis = null;
				if (context.Instance is AxisDataView)
				{
					axis = ((AxisDataView)context.Instance).axis;
				}
				else if (context.Instance is Cursor)
				{
					axis = ((Cursor)context.Instance).GetAxis();
				}
				if (axis != null && destinationType == typeof(string))
				{
					string text = ConvertDateTimeToString((double)value, axis.GetAxisValuesType(), axis.InternalIntervalType);
					if (text != null)
					{
						return text;
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public static string ConvertDateTimeToString(double dtValue, ChartValueTypes axisValuesType, DateTimeIntervalType dtIntervalType)
		{
			string text = null;
			if (dtIntervalType == DateTimeIntervalType.Auto)
			{
				if (axisValuesType == ChartValueTypes.DateTime || axisValuesType == ChartValueTypes.Time || axisValuesType == ChartValueTypes.Date || axisValuesType == ChartValueTypes.DateTimeOffset)
				{
					text = DateTime.FromOADate(dtValue).ToString("g", CultureInfo.CurrentCulture);
				}
			}
			else if (dtIntervalType != DateTimeIntervalType.Number)
			{
				text = ((dtIntervalType >= DateTimeIntervalType.Hours) ? DateTime.FromOADate(dtValue).ToString("g", CultureInfo.CurrentCulture) : DateTime.FromOADate(dtValue).ToShortDateString());
			}
			if (axisValuesType == ChartValueTypes.DateTimeOffset && text != null)
			{
				text += " +0";
			}
			return text;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			object obj = null;
			bool flag = false;
			if (value is string && string.Compare((string)value, "NotSet", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return double.NaN;
			}
			if (context != null && context.Instance != null && context.Instance is Axis)
			{
				Axis axis = null;
				if (context.Instance is AxisDataView)
				{
					axis = ((AxisDataView)context.Instance).axis;
				}
				else if (context.Instance is Cursor)
				{
					axis = ((Cursor)context.Instance).GetAxis();
				}
				if (axis != null && value is string)
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
