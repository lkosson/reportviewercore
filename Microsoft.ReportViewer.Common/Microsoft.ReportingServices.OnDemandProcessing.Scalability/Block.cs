namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class Block
	{
		private long m_offset;

		private long m_endOffset;

		private long m_size;

		public long Offset
		{
			get
			{
				return m_offset;
			}
			set
			{
				m_offset = value;
			}
		}

		public long EndOffset
		{
			get
			{
				return m_endOffset;
			}
			set
			{
				m_endOffset = value;
			}
		}

		public long Size
		{
			get
			{
				return m_size;
			}
			set
			{
				m_size = value;
			}
		}

		public Block(long offset, long size)
		{
			m_offset = offset;
			m_size = size;
		}
	}
}
