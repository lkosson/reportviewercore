using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class FileExtensionMissingException : ReportCatalogException
	{
		public FileExtensionMissingException()
			: base(ErrorCode.rsFileExtensionRequired, ErrorStrings.rsFileExtensionRequired, null, null)
		{
		}

		private FileExtensionMissingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
