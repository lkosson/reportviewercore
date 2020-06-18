using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataPointInstancesList : ArrayList
	{
		internal new ChartDataPointInstanceList this[int index] => (ChartDataPointInstanceList)base[index];

		internal ChartDataPointInstancesList()
		{
		}

		internal ChartDataPointInstancesList(int capacity)
			: base(capacity)
		{
		}
	}
}
