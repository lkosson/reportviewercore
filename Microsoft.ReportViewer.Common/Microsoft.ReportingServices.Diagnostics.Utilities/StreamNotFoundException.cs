using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class StreamNotFoundException : ReportCatalogException
	{
		public StreamNotFoundException(string streamId)
			: base(ErrorCode.rsStreamNotFound, ErrorStrings.rsStreamNotFound, null, null)
		{
		}

		private StreamNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
