using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class FieldPropertyHashtable
	{
		private Hashtable m_hashtable;

		internal int Count => m_hashtable.Count;

		internal FieldPropertyHashtable()
		{
			m_hashtable = new Hashtable();
		}

		internal FieldPropertyHashtable(int capacity)
		{
			m_hashtable = new Hashtable(capacity);
		}

		internal void Add(string key)
		{
			m_hashtable.Add(key, null);
		}

		internal bool ContainsKey(string key)
		{
			return m_hashtable.ContainsKey(key);
		}

		internal IDictionaryEnumerator GetEnumerator()
		{
			return m_hashtable.GetEnumerator();
		}
	}
}
