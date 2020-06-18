using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ExecuteQueriesFailureException : ReportCatalogException
	{
		public ExecuteQueriesFailureException(string dataSourceName, ErrorCode errorCode, Exception innerException)
			: base(errorCode, ErrorStrings.rsExecuteQueriesFailure(dataSourceName), innerException, null)
		{
		}

		private ExecuteQueriesFailureException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
