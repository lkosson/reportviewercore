using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataAggregateInfoList : ArrayList
	{
		internal new DataAggregateInfo this[int index] => (DataAggregateInfo)base[index];

		internal DataAggregateInfoList()
		{
		}

		internal DataAggregateInfoList(int capacity)
			: base(capacity)
		{
		}
	}
}
