using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RSAddinVersionMismatchException : ReportCatalogException
	{
		public RSAddinVersionMismatchException()
			: base(ErrorCode.rsVersionMismatch, ErrorStrings.rsVersionMismatch, null, null)
		{
		}

		private RSAddinVersionMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
