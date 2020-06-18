using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CannotRetrieveModelException : ReportCatalogException
	{
		public static bool IsCannotRetrieveModelErrorCode(ErrorCode errorCode)
		{
			if (errorCode == ErrorCode.rsCannotRetrieveModel || errorCode == ErrorCode.rsUnsupportedMetadataVersionRequested || errorCode == ErrorCode.rsInvalidPerspectiveAndVersion)
			{
				return true;
			}
			return false;
		}

		public CannotRetrieveModelException(ErrorCode errorCode, string itemName, Exception innerException)
			: base(errorCode, ErrorStrings.rsCannotRetrieveModel(itemName), innerException, null)
		{
		}

		public CannotRetrieveModelException(string itemName, Exception innerException)
			: base(ErrorCode.rsCannotRetrieveModel, ErrorStrings.rsCannotRetrieveModel(itemName), innerException, null)
		{
		}

		private CannotRetrieveModelException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
