using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2008.Upgrade
{
	internal class UpgradeSerializerSettings2008 : RdlSerializerSettings
	{
		private bool m_skippingInvalidElements;

		private SerializerHost2008 m_host;

		private const string m_xsdResourceId = "Microsoft.ReportingServices.RdlObjectModel.RdlUpgrade.Rdl2008ObjectModel.ReportDefinition.xsd";

		private static XmlElementAttribute[] m_deserializingReportItems = new XmlElementAttribute[0];

		public SerializerHost2008 SerializerHost => m_host;

		private UpgradeSerializerSettings2008(bool serializing)
		{
			m_host = new SerializerHost2008(serializing);
			base.Host = m_host;
		}

		public static UpgradeSerializerSettings2008 CreateReaderSettings()
		{
			UpgradeSerializerSettings2008 upgradeSerializerSettings = new UpgradeSerializerSettings2008(serializing: false);
			upgradeSerializerSettings.ValidateXml = true;
			upgradeSerializerSettings.Normalize = false;
			upgradeSerializerSettings.XmlSchema = XmlSchema.Read(Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.ReportingServices.RdlObjectModel.RdlUpgrade.Rdl2008ObjectModel.ReportDefinition.xsd"), null);
			upgradeSerializerSettings.XmlValidationEventHandler = (ValidationEventHandler)Delegate.Combine(upgradeSerializerSettings.XmlValidationEventHandler, new ValidationEventHandler(upgradeSerializerSettings.ValidationEventHandler));
			upgradeSerializerSettings.IgnoreWhitespace = false;
			return upgradeSerializerSettings;
		}

		public static UpgradeSerializerSettings2008 CreateWriterSettings()
		{
			return new UpgradeSerializerSettings2008(serializing: true);
		}

		private void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			XmlReader xmlReader = sender as XmlReader;
			if (xmlReader == null)
			{
				return;
			}
			string b = RDLUpgrader.Get2008NamespaceURI();
			if (xmlReader.NamespaceURI == b)
			{
				throw e.Exception;
			}
			if (m_skippingInvalidElements)
			{
				return;
			}
			m_skippingInvalidElements = true;
			StringBuilder stringBuilder = new StringBuilder();
			while (!xmlReader.EOF && (xmlReader.NodeType == XmlNodeType.Element || xmlReader.NodeType == XmlNodeType.Text) && xmlReader.NamespaceURI != b)
			{
				if (xmlReader.NodeType == XmlNodeType.Text)
				{
					stringBuilder.Append(xmlReader.ReadString());
				}
				else
				{
					xmlReader.Skip();
				}
				xmlReader.MoveToContent();
			}
			m_host.ExtraStringData = stringBuilder.ToString();
			m_skippingInvalidElements = false;
		}
	}
}
