using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class UnPinCascadeHolder : IDisposable
	{
		private List<IDisposable> m_cleanupRefs;

		internal UnPinCascadeHolder()
		{
			m_cleanupRefs = new List<IDisposable>();
		}

		internal void AddCleanupRef(IDisposable cleanupRef)
		{
			m_cleanupRefs.Add(cleanupRef);
		}

		public void Dispose()
		{
			for (int i = 0; i < m_cleanupRefs.Count; i++)
			{
				m_cleanupRefs[i].Dispose();
			}
		}
	}
}
