using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ContainerTypeNotSupportedException : ReportCatalogException
	{
		public ContainerTypeNotSupportedException()
			: base(ErrorCode.rsContainerNotSupported, ErrorStrings.rsContainerNotSupported, null, null)
		{
		}

		private ContainerTypeNotSupportedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
