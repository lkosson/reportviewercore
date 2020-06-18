using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal static class XmlUtil
	{
		public static void SafeOpenXmlDocumentFile(XmlDocument doc, string pathToXmlFile)
		{
			FileStream fileStream = new FileStream(pathToXmlFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			try
			{
				XmlTextReader reader = new XmlTextReader(fileStream);
				ApplyDtdDosDefense(reader);
				doc.Load(reader);
			}
			finally
			{
				fileStream.Close();
			}
		}

		public static void SafeOpenXmlDocumentString(XmlDocument doc, string xmlContent)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(xmlContent));
			ApplyDtdDosDefense(xmlTextReader);
			xmlTextReader.MoveToContent();
			doc.Load(xmlTextReader);
		}

		public static XmlTextReader SafeCreateXmlTextReader(string xmlContent)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(xmlContent));
			ApplyDtdDosDefense(xmlTextReader);
			return xmlTextReader;
		}

		public static XmlTextReader SafeCreateXmlTextReader(Stream xmlStream)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(xmlStream);
			ApplyDtdDosDefense(xmlTextReader);
			return xmlTextReader;
		}

		public static NameValueCollection ShallowXmlToNameValueCollection(string xml, string topElementTag)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			if (xml == null || xml == string.Empty)
			{
				return nameValueCollection;
			}
			XmlTextReader xmlTextReader = SafeCreateXmlTextReader(xml);
			try
			{
				xmlTextReader.MoveToContent();
				if (xmlTextReader.NodeType != XmlNodeType.Element || string.Compare(xmlTextReader.Name, topElementTag, StringComparison.Ordinal) != 0)
				{
					throw new InvalidXmlException();
				}
				while (xmlTextReader.Read())
				{
					if (xmlTextReader.IsStartElement())
					{
						bool isEmptyElement = xmlTextReader.IsEmptyElement;
						string name = xmlTextReader.Name;
						name = DecodePropertyName(name);
						string value = xmlTextReader.ReadString();
						if (nameValueCollection.GetValues(name) != null)
						{
							throw new InvalidXmlException();
						}
						nameValueCollection[name] = value;
						if (!isEmptyElement && xmlTextReader.IsStartElement())
						{
							throw new InvalidXmlException();
						}
					}
				}
				return nameValueCollection;
			}
			catch (XmlException ex)
			{
				throw new MalformedXmlException(ex);
			}
		}

		public static NameValueCollection DeepXmlToNameValueCollection(string xml, string topElementTag, string eachElementTag, string nameElementTag, string valueElementTag)
		{
			NameValueCollection nameValueCollection = new NameValueCollection(StringComparer.InvariantCulture);
			if (xml == null || xml == string.Empty)
			{
				return nameValueCollection;
			}
			XmlTextReader xmlTextReader = SafeCreateXmlTextReader(xml);
			try
			{
				xmlTextReader.MoveToContent();
				if (xmlTextReader.NodeType != XmlNodeType.Element || string.Compare(xmlTextReader.Name, topElementTag, StringComparison.Ordinal) != 0)
				{
					throw new InvalidXmlException();
				}
				while (xmlTextReader.Read())
				{
					if (!xmlTextReader.IsStartElement())
					{
						continue;
					}
					if (xmlTextReader.IsEmptyElement || string.Compare(xmlTextReader.Name, eachElementTag, StringComparison.Ordinal) != 0)
					{
						throw new InvalidXmlException();
					}
					xmlTextReader.Read();
					string text = null;
					string value = null;
					while (xmlTextReader.IsStartElement())
					{
						bool isEmptyElement = xmlTextReader.IsEmptyElement;
						string name = xmlTextReader.Name;
						string text2 = xmlTextReader.ReadString();
						if (string.Compare(name, nameElementTag, StringComparison.Ordinal) == 0)
						{
							text = text2;
						}
						else if (string.Compare(name, valueElementTag, StringComparison.Ordinal) == 0)
						{
							value = text2;
						}
						if (!isEmptyElement)
						{
							xmlTextReader.ReadEndElement();
						}
						else
						{
							xmlTextReader.Read();
						}
					}
					if (text == null)
					{
						throw new InvalidXmlException();
					}
					nameValueCollection.Add(text, value);
				}
				return nameValueCollection;
			}
			catch (XmlException ex)
			{
				throw new MalformedXmlException(ex);
			}
		}

		public static string NameValueCollectionToShallowXml(NameValueCollection parameters, string topElementTag)
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartElement(topElementTag);
			for (int i = 0; i < parameters.Count; i++)
			{
				string key = parameters.GetKey(i);
				string text = parameters.Get(i);
				if (key != null && text != null)
				{
					if (string.IsNullOrEmpty(key))
					{
						throw new InternalCatalogException("Empty Property Name");
					}
					string text2 = EncodePropertyName(key);
					RSTrace.CatalogTrace.Assert(!string.IsNullOrEmpty(text2), "encodedName");
					xmlTextWriter.WriteStartElement(text2);
					xmlTextWriter.WriteString(text);
					xmlTextWriter.WriteEndElement();
				}
			}
			xmlTextWriter.WriteEndElement();
			return stringWriter.ToString();
		}

		public static string NameValueCollectionToDeepXml(NameValueCollection parameters, string topElementTag, string eachElementTag, string nameElementTag, string valueElementTag)
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartElement(topElementTag);
			for (int i = 0; i < parameters.Count; i++)
			{
				xmlTextWriter.WriteStartElement(eachElementTag);
				string key = parameters.GetKey(i);
				if (key != null)
				{
					xmlTextWriter.WriteElementString(nameElementTag, key);
				}
				string text = parameters.Get(i);
				if (text != null)
				{
					xmlTextWriter.WriteElementString(valueElementTag, text);
				}
				xmlTextWriter.WriteEndElement();
			}
			xmlTextWriter.WriteEndElement();
			return stringWriter.ToString();
		}

		public static string EncodePropertyName(string name)
		{
			RSTrace.CatalogTrace.Assert(!string.IsNullOrEmpty(name), "name");
			return XmlConvert.EncodeLocalName(name);
		}

		public static string DecodePropertyName(string name)
		{
			RSTrace.CatalogTrace.Assert(!string.IsNullOrEmpty(name), "name");
			return XmlConvert.DecodeName(name);
		}

		public static XmlReaderSettings ApplyDtdDosDefense(XmlReaderSettings settings)
		{
			settings.ProhibitDtd = true;
			settings.XmlResolver = null;
			return settings;
		}

		public static XmlTextReader ApplyDtdDosDefense(XmlTextReader reader)
		{
			reader.ProhibitDtd = true;
			reader.XmlResolver = null;
			return reader;
		}
	}
}
