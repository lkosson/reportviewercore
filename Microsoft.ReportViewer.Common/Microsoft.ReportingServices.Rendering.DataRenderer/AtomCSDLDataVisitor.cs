using System.Collections;
using System.Collections.Specialized;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

public class AtomCSDLDataVisitor : AtomDataFeedVisitor
{
	private const string ElementNameEntityType = "EntityType";

	private const string ElementNameKey = "Key";

	private const string ElementNamePropertyRef = "PropertyRef";

	private const string AttributeNameName = "Name";

	private const string AttributeNameDataRegionType = "DataRegionType";

	private const string Tablix = "Tablix";

	private const string Chart = "Chart";

	private const string Gauge = "Gauge";

	private const string Map = "Map";

	private const string Unknown = "Unknown";

	private const string KeyColumnName = "Key";

	public AtomCSDLDataVisitor(XmlWriter xmlWriter, NameValueCollection reportServerParameters, ReportParameterCollection reportParamters)
		: base(xmlWriter, reportServerParameters, reportParamters, null)
	{
	}

	protected override void WriteDataRegionFeed(string feedName)
	{
		IEnumerator enumerator = m_syndicationFeed.Items.GetEnumerator();
		enumerator.MoveNext();
		SyndicationItem syndicationItem = enumerator.Current as SyndicationItem;
		m_xmlWriter.WriteStartElement("EntityType");
		m_xmlWriter.WriteAttributeString("Name", feedName);
		string dataRegionType = GetDataRegionType(m_reportItem);
		m_xmlWriter.WriteAttributeString("DataRegionType", "http://schemas.microsoft.com/sqlserver/reporting/2010/01/feedmetadata", dataRegionType);
		m_xmlWriter.WriteStartElement("Key");
		m_xmlWriter.WriteStartElement("PropertyRef");
		m_xmlWriter.WriteAttributeString("Name", "Key");
		m_xmlWriter.WriteEndElement();
		m_xmlWriter.WriteEndElement();
		m_xmlWriter.WriteStartElement("Property");
		m_xmlWriter.WriteAttributeString("Name", "Key");
		m_xmlWriter.WriteAttributeString("Type", "Edm.String");
		m_xmlWriter.WriteAttributeString("Nullable", "false");
		m_xmlWriter.WriteEndElement();
		if (syndicationItem != null)
		{
			DictionaryContent dictionaryContent = (DictionaryContent)syndicationItem.Content;
			int reportParameterCount = 0;
			if (m_reportParamters != null)
			{
				reportParameterCount = m_reportParamters.Count;
			}
			int feedLevelColumnCount = 0;
			if (m_feedLevelRowContent != null)
			{
				feedLevelColumnCount = m_feedLevelRowContent.Count;
			}
			dictionaryContent.WriteRowContentsCSDL(m_xmlWriter, m_columnMetadata, reportParameterCount, feedLevelColumnCount);
		}
		m_xmlWriter.WriteEndElement();
	}

	private static string GetDataRegionType(ReportItem reportItem)
	{
		if (reportItem is Tablix)
		{
			return "Tablix";
		}
		if (reportItem is Chart)
		{
			return "Chart";
		}
		if (reportItem is GaugePanel)
		{
			return "Gauge";
		}
		if (reportItem is Map)
		{
			return "Map";
		}
		return "Unknown";
	}
}
