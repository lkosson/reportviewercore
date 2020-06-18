using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class AllowNullKeyDictionary<TKey, TValue> where TKey : class where TValue : class
	{
		private Dictionary<TKey, TValue> m_hashtable = new Dictionary<TKey, TValue>();

		private TValue m_valueForNullKey;

		internal TValue this[TKey key]
		{
			get
			{
				TValue value = null;
				TryGetValue(key, out value);
				return value;
			}
			set
			{
				Add(key, value);
			}
		}

		internal void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				m_valueForNullKey = value;
			}
			else
			{
				m_hashtable.Add(key, value);
			}
		}

		internal bool TryGetValue(TKey key, out TValue value)
		{
			value = null;
			if (key == null)
			{
				if (m_valueForNullKey == null)
				{
					return false;
				}
				value = m_valueForNullKey;
				return true;
			}
			return m_hashtable.TryGetValue(key, out value);
		}
	}
}
