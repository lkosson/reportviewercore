using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SharePointException : ReportCatalogException
	{
		public SharePointException(Exception innerException)
			: base(ErrorCode.rsSharePointError, GetExceptionMessage(innerException), innerException, null)
		{
		}

		private SharePointException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private static string GetExceptionMessage(Exception innerException)
		{
			if (innerException is SqlException)
			{
				return ErrorStrings.rsSharePointContentDBAccessError;
			}
			return ErrorStrings.rsSharePointError;
		}
	}
}
