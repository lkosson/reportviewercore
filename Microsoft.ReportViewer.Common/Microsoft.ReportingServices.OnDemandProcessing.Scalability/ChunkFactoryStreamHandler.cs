using Microsoft.ReportingServices.ReportProcessing;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ChunkFactoryStreamHandler : IStreamHandler
	{
		private string m_chunkName;

		private Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes m_chunkType;

		private IChunkFactory m_chunkFactory;

		private bool m_existingChunk;

		internal ChunkFactoryStreamHandler(string chunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes chunkType, IChunkFactory chunkFactory, bool existingChunk)
		{
			m_chunkName = chunkName;
			m_chunkType = chunkType;
			m_chunkFactory = chunkFactory;
			m_existingChunk = existingChunk;
		}

		public Stream OpenStream()
		{
			string mimeType;
			if (m_existingChunk)
			{
				return m_chunkFactory.GetChunk(m_chunkName, m_chunkType, ChunkMode.Open, out mimeType);
			}
			return m_chunkFactory.CreateChunk(m_chunkName, m_chunkType, null);
		}
	}
}
