using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MarginExpandableObjectConverter : ExpandableObjectConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			Margins margins = value as Margins;
			if (destinationType == typeof(string) && margins != null)
			{
				return string.Format(CultureInfo.CurrentCulture, "{0:D}, {1:D}, {2:D}, {3:D}", margins.Top, margins.Bottom, margins.Left, margins.Right);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				Margins margins = new Margins();
				string[] array = ((string)value).Split(',');
				if (array.Length == 4)
				{
					try
					{
						margins.Top = int.Parse(array[0].Trim(), CultureInfo.CurrentCulture);
						margins.Bottom = int.Parse(array[1].Trim(), CultureInfo.CurrentCulture);
						margins.Left = int.Parse(array[2].Trim(), CultureInfo.CurrentCulture);
						margins.Right = int.Parse(array[3].Trim(), CultureInfo.CurrentCulture);
						return margins;
					}
					catch
					{
						throw new InvalidOperationException("Can not convert string'" + (string)value + "'to Margins object.");
					}
				}
				throw new InvalidOperationException("Can not convert string'" + (string)value + "'to Margins object.");
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
