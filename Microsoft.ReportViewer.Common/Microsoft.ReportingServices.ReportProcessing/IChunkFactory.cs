using System.IO;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal interface IChunkFactory
	{
		ReportProcessingFlags ReportProcessingFlags
		{
			get;
		}

		Stream CreateChunk(string chunkName, ReportProcessing.ReportChunkTypes type, string mimeType);

		Stream GetChunk(string chunkName, ReportProcessing.ReportChunkTypes type, ChunkMode mode, out string mimeType);

		bool Erase(string chunkName, ReportProcessing.ReportChunkTypes type);
	}
}
