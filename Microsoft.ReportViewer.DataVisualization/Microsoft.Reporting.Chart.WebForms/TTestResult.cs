namespace Microsoft.Reporting.Chart.WebForms
{
	internal class TTestResult
	{
		internal double firstSeriesMean;

		internal double secondSeriesMean;

		internal double firstSeriesVariance;

		internal double secondSeriesVariance;

		internal double tValue;

		internal double degreeOfFreedom;

		internal double probabilityTOneTail;

		internal double tCriticalValueOneTail;

		internal double probabilityTTwoTail;

		internal double tCriticalValueTwoTail;

		public double FirstSeriesMean => firstSeriesMean;

		public double SecondSeriesMean => secondSeriesMean;

		public double FirstSeriesVariance => firstSeriesVariance;

		public double SecondSeriesVariance => secondSeriesVariance;

		public double TValue => tValue;

		public double DegreeOfFreedom => degreeOfFreedom;

		public double ProbabilityTOneTail => probabilityTOneTail;

		public double TCriticalValueOneTail => tCriticalValueOneTail;

		public double ProbabilityTTwoTail => probabilityTTwoTail;

		public double TCriticalValueTwoTail => tCriticalValueTwoTail;
	}
}
