using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	[HashtableOfReferences]
	internal sealed class QuickFindHashtable : HashtableInstanceInfo
	{
		internal ReportItemInstance this[int key] => (ReportItemInstance)m_hashtable[key];

		internal QuickFindHashtable()
		{
		}

		internal QuickFindHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(int key, ReportItemInstance val)
		{
			m_hashtable.Add(key, val);
		}
	}
}
