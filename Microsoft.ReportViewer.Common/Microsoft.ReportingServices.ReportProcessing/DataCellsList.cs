using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataCellsList : ArrayList
	{
		internal new DataCellList this[int index] => (DataCellList)base[index];

		internal DataCellsList()
		{
		}

		internal DataCellsList(int capacity)
			: base(capacity)
		{
		}
	}
}
