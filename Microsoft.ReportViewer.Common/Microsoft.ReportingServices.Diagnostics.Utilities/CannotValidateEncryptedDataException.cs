using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CannotValidateEncryptedDataException : ReportCatalogException
	{
		public CannotValidateEncryptedDataException()
			: base(ErrorCode.rsCannotValidateEncryptedData, ErrorStrings.rsCannotValidateEncryptedData, null, null)
		{
		}

		public CannotValidateEncryptedDataException(Exception e)
			: base(ErrorCode.rsCannotValidateEncryptedData, ErrorStrings.rsCannotValidateEncryptedData, e, null)
		{
		}

		private CannotValidateEncryptedDataException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
