using System;
using System.IO;
using System.Text;

namespace Microsoft.Reporting
{
	[Serializable]
	internal sealed class CachedStream : IDisposable
	{
		private Stream m_stream;

		private Encoding m_encoding;

		private string m_mimeType;

		private string m_fileExtension;

		public Stream Stream
		{
			get
			{
				m_stream.Seek(0L, SeekOrigin.Begin);
				return m_stream;
			}
		}

		public Encoding Encoding => m_encoding;

		public string MimeType => m_mimeType;

		public string FileExtension => m_fileExtension;

		public CachedStream(Stream stream, Encoding encoding, string mimeType, string fileExtension)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			m_stream = stream;
			m_encoding = encoding;
			m_mimeType = mimeType;
			m_fileExtension = fileExtension;
		}

		public static Stream Extract(CachedStream cachedStream, out string encoding, out string mimeType, out string fileExtension)
		{
			if (cachedStream != null)
			{
				if (cachedStream.Encoding == null)
				{
					encoding = null;
				}
				else
				{
					encoding = cachedStream.Encoding.EncodingName;
				}
				mimeType = cachedStream.MimeType;
				fileExtension = cachedStream.FileExtension;
				return cachedStream.Stream;
			}
			encoding = null;
			mimeType = null;
			fileExtension = null;
			return null;
		}

		public void Dispose()
		{
			m_stream.Dispose();
		}
	}
}
