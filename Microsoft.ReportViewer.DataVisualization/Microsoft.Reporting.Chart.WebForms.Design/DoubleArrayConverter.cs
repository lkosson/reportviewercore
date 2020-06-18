using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class DoubleArrayConverter : ArrayConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			object obj = null;
			bool flag = false;
			if (context != null && context.Instance != null)
			{
				DataPoint dataPoint = (DataPoint)context.Instance;
				if (dataPoint.series != null && dataPoint.series.IsYValueDateTime())
				{
					flag = true;
				}
			}
			if (value is string)
			{
				string[] array = ((string)value).Split(',');
				double[] array2 = new double[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					if (flag)
					{
						try
						{
							obj = DateTime.Parse(array[i], CultureInfo.InvariantCulture).ToOADate();
						}
						catch (Exception)
						{
							try
							{
								obj = DateTime.Parse(array[i], CultureInfo.CurrentCulture).ToOADate();
							}
							catch (Exception)
							{
								obj = null;
							}
						}
					}
					if (obj != null)
					{
						array2[i] = (double)obj;
					}
					else
					{
						array2[i] = CommonElements.ParseDouble(array[i]);
					}
				}
				return array2;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			bool flag = false;
			if (context != null && context.Instance != null)
			{
				DataPoint dataPoint = (DataPoint)context.Instance;
				if (dataPoint.series != null && dataPoint.series.IsYValueDateTime())
				{
					flag = true;
				}
			}
			if (destinationType == typeof(string))
			{
				double[] obj = (double[])value;
				string text = "";
				double[] array = obj;
				for (int i = 0; i < array.Length; i++)
				{
					double d = array[i];
					text = ((!flag) ? (text + d.ToString(CultureInfo.InvariantCulture) + ",") : (text + DateTime.FromOADate(d).ToString("g", CultureInfo.InvariantCulture) + ","));
				}
				return text.TrimEnd(',');
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
