using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal interface IChartHandler
{
	void OnChartBegin(Chart chart, bool outputChart, ref bool walkChildren);

	void OnChartEnd(Chart chart);

	void OnChartMemberBegin(ChartMember chartMember, bool outputThisMember, bool outputMemberLabelColumn, string parentScopeName, ref bool walkThisMember);

	void OnChartMemberEnd(ChartMember chartMember);

	void OnChartDataPointBegin(ChartDataPoint dataPoint, ref bool walkDataPoint);

	void OnChartDataPointEnd(ChartDataPoint dataPoint);

	void OnChartDataPointValuesBegin(ChartDataPointValues dataPointValues, bool output, bool oldDataPointsGeneration, ChartSeriesType type, ChartSeriesSubtype subType, string parentScopeName);

	void OnChartDataPointValuesEnd(ChartDataPointValues dataPointValues);
}
