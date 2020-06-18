using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableGroupInstanceList : ArrayList
	{
		internal new TableGroupInstance this[int index] => (TableGroupInstance)base[index];

		internal TableGroupInstanceList()
		{
		}

		internal TableGroupInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
