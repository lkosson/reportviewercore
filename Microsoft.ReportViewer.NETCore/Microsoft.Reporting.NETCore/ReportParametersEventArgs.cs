using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.NETCore
{
	[ComVisible(false)]
	public class ReportParametersEventArgs : CancelEventArgs
	{
		private ReportParameterCollection m_parameters;

		private bool m_autoSubmit;

		public ReportParameterCollection Parameters => m_parameters;

		public bool AutoSubmit => m_autoSubmit;

		internal ReportParametersEventArgs(ReportParameterCollection parameters, bool autoSubmit)
		{
			m_parameters = parameters;
			m_autoSubmit = autoSubmit;
		}
	}
}
