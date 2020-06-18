using Microsoft.ReportingServices.Diagnostics.Utilities;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal sealed class DataSourceFaultContext
	{
		public readonly ErrorCode m_errorCode;

		public readonly string m_errorString;

		public ErrorCode ErrorCode => m_errorCode;

		public string ErrorString => m_errorString;

		internal DataSourceFaultContext(ErrorCode errorCode, string errorString)
		{
			m_errorCode = errorCode;
			m_errorString = errorString;
		}
	}
}
