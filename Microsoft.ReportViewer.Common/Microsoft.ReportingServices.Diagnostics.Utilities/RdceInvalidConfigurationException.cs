using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RdceInvalidConfigurationException : RSException
	{
		public RdceInvalidConfigurationException()
			: base(ErrorCode.rsRdceInvalidConfigurationError, ErrorStrings.rsRdceInvalidConfigurationError, null, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
		}

		private RdceInvalidConfigurationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
