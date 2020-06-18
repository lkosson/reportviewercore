using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class RIFAppendOnlyStorage : IStorage, IDisposable
	{
		private Stream m_stream;

		private IntermediateFormatWriter m_writer;

		private IntermediateFormatReader m_reader;

		private IScalabilityCache m_scalabilityCache;

		private bool m_writerSetup;

		private IStreamHandler m_streamCreator;

		private IReferenceCreator m_referenceCreator;

		private UnifiedObjectCreator m_unifiedObjectCreator;

		private bool m_fromExistingStream;

		private GlobalIDOwnerCollection m_globalIdsFromOtherStream;

		private readonly int m_rifCompatVersion;

		private bool m_atEnd;

		private long m_streamLength = -1L;

		private readonly bool m_prohibitSerializableValues;

		internal bool FromExistingStream => m_fromExistingStream;

		public long StreamSize => m_streamLength;

		public IScalabilityCache ScalabilityCache
		{
			get
			{
				return m_scalabilityCache;
			}
			set
			{
				m_scalabilityCache = value;
			}
		}

		public IReferenceCreator ReferenceCreator => m_referenceCreator;

		public bool FreezeAllocations
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		internal RIFAppendOnlyStorage(IStreamHandler streamHandler, IScalabilityObjectCreator appObjectCreator, IReferenceCreator appReferenceCreator, GlobalIDOwnerCollection globalIdsFromOtherStream, bool fromExistingStream, int rifCompatVersion, bool prohibitSerializableValues)
		{
			m_streamCreator = streamHandler;
			m_scalabilityCache = null;
			m_stream = null;
			m_unifiedObjectCreator = new UnifiedObjectCreator(appObjectCreator, appReferenceCreator);
			m_referenceCreator = new UnifiedReferenceCreator(appReferenceCreator);
			m_fromExistingStream = fromExistingStream;
			m_globalIdsFromOtherStream = globalIdsFromOtherStream;
			m_prohibitSerializableValues = prohibitSerializableValues;
			m_rifCompatVersion = rifCompatVersion;
		}

		private void SetupStorage()
		{
			if (m_stream != null)
			{
				return;
			}
			m_stream = m_streamCreator.OpenStream();
			m_streamCreator = null;
			List<Declaration> declarations = m_unifiedObjectCreator.GetDeclarations();
			if (m_fromExistingStream)
			{
				m_reader = new IntermediateFormatReader(m_stream, m_unifiedObjectCreator, m_globalIdsFromOtherStream, m_scalabilityCache);
				if (m_stream.CanWrite)
				{
					m_writer = new IntermediateFormatWriter(m_stream, m_stream.Length, declarations, m_scalabilityCache, m_rifCompatVersion, m_prohibitSerializableValues);
					m_writerSetup = true;
				}
				m_atEnd = false;
			}
			else
			{
				m_writer = new IntermediateFormatWriter(m_stream, declarations, m_scalabilityCache, m_rifCompatVersion, m_prohibitSerializableValues);
				m_writerSetup = true;
				m_reader = new IntermediateFormatReader(m_stream, m_unifiedObjectCreator, m_globalIdsFromOtherStream, m_scalabilityCache, declarations, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatVersion.Current, PersistenceFlags.Seekable);
				m_atEnd = true;
			}
			m_fromExistingStream = true;
		}

		public IPersistable Retrieve(long offset, out long persistedSize)
		{
			SetupStorage();
			m_stream.Seek(offset, SeekOrigin.Begin);
			m_atEnd = false;
			long position = m_stream.Position;
			IPersistable result = m_reader.ReadRIFObject();
			persistedSize = m_stream.Position - position;
			return result;
		}

		public IPersistable Retrieve(long offset)
		{
			long persistedSize;
			return Retrieve(offset, out persistedSize);
		}

		public T Retrieve<T>(long offset, out long persistedSize) where T : IPersistable, new()
		{
			SetupStorage();
			m_stream.Seek(offset, SeekOrigin.Begin);
			m_atEnd = false;
			long position = m_stream.Position;
			T result = m_reader.ReadRIFObject<T>();
			persistedSize = m_stream.Position - position;
			return result;
		}

		public long Allocate(IPersistable obj)
		{
			SetupStorage();
			if (!m_atEnd)
			{
				m_stream.Seek(0L, SeekOrigin.End);
				m_atEnd = true;
			}
			m_streamLength = m_stream.Position;
			m_writer.Write(obj);
			return m_streamLength;
		}

		public void Free(long offset, int size)
		{
			Global.Tracer.Assert(condition: false, "RIFAppendOnlyStorage does not support Free(...)");
		}

		public long Update(IPersistable obj, long offset, long oldPersistedSize)
		{
			Global.Tracer.Assert(condition: false, "RIFAppendOnlyStorage does not support Update(...)s");
			return -1L;
		}

		public void Close()
		{
			if (m_stream != null)
			{
				m_stream.Close();
				m_stream = null;
			}
		}

		public void Flush()
		{
			if (m_stream != null)
			{
				m_stream.Flush();
			}
		}

		internal void Reset(IStreamHandler streamHandler)
		{
			Close();
			m_writerSetup = false;
			m_streamCreator = streamHandler;
		}

		public void TraceStats()
		{
		}

		public void Dispose()
		{
			Close();
		}
	}
}
