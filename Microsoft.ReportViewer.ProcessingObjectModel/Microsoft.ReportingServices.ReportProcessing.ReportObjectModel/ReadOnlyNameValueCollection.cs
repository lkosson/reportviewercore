using System;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class ReadOnlyNameValueCollection : MarshalByRefObject
	{
		private NameValueCollection m_originalCollection;

		public string[] AllKeys => m_originalCollection.AllKeys;

		public string[] AllValues
		{
			get
			{
				int count = m_originalCollection.Count;
				string[] array = new string[count];
				if (count > 0)
				{
					m_originalCollection.CopyTo(array, 0);
				}
				return array;
			}
		}

		public string this[int index] => m_originalCollection[index];

		public string this[string name] => m_originalCollection[name];

		public int Count => m_originalCollection.Count;

		public NameObjectCollectionBase.KeysCollection Keys => m_originalCollection.Keys;

		internal ReadOnlyNameValueCollection(NameValueCollection originalCollection)
		{
			if (originalCollection == null)
			{
				throw new ArgumentNullException("originalCollection");
			}
			m_originalCollection = originalCollection;
		}

		public void CopyTo(Array dest, int index)
		{
			m_originalCollection.CopyTo(dest, index);
		}

		public string Get(int index)
		{
			return m_originalCollection.Get(index);
		}

		public string Get(string name)
		{
			return m_originalCollection.Get(name);
		}

		public string GetKey(int index)
		{
			return m_originalCollection.GetKey(index);
		}

		public string[] GetValues(int index)
		{
			return m_originalCollection.GetValues(index);
		}

		public string[] GetValues(string name)
		{
			return m_originalCollection.GetValues(name);
		}

		public bool HasKeys()
		{
			return m_originalCollection.HasKeys();
		}

		public IEnumerator GetEnumerator()
		{
			return m_originalCollection.GetEnumerator();
		}
	}
}
