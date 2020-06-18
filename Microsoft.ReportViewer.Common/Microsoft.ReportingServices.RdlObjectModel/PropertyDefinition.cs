using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class PropertyDefinition
	{
		private string m_name;

		public string Name => m_name;

		protected PropertyDefinition(string name)
		{
			m_name = name;
		}

		public static IPropertyDefinition Create(Type componentType, string propertyName)
		{
			PropertyInfo property = componentType.GetProperty(propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			if (property == null)
			{
				property = componentType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
			}
			Type type = property.PropertyType;
			object obj = null;
			object obj2 = null;
			object obj3 = null;
			IList<int> validValues = null;
			object[] customAttributes = property.GetCustomAttributes(inherit: true);
			foreach (object obj4 in customAttributes)
			{
				if (obj4 is DefaultValueAttribute)
				{
					obj = ((DefaultValueAttribute)obj4).Value;
					if (obj is IExpression)
					{
						obj = ((IExpression)obj).Value;
					}
				}
				else if (obj4 is ValidValuesAttribute)
				{
					obj2 = ((ValidValuesAttribute)obj4).Minimum;
					obj3 = ((ValidValuesAttribute)obj4).Maximum;
				}
				else if (obj4 is ValidEnumValuesAttribute)
				{
					validValues = ((ValidEnumValuesAttribute)obj4).ValidValues;
				}
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ReportExpression<>))
			{
				type = type.GetGenericArguments()[0];
			}
			if (type == typeof(int))
			{
				return new IntProperty(propertyName, (int?)obj, (int?)obj2, (int?)obj3);
			}
			if (type == typeof(double))
			{
				return new DoubleProperty(propertyName, (double?)obj, (double?)obj2, (double?)obj3);
			}
			if (type == typeof(string))
			{
				return new StringProperty(propertyName, (string)obj);
			}
			if (type == typeof(ReportSize))
			{
				return new SizeProperty(propertyName, (ReportSize?)obj, (ReportSize?)obj2, (ReportSize?)obj3);
			}
			if (type == typeof(ReportColor))
			{
				return new ColorProperty(propertyName, (ReportColor?)obj);
			}
			if (type.IsSubclassOf(typeof(Enum)))
			{
				return new EnumProperty(propertyName, type, obj, validValues);
			}
			return null;
		}
	}
	internal abstract class PropertyDefinition<T> : PropertyDefinition, IPropertyDefinition where T : struct
	{
		private T? m_default;

		public T? Default => m_default;

		object IPropertyDefinition.Default => m_default;

		object IPropertyDefinition.Minimum => null;

		object IPropertyDefinition.Maximum => null;

		void IPropertyDefinition.Validate(object component, object value)
		{
		}

		protected PropertyDefinition(string name, T? defaultValue)
			: base(name)
		{
			m_default = defaultValue;
		}
	}
}
