using Microsoft.ReportingServices.DataExtensions;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;

namespace Microsoft.Reporting.WinForms
{
	internal class DataSetExtensionConnection : IProcessingDataExtensionConnection
	{
		private LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback m_subreportCallback;

		private IEnumerable m_dataSources;

		public bool MustResolveSharedDataSources => false;

		public DataSetExtensionConnection(LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback subreportCallback, IEnumerable dataSources)
		{
			m_subreportCallback = subreportCallback;
			m_dataSources = dataSources;
		}

		public void DataSetRetrieveForReportInstance(ICatalogItemContext itemContext, ParameterInfoCollection reportParameters)
		{
			IEnumerable enumerable = m_subreportCallback((PreviewItemContext)itemContext, reportParameters);
			m_dataSources = new DataSourceCollectionWrapper((ReportDataSourceCollection)enumerable);
		}

		public void HandleImpersonation(IProcessingDataSource dataSource, DataSourceInfo dataSourceInfo, string datasetName, IDbConnection connection, System.Action afterImpersonationAction)
		{
			afterImpersonationAction?.Invoke();
		}

		public IDbConnection OpenDataSourceExtensionConnection(IProcessingDataSource dataSource, string connectionString, DataSourceInfo dataSourceInfo, string datasetName)
		{
			return new DataSetProcessingExtension(m_dataSources, datasetName);
		}

		public void CloseConnection(IDbConnection connection, IProcessingDataSource dataSourceObj, DataSourceInfo dataSourceInfo)
		{
			CloseConnectionWithoutPool(connection);
		}

		public void CloseConnectionWithoutPool(IDbConnection connection)
		{
			connection.Close();
		}
	}
}
