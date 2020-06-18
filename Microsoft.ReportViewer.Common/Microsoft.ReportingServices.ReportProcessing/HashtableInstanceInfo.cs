using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class HashtableInstanceInfo : InstanceInfo
	{
		protected Hashtable m_hashtable;

		internal int Count => m_hashtable.Count;

		protected HashtableInstanceInfo()
		{
			m_hashtable = new Hashtable();
		}

		protected HashtableInstanceInfo(int capacity)
		{
			m_hashtable = new Hashtable(capacity);
		}

		internal bool ContainsKey(int key)
		{
			return m_hashtable.ContainsKey(key);
		}

		internal IDictionaryEnumerator GetEnumerator()
		{
			return m_hashtable.GetEnumerator();
		}
	}
}
