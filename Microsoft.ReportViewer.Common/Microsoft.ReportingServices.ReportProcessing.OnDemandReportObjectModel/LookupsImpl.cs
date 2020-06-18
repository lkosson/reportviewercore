using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class LookupsImpl : Lookups
	{
		private Dictionary<string, LookupImpl> m_collection;

		public override Lookup this[string key]
		{
			get
			{
				LookupImpl value = null;
				if (key == null || m_collection == null || !m_collection.TryGetValue(key, out value))
				{
					throw new ReportProcessingException_NonExistingLookupReference();
				}
				return value;
			}
		}

		internal LookupsImpl()
		{
		}

		internal void Add(LookupImpl lookup)
		{
			if (m_collection == null)
			{
				m_collection = new Dictionary<string, LookupImpl>();
			}
			m_collection.Add(lookup.Name, lookup);
		}
	}
}
