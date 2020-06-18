namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class GaugeRowList : RowList
	{
		internal new GaugeRow this[int index] => (GaugeRow)base[index];

		public GaugeRowList()
		{
		}

		internal GaugeRowList(int capacity)
			: base(capacity)
		{
		}
	}
}
