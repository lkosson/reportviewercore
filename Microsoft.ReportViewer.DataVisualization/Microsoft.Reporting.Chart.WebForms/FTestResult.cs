namespace Microsoft.Reporting.Chart.WebForms
{
	internal class FTestResult
	{
		internal double firstSeriesMean;

		internal double secondSeriesMean;

		internal double firstSeriesVariance;

		internal double secondSeriesVariance;

		internal double fValue;

		internal double probabilityFOneTail;

		internal double fCriticalValueOneTail;

		public double FirstSeriesMean => firstSeriesMean;

		public double SecondSeriesMean => secondSeriesMean;

		public double FirstSeriesVariance => firstSeriesVariance;

		public double SecondSeriesVariance => secondSeriesVariance;

		public double FValue => fValue;

		public double ProbabilityFOneTail => probabilityFOneTail;

		public double FCriticalValueOneTail => fCriticalValueOneTail;
	}
}
