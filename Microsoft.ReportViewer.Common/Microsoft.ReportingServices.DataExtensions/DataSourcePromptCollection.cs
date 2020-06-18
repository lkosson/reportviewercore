using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class DataSourcePromptCollection
	{
		private Hashtable m_collection = new Hashtable();

		private bool m_needPrompt;

		public bool NeedPrompt => m_needPrompt;

		public int Count => m_collection.Count;

		public IEnumerator GetEnumerator()
		{
			return m_collection.Values.GetEnumerator();
		}

		internal void Add(DataSourceInfo dataSource, ServerDataSourceSettings serverDatasourceSettings)
		{
			string originalName = dataSource.OriginalName;
			Global.Tracer.Assert(m_collection[originalName] == null, "Collection already contains this data source.");
			dataSource.ThrowIfNotUsable(serverDatasourceSettings);
			m_collection.Add(originalName, dataSource);
			if (dataSource.NeedPrompt)
			{
				m_needPrompt = true;
			}
		}

		public void AddSingleIfPrompt(DataSourceInfo dataSource, ServerDataSourceSettings serverDatasourceSettings)
		{
			Global.Tracer.Assert(dataSource.OriginalName == null, "Data source has non-null name when adding single");
			if (m_collection.Count != 0)
			{
				throw new InternalCatalogException("Prompt collection is not empty when adding single data source");
			}
			if (dataSource.CredentialsRetrieval == DataSourceInfo.CredentialsRetrievalOption.Prompt)
			{
				dataSource.ThrowIfNotUsable(serverDatasourceSettings);
				m_collection.Add("", dataSource);
				if (dataSource.NeedPrompt)
				{
					m_needPrompt = true;
				}
			}
		}
	}
}
