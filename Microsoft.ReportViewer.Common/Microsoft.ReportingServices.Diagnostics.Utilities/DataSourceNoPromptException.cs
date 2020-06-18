using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DataSourceNoPromptException : ReportCatalogException
	{
		public DataSourceNoPromptException(string dataSource)
			: base(ErrorCode.rsDataSourceNoPrompt, ErrorStrings.rsDataSourceNoPromptException(dataSource), null, null)
		{
		}

		private DataSourceNoPromptException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
