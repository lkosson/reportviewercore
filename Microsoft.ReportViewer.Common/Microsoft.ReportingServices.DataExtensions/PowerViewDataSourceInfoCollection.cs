using Microsoft.ReportingServices.Diagnostics.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal sealed class PowerViewDataSourceInfoCollection : IPowerViewDataSourceCollection, IEnumerable
	{
		private Dictionary<string, DataSourceInfo> m_dataSourceInfos = new Dictionary<string, DataSourceInfo>();

		public int Count => m_dataSourceInfos.Count;

		public void AddOrUpdate(string key, DataSourceInfo dsInfo)
		{
			RSTrace.ProcessingTracer.Assert(key != null, "PowerViewDataSourceInfoCollection.AddOrUpdate: key != null");
			if (m_dataSourceInfos.ContainsKey(key))
			{
				m_dataSourceInfos.Remove(key);
			}
			m_dataSourceInfos.Add(key, dsInfo);
		}

		public IEnumerator GetEnumerator()
		{
			return m_dataSourceInfos.Values.GetEnumerator();
		}

		public DataSourceInfo GetDataSourceFromKey(string key)
		{
			m_dataSourceInfos.TryGetValue(key, out DataSourceInfo value);
			return value;
		}
	}
}
