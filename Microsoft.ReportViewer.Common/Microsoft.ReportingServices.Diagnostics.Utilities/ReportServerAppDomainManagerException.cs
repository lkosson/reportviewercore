using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportServerAppDomainManagerException : ReportCatalogException
	{
		public ReportServerAppDomainManagerException(Exception innerException, string appDomain, string additionalTraceMessage)
			: base(ErrorCode.rsAppDomainManagerError, ErrorStrings.rsAppDomainManagerError(appDomain), innerException, additionalTraceMessage)
		{
		}

		public ReportServerAppDomainManagerException(string appDomain, string additionalTraceMessage)
			: base(ErrorCode.rsAppDomainManagerError, ErrorStrings.rsAppDomainManagerError(appDomain), null, additionalTraceMessage)
		{
		}

		private ReportServerAppDomainManagerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
