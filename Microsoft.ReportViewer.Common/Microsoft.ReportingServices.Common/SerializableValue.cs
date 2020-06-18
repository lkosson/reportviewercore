using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.Common
{
	internal abstract class SerializableValue : IXmlSerializable
	{
		private object m_value;

		private DataTypeCode m_dataTypeCode = DataTypeCode.Unknown;

		internal const string TYPECODE = "TypeCode";

		internal const string VALUE = "Value";

		internal DataTypeCode TypeCode => m_dataTypeCode;

		internal object Value => m_value;

		protected SerializableValue()
		{
		}

		protected SerializableValue(object value)
		{
			m_value = value;
		}

		protected SerializableValue(object value, DataTypeCode dataTypeCode)
		{
			m_value = value;
			m_dataTypeCode = dataTypeCode;
		}

		public XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		protected abstract void ReadDerivedXml(XmlReader xmlReader);

		public void ReadXml(XmlReader xmlReader)
		{
			while (xmlReader.Read())
			{
				XmlNodeType nodeType = xmlReader.NodeType;
				if (nodeType == XmlNodeType.Element)
				{
					string name = xmlReader.Name;
					if (name == "TypeCode")
					{
						xmlReader.Read();
						m_dataTypeCode = (DataTypeCode)Enum.Parse(typeof(DataTypeCode), xmlReader.Value, ignoreCase: false);
						continue;
					}
					if (name == "Value")
					{
						xmlReader.Read();
						m_value = ObjectSerializer.Read(xmlReader, m_dataTypeCode);
						continue;
					}
				}
				ReadDerivedXml(xmlReader);
			}
		}

		public abstract void WriteXml(XmlWriter writer);

		protected void WriteBaseXml(XmlWriter writer)
		{
			writer.WriteElementString("TypeCode", m_dataTypeCode.ToString());
			if (m_value != null)
			{
				writer.WriteStartElement("Value");
				ObjectSerializer.Write(writer, m_value, m_dataTypeCode);
				writer.WriteEndElement();
			}
		}
	}
}
