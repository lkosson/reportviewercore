using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class PBIServiceUnavailable : ReportCatalogException
	{
		public PBIServiceUnavailable(string correlationId)
			: base(ErrorCode.rsPBIServiceUnavailable, ErrorStrings.rsPBIServiceUnavailable(correlationId), null, null)
		{
		}

		private PBIServiceUnavailable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
