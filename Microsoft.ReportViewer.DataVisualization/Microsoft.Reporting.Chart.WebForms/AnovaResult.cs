namespace Microsoft.Reporting.Chart.WebForms
{
	internal class AnovaResult
	{
		internal double sumOfSquaresBetweenGroups;

		internal double sumOfSquaresWithinGroups;

		internal double sumOfSquaresTotal;

		internal double degreeOfFreedomBetweenGroups;

		internal double degreeOfFreedomWithinGroups;

		internal double degreeOfFreedomTotal;

		internal double meanSquareVarianceBetweenGroups;

		internal double meanSquareVarianceWithinGroups;

		internal double fRatio;

		internal double fCriticalValue;

		public double SumOfSquaresBetweenGroups => sumOfSquaresBetweenGroups;

		public double SumOfSquaresWithinGroups => sumOfSquaresWithinGroups;

		public double SumOfSquaresTotal => sumOfSquaresTotal;

		public double DegreeOfFreedomBetweenGroups => degreeOfFreedomBetweenGroups;

		public double DegreeOfFreedomWithinGroups => degreeOfFreedomWithinGroups;

		public double DegreeOfFreedomTotal => degreeOfFreedomTotal;

		public double MeanSquareVarianceBetweenGroups => meanSquareVarianceBetweenGroups;

		public double MeanSquareVarianceWithinGroups => meanSquareVarianceWithinGroups;

		public double FRatio => fRatio;

		public double FCriticalValue => fCriticalValue;
	}
}
