using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RenderingObjectModelException : RSException
	{
		private ProcessingErrorCode m_processingErrorCode;

		internal ProcessingErrorCode ProcessingErrorCode => m_processingErrorCode;

		private RenderingObjectModelException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal RenderingObjectModelException(string LocalizedErrorMessage)
			: base(ErrorCode.rrRenderingError, LocalizedErrorMessage, null, Global.RenderingTracer, null)
		{
		}

		internal RenderingObjectModelException(Exception innerException)
			: base(ErrorCode.rrRenderingError, innerException.Message, innerException, Global.RenderingTracer, null)
		{
		}

		internal RenderingObjectModelException(ProcessingErrorCode errCode)
			: base(ErrorCode.rrRenderingError, RPRes.Keys.GetString(errCode.ToString()), null, Global.RenderingTracer, null)
		{
			m_processingErrorCode = errCode;
		}

		internal RenderingObjectModelException(ProcessingErrorCode errCode, params object[] arguments)
			: base(ErrorCode.rrRenderingError, string.Format(CultureInfo.CurrentCulture, RPRes.Keys.GetString(errCode.ToString()), arguments), null, Global.RenderingTracer, null)
		{
			m_processingErrorCode = errCode;
		}

		internal RenderingObjectModelException(ErrorCode code, params object[] arguments)
			: base(code, string.Format(CultureInfo.CurrentCulture, RPRes.Keys.GetString(code.ToString()), arguments), null, Global.Tracer, null)
		{
		}

		internal RenderingObjectModelException(ErrorCode code, Exception innerException, params object[] arguments)
			: base(code, string.Format(CultureInfo.CurrentCulture, RPRes.Keys.GetString(code.ToString()), arguments), innerException, Global.Tracer, null)
		{
		}
	}
}
