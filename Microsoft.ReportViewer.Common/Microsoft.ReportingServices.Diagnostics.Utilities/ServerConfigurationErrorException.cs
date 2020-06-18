using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ServerConfigurationErrorException : ReportCatalogException
	{
		public ServerConfigurationErrorException(Exception innerException, string additionalTraceMessage)
			: base(ErrorCode.rsServerConfigurationError, ErrorStrings.rsServerConfigurationError(null), innerException, additionalTraceMessage)
		{
		}

		public ServerConfigurationErrorException(Exception innerException, string additionalTraceMessage, string additionalMessage)
			: base(ErrorCode.rsServerConfigurationError, ErrorStrings.rsServerConfigurationError(additionalMessage), innerException, additionalTraceMessage)
		{
		}

		public ServerConfigurationErrorException(string additionalTraceMessage)
			: base(ErrorCode.rsServerConfigurationError, ErrorStrings.rsServerConfigurationError(null), null, additionalTraceMessage)
		{
		}

		private ServerConfigurationErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
