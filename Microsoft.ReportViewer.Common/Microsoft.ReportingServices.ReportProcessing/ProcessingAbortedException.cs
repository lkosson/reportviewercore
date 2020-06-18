using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ProcessingAbortedException : RSException
	{
		public enum Reason
		{
			UserCanceled,
			AbnormalTermination
		}

		private Reason m_reason;

		private readonly CancelationTrigger m_cancelationTrigger;

		public Reason ReasonForAbort => m_reason;

		internal CancelationTrigger Trigger => m_cancelationTrigger;

		private ProcessingAbortedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal ProcessingAbortedException()
			: this(CancelationTrigger.None)
		{
		}

		internal ProcessingAbortedException(CancelationTrigger cancelationTrigger)
			: base(ErrorCode.rsProcessingAborted, RPRes.rsProcessingAbortedByUser, null, Global.Tracer, CreateAdditionalTraceMessage(Reason.UserCanceled, cancelationTrigger))
		{
			m_reason = Reason.UserCanceled;
			m_cancelationTrigger = cancelationTrigger;
		}

		internal ProcessingAbortedException(Exception innerException)
			: this(CancelationTrigger.None, innerException)
		{
		}

		internal ProcessingAbortedException(CancelationTrigger cancelationTrigger, Exception innerException)
			: base(ErrorCode.rsProcessingAborted, RPRes.rsProcessingAbortedByError, innerException, Global.Tracer, CreateAdditionalTraceMessage(Reason.AbnormalTermination, cancelationTrigger))
		{
			m_reason = Reason.AbnormalTermination;
			m_cancelationTrigger = cancelationTrigger;
		}

		private static string CreateAdditionalTraceMessage(Reason reason, CancelationTrigger trigger)
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0}:{1}]", reason.ToString(), trigger.ToString());
		}
	}
}
