namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal struct Pair<T, U>
	{
		internal T First;

		internal U Second;

		internal Pair(T first, U second)
		{
			First = first;
			Second = second;
		}
	}
}
