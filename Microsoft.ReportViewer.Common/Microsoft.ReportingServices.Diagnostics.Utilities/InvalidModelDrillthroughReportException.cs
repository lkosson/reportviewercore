using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidModelDrillthroughReportException : ReportCatalogException
	{
		public InvalidModelDrillthroughReportException(string reportName)
			: base(ErrorCode.rsInvalidModelDrillthroughReport, ErrorStrings.rsInvalidModelDrillthroughReport(reportName), null, null)
		{
		}

		private InvalidModelDrillthroughReportException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
