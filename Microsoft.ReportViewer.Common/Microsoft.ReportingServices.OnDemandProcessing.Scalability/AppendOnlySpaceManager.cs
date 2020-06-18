using Microsoft.ReportingServices.ReportProcessing;
using System.Diagnostics;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class AppendOnlySpaceManager : ISpaceManager
	{
		private long m_streamEnd;

		private long m_unuseableBytes;

		public long StreamEnd
		{
			get
			{
				return m_streamEnd;
			}
			set
			{
				m_streamEnd = value;
			}
		}

		internal AppendOnlySpaceManager()
		{
			m_streamEnd = 0L;
		}

		public void Seek(long offset, SeekOrigin origin)
		{
		}

		public void Free(long offset, long size)
		{
			m_unuseableBytes += size;
		}

		public long AllocateSpace(long size)
		{
			long streamEnd = m_streamEnd;
			m_streamEnd += size;
			return streamEnd;
		}

		public long Resize(long offset, long oldSize, long newSize)
		{
			Free(offset, oldSize);
			return AllocateSpace(newSize);
		}

		public void TraceStats()
		{
			Global.Tracer.Trace(TraceLevel.Verbose, "AppendOnlySpaceManager Stats. StreamSize: {0} MB. UnusuableSpace: {1} KB.", m_streamEnd / 1048576, m_unuseableBytes / 1024);
		}
	}
}
