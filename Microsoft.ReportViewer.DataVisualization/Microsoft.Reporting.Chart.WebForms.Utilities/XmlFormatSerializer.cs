using Microsoft.Reporting.Chart.WebForms.Design;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	internal class XmlFormatSerializer : SerializerBase
	{
		public void Serialize(object objectToSerialize, Stream stream)
		{
			Serialize(objectToSerialize, (object)stream);
		}

		public void Serialize(object objectToSerialize, XmlWriter xmlWriter)
		{
			Serialize(objectToSerialize, (object)xmlWriter);
		}

		public void Serialize(object objectToSerialize, TextWriter textWriter)
		{
			Serialize(objectToSerialize, (object)textWriter);
		}

		public void Serialize(object objectToSerialize, string fileName)
		{
			Serialize(objectToSerialize, (object)fileName);
		}

		public override void Serialize(object objectToSerialize, object writer)
		{
			if (objectToSerialize == null)
			{
				throw new ArgumentNullException("objectToSerialize");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (!(writer is Stream) && !(writer is TextWriter) && !(writer is XmlWriter) && !(writer is string))
			{
				throw new ArgumentException(SR.ExceptionChartSerializerWriterObjectInvalid, "writer");
			}
			XmlDocument xmlDocument = new XmlDocument();
			binaryFormatVersion = 300;
			XmlDocumentFragment xmlDocumentFragment = xmlDocument.CreateDocumentFragment();
			SerializeObject(objectToSerialize, null, GetObjectName(objectToSerialize), xmlDocumentFragment, xmlDocument);
			xmlDocument.AppendChild(xmlDocumentFragment);
			RemoveEmptyChildNodes(xmlDocument);
			if (writer is Stream)
			{
				xmlDocument.Save((Stream)writer);
				((Stream)writer).Flush();
				((Stream)writer).Seek(0L, SeekOrigin.Begin);
			}
			if (writer is string)
			{
				xmlDocument.Save((string)writer);
			}
			if (writer is XmlWriter)
			{
				xmlDocument.Save((XmlWriter)writer);
			}
			if (writer is TextWriter)
			{
				xmlDocument.Save((TextWriter)writer);
			}
		}

		protected virtual void SerializeObject(object objectToSerialize, object parent, string elementName, XmlNode xmlParentNode, XmlDocument xmlDocument)
		{
			if (objectToSerialize == null)
			{
				return;
			}
			if (parent != null)
			{
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(parent)[elementName];
				if (propertyDescriptor != null)
				{
					SerializationVisibilityAttribute serializationVisibilityAttribute = (SerializationVisibilityAttribute)propertyDescriptor.Attributes[typeof(SerializationVisibilityAttribute)];
					if (serializationVisibilityAttribute != null && serializationVisibilityAttribute.Visibility == SerializationVisibility.Hidden)
					{
						return;
					}
				}
			}
			if (objectToSerialize is ICollection)
			{
				SerializeCollection(objectToSerialize, elementName, xmlParentNode, xmlDocument);
				return;
			}
			XmlNode xmlNode = xmlDocument.CreateElement(elementName);
			xmlParentNode.AppendChild(xmlNode);
			bool flag = false;
			if (base.TemplateMode && parent is IList)
			{
				XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("_Template_");
				if (((IList)parent).Count == 1)
				{
					xmlAttribute.Value = "All";
				}
				else
				{
					xmlAttribute.Value = ((IList)parent).IndexOf(objectToSerialize).ToString(CultureInfo.InvariantCulture);
				}
				xmlNode.Attributes.Append(xmlAttribute);
				flag = true;
			}
			PropertyInfo[] properties = objectToSerialize.GetType().GetProperties();
			if (properties == null)
			{
				return;
			}
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if ((flag && propertyInfo.Name == "Name") || IsChartBaseProperty(objectToSerialize, parent, propertyInfo))
				{
					continue;
				}
				if (propertyInfo.CanRead && propertyInfo.PropertyType.GetInterface("ICollection", ignoreCase: true) != null && !SerializeICollAsAtribute(propertyInfo, objectToSerialize))
				{
					bool flag2 = true;
					if (objectToSerialize != null)
					{
						PropertyDescriptor propertyDescriptor2 = TypeDescriptor.GetProperties(objectToSerialize)[propertyInfo.Name];
						if (propertyDescriptor2 != null)
						{
							SerializationVisibilityAttribute serializationVisibilityAttribute2 = (SerializationVisibilityAttribute)propertyDescriptor2.Attributes[typeof(SerializationVisibilityAttribute)];
							if (serializationVisibilityAttribute2 != null && serializationVisibilityAttribute2.Visibility == SerializationVisibility.Hidden)
							{
								flag2 = false;
							}
						}
					}
					MethodInfo method = objectToSerialize.GetType().GetMethod("ShouldSerialize" + propertyInfo.Name);
					if (method != null)
					{
						object obj = method.Invoke(objectToSerialize, null);
						if (obj is bool && !(bool)obj)
						{
							flag2 = false;
						}
					}
					if (flag2)
					{
						SerializeCollection(propertyInfo.GetValue(objectToSerialize, null), propertyInfo.Name, xmlNode, xmlDocument);
					}
				}
				else
				{
					if (!propertyInfo.CanRead || !propertyInfo.CanWrite || propertyInfo.Name == "Item")
					{
						continue;
					}
					if (ShouldSerializeAsAttribute(propertyInfo, objectToSerialize))
					{
						if (IsSerializableContent(propertyInfo.Name, objectToSerialize))
						{
							SerializeProperty(propertyInfo.GetValue(objectToSerialize, null), objectToSerialize, propertyInfo.Name, xmlNode, xmlDocument);
						}
					}
					else
					{
						SerializeObject(propertyInfo.GetValue(objectToSerialize, null), objectToSerialize, propertyInfo.Name, xmlNode, xmlDocument);
					}
				}
			}
		}

		protected virtual void SerializeCollection(object objectToSerialize, string elementName, XmlNode xmlParentNode, XmlDocument xmlDocument)
		{
			if (!(objectToSerialize is ICollection))
			{
				return;
			}
			XmlNode xmlNode = xmlDocument.CreateElement(elementName);
			xmlParentNode.AppendChild(xmlNode);
			foreach (object item in (ICollection)objectToSerialize)
			{
				SerializeObject(item, objectToSerialize, GetObjectName(item), xmlNode, xmlDocument);
			}
		}

		protected virtual void SerializeProperty(object objectToSerialize, object parent, string elementName, XmlNode xmlParentNode, XmlDocument xmlDocument)
		{
			if (objectToSerialize == null || parent == null)
			{
				return;
			}
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(parent)[elementName];
			if (propertyDescriptor != null)
			{
				DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)propertyDescriptor.Attributes[typeof(DefaultValueAttribute)];
				if (defaultValueAttribute != null)
				{
					if (objectToSerialize.Equals(defaultValueAttribute.Value))
					{
						return;
					}
				}
				else
				{
					MethodInfo method = parent.GetType().GetMethod("ShouldSerialize" + elementName);
					if (method != null)
					{
						object obj = method.Invoke(parent, null);
						if (obj is bool && !(bool)obj)
						{
							return;
						}
					}
				}
				SerializationVisibilityAttribute serializationVisibilityAttribute = (SerializationVisibilityAttribute)propertyDescriptor.Attributes[typeof(SerializationVisibilityAttribute)];
				if (serializationVisibilityAttribute != null && serializationVisibilityAttribute.Visibility == SerializationVisibility.Hidden)
				{
					return;
				}
			}
			XmlAttribute xmlAttribute = xmlDocument.CreateAttribute(elementName);
			xmlAttribute.Value = GetXmlValue(objectToSerialize, parent, elementName);
			xmlParentNode.Attributes.Append(xmlAttribute);
		}

		protected string GetXmlValue(object obj, object parent, string elementName)
		{
			if (obj is string)
			{
				return (string)obj;
			}
			if (obj is Font)
			{
				return SerializerBase.FontToString((Font)obj);
			}
			if (obj is Color)
			{
				return SerializerBase.colorConverter.ConvertToString(null, CultureInfo.InvariantCulture, obj);
			}
			if (obj is Color[])
			{
				return ColorArrayConverter.ColorArrayToString((Color[])obj);
			}
			if (obj is Image)
			{
				return SerializerBase.ImageToString((Image)obj);
			}
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(parent)[elementName];
			if (propertyDescriptor != null && propertyDescriptor.Converter != null && propertyDescriptor.Converter.CanConvertTo(typeof(string)))
			{
				return propertyDescriptor.Converter.ConvertToString(null, CultureInfo.InvariantCulture, obj);
			}
			return obj.ToString();
		}

		private void RemoveEmptyChildNodes(XmlNode xmlNode)
		{
			for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
			{
				RemoveEmptyChildNodes(xmlNode.ChildNodes[i]);
				XmlNode xmlNode2 = xmlNode.ChildNodes[i];
				if (xmlNode2.ParentNode != null && !(xmlNode2.ParentNode is XmlDocument) && !xmlNode2.HasChildNodes && (xmlNode2.Attributes == null || xmlNode2.Attributes.Count == 0))
				{
					xmlNode.RemoveChild(xmlNode.ChildNodes[i]);
					i--;
				}
				if (!xmlNode2.HasChildNodes && xmlNode2.Attributes.Count == 1 && xmlNode2.Attributes["_Template_"] != null)
				{
					xmlNode.RemoveChild(xmlNode.ChildNodes[i]);
					i--;
				}
			}
		}

		public void Deserialize(object objectToDeserialize, Stream stream)
		{
			Deserialize(objectToDeserialize, (object)stream);
		}

		public void Deserialize(object objectToDeserialize, XmlReader xmlReader)
		{
			Deserialize(objectToDeserialize, (object)xmlReader);
		}

		public void Deserialize(object objectToDeserialize, TextReader textReader)
		{
			Deserialize(objectToDeserialize, (object)textReader);
		}

		public void Deserialize(object objectToDeserialize, string fileName)
		{
			Deserialize(objectToDeserialize, (object)fileName);
		}

		public override void Deserialize(object objectToDeserialize, object reader)
		{
			if (objectToDeserialize == null)
			{
				throw new ArgumentNullException("objectToDeserialize");
			}
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (!(reader is Stream) && !(reader is TextReader) && !(reader is XmlReader) && !(reader is string))
			{
				throw new ArgumentException(SR.ExceptionChartSerializerReaderObjectInvalid, "reader");
			}
			XmlDocument xmlDocument = new XmlDocument();
			binaryFormatVersion = 300;
			if (reader is Stream)
			{
				xmlDocument.Load((Stream)reader);
			}
			if (reader is string)
			{
				xmlDocument.Load((string)reader);
			}
			if (reader is XmlReader)
			{
				xmlDocument.Load((XmlReader)reader);
			}
			if (reader is TextReader)
			{
				xmlDocument.Load((TextReader)reader);
			}
			if (base.ResetWhenLoading)
			{
				ResetObjectProperties(objectToDeserialize);
			}
			DeserializeObject(objectToDeserialize, null, GetObjectName(objectToDeserialize), xmlDocument.DocumentElement, xmlDocument);
		}

		protected virtual int DeserializeObject(object objectToDeserialize, object parent, string elementName, XmlNode xmlParentNode, XmlDocument xmlDocument)
		{
			int num = 0;
			if (objectToDeserialize == null)
			{
				return num;
			}
			foreach (XmlAttribute attribute in xmlParentNode.Attributes)
			{
				if (!(attribute.Name == "_Template_") && IsSerializableContent(attribute.Name, objectToDeserialize))
				{
					SetXmlValue(objectToDeserialize, attribute.Name, attribute.Value);
					num++;
				}
			}
			if (base.TemplateMode && objectToDeserialize is IList && xmlParentNode.FirstChild.Attributes["_Template_"] != null)
			{
				int num2 = 0;
				foreach (object item in (IList)objectToDeserialize)
				{
					XmlNode xmlNode = null;
					foreach (XmlNode childNode in xmlParentNode.ChildNodes)
					{
						string value = childNode.Attributes["_Template_"].Value;
						if (value != null && value.Length > 0)
						{
							if (value == "All")
							{
								xmlNode = childNode;
								break;
							}
							int num3;
							for (num3 = num2; num3 > xmlParentNode.ChildNodes.Count - 1; num3 -= xmlParentNode.ChildNodes.Count)
							{
							}
							if (int.Parse(value, CultureInfo.InvariantCulture) == num3)
							{
								xmlNode = childNode;
								break;
							}
						}
					}
					if (xmlNode != null)
					{
						DeserializeObject(item, objectToDeserialize, "", xmlNode, xmlDocument);
					}
					num2++;
				}
				return 0;
			}
			int num4 = 0;
			foreach (XmlNode childNode2 in xmlParentNode.ChildNodes)
			{
				if (objectToDeserialize is IList)
				{
					string itemName = null;
					if (childNode2.Attributes["Name"] != null)
					{
						itemName = childNode2.Attributes["Name"].Value;
					}
					bool reusedObject = false;
					object listNewItem = GetListNewItem((IList)objectToDeserialize, childNode2.Name, ref itemName, ref reusedObject);
					int num5 = DeserializeObject(listNewItem, objectToDeserialize, "", childNode2, xmlDocument);
					num += num5;
					if (num5 > 0 || reusedObject)
					{
						((IList)objectToDeserialize).Insert(num4++, listNewItem);
					}
				}
				else
				{
					PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(objectToDeserialize)[childNode2.Name];
					if (propertyDescriptor != null)
					{
						object value2 = propertyDescriptor.GetValue(objectToDeserialize);
						num += DeserializeObject(value2, objectToDeserialize, childNode2.Name, childNode2, xmlDocument);
					}
					else if (!base.IgnoreUnknownAttributes)
					{
						throw new InvalidOperationException(SR.ExceptionChartSerializerPropertyNameUnknown(childNode2.Name, objectToDeserialize.GetType().ToString()));
					}
				}
			}
			return num;
		}

		private void SetXmlValue(object obj, string attrName, string attrValue)
		{
			PropertyInfo property = obj.GetType().GetProperty(attrName);
			if (property != null)
			{
				object value = attrValue;
				if (property.PropertyType == typeof(string))
				{
					value = attrValue;
				}
				else if (property.PropertyType == typeof(Font))
				{
					value = SerializerBase.FontFromString(attrValue);
				}
				else if (property.PropertyType == typeof(Color))
				{
					value = (Color)SerializerBase.colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, attrValue);
				}
				else if (property.PropertyType == typeof(Image))
				{
					value = SerializerBase.ImageFromString(attrValue);
				}
				else
				{
					PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(obj)[attrName];
					bool flag = false;
					if (propertyDescriptor != null)
					{
						try
						{
							TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)propertyDescriptor.Attributes[typeof(TypeConverterAttribute)];
							if (typeConverterAttribute != null && typeConverterAttribute.ConverterTypeName.Length > 0)
							{
								Assembly assembly = Assembly.GetAssembly(GetType());
								string[] array = typeConverterAttribute.ConverterTypeName.Split(',');
								TypeConverter typeConverter = (TypeConverter)assembly.CreateInstance(array[0]);
								if (typeConverter != null && typeConverter.CanConvertFrom(typeof(string)))
								{
									value = typeConverter.ConvertFromString(null, CultureInfo.InvariantCulture, attrValue);
									flag = true;
								}
							}
						}
						catch (Exception)
						{
						}
						if (!flag && propertyDescriptor.Converter != null && propertyDescriptor.Converter.CanConvertFrom(typeof(string)))
						{
							value = propertyDescriptor.Converter.ConvertFromString(null, CultureInfo.InvariantCulture, attrValue);
						}
					}
				}
				property.SetValue(obj, value, null);
			}
			else if (!base.IgnoreUnknownAttributes)
			{
				throw new InvalidOperationException(SR.ExceptionChartSerializerPropertyNameUnknown(attrName, obj.GetType().ToString()));
			}
		}
	}
}
