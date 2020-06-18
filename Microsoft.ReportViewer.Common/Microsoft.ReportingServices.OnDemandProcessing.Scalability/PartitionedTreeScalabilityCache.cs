using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal abstract class PartitionedTreeScalabilityCache : BaseScalabilityCache
	{
		private long m_cacheFreeableBytes;

		private LinkedBucketedQueue<BaseReference> m_serializationQueue;

		private LinkedBucketedQueue<BaseReference> m_pinnedItems;

		private LinkedLRUCache<ItemHolder> m_cachePriority;

		private SegmentedDictionary<ReferenceID, BaseReference> m_cacheLookup;

		private long m_nextId = -1L;

		private TreePartitionManager m_partitionManager;

		private bool m_lockedDownForFlush;

		public abstract override ScalabilityCacheType CacheType
		{
			get;
		}

		protected sealed override long InternalFreeableBytes => m_cacheFreeableBytes;

		internal long CacheSizeBytes => m_cacheSizeBytes;

		internal long CacheFreeableBytes => m_cacheFreeableBytes;

		internal long CacheCapacityBytes
		{
			get
			{
				return m_cacheCapacityBytes;
			}
			set
			{
				m_cacheCapacityBytes = value;
			}
		}

		public PartitionedTreeScalabilityCache(TreePartitionManager partitionManager, IStorage storage, long cacheExpansionIntervalMs, double cacheExpansionRatio, long minReservedMemoryBytes)
			: base(storage, cacheExpansionIntervalMs, cacheExpansionRatio, ComponentType.Processing, minReservedMemoryBytes)
		{
			m_serializationQueue = new LinkedBucketedQueue<BaseReference>(100);
			m_cachePriority = new LinkedLRUCache<ItemHolder>();
			m_cacheFreeableBytes = 0L;
			m_partitionManager = partitionManager;
		}

		public sealed override IReference<T> Allocate<T>(T obj, int priority)
		{
			Global.Tracer.Assert(condition: false, "Allocate should not be used on PartitionedTreeScalabilityCache.  Use AllocateAndPin instead.");
			return null;
		}

		public sealed override IReference<T> Allocate<T>(T obj, int priority, int initialSize)
		{
			Global.Tracer.Assert(condition: false, "Allocate should not be used on PartitionedTreeScalabilityCache.  Use AllocateAndPin instead.");
			return null;
		}

		public sealed override IReference<T> AllocateAndPin<T>(T obj, int priority)
		{
			return (IReference<T>)AllocateAndPin(obj, ItemSizes.SizeOf(obj));
		}

		public sealed override IReference<T> AllocateAndPin<T>(T obj, int priority, int initialSize)
		{
			return (IReference<T>)AllocateAndPin(obj, initialSize);
		}

		protected BaseReference AllocateAndPin(IStorable obj, int initialSize)
		{
			Global.Tracer.Assert(obj != null, "Cannot allocate reference to null");
			BaseReference baseReference = CreateReference(obj);
			baseReference.Init(this, GenerateTempId());
			CacheItem(baseReference, obj, fromDeserialize: false, initialSize);
			baseReference.PinValue();
			return baseReference;
		}

		public sealed override IReference<T> GenerateFixedReference<T>(T obj)
		{
			BaseReference baseReference = CreateReference(obj);
			baseReference.Init(this, GenerateTempId());
			ItemHolder itemHolder = new ItemHolder();
			itemHolder.Reference = baseReference;
			itemHolder.Item = obj;
			baseReference.Item = itemHolder;
			baseReference.InQueue = InQueueState.InQueue;
			return (IReference<T>)baseReference;
		}

		public sealed override int StoreStaticReference(object item)
		{
			Global.Tracer.Assert(condition: false, "Static references are not supported in the PartitionedTreeScalabilityCache");
			return -1;
		}

		public sealed override object FetchStaticReference(int id)
		{
			Global.Tracer.Assert(condition: false, "Static references are not supported in the PartitionedTreeScalabilityCache");
			return null;
		}

		public sealed override IReference PoolReference(IReference reference)
		{
			if (CacheTryGetValue(reference.Id, out BaseReference item))
			{
				reference = item;
			}
			return reference;
		}

		internal IReference<T> AllocateEmptyTreePartition<T>(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceObjectType)
		{
			if (!m_referenceCreator.TryCreateReference(referenceObjectType, out BaseReference newReference))
			{
				Global.Tracer.Assert(false, "Cannot create reference of type: {0}", referenceObjectType);
			}
			newReference.Init(this, m_partitionManager.AllocateNewTreePartition());
			return (IReference<T>)newReference;
		}

		internal void SetTreePartitionContentsAndPin<T>(IReference<T> emptyPartitionRef, T contents) where T : IStorable
		{
			BaseReference baseReference = (BaseReference)emptyPartitionRef;
			m_partitionManager.TreeHasChanged = true;
			CacheItem(baseReference, contents, fromDeserialize: false, ItemSizes.SizeOf(contents));
			baseReference.PinValue();
			CacheSetValue(baseReference.Id, baseReference);
		}

		internal void Flush()
		{
			foreach (BaseReference item in m_serializationQueue)
			{
				WriteItem(item);
			}
			m_serializationQueue.Clear();
			m_cachePriority.Clear();
			m_cacheLookup = null;
			m_cacheCapacityBytes = 0L;
			m_cacheFreeableBytes = 0L;
			m_cacheSizeBytes = 0L;
			m_storage.Flush();
		}

		internal void PrepareForFlush()
		{
			m_cachePriority.Clear();
			m_cacheLookup = null;
			m_cacheFreeableBytes = 0L;
			m_pendingFreeBytes = 0L;
			m_lockedDownForFlush = true;
		}

		public sealed override void Dispose()
		{
			m_cacheLookup = null;
			m_cachePriority = null;
			m_serializationQueue = null;
			m_pinnedItems = null;
			base.Dispose();
		}

		internal override BaseReference TransferTo(BaseReference reference)
		{
			Global.Tracer.Assert(condition: false, "PartitionedTreeScalabilityCache does not support the TransferTo operation");
			return null;
		}

		internal sealed override void Free(BaseReference reference)
		{
			Global.Tracer.Assert(condition: false, "PartitionedTreeScalabilityCache does not support Free");
		}

		internal sealed override IStorable Retrieve(BaseReference reference)
		{
			if (reference.Item == null)
			{
				ReferenceID id = reference.Id;
				if (CacheTryGetValue(id, out BaseReference item) && item.Item != null)
				{
					IStorable item2 = item.Item.Item;
					CacheItem(reference, item2, fromDeserialize: true, ItemSizes.SizeOf(item2));
				}
				else
				{
					LoadItem(reference);
				}
			}
			IStorable result = null;
			if (reference.Item != null)
			{
				result = reference.Item.Item;
			}
			return result;
		}

		internal sealed override void ReferenceValueCallback(BaseReference reference)
		{
			if (reference.InQueue == InQueueState.Exempt)
			{
				m_cachePriority.Bump(reference.Item);
			}
			base.ReferenceValueCallback(reference);
		}

		internal sealed override void Pin(BaseReference reference)
		{
			Retrieve(reference);
		}

		internal sealed override void UnPin(BaseReference reference)
		{
			if (reference.PinCount == 0 && (reference.Id.IsTemporary || reference.Id.HasMultiPart) && reference.InQueue == InQueueState.None)
			{
				reference.InQueue = InQueueState.InQueue;
				m_serializationQueue.Enqueue(reference);
				if (!m_lockedDownForFlush)
				{
					ItemHolder item = reference.Item;
					IStorable obj = null;
					if (item != null)
					{
						obj = item.Item;
					}
					m_cacheFreeableBytes += ItemSizes.SizeOf(obj);
				}
			}
			if (!m_lockedDownForFlush)
			{
				PeriodicOperationCheck();
			}
		}

		internal sealed override void ReferenceSerializeCallback(BaseReference reference)
		{
		}

		internal sealed override void UpdateTargetSize(BaseReference reference, int sizeDeltaBytes)
		{
			m_cacheSizeBytes += sizeDeltaBytes;
			m_totalAuditedBytes += sizeDeltaBytes;
			if (!reference.Id.IsTemporary)
			{
				m_cacheFreeableBytes += sizeDeltaBytes;
			}
		}

		internal bool CacheTryGetValue(ReferenceID id, out BaseReference item)
		{
			item = null;
			bool result = false;
			if (m_cacheLookup != null)
			{
				result = m_cacheLookup.TryGetValue(id, out item);
			}
			return result;
		}

		internal bool CacheRemoveValue(ReferenceID id)
		{
			bool result = false;
			if (m_cacheLookup != null)
			{
				result = m_cacheLookup.Remove(id);
			}
			return result;
		}

		internal void CacheSetValue(ReferenceID id, BaseReference value)
		{
			if (m_cacheLookup == null)
			{
				m_cacheLookup = new SegmentedDictionary<ReferenceID, BaseReference>(503, 17, ReferenceIDEqualityComparer.Instance);
			}
			m_cacheLookup[id] = value;
		}

		private IStorable LoadItem(BaseReference reference)
		{
			if (m_inStreamOper)
			{
				Global.Tracer.Assert(condition: false, "PartitionedTreeScalabilityCache should not Load during serialization");
			}
			IStorable storable = null;
			try
			{
				m_inStreamOper = true;
				m_deserializationTimer.Start();
				ReferenceID id = reference.Id;
				long num;
				if (!id.IsTemporary && id.HasMultiPart)
				{
					num = m_partitionManager.GetTreePartitionOffset(id);
					if (num < 0)
					{
						return null;
					}
				}
				else
				{
					num = reference.Id.Value;
				}
				if (num < 0)
				{
					Global.Tracer.Assert(false, "Invalid offset for item.  ReferenceID: {0}, Offset: {1}", id, num);
				}
				storable = (IStorable)m_storage.Retrieve(num, out long _);
			}
			finally
			{
				m_inStreamOper = false;
				m_deserializationTimer.Stop();
			}
			CacheItem(reference, storable, fromDeserialize: true, ItemSizes.SizeOf(storable));
			return storable;
		}

		private void CacheItem(BaseReference reference, IStorable item, bool fromDeserialize, int newItemSize)
		{
			ItemHolder itemHolder = new ItemHolder();
			itemHolder.Reference = reference;
			itemHolder.Item = item;
			reference.Item = itemHolder;
			FreeCacheSpace(newItemSize, m_cacheFreeableBytes);
			if (fromDeserialize)
			{
				CacheSetValue(reference.Id, reference);
				m_cacheFreeableBytes += newItemSize;
			}
			else
			{
				m_totalAuditedBytes += newItemSize;
			}
			m_cacheSizeBytes += newItemSize;
			EnqueueItem(reference);
		}

		private void EnqueueItem(BaseReference itemRef)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType objectType = itemRef.GetObjectType();
			if (objectType == Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstanceReference)
			{
				if (m_pinnedItems == null)
				{
					m_pinnedItems = new LinkedBucketedQueue<BaseReference>(25);
				}
				m_pinnedItems.Enqueue(itemRef);
				return;
			}
			ReferenceID id = itemRef.Id;
			if (!id.IsTemporary && (!id.HasMultiPart || m_partitionManager.GetTreePartitionOffset(id) != TreePartitionManager.EmptyTreePartitionOffset))
			{
				m_cachePriority.Add(itemRef.Item);
				itemRef.InQueue = InQueueState.Exempt;
			}
		}

		protected sealed override void FulfillInProgressFree()
		{
			int num = m_cachePriority.Count;
			while (m_inProgressFreeBytes > 0 && num > 0)
			{
				num--;
				ItemHolder itemHolder = m_cachePriority.Peek();
				BaseReference reference = itemHolder.Reference;
				if (reference.PinCount == 0)
				{
					m_cachePriority.ExtractLRU();
					reference.InQueue = InQueueState.None;
					if (itemHolder.Item != null)
					{
						UpdateStatsForRemovedItem(reference, ref m_inProgressFreeBytes);
						CacheRemoveValue(reference.Id);
						itemHolder.Item = null;
						itemHolder.Reference = null;
						reference.Item = null;
					}
				}
				else
				{
					m_cachePriority.Bump(itemHolder);
				}
			}
			if (m_inProgressFreeBytes <= 0)
			{
				return;
			}
			using (IDecumulator<BaseReference> decumulator = m_serializationQueue.GetDecumulator())
			{
				while (m_inProgressFreeBytes > 0 && decumulator.MoveNext())
				{
					BaseReference current = decumulator.Current;
					decumulator.RemoveCurrent();
					if (current.Item != null)
					{
						if (current.PinCount == 0)
						{
							UpdateStatsForRemovedItem(current, ref m_inProgressFreeBytes);
						}
						WriteItem(current);
						if (current.PinCount > 0)
						{
							EnqueueItem(current);
							CacheSetValue(current.Id, current);
						}
					}
				}
			}
		}

		private void UpdateStatsForRemovedItem(BaseReference itemRef, ref long bytesToFree)
		{
			long num = ItemSizes.SizeOf(itemRef.Item.Item);
			long num2 = m_cacheSizeBytes - num;
			long num3 = m_cacheFreeableBytes - num;
			if (num3 < 0)
			{
				num3 = 0L;
			}
			if (num2 < num3)
			{
				num2 = num3;
			}
			m_cacheFreeableBytes = num3;
			m_cacheSizeBytes = num2;
			bytesToFree -= num;
		}

		private void WriteItem(BaseReference itemRef)
		{
			ItemHolder item = itemRef.Item;
			IStorable item2 = item.Item;
			ReferenceID id = itemRef.Id;
			long num = m_storage.Allocate(item2);
			if (id.HasMultiPart && !id.IsTemporary)
			{
				m_partitionManager.UpdateTreePartitionOffset(id, num);
				if (itemRef.PinCount == 0)
				{
					CacheRemoveValue(id);
				}
			}
			else
			{
				id = new ReferenceID(num);
				id.IsTemporary = false;
				id.HasMultiPart = false;
				itemRef.Id = id;
			}
			if (itemRef.PinCount == 0)
			{
				item.Item = null;
				item.Reference = null;
				itemRef.Item = null;
			}
		}

		private ReferenceID GenerateTempId()
		{
			return new ReferenceID(m_nextId--);
		}
	}
}
