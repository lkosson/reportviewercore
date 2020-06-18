using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CollectionItemTypeConverter : TypeConverter
	{
		internal Type simpleType = typeof(object);

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return new InstanceDescriptor(simpleType.GetConstructor(Type.EmptyTypes), null, isComplete: false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
