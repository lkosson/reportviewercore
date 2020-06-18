using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class RuntimeSortFilterEventInfoList : ArrayList
	{
		internal new RuntimeSortFilterEventInfo this[int index] => (RuntimeSortFilterEventInfo)base[index];

		internal RuntimeSortFilterEventInfoList()
		{
		}
	}
}
