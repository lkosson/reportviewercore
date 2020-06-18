using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class SortFilterEventInfoHashtable : HashtableInstanceInfo
	{
		internal SortFilterEventInfo this[int key] => (SortFilterEventInfo)m_hashtable[key];

		internal SortFilterEventInfoHashtable()
		{
		}

		internal SortFilterEventInfoHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(int key, SortFilterEventInfo val)
		{
			m_hashtable.Add(key, val);
		}
	}
}
