using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportItemIndexerList : ArrayList
	{
		internal new ReportItemIndexer this[int index] => (ReportItemIndexer)base[index];

		internal ReportItemIndexerList()
		{
		}

		internal ReportItemIndexerList(int capacity)
			: base(capacity)
		{
		}
	}
}
