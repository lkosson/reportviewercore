using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal sealed class FileSizeException : ReportCatalogException
	{
		public FileSizeException()
			: base(ErrorCode.rsDataCacheMismatch, ErrorStrings.rsFileSize, null, null)
		{
		}

		private FileSizeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
