using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class DynamicBucketedHeapSpaceManager : ISpaceManager
	{
		private bool m_allowEndAllocation = true;

		private SortedBucket[] m_buckets;

		private int m_bucketCount;

		private long m_end;

		private long m_unuseableBytes;

		private int m_maxSpacesPerBucket;

		private int m_maxBucketCount;

		private int m_bucketSplitThreshold;

		private int m_minimumTrackedSize;

		private const int DefaultSpacesPerBucket = 2500;

		private const int DefaultMaxBucketCount = 10;

		private const int DefaultBucketSplitThreshold = 50;

		private const int DefaultMinimumTrackedSize = 35;

		public long StreamEnd
		{
			get
			{
				return m_end;
			}
			set
			{
				m_end = value;
			}
		}

		public bool AllowEndAllocation
		{
			get
			{
				return m_allowEndAllocation;
			}
			set
			{
				m_allowEndAllocation = value;
			}
		}

		public long UnuseableBytes => m_unuseableBytes;

		internal SortedBucket[] Buckets => m_buckets;

		internal int BucketCount => m_bucketCount;

		internal DynamicBucketedHeapSpaceManager()
			: this(50, 10, 2500, 35)
		{
		}

		internal DynamicBucketedHeapSpaceManager(int splitThreshold, int maxBucketCount, int maxSpacesPerBucket, int minTrackedSizeBytes)
		{
			m_bucketSplitThreshold = splitThreshold;
			m_maxBucketCount = maxBucketCount;
			m_maxSpacesPerBucket = maxSpacesPerBucket;
			m_minimumTrackedSize = minTrackedSizeBytes;
			m_buckets = new SortedBucket[m_maxBucketCount];
			SortedBucket sortedBucket = new SortedBucket(m_maxSpacesPerBucket)
			{
				Limit = 0
			};
			m_buckets[0] = sortedBucket;
			m_bucketCount++;
		}

		public void Seek(long offset, SeekOrigin origin)
		{
		}

		public void Free(long offset, long size)
		{
			Space space = new Space(offset, size);
			InsertSpace(space);
		}

		private void InsertSpace(Space space)
		{
			if (space.Size < m_minimumTrackedSize)
			{
				m_unuseableBytes += space.Size;
				return;
			}
			int bucketIndex = GetBucketIndex(space.Size);
			SortedBucket sortedBucket = m_buckets[bucketIndex];
			if (sortedBucket.Count == m_maxSpacesPerBucket)
			{
				if (m_bucketCount < m_maxBucketCount && sortedBucket.Maximum - sortedBucket.Minimum > m_bucketSplitThreshold)
				{
					SortedBucket sortedBucket2 = sortedBucket.Split(m_maxSpacesPerBucket);
					for (int num = m_bucketCount; num > bucketIndex + 1; num--)
					{
						m_buckets[num] = m_buckets[num - 1];
					}
					m_buckets[bucketIndex + 1] = sortedBucket2;
					m_bucketCount++;
					InsertSpace(space);
				}
				else if (sortedBucket.Peek().Size < space.Size)
				{
					Space space2 = sortedBucket.ExtractMax();
					m_unuseableBytes += space2.Size;
					sortedBucket.Insert(space);
				}
				else
				{
					m_unuseableBytes += space.Size;
				}
			}
			else
			{
				sortedBucket.Insert(space);
			}
		}

		private int GetBucketIndex(long size)
		{
			for (int i = 1; i < m_bucketCount; i++)
			{
				if (m_buckets[i].Limit > size)
				{
					return i - 1;
				}
			}
			return m_bucketCount - 1;
		}

		public long AllocateSpace(long size)
		{
			long num = -1L;
			for (int i = GetBucketIndex(size); i < m_bucketCount; i++)
			{
				if (num != -1)
				{
					break;
				}
				SortedBucket sortedBucket = m_buckets[i];
				if (sortedBucket.Count <= 0)
				{
					continue;
				}
				Space space = sortedBucket.Peek();
				if (space.Size >= size)
				{
					sortedBucket.ExtractMax();
					num = space.Offset;
					space.Offset += size;
					space.Size -= size;
					if (space.Size > 0)
					{
						InsertSpace(space);
					}
					if (sortedBucket.Count == 0 && i != 0)
					{
						Array.Copy(m_buckets, i + 1, m_buckets, i, m_bucketCount - i - 1);
						m_bucketCount--;
					}
				}
			}
			if (num == -1 && m_allowEndAllocation)
			{
				num = m_end;
				m_end += size;
			}
			return num;
		}

		public long Resize(long offset, long oldSize, long newSize)
		{
			Free(offset, oldSize);
			return AllocateSpace(newSize);
		}

		public void TraceStats()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < m_bucketCount; i++)
			{
				SortedBucket sortedBucket = m_buckets[i];
				stringBuilder.AppendFormat("\r\n\t\tBucket: Limit: {0} Count: {1}", sortedBucket.Limit, sortedBucket.Count);
			}
			Global.Tracer.Trace(TraceLevel.Verbose, "DynamicBucketedHeapSpaceManager Stats. StreamSize: {0} MB. UnusableSpace: {1} KB. \r\n\tBucketInfo: {2}", m_end / 1048576, m_unuseableBytes / 1024, stringBuilder.ToString());
		}
	}
}
