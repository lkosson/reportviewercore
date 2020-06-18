using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class MissingParameterException : ReportCatalogException
	{
		public MissingParameterException(string parameterName)
			: base(ErrorCode.rsMissingParameter, ErrorStrings.rsMissingParameter(parameterName), null, null)
		{
		}

		private MissingParameterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
