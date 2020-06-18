using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class UnsupportedParameterForModeException : ReportCatalogException
	{
		public UnsupportedParameterForModeException(string mode, string parameterName)
			: base(ErrorCode.rsUnsupportedParameterForMode, ErrorStrings.rsUnsupportedParameterForMode(mode, parameterName), null, null)
		{
		}

		private UnsupportedParameterForModeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
