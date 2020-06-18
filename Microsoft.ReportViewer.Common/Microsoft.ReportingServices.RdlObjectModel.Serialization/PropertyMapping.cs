using System;
using System.Reflection;

namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal class PropertyMapping : MemberMapping
	{
		internal enum PropertyTypeCode
		{
			None,
			Object,
			ContainedObject,
			Boolean,
			Integer,
			Size,
			Enum,
			ValueType
		}

		private PropertyInfo m_property;

		private int m_index;

		private PropertyTypeCode m_typeCode;

		private IPropertyDefinition m_definition;

		public PropertyInfo Property => m_property;

		public int Index
		{
			get
			{
				return m_index;
			}
			set
			{
				m_index = value;
			}
		}

		public PropertyTypeCode TypeCode
		{
			get
			{
				return m_typeCode;
			}
			set
			{
				m_typeCode = value;
			}
		}

		public IPropertyDefinition Definition
		{
			get
			{
				return m_definition;
			}
			set
			{
				m_definition = value;
			}
		}

		public PropertyMapping(Type propertyType, string name, string ns, PropertyInfo property)
			: base(propertyType, name, ns, !property.CanWrite)
		{
			m_property = property;
		}

		public override void SetValue(object obj, object value)
		{
			if (m_typeCode != 0)
			{
				IPropertyStore propertyStore = ((ReportObject)obj).PropertyStore;
				if (m_definition != null)
				{
					m_definition.Validate(obj, value);
				}
				switch (m_typeCode)
				{
				default:
					propertyStore.SetObject(m_index, value);
					break;
				case PropertyTypeCode.ContainedObject:
					propertyStore.SetObject(m_index, (IContainedObject)value);
					break;
				case PropertyTypeCode.Boolean:
					propertyStore.SetBoolean(m_index, (bool)value);
					break;
				case PropertyTypeCode.Integer:
				case PropertyTypeCode.Enum:
					propertyStore.SetInteger(m_index, (int)value);
					break;
				case PropertyTypeCode.Size:
					propertyStore.SetSize(m_index, (ReportSize)value);
					break;
				}
			}
			else
			{
				m_property.SetValue(obj, value, null);
			}
		}

		public override object GetValue(object obj)
		{
			if (m_typeCode != 0)
			{
				IPropertyStore propertyStore = ((ReportObject)obj).PropertyStore;
				switch (m_typeCode)
				{
				default:
					return propertyStore.GetObject(m_index);
				case PropertyTypeCode.Boolean:
					return propertyStore.GetBoolean(m_index);
				case PropertyTypeCode.Integer:
					return propertyStore.GetInteger(m_index);
				case PropertyTypeCode.Size:
					return propertyStore.GetSize(m_index);
				case PropertyTypeCode.Enum:
				{
					int integer = propertyStore.GetInteger(m_index);
					return Enum.ToObject(Type, integer);
				}
				case PropertyTypeCode.ValueType:
				{
					object obj2 = propertyStore.GetObject(m_index);
					if (obj2 == null)
					{
						obj2 = Activator.CreateInstance(Type);
					}
					return obj2;
				}
				}
			}
			return m_property.GetValue(obj, null);
		}

		public override bool HasValue(object obj)
		{
			if (m_typeCode != 0)
			{
				IPropertyStore propertyStore = ((ReportObject)obj).PropertyStore;
				switch (m_typeCode)
				{
				default:
					return propertyStore.ContainsObject(m_index);
				case PropertyTypeCode.Boolean:
					return propertyStore.ContainsBoolean(m_index);
				case PropertyTypeCode.Integer:
				case PropertyTypeCode.Enum:
					return propertyStore.ContainsInteger(m_index);
				case PropertyTypeCode.Size:
					return propertyStore.ContainsSize(m_index);
				}
			}
			return m_property.GetValue(obj, null) != null;
		}
	}
}
