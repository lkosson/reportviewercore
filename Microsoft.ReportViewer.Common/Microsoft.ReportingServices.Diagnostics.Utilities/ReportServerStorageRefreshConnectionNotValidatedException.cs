using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportServerStorageRefreshConnectionNotValidatedException : ReportCatalogException
	{
		public ReportServerStorageRefreshConnectionNotValidatedException(long modelId, long refreshConnectionId)
			: base(ErrorCode.rsReportServerStorageRefreshConnectionNotValidated, ErrorStrings.rsReportServerStorageRefreshConnectionNotValidated(modelId.ToString(), refreshConnectionId.ToString()), null, null)
		{
		}

		private ReportServerStorageRefreshConnectionNotValidatedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
