using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Reporting.WinForms
{
	public sealed class RenderingCompleteEventArgs : EventArgs
	{
		private IList<Warning> m_warnings;

		private Exception m_exception;

		public IList<Warning> Warnings => m_warnings;

		public Exception Exception => m_exception;

		internal RenderingCompleteEventArgs(Warning[] warnings, Exception e)
		{
			if (warnings != null)
			{
				m_warnings = new ReadOnlyCollection<Warning>(warnings);
			}
			else
			{
				m_warnings = new ReadOnlyCollection<Warning>(new Warning[0]);
			}
			m_exception = e;
		}
	}
}
