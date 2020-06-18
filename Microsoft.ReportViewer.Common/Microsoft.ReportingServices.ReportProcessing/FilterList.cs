using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class FilterList : ArrayList
	{
		internal new Filter this[int index] => (Filter)base[index];

		internal FilterList()
		{
		}

		internal FilterList(int capacity)
			: base(capacity)
		{
		}
	}
}
