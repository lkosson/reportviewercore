using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal sealed class ReportDataSource
	{
		private readonly string m_dataSourceType;

		private readonly Guid m_modelID;

		private readonly Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CreateDataExtensionInstance m_createDataExtensionInstance;

		public ReportDataSource(string dataSourceType, Guid modelID, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CreateDataExtensionInstance createDataExtensionInstance)
		{
			if (dataSourceType == null)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "The data source type is null. Cannot instantiate data processing component.");
				}
				throw new ReportProcessingException(ErrorCode.rsDataSourceTypeNull);
			}
			m_dataSourceType = dataSourceType;
			m_modelID = modelID;
			m_createDataExtensionInstance = createDataExtensionInstance;
		}

		public IDbConnection CreateConnection()
		{
			IDbConnection dbConnection = m_createDataExtensionInstance(m_dataSourceType, m_modelID);
			if (dbConnection == null)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "The connection object of the data source type {0} does not implement any of the required interfaces.", m_dataSourceType);
				}
				throw new DataExtensionNotFoundException(m_dataSourceType);
			}
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "A connection object for the {0} data source has been created.", m_dataSourceType);
			}
			return dbConnection;
		}
	}
}
