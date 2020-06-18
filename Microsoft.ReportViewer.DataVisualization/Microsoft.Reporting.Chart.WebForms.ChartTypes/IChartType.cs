using System.Collections;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal interface IChartType
	{
		string Name
		{
			get;
		}

		bool Stacked
		{
			get;
		}

		bool SupportStackedGroups
		{
			get;
		}

		bool StackSign
		{
			get;
		}

		bool RequireAxes
		{
			get;
		}

		bool CircularChartArea
		{
			get;
		}

		bool SupportLogarithmicAxes
		{
			get;
		}

		bool SwitchValueAxes
		{
			get;
		}

		bool SideBySideSeries
		{
			get;
		}

		bool DataPointsInLegend
		{
			get;
		}

		bool ApplyPaletteColorsToPoints
		{
			get;
		}

		bool ExtraYValuesConnectedToYAxis
		{
			get;
		}

		bool ZeroCrossing
		{
			get;
		}

		int YValuesPerPoint
		{
			get;
		}

		bool SecondYScale
		{
			get;
		}

		bool HundredPercent
		{
			get;
		}

		bool HundredPercentSupportNegative
		{
			get;
		}

		Image GetImage(ChartTypeRegistry registry);

		LegendImageStyle GetLegendImageStyle(Series series);

		void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw);

		double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex);

		void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list);
	}
}
