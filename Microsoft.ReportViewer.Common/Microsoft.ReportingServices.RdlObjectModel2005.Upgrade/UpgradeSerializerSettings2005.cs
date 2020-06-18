using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005.Upgrade
{
	internal class UpgradeSerializerSettings2005 : RdlSerializerSettings
	{
		private bool m_skippingInvalidElements;

		private SerializerHost2005 m_host;

		private const string m_xsdResourceId = "Microsoft.ReportingServices.RdlObjectModel.RdlUpgrade.Rdl2005ObjectModel.ReportDefinition.xsd";

		private static XmlElementAttribute[] m_deserializingReportItems = new XmlElementAttribute[10]
		{
			new XmlElementClassAttribute("Line", typeof(Line2005)),
			new XmlElementClassAttribute("Rectangle", typeof(Rectangle2005)),
			new XmlElementClassAttribute("Textbox", typeof(Textbox2005)),
			new XmlElementClassAttribute("Image", typeof(Image2005)),
			new XmlElementClassAttribute("Subreport", typeof(Subreport2005)),
			new XmlElementClassAttribute("Chart", typeof(Chart2005)),
			new XmlElementClassAttribute("List", typeof(List2005)),
			new XmlElementClassAttribute("Table", typeof(Table2005)),
			new XmlElementClassAttribute("Matrix", typeof(Matrix2005)),
			new XmlElementClassAttribute("CustomReportItem", typeof(CustomReportItem2005))
		};

		public SerializerHost2005 SerializerHost => m_host;

		private UpgradeSerializerSettings2005(bool serializing)
		{
			m_host = new SerializerHost2005(serializing);
			base.Host = m_host;
		}

		public static UpgradeSerializerSettings2005 CreateReaderSettings()
		{
			UpgradeSerializerSettings2005 upgradeSerializerSettings = new UpgradeSerializerSettings2005(serializing: false);
			upgradeSerializerSettings.ValidateXml = true;
			upgradeSerializerSettings.Normalize = false;
			upgradeSerializerSettings.XmlSchema = XmlSchema.Read(Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.ReportingServices.RdlObjectModel.RdlUpgrade.Rdl2005ObjectModel.ReportDefinition.xsd"), null);
			upgradeSerializerSettings.XmlValidationEventHandler = (ValidationEventHandler)Delegate.Combine(upgradeSerializerSettings.XmlValidationEventHandler, new ValidationEventHandler(upgradeSerializerSettings.ValidationEventHandler));
			upgradeSerializerSettings.IgnoreWhitespace = true;
			XmlAttributeOverrides xmlAttributeOverrides2 = upgradeSerializerSettings.XmlAttributeOverrides = new XmlAttributeOverrides();
			XmlAttributes xmlAttributes = new XmlAttributes();
			XmlElementAttribute[] deserializingReportItems = m_deserializingReportItems;
			foreach (XmlElementAttribute attribute in deserializingReportItems)
			{
				xmlAttributes.XmlElements.Add(attribute);
			}
			xmlAttributeOverrides2.Add(typeof(Microsoft.ReportingServices.RdlObjectModel.ReportItem), xmlAttributes);
			xmlAttributes = new XmlAttributes();
			xmlAttributes.XmlElements.Add(new XmlElementAttribute("SortBy", typeof(SortBy2005)));
			xmlAttributeOverrides2.Add(typeof(SortExpression), xmlAttributes);
			return upgradeSerializerSettings;
		}

		public static UpgradeSerializerSettings2005 CreateWriterSettings()
		{
			return new UpgradeSerializerSettings2005(serializing: true);
		}

		private void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			XmlReader xmlReader = sender as XmlReader;
			if (xmlReader == null)
			{
				return;
			}
			string b = RDLUpgrader.Get2005NamespaceURI();
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
