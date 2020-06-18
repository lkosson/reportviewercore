using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal class ImageRequestMessageReader : ImageMessageReader<ImageRequestMessageElement>
	{
		private XmlReader m_xmlReader;

		public ImageRequestMessageReader(XmlReader xmlReader)
		{
			m_xmlReader = xmlReader;
		}

		public override IEnumerator<ImageRequestMessageElement> GetEnumerator()
		{
			while (m_xmlReader.Read())
			{
				if (m_xmlReader.NodeType == XmlNodeType.Element && "ExternalImage".Equals(m_xmlReader.Name, StringComparison.Ordinal))
				{
					using (XmlReader externalImageReader = m_xmlReader.ReadSubtree())
					{
						ImageRequestMessageElement imageRequestMessageElement = new ImageRequestMessageElement();
						imageRequestMessageElement.Read(externalImageReader);
						yield return imageRequestMessageElement;
					}
				}
			}
		}

		public override void InternalDispose()
		{
			m_xmlReader.Close();
		}
	}
}
