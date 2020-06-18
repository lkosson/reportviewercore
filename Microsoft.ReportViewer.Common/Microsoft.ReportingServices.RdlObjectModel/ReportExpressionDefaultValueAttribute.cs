using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class ReportExpressionDefaultValueAttribute : DefaultValueAttribute
	{
		public ReportExpressionDefaultValueAttribute()
			: base(default(ReportExpression))
		{
		}

		public ReportExpressionDefaultValueAttribute(string value)
			: base(new ReportExpression(value))
		{
		}

		public ReportExpressionDefaultValueAttribute(Type type)
			: base(Activator.CreateInstance(ConstructGenericType(type)))
		{
		}

		public ReportExpressionDefaultValueAttribute(Type type, object value)
			: base(CreateInstance(type, value))
		{
		}

		internal static Type ConstructGenericType(Type type)
		{
			return typeof(ReportExpression<>).MakeGenericType(type);
		}

		internal static object CreateInstance(Type type, object value)
		{
			type = ConstructGenericType(type);
			if (value is string)
			{
				return type.GetConstructor(new Type[2]
				{
					typeof(string),
					typeof(IFormatProvider)
				}).Invoke(new object[2]
				{
					value,
					CultureInfo.InvariantCulture
				});
			}
			return Activator.CreateInstance(type, value);
		}
	}
}
