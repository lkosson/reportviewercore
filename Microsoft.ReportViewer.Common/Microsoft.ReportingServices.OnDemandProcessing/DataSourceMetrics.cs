using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Internal;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class DataSourceMetrics
	{
		private readonly string m_dataSourceName;

		private readonly string m_dataSourceReference;

		private readonly string m_dataSourceType;

		private readonly string m_embeddedConnectionString;

		private readonly long m_openConnectionDurationMs;

		private readonly bool? m_connectionFromPool;

		private readonly DataProcessingMetrics[] m_dataSetsMetrics;

		public long OpenConnectionDurationMs => m_openConnectionDurationMs;

		public DataSourceMetrics(string dataSourceName, string dataSourceReference, string dataSourceType, DataProcessingMetrics aggregatedMetrics, DataProcessingMetrics[] dataSetsMetrics)
			: this(dataSourceName, dataSourceReference, dataSourceType, aggregatedMetrics.ResolvedConnectionString, aggregatedMetrics.OpenConnectionDurationMs, aggregatedMetrics.ConnectionFromPool)
		{
			m_dataSetsMetrics = dataSetsMetrics;
		}

		public DataSourceMetrics(string dataSourceName, string dataSourceReference, string dataSourceType, DataProcessingMetrics parallelDataSetMetrics)
			: this(dataSourceName, dataSourceReference, dataSourceType, parallelDataSetMetrics.ResolvedConnectionString, parallelDataSetMetrics.OpenConnectionDurationMs, parallelDataSetMetrics.ConnectionFromPool)
		{
			m_dataSetsMetrics = new DataProcessingMetrics[1];
			m_dataSetsMetrics[0] = parallelDataSetMetrics;
		}

		private DataSourceMetrics(string dataSourceName, string dataSourceReference, string dataSourceType, string embeddedConnectionString, long openConnectionDurationMs, bool? connectionFromPool)
		{
			m_dataSourceName = dataSourceName;
			m_dataSourceReference = dataSourceReference;
			m_dataSourceType = dataSourceType;
			m_embeddedConnectionString = ((dataSourceReference == null) ? embeddedConnectionString : null);
			m_openConnectionDurationMs = openConnectionDurationMs;
			m_connectionFromPool = connectionFromPool;
		}

		internal Connection ToAdditionalInfoConnection(IJobContext jobContext)
		{
			if (jobContext == null)
			{
				return null;
			}
			Connection connection = new Connection();
			connection.ConnectionOpenTime = m_openConnectionDurationMs;
			connection.ConnectionFromPool = m_connectionFromPool;
			if (jobContext.ExecutionLogLevel == ExecutionLogLevel.Verbose)
			{
				DataSource dataSource = new DataSource();
				dataSource.Name = m_dataSourceName;
				if (m_dataSourceReference != null)
				{
					dataSource.DataSourceReference = m_dataSourceReference;
				}
				else if (m_embeddedConnectionString != null)
				{
					dataSource.ConnectionString = m_embeddedConnectionString;
				}
				dataSource.DataExtension = m_dataSourceType;
				connection.DataSource = dataSource;
			}
			if (m_dataSetsMetrics != null)
			{
				connection.DataSets = new List<DataSet>(m_dataSetsMetrics.Length);
				for (int i = 0; i < m_dataSetsMetrics.Length; i++)
				{
					connection.DataSets.Add(m_dataSetsMetrics[i].ToAdditionalInfoDataSet(jobContext));
				}
			}
			return connection;
		}
	}
}
