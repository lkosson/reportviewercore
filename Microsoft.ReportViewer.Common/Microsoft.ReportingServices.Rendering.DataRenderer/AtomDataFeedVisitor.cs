using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

public class AtomDataFeedVisitor : AtomVisitor
{
	protected class TypedColumnValue
	{
		public string Value;

		public string EdmType;

		public TypedColumnValue(string value, string edmType)
		{
			Value = value;
			EdmType = edmType;
		}
	}

	private const string NsNameData = "d";

	private const string NsNameMetadata = "m";

	public const string NsUriData = "http://schemas.microsoft.com/ado/2007/08/dataservices";

	public const string NsUriMetadata = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

	private const char ParameterValueSeparator = ',';

	private const char ColumnNameSuffixConnectString = '_';

	protected ReportParameterCollection m_reportParamters;

	protected SyndicationFeed m_syndicationFeed;

	protected SyndicationItem m_syndicationItem;

	private DictionaryContent m_rowContent;

	protected List<TypedColumnValue> m_feedLevelRowContent;

	private ReportWalker m_dataRegionWalker;

	protected ReportItem m_reportItem;

	private List<string> m_parameterNames;

	private Dictionary<int, string> m_columnNames = new Dictionary<int, string>();

	protected Dictionary<int, ColumnMetadata> m_columnMetadata = new Dictionary<int, ColumnMetadata>();

	private Dictionary<int, string> m_resolvedColumnEdmTypes;

	protected int m_columnCount;

	protected Dictionary<TypeCode, string> m_typeCodeToEdmMapping;

	private bool m_discardRow;

	private bool m_hasDuplicateColumnNames;

	private Dictionary<string, object> m_dataFeedQueries;

	private Hashtable m_selectedColumns;

	public AtomDataFeedVisitor(XmlWriter xmlWriter, NameValueCollection reportServerParameters, ReportParameterCollection reportParamters, Dictionary<string, object> dataFeedQueries)
		: base(xmlWriter, reportServerParameters)
	{
		m_reportParamters = reportParamters;
		if (m_reportParamters != null)
		{
			m_parameterNames = new List<string>(m_reportParamters.Count);
			foreach (ReportParameter reportParamter in m_reportParamters)
			{
				m_parameterNames.Add(reportParamter.Name);
			}
		}
		else
		{
			m_parameterNames = new List<string>(0);
		}
		m_typeCodeToEdmMapping = new Dictionary<TypeCode, string>();
		m_typeCodeToEdmMapping.Add(TypeCode.Boolean, "Edm.Boolean");
		m_typeCodeToEdmMapping.Add(TypeCode.Byte, "Edm.Byte");
		m_typeCodeToEdmMapping.Add(TypeCode.SByte, "Edm.SByte");
		m_typeCodeToEdmMapping.Add(TypeCode.DateTime, "Edm.DateTime");
		m_typeCodeToEdmMapping.Add(TypeCode.Decimal, "Edm.Decimal");
		m_typeCodeToEdmMapping.Add(TypeCode.Single, "Edm.Single");
		m_typeCodeToEdmMapping.Add(TypeCode.Double, "Edm.Double");
		m_typeCodeToEdmMapping.Add(TypeCode.Int16, "Edm.Int16");
		m_typeCodeToEdmMapping.Add(TypeCode.Int32, "Edm.Int32");
		m_typeCodeToEdmMapping.Add(TypeCode.Int64, "Edm.Int64");
		m_typeCodeToEdmMapping.Add(TypeCode.UInt16, "Edm.Int16");
		m_typeCodeToEdmMapping.Add(TypeCode.UInt32, "Edm.Int32");
		m_typeCodeToEdmMapping.Add(TypeCode.UInt64, "Edm.Int64");
		m_dataFeedQueries = dataFeedQueries;
		string[] queryValueSelect = GetQueryValueSelect();
		if (queryValueSelect == null)
		{
			return;
		}
		m_selectedColumns = new Hashtable();
		string[] array = queryValueSelect;
		foreach (string key in array)
		{
			if (!m_selectedColumns.ContainsKey(key))
			{
				m_selectedColumns.Add(key, null);
			}
		}
	}

	internal void WriteDataRegionFeed(ReportWalker dataRegionWalker, ReportItem reportItem)
	{
		WriteDataRegionFeed(dataRegionWalker, reportItem, null);
	}

	internal void WriteDataRegionFeed(ReportWalker dataRegionWalker, ReportItem reportItem, string feedName)
	{
		m_dataRegionWalker = dataRegionWalker;
		m_reportItem = reportItem;
		if (m_hasDuplicateColumnNames)
		{
			ResolveDuplicateColumnNames();
		}
		m_syndicationFeed.Items = GenerateSyndicationFeedItems();
		WriteDataRegionFeed(feedName);
	}

	protected virtual void WriteDataRegionFeed(string feedName)
	{
		Atom10FeedFormatter atom10Formatter = m_syndicationFeed.GetAtom10Formatter();
		atom10Formatter.WriteTo(m_xmlWriter);
	}

	private void ResolveDuplicateColumnNames()
	{
		List<string> list = new List<string>(m_columnCount);
		for (int i = 1; i <= m_columnCount; i++)
		{
			string text = m_columnNames[i];
			if (list.Contains(text) || m_parameterNames.Contains(text))
			{
				int num = 1;
				string text2;
				do
				{
					text2 = text + '_' + num.ToString(CultureInfo.InvariantCulture);
					num++;
				}
				while (list.Contains(text2) || m_columnNames.ContainsValue(text2) || m_parameterNames.Contains(text2));
				text = text2;
			}
			list.Add(text);
			m_columnNames[i] = null;
		}
		for (int j = 1; j <= m_columnCount; j++)
		{
			m_columnNames[j] = list[j - 1];
		}
	}

	public void CreateSyndicationFeed()
	{
		m_syndicationFeed = new SyndicationFeed();
		XmlQualifiedName key = new XmlQualifiedName("d", "http://www.w3.org/2000/xmlns/");
		m_syndicationFeed.AttributeExtensions.Add(key, "http://schemas.microsoft.com/ado/2007/08/dataservices");
		key = new XmlQualifiedName("m", "http://www.w3.org/2000/xmlns/");
		m_syndicationFeed.AttributeExtensions.Add(key, "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
	}

	public void DiscardRow()
	{
		m_discardRow = true;
	}

	protected virtual void ResetRowState()
	{
		m_syndicationItem = null;
		m_discardRow = false;
	}

	private bool IsRowOnPath()
	{
		if (m_syndicationItem != null)
		{
			return !m_discardRow;
		}
		return false;
	}

	public IEnumerable<SyndicationItem> GenerateSyndicationFeedItems()
	{
		uint? numTopItems = GetQueryValueTop();
		if (numTopItems.HasValue)
		{
			uint? num = numTopItems;
			if (num.GetValueOrDefault() == 0 && num.HasValue)
			{
				yield break;
			}
		}
		uint numItems = 0u;
		if (!m_dataRegionWalker.InitializeDataRegionOrMap(m_reportItem))
		{
			yield break;
		}
		ResetRowState();
		while (true)
		{
			if (m_dataRegionWalker.WalkNextDataRegionOrMapRow(m_reportItem))
			{
				if (IsRowOnPath())
				{
					AddReportParametersToRowContent();
					yield return m_syndicationItem;
					numItems++;
					if (numTopItems.HasValue && numItems == numTopItems)
					{
						break;
					}
				}
				ResetRowState();
				continue;
			}
			if (IsRowOnPath())
			{
				AddReportParametersToRowContent();
				yield return m_syndicationItem;
			}
			break;
		}
	}

	private void CreateNewSyndicationItem()
	{
		m_columnCount = 0;
		m_syndicationItem = new SyndicationItem();
		m_syndicationItem.Title = new TextSyndicationContent(string.Empty);
		m_syndicationItem.Authors.Add(new SyndicationPerson());
		m_rowContent = new DictionaryContent(m_selectedColumns);
		m_syndicationItem.Content = m_rowContent;
		if (m_feedLevelRowContent == null)
		{
			return;
		}
		foreach (TypedColumnValue item in m_feedLevelRowContent)
		{
			m_columnCount++;
			string columnName = m_columnNames[m_columnCount];
			m_rowContent.Add(columnName, item.EdmType, item.Value);
		}
	}

	internal virtual void WriteValue(string value, TypeCode typeCode)
	{
		if (m_syndicationItem == null)
		{
			CreateNewSyndicationItem();
		}
		m_columnCount++;
		string columnName = m_columnNames[m_columnCount];
		string value2;
		if (m_resolvedColumnEdmTypes.ContainsKey(m_columnCount))
		{
			value2 = m_resolvedColumnEdmTypes[m_columnCount];
		}
		else
		{
			m_typeCodeToEdmMapping.TryGetValue(typeCode, out value2);
		}
		m_rowContent.Add(columnName, value2, value);
	}

	internal void WriteFeedLevelValue(string value, TypeCode typeCode)
	{
		if (m_feedLevelRowContent == null)
		{
			m_feedLevelRowContent = new List<TypedColumnValue>();
		}
		m_typeCodeToEdmMapping.TryGetValue(typeCode, out var value2);
		m_feedLevelRowContent.Add(new TypedColumnValue(value, value2));
	}

	internal void AddReportParametersToRowContent()
	{
		if (m_reportParamters == null)
		{
			return;
		}
		for (int num = m_reportParamters.Count - 1; num >= 0; num--)
		{
			ReportParameter reportParameter = m_reportParamters[num];
			string value = null;
			string value2 = null;
			if (reportParameter.MultiValue)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < reportParameter.Instance.Values.Count; i++)
				{
					if (i != 0)
					{
						stringBuilder.Append(',');
					}
					string value3 = AtomDataFeedHandler.XmlConvertOrginalValue(reportParameter.Instance.Values[i]);
					stringBuilder.Append(value3);
				}
				value2 = stringBuilder.ToString();
			}
			else
			{
				m_typeCodeToEdmMapping.TryGetValue(reportParameter.DataType, out value);
				if (reportParameter.Instance.Value != null)
				{
					value2 = AtomDataFeedHandler.XmlConvertOrginalValue(reportParameter.Instance.Value);
				}
			}
			m_rowContent.AddBeginning(reportParameter.Name, value, value2);
		}
	}

	internal void ClearColumnNames()
	{
		m_columnNames.Clear();
		m_columnCount = 0;
		m_hasDuplicateColumnNames = false;
		m_columnMetadata.Clear();
	}

	internal void ResetColumnNames()
	{
		if (m_feedLevelRowContent == null)
		{
			m_columnNames.Clear();
			m_columnCount = 0;
			m_columnMetadata.Clear();
		}
		else
		{
			int count = m_columnNames.Count;
			for (int i = m_feedLevelRowContent.Count + 1; i <= count; i++)
			{
				m_columnNames.Remove(i);
				m_columnMetadata.Remove(i);
			}
			m_columnCount = m_feedLevelRowContent.Count;
		}
		m_hasDuplicateColumnNames = false;
	}

	internal void ClearFeedLevelRowContent()
	{
		m_feedLevelRowContent = null;
	}

	internal void AddColumnName(string columnName)
	{
		AddColumnName(columnName, null);
	}

	internal void AddColumnName(string columnName, object source)
	{
		m_columnCount++;
		if (!m_hasDuplicateColumnNames && (m_columnNames.ContainsValue(columnName) || m_parameterNames.Contains(columnName)))
		{
			m_hasDuplicateColumnNames = true;
		}
		m_columnNames.Add(m_columnCount, columnName);
		if (source != null)
		{
			ColumnMetadata columnMetadata = GetColumnMetadata(source);
			if (columnMetadata != null)
			{
				m_columnMetadata.Add(m_columnCount, columnMetadata);
			}
		}
	}

	internal void SetResolvedEdmTypes(List<string> resolvedEdmTypes)
	{
		m_resolvedColumnEdmTypes = new Dictionary<int, string>();
		if (resolvedEdmTypes == null)
		{
			return;
		}
		int num = 1;
		if (m_feedLevelRowContent != null)
		{
			num += m_feedLevelRowContent.Count;
		}
		foreach (string resolvedEdmType in resolvedEdmTypes)
		{
			m_resolvedColumnEdmTypes.Add(num, resolvedEdmType);
			num++;
		}
	}

	private ColumnMetadata GetColumnMetadata(object item)
	{
		if (item is TextBox textBox)
		{
			ColumnMetadata columnMetadata = new ColumnMetadata();
			columnMetadata.IsDynamic = false;
			if (textBox.ParentDefinitionPath is TablixHeader tablixHeader && tablixHeader.ParentDefinitionPath is TablixMember tablixMember)
			{
				columnMetadata.IsDynamic = !tablixMember.IsStatic;
			}
			return columnMetadata;
		}
		if (item is ChartMember chartMember)
		{
			ColumnMetadata columnMetadata2 = new ColumnMetadata();
			columnMetadata2.IsDynamic = !chartMember.IsStatic;
			return columnMetadata2;
		}
		if (item is MapMember mapMember)
		{
			ColumnMetadata columnMetadata3 = new ColumnMetadata();
			columnMetadata3.IsDynamic = !mapMember.IsStatic;
			return columnMetadata3;
		}
		return null;
	}

	private uint? GetQueryValueTop()
	{
		if (m_dataFeedQueries != null && m_dataFeedQueries.TryGetValue("Top", out var value))
		{
			return (uint)value;
		}
		return null;
	}

	private string[] GetQueryValueSelect()
	{
		if (m_dataFeedQueries != null && m_dataFeedQueries.TryGetValue("Select", out var value))
		{
			return (string[])value;
		}
		return null;
	}
}
