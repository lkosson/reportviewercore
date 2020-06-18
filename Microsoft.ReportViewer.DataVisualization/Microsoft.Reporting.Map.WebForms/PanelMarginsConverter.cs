using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PanelMarginsConverter : TypeConverter
	{
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
			if (destinationType == null)
			{
				throw new ArgumentNullException(SR.no_destination_type);
			}
			if (value is PanelMargins && destinationType == typeof(string))
			{
				PanelMargins panelMargins = (PanelMargins)value;
				if (culture == null)
				{
					culture = CultureInfo.CurrentCulture;
				}
				string listSeparator = culture.TextInfo.ListSeparator;
				return string.Format(culture, "{1}{0} {2}{0} {3}{0} {4}", listSeparator, panelMargins.Left.ToString(culture), panelMargins.Top.ToString(culture), panelMargins.Right.ToString(culture), panelMargins.Bottom.ToString(culture));
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text == null)
			{
				return base.ConvertFrom(context, culture, value);
			}
			text = text.Trim();
			if (text.Length == 0)
			{
				return base.ConvertFrom(context, culture, text);
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			char c = culture.TextInfo.ListSeparator[0];
			string[] array = text.Split(c);
			if (array.Length != 4)
			{
				throw new ArgumentException(SR.wrong_padding_format);
			}
			return new PanelMargins(int.Parse(array[0], culture), int.Parse(array[1], culture), int.Parse(array[2], culture), int.Parse(array[3], culture));
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(PanelMargins), attributes).Sort(new string[5]
			{
				"All",
				"Left",
				"Top",
				"Right",
				"Bottom"
			});
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
