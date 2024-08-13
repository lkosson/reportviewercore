using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class CsvColumnHeaderHandler : HandlerBase
{
	private CsvVisitor m_visitor;

	private bool m_noHeaders;

	private bool m_excelMode = true;

	private int m_columnsNum;

	internal int Columns => m_columnsNum;

	internal bool NoHeaders => m_noHeaders;

	internal bool HeaderOnlyMode { get; set; }

	internal int HeaderOnlyColumns { get; private set; }

	internal CsvColumnHeaderHandler(CsvVisitor csvVisitor, bool noHeaders, bool excelMode)
	{
		m_visitor = csvVisitor;
		m_noHeaders = noHeaders;
		m_excelMode = excelMode;
	}

	public override void OnTextBoxBegin(TextBox textBox, bool output, ref bool walkTextBox)
	{
		if (textBox.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkTextBox = true;
			m_columnsNum++;
			if (HeaderOnlyMode)
			{
				HeaderOnlyColumns++;
			}
			if (!m_noHeaders)
			{
				m_visitor.WriteValue(textBox.DataElementName, m_excelMode);
			}
		}
	}

	public override void OnChartMemberBegin(ChartMember chartMember, bool outputThisMember, bool outputMemberLabelColumn, string parentScopeName, ref bool walkThisMember)
	{
		base.OnChartMemberBegin(chartMember, outputThisMember, outputMemberLabelColumn, parentScopeName, ref walkThisMember);
		if (walkThisMember && outputMemberLabelColumn)
		{
			m_columnsNum++;
			if (!m_noHeaders)
			{
				m_visitor.WriteValue(string.IsNullOrEmpty(parentScopeName) ? "Label" : (parentScopeName + "_label"), m_excelMode);
			}
		}
	}

	public override void OnChartDataPointValuesBegin(ChartDataPointValues dataPointValues, bool output, bool oldDataPointsGeneration, ChartSeriesType type, ChartSeriesSubtype subType, string parentScopeName)
	{
		bool hasStart = dataPointValues.Start != null;
		WriteDataPointValueColumn(dataPointValues.X, GetDataValuesColumnName("X", DataPointValueProperties.X, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		WriteDataPointValueColumn(dataPointValues.Y, GetDataValuesColumnName("Y", DataPointValueProperties.Y, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		WriteDataPointValueColumn(dataPointValues.Size, GetDataValuesColumnName("Size", DataPointValueProperties.Size, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		WriteDataPointValueColumn(dataPointValues.High, GetDataValuesColumnName("High", DataPointValueProperties.High, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		WriteDataPointValueColumn(dataPointValues.Low, GetDataValuesColumnName("Low", DataPointValueProperties.Low, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		WriteDataPointValueColumn(dataPointValues.Start, GetDataValuesColumnName("Start", DataPointValueProperties.Start, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		WriteDataPointValueColumn(dataPointValues.End, GetDataValuesColumnName("End", DataPointValueProperties.End, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		WriteDataPointValueColumn(dataPointValues.Mean, GetDataValuesColumnName("Mean", DataPointValueProperties.Mean, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
		WriteDataPointValueColumn(dataPointValues.Median, GetDataValuesColumnName("Median", DataPointValueProperties.Median, oldDataPointsGeneration, type, subType, hasStart, parentScopeName));
	}

	private void WriteDataPointValueColumn(ReportVariantProperty dataPointValue, string colName)
	{
		if (dataPointValue != null)
		{
			m_columnsNum++;
			if (!m_noHeaders)
			{
				m_visitor.WriteValue(colName, m_excelMode);
			}
		}
	}

	public override void OnGaugeInputValueBegin(GaugeInputValue gaugeInputValue, string defaultGaugeInputValueColName, bool output, object value, ref bool walkThisGaugeInputValue)
	{
		base.OnGaugeInputValueBegin(gaugeInputValue, defaultGaugeInputValueColName, output, value, ref walkThisGaugeInputValue);
		if (!walkThisGaugeInputValue)
		{
			return;
		}
		m_columnsNum++;
		if (!m_noHeaders)
		{
			string text = gaugeInputValue.DataElementName;
			if (string.IsNullOrEmpty(text))
			{
				text = defaultGaugeInputValueColName;
			}
			m_visitor.WriteValue(text, m_excelMode);
		}
	}

	public override void OnMapMemberBegin(MapMember mapMember, MapSpatialElementTemplate template, bool outputMapMemberLabel, bool outputDynamicMembers, MapVectorLayer layer, ref bool walkMapMember)
	{
		base.OnMapMemberBegin(mapMember, template, outputMapMemberLabel, outputDynamicMembers, layer, ref walkMapMember);
		if (outputMapMemberLabel)
		{
			m_columnsNum++;
			if (!m_noHeaders)
			{
				string layerName = GetLayerName(layer);
				m_visitor.WriteValue(GetMapMemberColumnName(template, layerName), m_excelMode);
			}
		}
	}

	public override void OnMapAppearanceRuleBegin(MapAppearanceRule mapRule, bool outputMapRule, ref int index, ref bool walkRule)
	{
		base.OnMapAppearanceRuleBegin(mapRule, outputMapRule, ref index, ref walkRule);
		if (!walkRule)
		{
			return;
		}
		m_columnsNum++;
		if (m_noHeaders)
		{
			return;
		}
		string ruleValue = string.Empty;
		if (GetMapRuleDataValue(mapRule, out ruleValue))
		{
			string text = mapRule.DataElementName;
			if (string.IsNullOrEmpty(text))
			{
				text = GetMapRuleColName(index);
				index++;
			}
			m_visitor.WriteValue(text, m_excelMode);
		}
	}

	public override void OnStateIndicatorStateNameBegin(StateIndicator stateIndicator, string defaultStateIndicatorStateNameColName, bool output, ref bool walkThisStateName)
	{
		base.OnStateIndicatorStateNameBegin(stateIndicator, defaultStateIndicatorStateNameColName, output, ref walkThisStateName);
		if (!walkThisStateName)
		{
			return;
		}
		m_columnsNum++;
		if (!m_noHeaders)
		{
			string text = stateIndicator.StateDataElementName;
			if (string.IsNullOrEmpty(text))
			{
				text = defaultStateIndicatorStateNameColName;
			}
			m_visitor.WriteValue(text, m_excelMode);
		}
	}

	public override void OnRowEnd()
	{
		if (m_columnsNum != 0 && !m_noHeaders)
		{
			m_visitor.EndRow();
		}
	}
}
