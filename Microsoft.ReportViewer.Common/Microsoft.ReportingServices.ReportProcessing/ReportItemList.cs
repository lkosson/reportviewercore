using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportItemList : ArrayList
	{
		internal new ReportItem this[int index] => (ReportItem)base[index];

		internal ReportItemList()
		{
		}

		internal ReportItemList(int capacity)
			: base(capacity)
		{
		}
	}
}
