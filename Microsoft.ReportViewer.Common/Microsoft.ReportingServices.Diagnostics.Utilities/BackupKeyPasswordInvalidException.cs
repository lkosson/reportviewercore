using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class BackupKeyPasswordInvalidException : ReportCatalogException
	{
		public BackupKeyPasswordInvalidException()
			: base(ErrorCode.rsBackupKeyPasswordInvalid, ErrorStrings.rsBackupKeyPasswordInvalid, null, null)
		{
		}

		private BackupKeyPasswordInvalidException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
