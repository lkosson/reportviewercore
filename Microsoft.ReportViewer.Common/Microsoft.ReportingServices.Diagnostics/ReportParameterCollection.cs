using System;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal class ReportParameterCollection : NameValueCollection
	{
		internal ReportParameterCollection(NameValueCollection other)
			: base(other)
		{
		}

		public override int GetHashCode()
		{
			int num = 0;
			StringCollection stringCollection = new StringCollection();
			for (int i = 0; i < Count; i++)
			{
				string key = GetKey(i);
				stringCollection.Add(key);
				string[] values = GetValues(i);
				if (values == null)
				{
					continue;
				}
				string[] array = values;
				foreach (string text in array)
				{
					if (text != null)
					{
						stringCollection.Add(text);
					}
				}
			}
			StringEnumerator enumerator = stringCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					num ^= current.GetHashCode();
				}
				return num;
			}
			finally
			{
				(enumerator as IDisposable)?.Dispose();
			}
		}

		public override bool Equals(object obj)
		{
			NameValueCollection nameValueCollection = obj as NameValueCollection;
			if (nameValueCollection == null)
			{
				return false;
			}
			if (Count != nameValueCollection.Count)
			{
				return false;
			}
			for (int i = 0; i < Count; i++)
			{
				if (GetKey(i) != nameValueCollection.GetKey(i))
				{
					return false;
				}
				string[] values = GetValues(i);
				string[] values2 = nameValueCollection.GetValues(i);
				if (values == null != (values2 == null))
				{
					return false;
				}
				if (values == null)
				{
					continue;
				}
				if (values.Length != values2.Length)
				{
					return false;
				}
				for (int j = 0; j < values.Length; j++)
				{
					if (values[j] != values2[j])
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
