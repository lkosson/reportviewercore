using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class JobCanceledException : ReportCatalogException
	{
		public JobCanceledException(Exception innerException)
			: base(ErrorCode.rsJobWasCanceled, ErrorStrings.rsJobWasCanceled, innerException, null)
		{
		}

		private JobCanceledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
