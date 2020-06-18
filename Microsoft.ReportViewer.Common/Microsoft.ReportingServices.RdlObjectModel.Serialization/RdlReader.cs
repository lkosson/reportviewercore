using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal class RdlReader : RdlReaderWriterBase
	{
		private XmlReader m_reader;

		private RdlValidator m_validator;

		private XmlSchema m_schema;

		private const string m_xsdResourceId = "Microsoft.ReportingServices.RdlObjectModel.Serialization.ReportDefinition.xsd";

		private readonly HashSet<string> m_validNamespaces = new HashSet<string>
		{
			"http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition",
			"http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily"
		};

		public RdlReader(RdlSerializerSettings settings)
			: base(settings)
		{
		}

		public object Deserialize(Stream stream, Type rootType)
		{
			m_reader = XmlReader.Create(stream, GetXmlReaderSettings());
			return Deserialize(rootType);
		}

		public object Deserialize(TextReader textReader, Type rootType)
		{
			m_reader = XmlReader.Create(textReader, GetXmlReaderSettings());
			return Deserialize(rootType);
		}

		public object Deserialize(XmlReader xmlReader, Type rootType)
		{
			m_reader = XmlReader.Create(xmlReader, GetXmlReaderSettings());
			return Deserialize(rootType);
		}

		private object Deserialize(Type rootType)
		{
			List<string> list = new List<string>(m_validNamespaces);
			if (m_schema != null)
			{
				list.Add(m_schema.TargetNamespace);
			}
			if (base.Settings.ValidateXml)
			{
				m_validator = new RdlValidator(m_reader, list);
			}
			object result = ReadRoot(rootType);
			m_reader.Close();
			return result;
		}

		private XmlReaderSettings GetXmlReaderSettings()
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.CheckCharacters = false;
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreProcessingInstructions = true;
			xmlReaderSettings.IgnoreWhitespace = base.Settings.IgnoreWhitespace;
			if (base.Settings.ValidateXml)
			{
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				XmlSchema schema = XmlSchema.Read(Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.ReportingServices.RdlObjectModel.Serialization.ReportDefinition.xsd"), null);
				xmlReaderSettings.Schemas.Add(schema);
				if (base.Settings.XmlSchema != null)
				{
					if (base.Settings.XmlSchema.TargetNamespace.EndsWith("/reportdefinition", StringComparison.Ordinal))
					{
						m_schema = base.Settings.XmlSchema;
					}
					xmlReaderSettings.Schemas.Add(base.Settings.XmlSchema);
				}
				if (m_schema == null)
				{
					m_schema = schema;
				}
				if (base.Settings.XmlValidationEventHandler != null)
				{
					xmlReaderSettings.ValidationEventHandler += base.Settings.XmlValidationEventHandler;
				}
			}
			return xmlReaderSettings;
		}

		private object ReadRoot(Type type)
		{
			try
			{
				m_reader.MoveToContent();
				TypeMapping typeMapping = TypeMapper.GetTypeMapping(type);
				if (m_reader.NamespaceURI != typeMapping.Namespace)
				{
					throw new XmlException(SRErrors.NoRootElement);
				}
				return ReadObject(type, null, 0);
			}
			catch (XmlException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				if (innerException is TargetInvocationException && innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
				}
				string message;
				if (innerException is TargetInvocationException)
				{
					MethodBase targetSite = ((TargetInvocationException)innerException).TargetSite;
					message = SRErrors.DeserializationFailedMethod((targetSite != null) ? (targetSite.DeclaringType.Name + "." + targetSite.Name) : null);
				}
				else
				{
					message = SRErrors.DeserializationFailed(innerException.Message);
				}
				IXmlLineInfo xmlLineInfo = m_reader as IXmlLineInfo;
				XmlException ex2 = (xmlLineInfo == null) ? new XmlException(message, innerException) : new XmlException(message, innerException, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
				throw ex2;
			}
		}

		private object ReadObject(Type type, MemberMapping member, int nestingLevel)
		{
			ValidateStartElement();
			object result = (!TypeMapper.IsPrimitiveType(type)) ? ReadClassObject(type, member, nestingLevel) : ReadPrimitive(type);
			ValidateEndElement();
			return result;
		}

		private object ReadObjectContent(object value, MemberMapping member, int nestingLevel)
		{
			TypeMapping typeMapping = TypeMapper.GetTypeMapping(value.GetType());
			if (typeMapping is ArrayMapping)
			{
				ReadArrayContent(value, (ArrayMapping)typeMapping, member, nestingLevel);
			}
			else if (typeMapping is StructMapping)
			{
				ReadStructContent(value, (StructMapping)typeMapping);
			}
			else if (typeMapping is SpecialMapping)
			{
				ReadSpecialContent(value);
			}
			else
			{
				m_reader.Skip();
			}
			if (base.Host != null)
			{
				base.Host.OnDeserialization(value);
			}
			return value;
		}

		private object ReadPrimitive(Type type)
		{
			object result = null;
			string text = m_reader.ReadString();
			if (type.IsPrimitive)
			{
				switch (Type.GetTypeCode(type))
				{
				case TypeCode.Boolean:
					result = XmlConvert.ToBoolean(text);
					break;
				case TypeCode.Int16:
					result = XmlConvert.ToInt16(text);
					break;
				case TypeCode.Int32:
					result = XmlConvert.ToInt32(text);
					break;
				case TypeCode.Int64:
					result = XmlConvert.ToInt64(text);
					break;
				case TypeCode.Double:
					result = XmlConvert.ToDouble(text);
					break;
				case TypeCode.Single:
					result = XmlConvert.ToSingle(text);
					break;
				}
			}
			else if (type == typeof(string))
			{
				result = text;
				if (base.Settings.Normalize)
				{
					result = Regex.Replace(text, "(?<!\r)\n", "\r\n");
				}
			}
			else if (type.IsEnum)
			{
				result = Enum.Parse(type, text, ignoreCase: true);
			}
			else if (type == typeof(Guid))
			{
				result = new Guid(text);
			}
			else if (type == typeof(DateTime))
			{
				result = XmlCustomFormatter.ToDateTime(text);
			}
			m_reader.Skip();
			return result;
		}

		private object ReadClassObject(Type type, MemberMapping member, int nestingLevel)
		{
			type = GetSerializationType(type);
			object obj = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
			ReadObjectContent(obj, member, nestingLevel);
			return obj;
		}

		private object ReadSpecialContent(object obj)
		{
			((IXmlSerializable)obj)?.ReadXml(m_reader);
			return obj;
		}

		private object ReadArrayContent(object array, ArrayMapping mapping, MemberMapping member, int nestingLevel)
		{
			IList list = (IList)array;
			if (m_reader.IsEmptyElement)
			{
				m_reader.Skip();
			}
			else
			{
				m_reader.ReadStartElement();
				m_reader.MoveToContent();
				while (m_reader.NodeType != XmlNodeType.EndElement && m_reader.NodeType != 0)
				{
					if (m_reader.NodeType == XmlNodeType.Element)
					{
						string localName = m_reader.LocalName;
						_ = m_reader.NamespaceURI;
						Type value = null;
						bool flag = false;
						if (member != null && member.XmlAttributes.XmlArrayItems.Count > nestingLevel)
						{
							if (localName == member.XmlAttributes.XmlArrayItems[nestingLevel].ElementName)
							{
								XmlArrayItemAttribute xmlArrayItemAttribute = member.XmlAttributes.XmlArrayItems[nestingLevel];
								value = xmlArrayItemAttribute.Type;
								flag = xmlArrayItemAttribute.IsNullable;
							}
						}
						else
						{
							XmlElementAttributes xmlElementAttributes = null;
							if (base.XmlOverrides != null)
							{
								XmlAttributes xmlAttributes = base.XmlOverrides[mapping.ItemType];
								if (xmlAttributes != null && xmlAttributes.XmlElements != null)
								{
									xmlElementAttributes = xmlAttributes.XmlElements;
								}
							}
							if (xmlElementAttributes == null)
							{
								mapping.ElementTypes.TryGetValue(localName, out value);
							}
							else
							{
								foreach (XmlElementAttribute item in xmlElementAttributes)
								{
									if (localName == item.ElementName)
									{
										value = item.Type;
										break;
									}
								}
							}
						}
						if (value != null)
						{
							object value2;
							if (flag && m_reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true")
							{
								m_reader.Skip();
								value2 = null;
							}
							else
							{
								value2 = ReadObject(value, member, nestingLevel + 1);
							}
							list.Add(value2);
						}
						else
						{
							m_reader.Skip();
						}
					}
					else
					{
						m_reader.Skip();
					}
					m_reader.MoveToContent();
				}
				m_reader.ReadEndElement();
			}
			return array;
		}

		private void ReadStructContent(object obj, StructMapping mapping)
		{
			m_reader.MoveToContent();
			string name = m_reader.Name;
			string namespaceURI = m_reader.NamespaceURI;
			ReadStructAttributes(obj, mapping);
			if (m_reader.IsEmptyElement)
			{
				m_reader.Skip();
				return;
			}
			m_reader.ReadStartElement();
			m_reader.MoveToContent();
			while (m_reader.NodeType != XmlNodeType.EndElement && m_reader.NodeType != 0)
			{
				string localName = m_reader.LocalName;
				string namespaceURI2 = m_reader.NamespaceURI;
				namespaceURI2 = ((namespaceURI == namespaceURI2) ? string.Empty : namespaceURI2);
				MemberMapping memberMapping = mapping.GetElement(localName, namespaceURI2);
				Type type = null;
				if (memberMapping != null)
				{
					type = memberMapping.Type;
				}
				else
				{
					List<MemberMapping> typeNameElements = mapping.GetTypeNameElements();
					if (typeNameElements != null)
					{
						bool flag = false;
						for (int i = 0; i < typeNameElements.Count; i++)
						{
							memberMapping = typeNameElements[i];
							XmlElementAttributes xmlElements = memberMapping.XmlAttributes.XmlElements;
							if (base.XmlOverrides != null)
							{
								XmlAttributes xmlAttributes = base.XmlOverrides[obj.GetType()];
								if (xmlAttributes == null)
								{
									xmlAttributes = base.XmlOverrides[memberMapping.Type];
								}
								if (xmlAttributes != null && xmlAttributes.XmlElements != null)
								{
									xmlElements = xmlAttributes.XmlElements;
								}
							}
							foreach (XmlElementAttribute item in xmlElements)
							{
								if (item.ElementName == localName && item.Type != null)
								{
									type = item.Type;
									flag = true;
									break;
								}
							}
							if (flag)
							{
								break;
							}
						}
					}
				}
				if (type != null)
				{
					if (memberMapping.ChildAttributes != null)
					{
						foreach (MemberMapping childAttribute in memberMapping.ChildAttributes)
						{
							ReadChildAttribute(obj, mapping, childAttribute);
						}
					}
					if (memberMapping.IsReadOnly)
					{
						if (!TypeMapper.IsPrimitiveType(type))
						{
							object value = memberMapping.GetValue(obj);
							if (value != null)
							{
								ReadObjectContent(value, memberMapping, 0);
							}
							else
							{
								m_reader.Skip();
							}
						}
						else
						{
							m_reader.Skip();
						}
					}
					else
					{
						object obj2 = ReadObject(type, memberMapping, 0);
						if (obj2 != null)
						{
							memberMapping.SetValue(obj, obj2);
						}
					}
				}
				else
				{
					if (namespaceURI2 != string.Empty && m_validNamespaces.Contains(namespaceURI2))
					{
						IXmlLineInfo xmlLineInfo = (IXmlLineInfo)m_reader;
						throw new XmlException(RDLValidatingReaderStrings.rdlValidationInvalidMicroVersionedElement(m_reader.Name, name, xmlLineInfo.LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), xmlLineInfo.LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat)));
					}
					m_reader.Skip();
				}
				m_reader.MoveToContent();
			}
			m_reader.ReadEndElement();
		}

		private void ReadStructAttributes(object obj, StructMapping mapping)
		{
			if (!m_reader.HasAttributes)
			{
				return;
			}
			string text = null;
			foreach (MemberMapping value in mapping.Attributes.Values)
			{
				if (value.Type == typeof(string))
				{
					text = m_reader.GetAttribute(value.Name, value.Namespace);
					if (text != null)
					{
						value.SetValue(obj, text);
					}
				}
			}
		}

		private void ReadChildAttribute(object obj, StructMapping mapping, MemberMapping childMapping)
		{
			XmlAttributeAttribute xmlAttribute = childMapping.XmlAttributes.XmlAttribute;
			string attribute = m_reader.GetAttribute(xmlAttribute.AttributeName, xmlAttribute.Namespace);
			if (attribute != null)
			{
				childMapping.SetValue(obj, attribute);
			}
		}

		private void ValidateStartElement()
		{
			if (base.Settings.ValidateXml && !m_validator.ValidateStartElement(out string message))
			{
				throw new XmlSchemaException(message + "\r\n");
			}
		}

		private void ValidateEndElement()
		{
			if (base.Settings.ValidateXml && !m_validator.ValidateEndElement(out string message))
			{
				throw new XmlSchemaException(message + "\r\n");
			}
		}
	}
}
