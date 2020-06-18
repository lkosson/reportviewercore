using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class LogonFailedException : ReportCatalogException
	{
		public LogonFailedException(Exception innerException, string userName)
			: base(ErrorCode.rsLogonFailed, ErrorStrings.rsLogonFailed, innerException, BuildLogFileMessage(userName))
		{
		}

		public LogonFailedException(Exception innerException)
			: this(innerException, null)
		{
		}

		private static string BuildLogFileMessage(string userName)
		{
			if (string.IsNullOrEmpty(userName))
			{
				return null;
			}
			return string.Format(CultureInfo.InvariantCulture, "Logon attempt for user '{0}' failed.", userName);
		}

		private LogonFailedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
