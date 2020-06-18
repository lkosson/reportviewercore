namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class GaugeCellList : CellList
	{
		internal new GaugeCell this[int index] => (GaugeCell)base[index];
	}
}
