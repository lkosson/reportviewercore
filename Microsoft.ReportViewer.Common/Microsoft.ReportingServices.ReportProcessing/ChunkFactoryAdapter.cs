using System.IO;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal class ChunkFactoryAdapter
	{
		private IChunkFactory m_chunkFactory;

		internal ChunkFactoryAdapter(IChunkFactory aFactory)
		{
			m_chunkFactory = aFactory;
		}

		public Stream CreateReportChunk(string name, ReportProcessing.ReportChunkTypes type, string mimeType)
		{
			return m_chunkFactory.CreateChunk(name, type, mimeType);
		}

		public Stream GetReportChunk(string name, ReportProcessing.ReportChunkTypes type, out string mimeType)
		{
			return m_chunkFactory.GetChunk(name, type, ChunkMode.Open, out mimeType);
		}

		public string GetChunkMimeType(string name, ReportProcessing.ReportChunkTypes type)
		{
			m_chunkFactory.GetChunk(name, type, ChunkMode.Open, out string mimeType).Close();
			return mimeType;
		}
	}
}
