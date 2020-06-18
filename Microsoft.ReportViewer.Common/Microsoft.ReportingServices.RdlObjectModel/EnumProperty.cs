using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class EnumProperty : PropertyDefinition, IPropertyDefinition
	{
		private object m_default;

		private IList<int> m_validIntValues;

		private IList<object> m_validValues;

		private Type m_type;

		public object Default => m_default;

		public IList<object> ValidValues
		{
			get
			{
				if (m_validValues == null)
				{
					object[] array;
					if (m_validIntValues != null)
					{
						array = new object[m_validValues.Count];
						for (int i = 0; i < m_validIntValues.Count; i++)
						{
							array[i] = Enum.ToObject(m_type, m_validValues[i]);
						}
					}
					else
					{
						Array values = Enum.GetValues(m_type);
						array = new object[values.Length];
						values.CopyTo(array, 0);
					}
					m_validValues = new ReadOnlyCollection<object>(array);
				}
				return m_validValues;
			}
		}

		object IPropertyDefinition.Minimum => null;

		object IPropertyDefinition.Maximum => null;

		void IPropertyDefinition.Validate(object component, object value)
		{
			if (value is IExpression)
			{
				if (((IExpression)value).IsExpression)
				{
					return;
				}
				value = ((IExpression)value).Value;
			}
			if (value.GetType() == m_type)
			{
				Validate(component, (int)value);
				return;
			}
			throw new ArgumentException("Invalid type.");
		}

		public void Validate(object component, int value)
		{
			if (m_validIntValues != null && !m_validIntValues.Contains(value))
			{
				object value2 = Enum.ToObject(m_type, value);
				throw new ArgumentConstraintException(component, base.Name, value2, null, SRErrors.InvalidParam(base.Name, value2));
			}
		}

		public EnumProperty(string name, Type enumType, object defaultValue, IList<int> validValues)
			: base(name)
		{
			m_type = enumType;
			m_default = defaultValue;
			m_validIntValues = validValues;
		}
	}
}
