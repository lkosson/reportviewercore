namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal interface ICircularChartType
	{
		bool RequireClosedFigure();

		bool XAxisCrossingSupported();

		bool XAxisLabelsSupported();

		bool RadialGridLinesSupported();

		int GetNumerOfSectors(ChartArea area, SeriesCollection seriesCollection);

		float[] GetYAxisLocations(ChartArea area);
	}
}
