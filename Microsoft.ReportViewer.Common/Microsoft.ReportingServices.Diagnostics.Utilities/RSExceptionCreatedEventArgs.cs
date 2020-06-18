using System;
using System.Diagnostics;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	internal sealed class RSExceptionCreatedEventArgs : EventArgs
	{
		private readonly RSException m_e;

		public RSException Exception
		{
			[DebuggerStepThrough]
			get
			{
				return m_e;
			}
		}

		public RSExceptionCreatedEventArgs(RSException exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			m_e = exception;
		}
	}
}
