namespace Microsoft.ReportingServices.ReportProcessing
{
	internal class PairObj<T, U>
	{
		internal T First;

		internal U Second;

		internal PairObj(T first, U second)
		{
			First = first;
			Second = second;
		}
	}
}
