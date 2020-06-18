namespace Microsoft.ReportingServices.Diagnostics
{
	internal class ExternalResourceAbortHelper : IAbortHelper
	{
		private bool m_isAborted;

		public bool IsAborted => m_isAborted;

		public bool Abort(ProcessingStatus status)
		{
			m_isAborted = true;
			return true;
		}
	}
}
