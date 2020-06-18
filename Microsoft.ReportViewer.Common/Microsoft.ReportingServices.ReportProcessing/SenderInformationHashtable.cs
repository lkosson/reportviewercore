using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class SenderInformationHashtable : HashtableInstanceInfo
	{
		internal SenderInformation this[int key]
		{
			get
			{
				return (SenderInformation)m_hashtable[key];
			}
			set
			{
				m_hashtable[key] = value;
			}
		}

		internal SenderInformationHashtable()
		{
		}

		internal SenderInformationHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(int key, SenderInformation sender)
		{
			m_hashtable.Add(key, sender);
		}
	}
}
