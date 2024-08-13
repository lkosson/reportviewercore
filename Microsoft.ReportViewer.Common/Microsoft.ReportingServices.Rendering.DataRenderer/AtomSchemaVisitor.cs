using System.Collections.Specialized;
using System.Xml;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

public abstract class AtomSchemaVisitor : AtomVisitor
{
	protected string m_baseUrl;

	protected AtomSchemaVisitor(XmlWriter xmlWriter, NameValueCollection reportServerParameters)
		: base(xmlWriter, reportServerParameters)
	{
	}

	public abstract void WriteCollection(string definitionPath, string dataFeedName, string reportItemPath);

	protected string BuildBaseUrl(Report report, NameValueCollection reportServerParameters)
	{
		string reportUrl = report.GetReportUrl(addReportParameters: true);
		CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(reportUrl);
		catalogItemUrlBuilder.AppendCatalogParameter("Command", "Render");
		string val = reportServerParameters["Format"];
		catalogItemUrlBuilder.AppendCatalogParameter("Format", val);
		return catalogItemUrlBuilder.ToString();
	}
}
