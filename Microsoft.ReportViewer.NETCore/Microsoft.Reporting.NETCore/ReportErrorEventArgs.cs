using System;

namespace Microsoft.Reporting.NETCore
{
	public sealed class ReportErrorEventArgs : EventArgs
	{
		private Exception m_exception;

		private bool m_isHandled;

		public Exception Exception => m_exception;

		public bool Handled
		{
			get
			{
				return m_isHandled;
			}
			set
			{
				m_isHandled = value;
			}
		}

		internal ReportErrorEventArgs(Exception e)
		{
			m_exception = e;
		}
	}
}
