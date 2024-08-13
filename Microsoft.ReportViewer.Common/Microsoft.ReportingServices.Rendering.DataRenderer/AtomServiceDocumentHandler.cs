using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class AtomServiceDocumentHandler : HandlerBase
{
	internal class DynamicElement
	{
		public string DefinitionPath;

		public string DataElementName;

		public string ReportItemName;

		public List<DynamicElement> Children = new List<DynamicElement>();

		public DynamicElement(string definitionPath, string dataElementName, string reportItemName)
		{
			DefinitionPath = definitionPath;
			DataElementName = dataElementName;
			ReportItemName = reportItemName;
		}

		public bool IsBranch()
		{
			string text = null;
			foreach (DynamicElement child in Children)
			{
				if (text == null)
				{
					text = child.DefinitionPath;
					continue;
				}
				if (text != child.DefinitionPath)
				{
					return true;
				}
				text = child.DefinitionPath;
			}
			return false;
		}

		public bool IsRoot()
		{
			return DefinitionPath == null;
		}

		public static DynamicElement CreateRoot()
		{
			return new DynamicElement(null, null, null);
		}
	}

	private const char ReportItemPathSeparator = '.';

	private AtomSchemaVisitor m_visitor;

	private DynamicElement m_dynamicElementRoot;

	protected Stack<DynamicElement> m_dynamicElementScope;

	private Dictionary<string, int> m_feedNames;

	private bool m_multipleFeeds;

	internal DynamicElement DynamicElementRoot => m_dynamicElementRoot;

	internal AtomServiceDocumentHandler(AtomSchemaVisitor visitor)
		: this(visitor, new Dictionary<string, int>())
	{
	}

	internal AtomServiceDocumentHandler(AtomSchemaVisitor visitor, Dictionary<string, int> feedNames)
	{
		m_visitor = visitor;
		m_dynamicElementRoot = DynamicElement.CreateRoot();
		m_dynamicElementScope = new Stack<DynamicElement>();
		m_dynamicElementScope.Push(m_dynamicElementRoot);
		m_feedNames = feedNames;
		m_multipleFeeds = false;
	}

	protected void OnDynamicMemberBegin(string definitionPath, string dataElementName, string reportItemName)
	{
		DynamicElement item = new DynamicElement(definitionPath, dataElementName, reportItemName);
		DynamicElement dynamicElement = m_dynamicElementScope.Peek();
		dynamicElement.Children.Add(item);
		if (dynamicElement.IsBranch())
		{
			m_multipleFeeds = true;
		}
		m_dynamicElementScope.Push(item);
	}

	public override void OnTablixBegin(Tablix tablix, ref bool walkTablix, bool outputTablix)
	{
		base.OnTablixBegin(tablix, ref walkTablix, outputTablix);
		if (walkTablix)
		{
			OnDynamicMemberBegin(tablix.DefinitionPath, tablix.DataElementName, tablix.Name);
		}
	}

	public override void OnTablixEnd(Tablix tablix)
	{
		base.OnTablixEnd(tablix);
		m_dynamicElementScope.Pop();
	}

	public override void OnTablixMemberBegin(TablixMember tablixMember, ref bool walkThisMember, bool outputThisMember)
	{
		base.OnTablixMemberBegin(tablixMember, ref walkThisMember, outputThisMember);
		if (walkThisMember && tablixMember.Group != null && tablixMember.IsColumn)
		{
			OnDynamicMemberBegin(tablixMember.DefinitionPath, tablixMember.DataElementName, tablixMember.Group.Name);
		}
	}

	public override void OnTablixMemberEnd(TablixMember tablixMember)
	{
		base.OnTablixMemberEnd(tablixMember);
		if (tablixMember.Group != null && tablixMember.IsColumn)
		{
			m_dynamicElementScope.Pop();
		}
	}

	public override void OnChartBegin(Chart chart, bool outputChart, ref bool walkChart)
	{
		base.OnChartBegin(chart, outputChart, ref walkChart);
		if (walkChart)
		{
			OnDynamicMemberBegin(chart.DefinitionPath, chart.DataElementName, chart.Name);
		}
	}

	public override void OnChartEnd(Chart chart)
	{
		base.OnChartEnd(chart);
		m_dynamicElementScope.Pop();
	}

	public override void OnChartMemberBegin(ChartMember chartMember, bool outputThisMember, bool outputMemberLabelColumn, string parentScopeName, ref bool walkThisMember)
	{
		base.OnChartMemberBegin(chartMember, outputThisMember, outputMemberLabelColumn, parentScopeName, ref walkThisMember);
		if (walkThisMember && chartMember.Group != null && chartMember.IsCategory)
		{
			OnDynamicMemberBegin(chartMember.DefinitionPath, chartMember.DataElementName, chartMember.Group.Name);
		}
	}

	public override void OnChartMemberEnd(ChartMember chartMember)
	{
		base.OnChartMemberEnd(chartMember);
		if (chartMember.Group != null && chartMember.IsCategory)
		{
			m_dynamicElementScope.Pop();
		}
	}

	public override void OnGaugePanelBegin(GaugePanel gaugePanel, bool outputGaugePanelData, ref bool walkGaugePanel)
	{
		base.OnGaugePanelBegin(gaugePanel, outputGaugePanelData, ref walkGaugePanel);
		if (walkGaugePanel)
		{
			OnDynamicMemberBegin(gaugePanel.DefinitionPath, gaugePanel.DataElementName, gaugePanel.Name);
		}
	}

	public override void OnGaugePanelEnd(GaugePanel gaugePanel)
	{
		base.OnGaugePanelEnd(gaugePanel);
		m_dynamicElementScope.Pop();
	}

	public override void OnMapBegin(Map map, bool outputMap, ref bool walkMap)
	{
		base.OnMapBegin(map, outputMap, ref walkMap);
		if (walkMap)
		{
			OnDynamicMemberBegin(map.DefinitionPath, map.DataElementName, map.Name);
		}
	}

	public override void OnMapEnd(Map map)
	{
		base.OnMapEnd(map);
		m_dynamicElementScope.Pop();
	}

	public override void OnMapMemberBegin(MapMember mapMember, MapSpatialElementTemplate template, bool outputMapMemberLabel, bool outputDynamicMembers, MapVectorLayer layer, ref bool walkMapMember)
	{
		base.OnMapMemberBegin(mapMember, template, outputMapMemberLabel, outputDynamicMembers, layer, ref walkMapMember);
		if (mapMember.Group != null)
		{
			OnDynamicMemberBegin(mapMember.DefinitionPath, layer.MapDataRegionName, mapMember.Group.Name);
		}
	}

	public override void OnMapMemberEnd(MapMember mapMember)
	{
		base.OnMapMemberEnd(mapMember);
		if (mapMember.Group != null)
		{
			m_dynamicElementScope.Pop();
		}
	}

	public void WriteCollection(DynamicElement topLevelScope)
	{
		if (m_dynamicElementRoot.Children.Count > 0)
		{
			RSTrace.RenderingTracer.Assert(m_dynamicElementRoot.Children.Count == 1, "AtomServiceDocument.WriteCollection: There is more than one top level dynamic element");
			DynamicElement dynamicElement = m_dynamicElementRoot.Children[0];
			string feedName = null;
			if (!m_multipleFeeds)
			{
				feedName = dynamicElement.DataElementName;
			}
			string reportItemPath = null;
			if (topLevelScope != null)
			{
				GetScopeElementReportItemPath(topLevelScope, dynamicElement.DefinitionPath, ref reportItemPath);
			}
			List<string> feedsWritten = new List<string>();
			TraverseAndWriteCollections(dynamicElement, feedName, reportItemPath, feedsWritten);
		}
	}

	private bool GetScopeElementReportItemPath(DynamicElement scopeElement, string definitionPath, ref string reportItemPath)
	{
		if (scopeElement.DefinitionPath == definitionPath)
		{
			return true;
		}
		foreach (DynamicElement child in scopeElement.Children)
		{
			if (!GetScopeElementReportItemPath(child, definitionPath, ref reportItemPath))
			{
				continue;
			}
			if (!scopeElement.IsRoot())
			{
				if (reportItemPath == null)
				{
					reportItemPath = scopeElement.ReportItemName;
				}
				else
				{
					reportItemPath = scopeElement.ReportItemName + '.' + reportItemPath;
				}
			}
			return true;
		}
		return false;
	}

	public DynamicElement GetTopLevelDynamicElement()
	{
		return m_dynamicElementRoot.Children[0];
	}

	public static string AppendToReportItemPath(string parentReportItemPath, DynamicElement dynamicElement)
	{
		string text = parentReportItemPath;
		if (!string.IsNullOrEmpty(text))
		{
			text += '.';
		}
		return text + dynamicElement.ReportItemName;
	}

	private void TraverseAndWriteCollections(DynamicElement dynamicElement, string feedName, string parentReportItemPath, List<string> feedsWritten)
	{
		string text = AppendToReportItemPath(parentReportItemPath, dynamicElement);
		if (dynamicElement.Children.Count > 0)
		{
			foreach (DynamicElement child in dynamicElement.Children)
			{
				TraverseAndWriteCollections(child, feedName, text, feedsWritten);
			}
			return;
		}
		if (feedName == null)
		{
			feedName = dynamicElement.DataElementName;
		}
		if (feedsWritten.Contains(dynamicElement.DefinitionPath))
		{
			return;
		}
		if (m_feedNames.ContainsKey(feedName))
		{
			int num = m_feedNames[feedName];
			string text2;
			do
			{
				num = (m_feedNames[feedName] = num + 1);
				text2 = feedName + "_" + num.ToString(CultureInfo.InvariantCulture);
			}
			while (m_feedNames.ContainsKey(text2));
			feedName = text2;
		}
		m_feedNames.Add(feedName, 0);
		m_visitor.WriteCollection(dynamicElement.DefinitionPath, feedName, text);
		feedsWritten.Add(dynamicElement.DefinitionPath);
	}

	internal static string GetDefinitionPath(DynamicElement dynamicElement, string reportItemPath)
	{
		if (dynamicElement.IsRoot())
		{
			return GetDefinitionPath(dynamicElement.Children, reportItemPath);
		}
		string text = ((dynamicElement.ReportItemName == null) ? string.Empty : dynamicElement.ReportItemName);
		if (reportItemPath.StartsWith(text, StringComparison.Ordinal))
		{
			if (reportItemPath.Length == text.Length)
			{
				return dynamicElement.DefinitionPath;
			}
			if (reportItemPath[text.Length] == '.')
			{
				return GetDefinitionPath(dynamicElement.Children, reportItemPath.Substring(text.Length + 1));
			}
		}
		return null;
	}

	private static string GetDefinitionPath(List<DynamicElement> dynamicElements, string reportItemPath)
	{
		foreach (DynamicElement dynamicElement in dynamicElements)
		{
			string definitionPath = GetDefinitionPath(dynamicElement, reportItemPath);
			if (definitionPath != null)
			{
				return definitionPath;
			}
		}
		return null;
	}
}
