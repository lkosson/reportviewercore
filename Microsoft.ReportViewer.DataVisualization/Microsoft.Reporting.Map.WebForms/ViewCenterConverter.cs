using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ViewCenterConverter : ExpandableObjectConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(ViewCenter))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return ((ViewCenter)value).ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				_ = (string)value;
				string[] array = ((string)value).Split(',');
				if (array.Length == 2)
				{
					return new ViewCenter(null, float.Parse(array[0], CultureInfo.CurrentCulture), float.Parse(array[1], CultureInfo.CurrentCulture));
				}
				throw new ArgumentException("ElementPositionConverter - Incorrect string format. The X, Y of the location must be specified e.g. \"0,0\"");
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
