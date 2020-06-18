using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportServerDatabaseLogonFailedException : ReportCatalogException
	{
		public ReportServerDatabaseLogonFailedException(Exception innerException)
			: base(ErrorCode.rsReportServerDatabaseLogonFailed, ErrorStrings.rsReportServerDatabaseLogonFailed, innerException, null)
		{
		}

		private ReportServerDatabaseLogonFailedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
