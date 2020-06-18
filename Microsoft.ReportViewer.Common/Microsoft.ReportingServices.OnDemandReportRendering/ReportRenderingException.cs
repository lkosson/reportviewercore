using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	[Serializable]
	internal class ReportRenderingException : Exception
	{
		private ErrorCode m_ErrorCode;

		private bool m_Unexpected;

		public ErrorCode ErrorCode => m_ErrorCode;

		public bool Unexpected => m_Unexpected;

		protected ReportRenderingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ReportRenderingException(ErrorCode errCode)
			: this(errCode, RenderErrors.Keys.GetString(errCode.ToString()), unexpected: false)
		{
			m_ErrorCode = errCode;
		}

		public ReportRenderingException(string message)
			: this(ErrorCode.rrRenderingError, message, unexpected: false)
		{
		}

		public ReportRenderingException(string message, bool unexpected)
			: this(ErrorCode.rrRenderingError, message, unexpected)
		{
		}

		public ReportRenderingException(ErrorCode errCode, string message, bool unexpected)
			: base(message)
		{
			m_ErrorCode = errCode;
			m_Unexpected = unexpected;
		}

		public ReportRenderingException(Exception innerException)
			: this(ErrorCode.rrRenderingError, innerException, unexpected: false)
		{
		}

		public ReportRenderingException(Exception innerException, bool unexpected)
			: this(ErrorCode.rrRenderingError, innerException, unexpected)
		{
		}

		public ReportRenderingException(ErrorCode errCode, Exception innerException)
			: this(errCode, RenderErrors.Keys.GetString(errCode.ToString()), innerException, unexpected: false)
		{
		}

		public ReportRenderingException(ErrorCode errCode, Exception innerException, bool unexpected)
			: this(errCode, RenderErrors.Keys.GetString(errCode.ToString()), innerException, unexpected)
		{
		}

		public ReportRenderingException(string message, Exception innerException)
			: this(ErrorCode.rrRenderingError, message, innerException, unexpected: false)
		{
		}

		public ReportRenderingException(string message, Exception innerException, bool unexpected)
			: this(ErrorCode.rrRenderingError, message, innerException, unexpected)
		{
		}

		public ReportRenderingException(ErrorCode errCode, string message, Exception innerException, bool unexpected)
			: base(message, innerException)
		{
			m_ErrorCode = errCode;
			m_Unexpected = unexpected;
		}

		public ReportRenderingException(ErrorCode errCode, params object[] arguments)
			: base(string.Format(CultureInfo.CurrentCulture, RenderErrors.Keys.GetString(errCode.ToString()), arguments))
		{
			m_ErrorCode = errCode;
		}
	}
}
