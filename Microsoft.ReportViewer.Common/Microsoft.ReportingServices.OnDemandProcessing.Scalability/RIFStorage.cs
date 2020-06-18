using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class RIFStorage : IStorage, IDisposable
	{
		private PageBufferedStream m_stream;

		private MemoryStream m_memoryStream;

		private IntermediateFormatWriter m_writer;

		private IntermediateFormatReader m_reader;

		private int m_bufferPageSize;

		private int m_bufferPageCount;

		private int m_tempStreamSize;

		private IScalabilityCache m_scalabilityCache;

		private IStreamHandler m_streamCreator;

		private ISpaceManager m_spaceManager;

		private IReferenceCreator m_referenceCreator;

		private UnifiedObjectCreator m_unifiedObjectCreator;

		private bool m_fromExistingStream;

		private GlobalIDOwnerCollection m_globalIdsFromOtherStream;

		private bool m_freezeAllocations;

		private readonly int m_rifCompatVersion;

		public long StreamSize => m_spaceManager.StreamEnd;

		public IScalabilityCache ScalabilityCache
		{
			get
			{
				return m_scalabilityCache;
			}
			set
			{
				m_scalabilityCache = value;
				m_unifiedObjectCreator.ScalabilityCache = value;
			}
		}

		public IReferenceCreator ReferenceCreator => m_referenceCreator;

		public bool FreezeAllocations
		{
			get
			{
				return m_freezeAllocations;
			}
			set
			{
				m_freezeAllocations = value;
				if (m_stream != null)
				{
					m_stream.FreezePageAllocations = value;
				}
			}
		}

		public RIFStorage(IStreamHandler streamHandler, int bufferPageSize, int bufferPageCount, int tempStreamSize, ISpaceManager spaceManager, IScalabilityObjectCreator appObjectCreator, IReferenceCreator appReferenceCreator, GlobalIDOwnerCollection globalIdsFromOtherStream, bool fromExistingStream, int rifCompatVersion)
		{
			m_streamCreator = streamHandler;
			m_scalabilityCache = null;
			m_bufferPageSize = bufferPageSize;
			m_bufferPageCount = bufferPageCount;
			m_tempStreamSize = tempStreamSize;
			m_stream = null;
			m_spaceManager = spaceManager;
			m_unifiedObjectCreator = new UnifiedObjectCreator(appObjectCreator, appReferenceCreator);
			m_referenceCreator = new UnifiedReferenceCreator(appReferenceCreator);
			m_fromExistingStream = fromExistingStream;
			m_globalIdsFromOtherStream = globalIdsFromOtherStream;
			m_rifCompatVersion = rifCompatVersion;
		}

		private void SetupStorage()
		{
			if (m_stream == null)
			{
				Stream stream = m_streamCreator.OpenStream();
				m_streamCreator = null;
				m_stream = new PageBufferedStream(stream, m_bufferPageSize, m_bufferPageCount);
				m_stream.FreezePageAllocations = m_freezeAllocations;
				List<Declaration> declarations = m_unifiedObjectCreator.GetDeclarations();
				m_memoryStream = new MemoryStream(m_tempStreamSize);
				m_writer = new IntermediateFormatWriter(m_memoryStream, declarations, m_scalabilityCache, m_rifCompatVersion);
				if (m_fromExistingStream)
				{
					m_spaceManager.StreamEnd = m_stream.Length;
					m_reader = new IntermediateFormatReader(m_stream, m_unifiedObjectCreator, m_globalIdsFromOtherStream, m_scalabilityCache, declarations, IntermediateFormatVersion.Current, PersistenceFlags.Seekable);
				}
				else
				{
					m_spaceManager.StreamEnd = m_stream.Position;
					m_reader = new IntermediateFormatReader(m_stream, m_unifiedObjectCreator, m_globalIdsFromOtherStream, m_scalabilityCache, declarations, IntermediateFormatVersion.Current, PersistenceFlags.Seekable);
				}
			}
		}

		public IPersistable Retrieve(long offset)
		{
			long persistedSize;
			return Retrieve(offset, out persistedSize);
		}

		public IPersistable Retrieve(long offset, out long persistedSize)
		{
			SetupStorage();
			Seek(offset, SeekOrigin.Begin);
			IPersistable persistable = m_reader.ReadRIFObject();
			persistedSize = CalculatePersistedSize(persistable, offset);
			return persistable;
		}

		public T Retrieve<T>(long offset, out long persistedSize) where T : IPersistable, new()
		{
			SetupStorage();
			Seek(offset, SeekOrigin.Begin);
			T val = m_reader.ReadRIFObject<T>();
			persistedSize = CalculatePersistedSize(val, offset);
			return val;
		}

		private long CalculatePersistedSize(IPersistable item, long offset)
		{
			return m_stream.Position - offset;
		}

		public long Allocate(IPersistable obj)
		{
			return WriteObject(obj, -1L, -1L);
		}

		private long SeekToFreeSpace(long size)
		{
			long num = m_spaceManager.AllocateSpace(size);
			Seek(num, SeekOrigin.Begin);
			return num;
		}

		public long ReserveSpace(int length)
		{
			SetupStorage();
			return m_spaceManager.AllocateSpace(length);
		}

		public long Update(IPersistable obj, long offset, long oldPersistedSize)
		{
			return WriteObject(obj, offset, oldPersistedSize);
		}

		private long WriteObject(IPersistable obj, long offset, long oldSize)
		{
			SetupStorage();
			m_memoryStream.Seek(0L, SeekOrigin.Begin);
			m_writer.Write(obj);
			long position = m_memoryStream.Position;
			if (oldSize < 0 || offset < 0)
			{
				offset = SeekToFreeSpace(position);
			}
			else if (position != oldSize)
			{
				offset = m_spaceManager.Resize(offset, oldSize, position);
				Seek(offset, SeekOrigin.Begin);
			}
			else
			{
				Seek(offset, SeekOrigin.Begin);
			}
			m_stream.Write(m_memoryStream.GetBuffer(), 0, (int)position);
			return offset;
		}

		public void Free(long offset, int size)
		{
			m_spaceManager.Free(offset, size);
		}

		public void Close()
		{
			if (m_stream != null)
			{
				m_stream.Close();
				m_stream = null;
				m_memoryStream.Close();
				m_memoryStream = null;
			}
		}

		public void Flush()
		{
			if (m_stream != null)
			{
				m_stream.Flush();
			}
		}

		public void Dispose()
		{
			Close();
		}

		private void Seek(long offset, SeekOrigin origin)
		{
			m_stream.Seek(offset, origin);
			m_spaceManager.Seek(offset, origin);
		}

		public void TraceStats()
		{
			m_spaceManager.TraceStats();
		}
	}
}
