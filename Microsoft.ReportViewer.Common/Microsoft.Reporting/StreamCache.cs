using Microsoft.ReportingServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Reporting
{
	[Serializable]
	internal sealed class StreamCache : IDisposable
	{
		private CachedStream m_mainStream;

		private bool m_mainStreamDetached;

		private Dictionary<string, CachedStream> m_secondaryStreams = new Dictionary<string, CachedStream>();

		private CreateStreamDelegate m_createMainStreamDelegate;

		public StreamCache()
			: this(null)
		{
		}

		public StreamCache(CreateStreamDelegate createMainStreamDelegate)
		{
			m_createMainStreamDelegate = (createMainStreamDelegate ?? new CreateStreamDelegate(DefaultCreateStreamDelegate));
		}

		public void Dispose()
		{
			Clear();
		}

		public void Clear()
		{
			if (!m_mainStreamDetached && m_mainStream != null)
			{
				m_mainStream.Dispose();
			}
			foreach (CachedStream value in m_secondaryStreams.Values)
			{
				value.Dispose();
			}
			m_mainStream = null;
			m_mainStreamDetached = false;
			m_secondaryStreams.Clear();
		}

		public Stream StreamCallback(string name, string extension, Encoding encoding, string mimeType, bool useChunking, StreamOper operation)
		{
			if (operation == StreamOper.RegisterOnly)
			{
				return null;
			}
			bool flag = operation == StreamOper.CreateAndRegister && m_mainStream == null && !m_mainStreamDetached;
			CachedStream cachedStream = new CachedStream((flag ? m_createMainStreamDelegate : new CreateStreamDelegate(DefaultCreateStreamDelegate))(), encoding, mimeType, extension);
			if (operation == StreamOper.CreateAndRegister)
			{
				if (flag)
				{
					m_mainStream = cachedStream;
				}
				else
				{
					m_secondaryStreams.Add(name, cachedStream);
				}
			}
			return cachedStream.Stream;
		}

		public Stream GetMainStream(bool detach)
		{
			string encoding;
			string mimeType;
			string fileExtension;
			return GetMainStream(detach, out encoding, out mimeType, out fileExtension);
		}

		public Stream GetMainStream(bool detach, out string encoding, out string mimeType, out string fileExtension)
		{
			Stream result = CachedStream.Extract(m_mainStream, out encoding, out mimeType, out fileExtension);
			if (detach)
			{
				m_mainStreamDetached = detach;
				m_mainStream = null;
			}
			return result;
		}

		public byte[] GetMainStream(out string encoding, out string mimeType, out string fileExtension)
		{
			Stream mainStream = GetMainStream(detach: false, out encoding, out mimeType, out fileExtension);
			return StreamToBytes(mainStream);
		}

		public byte[] GetSecondaryStream(bool remove, string name, out string encoding, out string mimeType, out string fileExtension)
		{
			CachedStream value;
			bool num = m_secondaryStreams.TryGetValue(name, out value);
			Stream stream = CachedStream.Extract(value, out encoding, out mimeType, out fileExtension);
			byte[] result = StreamToBytes(stream);
			if (num && remove)
			{
				m_secondaryStreams.Remove(name);
				value.Dispose();
			}
			return result;
		}

		public void MoveSecondaryStreamsTo(StreamCache other)
		{
			foreach (KeyValuePair<string, CachedStream> secondaryStream in m_secondaryStreams)
			{
				other.m_secondaryStreams.Add(secondaryStream.Key, secondaryStream.Value);
			}
			m_secondaryStreams.Clear();
		}

		private byte[] StreamToBytes(Stream stream)
		{
			if (stream != null)
			{
				byte[] array = new byte[stream.Length];
				for (int i = 0; i < stream.Length; i += stream.Read(array, i, (int)stream.Length))
				{
				}
				return array;
			}
			return null;
		}

		private static Stream DefaultCreateStreamDelegate()
		{
			return new MemoryStream();
		}
	}
}
