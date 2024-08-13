using System;
using System.Globalization;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal abstract class HandlerBase : IReportHandler, ITablixHandler, IOutputRowHandler, IChartHandler, IGaugePanelHandler, IMapHandler
{
	protected enum DataPointValueProperties
	{
		X,
		Y,
		Size,
		High,
		Low,
		Start,
		End,
		Mean,
		Median
	}

	private const string Label = "label";

	protected const string DataPointValuesX = "X";

	protected const string DataPointValuesY = "Y";

	protected const string DataPointValuesSize = "Size";

	protected const string DataPointValuesHigh = "High";

	protected const string DataPointValuesLow = "Low";

	protected const string DataPointValuesStart = "Start";

	protected const string DataPointValuesEnd = "End";

	protected const string DataPointValuesMean = "Mean";

	protected const string DataPointValuesMedian = "Median";

	protected const string DataValueColumnName = "DataValue{0}";

	protected const string MapDataValueColumnName = "Value{0}";

	private const string ConnectString = "_";

	public virtual bool Done => false;

	internal static string GetChartMemberColumnName(ChartMember chartMember)
	{
		string text = chartMember.DataElementName;
		if (chartMember.IsStatic)
		{
			if (string.IsNullOrEmpty(text) && chartMember.Label != null)
			{
				text = chartMember.Label.Value;
			}
		}
		else
		{
			text = chartMember.Group.DataElementName;
			if (string.IsNullOrEmpty(text) && chartMember.Group.GroupExpressions.Count > 0)
			{
				object value = chartMember.Group.GroupExpressions[0].Value;
				if (value != null)
				{
					text = string.Format(CultureInfo.InvariantCulture, "{0}", chartMember.Group.GroupExpressions[0].Value);
				}
			}
		}
		return text;
	}

	protected string GetDataValuesColumnName(string colName, DataPointValueProperties dataPointValueProperty, bool oldDataPointsGeneration, ChartSeriesType type, ChartSeriesSubtype subType, bool hasStart, string parentScopeName)
	{
		if (oldDataPointsGeneration)
		{
			int num = MapDataValueName(dataPointValueProperty, type, subType, hasStart);
			return parentScopeName + "_" + string.Format(CultureInfo.InvariantCulture, "DataValue{0}", num);
		}
		return parentScopeName + "_" + colName;
	}

	protected string GetDataElementLabelValue(MapSpatialElementTemplate template)
	{
		if (template == null)
		{
			return null;
		}
		ReportStringProperty dataElementLabel = template.DataElementLabel;
		string result = null;
		if (dataElementLabel != null)
		{
			if (dataElementLabel.IsExpression)
			{
				if (template.Instance != null)
				{
					result = template.Instance.DataElementLabel;
				}
			}
			else
			{
				result = dataElementLabel.Value;
			}
		}
		return result;
	}

	protected string GetMapMemberColumnName(MapSpatialElementTemplate template, string layerName)
	{
		string text = layerName;
		if (!string.IsNullOrEmpty(text))
		{
			text += "_";
		}
		if (template != null && !string.IsNullOrEmpty(template.DataElementName))
		{
			text += template.DataElementName;
			text += "_";
		}
		return text + "label";
	}

	protected string GetMapRuleColName(int index)
	{
		return string.Format(CultureInfo.InvariantCulture, "Value{0}", index);
	}

	protected string GetLayerName(MapVectorLayer layer)
	{
		string text = layer.DataElementName;
		if (string.IsNullOrEmpty(text))
		{
			text = layer.Name;
		}
		return text;
	}

	protected bool GetMapRuleDataValue(MapAppearanceRule mapRule, out string ruleValue)
	{
		ReportVariantProperty dataValue = mapRule.DataValue;
		object obj = null;
		ruleValue = string.Empty;
		if (dataValue != null)
		{
			if (dataValue.IsExpression)
			{
				if (mapRule.Instance != null)
				{
					obj = mapRule.Instance.DataValue;
				}
			}
			else
			{
				obj = dataValue.Value;
			}
		}
		if (obj != null)
		{
			ruleValue = string.Format(CultureInfo.InvariantCulture, "{0}", obj);
		}
		if (obj is string && ((string)obj).StartsWith("#", StringComparison.Ordinal))
		{
			return false;
		}
		return true;
	}

	private int MapDataValueName(DataPointValueProperties dataPointValueProperty, ChartSeriesType type, ChartSeriesSubtype subtype, bool hasStart)
	{
		int result = -1;
		switch (dataPointValueProperty)
		{
		case DataPointValueProperties.X:
			if (type == ChartSeriesType.Scatter || subtype == ChartSeriesSubtype.Bubble)
			{
				result = 0;
			}
			break;
		case DataPointValueProperties.Y:
			if (type == ChartSeriesType.Column || type == ChartSeriesType.Bar || type == ChartSeriesType.Line || type == ChartSeriesType.Area || type == ChartSeriesType.Shape)
			{
				result = 0;
			}
			else if (type == ChartSeriesType.Scatter || subtype == ChartSeriesSubtype.Bubble)
			{
				result = 1;
			}
			break;
		case DataPointValueProperties.Size:
			if (subtype == ChartSeriesSubtype.Bubble)
			{
				result = 2;
			}
			break;
		case DataPointValueProperties.High:
			if (subtype == ChartSeriesSubtype.Stock || subtype == ChartSeriesSubtype.Candlestick)
			{
				result = 0;
			}
			break;
		case DataPointValueProperties.Low:
			if (subtype == ChartSeriesSubtype.Stock || subtype == ChartSeriesSubtype.Candlestick)
			{
				result = 1;
			}
			break;
		case DataPointValueProperties.Start:
			if (subtype == ChartSeriesSubtype.Stock || subtype == ChartSeriesSubtype.Candlestick)
			{
				result = 2;
			}
			break;
		case DataPointValueProperties.End:
			if (subtype == ChartSeriesSubtype.Stock || subtype == ChartSeriesSubtype.Candlestick)
			{
				result = ((!hasStart) ? 2 : 3);
			}
			break;
		}
		return result;
	}

	public virtual void OnReportBegin(Report report, ref bool walkChildren)
	{
		walkChildren = true;
	}

	public virtual void OnReportEnd(Report report)
	{
	}

	public virtual void OnSubReportBegin(SubReport subReport, ref bool walkSubreport)
	{
		if (subReport.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkSubreport = true;
		}
	}

	public virtual void OnSubReportEnd(SubReport subReport)
	{
	}

	public virtual void OnRectangleBegin(Rectangle rectangle, ref bool walkChildren)
	{
		if (rectangle.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkChildren = true;
		}
	}

	public virtual void OnRectangleEnd(Rectangle rectangle)
	{
	}

	public virtual void OnTextBoxBegin(TextBox textBox, bool output, ref bool walkTextBox)
	{
	}

	public virtual void OnTextBoxEnd(TextBox textBox)
	{
	}

	public virtual void OnTopLevelDataRegionOrMap(ReportItem reportItem)
	{
	}

	public virtual void OnTablixBegin(Tablix tablix, ref bool walkTablix, bool outputTablix)
	{
		if (tablix.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkTablix = true;
		}
	}

	public virtual void OnTablixEnd(Tablix tablix)
	{
	}

	public virtual void OnTablixMemberBegin(TablixMember tablixMember, ref bool walkThisMember, bool outputThisMember)
	{
		if (tablixMember.DataElementOutput != DataElementOutputTypes.NoOutput && (tablixMember.Group == null || tablixMember.Group.DataElementOutput != DataElementOutputTypes.NoOutput))
		{
			walkThisMember = true;
		}
	}

	public virtual void OnTablixMemberEnd(TablixMember tablixMember)
	{
	}

	public virtual void OnTablixCellBegin(TablixCell cell, ref bool walkCell)
	{
		if (cell.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkCell = true;
		}
	}

	public virtual void OnTablixCellEnd(TablixCell cell)
	{
	}

	public virtual void OnChartBegin(Chart chart, bool outputChart, ref bool walkChart)
	{
		if (chart.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkChart = true;
		}
	}

	public virtual void OnChartEnd(Chart chart)
	{
	}

	public virtual void OnChartMemberBegin(ChartMember chartMember, bool outputThisMember, bool outputMemberLabelColumn, string parentScopeName, ref bool walkThisMember)
	{
		if (chartMember.DataElementOutput != DataElementOutputTypes.NoOutput && (chartMember.Group == null || chartMember.Group.DataElementOutput != DataElementOutputTypes.NoOutput))
		{
			walkThisMember = true;
		}
	}

	public virtual void OnChartMemberEnd(ChartMember chartMember)
	{
	}

	public virtual void OnChartDataPointBegin(ChartDataPoint dataPoint, ref bool walkDataPoint)
	{
		if (dataPoint.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkDataPoint = true;
		}
	}

	public virtual void OnChartDataPointEnd(ChartDataPoint dataPoint)
	{
	}

	public virtual void OnChartDataPointValuesBegin(ChartDataPointValues dataPointValues, bool output, bool oldDataPointsGeneration, ChartSeriesType type, ChartSeriesSubtype subType, string parentScopeName)
	{
	}

	public virtual void OnChartDataPointValuesEnd(ChartDataPointValues dataPointValues)
	{
	}

	public virtual void OnGaugePanelBegin(GaugePanel gaugePanel, bool outputGaugePanelData, ref bool walkGaugePanel)
	{
		if (gaugePanel.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkGaugePanel = true;
		}
	}

	public virtual void OnGaugePanelEnd(GaugePanel gaugePanel)
	{
	}

	public virtual void OnGaugeInputValueBegin(GaugeInputValue gaugeInputValue, string defaultGaugeInputValueColName, bool output, object value, ref bool walkThisGaugeInputValue)
	{
		if (gaugeInputValue.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkThisGaugeInputValue = true;
		}
	}

	public virtual void OnGaugeInputValueEnd(GaugeInputValue gaugeInputValue)
	{
	}

	public virtual void OnMapBegin(Map map, bool outputMap, ref bool walkMap)
	{
		if (map.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkMap = true;
		}
	}

	public virtual void OnMapEnd(Map map)
	{
	}

	public virtual void OnMapVectorLayerBegin(MapVectorLayer layer, bool outputLayer, ref bool walkLayer)
	{
		if (layer.DataElementOutput != DataElementOutputTypes.NoOutput && layer.MapDataRegion != null)
		{
			walkLayer = true;
		}
	}

	public virtual void OnMapVectorLayerEnd(MapVectorLayer layer)
	{
	}

	public virtual void OnMapMemberBegin(MapMember mapMember, MapSpatialElementTemplate template, bool outputMapMemberLabel, bool outputDynamicMembers, MapVectorLayer layer, ref bool walkMapMember)
	{
	}

	public virtual void OnMapMemberEnd(MapMember mapMember)
	{
	}

	public virtual void OnMapAppearanceRuleBegin(MapAppearanceRule mapRule, bool outputMapRule, ref int index, ref bool walkRule)
	{
		if (mapRule != null && mapRule.DataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkRule = true;
		}
	}

	public virtual void OnMapAppearanceRuleEnd(MapAppearanceRule mapRule)
	{
	}

	public virtual void OnStateIndicatorStateNameBegin(StateIndicator stateIndicator, string defaultStateIndicatorStateNameColName, bool output, ref bool walkThisStateName)
	{
		if (stateIndicator.StateDataElementOutput != DataElementOutputTypes.NoOutput)
		{
			walkThisStateName = true;
		}
	}

	public virtual void OnStateIndicatorStateNameEnd(StateIndicator stateIndicator)
	{
	}

	public virtual void OnRowBegin()
	{
	}

	public virtual void OnRowEnd()
	{
	}

	public virtual void OnRegionBegin()
	{
	}

	public virtual void OnRegionEnd()
	{
	}
}
