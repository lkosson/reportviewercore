using System;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal sealed class ComponentPublishingError : RSException
	{
		public ComponentPublishingError(Exception innerException)
			: base(ErrorCode.rsComponentPublishingError, ErrorStrings.rsComponentPublishingError, innerException, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
		}
	}
}
