using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class PageBufferedStream : Stream
	{
		private sealed class CachePage
		{
			internal byte[] Buffer;

			internal bool Dirty;

			internal CachePage NextPage;

			internal long PageNumber;

			public CachePage(int size, long pageNum)
			{
				Buffer = new byte[size];
				Dirty = false;
				NextPage = null;
				PageNumber = pageNum;
			}

			public void Read(Stream stream)
			{
				int num = 0;
				int num2 = Buffer.Length;
				int num3 = 0;
				do
				{
					num3 = stream.Read(Buffer, num, num2);
					num += num3;
					num2 -= num3;
				}
				while (num3 > 0 && num2 > 0);
				Global.Tracer.Assert(num == Buffer.Length, "Error filling buffer page");
				Dirty = false;
			}

			internal void InitBuffer()
			{
				Dirty = false;
			}

			public void Write(Stream stream)
			{
				stream.Write(Buffer, 0, Buffer.Length);
				Dirty = false;
			}
		}

		private readonly int m_bytesPerPage;

		private readonly int m_pageCacheCapacity;

		private Stream m_stream;

		private Dictionary<long, CachePage> m_pageCache;

		private CachePage m_firstPageToEvict;

		private CachePage m_lastPageToEvict;

		private long m_position;

		private long m_length;

		private bool m_freezePageAllocations;

		public bool FreezePageAllocations
		{
			get
			{
				return m_freezePageAllocations;
			}
			set
			{
				m_freezePageAllocations = value;
			}
		}

		internal int PageCount => m_pageCache.Count;

		public override bool CanRead => true;

		public override bool CanSeek => true;

		public override bool CanWrite => true;

		public override long Length => m_length;

		public override long Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}

		public PageBufferedStream(Stream stream, int bytesPerPage, int cachePageCount)
		{
			if (!stream.CanSeek || !stream.CanRead || !stream.CanWrite)
			{
				Global.Tracer.Assert(condition: false, "PageBufferedStream: Must be able to Seek, Read, and Write stream");
			}
			m_bytesPerPage = bytesPerPage;
			m_pageCacheCapacity = cachePageCount;
			m_stream = stream;
			m_length = stream.Length;
			m_pageCache = new Dictionary<long, CachePage>(m_pageCacheCapacity);
		}

		public override void Flush()
		{
			foreach (long key in m_pageCache.Keys)
			{
				FlushPage(m_pageCache[key], key);
			}
		}

		public override int ReadByte()
		{
			CachePage page = GetPage(m_position);
			int num = CalcOffsetWithinPage(m_position);
			UpdatePosition(1L);
			return page.Buffer[num];
		}

		public override void WriteByte(byte value)
		{
			CachePage page = GetPage(m_position);
			int num = CalcOffsetWithinPage(m_position);
			UpdatePosition(1L);
			page.Buffer[num] = value;
			page.Dirty = true;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int result = count;
			long num = m_position + count;
			while (m_position < num)
			{
				CachePage page = GetPage(m_position);
				int num2 = CalcOffsetWithinPage(m_position);
				byte[] buffer2 = page.Buffer;
				int num3 = Math.Min(buffer2.Length - num2, count);
				Array.Copy(buffer2, num2, buffer, offset, num3);
				UpdatePosition(num3);
				count -= num3;
				offset += num3;
			}
			return result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				m_position = offset;
				break;
			case SeekOrigin.Current:
				m_position += offset;
				break;
			case SeekOrigin.End:
				m_position = m_length + offset;
				break;
			default:
				Global.Tracer.Assert(condition: false);
				break;
			}
			return m_position;
		}

		public override void SetLength(long value)
		{
			m_length = value;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			long num = m_position + count;
			while (m_position < num)
			{
				CachePage page = GetPage(m_position);
				int num2 = CalcOffsetWithinPage(m_position);
				byte[] buffer2 = page.Buffer;
				int num3 = Math.Min(buffer2.Length - num2, count);
				Array.Copy(buffer, offset, buffer2, num2, num3);
				UpdatePosition(num3);
				count -= num3;
				offset += num3;
				page.Dirty = true;
			}
		}

		public override void Close()
		{
			m_stream.Close();
		}

		private void UpdatePosition(long moveBy)
		{
			m_position += moveBy;
			if (m_position > m_length)
			{
				m_length = m_position;
			}
		}

		private CachePage GetPage(long fileOffset)
		{
			long num = CalcPageNum(fileOffset);
			CachePage value = null;
			if (!m_pageCache.TryGetValue(num, out value))
			{
				bool flag = false;
				if (m_pageCache.Count == m_pageCacheCapacity || (m_freezePageAllocations && m_pageCache.Count > 0))
				{
					value = m_firstPageToEvict;
					long pageNumber = value.PageNumber;
					m_firstPageToEvict = value.NextPage;
					m_pageCache.Remove(pageNumber);
					FlushPage(value, pageNumber);
					value.PageNumber = num;
				}
				else
				{
					value = new CachePage(m_bytesPerPage, num);
					flag = true;
				}
				long num2 = CalcPageOffset(num);
				if (num2 < m_length)
				{
					m_stream.Seek(num2, SeekOrigin.Begin);
					value.Read(m_stream);
				}
				else if (!flag)
				{
					value.InitBuffer();
				}
				m_pageCache[num] = value;
				if (m_firstPageToEvict == null)
				{
					m_firstPageToEvict = value;
				}
				if (m_lastPageToEvict != null)
				{
					m_lastPageToEvict.NextPage = value;
				}
				m_lastPageToEvict = value;
			}
			return value;
		}

		private void FlushPage(CachePage page, long pageNum)
		{
			if (page.Dirty)
			{
				long offset = CalcPageOffset(pageNum);
				m_stream.Seek(offset, SeekOrigin.Begin);
				page.Write(m_stream);
			}
		}

		private long CalcPageNum(long fileOffset)
		{
			return fileOffset / m_bytesPerPage;
		}

		private long CalcPageOffset(long pageNum)
		{
			return pageNum * m_bytesPerPage;
		}

		private int CalcOffsetWithinPage(long fileOffset)
		{
			return (int)(fileOffset % m_bytesPerPage);
		}
	}
}
