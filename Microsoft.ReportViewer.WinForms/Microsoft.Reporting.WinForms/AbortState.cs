using System.Net;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class AbortState
	{
		private object m_abortLock = new object();

		private bool m_pendingAbort;

		private HttpWebRequest m_abortableRequest;

		public void AbortRequest()
		{
			lock (m_abortLock)
			{
				if (m_abortableRequest != null)
				{
					m_abortableRequest.Abort();
				}
				m_pendingAbort = true;
			}
		}

		public bool RegisterAbortableRequest(HttpWebRequest request)
		{
			lock (m_abortLock)
			{
				if (m_pendingAbort)
				{
					return false;
				}
				m_abortableRequest = request;
				return true;
			}
		}

		public void ClearPendingAbort()
		{
			lock (m_abortLock)
			{
				m_pendingAbort = false;
				m_abortableRequest = null;
			}
		}
	}
}
