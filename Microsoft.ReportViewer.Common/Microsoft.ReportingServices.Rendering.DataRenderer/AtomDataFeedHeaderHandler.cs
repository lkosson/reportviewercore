using System.Collections.Generic;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class AtomDataFeedHeaderHandler : AtomDataFeedHandlerBase
{
	internal AtomDataFeedHeaderHandler(AtomDataFeedVisitor visitor, List<string> definitionPathSteps)
		: base(visitor, definitionPathSteps)
	{
	}

	public override void OnTextBoxBegin(TextBox textBox, bool output, ref bool walkTextBox)
	{
		if (textBox.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkTextBox = true;
			m_visitor.AddColumnName(textBox.DataElementName, textBox);
		}
	}

	public override void OnChartMemberBegin(ChartMember chartMember, bool outputThisMember, bool outputMemberLabelColumn, string parentScopeName, ref bool walkThisMember)
	{
		bool flag = walkThisMember;
		if (!flag)
		{
			base.OnChartMemberBegin(chartMember, outputThisMember, outputMemberLabelColumn, parentScopeName, ref walkThisMember);
		}
		if (walkThisMember)
		{
			if (chartMember.Group != null && chartMember.IsCategory && !flag)
			{
				walkThisMember = IsPartOfDefinitionPathSteps(chartMember.DefinitionPath);
			}
			if (walkThisMember && outputMemberLabelColumn)
			{
				m_visitor.AddColumnName(string.IsNullOrEmpty(parentScopeName) ? "Label" : (parentScopeName + "_label"), chartMember);
			}
		}
	}

	public override void OnChartDataPointValuesBegin(ChartDataPointValues dataPointValues, bool output, bool oldDataPointsGeneration, ChartSeriesType type, ChartSeriesSubtype subType, string parentScopeName)
	{
		bool hasStart = dataPointValues.Start != null;
		AddDataPointValueColumn(dataPointValues.X, GetDataValuesColumnName("X", DataPointValueProperties.X, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		AddDataPointValueColumn(dataPointValues.Y, GetDataValuesColumnName("Y", DataPointValueProperties.Y, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		AddDataPointValueColumn(dataPointValues.Size, GetDataValuesColumnName("Size", DataPointValueProperties.Size, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		AddDataPointValueColumn(dataPointValues.High, GetDataValuesColumnName("High", DataPointValueProperties.High, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		AddDataPointValueColumn(dataPointValues.Low, GetDataValuesColumnName("Low", DataPointValueProperties.Low, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		AddDataPointValueColumn(dataPointValues.Start, GetDataValuesColumnName("Start", DataPointValueProperties.Start, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		AddDataPointValueColumn(dataPointValues.End, GetDataValuesColumnName("End", DataPointValueProperties.End, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		AddDataPointValueColumn(dataPointValues.Mean, GetDataValuesColumnName("Mean", DataPointValueProperties.Mean, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		AddDataPointValueColumn(dataPointValues.Median, GetDataValuesColumnName("Median", DataPointValueProperties.Median, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
	}

	private void AddDataPointValueColumn(ReportVariantProperty dataPointValue, string colName)
	{
		if (dataPointValue != null)
		{
			m_visitor.AddColumnName(colName);
		}
	}

	public override void OnGaugeInputValueBegin(GaugeInputValue gaugeInputValue, string defaultGaugeInputValueColName, bool output, object value, ref bool walkThisGaugeInputValue)
	{
		base.OnGaugeInputValueBegin(gaugeInputValue, defaultGaugeInputValueColName, output, value, ref walkThisGaugeInputValue);
		if (walkThisGaugeInputValue)
		{
			string text = gaugeInputValue.DataElementName;
			if (string.IsNullOrEmpty(text))
			{
				text = defaultGaugeInputValueColName;
			}
			m_visitor.AddColumnName(text);
		}
	}

	public override void OnMapMemberBegin(MapMember mapMember, MapSpatialElementTemplate template, bool outputMapMemberLabel, bool outputDynamicMembers, MapVectorLayer layer, ref bool walkMapMember)
	{
		base.OnMapMemberBegin(mapMember, template, outputMapMemberLabel, outputDynamicMembers, layer, ref walkMapMember);
		if (mapMember.Group != null)
		{
			walkMapMember = IsPartOfDefinitionPathSteps(mapMember.DefinitionPath);
		}
		if (walkMapMember && outputMapMemberLabel)
		{
			string layerName = GetLayerName(layer);
			m_visitor.AddColumnName(GetMapMemberColumnName(template, layerName), mapMember);
		}
	}

	public override void OnMapAppearanceRuleBegin(MapAppearanceRule mapRule, bool outputMapRule, ref int index, ref bool walkRule)
	{
		base.OnMapAppearanceRuleBegin(mapRule, outputMapRule, ref index, ref walkRule);
		if (!walkRule)
		{
			return;
		}
		string text = mapRule.DataElementName;
		string ruleValue = string.Empty;
		if (GetMapRuleDataValue(mapRule, out ruleValue))
		{
			if (string.IsNullOrEmpty(text))
			{
				text = GetMapRuleColName(index);
				index++;
			}
			m_visitor.AddColumnName(text);
		}
	}

	public override void OnStateIndicatorStateNameBegin(StateIndicator stateIndicator, string defaultStateIndicatorStateNameColName, bool output, ref bool walkThisStateName)
	{
		base.OnStateIndicatorStateNameBegin(stateIndicator, defaultStateIndicatorStateNameColName, output, ref walkThisStateName);
		if (walkThisStateName)
		{
			string text = stateIndicator.StateDataElementName;
			if (string.IsNullOrEmpty(text))
			{
				text = defaultStateIndicatorStateNameColName;
			}
			m_visitor.AddColumnName(text);
		}
	}
}
