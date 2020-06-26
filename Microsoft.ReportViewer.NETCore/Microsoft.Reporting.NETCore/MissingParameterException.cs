using System;
using System.Runtime.Serialization;

namespace Microsoft.Reporting.NETCore
{
	[Serializable]
	public sealed class MissingParameterException : ReportViewerException
	{
		internal MissingParameterException(string parameterName)
			: base(CommonStrings.MissingParameter(parameterName))
		{
		}

		private MissingParameterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
