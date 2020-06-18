using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportParameterTypeMismatchException : ReportCatalogException
	{
		public ReportParameterTypeMismatchException(string parameterName)
			: base(ErrorCode.rsReportParameterTypeMismatch, ErrorStrings.rsReportParameterTypeMismatch(parameterName), null, null)
		{
		}

		private ReportParameterTypeMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
