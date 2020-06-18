using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataCellList : ArrayList
	{
		internal new DataValueCRIList this[int index] => (DataValueCRIList)base[index];

		internal DataCellList()
		{
		}

		internal DataCellList(int capacity)
			: base(capacity)
		{
		}
	}
}
