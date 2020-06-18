using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidParameterCombinationException : ReportCatalogException
	{
		public InvalidParameterCombinationException()
			: base(ErrorCode.rsInvalidParameterCombination, ErrorStrings.rsInvalidParameterCombination, null, null)
		{
		}

		private InvalidParameterCombinationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
