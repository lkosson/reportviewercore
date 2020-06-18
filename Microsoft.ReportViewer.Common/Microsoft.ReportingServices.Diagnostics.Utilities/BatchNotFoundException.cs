using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class BatchNotFoundException : ReportCatalogException
	{
		public BatchNotFoundException(string batchId)
			: base(ErrorCode.rsBatchNotFound, ErrorStrings.rsBatchNotFound(batchId), null, null)
		{
		}

		private BatchNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
