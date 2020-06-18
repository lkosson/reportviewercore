using System;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataCellList : CellList
	{
		internal new DataCell this[int index] => (DataCell)base[index];

		public DataCellList()
		{
		}

		internal DataCellList(int capacity)
			: base(capacity)
		{
		}
	}
}
