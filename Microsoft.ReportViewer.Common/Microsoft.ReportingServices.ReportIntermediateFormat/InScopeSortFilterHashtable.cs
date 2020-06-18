using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class InScopeSortFilterHashtable : Hashtable
	{
		internal List<int> this[int index] => (List<int>)base[index];

		public InScopeSortFilterHashtable()
		{
		}

		internal InScopeSortFilterHashtable(int capacity)
			: base(capacity)
		{
		}
	}
}
