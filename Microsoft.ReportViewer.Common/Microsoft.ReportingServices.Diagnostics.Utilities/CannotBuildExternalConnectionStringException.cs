using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CannotBuildExternalConnectionStringException : ReportCatalogException
	{
		public CannotBuildExternalConnectionStringException(string dataSource)
			: this(dataSource, null)
		{
		}

		public CannotBuildExternalConnectionStringException(string dataSource, Exception innerException)
			: base(ErrorCode.rsCannotBuildExternalConnectionString, ErrorStrings.cannotBuildExternalConnectionString(dataSource), innerException, null)
		{
		}

		private CannotBuildExternalConnectionStringException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
