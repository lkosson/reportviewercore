namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records
{
	internal struct Pair<T, U>
	{
		private T m_first;

		private U m_second;

		internal T First => m_first;

		internal U Second => m_second;

		internal Pair(T first, U second)
		{
			m_first = first;
			m_second = second;
		}
	}
}
