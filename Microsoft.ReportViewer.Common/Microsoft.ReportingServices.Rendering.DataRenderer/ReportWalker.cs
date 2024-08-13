using System.Collections;
using System.Collections.Generic;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class ReportWalker
{
	internal class ChartWalker
	{
		private IChartHandler m_chartHandler;

		private Chart m_chart;

		private ReportWalker m_reportWalker;

		private ChartMember m_currentSeries;

		private bool m_currentSeriesOutputState;

		private bool m_chartOutputMembers;

		private bool m_chartCategoryMemberLabelsOutput;

		private MemberState m_chartState;

		internal ChartWalker(Chart chart, ReportWalker reportWalker)
		{
			m_chart = chart;
			m_reportWalker = reportWalker;
			m_chartHandler = m_reportWalker.ChartHandler;
			m_chartCategoryMemberLabelsOutput = false;
		}

		internal void WalkChart(MemberState chartState, bool outputMembers)
		{
			if (m_chart == null || m_reportWalker == null || m_reportWalker.ChartHandler == null)
			{
				return;
			}
			m_chartOutputMembers = outputMembers;
			m_chartState = chartState;
			bool walkChildren = false;
			if (m_reportWalker.AtomRendererWalk)
			{
				walkChildren = ((!m_reportWalker.FirstInstancePass) ? (chartState.IsDynamic ? walkChildren : (m_chart.DataElementOutput != DataElementOutputTypes.NoOutput)) : (m_chart.DataElementOutput != DataElementOutputTypes.NoOutput));
			}
			m_chartHandler.OnChartBegin(m_chart, outputMembers, ref walkChildren);
			if (walkChildren)
			{
				if (m_chart.SeriesHierarchy != null)
				{
					ChartMemberCollection memberCollection = m_chart.SeriesHierarchy.MemberCollection;
					WalkChartMemberCollection(memberCollection, chartState, outputMembers, string.Empty);
				}
				m_chartHandler.OnChartEnd(m_chart);
			}
		}

		private void WalkChartMemberCollection(ChartMemberCollection memberCollection, MemberState parentMemberState, bool outputMembers, string parentScopeName)
		{
			if (memberCollection == null)
			{
				return;
			}
			for (int i = 0; i < memberCollection.Count; i++)
			{
				ChartMember chartMember = memberCollection[i];
				if (chartMember != null && !chartMember.IsTotal)
				{
					MemberState parentMemberState2 = parentMemberState;
					bool outputMember = outputMembers;
					bool overrideOutputMember = false;
					if (parentMemberState != null && (!chartMember.IsCategory || !chartMember.IsStatic))
					{
						parentMemberState2 = m_reportWalker.GetMemberState(chartMember, parentMemberState, ref outputMember, ref overrideOutputMember);
					}
					string text = parentScopeName;
					if (!string.IsNullOrEmpty(text))
					{
						text += "_";
					}
					text += HandlerBase.GetChartMemberColumnName(chartMember);
					WalkChartMember(chartMember, parentMemberState2, outputMember, text, overrideOutputMember);
				}
			}
		}

		private void WalkChartMember(ChartMember chartMember, MemberState parentMemberState, bool outputMembers, string parentScopeName, bool overrideOutputMember)
		{
			bool walkThisMember = false;
			if (m_reportWalker.AtomRendererWalk)
			{
				walkThisMember = ((!m_reportWalker.FirstInstancePass) ? (m_chartState.IsDynamic ? walkThisMember : (chartMember.DataElementOutput != DataElementOutputTypes.NoOutput)) : (chartMember.DataElementOutput != DataElementOutputTypes.NoOutput));
			}
			bool outputMemberLabelColumn = (!m_chartCategoryMemberLabelsOutput || !chartMember.IsCategory) && m_reportWalker.RenderOutDataInWalk;
			bool outputThisMember = outputMembers;
			if (chartMember.IsCategory && m_reportWalker.DynamicMemberHierarchy != null && !chartMember.IsStatic)
			{
				outputThisMember = MemberState.IndexOfMemberInDynamicPath(m_reportWalker.DynamicMemberHierarchy.ActiveDynamicMemberPath, chartMember.ID) > -1;
			}
			m_chartHandler.OnChartMemberBegin(chartMember, outputThisMember, outputMemberLabelColumn, parentScopeName, ref walkThisMember);
			if (!walkThisMember)
			{
				return;
			}
			if (chartMember.Children != null)
			{
				WalkChartMemberCollection(chartMember.Children, parentMemberState, outputMembers, parentScopeName);
			}
			else if (!chartMember.IsCategory)
			{
				m_currentSeries = chartMember;
				m_currentSeriesOutputState = outputMembers;
				if (m_chart.CategoryHierarchy != null)
				{
					if (parentMemberState != null)
					{
						RSTrace.RenderingTracer.Assert(m_chartState != null, "m_ChartState is null, parent hierarchy has not passed correctly");
					}
					WalkChartMemberCollection(m_chart.CategoryHierarchy.MemberCollection, (m_chartState != null) ? parentMemberState : null, m_chartOutputMembers, parentScopeName);
				}
				m_chartCategoryMemberLabelsOutput = true;
			}
			else
			{
				RSTrace.RenderingTracer.Assert(m_currentSeries != null, "WalkStaticChartMember -- CurrentSeries == null)");
				ChartDataPoint dataPoint = m_chart.ChartData.SeriesCollection[m_currentSeries.MemberCellIndex][chartMember.MemberCellIndex];
				bool flag = m_currentSeriesOutputState && outputMembers;
				if (!flag)
				{
					flag = overrideOutputMember;
				}
				WalkChartDataPoint(dataPoint, flag, parentScopeName);
			}
			m_chartHandler.OnChartMemberEnd(chartMember);
		}

		private void WalkChartDataPoint(ChartDataPoint dataPoint, bool outputMembers, string parentScopeName)
		{
			if (dataPoint == null)
			{
				return;
			}
			bool walkDataPoint = false;
			ChartSeriesType chartSeriesType = ChartSeriesType.Column;
			ChartSeriesSubtype subType = ChartSeriesSubtype.Plain;
			if (m_chart.DataValueSequenceRendering)
			{
				ReportEnumProperty<ChartSeriesType> type = m_chart.ChartData.SeriesCollection[m_currentSeries.MemberCellIndex].Type;
				ReportEnumProperty<ChartSeriesSubtype> subtype = m_chart.ChartData.SeriesCollection[m_currentSeries.MemberCellIndex].Subtype;
				RSTrace.RenderingTracer.Assert(type != null, "CSV Renderer WalkChartDataPoint: for an old report the chart series type is null");
				chartSeriesType = type.Value;
				if (chartSeriesType == ChartSeriesType.Range)
				{
					RSTrace.RenderingTracer.Assert(subtype != null, "CSV Renderer WalkChartDataPoint: for an old report the chart series subtype is null");
					subType = subtype.Value;
				}
			}
			m_chartHandler.OnChartDataPointBegin(dataPoint, ref walkDataPoint);
			if (!walkDataPoint)
			{
				return;
			}
			if (dataPoint.DataPointValues != null)
			{
				if (dataPoint.DataElementName != null)
				{
					parentScopeName = parentScopeName + "_" + dataPoint.DataElementName;
				}
				if (m_reportWalker.RenderOutDataInWalk)
				{
					m_chartHandler.OnChartDataPointValuesBegin(dataPoint.DataPointValues, outputMembers, m_chart.DataValueSequenceRendering, chartSeriesType, subType, parentScopeName);
				}
				m_chartHandler.OnChartDataPointValuesEnd(dataPoint.DataPointValues);
			}
			m_chartHandler.OnChartDataPointEnd(dataPoint);
		}
	}

	internal class GaugePanelWalker
	{
		private IGaugePanelHandler m_gaugePanelHandler;

		private ReportWalker m_reportWalker;

		private string GaugeScaleMaximumValue = "MaximumValue";

		private string GaugeScaleMinimumValue = "MinimumValue";

		private string GaugeInputValue = "GaugeInputValue";

		private string GaugeStartValue = "StartValue";

		private string GaugeEndValue = "EndValue";

		private string StateIndicatorStateName = "StateName";

		internal GaugePanelWalker(ReportWalker reportWalker)
		{
			m_reportWalker = reportWalker;
			m_gaugePanelHandler = m_reportWalker.GaugePanelHandler;
		}

		internal void WalkGaugePanel(GaugePanel gaugePanel, bool outputGaugePanelData)
		{
			if (gaugePanel == null || m_reportWalker == null || m_reportWalker.GaugePanelHandler == null)
			{
				return;
			}
			bool walkGaugePanel = false;
			m_gaugePanelHandler.OnGaugePanelBegin(gaugePanel, outputGaugePanelData, ref walkGaugePanel);
			if (!walkGaugePanel)
			{
				return;
			}
			if (gaugePanel.LinearGauges != null)
			{
				foreach (LinearGauge linearGauge in gaugePanel.LinearGauges)
				{
					WalkGauge(linearGauge, outputGaugePanelData);
				}
			}
			if (gaugePanel.RadialGauges != null)
			{
				foreach (RadialGauge radialGauge in gaugePanel.RadialGauges)
				{
					WalkGauge(radialGauge, outputGaugePanelData);
				}
			}
			if (gaugePanel.StateIndicators != null)
			{
				foreach (StateIndicator stateIndicator in gaugePanel.StateIndicators)
				{
					WalkStateIndicator(stateIndicator, outputGaugePanelData);
				}
			}
			m_gaugePanelHandler.OnGaugePanelEnd(gaugePanel);
		}

		private void WalkGauge(Gauge gauge, bool outputGaugeData)
		{
			if (gauge == null)
			{
				return;
			}
			if (gauge is LinearGauge)
			{
				LinearGauge linearGauge = gauge as LinearGauge;
				if (linearGauge.GaugeScales != null)
				{
					for (int i = 0; i < linearGauge.GaugeScales.Count; i++)
					{
						WalkGaugeScale(linearGauge.GaugeScales[i], outputGaugeData, gauge.Name);
					}
				}
				return;
			}
			RadialGauge radialGauge = gauge as RadialGauge;
			if (radialGauge.GaugeScales != null)
			{
				for (int j = 0; j < radialGauge.GaugeScales.Count; j++)
				{
					WalkGaugeScale(radialGauge.GaugeScales[j], outputGaugeData, gauge.Name);
				}
			}
		}

		private void WalkStateIndicator(StateIndicator stateIndicator, bool outputGaugeData)
		{
			WalkStateIndicatorStateName(stateIndicator, outputGaugeData);
			WalkGaugeInputValue(stateIndicator.GaugeInputValue, stateIndicator.Name + "_" + GaugeInputValue, outputGaugeData);
			if (stateIndicator.IndicatorStates != null)
			{
				for (int i = 0; i < stateIndicator.IndicatorStates.Count; i++)
				{
					WalkIndicatorState(stateIndicator.IndicatorStates[i], outputGaugeData, stateIndicator.Name);
				}
			}
		}

		private void WalkStateIndicatorStateName(StateIndicator stateIndicator, bool outputGaugeData)
		{
			bool walkThisStateName = false;
			if (m_reportWalker.RenderOutDataInWalk)
			{
				m_gaugePanelHandler.OnStateIndicatorStateNameBegin(stateIndicator, stateIndicator.Name + "_" + StateIndicatorStateName, outputGaugeData, ref walkThisStateName);
			}
			if (walkThisStateName)
			{
				m_gaugePanelHandler.OnStateIndicatorStateNameEnd(stateIndicator);
			}
		}

		private void WalkIndicatorState(IndicatorState indicatorState, bool outputGaugeData, string stateIndicatorName)
		{
			WalkGaugeInputValue(indicatorState.StartValue, stateIndicatorName + "_" + indicatorState.Name + "_" + GaugeStartValue, outputGaugeData);
			WalkGaugeInputValue(indicatorState.EndValue, stateIndicatorName + "_" + indicatorState.Name + "_" + GaugeEndValue, outputGaugeData);
		}

		private void WalkGaugeScale(GaugeScale gaugeScale, bool outputGaugeData, string gaugeName)
		{
			if (gaugeScale == null)
			{
				return;
			}
			WalkGaugeInputValue(gaugeScale.MinimumValue, gaugeName + "_" + gaugeScale.Name + "_" + GaugeScaleMinimumValue, outputGaugeData);
			WalkGaugeInputValue(gaugeScale.MaximumValue, gaugeName + "_" + gaugeScale.Name + "_" + GaugeScaleMaximumValue, outputGaugeData);
			if (gaugeScale is LinearScale)
			{
				LinearScale linearScale = gaugeScale as LinearScale;
				if (linearScale.GaugePointers != null)
				{
					for (int i = 0; i < linearScale.GaugePointers.Count; i++)
					{
						WalkGaugePointer(linearScale.GaugePointers[i], outputGaugeData, gaugeName);
					}
				}
				if (linearScale.ScaleRanges != null)
				{
					for (int j = 0; j < linearScale.ScaleRanges.Count; j++)
					{
						WalkScaleRange(linearScale.ScaleRanges[j], outputGaugeData, gaugeName);
					}
				}
			}
			if (!(gaugeScale is RadialScale))
			{
				return;
			}
			RadialScale radialScale = gaugeScale as RadialScale;
			if (radialScale.GaugePointers != null)
			{
				for (int k = 0; k < radialScale.GaugePointers.Count; k++)
				{
					WalkGaugePointer(radialScale.GaugePointers[k], outputGaugeData, gaugeName);
				}
			}
			if (radialScale.ScaleRanges != null)
			{
				for (int l = 0; l < radialScale.ScaleRanges.Count; l++)
				{
					WalkScaleRange(radialScale.ScaleRanges[l], outputGaugeData, gaugeName);
				}
			}
		}

		private void WalkGaugePointer(GaugePointer gaugePointer, bool outputGaugeData, string gaugeName)
		{
			if (gaugePointer == null)
			{
				return;
			}
			if (gaugePointer.CompiledInstances != null)
			{
				for (int i = 0; i < gaugePointer.CompiledInstances.Length; i++)
				{
					RSTrace.RenderingTracer.Assert(gaugePointer.CompiledInstances[i].GaugeInputValue != null, "GaugeInputValue Instance cannot be null.");
					WalkGaugeInputValue(gaugePointer.GaugeInputValue, gaugeName + "_" + gaugePointer.Name + "_" + GaugeInputValue, outputGaugeData, gaugePointer.CompiledInstances[i].GaugeInputValue.Value);
				}
			}
			else
			{
				WalkGaugeInputValue(gaugePointer.GaugeInputValue, gaugeName + "_" + gaugePointer.Name + "_" + GaugeInputValue, outputGaugeData);
			}
		}

		private void WalkScaleRange(ScaleRange scaleRange, bool outputGaugeData, string gaugeName)
		{
			if (scaleRange != null)
			{
				WalkGaugeInputValue(scaleRange.StartValue, gaugeName + "_" + scaleRange.Name + "_" + GaugeStartValue, outputGaugeData);
				WalkGaugeInputValue(scaleRange.EndValue, gaugeName + "_" + scaleRange.Name + "_" + GaugeEndValue, outputGaugeData);
			}
		}

		private void WalkGaugeInputValue(GaugeInputValue gaugeInputValue, string defaultGaugeInputValueColName, bool outputGaugeData)
		{
			WalkGaugeInputValue(gaugeInputValue, defaultGaugeInputValueColName, outputGaugeData, null);
		}

		private void WalkGaugeInputValue(GaugeInputValue gaugeInputValue, string defaultGaugeInputValueColName, bool outputGaugeData, object value)
		{
			if (gaugeInputValue != null)
			{
				bool walkThisGaugeInputValue = false;
				if (m_reportWalker.RenderOutDataInWalk)
				{
					m_gaugePanelHandler.OnGaugeInputValueBegin(gaugeInputValue, defaultGaugeInputValueColName, outputGaugeData, value, ref walkThisGaugeInputValue);
				}
				if (walkThisGaugeInputValue)
				{
					m_gaugePanelHandler.OnGaugeInputValueEnd(gaugeInputValue);
				}
			}
		}
	}

	internal class TablixWalker
	{
		private ITablixHandler m_tablixHandler;

		private Tablix m_tablix;

		private ReportWalker m_reportWalker;

		private TablixMember m_currentRow;

		private bool m_currentRowOutputState;

		private bool m_columnHeaderTraversed;

		private MemberState m_tablixState;

		internal TablixWalker(Tablix tablix, ReportWalker reportWalker)
		{
			m_tablix = tablix;
			m_reportWalker = reportWalker;
			m_tablixHandler = m_reportWalker.TablixHandler;
			m_columnHeaderTraversed = false;
		}

		internal void WalkTablix(MemberState tablixState, bool outputMembers, MemberState parentMemberState)
		{
			if (m_tablix == null || m_reportWalker == null || m_reportWalker.TablixHandler == null)
			{
				return;
			}
			m_tablixState = tablixState;
			bool walkChildren = false;
			if (m_reportWalker.AtomRendererWalk)
			{
				walkChildren = ((!m_reportWalker.FirstInstancePass) ? (tablixState.IsDynamic ? walkChildren : (m_tablix.DataElementOutput != DataElementOutputTypes.NoOutput)) : (m_tablix.DataElementOutput != DataElementOutputTypes.NoOutput));
			}
			TablixInstance tablixInstance = m_tablix.Instance as TablixInstance;
			CsvColumnHeaderHandler csvColumnHeaderHandler = m_reportWalker.TablixHandler as CsvColumnHeaderHandler;
			if (csvColumnHeaderHandler != null)
			{
				csvColumnHeaderHandler.HeaderOnlyMode = false;
			}
			if (tablixInstance != null && tablixInstance.NoRows && !tablixInstance.RenderingContext.IsSubReportContext && (parentMemberState == null || !parentMemberState.HasRows))
			{
				if (csvColumnHeaderHandler == null || csvColumnHeaderHandler.NoHeaders)
				{
					return;
				}
				csvColumnHeaderHandler.HeaderOnlyMode = true;
			}
			m_tablixHandler.OnTablixBegin(m_tablix, ref walkChildren, outputMembers);
			if (walkChildren)
			{
				WalkTablixCorner(m_tablix.Corner, tablixState, outputMembers);
				if (m_tablix.RowHierarchy != null)
				{
					TablixMemberCollection memberCollection = m_tablix.RowHierarchy.MemberCollection;
					WalkTablixMemberCollection(memberCollection, tablixState, outputMembers);
				}
				m_tablixHandler.OnTablixEnd(m_tablix);
			}
			if (csvColumnHeaderHandler != null)
			{
				csvColumnHeaderHandler.HeaderOnlyMode = false;
			}
		}

		private void WalkTablixCorner(TablixCorner corner, MemberState parentMemberState, bool outputMembers)
		{
			if (corner == null || corner.RowCollection == null)
			{
				return;
			}
			TablixCornerRowCollection rowCollection = corner.RowCollection;
			for (int i = 0; i < rowCollection.Count; i++)
			{
				TablixCornerRow tablixCornerRow = rowCollection[i];
				if (tablixCornerRow == null)
				{
					continue;
				}
				for (int j = 0; j < tablixCornerRow.Count; j++)
				{
					TablixCornerCell tablixCornerCell = tablixCornerRow[j];
					if (tablixCornerCell != null && tablixCornerCell.CellContents != null)
					{
						m_reportWalker.WalkReportItem(tablixCornerCell.CellContents.ReportItem, parentMemberState, outputMembers);
					}
				}
			}
		}

		private void WalkTablixMemberCollection(TablixMemberCollection memberCollection, MemberState parentMemberState, bool outputMembers)
		{
			if (memberCollection == null)
			{
				return;
			}
			for (int i = 0; i < memberCollection.Count; i++)
			{
				TablixMember tablixMember = memberCollection[i];
				if (tablixMember != null && !tablixMember.IsTotal)
				{
					MemberState parentMemberState2 = parentMemberState;
					bool outputMember = outputMembers;
					bool overrideOutputMember = false;
					if (parentMemberState != null && (!tablixMember.IsColumn || !tablixMember.IsStatic))
					{
						parentMemberState2 = m_reportWalker.GetMemberState(tablixMember, parentMemberState, ref outputMember, ref overrideOutputMember);
					}
					WalkTablixMember(tablixMember, parentMemberState2, outputMember, overrideOutputMember);
				}
			}
		}

		private void WalkTablixMember(TablixMember tablixMember, MemberState parentMemberState, bool outputMembers, bool overrideOutputMember)
		{
			bool walkThisMember = false;
			if (m_reportWalker.AtomRendererWalk)
			{
				walkThisMember = ((!m_reportWalker.FirstInstancePass) ? (m_tablixState.IsDynamic ? walkThisMember : (tablixMember.DataElementOutput != DataElementOutputTypes.NoOutput)) : (tablixMember.DataElementOutput != DataElementOutputTypes.NoOutput));
			}
			m_tablixHandler.OnTablixMemberBegin(tablixMember, ref walkThisMember, outputMembers);
			if (!walkThisMember)
			{
				return;
			}
			if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents != null && (!tablixMember.IsColumn || !m_columnHeaderTraversed))
			{
				bool outputMembers2 = outputMembers;
				if (tablixMember.IsColumn && m_reportWalker.DynamicMemberHierarchy != null && !tablixMember.IsStatic)
				{
					outputMembers2 = MemberState.IndexOfMemberInDynamicPath(m_reportWalker.DynamicMemberHierarchy.ActiveDynamicMemberPath, tablixMember.ID) > -1;
				}
				m_reportWalker.WalkReportItem(tablixMember.TablixHeader.CellContents.ReportItem, parentMemberState, outputMembers2);
			}
			if (tablixMember.Children != null)
			{
				WalkTablixMemberCollection(tablixMember.Children, parentMemberState, outputMembers);
			}
			else if (!tablixMember.IsColumn)
			{
				m_currentRow = tablixMember;
				m_currentRowOutputState = outputMembers;
				if (m_tablix.ColumnHierarchy != null)
				{
					if (parentMemberState != null)
					{
						RSTrace.RenderingTracer.Assert(m_tablixState != null, "m_TablixState is null, parent hierarchy not passed correctly");
					}
					WalkTablixMemberCollection(m_tablix.ColumnHierarchy.MemberCollection, (parentMemberState != null) ? parentMemberState : null, outputMembers);
				}
				m_columnHeaderTraversed = true;
			}
			else
			{
				RSTrace.RenderingTracer.Assert(m_currentRow != null, "WalkStaticTablixMember -- CurrentRow == null)");
				TablixCell cell = m_tablix.Body.RowCollection[m_currentRow.MemberCellIndex][tablixMember.MemberCellIndex];
				bool flag = m_currentRowOutputState && outputMembers;
				if (!flag)
				{
					flag = overrideOutputMember;
				}
				WalkTablixCell(cell, parentMemberState, flag);
			}
			m_tablixHandler.OnTablixMemberEnd(tablixMember);
		}

		private void WalkTablixCell(TablixCell cell, MemberState parentMemberState, bool outputMembers)
		{
			if (cell == null)
			{
				return;
			}
			bool walkCell = false;
			m_tablixHandler.OnTablixCellBegin(cell, ref walkCell);
			if (walkCell)
			{
				if (cell.CellContents != null)
				{
					m_reportWalker.WalkReportItem(cell.CellContents.ReportItem, parentMemberState, outputMembers);
				}
				m_tablixHandler.OnTablixCellEnd(cell);
			}
		}
	}

	internal abstract class MapVectorLayerWalker
	{
		private ReportWalker m_reportWalker;

		private IMapHandler m_mapHandler;

		protected abstract MapVectorLayer Layer { get; }

		protected abstract MapSpatialElementTemplate SpatialElementTemplate { get; }

		internal MapVectorLayerWalker(ReportWalker reportWalker)
		{
			m_reportWalker = reportWalker;
			m_mapHandler = m_reportWalker.MapHandler;
		}

		protected abstract void WalkRules(bool outputMembers);

		internal void WalkLayer(MemberState mapState, bool outputMapData)
		{
			if (Layer == null || m_reportWalker == null || m_mapHandler == null)
			{
				return;
			}
			bool walkLayer = false;
			m_mapHandler.OnMapVectorLayerBegin(Layer, outputMapData, ref walkLayer);
			if (walkLayer)
			{
				if (SpatialElementTemplate == null || SpatialElementTemplate.DataElementOutput != DataElementOutputTypes.NoOutput)
				{
					WalkGrouping(Layer.MapDataRegion.MapMember, mapState, outputMapData);
				}
				m_mapHandler.OnMapVectorLayerEnd(Layer);
			}
		}

		private void WalkGrouping(MapMember mapMember, MemberState parentMemberState, bool outputMembers)
		{
			if (mapMember != null)
			{
				MemberState parentMemberState2 = parentMemberState;
				bool outputMember = outputMembers;
				bool overrideOutputMember = false;
				if (!mapMember.IsStatic && parentMemberState != null)
				{
					parentMemberState2 = m_reportWalker.GetMemberState(mapMember, parentMemberState, ref outputMember, ref overrideOutputMember);
				}
				if (mapMember.ChildMapMember != null)
				{
					WalkGrouping(mapMember.ChildMapMember, parentMemberState2, outputMember);
				}
				else
				{
					WalkDataRow(mapMember, outputMember);
				}
			}
		}

		private void WalkDataRow(MapMember mapMember, bool outputMembers)
		{
			bool walkMapMember = true;
			bool renderOutDataInWalk = m_reportWalker.RenderOutDataInWalk;
			m_mapHandler.OnMapMemberBegin(mapMember, SpatialElementTemplate, renderOutDataInWalk, outputMembers, Layer, ref walkMapMember);
			if (walkMapMember)
			{
				WalkRules(outputMembers);
				m_mapHandler.OnMapMemberEnd(mapMember);
			}
		}

		protected void WalkRule(MapAppearanceRule mapRule, bool outputMembers, ref int index)
		{
			bool walkRule = false;
			if (m_reportWalker.RenderOutDataInWalk)
			{
				m_mapHandler.OnMapAppearanceRuleBegin(mapRule, outputMembers, ref index, ref walkRule);
			}
			if (walkRule)
			{
				m_mapHandler.OnMapAppearanceRuleEnd(mapRule);
			}
		}
	}

	private class MapPolygonLayerWalker : MapVectorLayerWalker
	{
		private MapPolygonLayer m_layer;

		protected override MapVectorLayer Layer => m_layer;

		protected override MapSpatialElementTemplate SpatialElementTemplate => m_layer.MapPolygonTemplate;

		internal MapPolygonLayerWalker(MapPolygonLayer layer, ReportWalker reportWalker)
			: base(reportWalker)
		{
			m_layer = layer;
		}

		protected override void WalkRules(bool outputMembers)
		{
			int index = 0;
			MapPolygonRules mapPolygonRules = m_layer.MapPolygonRules;
			if (mapPolygonRules != null)
			{
				WalkRule(mapPolygonRules.MapColorRule, outputMembers, ref index);
			}
			MapPointRules mapCenterPointRules = m_layer.MapCenterPointRules;
			if (mapCenterPointRules != null)
			{
				WalkRule(mapCenterPointRules.MapColorRule, outputMembers, ref index);
				WalkRule(mapCenterPointRules.MapSizeRule, outputMembers, ref index);
				WalkRule(mapCenterPointRules.MapMarkerRule, outputMembers, ref index);
			}
		}
	}

	private class MapPointLayerWalker : MapVectorLayerWalker
	{
		private MapPointLayer m_layer;

		protected override MapVectorLayer Layer => m_layer;

		protected override MapSpatialElementTemplate SpatialElementTemplate => m_layer.MapPointTemplate;

		internal MapPointLayerWalker(MapPointLayer layer, ReportWalker reportWalker)
			: base(reportWalker)
		{
			m_layer = layer;
		}

		protected override void WalkRules(bool outputMembers)
		{
			int index = 0;
			MapPointRules mapPointRules = m_layer.MapPointRules;
			if (mapPointRules != null)
			{
				WalkRule(mapPointRules.MapColorRule, outputMembers, ref index);
				WalkRule(mapPointRules.MapSizeRule, outputMembers, ref index);
				WalkRule(mapPointRules.MapMarkerRule, outputMembers, ref index);
			}
		}
	}

	private class MapLineLayerWalker : MapVectorLayerWalker
	{
		private MapLineLayer m_layer;

		protected override MapVectorLayer Layer => m_layer;

		protected override MapSpatialElementTemplate SpatialElementTemplate => m_layer.MapLineTemplate;

		internal MapLineLayerWalker(MapLineLayer layer, ReportWalker reportWalker)
			: base(reportWalker)
		{
			m_layer = layer;
		}

		protected override void WalkRules(bool outputMembers)
		{
			int index = 0;
			MapLineRules mapLineRules = m_layer.MapLineRules;
			if (mapLineRules != null)
			{
				WalkRule(mapLineRules.MapColorRule, outputMembers, ref index);
				WalkRule(mapLineRules.MapSizeRule, outputMembers, ref index);
			}
		}
	}

	protected const string ConnectString = "_";

	private IReportHandler m_reportHandler;

	private ITablixHandler m_tablixHandler;

	private IChartHandler m_chartHandler;

	private IOutputRowHandler m_outputRowHandler;

	private IGaugePanelHandler m_gaugePanelHandler;

	private IMapHandler m_mapHandler;

	private bool m_instanceWalk;

	private bool m_walkTopLevelOnly = true;

	private bool m_firstInstancePass = true;

	private bool m_atomHeaderInstanceWalk;

	private bool m_renderOutDataInWalk;

	private bool m_atomRendererWalk;

	private ArrayList m_dataRegionsOrMaps = new ArrayList();

	private MemberState m_dynamicMemberHierarchyRoot;

	private Dictionary<string, bool> m_activeDefinitionPaths = new Dictionary<string, bool>();

	internal IReportHandler ReportHandler => m_reportHandler;

	internal bool FirstInstancePass => m_firstInstancePass;

	internal ITablixHandler TablixHandler => m_tablixHandler;

	internal IChartHandler ChartHandler => m_chartHandler;

	internal IGaugePanelHandler GaugePanelHandler => m_gaugePanelHandler;

	internal IMapHandler MapHandler => m_mapHandler;

	internal bool InstanceWalk => m_instanceWalk;

	internal IEnumerator TopLevelDataRegionsOrMaps => m_dataRegionsOrMaps.GetEnumerator();

	internal MemberState DynamicMemberHierarchy
	{
		get
		{
			return m_dynamicMemberHierarchyRoot;
		}
		set
		{
			m_dynamicMemberHierarchyRoot = value;
		}
	}

	internal bool AtomHeaderInstanceWalk => m_atomHeaderInstanceWalk;

	internal bool AtomRendererWalk => m_atomRendererWalk;

	internal bool RenderOutDataInWalk => m_renderOutDataInWalk;

	internal ReportWalker(IReportHandler reportHandler, IOutputRowHandler outputRowHandler, ITablixHandler tablixHandler, HandlerBase dvHandler, bool instanceWalk, bool walkTopLevelOnly)
	{
		m_reportHandler = reportHandler;
		m_tablixHandler = tablixHandler;
		m_outputRowHandler = outputRowHandler;
		m_instanceWalk = instanceWalk;
		m_walkTopLevelOnly = walkTopLevelOnly;
		m_chartHandler = dvHandler;
		m_gaugePanelHandler = dvHandler;
		m_mapHandler = dvHandler;
	}

	internal void WalkDataRegionOrMap(ReportItem reportItem)
	{
		if (m_instanceWalk)
		{
			m_firstInstancePass = true;
		}
		bool flag = InitializeDataRegionOrMap(reportItem);
		while (flag)
		{
			flag = WalkNextDataRegionOrMapRow(reportItem);
		}
	}

	internal bool InitializeDataRegionOrMap(ReportItem reportItem)
	{
		if (m_outputRowHandler != null)
		{
			m_outputRowHandler.OnRegionBegin();
		}
		if (m_instanceWalk || m_atomHeaderInstanceWalk)
		{
			if (m_dynamicMemberHierarchyRoot == null)
			{
				m_dynamicMemberHierarchyRoot = AddContainer("DR" + reportItem.ID, null);
			}
			else if (!m_atomHeaderInstanceWalk)
			{
				m_dynamicMemberHierarchyRoot.ResetDynamicMembers();
			}
		}
		if (!m_firstInstancePass || (!m_instanceWalk && !m_atomHeaderInstanceWalk) || m_reportHandler is XmlTypeHandler)
		{
			m_renderOutDataInWalk = true;
		}
		return true;
	}

	private bool WalkRowWithActiveDynamicMemberPath()
	{
		bool flag = true;
		string activeDynamicMemberPath = m_dynamicMemberHierarchyRoot.ActiveDynamicMemberPath;
		if (!string.IsNullOrEmpty(activeDynamicMemberPath))
		{
			if (!m_activeDefinitionPaths.ContainsKey(activeDynamicMemberPath))
			{
				flag = !m_dynamicMemberHierarchyRoot.ContainsDynamicMemberPath("_0" + activeDynamicMemberPath);
				m_activeDefinitionPaths[activeDynamicMemberPath] = flag;
			}
			else
			{
				flag = m_activeDefinitionPaths[activeDynamicMemberPath];
			}
		}
		return flag;
	}

	internal bool WalkNextDataRegionOrMapRow(ReportItem reportItem)
	{
		bool flag = false;
		if (m_outputRowHandler != null)
		{
			m_outputRowHandler.OnRowBegin();
		}
		bool flag2 = true;
		if (m_instanceWalk && !m_firstInstancePass)
		{
			flag2 = WalkRowWithActiveDynamicMemberPath();
		}
		if (flag2)
		{
			WalkReportItem(reportItem, m_dynamicMemberHierarchyRoot, outputMembers: true);
		}
		if (m_instanceWalk || m_atomHeaderInstanceWalk)
		{
			if (!m_firstInstancePass)
			{
				if (!m_atomHeaderInstanceWalk)
				{
					flag = m_dynamicMemberHierarchyRoot.AdvanceDynamicMembers();
				}
			}
			else
			{
				bool flag3 = false;
				flag3 = m_dynamicMemberHierarchyRoot.ResetAllMembers(AtomHeaderInstanceWalk);
				m_firstInstancePass = false;
				m_renderOutDataInWalk = true;
				flag = true || flag3;
			}
		}
		if (m_reportHandler != null && m_reportHandler.Done)
		{
			flag = false;
		}
		if (m_outputRowHandler != null && flag2)
		{
			m_outputRowHandler.OnRowEnd();
		}
		if (m_outputRowHandler != null && !flag)
		{
			m_outputRowHandler.OnRegionEnd();
		}
		return flag;
	}

	internal void WalkReport(Report report)
	{
		if (report == null || report.ReportSections == null || report.ReportSections.Count == 0)
		{
			return;
		}
		if (m_reportHandler != null)
		{
			bool walkChildren = false;
			m_reportHandler.OnReportBegin(report, ref walkChildren);
		}
		bool flag = true;
		if (m_instanceWalk || m_atomHeaderInstanceWalk)
		{
			if (m_dynamicMemberHierarchyRoot == null)
			{
				m_dynamicMemberHierarchyRoot = AddContainer("Report" + report.ID, null);
			}
			else if (!m_atomHeaderInstanceWalk)
			{
				m_dynamicMemberHierarchyRoot.ResetDynamicMembers();
			}
		}
		if (m_outputRowHandler != null)
		{
			m_outputRowHandler.OnRegionBegin();
		}
		bool flag2 = true;
		if (!m_firstInstancePass || (!m_instanceWalk && !m_atomHeaderInstanceWalk) || m_reportHandler is XmlTypeHandler)
		{
			m_renderOutDataInWalk = true;
		}
		while (flag)
		{
			flag = false;
			if (m_outputRowHandler != null)
			{
				m_outputRowHandler.OnRowBegin();
			}
			if (m_instanceWalk && !m_firstInstancePass)
			{
				flag2 = WalkRowWithActiveDynamicMemberPath();
			}
			if (flag2)
			{
				foreach (ReportSection reportSection in report.ReportSections)
				{
					if (reportSection.Body != null)
					{
						ReportItemCollection reportItemCollection = reportSection.Body.ReportItemCollection;
						if (reportItemCollection != null && DataElementOutputTypes.NoOutput != reportSection.DataElementOutput)
						{
							WalkReportItemCollection(reportItemCollection, m_dynamicMemberHierarchyRoot, outputMembers: true);
						}
					}
				}
			}
			if (m_instanceWalk || m_atomHeaderInstanceWalk)
			{
				if (!m_firstInstancePass)
				{
					if (!m_atomHeaderInstanceWalk)
					{
						flag = m_dynamicMemberHierarchyRoot.AdvanceDynamicMembers();
					}
				}
				else
				{
					bool flag3 = false;
					flag3 = m_dynamicMemberHierarchyRoot.ResetAllMembers(m_atomHeaderInstanceWalk);
					m_firstInstancePass = false;
					m_renderOutDataInWalk = true;
					flag = true || flag3;
				}
			}
			if (m_reportHandler != null && m_reportHandler.Done)
			{
				flag = false;
			}
			if (m_outputRowHandler != null && flag2)
			{
				m_outputRowHandler.OnRowEnd();
			}
		}
		if (m_outputRowHandler != null)
		{
			m_outputRowHandler.OnRegionEnd();
		}
	}

	private void WalkMap(Map map, MemberState mapState, bool outputMapData)
	{
		if (map == null || m_mapHandler == null)
		{
			return;
		}
		bool walkMap = false;
		if (m_atomRendererWalk)
		{
			walkMap = ((!m_firstInstancePass) ? (mapState.IsDynamic ? walkMap : (map.DataElementOutput != DataElementOutputTypes.NoOutput)) : (map.DataElementOutput != DataElementOutputTypes.NoOutput));
		}
		m_mapHandler.OnMapBegin(map, outputMapData, ref walkMap);
		if (!walkMap)
		{
			return;
		}
		if (map.MapLayers != null)
		{
			foreach (MapLayer mapLayer in map.MapLayers)
			{
				if (mapLayer is MapPolygonLayer)
				{
					new MapPolygonLayerWalker((MapPolygonLayer)mapLayer, this).WalkLayer(mapState, outputMapData);
				}
				else if (mapLayer is MapPointLayer)
				{
					new MapPointLayerWalker((MapPointLayer)mapLayer, this).WalkLayer(mapState, outputMapData);
				}
				else if (mapLayer is MapLineLayer)
				{
					new MapLineLayerWalker((MapLineLayer)mapLayer, this).WalkLayer(mapState, outputMapData);
				}
			}
		}
		m_mapHandler.OnMapEnd(map);
	}

	private void WalkSubReport(SubReport subReport, MemberState parentMemberState, bool outputMembers)
	{
		if (subReport == null)
		{
			return;
		}
		bool walkSubreport = false;
		m_reportHandler.OnSubReportBegin(subReport, ref walkSubreport);
		if (!walkSubreport)
		{
			return;
		}
		SubReportInstance subReportInstance = (SubReportInstance)subReport.Instance;
		if (subReportInstance != null)
		{
			if (subReportInstance.ProcessedWithError)
			{
				throw new ReportRenderingException(subReportInstance.ErrorMessage);
			}
			if (subReport.Report != null && subReport.Report.ReportSections != null)
			{
				ReportSectionCollection reportSections = subReport.Report.ReportSections;
				for (int i = 0; i < reportSections.Count; i++)
				{
					ReportSection reportSection = reportSections[i];
					if (reportSection.Body != null && reportSection.Body.ReportItemCollection != null)
					{
						WalkReportItemCollection(reportSection.Body.ReportItemCollection, parentMemberState, outputMembers);
					}
				}
			}
		}
		m_reportHandler.OnSubReportEnd(subReport);
	}

	private void WalkRectangle(Rectangle rectangle, MemberState parentMemberState, bool outputMembers)
	{
		if (rectangle != null)
		{
			bool walkChildren = false;
			m_reportHandler.OnRectangleBegin(rectangle, ref walkChildren);
			if (walkChildren)
			{
				WalkReportItemCollection(rectangle.ReportItemCollection, parentMemberState, outputMembers);
				m_reportHandler.OnRectangleEnd(rectangle);
			}
		}
	}

	private void WalkTextBox(TextBox textBox, bool outputDynamic)
	{
		if (textBox != null)
		{
			bool render = false;
			if (m_renderOutDataInWalk)
			{
				m_reportHandler.OnTextBoxBegin(textBox, outputDynamic, ref render);
			}
			if (render)
			{
				m_reportHandler.OnTextBoxEnd(textBox);
			}
		}
	}

	private void WalkReportItem(ReportItem reportItem, MemberState parentMemberState, bool outputMembers)
	{
		if (reportItem == null)
		{
			return;
		}
		if (reportItem is TextBox)
		{
			WalkTextBox((TextBox)reportItem, outputMembers);
		}
		else if (reportItem is Tablix)
		{
			MemberState tablixState = null;
			bool outputReportItem = outputMembers;
			if (parentMemberState != null)
			{
				tablixState = GetMemberStateForReportItem(reportItem, parentMemberState, ref outputReportItem);
			}
			TablixWalker tablixWalker = new TablixWalker((Tablix)reportItem, this);
			tablixWalker.WalkTablix(tablixState, outputReportItem, parentMemberState);
		}
		else if (reportItem is Chart)
		{
			MemberState chartState = null;
			bool outputReportItem2 = outputMembers;
			if (parentMemberState != null)
			{
				chartState = GetMemberStateForReportItem(reportItem, parentMemberState, ref outputReportItem2);
			}
			ChartWalker chartWalker = new ChartWalker((Chart)reportItem, this);
			chartWalker.WalkChart(chartState, outputReportItem2);
		}
		else if (reportItem is GaugePanel)
		{
			GaugePanelState gaugePanelState = null;
			bool outputGaugePanelData = outputMembers;
			if (parentMemberState != null)
			{
				if (parentMemberState.GetDynamicItemID(reportItem.ID, out var id))
				{
					gaugePanelState = parentMemberState.FindChild(id) as GaugePanelState;
					RSTrace.RenderingTracer.Assert(gaugePanelState != null, "GaugePanel not found in the parent dynamic member hierarchy");
				}
				if (gaugePanelState == null)
				{
					gaugePanelState = AddGaugePanel((GaugePanel)reportItem, parentMemberState);
					if (m_instanceWalk)
					{
						gaugePanelState.ResetDynamicMembers();
					}
				}
				if (!gaugePanelState.IsActiveChild())
				{
					outputGaugePanelData = false;
				}
			}
			GaugePanelWalker gaugePanelWalker = new GaugePanelWalker(this);
			gaugePanelWalker.WalkGaugePanel((GaugePanel)reportItem, outputGaugePanelData);
		}
		else if (reportItem is Map)
		{
			MemberState mapState = null;
			bool outputReportItem3 = outputMembers;
			if (parentMemberState != null)
			{
				mapState = GetMemberStateForReportItem(reportItem, parentMemberState, ref outputReportItem3);
			}
			WalkMap((Map)reportItem, mapState, outputReportItem3);
		}
		else if (reportItem is Rectangle)
		{
			WalkRectangle((Rectangle)reportItem, parentMemberState, outputMembers);
		}
		else
		{
			if (!(reportItem is SubReport))
			{
				return;
			}
			MemberState memberState = null;
			bool outputMembers2 = outputMembers;
			if (parentMemberState != null)
			{
				if (parentMemberState.GetDynamicItemID(reportItem.ID, out var id2))
				{
					memberState = parentMemberState.FindChild(id2);
					RSTrace.RenderingTracer.Assert(memberState != null, "SubReport not found in the parent dynamic member hierarchy");
				}
				if (memberState == null)
				{
					memberState = AddDataRegionOrMapContainer(reportItem, parentMemberState);
				}
				if (memberState.IsDynamic && (!memberState.IsActiveChild() || !memberState.HasRows))
				{
					outputMembers2 = false;
				}
			}
			WalkSubReport((SubReport)reportItem, memberState, outputMembers2);
		}
	}

	private void WalkReportItemCollection(ReportItemCollection reportItemCollection, MemberState parentMemberState, bool outputMembers)
	{
		if (reportItemCollection == null)
		{
			return;
		}
		for (int i = 0; i < reportItemCollection.Count; i++)
		{
			ReportItem reportItem = reportItemCollection[i];
			if ((reportItem is DataRegion || reportItem is Map) && m_walkTopLevelOnly)
			{
				if (reportItem is Tablix || reportItem is Chart || reportItem is GaugePanel || reportItem is Map)
				{
					m_dataRegionsOrMaps.Add(reportItem);
					m_reportHandler.OnTopLevelDataRegionOrMap(reportItem);
				}
			}
			else
			{
				WalkReportItem(reportItem, parentMemberState, outputMembers);
			}
		}
	}

	private MemberState AddDynamicMember(DataRegionMember member, MemberState parentMemberState)
	{
		int id = parentMemberState.NextID++;
		MemberState memberState = new MemberState(member.IsStatic ? null : member, id);
		parentMemberState?.AddChild(memberState);
		parentMemberState.AddToMemoryMapper(member.ID, id);
		return memberState;
	}

	private MemberState AddDataRegionOrMapContainer(ReportItem dataRegionOrMap, MemberState parentMemberState)
	{
		RSTrace.RenderingTracer.Assert(dataRegionOrMap is Tablix || dataRegionOrMap is Chart || dataRegionOrMap is Map || dataRegionOrMap is SubReport, "Adding data region or map container memberstate for report item that's not tablix, chart, map, or subreport");
		MemberState memberState = AddContainer(dataRegionOrMap.ID, parentMemberState);
		if (dataRegionOrMap is SubReport)
		{
			memberState.SubReportMember = (SubReport)dataRegionOrMap;
		}
		parentMemberState?.AddChild(memberState);
		return memberState;
	}

	private GaugePanelState AddGaugePanel(GaugePanel gaugePanel, MemberState parentMemberState)
	{
		int id = parentMemberState.NextID++;
		GaugePanelState gaugePanelState = new GaugePanelState(id, gaugePanel.ID);
		parentMemberState?.AddChild(gaugePanelState);
		parentMemberState.AddToMemoryMapper(gaugePanel.ID, id);
		return gaugePanelState;
	}

	private MemberState AddContainer(string childID, MemberState parentMemberState)
	{
		int id = ((parentMemberState != null) ? parentMemberState.NextID++ : 0);
		MemberState result = new MemberState(null, id);
		parentMemberState?.AddToMemoryMapper(childID, id);
		return result;
	}

	private MemberState GetMemberState(DataRegionMember member, MemberState parentMemberState, ref bool outputMember, ref bool overrideOutputMember)
	{
		MemberState memberState = null;
		if (parentMemberState.GetDynamicItemID(member.ID, out var id))
		{
			memberState = parentMemberState.FindChild(id);
			RSTrace.RenderingTracer.Assert(memberState != null, "Dynamic member not found in the parent dynamic member hierarchy");
		}
		if (memberState == null)
		{
			memberState = AddDynamicMember(member, parentMemberState);
			if (m_instanceWalk && !member.IsStatic && !m_atomHeaderInstanceWalk)
			{
				memberState.ResetDynamicMembers();
			}
		}
		if (memberState.IsDynamic && (!memberState.IsActiveChild() || !memberState.HasRows))
		{
			outputMember = false;
			if (memberState.HasRows)
			{
				string text = memberState.CurrentDynamicMemberPath;
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(memberState.ActiveDynamicMemberPath))
				{
					text = text.Remove(text.Length - 1);
				}
				text += memberState.ActiveDynamicMemberPath;
				string activeDynamicMemberPath = DynamicMemberHierarchy.ActiveDynamicMemberPath;
				if (activeDynamicMemberPath.Contains(text))
				{
					outputMember = true;
					overrideOutputMember = true;
				}
			}
		}
		return memberState;
	}

	private MemberState GetMemberStateForReportItem(ReportItem reportItem, MemberState parentMemberState, ref bool outputReportItem)
	{
		int id;
		bool dynamicItemID = parentMemberState.GetDynamicItemID(reportItem.ID, out id);
		MemberState memberState = null;
		if (dynamicItemID)
		{
			memberState = parentMemberState.FindChild(id);
			RSTrace.RenderingTracer.Assert(memberState != null, "Report Item State not found in the parent dynamic member hierarchy");
		}
		if (memberState == null)
		{
			memberState = AddDataRegionOrMapContainer(reportItem, parentMemberState);
		}
		if (!memberState.IsActiveChild() && memberState.IsDynamic)
		{
			outputReportItem = false;
		}
		return memberState;
	}
}
