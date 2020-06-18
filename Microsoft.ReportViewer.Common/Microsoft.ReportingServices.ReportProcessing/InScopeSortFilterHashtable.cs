using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class InScopeSortFilterHashtable : Hashtable
	{
		internal IntList this[int index] => (IntList)base[index];

		internal InScopeSortFilterHashtable()
		{
		}

		internal InScopeSortFilterHashtable(int capacity)
			: base(capacity)
		{
		}
	}
}
