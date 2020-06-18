using System;
using System.Xml;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal sealed class ImageRequestMessageElement : ImageMessageElement
	{
		internal const string NameSpace2011 = "http://schemas.microsoft.com/sqlserver/reporting/2011/01/getexternalimages";

		internal const string ClientRequestElement = "ClientRequest";

		internal const string ExternalImagesElement = "ExternalImages";

		internal const string ExternalImageElement = "ExternalImage";

		private const string UriElement = "Uri";

		private const string MaxWidthElement = "MaxWidth";

		private const string MaxHeightElement = "MaxHeight";

		public ImageRequestMessageElement()
		{
		}

		public ImageRequestMessageElement(string imageUrl, string imageWidth, string imageHeight)
			: base(imageUrl, imageWidth, imageHeight)
		{
		}

		public void Write(XmlWriter writer)
		{
			writer.WriteStartElement("ExternalImage");
			writer.WriteStartElement("Uri");
			writer.WriteString(base.ImageUrl);
			writer.WriteEndElement();
			writer.WriteStartElement("MaxWidth");
			writer.WriteValue(base.ImageWidth);
			writer.WriteEndElement();
			writer.WriteStartElement("MaxHeight");
			writer.WriteValue(base.ImageHeight);
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		public void Read(XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if ("Uri".Equals(reader.Name, StringComparison.Ordinal))
					{
						base.ImageUrl = ReadStringValue(reader);
					}
					else if ("MaxWidth".Equals(reader.Name, StringComparison.Ordinal))
					{
						base.ImageWidth = ReadStringValue(reader);
					}
					else if ("MaxHeight".Equals(reader.Name, StringComparison.Ordinal))
					{
						base.ImageHeight = ReadStringValue(reader);
					}
				}
			}
		}

		private string ReadStringValue(XmlReader reader)
		{
			if (!reader.IsEmptyElement)
			{
				reader.Read();
			}
			return reader.Value;
		}
	}
}
