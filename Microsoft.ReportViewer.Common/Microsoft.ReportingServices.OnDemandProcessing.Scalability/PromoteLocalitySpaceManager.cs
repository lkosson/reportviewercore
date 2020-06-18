using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class PromoteLocalitySpaceManager : ISpaceManager
	{
		private long m_blockSize;

		private long m_position;

		private long m_streamEnd;

		private List<FileBlock> m_blocks;

		public long StreamEnd
		{
			get
			{
				return m_streamEnd;
			}
			set
			{
				m_streamEnd = value;
				m_position = value;
			}
		}

		internal PromoteLocalitySpaceManager(long blockSize)
		{
			m_blockSize = blockSize;
			m_position = 0L;
			m_streamEnd = 0L;
			m_blocks = new List<FileBlock>(10);
		}

		public void Seek(long offset, SeekOrigin origin)
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
				m_position = m_streamEnd + offset;
				break;
			default:
				Global.Tracer.Assert(condition: false);
				break;
			}
		}

		private FileBlock GetBlock(long offset)
		{
			FileBlock result = null;
			int num = (int)(offset / m_blockSize);
			if (num < m_blocks.Count)
			{
				result = m_blocks[num];
			}
			return result;
		}

		private FileBlock GetOrCreateBlock(long offset)
		{
			int num = (int)(offset / m_blockSize);
			if (num >= m_blocks.Count)
			{
				for (int i = m_blocks.Count - 1; i < num; i++)
				{
					m_blocks.Add(null);
				}
				m_blocks.Add(new FileBlock());
			}
			FileBlock fileBlock = m_blocks[num];
			if (fileBlock == null)
			{
				fileBlock = new FileBlock();
				m_blocks[num] = fileBlock;
			}
			return fileBlock;
		}

		public void Free(long offset, long size)
		{
			GetOrCreateBlock(offset).Free(offset, size);
		}

		public long AllocateSpace(long size)
		{
			long num = -1L;
			num = SearchBlock(m_position, size);
			long num2 = m_position - m_blockSize;
			long num3 = m_position + m_blockSize;
			while (num == -1 && (num2 >= 0 || num3 < m_streamEnd))
			{
				if (num2 >= 0)
				{
					num = SearchBlock(num2, size);
				}
				if (num3 < m_streamEnd && num == -1)
				{
					num = SearchBlock(num3, size);
				}
				num2 -= m_blockSize;
				num3 += m_blockSize;
			}
			if (num == -1)
			{
				num = m_streamEnd;
				m_streamEnd += size;
			}
			return num;
		}

		private long SearchBlock(long offset, long size)
		{
			long result = -1L;
			FileBlock block = GetBlock(offset);
			if (block != null)
			{
				result = block.AllocateSpace(size);
			}
			return result;
		}

		public long Resize(long offset, long oldSize, long newSize)
		{
			Free(offset, oldSize);
			return AllocateSpace(newSize);
		}

		public void TraceStats()
		{
			Global.Tracer.Trace(TraceLevel.Verbose, "LocalitySpaceManager Stats. StreamSize: {0} MB. FileBlocks:", m_streamEnd / 1048576);
			for (int i = 0; i < m_blocks.Count; i++)
			{
				m_blocks[i]?.TraceStats((i * m_blockSize / 1048576).ToString(CultureInfo.InvariantCulture));
			}
		}
	}
}
