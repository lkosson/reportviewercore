namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal static class FlagUtils
	{
		public static bool HasFlag(DataActions value, DataActions flagToTest)
		{
			return (value & flagToTest) != 0;
		}

		public static bool HasFlag(AggregateUpdateFlags value, AggregateUpdateFlags flagToTest)
		{
			return (value & flagToTest) != 0;
		}
	}
}
