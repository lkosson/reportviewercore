using System;
using System.Runtime.Serialization;

namespace Microsoft.Reporting.NETCore
{
	[Serializable]
	public sealed class MissingDataSourceCredentialsException : ReportViewerException
	{
		internal MissingDataSourceCredentialsException()
			: base(CommonStrings.MissingDataSourceCredentials)
		{
		}

		private MissingDataSourceCredentialsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
