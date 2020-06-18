using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class AnchorPointValueConverter : TypeConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value == null)
				{
					return "NotSet";
				}
				if (value is DataPoint)
				{
					DataPoint dataPoint = (DataPoint)value;
					if (dataPoint.series != null)
					{
						return string.Concat(str3: (dataPoint.series.Points.IndexOf(dataPoint) + 1).ToString(CultureInfo.InvariantCulture), str0: dataPoint.series.Name, str1: " - ", str2: SR.DescriptionTypePoint);
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
