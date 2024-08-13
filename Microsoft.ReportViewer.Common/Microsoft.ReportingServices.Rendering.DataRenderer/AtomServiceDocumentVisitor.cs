using System.Collections.Specialized;
using System.Xml;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

public class AtomServiceDocumentVisitor : AtomSchemaVisitor
{
	private const string ElementTagService = "service";

	private const string ElementTagWorkspace = "workspace";

	private const string ElementTagCollection = "collection";

	private const string ElementTagTitle = "title";

	private const string AttributeNameHref = "href";

	public AtomServiceDocumentVisitor(XmlWriter xmlWriter, NameValueCollection reportServerParameters)
		: base(xmlWriter, reportServerParameters)
	{
	}

	public void CreateServiceDocument(Report report)
	{
		m_xmlWriter.WriteStartElement("service", "http://www.w3.org/2007/app");
		m_xmlWriter.WriteAttributeString("xmlns", "atom", null, "http://www.w3.org/2005/Atom");
		m_xmlWriter.WriteAttributeString("xmlns", "app", null, "http://www.w3.org/2007/app");
		m_xmlWriter.WriteStartElement("workspace");
		m_xmlWriter.WriteElementString("title", "http://www.w3.org/2005/Atom", string.IsNullOrEmpty(report.Name) ? "Default" : report.Name);
		m_baseUrl = BuildBaseUrl(report, m_reportServerParameters);
	}

	public override void WriteCollection(string definitionPath, string dataFeedName, string reportItemPath)
	{
		CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(m_baseUrl);
		catalogItemUrlBuilder.AppendRenderingParameter("ItemPath", reportItemPath);
		string value = catalogItemUrlBuilder.ToString();
		m_xmlWriter.WriteStartElement("collection");
		m_xmlWriter.WriteAttributeString("href", value);
		m_xmlWriter.WriteElementString("title", "http://www.w3.org/2005/Atom", dataFeedName);
		m_xmlWriter.WriteEndElement();
	}
}
