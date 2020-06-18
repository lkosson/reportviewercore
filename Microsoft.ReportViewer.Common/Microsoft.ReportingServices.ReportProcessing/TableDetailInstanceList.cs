using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableDetailInstanceList : ArrayList
	{
		internal new TableDetailInstance this[int index] => (TableDetailInstance)base[index];

		internal TableDetailInstanceList()
		{
		}

		internal TableDetailInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
