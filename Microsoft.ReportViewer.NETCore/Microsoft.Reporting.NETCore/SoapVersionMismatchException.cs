using System;
using System.Runtime.Serialization;

namespace Microsoft.Reporting.NETCore
{
	[Serializable]
	public sealed class SoapVersionMismatchException : ReportServerException
	{
		internal SoapVersionMismatchException(string message, Exception innerException)
			: base(message, null, innerException)
		{
		}

		private SoapVersionMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
