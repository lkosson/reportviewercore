using System;
using System.Diagnostics;

namespace Microsoft.ReportingServices.Rendering.RichText
{
	internal class PerfCheck : IDisposable
	{
		private string m_Name;

		private Stopwatch m_sw = new Stopwatch();

		internal PerfCheck(string Name)
		{
			m_Name = Name;
		}

		[Conditional("PERF_CHECK")]
		private void Start()
		{
			m_sw.Start();
		}

		[Conditional("PERF_CHECK")]
		private void Stop()
		{
			m_sw.Stop();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
