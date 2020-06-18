using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ClosingRegisteredStreamException : ReportCatalogException
	{
		public ClosingRegisteredStreamException(Exception innerException)
			: base(ErrorCode.rsClosingRegisteredStreamException, ErrorStrings.rsClosingRegisteredStreamException, innerException, null)
		{
		}

		private ClosingRegisteredStreamException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
