using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ModelIDMismatchException : ReportCatalogException
	{
		public ModelIDMismatchException()
			: base(ErrorCode.rsModelIDMismatch, ErrorStrings.rsModelIDMismatch, null, null)
		{
		}

		private ModelIDMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
