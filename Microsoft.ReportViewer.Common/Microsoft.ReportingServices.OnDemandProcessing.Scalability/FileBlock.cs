namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class FileBlock
	{
		private DynamicBucketedHeapSpaceManager m_spaceManager;

		internal FileBlock()
		{
			m_spaceManager = new DynamicBucketedHeapSpaceManager();
			m_spaceManager.AllowEndAllocation = false;
		}

		public void Free(long offset, long size)
		{
			m_spaceManager.Free(offset, size);
		}

		public long AllocateSpace(long size)
		{
			return m_spaceManager.AllocateSpace(size);
		}

		public long Resize(long offset, long oldSize, long newSize)
		{
			return m_spaceManager.Resize(offset, oldSize, newSize);
		}

		public void TraceStats(string desc)
		{
			m_spaceManager.TraceStats();
		}
	}
}
