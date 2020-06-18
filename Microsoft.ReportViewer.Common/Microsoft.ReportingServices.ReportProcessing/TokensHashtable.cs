using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TokensHashtable : HashtableInstanceInfo
	{
		internal object this[int key]
		{
			get
			{
				return m_hashtable[key];
			}
			set
			{
				m_hashtable[key] = value;
			}
		}

		internal TokensHashtable()
		{
		}

		internal TokensHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(int tokenID, object tokenValue)
		{
			m_hashtable.Add(tokenID, tokenValue);
		}
	}
}
