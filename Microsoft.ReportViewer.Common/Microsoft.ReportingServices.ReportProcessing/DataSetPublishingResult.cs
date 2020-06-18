using Microsoft.ReportingServices.DataExtensions;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataSetPublishingResult
	{
		private DataSetDefinition m_dataSetDefinition;

		private DataSourceInfo m_dataSourceInfo;

		private UserLocationFlags m_userReferenceLocation;

		private ProcessingMessageList m_warnings;

		public DataSetDefinition DataSetDefinition => m_dataSetDefinition;

		public DataSourceInfo DataSourceInfo => m_dataSourceInfo;

		public bool HasUserProfileQueryDependencies
		{
			get
			{
				if ((m_userReferenceLocation & UserLocationFlags.ReportQueries) == 0)
				{
					return false;
				}
				return true;
			}
		}

		public ProcessingMessageList Warnings => m_warnings;

		public int TimeOut
		{
			get
			{
				if (m_dataSetDefinition != null && m_dataSetDefinition.DataSetCore != null && m_dataSetDefinition.DataSetCore.Query != null)
				{
					return m_dataSetDefinition.DataSetCore.Query.TimeOut;
				}
				return 0;
			}
		}

		internal DataSetPublishingResult(DataSetDefinition dataSetDefinition, DataSourceInfo dataSourceInfo, UserLocationFlags userReferenceLocation, ProcessingMessageList warnings)
		{
			m_dataSetDefinition = dataSetDefinition;
			m_dataSourceInfo = dataSourceInfo;
			m_userReferenceLocation = userReferenceLocation;
			m_warnings = warnings;
		}
	}
}
