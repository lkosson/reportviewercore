using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RenderingChunkManager
	{
		private string m_rendererID;

		private IChunkFactory m_chunkFactory;

		private Dictionary<string, Stream> m_chunks;

		internal RenderingChunkManager(string rendererID, IChunkFactory chunkFactory)
		{
			m_rendererID = rendererID;
			m_chunkFactory = chunkFactory;
			m_chunks = new Dictionary<string, Stream>();
		}

		internal Stream GetOrCreateChunk(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, string chunkName, bool createChunkIfNotExists, out bool isNewChunk)
		{
			isNewChunk = false;
			if (chunkName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "chunkName");
			}
			if (m_chunks.ContainsKey(chunkName))
			{
				return m_chunks[chunkName];
			}
			string mimeType;
			Stream stream = m_chunkFactory.GetChunk(chunkName, type, ChunkMode.Open, out mimeType);
			if (createChunkIfNotExists && stream == null)
			{
				stream = m_chunkFactory.CreateChunk(chunkName, type, null);
				isNewChunk = true;
			}
			if (stream != null)
			{
				m_chunks.Add(chunkName, stream);
			}
			return stream;
		}

		internal Stream CreateChunk(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, string chunkName)
		{
			if (chunkName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "chunkName");
			}
			if (m_chunks.TryGetValue(chunkName, out Stream value))
			{
				value.Close();
				m_chunks.Remove(chunkName);
			}
			value = m_chunkFactory.CreateChunk(chunkName, type, null);
			if (value != null)
			{
				m_chunks.Add(chunkName, value);
			}
			return value;
		}

		internal void CloseAllChunks()
		{
			foreach (Stream value in m_chunks.Values)
			{
				value.Close();
			}
			m_chunks.Clear();
		}
	}
}
