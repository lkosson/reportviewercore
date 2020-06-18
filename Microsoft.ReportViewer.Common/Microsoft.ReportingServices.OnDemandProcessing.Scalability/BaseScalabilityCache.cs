using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal abstract class BaseScalabilityCache : IScalabilityCache, PersistenceHelper, IDisposable
	{
		protected bool m_disposed;

		protected long m_minReservedMemoryKB;

		protected long m_cacheSizeBytes;

		protected long m_cacheCapacityBytes = DefaultCacheCapacityBytes;

		protected long m_totalAuditedBytes;

		protected long m_totalFreedBytes;

		protected int m_pendingNotificationCount;

		protected long m_inProgressFreeBytes;

		protected long m_pendingFreeBytes;

		protected bool m_freeingSpace;

		protected bool m_inStreamOper;

		protected IStorage m_storage;

		protected IReferenceCreator m_referenceCreator;

		protected ComponentType m_ownerComponent;

		protected Stopwatch m_serializationTimer = new Stopwatch();

		protected Stopwatch m_deserializationTimer = new Stopwatch();

		protected long m_totalBytesSerialized;

		protected long m_lastExpansionOrNotificationMs = -1L;

		protected long m_expansionIntervalMs;

		protected Stopwatch m_cacheLifetimeTimer;

		protected double m_cacheExpansionRatio;

		protected bool m_receivedShrinkRequest;

		protected long m_peakCacheSizeBytes = -1L;

		protected static long DefaultCacheCapacityBytes = -1L;

		protected static readonly long CacheExpansionMaxBytes = ComputeMaxExpansionBytes(Environment.ProcessorCount);

		internal const long DefaultCacheExpansionMaxBytes = 15728640L;

		internal const long CacheExpansionMinBytes = 1048576L;

		public IStorage Storage => m_storage;

		public abstract ScalabilityCacheType CacheType
		{
			get;
		}

		public ComponentType OwnerComponent => m_ownerComponent;

		public long SerializationDurationMs => ReadTime(m_serializationTimer);

		public long DeserializationDurationMs => ReadTime(m_deserializationTimer);

		public long ScalabilityDurationMs => SerializationDurationMs + DeserializationDurationMs;

		public long PeakMemoryUsageKBytes => Math.Max(m_peakCacheSizeBytes, m_cacheSizeBytes) / 1024;

		public long CacheSizeKBytes => m_cacheSizeBytes / 1024;

		protected long MinReservedMemoryKB => m_minReservedMemoryKB;

		protected abstract long InternalFreeableBytes
		{
			get;
		}

		internal long TotalSerializedHeapBytes => m_totalBytesSerialized;

		internal BaseScalabilityCache(IStorage storage, long cacheExpansionIntervalMs, double cacheExpansionRatio, ComponentType ownerComponent, long minReservedMemoryBytes)
		{
			m_cacheSizeBytes = 0L;
			m_storage = storage;
			m_referenceCreator = storage.ReferenceCreator;
			storage.ScalabilityCache = this;
			Global.Tracer.Assert(cacheExpansionIntervalMs > 0, "CacheExpansionIntervalMs must be greater than 0");
			m_expansionIntervalMs = cacheExpansionIntervalMs;
			m_cacheExpansionRatio = cacheExpansionRatio;
			m_ownerComponent = ownerComponent;
			m_minReservedMemoryKB = minReservedMemoryBytes / 1024;
		}

		public abstract IReference<T> Allocate<T>(T obj, int priority) where T : IStorable;

		public abstract IReference<T> Allocate<T>(T obj, int priority, int initialSize) where T : IStorable;

		public abstract IReference<T> AllocateAndPin<T>(T obj, int priority) where T : IStorable;

		public abstract IReference<T> AllocateAndPin<T>(T obj, int priority, int initialSize) where T : IStorable;

		public abstract IReference<T> GenerateFixedReference<T>(T obj) where T : IStorable;

		public void Close()
		{
			Dispose();
		}

		public abstract int StoreStaticReference(object item);

		public abstract object FetchStaticReference(int id);

		public abstract IReference PoolReference(IReference reference);

		public virtual void Dispose()
		{
			try
			{
				if (m_disposed)
				{
					return;
				}
				m_disposed = true;
				if (m_serializationTimer != null && Global.Tracer.TraceVerbose)
				{
					long num = -1L;
					if (m_storage != null)
					{
						num = m_storage.StreamSize;
					}
					long num2 = m_totalBytesSerialized / 1024;
					double num3 = (double)SerializationDurationMs / 1000.0;
					Global.Tracer.Trace(TraceLevel.Verbose, "ScalabilityCache {0}|{1} Done. AuditedHeapSerialized: {2} KB. Serialization Time {3} s. AvgSpeed {4:F2} KB/s. StreamSize {5} MB. FinalAuditedHeapSize {6} KB. LifetimeFreedHeapSize {7} KB.", OwnerComponent, CacheType, num2, num3, (double)num2 / num3, num / 1048576, m_totalAuditedBytes / 1024, m_totalFreedBytes / 1024);
				}
				if (m_storage != null)
				{
					m_storage.Close();
					m_storage = null;
				}
				m_serializationTimer = null;
				m_deserializationTimer = null;
			}
			finally
			{
				m_cacheSizeBytes = 0L;
				m_cacheCapacityBytes = 0L;
			}
		}

		internal abstract void Free(BaseReference reference);

		internal abstract IStorable Retrieve(BaseReference reference);

		[DebuggerStepThrough]
		internal virtual void ReferenceValueCallback(BaseReference reference)
		{
			PeriodicOperationCheck();
		}

		internal abstract void UnPin(BaseReference reference);

		internal abstract void Pin(BaseReference reference);

		internal abstract void ReferenceSerializeCallback(BaseReference reference);

		internal abstract void UpdateTargetSize(BaseReference reference, int sizeDeltaBytes);

		internal abstract BaseReference TransferTo(BaseReference reference);

		[DebuggerStepThrough]
		internal void PeriodicOperationCheck()
		{
			if (m_pendingNotificationCount > 0)
			{
				FreeCacheSpace(0, InternalFreeableBytes);
			}
		}

		protected abstract void FulfillInProgressFree();

		protected BaseReference CreateReference(IStorable storable)
		{
			if (!m_referenceCreator.TryCreateReference(storable, out BaseReference newReference))
			{
				Global.Tracer.Assert(false, "Cannot create reference to: {0}", storable);
			}
			return newReference;
		}

		[Conditional("DEBUG")]
		protected void CheckDisposed(string opName)
		{
			_ = m_disposed;
		}

		protected void UpdatePeakCacheSize()
		{
			m_peakCacheSizeBytes = Math.Max(m_peakCacheSizeBytes, m_cacheSizeBytes);
		}

		protected void FreeCacheSpace(int count, long freeableBytes)
		{
			if (m_freeingSpace || m_inStreamOper)
			{
				return;
			}
			int num = 0;
			try
			{
				m_inStreamOper = true;
				m_freeingSpace = true;
				long num2 = Interlocked.Read(ref m_cacheCapacityBytes);
				if (num2 <= 0)
				{
					return;
				}
				num = Interlocked.Exchange(ref m_pendingNotificationCount, 0);
				Interlocked.Exchange(ref m_pendingFreeBytes, 0L);
				long num3 = freeableBytes + count - num2;
				if (num3 > 0)
				{
					if (num != 0 || m_cacheLifetimeTimer == null || m_cacheLifetimeTimer.ElapsedMilliseconds - m_expansionIntervalMs <= m_lastExpansionOrNotificationMs)
					{
						goto IL_0198;
					}
					ResetExpansionOrNotificationInterval();
					long num4 = (long)((double)(m_totalAuditedBytes - num2) * m_cacheExpansionRatio);
					if (num4 > CacheExpansionMaxBytes)
					{
						num4 = CacheExpansionMaxBytes;
					}
					else if (num4 < 1048576)
					{
						num4 = 1048576L;
						m_totalAuditedBytes += 1048576L;
					}
					if (num4 <= 0)
					{
						goto IL_0198;
					}
					num2 = Interlocked.Add(ref m_cacheCapacityBytes, num4);
					if (Global.Tracer.TraceVerbose)
					{
						Global.Tracer.Trace(TraceLevel.Verbose, "ScalabilityCache {0}|{1} expanding cache. ExpansionKB: {2} TotalAllocation: {3} CacheSizeKB: {4} CacheCapacityKB: {5}", OwnerComponent, CacheType, num4 / 1024, m_totalAuditedBytes / 1024, m_cacheSizeBytes / 1024, m_cacheCapacityBytes / 1024);
					}
					num3 -= num4;
					if (num3 > 0)
					{
						goto IL_0198;
					}
				}
				goto end_IL_0013;
				IL_0198:
				Stopwatch stopwatch = null;
				if (num > 0 && Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "ScalabilityCache {0}|{1} responding to pressure.  PendingNotifications: {2} CacheSizeKB: {3} CacheCapacityKB: {4}", OwnerComponent, CacheType, num, m_cacheSizeBytes / 1024, m_cacheCapacityBytes / 1024);
					stopwatch = new Stopwatch();
					stopwatch.Start();
				}
				m_serializationTimer.Start();
				m_inProgressFreeBytes = num3;
				FulfillInProgressFree();
				m_serializationTimer.Stop();
				long num5 = num3 - m_inProgressFreeBytes;
				m_totalBytesSerialized += num5;
				UpdatePeakCacheSize();
				if (num > 0 && Global.Tracer.TraceVerbose)
				{
					stopwatch.Stop();
					double num6 = (double)stopwatch.ElapsedMilliseconds / 1000.0;
					long num7 = num5 / 1024;
					double num8 = (double)num7 / num6;
					Global.Tracer.Trace(TraceLevel.Verbose, "ScalabilityCache {0}|{1} done responding to pressure.  Freed: {2} KB. Speed: {3:F2} KB/Sec.  CacheSizeKB {4} CacheCapacityKB {5}", OwnerComponent, CacheType, num7, num8, m_cacheSizeBytes / 1024, m_cacheCapacityBytes / 1024);
				}
				end_IL_0013:;
			}
			finally
			{
				m_freeingSpace = false;
				m_inStreamOper = false;
				m_inProgressFreeBytes = 0L;
				if (num > 0)
				{
					ResetExpansionOrNotificationInterval();
				}
			}
		}

		private long ReadTime(Stopwatch timer)
		{
			long num = 0L;
			if (timer != null)
			{
				num = timer.ElapsedMilliseconds;
			}
			if (num < 0)
			{
				num = -1L;
			}
			return num;
		}

		private void ResetExpansionOrNotificationInterval()
		{
			if (m_cacheLifetimeTimer == null)
			{
				m_cacheLifetimeTimer = new Stopwatch();
				m_cacheLifetimeTimer.Start();
				Random random = new Random();
				m_expansionIntervalMs += random.Next(1500);
			}
			m_lastExpansionOrNotificationMs = m_cacheLifetimeTimer.ElapsedMilliseconds;
		}

		private static void SetDefaultCacheCapacityBytes(long defaultCapacityBytes)
		{
			DefaultCacheCapacityBytes = defaultCapacityBytes;
		}

		internal static long ComputeMaxExpansionBytes(int processorCount)
		{
			long num = Math.Max(1, processorCount / 4 * 2);
			return Math.Max(15728640 / num, 1048576L);
		}
	}
}
