using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_FieldError : Exception
	{
		private DataFieldStatus m_status;

		internal DataFieldStatus Status => m_status;

		internal ReportProcessingException_FieldError(DataFieldStatus status, string message)
			: base((message == null) ? "" : message, null)
		{
			m_status = status;
		}

		private ReportProcessingException_FieldError(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
