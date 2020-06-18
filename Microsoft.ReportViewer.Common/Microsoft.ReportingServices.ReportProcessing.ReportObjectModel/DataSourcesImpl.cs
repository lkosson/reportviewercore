using System.Collections;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class DataSourcesImpl : DataSources
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		internal const string Name = "DataSources";

		internal const string FullName = "Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.DataSources";

		public override DataSource this[string key]
		{
			get
			{
				if (key == null || m_collection == null)
				{
					throw new ReportProcessingException_NonExistingDataSourceReference(key);
				}
				try
				{
					DataSource dataSource = m_collection[key] as DataSource;
					if (dataSource == null)
					{
						throw new ReportProcessingException_NonExistingDataSourceReference(key);
					}
					return dataSource;
				}
				catch
				{
					throw new ReportProcessingException_NonExistingDataSourceReference(key);
				}
			}
		}

		internal DataSourcesImpl(int size)
		{
			m_lockAdd = (size > 1);
			m_collection = new Hashtable(size);
		}

		internal void Add(Microsoft.ReportingServices.ReportProcessing.DataSource dataSourceDef)
		{
			try
			{
				if (m_lockAdd)
				{
					Monitor.Enter(m_collection);
				}
				if (!m_collection.ContainsKey(dataSourceDef.Name))
				{
					m_collection.Add(dataSourceDef.Name, new DataSourceImpl(dataSourceDef));
				}
			}
			finally
			{
				if (m_lockAdd)
				{
					Monitor.Exit(m_collection);
				}
			}
		}
	}
}
