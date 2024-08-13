using System;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class XmlReportHandler
{
	private abstract class MapVectorLayerWalker
	{
		private IXmlVisitor m_Visitor;

		private Report.DataElementStyles m_dataElementStyle;

		private static string m_dataRowTag = "MapDataRow";

		private static string m_labelTag = "Label";

		protected abstract MapVectorLayer Layer { get; }

		protected abstract MapSpatialElementTemplate SpatialElementTemplate { get; }

		internal MapVectorLayerWalker(IXmlVisitor visitor, Report.DataElementStyles dataElementStyle)
		{
			m_Visitor = visitor;
			m_dataElementStyle = dataElementStyle;
		}

		internal void Walk(bool firstInstance)
		{
			if (Layer.DataElementOutput == DataElementOutputTypes.NoOutput)
			{
				return;
			}
			MapDataRegion mapDataRegion = Layer.MapDataRegion;
			if (mapDataRegion != null)
			{
				m_Visitor.StartMapLayer(m_Visitor.EncodeString(Layer.DataElementName), firstInstance);
				if (SpatialElementTemplate == null || SpatialElementTemplate.DataElementOutput != DataElementOutputTypes.NoOutput)
				{
					WalkGrouping(mapDataRegion.MapMember, ref firstInstance);
				}
				m_Visitor.EndElement(firstInstance);
			}
		}

		private void WalkGrouping(MapMember mapMember, ref bool firstInstance)
		{
			if (!mapMember.IsStatic)
			{
				MapDynamicMemberInstance mapDynamicMemberInstance = (MapDynamicMemberInstance)mapMember.Instance;
				mapDynamicMemberInstance.ResetContext();
				while (mapDynamicMemberInstance.MoveNext())
				{
					if (mapMember.ChildMapMember != null)
					{
						WalkGrouping(mapMember.ChildMapMember, ref firstInstance);
						continue;
					}
					WalkDataRow(mapMember, ref firstInstance);
					if (m_Visitor.Count(noRows: false) == RowCount.More)
					{
						continue;
					}
					break;
				}
			}
			else if (mapMember.ChildMapMember != null)
			{
				WalkGrouping(mapMember.ChildMapMember, ref firstInstance);
			}
			else
			{
				WalkDataRow(mapMember, ref firstInstance);
			}
		}

		private void WalkDataRow(MapMember mapMember, ref bool firstInstance)
		{
			string name = ((SpatialElementTemplate != null && !string.IsNullOrEmpty(SpatialElementTemplate.DataElementName)) ? m_Visitor.EncodeString(SpatialElementTemplate.DataElementName) : m_dataRowTag);
			m_Visitor.StartGroup(name, firstInstance);
			string val = EvaluateDataElementLabel();
			if (m_dataElementStyle == Report.DataElementStyles.Element)
			{
				m_Visitor.ValueElement(m_labelTag, val, TypeCode.Object, firstInstance);
			}
			else
			{
				m_Visitor.ValueAttribute(m_labelTag, val, TypeCode.Object, firstInstance);
			}
			WalkRules(firstInstance);
			m_Visitor.EndElement(firstInstance);
			firstInstance = false;
		}

		private object EvaluateRuleDataValue(MapAppearanceRule mapRule)
		{
			ReportVariantProperty dataValue = mapRule.DataValue;
			object result = null;
			if (dataValue != null)
			{
				result = (dataValue.IsExpression ? mapRule.Instance.DataValue : dataValue.Value);
			}
			return result;
		}

		private string EvaluateDataElementLabel()
		{
			if (SpatialElementTemplate == null)
			{
				return null;
			}
			ReportStringProperty dataElementLabel = SpatialElementTemplate.DataElementLabel;
			string result = null;
			if (dataElementLabel != null)
			{
				result = (dataElementLabel.IsExpression ? SpatialElementTemplate.Instance.DataElementLabel : dataElementLabel.Value);
			}
			return result;
		}

		protected void WalkRule(MapAppearanceRule mapRule, bool firstInstance, ref int index)
		{
			if (mapRule == null || mapRule.DataElementOutput == DataElementOutputTypes.NoOutput)
			{
				return;
			}
			object obj = EvaluateRuleDataValue(mapRule);
			if (obj == null)
			{
				return;
			}
			TypeCode typeCode = Type.GetTypeCode(obj.GetType());
			if (typeCode != TypeCode.String || !((string)obj).StartsWith("#", StringComparison.Ordinal))
			{
				string text = m_Visitor.EncodeString(mapRule.DataElementName);
				if (string.IsNullOrEmpty(text))
				{
					text = m_Visitor.getDefaultMapRuleValueTag(index);
					index++;
				}
				if (m_dataElementStyle == Report.DataElementStyles.Element)
				{
					m_Visitor.ValueElement(text, obj, TypeCode.Object, firstInstance);
				}
				else
				{
					m_Visitor.ValueAttribute(text, obj, TypeCode.Object, firstInstance);
				}
			}
		}

		protected abstract void WalkRules(bool firstInstance);
	}

	private class MapPolygonLayerWalker : MapVectorLayerWalker
	{
		private MapPolygonLayer m_layer;

		protected override MapVectorLayer Layer => m_layer;

		protected override MapSpatialElementTemplate SpatialElementTemplate => m_layer.MapPolygonTemplate;

		internal MapPolygonLayerWalker(MapPolygonLayer layer, IXmlVisitor visitor, Report.DataElementStyles dataElementStyle)
			: base(visitor, dataElementStyle)
		{
			m_layer = layer;
		}

		protected override void WalkRules(bool firstInstance)
		{
			int index = 0;
			MapPolygonRules mapPolygonRules = m_layer.MapPolygonRules;
			if (mapPolygonRules != null)
			{
				WalkRule(mapPolygonRules.MapColorRule, firstInstance, ref index);
			}
			MapPointRules mapCenterPointRules = m_layer.MapCenterPointRules;
			if (mapCenterPointRules != null)
			{
				WalkRule(mapCenterPointRules.MapColorRule, firstInstance, ref index);
				WalkRule(mapCenterPointRules.MapSizeRule, firstInstance, ref index);
				WalkRule(mapCenterPointRules.MapMarkerRule, firstInstance, ref index);
			}
		}
	}

	private class MapPointLayerWalker : MapVectorLayerWalker
	{
		private MapPointLayer m_layer;

		protected override MapVectorLayer Layer => m_layer;

		protected override MapSpatialElementTemplate SpatialElementTemplate => m_layer.MapPointTemplate;

		internal MapPointLayerWalker(MapPointLayer layer, IXmlVisitor visitor, Report.DataElementStyles dataElementStyle)
			: base(visitor, dataElementStyle)
		{
			m_layer = layer;
		}

		protected override void WalkRules(bool firstInstance)
		{
			MapPointRules mapPointRules = m_layer.MapPointRules;
			if (mapPointRules != null)
			{
				int index = 0;
				WalkRule(mapPointRules.MapColorRule, firstInstance, ref index);
				WalkRule(mapPointRules.MapSizeRule, firstInstance, ref index);
				WalkRule(mapPointRules.MapMarkerRule, firstInstance, ref index);
			}
		}
	}

	private class MapLineLayerWalker : MapVectorLayerWalker
	{
		private MapLineLayer m_layer;

		protected override MapVectorLayer Layer => m_layer;

		protected override MapSpatialElementTemplate SpatialElementTemplate => m_layer.MapLineTemplate;

		internal MapLineLayerWalker(MapLineLayer layer, IXmlVisitor visitor, Report.DataElementStyles dataElementStyle)
			: base(visitor, dataElementStyle)
		{
			m_layer = layer;
		}

		protected override void WalkRules(bool firstInstance)
		{
			MapLineRules mapLineRules = m_layer.MapLineRules;
			if (mapLineRules != null)
			{
				int index = 0;
				WalkRule(mapLineRules.MapColorRule, firstInstance, ref index);
				WalkRule(mapLineRules.MapSizeRule, firstInstance, ref index);
			}
		}
	}

	private IXmlVisitor m_Visitor;

	private bool m_UseFormattedValues;

	private Report m_RootReport;

	private Report m_CurrentReport;

	private TablixMember m_CurrentRow;

	private ChartMember m_CurrentSeries;

	private bool m_DoAttributesPass = true;

	public XmlReportHandler(Report aReport, IXmlVisitor aXmlVisitor, bool aUseFormattedValues, bool aDoAttributesPass)
	{
		m_RootReport = aReport;
		m_Visitor = aXmlVisitor;
		m_UseFormattedValues = aUseFormattedValues;
		m_DoAttributesPass = aDoAttributesPass;
	}

	public void ProcessReport()
	{
		WalkReport(m_RootReport, rootReport: true, firstInstance: true);
	}

	private void WalkReport(Report report, bool rootReport, bool firstInstance)
	{
		Report currentReport = m_CurrentReport;
		m_CurrentReport = report;
		string text = m_Visitor.EncodeString(report.DataElementName);
		if (string.IsNullOrEmpty(text))
		{
			text = m_Visitor.getDefaultReportTag();
		}
		if (rootReport)
		{
			m_Visitor.StartRootReport(text);
		}
		else
		{
			m_Visitor.StartReport(text, firstInstance);
		}
		m_Visitor.ValueAttribute(m_Visitor.getXmlRdlNameTag(), report.Name, TypeCode.String, firstInstance);
		if (report.ReportSections != null && report.ReportSections.Count > 0)
		{
			for (int i = 0; i < report.ReportSections.Count; i++)
			{
				WalkReportSection(report.ReportSections[i], i, firstInstance);
			}
		}
		m_Visitor.EndElement(firstInstance);
		m_CurrentReport = currentReport;
	}

	private void WalkReportSection(ReportSection section, int index, bool firstInstance)
	{
		if (section.Body == null || section.Body.ReportItemCollection == null)
		{
			return;
		}
		if (section.DataElementOutput == DataElementOutputTypes.ContentsOnly)
		{
			WalkTopLevelReportItemCollection(section.Body.ReportItemCollection, firstInstance);
		}
		else if (section.DataElementOutput == DataElementOutputTypes.Output)
		{
			string text = m_Visitor.EncodeString(section.DataElementName);
			if (string.IsNullOrEmpty(text))
			{
				text = m_Visitor.getDefaultSectionTag() + section.ID;
			}
			m_Visitor.StartReportSection(text, firstInstance);
			WalkTopLevelReportItemCollection(section.Body.ReportItemCollection, firstInstance);
			m_Visitor.EndElement(firstInstance);
		}
	}

	private void WalkRectangle(Rectangle rect, bool firstInstance)
	{
		string text = m_Visitor.EncodeString(rect.DataElementName);
		if (string.IsNullOrEmpty(text))
		{
			text = m_Visitor.getDefaultRectangleTag();
		}
		m_Visitor.StartRectangle(text, firstInstance);
		WalkTopLevelRectangleContents(rect, firstInstance);
		m_Visitor.EndElement(firstInstance);
	}

	private void WalkTopLevelRectangleContents(Rectangle rect, bool firstInstance)
	{
		if (WalkRectangleContents(rect, m_DoAttributesPass, firstInstance))
		{
			WalkRectangleContents(rect, attributesOnly: false, firstInstance);
		}
	}

	private bool WalkRectangleContents(Rectangle rect, bool attributesOnly, bool firstInstance)
	{
		ReportItemCollection reportItemCollection = rect.ReportItemCollection;
		if (reportItemCollection == null)
		{
			return false;
		}
		return WalkReportItemCollection(reportItemCollection, attributesOnly, firstInstance);
	}

	private void WalkTopLevelReportItemCollection(ReportItemCollection ric, bool firstInstance)
	{
		if (WalkReportItemCollection(ric, m_DoAttributesPass, firstInstance))
		{
			WalkReportItemCollection(ric, attributesOnly: false, firstInstance);
		}
	}

	private bool WalkReportItemCollection(ReportItemCollection ric, bool attributesOnly, bool firstInstance)
	{
		bool flag = false;
		int count = ric.Count;
		for (int i = 0; i < count; i++)
		{
			flag |= WalkReportItem(ric[i], attributesOnly, firstInstance);
		}
		return flag;
	}

	private void WalkTopLevelReportItem(ReportItem ri, bool firstInstance)
	{
		if (WalkReportItem(ri, m_DoAttributesPass, firstInstance))
		{
			WalkReportItem(ri, attributesOnly: false, firstInstance);
		}
	}

	private bool WalkReportItem(ReportItem ri, bool attributesOnly, bool firstInstance)
	{
		if (ri == null || ri.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			return false;
		}
		if (ri is TextBox)
		{
			return WalkTextBox((TextBox)ri, attributesOnly, firstInstance);
		}
		if (ri is Rectangle && ri.DataElementOutput == DataElementOutputTypes.ContentsOnly)
		{
			return WalkRectangleContents((Rectangle)ri, attributesOnly, firstInstance);
		}
		if (ri is Rectangle || ri is SubReport || ri is DataRegion || ri is Map)
		{
			if (attributesOnly)
			{
				return true;
			}
			if (ri is Rectangle)
			{
				WalkRectangle((Rectangle)ri, firstInstance);
			}
			else if (ri is Tablix)
			{
				WalkTablix((Tablix)ri, firstInstance);
			}
			else if (ri is Chart)
			{
				WalkChart((Chart)ri, firstInstance);
			}
			else if (ri is GaugePanel)
			{
				WalkGaugePanel((GaugePanel)ri, firstInstance);
			}
			else if (ri is Map)
			{
				WalkMap((Map)ri, firstInstance);
			}
			else if (ri is SubReport)
			{
				WalkSubReport((SubReport)ri, firstInstance);
			}
		}
		return false;
	}

	private bool WalkTextBox(TextBox tb, bool attributesOnly, bool firstInstance)
	{
		string text = m_Visitor.EncodeString(tb.DataElementName);
		if (string.IsNullOrEmpty(text))
		{
			text = m_Visitor.getDefaultTextboxTag();
		}
		bool flag = tb.DataElementStyle == Report.DataElementStyles.Element;
		if (flag == attributesOnly)
		{
			return attributesOnly;
		}
		object val = ((!m_UseFormattedValues) ? ((TextBoxInstance)tb.Instance).OriginalValue : ((TextBoxInstance)tb.Instance).Value);
		if (flag)
		{
			m_Visitor.ValueElement(text, val, tb.ID, firstInstance);
		}
		else
		{
			m_Visitor.ValueAttribute(text, val, tb.ID, firstInstance);
		}
		return false;
	}

	private void WalkTablix(Tablix aTablix, bool firstInstance)
	{
		TablixMember currentRow = m_CurrentRow;
		if (aTablix.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			string text = m_Visitor.EncodeString(aTablix.DataElementName);
			if (string.IsNullOrEmpty(text))
			{
				text = m_Visitor.getDefaultTablixTag();
			}
			m_Visitor.StartDataRegion(text, firstInstance, optional: false);
			bool flag = false;
			if (aTablix.Corner != null)
			{
				flag = WalkTablixCorner(aTablix.Corner, m_DoAttributesPass, firstInstance);
			}
			bool flag2 = false;
			if (aTablix.RowHierarchy != null && aTablix.RowHierarchy.MemberCollection != null)
			{
				flag2 = WalkTablixMemberCollection(aTablix.RowHierarchy.MemberCollection, aTablix, m_DoAttributesPass, firstInstance);
			}
			if (flag)
			{
				WalkTablixCorner(aTablix.Corner, aAttributesOnly: false, firstInstance);
			}
			if (flag2)
			{
				WalkTablixMemberCollection(aTablix.RowHierarchy.MemberCollection, aTablix, aAttributesOnly: false, firstInstance);
			}
			m_Visitor.EndElement(firstInstance);
			m_CurrentRow = currentRow;
		}
	}

	private void WalkTopLevelTablixCorner(TablixCorner tablixCorner, bool firstInstance)
	{
		if (WalkTablixCorner(tablixCorner, m_DoAttributesPass, firstInstance))
		{
			WalkTablixCorner(tablixCorner, aAttributesOnly: false, firstInstance);
		}
	}

	private bool WalkTablixCorner(TablixCorner aTablixCorner, bool aAttributesOnly, bool firstInstance)
	{
		bool flag = false;
		for (int i = 0; i < aTablixCorner.RowCollection.Count; i++)
		{
			TablixCornerRow tablixCornerRow = aTablixCorner.RowCollection[i];
			if (tablixCornerRow == null)
			{
				continue;
			}
			for (int j = 0; j < tablixCornerRow.Count; j++)
			{
				TablixCornerCell tablixCornerCell = tablixCornerRow[j];
				if (tablixCornerCell != null && tablixCornerCell.CellContents != null)
				{
					flag |= WalkReportItem(tablixCornerCell.CellContents.ReportItem, aAttributesOnly, firstInstance);
				}
			}
		}
		return flag;
	}

	private void WalkTopLevelTablixMemberCollection(TablixMemberCollection aMemberCollection, Tablix aTablix, bool firstInstance)
	{
		if (WalkTablixMemberCollection(aMemberCollection, aTablix, m_DoAttributesPass, firstInstance))
		{
			WalkTablixMemberCollection(aMemberCollection, aTablix, aAttributesOnly: false, firstInstance);
		}
	}

	private bool WalkTablixMemberCollection(TablixMemberCollection aMemberCollection, Tablix aTablix, bool aAttributesOnly, bool firstInstance)
	{
		bool flag = false;
		int aNonameMemberIndex = 0;
		for (int i = 0; i < aMemberCollection.Count; i++)
		{
			TablixMember tablixMember = aMemberCollection[i];
			if (tablixMember != null && !tablixMember.IsTotal)
			{
				flag = ((!tablixMember.IsStatic) ? (flag | WalkDynamicTablixMember(tablixMember, aTablix, aAttributesOnly, ref aNonameMemberIndex, firstInstance)) : (flag | WalkStaticTablixMember(tablixMember, aTablix, aAttributesOnly, ref aNonameMemberIndex, firstInstance)));
			}
		}
		return flag;
	}

	private bool WalkStaticTablixMember(TablixMember aTablixMember, Tablix aTablix, bool aAttributesOnly, ref int aNonameMemberIndex, bool firstInstance)
	{
		if (aTablixMember.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = aTablixMember.DataElementOutput == DataElementOutputTypes.Output;
		if (flag2)
		{
			if (aAttributesOnly)
			{
				return true;
			}
			string text = m_Visitor.EncodeString(aTablixMember.DataElementName);
			if (string.IsNullOrEmpty(text))
			{
				text = m_Visitor.getDefaultTablixStaticMemberTag(aTablixMember.IsColumn, aNonameMemberIndex++);
			}
			m_Visitor.StartTablixMember(text, isStatic: true, firstInstance);
		}
		if (aTablixMember.TablixHeader != null && aTablixMember.TablixHeader.CellContents != null)
		{
			if (flag2)
			{
				WalkTopLevelReportItem(aTablixMember.TablixHeader.CellContents.ReportItem, firstInstance);
			}
			else
			{
				flag |= WalkReportItem(aTablixMember.TablixHeader.CellContents.ReportItem, aAttributesOnly, firstInstance);
			}
		}
		if (aTablixMember.Children != null)
		{
			if (flag2)
			{
				WalkTopLevelTablixMemberCollection(aTablixMember.Children, aTablix, firstInstance);
			}
			else
			{
				flag |= WalkTablixMemberCollection(aTablixMember.Children, aTablix, aAttributesOnly, firstInstance);
			}
		}
		else if (!aTablixMember.IsColumn)
		{
			m_CurrentRow = aTablixMember;
			if (flag2)
			{
				WalkTopLevelTablixMemberCollection(aTablix.ColumnHierarchy.MemberCollection, aTablix, firstInstance);
			}
			else
			{
				flag |= WalkTablixMemberCollection(aTablix.ColumnHierarchy.MemberCollection, aTablix, aAttributesOnly, firstInstance);
			}
		}
		else
		{
			RSTrace.RenderingTracer.Assert(m_CurrentRow != null, "WalkStaticTablixMember -- CurrentRow == null");
			TablixCell tablixCell = aTablix.Body.RowCollection[m_CurrentRow.MemberCellIndex][aTablixMember.MemberCellIndex];
			if (tablixCell != null)
			{
				if (flag2)
				{
					WalkTopLevelTablixCell(tablixCell, aTablix, firstInstance);
				}
				else
				{
					flag |= WalkTablixCell(tablixCell, aTablix, aAttributesOnly, firstInstance);
				}
			}
		}
		if (flag2 && !aAttributesOnly)
		{
			m_Visitor.EndElement(firstInstance);
		}
		return flag;
	}

	private bool WalkDynamicTablixMember(TablixMember aTablixMember, Tablix aTablix, bool aAttributesOnly, ref int aNonameMemberIndex, bool aFirstInstance)
	{
		if (aTablixMember.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			return false;
		}
		RSTrace.RenderingTracer.Assert(aTablixMember.Group != null, "WalkDynamicTablixMember -- Group == null");
		TablixDynamicMemberInstance tablixDynamicMemberInstance = (TablixDynamicMemberInstance)aTablixMember.Instance;
		tablixDynamicMemberInstance.ResetContext();
		bool flag = tablixDynamicMemberInstance.MoveNext();
		TablixInstance tablixInstance = (TablixInstance)aTablix.Instance;
		RowCount rowCount = m_Visitor.Count(tablixInstance.NoRows);
		if (aAttributesOnly)
		{
			return true;
		}
		if (rowCount == RowCount.Zero && !(m_Visitor is XmlSchemaVisitor))
		{
			while (tablixDynamicMemberInstance.MoveNext())
			{
			}
			return false;
		}
		string text = m_Visitor.EncodeString(aTablixMember.DataElementName);
		if (string.IsNullOrEmpty(text))
		{
			text = m_Visitor.getDefaultGroupCollectionTag(aTablixMember.IsColumn, aNonameMemberIndex);
		}
		m_Visitor.StartCollection(text, aFirstInstance);
		bool flag2 = true;
		if (m_Visitor is XmlSchemaVisitor)
		{
			flag = true;
		}
		if (aTablixMember.Group.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			flag = false;
		}
		while (flag)
		{
			bool firstInstance = aFirstInstance && flag2;
			string text2 = m_Visitor.EncodeString(aTablixMember.Group.DataElementName);
			if (string.IsNullOrEmpty(text2))
			{
				text2 = m_Visitor.getDefaultGroupDetailTag();
			}
			m_Visitor.StartTablixMember(text2, isStatic: false, firstInstance);
			bool flag3 = false;
			if (aTablixMember.TablixHeader != null && aTablixMember.TablixHeader.CellContents != null)
			{
				flag3 = WalkReportItem(aTablixMember.TablixHeader.CellContents.ReportItem, m_DoAttributesPass, firstInstance);
			}
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			TablixCell tablixCell = null;
			if (aTablixMember.Children != null)
			{
				flag4 = WalkTablixMemberCollection(aTablixMember.Children, aTablix, m_DoAttributesPass, firstInstance);
			}
			else if (!aTablixMember.IsColumn)
			{
				m_CurrentRow = aTablixMember;
				flag5 = WalkTablixMemberCollection(aTablix.ColumnHierarchy.MemberCollection, aTablix, m_DoAttributesPass, firstInstance);
			}
			else
			{
				RSTrace.RenderingTracer.Assert(m_CurrentRow != null, "WalkStaticTablixMember -- CurrentRow == null");
				tablixCell = aTablix.Body.RowCollection[m_CurrentRow.MemberCellIndex][aTablixMember.MemberCellIndex];
				if (tablixCell != null)
				{
					flag6 = WalkTablixCell(tablixCell, aTablix, aAttributesOnly: true, firstInstance);
				}
			}
			if (flag3)
			{
				WalkReportItem(aTablixMember.TablixHeader.CellContents.ReportItem, attributesOnly: false, firstInstance);
			}
			if (flag4)
			{
				WalkTablixMemberCollection(aTablixMember.Children, aTablix, aAttributesOnly: false, firstInstance);
			}
			if (flag5)
			{
				WalkTablixMemberCollection(aTablix.ColumnHierarchy.MemberCollection, aTablix, aAttributesOnly: false, firstInstance);
			}
			if (flag6)
			{
				WalkTablixCell(tablixCell, aTablix, aAttributesOnly: false, firstInstance);
			}
			m_Visitor.EndElement(firstInstance);
			flag2 = false;
			flag = rowCount == RowCount.More && tablixDynamicMemberInstance.MoveNext();
		}
		m_Visitor.EndElement(aFirstInstance);
		return false;
	}

	private void WalkTopLevelTablixCell(TablixCell aCell, Tablix aTablix, bool firstInstance)
	{
		if (WalkTablixCell(aCell, aTablix, m_DoAttributesPass, firstInstance))
		{
			WalkTablixCell(aCell, aTablix, aAttributesOnly: false, firstInstance);
		}
	}

	private bool WalkTablixCell(TablixCell aCell, Tablix aTablix, bool aAttributesOnly, bool firstInstance)
	{
		if (aCell == null || aCell.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			return false;
		}
		bool flag = aCell.DataElementOutput == DataElementOutputTypes.Output;
		if (flag && aAttributesOnly)
		{
			return true;
		}
		bool result = false;
		if (flag)
		{
			string text = m_Visitor.EncodeString(aCell.DataElementName);
			if (string.IsNullOrEmpty(text))
			{
				text = m_Visitor.getDefaultCellTag(aCell.ID);
			}
			m_Visitor.StartCell(text, firstInstance);
			if (aCell.CellContents != null)
			{
				WalkTopLevelReportItem(aCell.CellContents.ReportItem, firstInstance);
			}
			m_Visitor.EndElement(firstInstance);
		}
		else if (aCell.CellContents != null)
		{
			result = WalkReportItem(aCell.CellContents.ReportItem, aAttributesOnly, firstInstance);
		}
		return result;
	}

	private void WalkChart(Chart chart, bool firstInstance)
	{
		if (chart.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			string text = m_Visitor.EncodeString(chart.DataElementName);
			if (string.IsNullOrEmpty(text))
			{
				text = m_Visitor.getDefaultChartTag();
			}
			m_Visitor.StartDataRegion(text, firstInstance, optional: false);
			if (chart.SeriesHierarchy != null)
			{
				WalkChartMemberCollection(chart.SeriesHierarchy.MemberCollection, chart, firstInstance);
			}
			m_Visitor.EndElement(firstInstance);
		}
	}

	private void WalkChartMemberCollection(ChartMemberCollection chartMemberCollection, Chart chart, bool firstInstance)
	{
		if (chartMemberCollection == null)
		{
			return;
		}
		int noNameMemberIndex = 0;
		for (int i = 0; i < chartMemberCollection.Count; i++)
		{
			ChartMember chartMember = chartMemberCollection[i];
			if (chartMember != null)
			{
				WalkChartMember(chartMember, chart, ref noNameMemberIndex, firstInstance);
			}
		}
	}

	private void WalkChartMember(ChartMember chartMember, Chart chart, ref int noNameMemberIndex, bool firstInstance)
	{
		if (chartMember.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			return;
		}
		ChartDynamicMemberInstance chartDynamicMemberInstance = null;
		bool flag = true;
		RowCount rowCount = RowCount.One;
		if (!chartMember.IsStatic)
		{
			chartDynamicMemberInstance = (ChartDynamicMemberInstance)chartMember.Instance;
			chartDynamicMemberInstance.ResetContext();
			flag = chartDynamicMemberInstance.MoveNext();
			ChartInstance chartInstance = (ChartInstance)chart.Instance;
			rowCount = m_Visitor.Count(chartInstance.NoRows);
			if (rowCount == RowCount.Zero)
			{
				return;
			}
		}
		string text = m_Visitor.EncodeString(chartMember.DataElementName);
		if (string.IsNullOrEmpty(text) && chartMember.IsStatic && chartMember.Label != null)
		{
			text = m_Visitor.EncodeString(chartMember.Label.Value);
		}
		if (string.IsNullOrEmpty(text))
		{
			text = m_Visitor.getDefaultChartMemberTag(chartMember.IsCategory, noNameMemberIndex++);
		}
		m_Visitor.StartChartMember(text, isStatic: true, firstInstance);
		int num = 0;
		bool flag2 = true;
		bool firstInstance2 = firstInstance;
		while (flag)
		{
			if (!chartMember.IsStatic)
			{
				string text2 = m_Visitor.EncodeString(chartMember.Group.DataElementName);
				if (string.IsNullOrEmpty(text2))
				{
					if (chartMember.Group.GroupExpressions.Count > 0)
					{
						object value = chartMember.Group.GroupExpressions[0].Value;
						if (value != null)
						{
							text2 = m_Visitor.EncodeString(chartMember.Group.GroupExpressions[0].Value.ToString());
						}
					}
					if (string.IsNullOrEmpty(text2))
					{
						text2 = m_Visitor.getDefaultChartCollectionTag(chartMember.IsCategory, num++);
					}
				}
				firstInstance2 = firstInstance && flag2;
				m_Visitor.StartGroup(text2, firstInstance2);
			}
			if (chartMember.Children != null)
			{
				WalkTopLevelChartMemberDetails(chartMember, firstInstance2);
				WalkChartMemberCollection(chartMember.Children, chart, firstInstance2);
			}
			else if (!chartMember.IsCategory)
			{
				m_CurrentSeries = chartMember;
				WalkTopLevelChartMemberDetails(chartMember, firstInstance2);
				WalkChartMemberCollection(chart.CategoryHierarchy.MemberCollection, chart, firstInstance2);
			}
			else
			{
				WalkChartCategoryDetails(chartMember, chart, firstInstance2);
			}
			if (chartMember.IsStatic)
			{
				flag = false;
			}
			else
			{
				m_Visitor.EndElement(firstInstance2);
				flag = rowCount == RowCount.More && chartDynamicMemberInstance.MoveNext();
			}
			flag2 = false;
		}
		m_Visitor.EndElement(firstInstance);
	}

	private void WalkTopLevelChartMemberDetails(ChartMember chartMember, bool firstInstance)
	{
		if (WalkChartMemberDetails(chartMember, m_DoAttributesPass, firstInstance))
		{
			WalkChartMemberDetails(chartMember, attributesOnly: false, firstInstance);
		}
	}

	private bool WalkChartMemberDetails(ChartMember chartMember, bool attributesOnly, bool firstInstance)
	{
		bool flag = m_CurrentReport.DataElementStyle == Report.DataElementStyles.Element;
		if (flag == attributesOnly)
		{
			return attributesOnly;
		}
		ChartMemberInstance instance = chartMember.Instance;
		string chartMemberLabelTag = m_Visitor.getChartMemberLabelTag();
		object label = instance.Label;
		if (flag)
		{
			m_Visitor.ValueElement(chartMemberLabelTag, label, TypeCode.Object, firstInstance);
		}
		else
		{
			m_Visitor.ValueAttribute(chartMemberLabelTag, label, TypeCode.Object, firstInstance);
		}
		return false;
	}

	private void WalkChartCategoryDetails(ChartMember chartMember, Chart chart, bool firstInstance)
	{
		RSTrace.RenderingTracer.Assert(m_CurrentSeries != null, "WalkChartCategoryDetails -- m_CurrentSeries == null");
		ChartDataPoint chartDataPoint = chart.ChartData.SeriesCollection[m_CurrentSeries.MemberCellIndex][chartMember.MemberCellIndex];
		if (chartDataPoint == null)
		{
			return;
		}
		ChartSeriesType chartSeriesType = ChartSeriesType.Column;
		ChartSeriesSubtype subtype = ChartSeriesSubtype.Plain;
		if (chart.DataValueSequenceRendering)
		{
			ReportEnumProperty<ChartSeriesType> type = chart.ChartData.SeriesCollection[m_CurrentSeries.MemberCellIndex].Type;
			ReportEnumProperty<ChartSeriesSubtype> subtype2 = chart.ChartData.SeriesCollection[m_CurrentSeries.MemberCellIndex].Subtype;
			RSTrace.RenderingTracer.Assert(type != null, "WalkChartCategoryDetails: for an old report the chart series type is null");
			chartSeriesType = type.Value;
			if (chartSeriesType == ChartSeriesType.Range)
			{
				RSTrace.RenderingTracer.Assert(subtype2 != null, "WalkChartCategoryDetails: for an old report the chart series subtype is null");
				subtype = subtype2.Value;
			}
		}
		switch (chartDataPoint.DataElementOutput)
		{
		case DataElementOutputTypes.NoOutput:
			WalkTopLevelChartMemberDetails(chartMember, firstInstance);
			break;
		case DataElementOutputTypes.Output:
		{
			WalkTopLevelChartMemberDetails(chartMember, firstInstance);
			string text = m_Visitor.EncodeString(chartDataPoint.DataElementName);
			if (string.IsNullOrEmpty(text))
			{
				text = m_Visitor.getDefaultChartDataPointTag();
			}
			m_Visitor.StartChartDataPoint(text, firstInstance);
			WalkTopLevelChartDataPoint(chartDataPoint, chartSeriesType, subtype, chart.DataValueSequenceRendering, firstInstance);
			m_Visitor.EndElement(firstInstance);
			break;
		}
		case DataElementOutputTypes.ContentsOnly:
		{
			bool flag = WalkChartMemberDetails(chartMember, m_DoAttributesPass, firstInstance);
			bool flag2 = WalkChartDataPoint(chartDataPoint, chartSeriesType, subtype, chart.DataValueSequenceRendering, m_DoAttributesPass, firstInstance);
			if (flag)
			{
				WalkChartMemberDetails(chartMember, attributesOnly: false, firstInstance);
			}
			if (flag2)
			{
				WalkChartDataPoint(chartDataPoint, chartSeriesType, subtype, chart.DataValueSequenceRendering, attributesOnly: false, firstInstance);
			}
			break;
		}
		}
	}

	private void WalkTopLevelChartDataPoint(ChartDataPoint dataPoint, ChartSeriesType type, ChartSeriesSubtype subtype, bool oldDataPointsGeneration, bool firstInstance)
	{
		if (WalkChartDataPoint(dataPoint, type, subtype, oldDataPointsGeneration, m_DoAttributesPass, firstInstance))
		{
			WalkChartDataPoint(dataPoint, type, subtype, oldDataPointsGeneration, attributesOnly: false, firstInstance);
		}
	}

	private bool WalkChartDataPoint(ChartDataPoint dataPoint, ChartSeriesType type, ChartSeriesSubtype subtype, bool oldDataPointsGeneration, bool attributesOnly, bool firstInstance)
	{
		RSTrace.RenderingTracer.Assert(dataPoint != null, "WalkChartDataPoint: Null data point");
		bool flag = m_CurrentReport.DataElementStyle == Report.DataElementStyles.Element;
		if (flag == attributesOnly)
		{
			return attributesOnly;
		}
		ChartDataPointValues dataPointValues = dataPoint.DataPointValues;
		if (dataPointValues != null)
		{
			ChartDataPointValuesInstance instance = dataPoint.DataPointValues.Instance;
			bool hasStart = false;
			if (dataPointValues.X != null)
			{
				if (oldDataPointsGeneration)
				{
					int aIndex = DataValueMappingTable("X", type, subtype, hasStart);
					WalkChartDataPointValue(m_Visitor.getChartDataValueTag(aIndex), instance.X, flag, firstInstance);
				}
				else
				{
					WalkChartDataPointValue("X", instance.X, flag, firstInstance);
				}
			}
			if (dataPointValues.Y != null)
			{
				if (oldDataPointsGeneration)
				{
					int aIndex2 = DataValueMappingTable("Y", type, subtype, hasStart);
					WalkChartDataPointValue(m_Visitor.getChartDataValueTag(aIndex2), instance.Y, flag, firstInstance);
				}
				else
				{
					WalkChartDataPointValue("Y", instance.Y, flag, firstInstance);
				}
			}
			if (dataPointValues.Size != null)
			{
				if (oldDataPointsGeneration)
				{
					int aIndex3 = DataValueMappingTable("Size", type, subtype, hasStart);
					WalkChartDataPointValue(m_Visitor.getChartDataValueTag(aIndex3), instance.Size, flag, firstInstance);
				}
				else
				{
					WalkChartDataPointValue("Size", instance.Size, flag, firstInstance);
				}
			}
			if (dataPointValues.High != null)
			{
				if (oldDataPointsGeneration)
				{
					int aIndex4 = DataValueMappingTable("High", type, subtype, hasStart);
					WalkChartDataPointValue(m_Visitor.getChartDataValueTag(aIndex4), instance.High, flag, firstInstance);
				}
				else
				{
					WalkChartDataPointValue("High", instance.High, flag, firstInstance);
				}
			}
			if (dataPointValues.Low != null)
			{
				if (oldDataPointsGeneration)
				{
					int aIndex5 = DataValueMappingTable("Low", type, subtype, hasStart);
					WalkChartDataPointValue(m_Visitor.getChartDataValueTag(aIndex5), instance.Low, flag, firstInstance);
				}
				else
				{
					WalkChartDataPointValue("Low", instance.Low, flag, firstInstance);
				}
			}
			if (dataPointValues.Start != null)
			{
				hasStart = true;
				if (oldDataPointsGeneration)
				{
					int aIndex6 = DataValueMappingTable("Start", type, subtype, hasStart);
					WalkChartDataPointValue(m_Visitor.getChartDataValueTag(aIndex6), instance.Start, flag, firstInstance);
				}
				else
				{
					WalkChartDataPointValue("Start", instance.Start, flag, firstInstance);
				}
			}
			if (dataPointValues.End != null)
			{
				if (oldDataPointsGeneration)
				{
					int aIndex7 = DataValueMappingTable("End", type, subtype, hasStart);
					WalkChartDataPointValue(m_Visitor.getChartDataValueTag(aIndex7), instance.End, flag, firstInstance);
				}
				else
				{
					WalkChartDataPointValue("End", instance.End, flag, firstInstance);
				}
			}
			if (dataPointValues.Mean != null)
			{
				if (oldDataPointsGeneration)
				{
					int aIndex8 = DataValueMappingTable("Mean", type, subtype, hasStart);
					WalkChartDataPointValue(m_Visitor.getChartDataValueTag(aIndex8), instance.Mean, flag, firstInstance);
				}
				else
				{
					WalkChartDataPointValue("Mean", instance.Mean, flag, firstInstance);
				}
			}
			if (dataPointValues.Median != null)
			{
				if (oldDataPointsGeneration)
				{
					int aIndex9 = DataValueMappingTable("Median", type, subtype, hasStart);
					WalkChartDataPointValue(m_Visitor.getChartDataValueTag(aIndex9), instance.Median, flag, firstInstance);
				}
				else
				{
					WalkChartDataPointValue("Median", instance.Median, flag, firstInstance);
				}
			}
		}
		return false;
	}

	private void WalkChartDataPointValue(string name, object value, bool renderAsElement, bool firstInstance)
	{
		if (renderAsElement)
		{
			m_Visitor.ValueElement(name, value, TypeCode.Object, firstInstance);
		}
		else
		{
			m_Visitor.ValueAttribute(name, value, TypeCode.Object, firstInstance);
		}
	}

	private int DataValueMappingTable(string name, ChartSeriesType type, ChartSeriesSubtype subtype, bool hasStart)
	{
		RSTrace.RenderingTracer.Assert(name != null, "ChartDataPoint element name cannot be null.");
		int result = -1;
		switch (name)
		{
		case "X":
			if (type == ChartSeriesType.Scatter || subtype == ChartSeriesSubtype.Bubble)
			{
				result = 0;
			}
			break;
		case "Y":
			if (type == ChartSeriesType.Column || type == ChartSeriesType.Bar || type == ChartSeriesType.Line || type == ChartSeriesType.Area || type == ChartSeriesType.Shape)
			{
				result = 0;
			}
			else if (type == ChartSeriesType.Scatter || subtype == ChartSeriesSubtype.Bubble)
			{
				result = 1;
			}
			break;
		case "Size":
			if (subtype == ChartSeriesSubtype.Bubble)
			{
				result = 2;
			}
			break;
		case "High":
			if (subtype == ChartSeriesSubtype.Stock || subtype == ChartSeriesSubtype.Candlestick)
			{
				result = 0;
			}
			break;
		case "Low":
			if (subtype == ChartSeriesSubtype.Stock || subtype == ChartSeriesSubtype.Candlestick)
			{
				result = 1;
			}
			break;
		case "Start":
			if (subtype == ChartSeriesSubtype.Stock || subtype == ChartSeriesSubtype.Candlestick)
			{
				result = 2;
			}
			break;
		case "End":
			if (subtype == ChartSeriesSubtype.Stock || subtype == ChartSeriesSubtype.Candlestick)
			{
				result = ((!hasStart) ? 2 : 3);
			}
			break;
		}
		return result;
	}

	private void WalkGaugePanel(GaugePanel gaugePanel, bool firstInstance)
	{
		if (gaugePanel.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			return;
		}
		string text = m_Visitor.EncodeString(gaugePanel.DataElementName);
		if (string.IsNullOrEmpty(text))
		{
			text = m_Visitor.getDefaultGaugePanelTag();
		}
		m_Visitor.StartDataRegion(text, firstInstance, firstInstance);
		int num = 0;
		if (gaugePanel.LinearGauges != null)
		{
			for (int i = 0; i < gaugePanel.LinearGauges.Count; i++)
			{
				WalkGauge(gaugePanel.LinearGauges[i], num++, firstInstance);
			}
		}
		if (gaugePanel.RadialGauges != null)
		{
			for (int j = 0; j < gaugePanel.RadialGauges.Count; j++)
			{
				WalkGauge(gaugePanel.RadialGauges[j], num++, firstInstance);
			}
		}
		int num2 = 0;
		if (gaugePanel.StateIndicators != null)
		{
			for (int k = 0; k < gaugePanel.StateIndicators.Count; k++)
			{
				WalkStateIndicator(gaugePanel.StateIndicators[k], num2++, firstInstance);
			}
		}
		m_Visitor.EndElement(firstInstance);
	}

	private void WalkGauge(Gauge gauge, int gaugeIndex, bool firstInstance)
	{
		if (gauge != null)
		{
			m_Visitor.StartGroup(m_Visitor.getGaugeTag(gaugeIndex), firstInstance);
			if (gauge is LinearGauge)
			{
				WalkLinearScaleCollection((gauge as LinearGauge).GaugeScales, firstInstance);
			}
			else
			{
				WalkRadialScaleCollection((gauge as RadialGauge).GaugeScales, firstInstance);
			}
			m_Visitor.EndElement(firstInstance);
		}
	}

	private void WalkStateIndicator(StateIndicator stateIndicator, int stateIndicatorIndex, bool firstInstance)
	{
		m_Visitor.StartGroup(m_Visitor.getStateIndicatorTag(stateIndicatorIndex), firstInstance);
		WalkStateIndicatorName(m_Visitor.getDefaultStateNameTag(), stateIndicator, firstInstance);
		WalkGaugeInputValue(m_Visitor.getDefaultGaugeInputValueTag(), stateIndicator.GaugeInputValue, firstInstance);
		if (stateIndicator.IndicatorStates != null)
		{
			WalkIndicatorStateCollection(stateIndicator.IndicatorStates, firstInstance);
		}
		m_Visitor.EndElement(firstInstance);
	}

	private void WalkIndicatorStateCollection(IndicatorStateCollection indicatorStateCollection, bool firstInstance)
	{
		m_Visitor.StartCollection(m_Visitor.getIndicatorStateCollectionTag(), firstInstance);
		for (int i = 0; i < indicatorStateCollection.Count; i++)
		{
			WalkIndicatorState(indicatorStateCollection[i], i, firstInstance);
		}
		m_Visitor.EndElement(firstInstance);
	}

	private void WalkIndicatorState(IndicatorState indicatorState, int indicatorStateIndex, bool firstInstance)
	{
		m_Visitor.StartGroup(m_Visitor.getIndicatorStateTag(indicatorStateIndex), firstInstance);
		WalkGaugeInputValue(m_Visitor.getDefaultGaugeStartValueTag(), indicatorState.StartValue, firstInstance);
		WalkGaugeInputValue(m_Visitor.getDefaultGaugeEndValueTag(), indicatorState.EndValue, firstInstance);
		m_Visitor.EndElement(firstInstance);
	}

	private void WalkLinearScaleCollection(LinearScaleCollection linearScaleCollection, bool firstInstance)
	{
		if (linearScaleCollection != null)
		{
			m_Visitor.StartCollection(m_Visitor.getGaugeScaleCollectionTag(), firstInstance);
			for (int i = 0; i < linearScaleCollection.Count; i++)
			{
				WalkGaugeScale(linearScaleCollection[i], i, firstInstance);
			}
			m_Visitor.EndElement(firstInstance);
		}
	}

	private void WalkRadialScaleCollection(RadialScaleCollection radialScaleCollection, bool firstInstance)
	{
		if (radialScaleCollection != null)
		{
			m_Visitor.StartCollection(m_Visitor.getGaugeScaleCollectionTag(), firstInstance);
			for (int i = 0; i < radialScaleCollection.Count; i++)
			{
				WalkGaugeScale(radialScaleCollection[i], i, firstInstance);
			}
			m_Visitor.EndElement(firstInstance);
		}
	}

	private void WalkGaugeScale(GaugeScale gaugeScale, int gaugeScaleIndex, bool firstInstance)
	{
		if (gaugeScale != null)
		{
			m_Visitor.StartGroup(m_Visitor.getGaugeScaleTag(gaugeScaleIndex), firstInstance);
			WalkGaugeInputValue(m_Visitor.getDefaultGaugeMinimumValueTag(), gaugeScale.MinimumValue, firstInstance);
			WalkGaugeInputValue(m_Visitor.getDefaultGaugeMaximumValueTag(), gaugeScale.MaximumValue, firstInstance);
			if (gaugeScale is LinearScale)
			{
				WalkLinearPointerCollection((gaugeScale as LinearScale).GaugePointers, firstInstance);
			}
			else
			{
				WalkRadialPointerCollection((gaugeScale as RadialScale).GaugePointers, firstInstance);
			}
			WalkGaugeScaleRangeCollection(gaugeScale.ScaleRanges, firstInstance);
			m_Visitor.EndElement(firstInstance);
		}
	}

	private void WalkLinearPointerCollection(LinearPointerCollection linearPointerCollection, bool firstInstance)
	{
		if (linearPointerCollection != null)
		{
			m_Visitor.StartCollection(m_Visitor.getGaugePointerCollectionTag(), firstInstance);
			for (int i = 0; i < linearPointerCollection.Count; i++)
			{
				WalkGaugePointer(linearPointerCollection[i], i, firstInstance);
			}
			m_Visitor.EndElement(firstInstance);
		}
	}

	private void WalkRadialPointerCollection(RadialPointerCollection radialPointerCollection, bool firstInstance)
	{
		if (radialPointerCollection != null)
		{
			m_Visitor.StartCollection(m_Visitor.getGaugePointerCollectionTag(), firstInstance);
			for (int i = 0; i < radialPointerCollection.Count; i++)
			{
				WalkGaugePointer(radialPointerCollection[i], i, firstInstance);
			}
			m_Visitor.EndElement(firstInstance);
		}
	}

	private void WalkGaugePointer(GaugePointer gaugePointer, int gaugePointerIndex, bool firstInstance)
	{
		if (gaugePointer == null)
		{
			return;
		}
		if (gaugePointer.CompiledInstances != null)
		{
			string gaugePointerTag = m_Visitor.getGaugePointerTag(gaugePointerIndex);
			string defaultGaugeInputValueTag = m_Visitor.getDefaultGaugeInputValueTag();
			for (int i = 0; i < gaugePointer.CompiledInstances.Length; i++)
			{
				m_Visitor.StartGroup(gaugePointerTag, firstInstance && i == 0);
				RSTrace.RenderingTracer.Assert(gaugePointer.CompiledInstances[i].GaugeInputValue != null, "GaugeInputValue Instance cannot be null.");
				WalkGaugeInputValue(defaultGaugeInputValueTag, gaugePointer.GaugeInputValue, firstInstance, gaugePointer.CompiledInstances[i].GaugeInputValue.Value);
				m_Visitor.EndElement(firstInstance && i == 0);
			}
		}
		else
		{
			m_Visitor.StartGroup(m_Visitor.getGaugePointerTag(gaugePointerIndex), firstInstance);
			WalkGaugeInputValue(m_Visitor.getDefaultGaugeInputValueTag(), gaugePointer.GaugeInputValue, firstInstance);
			m_Visitor.EndElement(firstInstance);
		}
	}

	private void WalkGaugeScaleRangeCollection(ScaleRangeCollection scaleRangeCollection, bool firstInstance)
	{
		if (scaleRangeCollection != null)
		{
			m_Visitor.StartCollection(m_Visitor.getGaugeScaleRangeCollectionTag(), firstInstance);
			for (int i = 0; i < scaleRangeCollection.Count; i++)
			{
				WalkGaugeScaleRange(scaleRangeCollection[i], i, firstInstance);
			}
			m_Visitor.EndElement(firstInstance);
		}
	}

	private void WalkGaugeScaleRange(ScaleRange scaleRange, int scaleRangeIndex, bool firstInstance)
	{
		if (scaleRange != null)
		{
			m_Visitor.StartGroup(m_Visitor.getGaugeScaleRangeTag(scaleRangeIndex), firstInstance);
			WalkGaugeInputValue(m_Visitor.getDefaultGaugeStartValueTag(), scaleRange.StartValue, firstInstance);
			WalkGaugeInputValue(m_Visitor.getDefaultGaugeEndValueTag(), scaleRange.EndValue, firstInstance);
			m_Visitor.EndElement(firstInstance);
		}
	}

	private void WalkStateIndicatorName(string defaultTag, StateIndicator stateIndicator, bool firstInstance)
	{
		string compiledStateName = stateIndicator.CompiledStateName;
		if (stateIndicator.StateDataElementOutput != DataElementOutputTypes.NoOutput && compiledStateName != null)
		{
			RSTrace.RenderingTracer.Assert(stateIndicator.StateDataElementOutput == DataElementOutputTypes.Output, "stateIndicator.StateDataElementOutput = " + stateIndicator.StateDataElementOutput.ToString() + " is not supported");
			string text = m_Visitor.EncodeString(stateIndicator.StateDataElementName);
			if (string.IsNullOrEmpty(text))
			{
				text = defaultTag;
			}
			if (m_CurrentReport.DataElementStyle == Report.DataElementStyles.Element)
			{
				m_Visitor.ValueElement(text, compiledStateName, TypeCode.Object, firstInstance);
			}
			else
			{
				m_Visitor.ValueAttribute(text, compiledStateName, TypeCode.Object, firstInstance);
			}
		}
	}

	private void WalkGaugeInputValue(string defaultTag, GaugeInputValue gaugeInputValue, bool firstInstance)
	{
		if (gaugeInputValue != null)
		{
			object value;
			if (gaugeInputValue.CompiledInstance != null)
			{
				value = gaugeInputValue.CompiledInstance.Value;
			}
			else
			{
				RSTrace.RenderingTracer.Assert(gaugeInputValue.Instance != null, "GaugeInputValue.Instance cannot be null.");
				value = gaugeInputValue.Instance.Value;
			}
			WalkGaugeInputValue(defaultTag, gaugeInputValue, firstInstance, value);
		}
	}

	private void WalkGaugeInputValue(string defaultTag, GaugeInputValue gaugeInputValue, bool firstInstance, object value)
	{
		if (gaugeInputValue != null && gaugeInputValue.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			RSTrace.RenderingTracer.Assert(gaugeInputValue.DataElementOutput == DataElementOutputTypes.Output, "GaugeInputValue.DataElementOutput = " + gaugeInputValue.DataElementOutput.ToString() + " is not supported");
			string text = m_Visitor.EncodeString(gaugeInputValue.DataElementName);
			if (string.IsNullOrEmpty(text))
			{
				text = defaultTag;
			}
			if (m_CurrentReport.DataElementStyle == Report.DataElementStyles.Element)
			{
				m_Visitor.ValueElement(text, value, TypeCode.Object, firstInstance);
			}
			else
			{
				m_Visitor.ValueAttribute(text, value, TypeCode.Object, firstInstance);
			}
		}
	}

	private void WalkMap(Map map, bool firstInstance)
	{
		if (map.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			return;
		}
		m_Visitor.StartDataRegion(m_Visitor.EncodeString(map.DataElementName), firstInstance, optional: false);
		if (map.MapLayers == null)
		{
			return;
		}
		foreach (MapLayer mapLayer in map.MapLayers)
		{
			if (mapLayer is MapPolygonLayer)
			{
				new MapPolygonLayerWalker((MapPolygonLayer)mapLayer, m_Visitor, m_CurrentReport.DataElementStyle).Walk(firstInstance);
			}
			else if (mapLayer is MapPointLayer)
			{
				new MapPointLayerWalker((MapPointLayer)mapLayer, m_Visitor, m_CurrentReport.DataElementStyle).Walk(firstInstance);
			}
			else if (mapLayer is MapLineLayer)
			{
				new MapLineLayerWalker((MapLineLayer)mapLayer, m_Visitor, m_CurrentReport.DataElementStyle).Walk(firstInstance);
			}
		}
		m_Visitor.EndElement(firstInstance);
	}

	private void WalkSubReport(SubReport sr, bool firstInstance)
	{
		string text = m_Visitor.EncodeString(sr.DataElementName);
		if (string.IsNullOrEmpty(text))
		{
			text = m_Visitor.getDefaultSubreportTag();
		}
		m_Visitor.StartDataRegion(text, firstInstance, optional: false);
		SubReportInstance subReportInstance = (SubReportInstance)sr.Instance;
		if (subReportInstance.ProcessedWithError)
		{
			throw new ReportRenderingException(subReportInstance.ErrorMessage);
		}
		if (sr.Report != null)
		{
			WalkReport(sr.Report, rootReport: false, firstInstance);
		}
		m_Visitor.EndElement(firstInstance);
	}

	private bool getOutputForStaticMember(TablixMember aTablixMember, Tablix aTablix)
	{
		bool flag = false;
		if (aTablixMember.Children != null)
		{
			for (int i = 0; i < aTablixMember.Children.Count; i++)
			{
				flag = ((!aTablixMember.Children[i].IsStatic) ? (flag | (aTablixMember.Children[i].Group.DataElementOutput == DataElementOutputTypes.Output)) : (flag | getOutputForStaticMember(aTablixMember.Children[i], aTablix)));
			}
		}
		else if (aTablixMember.IsColumn)
		{
			for (int j = 0; j < aTablix.Body.RowCollection.Count; j++)
			{
				flag |= getOutputForTablixCell(aTablix.Body.RowCollection[j][aTablixMember.MemberCellIndex]);
			}
		}
		else
		{
			for (int k = 0; k < aTablix.Body.ColumnCollection.Count; k++)
			{
				flag |= getOutputForTablixCell(aTablix.Body.RowCollection[aTablixMember.MemberCellIndex][k]);
			}
		}
		return flag;
	}

	private bool getOutputForTablixCell(TablixCell aTablixCell)
	{
		if (aTablixCell == null)
		{
			return false;
		}
		if (aTablixCell.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			return false;
		}
		if (aTablixCell.DataElementOutput == DataElementOutputTypes.Output)
		{
			return true;
		}
		if (aTablixCell.CellContents == null || aTablixCell.CellContents.ReportItem == null)
		{
			return false;
		}
		return aTablixCell.CellContents.ReportItem.DataElementOutput == DataElementOutputTypes.Output;
	}
}
