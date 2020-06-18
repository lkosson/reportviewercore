using System;
using System.Runtime.Serialization;

namespace Microsoft.Reporting.WinForms
{
	[Serializable]
	public sealed class MissingEndpointException : ReportServerException
	{
		internal MissingEndpointException(string message, Exception innerException)
			: base(message, null, innerException)
		{
		}

		private MissingEndpointException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
