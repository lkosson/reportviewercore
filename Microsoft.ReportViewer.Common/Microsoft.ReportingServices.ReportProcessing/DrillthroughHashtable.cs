using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DrillthroughHashtable : HashtableInstanceInfo
	{
		internal DrillthroughInformation this[string key]
		{
			get
			{
				return (DrillthroughInformation)m_hashtable[key];
			}
			set
			{
				m_hashtable[key] = value;
			}
		}

		internal DrillthroughHashtable()
		{
		}

		internal DrillthroughHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(string drillthroughId, DrillthroughInformation drillthroughInfo)
		{
			m_hashtable.Add(drillthroughId, drillthroughInfo);
		}
	}
}
