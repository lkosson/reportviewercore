using System.Collections;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class DictionaryContent : SyndicationContent
{
	private const string MimeApplicationXml = "application/xml";

	private const string ElementNameProperties = "properties";

	private const string AttributeNameType = "type";

	private const string AttributeNameNull = "null";

	public const string ElementNameProperty = "Property";

	public const string AttributeNameName = "Name";

	public const string AttributeNameCSDLType = "Type";

	public const string AttributeNameNullable = "Nullable";

	private const string AttributeNameIsGroup = "IsGroup";

	private const string AttributeNameIsParameter = "IsParameter";

	private const string AttributeNameIsContext = "IsContext";

	private const string AttributeValueTrue = "true";

	public const string AttributeValueFalse = "false";

	public const string EdmDefaultType = "Edm.String";

	private List<string> m_valueNames;

	private List<string> m_valueTypes;

	private List<string> m_valueContents;

	private Hashtable m_selectedColumns;

	public override string Type => "application/xml";

	public int Count => m_valueNames.Count;

	public DictionaryContent(Hashtable selectedColumns)
	{
		m_valueNames = new List<string>();
		m_valueTypes = new List<string>();
		m_valueContents = new List<string>();
		m_selectedColumns = selectedColumns;
	}

	private DictionaryContent(DictionaryContent other)
	{
		m_valueContents = other.m_valueContents;
		m_valueTypes = other.m_valueTypes;
		m_valueNames = other.m_valueNames;
		m_selectedColumns = other.m_selectedColumns;
	}

	public override SyndicationContent Clone()
	{
		return new DictionaryContent(this);
	}

	private bool CheckColumnIsSelected(string columnName)
	{
		if (m_selectedColumns != null)
		{
			return m_selectedColumns.ContainsKey(columnName);
		}
		return true;
	}

	internal void Add(string columnName, string edmType, string value)
	{
		if (CheckColumnIsSelected(columnName))
		{
			m_valueNames.Add(columnName);
			m_valueTypes.Add(edmType);
			m_valueContents.Add(value);
		}
	}

	internal void AddBeginning(string columnName, string edmType, string value)
	{
		if (CheckColumnIsSelected(columnName))
		{
			m_valueNames.Insert(0, columnName);
			m_valueTypes.Insert(0, edmType);
			m_valueContents.Insert(0, value);
		}
	}

	protected override void WriteContentsTo(XmlWriter xmlWriter)
	{
		if (0 < m_valueNames.Count)
		{
			xmlWriter.WriteStartElement("properties", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
			WriteRowContentsTo(xmlWriter);
			xmlWriter.WriteEndElement();
		}
	}

	private void WriteRowContentsTo(XmlWriter xmlWriter)
	{
		for (int i = 0; i < m_valueNames.Count; i++)
		{
			string name = m_valueNames[i];
			string text = m_valueTypes[i];
			string text2 = m_valueContents[i];
			string localName = XmlConvert.EncodeLocalName(name);
			xmlWriter.WriteStartElement(localName, "http://schemas.microsoft.com/ado/2007/08/dataservices");
			if (text != null)
			{
				xmlWriter.WriteAttributeString("type", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", text);
			}
			if (text2 == null)
			{
				xmlWriter.WriteAttributeString("null", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata", "true");
			}
			else
			{
				xmlWriter.WriteString(XmlVisitorBase.FilterInvalidXMLCharacters(text2));
			}
			xmlWriter.WriteEndElement();
		}
	}

	public void WriteRowContentsCSDL(XmlWriter xmlWriter, Dictionary<int, ColumnMetadata> metadata, int reportParameterCount, int feedLevelColumnCount)
	{
		int num = reportParameterCount + feedLevelColumnCount;
		for (int i = 0; i < m_valueNames.Count; i++)
		{
			string name = m_valueNames[i];
			string text = m_valueTypes[i];
			if (text == null)
			{
				text = "Edm.String";
			}
			string value = XmlConvert.EncodeLocalName(name);
			xmlWriter.WriteStartElement("Property");
			xmlWriter.WriteAttributeString("Name", value);
			xmlWriter.WriteAttributeString("Type", text);
			if (i < reportParameterCount)
			{
				xmlWriter.WriteAttributeString("IsParameter", "http://schemas.microsoft.com/sqlserver/reporting/2010/01/feedmetadata", "true");
			}
			else if (i < num)
			{
				xmlWriter.WriteAttributeString("IsContext", "http://schemas.microsoft.com/sqlserver/reporting/2010/01/feedmetadata", "true");
			}
			else
			{
				metadata.TryGetValue(i - reportParameterCount + 1, out var value2);
				if (value2 != null && value2.IsDynamic)
				{
					xmlWriter.WriteAttributeString("IsGroup", "http://schemas.microsoft.com/sqlserver/reporting/2010/01/feedmetadata", "true");
				}
			}
			xmlWriter.WriteEndElement();
		}
	}
}
