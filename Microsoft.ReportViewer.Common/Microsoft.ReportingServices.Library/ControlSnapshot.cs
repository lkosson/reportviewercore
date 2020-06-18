using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Library
{
	[Serializable]
	internal sealed class ControlSnapshot : SnapshotBase, IChunkFactory, IDisposable
	{
		[Serializable]
		private class Chunk : IDisposable
		{
			private ChunkHeader m_header;

			private ChunkMemoryStream m_stream = new ChunkMemoryStream();

			public ChunkHeader Header => m_header;

			public ChunkMemoryStream Stream => m_stream;

			public Chunk(string mimeType, string name, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type)
			{
				m_header = new ChunkHeader(name, (int)type, ChunkFlags.None, mimeType, ChunkHeader.CurrentVersion, 0L);
			}

			public Chunk(Chunk baseChunk)
			{
				m_header = new ChunkHeader(baseChunk.Header);
				m_stream = baseChunk.Stream;
			}

			public void Dispose()
			{
				if (m_stream != null)
				{
					m_stream.Dispose();
					m_stream = null;
				}
				GC.SuppressFinalize(this);
			}
		}

		private IList<Chunk> m_allChunks = new List<Chunk>();

		public ReportProcessingFlags ReportProcessingFlags => ReportProcessingFlags.OnDemandEngine;

		public ControlSnapshot()
			: base(isPermanentSnapshot: false)
		{
		}

		public void Dispose()
		{
			DeleteSnapshotAndChunks();
			GC.SuppressFinalize(this);
		}

		public override SnapshotBase Duplicate()
		{
			throw new NotImplementedException();
		}

		private void CopyAllChunksTo(ControlSnapshot target)
		{
			foreach (Chunk allChunk in m_allChunks)
			{
				if (!target.m_allChunks.Contains(allChunk))
				{
					target.m_allChunks.Add(allChunk);
				}
			}
		}

		public void CopyDataChunksTo(IChunkFactory chunkFactory, out bool hasDataChunks)
		{
			hasDataChunks = false;
			foreach (Chunk allChunk in m_allChunks)
			{
				if (allChunk.Header.ChunkType == 5)
				{
					hasDataChunks = true;
					using (Stream to = chunkFactory.CreateChunk(allChunk.Header.ChunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Data, null))
					{
						allChunk.Stream.Position = 0L;
						StreamSupport.CopyStreamUsingBuffer(allChunk.Stream, to, 4096);
					}
				}
			}
		}

		public override void PrepareExecutionSnapshot(SnapshotBase target, string compiledDefinitionChunkName)
		{
			ControlSnapshot controlSnapshot = (ControlSnapshot)target;
			int imageChunkTypeToCopy = (int)Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetImageChunkTypeToCopy(ReportProcessingFlags.OnDemandEngine);
			string text = compiledDefinitionChunkName;
			if (text == null)
			{
				text = "CompiledDefinition";
			}
			foreach (Chunk allChunk in m_allChunks)
			{
				ChunkHeader header = allChunk.Header;
				if (header.ChunkType == imageChunkTypeToCopy)
				{
					controlSnapshot.m_allChunks.Add(allChunk);
				}
				else if (header.ChunkName.Equals("CompiledDefinition", StringComparison.Ordinal))
				{
					Chunk chunk = new Chunk(allChunk);
					chunk.Header.ChunkName = text;
					controlSnapshot.m_allChunks.Add(chunk);
				}
				else if (header.ChunkType == 5 || header.ChunkType == 9)
				{
					Chunk item = new Chunk(allChunk);
					controlSnapshot.m_allChunks.Add(item);
				}
			}
		}

		[Obsolete("Use PrepareExecutionSnapshot instead")]
		public override void CopyImageChunksTo(SnapshotBase target)
		{
			ControlSnapshot target2 = (ControlSnapshot)target;
			CopyAllChunksTo(target2);
		}

		public override void DeleteSnapshotAndChunks()
		{
			foreach (Chunk allChunk in m_allChunks)
			{
				allChunk.Stream.CanBeClosed = true;
				allChunk.Dispose();
			}
			m_allChunks.Clear();
		}

		public override Stream GetChunk(string name, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, out string mimeType)
		{
			Chunk chunkImpl = GetChunkImpl(name, type);
			if (chunkImpl == null)
			{
				mimeType = null;
				return null;
			}
			mimeType = chunkImpl.Header.MimeType;
			chunkImpl.Stream.Seek(0L, SeekOrigin.Begin);
			if (chunkImpl.Header.ChunkFlag == ChunkFlags.Compressed)
			{
				throw new InternalCatalogException("Cannot read compressed chunk.");
			}
			return chunkImpl.Stream;
		}

		public override string GetStreamMimeType(string name, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type)
		{
			return GetChunkImpl(name, type)?.Header.MimeType;
		}

		public override Stream CreateChunk(string name, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, string mimeType)
		{
			Erase(name, type);
			Chunk chunk = new Chunk(mimeType, name, type);
			m_allChunks.Add(chunk);
			return chunk.Stream;
		}

		public Stream GetChunk(string chunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes chunkType, ChunkMode chunkMode, out string mimeType)
		{
			Stream stream = GetChunk(chunkName, chunkType, out mimeType);
			if (chunkMode == ChunkMode.OpenOrCreate && stream == null)
			{
				mimeType = null;
				stream = CreateChunk(chunkName, chunkType, mimeType);
			}
			return stream;
		}

		public bool Erase(string chunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type)
		{
			foreach (Chunk allChunk in m_allChunks)
			{
				if (allChunk.Header.ChunkName == chunkName && allChunk.Header.ChunkType == (int)type)
				{
					m_allChunks.Remove(allChunk);
					return true;
				}
			}
			return false;
		}

		private Chunk GetChunkImpl(string name, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type)
		{
			foreach (Chunk allChunk in m_allChunks)
			{
				if (allChunk.Header.ChunkName == name && allChunk.Header.ChunkType == (int)type)
				{
					return allChunk;
				}
			}
			return null;
		}
	}
}
