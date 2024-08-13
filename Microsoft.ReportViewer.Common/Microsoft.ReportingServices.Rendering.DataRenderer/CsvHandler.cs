using System.Globalization;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class CsvHandler : HandlerBase
{
	private CsvVisitor m_visitor;

	private bool m_stackedMode;

	private int m_currentColumn;

	private bool m_useFormattedValues;

	internal int Columns => m_currentColumn;

	public CsvHandler(CsvVisitor csvVisitor, bool stackedMode, bool useFormattedValues)
	{
		m_visitor = csvVisitor;
		m_stackedMode = stackedMode;
		m_useFormattedValues = useFormattedValues;
	}

	public override void OnChartMemberBegin(ChartMember chartMember, bool outputThisMember, bool outputMemberLabelColumn, string parentScopeName, ref bool walkThisMember)
	{
		base.OnChartMemberBegin(chartMember, outputThisMember, outputMemberLabelColumn, parentScopeName, ref walkThisMember);
		if (!walkThisMember || !outputMemberLabelColumn)
		{
			return;
		}
		m_currentColumn++;
		string unformattedValue = string.Empty;
		if (outputThisMember)
		{
			ChartMemberInstance instance = chartMember.Instance;
			if (instance != null)
			{
				unformattedValue = instance.Label;
			}
		}
		m_visitor.WriteValue(unformattedValue, m_stackedMode);
	}

	public override void OnChartDataPointValuesBegin(ChartDataPointValues dataPointValues, bool output, bool oldDataPointsGeneration, ChartSeriesType type, ChartSeriesSubtype subType, string parentScopeName)
	{
		ChartDataPointValuesInstance instance = dataPointValues.Instance;
		string unformattedValue = string.Empty;
		if (dataPointValues.X != null)
		{
			m_currentColumn++;
			if (output && instance != null && instance.X != null)
			{
				unformattedValue = string.Format(CultureInfo.InvariantCulture, "{0}", instance.X);
			}
			m_visitor.WriteValue(unformattedValue, m_stackedMode);
		}
		if (dataPointValues.Y != null)
		{
			m_currentColumn++;
			if (output && instance != null && instance.Y != null)
			{
				unformattedValue = string.Format(CultureInfo.InvariantCulture, "{0}", instance.Y);
			}
			m_visitor.WriteValue(unformattedValue, m_stackedMode);
		}
		if (dataPointValues.Size != null)
		{
			m_currentColumn++;
			if (output && instance != null && instance.Size != null)
			{
				unformattedValue = string.Format(CultureInfo.InvariantCulture, "{0}", instance.Size);
			}
			m_visitor.WriteValue(unformattedValue, m_stackedMode);
		}
		if (dataPointValues.High != null)
		{
			m_currentColumn++;
			if (output && instance != null && instance.High != null)
			{
				unformattedValue = string.Format(CultureInfo.InvariantCulture, "{0}", instance.High);
			}
			m_visitor.WriteValue(unformattedValue, m_stackedMode);
		}
		if (dataPointValues.Low != null)
		{
			m_currentColumn++;
			if (output && instance != null && instance.Low != null)
			{
				unformattedValue = string.Format(CultureInfo.InvariantCulture, "{0}", instance.Low);
			}
			m_visitor.WriteValue(unformattedValue, m_stackedMode);
		}
		if (dataPointValues.Start != null)
		{
			m_currentColumn++;
			if (output && instance != null && instance.Start != null)
			{
				unformattedValue = string.Format(CultureInfo.InvariantCulture, "{0}", instance.Start);
			}
			m_visitor.WriteValue(unformattedValue, m_stackedMode);
		}
		if (dataPointValues.End != null)
		{
			m_currentColumn++;
			if (output && instance != null && instance.End != null)
			{
				unformattedValue = string.Format(CultureInfo.InvariantCulture, "{0}", instance.End);
			}
			m_visitor.WriteValue(unformattedValue, m_stackedMode);
		}
		if (dataPointValues.Mean != null)
		{
			m_currentColumn++;
			if (output && instance != null && instance.Mean != null)
			{
				unformattedValue = string.Format(CultureInfo.InvariantCulture, "{0}", instance.Mean);
			}
			m_visitor.WriteValue(unformattedValue, m_stackedMode);
		}
		if (dataPointValues.Median != null)
		{
			m_currentColumn++;
			if (output && instance != null && instance.Median != null)
			{
				unformattedValue = string.Format(CultureInfo.InvariantCulture, "{0}", instance.Median);
			}
			m_visitor.WriteValue(unformattedValue, m_stackedMode);
		}
	}

	public override void OnTextBoxBegin(TextBox textBox, bool output, ref bool walkTextBox)
	{
		if (textBox.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			return;
		}
		walkTextBox = true;
		m_currentColumn++;
		string unformattedValue = string.Empty;
		if (output)
		{
			if (m_useFormattedValues)
			{
				if (textBox.Instance != null)
				{
					unformattedValue = ((TextBoxInstance)textBox.Instance).Value;
				}
			}
			else
			{
				object obj = null;
				if (textBox.Instance != null)
				{
					obj = ((TextBoxInstance)textBox.Instance).OriginalValue;
				}
				if (obj != null)
				{
					unformattedValue = string.Format(CultureInfo.InvariantCulture, "{0}", obj);
				}
			}
		}
		m_visitor.WriteValue(unformattedValue, m_stackedMode);
	}

	public override void OnGaugeInputValueBegin(GaugeInputValue gaugeInputValue, string defaultGaugeInputValueColName, bool output, object value, ref bool walkThisGaugeInputValue)
	{
		base.OnGaugeInputValueBegin(gaugeInputValue, defaultGaugeInputValueColName, output, value, ref walkThisGaugeInputValue);
		if (!walkThisGaugeInputValue)
		{
			return;
		}
		m_currentColumn++;
		string unformattedValue = string.Empty;
		if (output)
		{
			if (value == null)
			{
				if (gaugeInputValue.CompiledInstance != null && gaugeInputValue.CompiledInstance.Value != null)
				{
					value = gaugeInputValue.CompiledInstance.Value;
				}
				else if (gaugeInputValue.Instance != null && gaugeInputValue.Instance.Value != null)
				{
					value = gaugeInputValue.Instance.Value;
				}
			}
			if (value != null)
			{
				unformattedValue = string.Format(CultureInfo.InvariantCulture, "{0}", value);
			}
		}
		m_visitor.WriteValue(unformattedValue, m_stackedMode);
	}

	public override void OnMapMemberBegin(MapMember mapMember, MapSpatialElementTemplate template, bool outputMapMemberLabel, bool outputDynamicMembers, MapVectorLayer layer, ref bool walkMapMember)
	{
		base.OnMapMemberBegin(mapMember, template, outputMapMemberLabel, outputDynamicMembers, layer, ref walkMapMember);
		if (outputMapMemberLabel)
		{
			m_currentColumn++;
			string unformattedValue = string.Empty;
			if (outputDynamicMembers)
			{
				unformattedValue = GetDataElementLabelValue(template);
			}
			m_visitor.WriteValue(unformattedValue, m_stackedMode);
		}
	}

	public override void OnMapAppearanceRuleBegin(MapAppearanceRule mapRule, bool outputMapRule, ref int index, ref bool walkRule)
	{
		base.OnMapAppearanceRuleBegin(mapRule, outputMapRule, ref index, ref walkRule);
		if (walkRule)
		{
			m_currentColumn++;
			string ruleValue = string.Empty;
			if (!outputMapRule || GetMapRuleDataValue(mapRule, out ruleValue))
			{
				m_visitor.WriteValue(ruleValue, m_stackedMode);
			}
		}
	}

	public override void OnStateIndicatorStateNameBegin(StateIndicator stateIndicator, string defaultStateIndicatorStateNameColName, bool output, ref bool walkThisStateName)
	{
		base.OnStateIndicatorStateNameBegin(stateIndicator, defaultStateIndicatorStateNameColName, output, ref walkThisStateName);
		if (walkThisStateName)
		{
			m_currentColumn++;
			string unformattedValue = string.Empty;
			if (output && stateIndicator.CompiledStateName != null)
			{
				unformattedValue = string.Format(CultureInfo.InvariantCulture, "{0}", stateIndicator.CompiledStateName);
			}
			m_visitor.WriteValue(unformattedValue, m_stackedMode);
		}
	}

	public override void OnRowEnd()
	{
		if (m_currentColumn != 0)
		{
			m_visitor.EndRow();
		}
	}

	public override void OnRegionEnd()
	{
		if (m_stackedMode)
		{
			m_visitor.EndRegion();
		}
	}
}
