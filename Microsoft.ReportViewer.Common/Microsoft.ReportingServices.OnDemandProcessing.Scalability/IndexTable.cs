using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class IndexTable : IIndexStrategy
	{
		private Dictionary<int, IndexTablePage> m_pageCache;

		private IndexTablePage m_queueFirstPage;

		private IndexTablePage m_queueLastPage;

		private long m_nextTempId;

		private int m_pageSize;

		private int m_cacheSize;

		private Stream m_stream;

		private IStreamHandler m_streamCreator;

		private long m_nextIdPageNum;

		private long m_nextIdPageSlot;

		private readonly int m_slotsPerPage;

		private readonly int m_idShift;

		private const int m_valueSize = 8;

		public ReferenceID MaxId => new ReferenceID((m_nextTempId + 1) * -1);

		public IndexTable(IStreamHandler streamCreator, int pageSize, int cacheSize)
		{
			if (pageSize % 8 != 0)
			{
				Global.Tracer.Assert(false, "Page size must be divisible by value size: {0}", 8);
			}
			m_streamCreator = streamCreator;
			m_stream = null;
			m_nextTempId = -1L;
			m_pageSize = pageSize;
			m_cacheSize = cacheSize;
			m_pageCache = new Dictionary<int, IndexTablePage>(m_cacheSize);
			m_queueFirstPage = null;
			m_queueLastPage = null;
			m_slotsPerPage = m_pageSize / 8;
			m_idShift = (int)Math.Log(m_slotsPerPage, 2.0);
		}

		public ReferenceID GenerateTempId()
		{
			return new ReferenceID(m_nextTempId--);
		}

		public ReferenceID GenerateId(ReferenceID tempId)
		{
			ReferenceID result = tempId;
			if (tempId.IsTemporary)
			{
				if (m_nextIdPageSlot >= m_slotsPerPage)
				{
					m_nextIdPageSlot = 0L;
					m_nextIdPageNum++;
				}
				long nextIdPageSlot = m_nextIdPageSlot;
				nextIdPageSlot |= m_nextIdPageNum << m_idShift;
				result = new ReferenceID(nextIdPageSlot);
				m_nextIdPageSlot++;
			}
			return result;
		}

		public void Update(ReferenceID id, long value)
		{
			IndexTablePage page = GetPage(id.Value);
			WriteValue(id.Value, page, value);
		}

		public long Retrieve(ReferenceID id)
		{
			IndexTablePage page = GetPage(id.Value);
			return ReadValue(id.Value, page);
		}

		public void Close()
		{
			if (m_stream != null)
			{
				m_stream.Close();
				m_stream = null;
			}
		}

		private IndexTablePage GetPage(long id)
		{
			int num = CalcPageNum(id);
			IndexTablePage value = null;
			if (!m_pageCache.TryGetValue(num, out value))
			{
				if (m_pageCache.Count == m_cacheSize)
				{
					if (m_stream == null)
					{
						m_stream = m_streamCreator.OpenStream();
						m_streamCreator = null;
						if (!m_stream.CanSeek || !m_stream.CanRead || !m_stream.CanWrite)
						{
							Global.Tracer.Assert(condition: false, "Must be able to Seek, Read, and Write stream");
						}
					}
					value = QueueExtractFirst();
					int pageNumber = value.PageNumber;
					m_pageCache.Remove(pageNumber);
					if (value.Dirty)
					{
						long offset = CalcPageOffset(pageNumber);
						m_stream.Seek(offset, SeekOrigin.Begin);
						value.Write(m_stream);
					}
					long offset2 = CalcPageOffset(num);
					m_stream.Seek(offset2, SeekOrigin.Begin);
					value.Read(m_stream);
				}
				else
				{
					value = new IndexTablePage(m_pageSize);
				}
				value.PageNumber = num;
				m_pageCache[num] = value;
				QueueAppendPage(value);
			}
			return value;
		}

		private long ReadValue(long id, IndexTablePage page)
		{
			long num = 0L;
			byte[] buffer = page.Buffer;
			int num2 = CalcValueOffset(id);
			int num3 = num2 + 8;
			for (int i = num2; i < num3; i++)
			{
				num <<= 8;
				num |= buffer[i];
			}
			return num;
		}

		private void WriteValue(long id, IndexTablePage page, long value)
		{
			byte[] buffer = page.Buffer;
			int num = CalcValueOffset(id);
			for (int num2 = num + 8 - 1; num2 >= num; num2--)
			{
				buffer[num2] = (byte)value;
				value >>= 8;
			}
			page.Dirty = true;
		}

		private int CalcPageNum(long id)
		{
			return (int)(id >> m_idShift);
		}

		private long CalcPageOffset(long pageNum)
		{
			return pageNum * m_pageSize;
		}

		private int CalcValueOffset(long id)
		{
			return (int)((ulong)(id << 64 - m_idShift) >> 64 - m_idShift) * 8;
		}

		private void QueueAppendPage(IndexTablePage page)
		{
			if (m_queueFirstPage == null)
			{
				m_queueFirstPage = page;
				m_queueLastPage = page;
			}
			else
			{
				page.PreviousPage = m_queueLastPage;
				m_queueLastPage.NextPage = page;
				m_queueLastPage = page;
			}
		}

		private IndexTablePage QueueExtractFirst()
		{
			if (m_queueFirstPage == null)
			{
				return null;
			}
			IndexTablePage queueFirstPage = m_queueFirstPage;
			m_queueFirstPage = queueFirstPage.NextPage;
			queueFirstPage.NextPage = null;
			if (m_queueFirstPage == null)
			{
				m_queueLastPage = null;
			}
			else
			{
				m_queueFirstPage.PreviousPage = null;
			}
			return queueFirstPage;
		}
	}
}
