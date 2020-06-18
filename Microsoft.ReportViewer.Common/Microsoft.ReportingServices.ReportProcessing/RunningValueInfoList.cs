using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RunningValueInfoList : ArrayList
	{
		internal new RunningValueInfo this[int index] => (RunningValueInfo)base[index];

		internal RunningValueInfoList()
		{
		}

		internal RunningValueInfoList(int capacity)
			: base(capacity)
		{
		}
	}
}
