using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MultiChartInstanceList : ArrayList
	{
		internal new MultiChartInstance this[int index] => (MultiChartInstance)base[index];

		internal MultiChartInstanceList()
		{
		}

		internal MultiChartInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
