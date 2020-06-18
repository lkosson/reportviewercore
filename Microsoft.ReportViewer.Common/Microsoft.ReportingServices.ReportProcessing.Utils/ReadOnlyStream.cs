using System;
using System.IO;

namespace Microsoft.ReportingServices.ReportProcessing.Utils
{
	internal class ReadOnlyStream : Stream
	{
		private readonly Stream m_underlyingStream;

		private readonly bool m_canCloseUnderlyingStream;

		public override bool CanRead => true;

		public override bool CanSeek => m_underlyingStream.CanSeek;

		public override bool CanWrite => false;

		public override long Length => m_underlyingStream.Length;

		public override long Position
		{
			get
			{
				return m_underlyingStream.Position;
			}
			set
			{
				throw new InvalidOperationException("This Stream does not support this operation.");
			}
		}

		public ReadOnlyStream(Stream underlyingStream, bool canCloseUnderlyingStream)
		{
			if (underlyingStream == null)
			{
				throw new ArgumentNullException("underlyingStream");
			}
			m_underlyingStream = underlyingStream;
			m_canCloseUnderlyingStream = canCloseUnderlyingStream;
		}

		public override void Flush()
		{
			throw new InvalidOperationException("This Stream does not support this operation.");
		}

		public override int ReadByte()
		{
			return m_underlyingStream.ReadByte();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return m_underlyingStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return m_underlyingStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			throw new InvalidOperationException("This Stream does not support this operation.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException("This Stream does not support this operation.");
		}

		public override void Close()
		{
			if (m_canCloseUnderlyingStream)
			{
				m_underlyingStream.Close();
			}
		}
	}
}
