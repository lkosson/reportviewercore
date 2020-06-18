using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReceiverInformationHashtable : HashtableInstanceInfo
	{
		internal ReceiverInformation this[int key]
		{
			get
			{
				return (ReceiverInformation)m_hashtable[key];
			}
			set
			{
				m_hashtable[key] = value;
			}
		}

		internal ReceiverInformationHashtable()
		{
		}

		internal ReceiverInformationHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(int key, ReceiverInformation receiver)
		{
			m_hashtable.Add(key, receiver);
		}
	}
}
