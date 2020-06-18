using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableColumnList : ArrayList
	{
		internal new TableColumn this[int index] => (TableColumn)base[index];

		internal TableColumnList()
		{
		}

		internal TableColumnList(int capacity)
			: base(capacity)
		{
		}
	}
}
