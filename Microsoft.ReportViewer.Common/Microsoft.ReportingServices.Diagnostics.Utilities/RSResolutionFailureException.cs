using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RSResolutionFailureException : ReportCatalogException
	{
		public RSResolutionFailureException(string databaseFullName)
			: this(null, databaseFullName)
		{
		}

		public RSResolutionFailureException(Exception innerException, string databaseFullName)
			: base(ErrorCode.rsResolutionFailureException, ErrorStrings.rsResolutionFailureException(databaseFullName), innerException, null)
		{
		}

		private RSResolutionFailureException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
