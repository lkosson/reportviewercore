using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RdceMismatchRdlVersion : RSException
	{
		public RdceMismatchRdlVersion(string originalNamespace, string processedNamespace)
			: base(ErrorCode.rsRdceMismatchRdlVersion, ErrorStrings.rsRdceMismatchRdlVersion, null, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, string.Format(CultureInfo.CurrentCulture, "Original namespace = '{0}', new namesapce = '{1}'", originalNamespace, processedNamespace))
		{
		}

		private RdceMismatchRdlVersion(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
