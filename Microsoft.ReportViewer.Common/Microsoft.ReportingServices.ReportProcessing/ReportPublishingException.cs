using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportPublishingException : ReportProcessingException
	{
		private ReportProcessingFlags m_processingFlags;

		private const string ReportProcessingFlagsName = "ReportProcessingFlags";

		public ReportProcessingFlags ReportProcessingFlags => m_processingFlags;

		public ReportPublishingException(ProcessingMessageList messages, ReportProcessingFlags processingFlags)
			: base(messages)
		{
			m_processingFlags = processingFlags;
		}

		public ReportPublishingException(ProcessingMessageList messages, Exception innerException, ReportProcessingFlags processingFlags)
			: base(messages, innerException)
		{
			m_processingFlags = processingFlags;
		}

		public ReportPublishingException(ErrorCode code, Exception innerException, params object[] arguments)
			: base(code, innerException, arguments)
		{
		}

		private ReportPublishingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			m_processingFlags = (ReportProcessingFlags)info.GetValue("ReportProcessingFlags", typeof(ReportProcessingFlags));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ReportProcessingFlags", m_processingFlags);
		}
	}
}
