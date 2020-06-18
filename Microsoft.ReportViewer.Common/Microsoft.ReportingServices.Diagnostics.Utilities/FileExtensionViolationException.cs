using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class FileExtensionViolationException : ReportCatalogException
	{
		public FileExtensionViolationException(string targetFileExtension, string sourceFileExtension)
			: base(ErrorCode.rsFileExtensionViolation, ErrorStrings.rsFileExtensionViolation(targetFileExtension, sourceFileExtension), null, null)
		{
		}

		private FileExtensionViolationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
