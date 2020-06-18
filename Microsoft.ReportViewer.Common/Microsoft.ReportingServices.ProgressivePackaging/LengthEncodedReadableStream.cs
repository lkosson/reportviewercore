using System;
using System.IO;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal class LengthEncodedReadableStream : Stream
	{
		private BinaryReader m_reader;

		private int m_length;

		private int m_position;

		private bool m_closed;

		public override bool CanRead => m_reader.BaseStream.CanRead;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length => m_length;

		public override long Position
		{
			get
			{
				return m_position;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal bool Closed => m_closed;

		internal LengthEncodedReadableStream(BinaryReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			m_reader = reader;
			m_length = m_reader.ReadInt32();
			m_position = 0;
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (m_length == m_position)
			{
				return 0;
			}
			int count2 = Math.Min(count, m_length - m_position);
			int num = m_reader.Read(buffer, offset, count2);
			m_position += num;
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override void Close()
		{
			if (!m_closed)
			{
				m_closed = true;
				base.Close();
			}
		}
	}
}
