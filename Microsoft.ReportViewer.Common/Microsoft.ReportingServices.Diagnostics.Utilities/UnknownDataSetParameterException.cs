using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class UnknownDataSetParameterException : ReportCatalogException
	{
		public UnknownDataSetParameterException(string parameterName)
			: base(ErrorCode.rsUnknownDataSetParameter, ErrorStrings.rsUnknownDataSetParameter(parameterName), null, null)
		{
		}

		private UnknownDataSetParameterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
