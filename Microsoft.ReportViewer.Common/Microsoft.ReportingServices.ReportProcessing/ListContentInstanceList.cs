using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ListContentInstanceList : ArrayList
	{
		internal new ListContentInstance this[int index] => (ListContentInstance)base[index];

		internal ListContentInstanceList()
		{
		}

		internal ListContentInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
