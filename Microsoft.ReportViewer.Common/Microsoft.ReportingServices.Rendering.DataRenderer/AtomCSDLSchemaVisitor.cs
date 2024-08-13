using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

public class AtomCSDLSchemaVisitor : AtomSchemaVisitor
{
	private const string ElementTagEdmx = "Edmx";

	private const string ElementTagDataServices = "DataServices";

	private const string ElementTagSchema = "Schema";

	private const string ElementTagEntityContainer = "EntityContainer";

	private const string ElementTagNamespace = "Namespace";

	private const string ElementTagEntitySet = "EntitySet";

	private const string AttributeNameName = "Name";

	private const string AttributeNameEntityType = "EntityType";

	private const string AttributeNameVersion = "Version";

	protected const string NsUriEdm = "http://schemas.microsoft.com/ado/2006/04/edm";

	protected const string NsNameEdmx = "edmx";

	protected const string NsUriEdmx = "http://schemas.microsoft.com/ado/2007/06/edmx";

	public const string NsNameFeedmetadata = "rs";

	public const string NsUriFeedmetadata = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/feedmetadata";

	private const char NamespaceSeparator = '.';

	private const string EntityTypePostfix = "Item";

	private const string EdmxVersion = "1.0";

	private const string SchemaNamespaceName = "Report";

	private Report m_report;

	private Dictionary<string, string> m_entityTypes;

	public Dictionary<string, string> EntityTypes => m_entityTypes;

	public string SchemaNamespace => "Report";

	public AtomCSDLSchemaVisitor(XmlWriter xmlWriter, NameValueCollection reportServerParameters)
		: base(xmlWriter, reportServerParameters)
	{
	}

	public void CreateCSDLDocument(Report report)
	{
		m_report = report;
		m_entityTypes = new Dictionary<string, string>();
		m_xmlWriter.WriteStartElement("edmx", "Edmx", "http://schemas.microsoft.com/ado/2007/06/edmx");
		m_xmlWriter.WriteAttributeString("Version", "1.0");
		m_xmlWriter.WriteStartElement("DataServices", "http://schemas.microsoft.com/ado/2007/06/edmx");
		m_xmlWriter.WriteStartElement("Schema", "http://schemas.microsoft.com/ado/2006/04/edm");
		m_xmlWriter.WriteAttributeString("xmlns", "rs", null, "http://schemas.microsoft.com/sqlserver/reporting/2010/01/feedmetadata");
		m_xmlWriter.WriteAttributeString("Namespace", SchemaNamespace);
	}

	public void StartEntityContainer()
	{
		m_xmlWriter.WriteStartElement("EntityContainer");
		m_xmlWriter.WriteAttributeString("Name", "ReportDataFeedContainer");
	}

	public void EndEntityContainer()
	{
		m_xmlWriter.WriteEndElement();
	}

	public override void WriteCollection(string definitionPath, string dataFeedName, string reportItemPath)
	{
		m_xmlWriter.WriteStartElement("EntitySet");
		m_xmlWriter.WriteAttributeString("Name", dataFeedName);
		m_xmlWriter.WriteAttributeString("EntityType", BuildEntityTypeName(dataFeedName, includeNamespace: true));
		m_entityTypes.Add(definitionPath, BuildEntityTypeName(dataFeedName, includeNamespace: false));
		m_xmlWriter.WriteEndElement();
	}

	private string BuildEntityTypeName(string dataFeedName, bool includeNamespace)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (includeNamespace)
		{
			stringBuilder.Append(SchemaNamespace);
			stringBuilder.Append('.');
		}
		stringBuilder.Append(dataFeedName);
		stringBuilder.Append("Item");
		return stringBuilder.ToString();
	}
}
