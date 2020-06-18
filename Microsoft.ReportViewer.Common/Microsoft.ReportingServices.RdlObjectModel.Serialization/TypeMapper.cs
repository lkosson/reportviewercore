using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal static class TypeMapper
	{
		private static Dictionary<Type, TypeMapping> m_mappings = new Dictionary<Type, TypeMapping>();

		private static ReaderWriterLock m_lock = new ReaderWriterLock();

		public static TypeMapping GetTypeMapping(Type type)
		{
			m_lock.AcquireReaderLock(-1);
			try
			{
				if (m_mappings.ContainsKey(type))
				{
					return m_mappings[type];
				}
			}
			finally
			{
				m_lock.ReleaseReaderLock();
			}
			TypeMapping typeMapping = typeof(IXmlSerializable).IsAssignableFrom(type) ? ImportSpecialMapping(type) : (IsPrimitiveType(type) ? ImportPrimitiveMapping(type) : ((!type.IsArray && !typeof(IEnumerable).IsAssignableFrom(type)) ? ((TypeMapping)ImportStructMapping(type)) : ((TypeMapping)ImportArrayMapping(type))));
			m_lock.AcquireWriterLock(-1);
			try
			{
				m_mappings[type] = typeMapping;
				return typeMapping;
			}
			finally
			{
				m_lock.ReleaseWriterLock();
			}
		}

		public static bool IsPrimitiveType(Type type)
		{
			if (!type.IsEnum && !type.IsPrimitive && !(type == typeof(string)) && !(type == typeof(Guid)))
			{
				return type == typeof(DateTime);
			}
			return true;
		}

		private static SpecialMapping ImportSpecialMapping(Type type)
		{
			SpecialMapping specialMapping = new SpecialMapping(type);
			foreach (XmlElementAttribute item in (IEnumerable)type.GetCustomAttributes(typeof(XmlElementClassAttribute), inherit: true))
			{
				if (item.Type == null || type == item.Type)
				{
					if (item.Namespace != null)
					{
						specialMapping.Namespace = item.Namespace;
					}
					if (item.ElementName != null)
					{
						specialMapping.Name = item.ElementName;
						return specialMapping;
					}
					return specialMapping;
				}
			}
			return specialMapping;
		}

		private static PrimitiveMapping ImportPrimitiveMapping(Type type)
		{
			return new PrimitiveMapping(type);
		}

		private static ArrayMapping ImportArrayMapping(Type type)
		{
			ArrayMapping arrayMapping = new ArrayMapping(type);
			arrayMapping.ElementTypes = new Dictionary<string, Type>();
			if (type.IsArray)
			{
				Type type2 = arrayMapping.ItemType = type.GetElementType();
				arrayMapping.ElementTypes.Add(type2.Name, type2);
			}
			else
			{
				GetCollectionElementTypes(type, arrayMapping);
			}
			return arrayMapping;
		}

		private static void GetCollectionElementTypes(Type type, ArrayMapping mapping)
		{
			PropertyInfo propertyInfo = null;
			Type type2 = type;
			while (type2 != null)
			{
				MemberInfo[] defaultMembers = type.GetDefaultMembers();
				if (defaultMembers != null)
				{
					for (int i = 0; i < defaultMembers.Length; i++)
					{
						if (defaultMembers[i] is PropertyInfo)
						{
							PropertyInfo propertyInfo2 = (PropertyInfo)defaultMembers[i];
							if (propertyInfo2.CanRead && propertyInfo2.GetGetMethod().GetParameters().Length == 1 && (propertyInfo == null || propertyInfo.PropertyType == typeof(object)))
							{
								propertyInfo = propertyInfo2;
							}
						}
					}
				}
				if (propertyInfo != null)
				{
					break;
				}
				type2 = type2.BaseType;
			}
			if (propertyInfo == null)
			{
				throw new Exception("NoDefaultAccessors");
			}
			if (type.GetMethod("Add", new Type[1]
			{
				propertyInfo.PropertyType
			}) == null)
			{
				throw new Exception("NoAddMethod");
			}
			mapping.ItemType = propertyInfo.PropertyType;
			IList customAttributes = propertyInfo.PropertyType.GetCustomAttributes(typeof(XmlElementClassAttribute), inherit: true);
			if (customAttributes != null && customAttributes.Count > 0)
			{
				foreach (XmlElementClassAttribute item in customAttributes)
				{
					mapping.ElementTypes.Add((item.ElementName != string.Empty) ? item.ElementName : item.Type.Name, item.Type);
				}
			}
			else
			{
				string name = propertyInfo.PropertyType.Name;
				mapping.ElementTypes.Add(name, propertyInfo.PropertyType);
			}
		}

		private static void GetMemberName(XmlAttributes attributes, ref string tagName, ref string ns)
		{
			if (attributes.XmlElements.Count > 0)
			{
				if (attributes.XmlElements[0].ElementName != null && attributes.XmlElements[0].ElementName != string.Empty)
				{
					tagName = attributes.XmlElements[0].ElementName;
				}
				if (attributes.XmlElements[0].Namespace != null && attributes.XmlElements[0].Namespace != string.Empty)
				{
					ns = attributes.XmlElements[0].Namespace;
				}
			}
		}

		private static void ImportTypeMembers(StructMapping mapping, Type type)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (PropertyInfo prop in properties)
			{
				ImportPropertyInfo(mapping, prop);
			}
		}

		private static void ImportPropertyInfo(StructMapping mapping, PropertyInfo prop)
		{
			Type type = prop.PropertyType;
			bool flag = false;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				flag = true;
				type = type.GetGenericArguments()[0];
			}
			bool flag2 = false;
			XmlAttributes xmlAttributes = new XmlAttributes();
			object[] array = type.GetCustomAttributes(inherit: true);
			object[] customAttributes = prop.GetCustomAttributes(inherit: true);
			bool flag3 = false;
			int num = array.Length;
			Array.Resize(ref array, num + customAttributes.Length);
			customAttributes.CopyTo(array, num);
			object[] array2 = array;
			foreach (object obj in array2)
			{
				Type type2 = obj.GetType();
				if (type2 == typeof(XmlIgnoreAttribute))
				{
					return;
				}
				if (typeof(DefaultValueAttribute).IsAssignableFrom(type2))
				{
					xmlAttributes.XmlDefaultValue = ((DefaultValueAttribute)obj).Value;
				}
				else if (typeof(XmlElementAttribute).IsAssignableFrom(type2))
				{
					XmlElementAttribute xmlElementAttribute = (XmlElementAttribute)obj;
					xmlAttributes.XmlElements.Add(xmlElementAttribute);
					if (xmlElementAttribute.Type != null)
					{
						if (string.IsNullOrEmpty(xmlElementAttribute.ElementName))
						{
							type = xmlElementAttribute.Type;
						}
						else
						{
							flag2 = true;
						}
					}
				}
				else if (type2 == typeof(XmlArrayItemAttribute))
				{
					XmlArrayItemAttribute xmlArrayItemAttribute = (XmlArrayItemAttribute)obj;
					int j;
					for (j = 0; j < xmlAttributes.XmlArrayItems.Count && xmlAttributes.XmlArrayItems[j].NestingLevel <= xmlArrayItemAttribute.NestingLevel; j++)
					{
					}
					xmlAttributes.XmlArrayItems.Insert(j, xmlArrayItemAttribute);
				}
				else if (typeof(XmlAttributeAttribute).IsAssignableFrom(type2))
				{
					xmlAttributes.XmlAttribute = (XmlAttributeAttribute)obj;
				}
				else if (type2 == typeof(ValidValuesAttribute) || type2 == typeof(ValidEnumValuesAttribute))
				{
					flag3 = true;
				}
			}
			string tagName = prop.Name;
			string ns = string.Empty;
			if (!flag2)
			{
				GetMemberName(xmlAttributes, ref tagName, ref ns);
			}
			if (mapping.GetElement(tagName, ns) != null || mapping.GetAttribute(tagName, ns) != null)
			{
				return;
			}
			PropertyMapping propertyMapping = new PropertyMapping(type, tagName, ns, prop);
			propertyMapping.XmlAttributes = xmlAttributes;
			mapping.Members.Add(propertyMapping);
			if (xmlAttributes.XmlAttribute != null)
			{
				if (xmlAttributes.XmlAttribute is XmlChildAttributeAttribute)
				{
					mapping.AddChildAttribute(propertyMapping);
				}
				else
				{
					mapping.Attributes[tagName, ns] = propertyMapping;
				}
			}
			else
			{
				mapping.Elements[tagName, ns] = propertyMapping;
				if (flag2)
				{
					mapping.AddUseTypeInfo(tagName, ns);
				}
			}
			Type declaringType = prop.DeclaringType;
			if (!declaringType.IsSubclassOf(typeof(ReportObject)))
			{
				return;
			}
			Type type3 = declaringType.Assembly.GetType(declaringType.FullName + "+Definition+Properties", throwOnError: false);
			FieldInfo field;
			if (type3 != null && type3.IsEnum && (field = type3.GetField(prop.Name)) != null)
			{
				propertyMapping.Index = (int)field.GetRawConstantValue();
				propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.Object;
				if (flag)
				{
					propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.Object;
				}
				else if (type.IsSubclassOf(typeof(IContainedObject)))
				{
					propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.ContainedObject;
				}
				else if (type == typeof(bool))
				{
					propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.Boolean;
				}
				else if (type == typeof(int))
				{
					propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.Integer;
				}
				else if (type == typeof(ReportSize))
				{
					propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.Size;
				}
				else if (type.IsEnum)
				{
					propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.Enum;
				}
				else if (type.IsValueType)
				{
					propertyMapping.TypeCode = PropertyMapping.PropertyTypeCode.ValueType;
				}
				if (flag3)
				{
					type3 = declaringType.Assembly.GetType(declaringType.FullName + "+Definition", throwOnError: false);
					propertyMapping.Definition = (IPropertyDefinition)type3.InvokeMember("GetProperty", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod, null, null, new object[1]
					{
						propertyMapping.Index
					}, CultureInfo.InvariantCulture);
				}
			}
		}

		private static StructMapping ImportStructMapping(Type type)
		{
			StructMapping structMapping = new StructMapping(type);
			foreach (XmlElementAttribute item in (IEnumerable)type.GetCustomAttributes(typeof(XmlElementClassAttribute), inherit: true))
			{
				if (item.Type == null || type == item.Type)
				{
					if (item.Namespace != null)
					{
						structMapping.Namespace = item.Namespace;
					}
					if (item.ElementName != null)
					{
						structMapping.Name = item.ElementName;
					}
					break;
				}
			}
			ImportTypeMembers(structMapping, type);
			structMapping.ResolveChildAttributes();
			return structMapping;
		}
	}
}
