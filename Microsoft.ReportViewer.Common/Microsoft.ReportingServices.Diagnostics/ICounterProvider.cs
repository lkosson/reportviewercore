namespace Microsoft.ReportingServices.Diagnostics
{
	internal interface ICounterProvider
	{
		ICounter GetCounterNumberOfItems(string categoryName, string counterName, bool resetCounter);

		ICounter GetCounterRatePerSecond(string categoryName, string counterNameTotal, string counterNamePerSecond, bool resetCounter);

		ICounter GetCounterAverageCount(string categoryName, string counterNameAverage, string counterNameBase, bool resetCounter);
	}
}
