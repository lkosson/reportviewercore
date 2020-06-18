using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataPointInstanceList : ArrayList
	{
		internal new ChartDataPointInstance this[int index] => (ChartDataPointInstance)base[index];

		internal ChartDataPointInstanceList()
		{
		}

		internal ChartDataPointInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
