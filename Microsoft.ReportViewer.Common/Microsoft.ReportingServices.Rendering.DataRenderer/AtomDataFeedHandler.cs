using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal class AtomDataFeedHandler : AtomDataFeedHandlerBase
{
	internal AtomDataFeedHandler(AtomDataFeedVisitor visitor, List<string> definitionPathSteps)
		: base(visitor, definitionPathSteps)
	{
	}

	public override void OnTextBoxBegin(TextBox textBox, bool output, ref bool walkTextBox)
	{
		if (textBox.DataElementOutput == DataElementOutputTypes.NoOutput)
		{
			return;
		}
		walkTextBox = true;
		string value = null;
		TypeCode typeCode = TypeCode.Empty;
		if (output)
		{
			object obj = null;
			if (textBox.Instance != null)
			{
				TextBoxInstance textBoxInstance = (TextBoxInstance)textBox.Instance;
				obj = textBoxInstance.OriginalValue;
			}
			if (obj != null)
			{
				value = XmlConvertOrginalValue(obj);
				typeCode = Type.GetTypeCode(obj.GetType());
			}
		}
		m_visitor.WriteValue(value, typeCode);
	}

	public static string XmlConvertOrginalValue(object value)
	{
		if (value is string)
		{
			return (string)value;
		}
		if (value is DateTime dateTime)
		{
			return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.RoundtripKind);
		}
		if (value is TimeSpan timeSpan)
		{
			return XmlConvert.ToString(timeSpan);
		}
		if (value is IFormattable)
		{
			IFormattable formattable = (IFormattable)value;
			return formattable.ToString(null, CultureInfo.InvariantCulture);
		}
		return string.Format(CultureInfo.InvariantCulture, "{0}", value);
	}

	public override void OnChartMemberBegin(ChartMember chartMember, bool outputThisMember, bool outputMemberLabelColumn, string parentScopeName, ref bool walkThisMember)
	{
		bool flag = walkThisMember;
		if (!flag)
		{
			base.OnChartMemberBegin(chartMember, outputThisMember, outputMemberLabelColumn, parentScopeName, ref walkThisMember);
		}
		if (!walkThisMember)
		{
			return;
		}
		if (chartMember.Group != null && chartMember.IsCategory)
		{
			if (!flag)
			{
				walkThisMember = IsPartOfDefinitionPathSteps(chartMember.DefinitionPath);
			}
			ValidateCurrentRow(walkThisMember, outputThisMember);
		}
		if (!walkThisMember || !outputMemberLabelColumn)
		{
			return;
		}
		string value = null;
		TypeCode typeCode = TypeCode.String;
		if (outputThisMember)
		{
			ChartMemberInstance instance = chartMember.Instance;
			if (instance != null && instance.Label != null)
			{
				value = instance.Label;
				typeCode = instance.Label.GetTypeCode();
			}
		}
		m_visitor.WriteValue(value, typeCode);
	}

	public override void OnChartDataPointValuesBegin(ChartDataPointValues dataPointValues, bool output, bool oldDataPointsGeneration, ChartSeriesType type, ChartSeriesSubtype subType, string parentScopeName)
	{
		ChartDataPointValuesInstance instance = dataPointValues.Instance;
		string value = null;
		TypeCode typeCode = TypeCode.String;
		if (dataPointValues.X != null)
		{
			if (output && instance != null && instance.X != null)
			{
				value = XmlConvertOrginalValue(instance.X);
				typeCode = Type.GetTypeCode(instance.X.GetType());
			}
			m_visitor.WriteValue(value, typeCode);
		}
		if (dataPointValues.Y != null)
		{
			if (output && instance != null && instance.Y != null)
			{
				value = XmlConvertOrginalValue(instance.Y);
				typeCode = Type.GetTypeCode(instance.Y.GetType());
			}
			m_visitor.WriteValue(value, typeCode);
		}
		if (dataPointValues.Size != null)
		{
			if (output && instance != null && instance.Size != null)
			{
				value = XmlConvertOrginalValue(instance.Size);
				typeCode = Type.GetTypeCode(instance.Size.GetType());
			}
			m_visitor.WriteValue(value, typeCode);
		}
		if (dataPointValues.High != null)
		{
			if (output && instance != null && instance.High != null)
			{
				value = XmlConvertOrginalValue(instance.High);
				typeCode = Type.GetTypeCode(instance.High.GetType());
			}
			m_visitor.WriteValue(value, typeCode);
		}
		if (dataPointValues.Low != null)
		{
			if (output && instance != null && instance.Low != null)
			{
				value = XmlConvertOrginalValue(instance.Low);
				typeCode = Type.GetTypeCode(instance.Low.GetType());
			}
			m_visitor.WriteValue(value, typeCode);
		}
		if (dataPointValues.Start != null)
		{
			if (output && instance != null && instance.Start != null)
			{
				value = XmlConvertOrginalValue(instance.Start);
				typeCode = Type.GetTypeCode(instance.Start.GetType());
			}
			m_visitor.WriteValue(value, typeCode);
		}
		if (dataPointValues.End != null)
		{
			if (output && instance != null && instance.End != null)
			{
				value = XmlConvertOrginalValue(instance.End);
				typeCode = Type.GetTypeCode(instance.End.GetType());
			}
			m_visitor.WriteValue(value, typeCode);
		}
		if (dataPointValues.Mean != null)
		{
			if (output && instance != null && instance.Mean != null)
			{
				value = XmlConvertOrginalValue(instance.Mean);
				typeCode = Type.GetTypeCode(instance.Mean.GetType());
			}
			m_visitor.WriteValue(value, typeCode);
		}
		if (dataPointValues.Median != null)
		{
			if (output && instance != null && instance.Median != null)
			{
				value = XmlConvertOrginalValue(instance.Median);
				typeCode = Type.GetTypeCode(instance.Median.GetType());
			}
			m_visitor.WriteValue(value, typeCode);
		}
	}

	public override void OnGaugeInputValueBegin(GaugeInputValue gaugeInputValue, string defaultGaugeInputValueColName, bool output, object value, ref bool walkThisGaugeInputValue)
	{
		base.OnGaugeInputValueBegin(gaugeInputValue, defaultGaugeInputValueColName, output, value, ref walkThisGaugeInputValue);
		if (!walkThisGaugeInputValue)
		{
			return;
		}
		string value2 = string.Empty;
		TypeCode typeCode = TypeCode.String;
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
				value2 = XmlConvertOrginalValue(value);
				typeCode = Type.GetTypeCode(value.GetType());
			}
		}
		m_visitor.WriteValue(value2, typeCode);
	}

	public override void OnMapMemberBegin(MapMember mapMember, MapSpatialElementTemplate template, bool outputMapMemberLabel, bool outputDynamicMembers, MapVectorLayer layer, ref bool walkMapMember)
	{
		base.OnMapMemberBegin(mapMember, template, outputMapMemberLabel, outputDynamicMembers, layer, ref walkMapMember);
		if (mapMember.Group != null)
		{
			walkMapMember = IsPartOfDefinitionPathSteps(mapMember.DefinitionPath);
			ValidateCurrentRow(walkMapMember, outputDynamicMembers);
		}
		if (!walkMapMember || !outputMapMemberLabel)
		{
			return;
		}
		string text = null;
		TypeCode typeCode = TypeCode.String;
		if (outputDynamicMembers)
		{
			text = GetDataElementLabelValue(template);
			if (text != null)
			{
				typeCode = text.GetTypeCode();
			}
		}
		m_visitor.WriteValue(text, typeCode);
	}

	public override void OnMapAppearanceRuleBegin(MapAppearanceRule mapRule, bool outputMapRule, ref int index, ref bool walkRule)
	{
		base.OnMapAppearanceRuleBegin(mapRule, outputMapRule, ref index, ref walkRule);
		if (walkRule)
		{
			string ruleValue = string.Empty;
			TypeCode typeCode = TypeCode.String;
			if (!outputMapRule || GetMapRuleDataValue(mapRule, out ruleValue))
			{
				m_visitor.WriteValue(ruleValue, typeCode);
			}
		}
	}

	public override void OnStateIndicatorStateNameBegin(StateIndicator stateIndicator, string defaultStateIndicatorStateNameColName, bool output, ref bool walkThisStateName)
	{
		base.OnStateIndicatorStateNameBegin(stateIndicator, defaultStateIndicatorStateNameColName, output, ref walkThisStateName);
		if (walkThisStateName)
		{
			string value = string.Empty;
			if (output && stateIndicator.CompiledStateName != null)
			{
				value = XmlConvertOrginalValue(stateIndicator.CompiledStateName);
			}
			m_visitor.WriteValue(value, TypeCode.String);
		}
	}
}
