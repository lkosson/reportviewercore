using System;
using System.Runtime.Serialization;

namespace Microsoft.Reporting.NETCore
{
	[Serializable]
	public sealed class MissingDataSourceException : ReportViewerException
	{
		internal MissingDataSourceException(string dataSourceName)
			: base(CommonStrings.MissingDataSource(dataSourceName))
		{
		}

		private MissingDataSourceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
