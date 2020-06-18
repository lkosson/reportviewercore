using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class DataSourceErrorInspector
	{
		private readonly IDbConnection m_connection;

		internal DataSourceErrorInspector(IDbConnection connection)
		{
			m_connection = connection;
		}

		internal bool TryInterpretProviderErrorCode(Exception e, out ErrorCode errorCode)
		{
			IDbErrorInspectorFactory dbErrorInspectorFactory = m_connection as IDbErrorInspectorFactory;
			if (dbErrorInspectorFactory != null)
			{
				IDbErrorInspector dbErrorInspector = dbErrorInspectorFactory.CreateErrorInspector();
				if (dbErrorInspector.IsQueryMemoryLimitExceeded(e))
				{
					errorCode = ErrorCode.rsQueryMemoryLimitExceeded;
					return true;
				}
				if (dbErrorInspector.IsQueryTimeout(e))
				{
					errorCode = ErrorCode.rsQueryTimeoutExceeded;
					return true;
				}
			}
			errorCode = ErrorCode.rsSuccess;
			return false;
		}

		internal bool IsOnPremiseServiceException(Exception e)
		{
			return (m_connection as IDbErrorInspectorFactory)?.CreateErrorInspector().IsOnPremisesServiceException(e) ?? false;
		}
	}
}
