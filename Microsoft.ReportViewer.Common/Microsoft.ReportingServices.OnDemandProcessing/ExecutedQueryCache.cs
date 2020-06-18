using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class ExecutedQueryCache
	{
		private readonly List<ExecutedQuery> m_queries;

		internal ExecutedQueryCache()
		{
			m_queries = new List<ExecutedQuery>();
		}

		internal void Add(ExecutedQuery query)
		{
			int indexInCollection = query.DataSet.IndexInCollection;
			for (int i = m_queries.Count - 1; i <= indexInCollection; i++)
			{
				m_queries.Add(null);
			}
			m_queries[indexInCollection] = query;
		}

		internal void Extract(DataSet dataSet, out ExecutedQuery query)
		{
			int indexInCollection = dataSet.IndexInCollection;
			if (indexInCollection >= m_queries.Count)
			{
				query = null;
				return;
			}
			query = m_queries[indexInCollection];
			m_queries[indexInCollection] = null;
		}

		internal void Close()
		{
			for (int i = 0; i < m_queries.Count; i++)
			{
				m_queries[i]?.Close();
				m_queries[i] = null;
			}
		}
	}
}
