using System;
using System.IO;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal sealed class LengthEncodedWritableStream : Stream
	{
		private MemoryStream m_bufferStream;

		private BinaryWriter m_writer;

		private string m_name;

		private bool m_closed;

		private long m_length;

		public override bool CanRead => false;

		public override bool CanSeek => true;

		public override bool CanWrite => m_writer.BaseStream.CanWrite;

		public override long Length => m_length;

		public override long Position
		{
			get
			{
				return m_length;
			}
			set
			{
				throw new NotSupportedException("LengthEncodedWritableStream.set_Position");
			}
		}

		internal bool Closed => m_closed;

		internal LengthEncodedWritableStream(BinaryWriter writer, string name)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}
			m_writer = writer;
			m_name = name;
			m_bufferStream = new MemoryStream();
			m_length = 0L;
		}

		public override void Flush()
		{
			m_bufferStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("LengthEncodedWritableStream.Read");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("LengthEncodedWritableStream.Seek");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("LengthEncodedWritableStream.SetLength");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			m_bufferStream.Write(buffer, offset, count);
			m_length += count;
		}

		public override void Close()
		{
			if (!m_closed)
			{
				m_closed = true;
				try
				{
					m_writer.Write(m_name);
					MessageUtil.WriteByteArray(m_writer, m_bufferStream.GetBuffer(), 0, checked((int)m_bufferStream.Length));
				}
				finally
				{
					m_bufferStream.Close();
					base.Close();
				}
			}
		}
	}
}
