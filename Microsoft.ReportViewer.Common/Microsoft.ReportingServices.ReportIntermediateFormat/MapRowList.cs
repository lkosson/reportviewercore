namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class MapRowList : RowList
	{
		internal new MapRow this[int index] => (MapRow)base[index];

		public MapRowList()
		{
		}

		internal MapRowList(int capacity)
			: base(capacity)
		{
		}
	}
}
