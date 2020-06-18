using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportItemInstanceList : ArrayList
	{
		internal new ReportItemInstance this[int index] => (ReportItemInstance)base[index];

		internal ReportItemInstanceList()
		{
		}

		internal ReportItemInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
