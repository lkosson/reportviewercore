using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class DataSourcesImpl : DataSources
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		public override Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.DataSource this[string key]
		{
			get
			{
				if (key == null || m_collection == null)
				{
					throw new ReportProcessingException_NonExistingDataSourceReference(key);
				}
				try
				{
					Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.DataSource dataSource = m_collection[key] as Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.DataSource;
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

		internal void Add(Microsoft.ReportingServices.ReportIntermediateFormat.DataSource dataSourceDef)
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
