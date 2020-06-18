using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidParameterException : ReportCatalogException
	{
		public readonly string ParameterName;

		public InvalidParameterException(string parameterName)
			: base(ErrorCode.rsInvalidParameter, ErrorStrings.rsInvalidParameter(parameterName), null, null)
		{
			ParameterName = parameterName;
		}

		public InvalidParameterException(string parameterName, Exception innnerException)
			: base(ErrorCode.rsInvalidParameter, ErrorStrings.rsInvalidParameter(parameterName), innnerException, null)
		{
		}

		private InvalidParameterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
