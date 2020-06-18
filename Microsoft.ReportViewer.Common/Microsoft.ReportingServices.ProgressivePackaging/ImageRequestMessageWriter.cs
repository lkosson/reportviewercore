using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal static class ImageRequestMessageWriter
	{
		public static void WriteElementsToStream(IEnumerable<ImageRequestMessageElement> messageElements, Stream s)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.CheckCharacters = false;
			xmlWriterSettings.Encoding = MessageUtil.StringEncoding;
			using (XmlWriter xmlWriter = XmlWriter.Create(s, xmlWriterSettings))
			{
				xmlWriter.WriteStartDocument();
				xmlWriter.WriteStartElement("ClientRequest", "http://schemas.microsoft.com/sqlserver/reporting/2011/01/getexternalimages");
				xmlWriter.WriteStartElement("ExternalImages");
				foreach (ImageRequestMessageElement messageElement in messageElements)
				{
					messageElement.Write(xmlWriter);
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndDocument();
			}
		}
	}
}
