namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ZTestResult
	{
		internal double firstSeriesMean;

		internal double secondSeriesMean;

		internal double firstSeriesVariance;

		internal double secondSeriesVariance;

		internal double zValue;

		internal double probabilityZOneTail;

		internal double zCriticalValueOneTail;

		internal double probabilityZTwoTail;

		internal double zCriticalValueTwoTail;

		public double FirstSeriesMean => firstSeriesMean;

		public double SecondSeriesMean => secondSeriesMean;

		public double FirstSeriesVariance => firstSeriesVariance;

		public double SecondSeriesVariance => secondSeriesVariance;

		public double ZValue => zValue;

		public double ProbabilityZOneTail => probabilityZOneTail;

		public double ZCriticalValueOneTail => zCriticalValueOneTail;

		public double ProbabilityZTwoTail => probabilityZTwoTail;

		public double ZCriticalValueTwoTail => zCriticalValueTwoTail;
	}
}
