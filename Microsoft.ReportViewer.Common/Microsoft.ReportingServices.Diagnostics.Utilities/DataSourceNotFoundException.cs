using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DataSourceNotFoundException : ReportCatalogException
	{
		public DataSourceNotFoundException(string dataSource)
			: base(ErrorCode.rsDataSourceNotFound, ErrorStrings.rsDataSourceNotFound(dataSource), null, null)
		{
		}

		private DataSourceNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
