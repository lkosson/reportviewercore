using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.NETCore
{
	[ComVisible(false)]
	internal sealed class TypeNameHidingExpandableObjectConverter : ExpandableObjectConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return string.Empty;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
