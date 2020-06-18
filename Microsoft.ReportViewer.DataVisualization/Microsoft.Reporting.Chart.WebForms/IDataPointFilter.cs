namespace Microsoft.Reporting.Chart.WebForms
{
	internal interface IDataPointFilter
	{
		bool FilterDataPoint(DataPoint point, Series series, int pointIndex);
	}
}
