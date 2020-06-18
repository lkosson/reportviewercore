using System;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class ProcessingAbortEventArgs : EventArgs
	{
		private string m_uniqueName;

		internal string UniqueName => m_uniqueName;

		internal ProcessingAbortEventArgs(string uniqueName)
		{
			m_uniqueName = uniqueName;
		}
	}
}
