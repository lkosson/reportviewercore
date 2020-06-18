using System;
using System.IO;
using System.Xml;

namespace Microsoft.ReportingServices.RdlObjectModel.Serialization
{
	internal class RdlSerializer
	{
		private RdlSerializerSettings m_settings;

		public RdlSerializerSettings Settings => m_settings;

		public RdlSerializer()
		{
			m_settings = new RdlSerializerSettings();
		}

		public RdlSerializer(RdlSerializerSettings settings)
		{
			m_settings = settings;
		}

		public Report Deserialize(Stream stream)
		{
			return (Report)Deserialize(stream, typeof(Report));
		}

		public Report Deserialize(TextReader textReader)
		{
			return (Report)Deserialize(textReader, typeof(Report));
		}

		public Report Deserialize(XmlReader xmlReader)
		{
			return (Report)Deserialize(xmlReader, typeof(Report));
		}

		public object Deserialize(Stream stream, Type objectType)
		{
			return new RdlReader(m_settings).Deserialize(stream, objectType);
		}

		public object Deserialize(TextReader textReader, Type objectType)
		{
			return new RdlReader(m_settings).Deserialize(textReader, objectType);
		}

		public object Deserialize(XmlReader xmlReader, Type objectType)
		{
			return new RdlReader(m_settings).Deserialize(xmlReader, objectType);
		}

		public void Serialize(Stream stream, object o)
		{
			XmlWriter xmlWriter = XmlWriter.Create(stream, GetXmlWriterSettings());
			Serialize(xmlWriter, o);
		}

		public void Serialize(TextWriter textWriter, object o)
		{
			XmlWriter xmlWriter = XmlWriter.Create(textWriter, GetXmlWriterSettings());
			Serialize(xmlWriter, o);
		}

		public void Serialize(XmlWriter xmlWriter, object o)
		{
			new RdlWriter(m_settings).Serialize(xmlWriter, o);
		}

		private XmlWriterSettings GetXmlWriterSettings()
		{
			return new XmlWriterSettings
			{
				Indent = true
			};
		}
	}
}
