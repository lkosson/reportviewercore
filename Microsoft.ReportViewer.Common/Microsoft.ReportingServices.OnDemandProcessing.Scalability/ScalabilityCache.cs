using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalabilityCache : BaseScalabilityCache
	{
		private LinkedLRUCache<StorageItem> m_cachePriority;

		private SegmentedDictionary<ReferenceID, StorageItem> m_cacheLookup;

		private IIndexStrategy m_offsetMap;

		private Dictionary<int, StaticReferenceHolder> m_staticReferences;

		private Dictionary<object, int> m_staticIdLookup;

		private const long CacheExpansionIntervalMs = 3000L;

		private const double CacheExpansionRatio = 0.2;

		private const int ID_NULL = -2147483647;

		public const int ID_NOREF = int.MinValue;

		public override ScalabilityCacheType CacheType => ScalabilityCacheType.Standard;

		protected override long InternalFreeableBytes => m_cacheSizeBytes;

		public ScalabilityCache(IStorage storage, IIndexStrategy indexStrategy, ComponentType ownerComponent, long minReservedMemoryBytes)
			: base(storage, 3000L, 0.2, ownerComponent, minReservedMemoryBytes)
		{
			m_cachePriority = new LinkedLRUCache<StorageItem>();
			m_offsetMap = indexStrategy;
		}

		public override IReference<T> Allocate<T>(T obj, int priority)
		{
			return InternalAllocate(obj, priority, startPinned: false, ItemSizes.SizeOf(obj));
		}

		public override IReference<T> Allocate<T>(T obj, int priority, int initialSize)
		{
			return InternalAllocate(obj, priority, startPinned: false, initialSize);
		}

		public override IReference<T> AllocateAndPin<T>(T obj, int priority)
		{
			return InternalAllocate(obj, priority, startPinned: true, ItemSizes.SizeOf(obj));
		}

		public override IReference<T> AllocateAndPin<T>(T obj, int priority, int initialSize)
		{
			return InternalAllocate(obj, priority, startPinned: true, initialSize);
		}

		private IReference<T> InternalAllocate<T>(T obj, int priority, bool startPinned, int initialSize) where T : IStorable
		{
			Global.Tracer.Assert(obj != null, "Cannot allocate reference to null");
			BaseReference baseReference = CreateReference(obj);
			baseReference.Init(this, m_offsetMap.GenerateTempId());
			CacheItem(baseReference, obj, priority, initialSize);
			if (startPinned)
			{
				baseReference.PinValue();
			}
			return (IReference<T>)baseReference;
		}

		public override void Dispose()
		{
			try
			{
				if (m_offsetMap != null)
				{
					m_offsetMap.Close();
					m_offsetMap = null;
				}
				m_cacheLookup = null;
				m_cachePriority = null;
				m_staticIdLookup = null;
				m_staticReferences = null;
			}
			finally
			{
				base.Dispose();
			}
		}

		public override IReference<T> GenerateFixedReference<T>(T obj)
		{
			BaseReference baseReference = CreateReference(obj);
			baseReference.Init(this, m_offsetMap.GenerateTempId());
			StorageItem storageItem = (StorageItem)(baseReference.Item = new StorageItem(baseReference.Id, -1, obj, ItemSizes.SizeOf(obj)));
			storageItem.AddReference(baseReference);
			storageItem.InQueue = InQueueState.Exempt;
			storageItem.HasBeenUnPinned = true;
			(obj as ISelfReferential)?.SetReference(baseReference);
			return (IReference<T>)baseReference;
		}

		public override int StoreStaticReference(object item)
		{
			int num;
			if (item == null)
			{
				num = -2147483647;
			}
			else
			{
				IStaticReferenceable staticReferenceable = item as IStaticReferenceable;
				if (staticReferenceable != null)
				{
					num = InternalStoreStaticReference(staticReferenceable.ID, item);
					staticReferenceable.SetID(num);
				}
				else
				{
					bool flag = true;
					if (m_staticIdLookup == null || !m_staticIdLookup.TryGetValue(item, out num))
					{
						num = int.MinValue;
						flag = false;
					}
					num = InternalStoreStaticReference(num, item);
					if (!flag)
					{
						if (m_staticIdLookup == null)
						{
							m_staticIdLookup = new Dictionary<object, int>();
						}
						m_staticIdLookup[item] = num;
					}
				}
			}
			return num;
		}

		private int InternalStoreStaticReference(int id, object item)
		{
			if (m_staticReferences == null)
			{
				m_staticReferences = new Dictionary<int, StaticReferenceHolder>();
			}
			int num = id;
			if (id == int.MinValue)
			{
				num = (int)m_offsetMap.GenerateTempId().Value;
			}
			StaticReferenceHolder value;
			while (m_staticReferences.TryGetValue(num, out value) && item != value.Value)
			{
				num = (int)m_offsetMap.GenerateTempId().Value;
			}
			if (value != null)
			{
				value.RefCount++;
			}
			else
			{
				value = new StaticReferenceHolder();
				value.Value = item;
				value.RefCount = 1;
				m_staticReferences[num] = value;
			}
			return num;
		}

		public override object FetchStaticReference(int id)
		{
			if (id == -2147483647)
			{
				return null;
			}
			object result;
			if (m_staticReferences.TryGetValue(id, out StaticReferenceHolder value))
			{
				result = value.Value;
				value.RefCount--;
				if (value.RefCount <= 0)
				{
					m_staticReferences.Remove(id);
				}
			}
			else
			{
				Global.Tracer.Assert(condition: false, "Missing static reference");
				result = null;
			}
			return result;
		}

		public override IReference PoolReference(IReference reference)
		{
			if (CacheTryGetValue(reference.Id, out StorageItem item) && item.Reference != null)
			{
				reference = item.Reference;
			}
			return reference;
		}

		internal override void UpdateTargetSize(BaseReference reference, int sizeDeltaBytes)
		{
			((StorageItem)reference.Item).UpdateSize(sizeDeltaBytes);
			m_cacheSizeBytes += sizeDeltaBytes;
			m_totalAuditedBytes += sizeDeltaBytes;
		}

		internal override BaseReference TransferTo(BaseReference reference)
		{
			Global.Tracer.Assert(condition: false, "ScalabilityCache does not support the TransferTo operation");
			return null;
		}

		internal override void ReferenceSerializeCallback(BaseReference reference)
		{
			ReferenceID id = reference.Id;
			if (id.IsTemporary)
			{
				StorageItem storageItem = (StorageItem)reference.Item;
				ReferenceID referenceID = m_offsetMap.GenerateId(id);
				if (id != referenceID)
				{
					reference.Id = referenceID;
					storageItem.Id = referenceID;
					CacheRemoveValue(id);
				}
				CacheSetValue(reference.Id, storageItem);
			}
		}

		internal override void Free(BaseReference reference)
		{
			if (reference == null)
			{
				return;
			}
			ReferenceID id = reference.Id;
			if (CacheTryGetValue(id, out StorageItem item))
			{
				CacheRemoveValue(id);
			}
			if (item == null)
			{
				item = (StorageItem)reference.Item;
			}
			if (item != null)
			{
				if (item.InQueue == InQueueState.InQueue)
				{
					m_cachePriority.Remove(item);
				}
				int num = ItemSizes.SizeOf(item);
				m_cacheSizeBytes -= num;
				m_totalAuditedBytes -= num;
				m_totalFreedBytes += num;
				UpdatePeakCacheSize();
				item.Item = null;
				item.UnlinkReferences(updateId: false);
			}
			reference.Item = null;
		}

		internal override IStorable Retrieve(BaseReference reference)
		{
			if (!CacheTryGetValue(reference.Id, out StorageItem item))
			{
				item = LoadItem(reference);
			}
			PeriodicOperationCheck();
			return item.Item;
		}

		internal override void Pin(BaseReference reference)
		{
			StorageItem item = (StorageItem)reference.Item;
			if (item == null)
			{
				if (CacheTryGetValue(reference.Id, out item))
				{
					reference.Item = item;
					item.AddReference(reference);
					if (item.InQueue == InQueueState.InQueue)
					{
						m_cachePriority.Bump(item);
					}
				}
				else
				{
					item = LoadItem(reference);
				}
			}
			else if (item.InQueue == InQueueState.InQueue)
			{
				m_cachePriority.Bump(item);
			}
			item.PinCount++;
		}

		internal override void UnPin(BaseReference reference)
		{
			StorageItem storageItem = (StorageItem)reference.Item;
			if (--storageItem.PinCount == 0)
			{
				if (storageItem.InQueue == InQueueState.None)
				{
					EnqueueItem(storageItem);
				}
				if (!storageItem.HasBeenUnPinned)
				{
					int num = storageItem.UpdateSize();
					m_cacheSizeBytes += num;
					m_totalAuditedBytes += num;
					storageItem.HasBeenUnPinned = true;
				}
			}
		}

		internal bool CacheTryGetValue(ReferenceID id, out StorageItem item)
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

		internal void CacheSetValue(ReferenceID id, StorageItem value)
		{
			if (m_cacheLookup == null)
			{
				m_cacheLookup = new SegmentedDictionary<ReferenceID, StorageItem>(503, 17, ReferenceIDEqualityComparer.Instance);
			}
			m_cacheLookup[id] = value;
		}

		private StorageItem LoadItem(BaseReference reference)
		{
			if (m_inStreamOper)
			{
				Global.Tracer.Assert(condition: false, "ScalabilityCache should not Load during serialization");
			}
			StorageItem storageItem = null;
			try
			{
				m_inStreamOper = true;
				m_deserializationTimer.Start();
				long num = m_offsetMap.Retrieve(reference.Id);
				if (num >= 0)
				{
					storageItem = (StorageItem)m_storage.Retrieve(num, out long persistedSize);
					storageItem.Offset = num;
					storageItem.PersistedSize = persistedSize;
					storageItem.UpdateSize();
					storageItem.HasBeenUnPinned = true;
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
			finally
			{
				m_inStreamOper = false;
				m_deserializationTimer.Stop();
			}
			CacheItem(reference, storageItem, fromDeserialize: true);
			return storageItem;
		}

		private void CacheItem(BaseReference reference, StorageItem item, bool fromDeserialize)
		{
			reference.Item = item;
			item.AddReference(reference);
			int num = ItemSizes.SizeOf(item);
			FreeCacheSpace(num, m_cacheSizeBytes);
			if (fromDeserialize)
			{
				CacheSetValue(reference.Id, item);
			}
			else
			{
				m_totalAuditedBytes += num;
			}
			m_cacheSizeBytes += num;
			EnqueueItem(item);
			(item.Item as ISelfReferential)?.SetReference(reference);
		}

		private void CacheItem(BaseReference reference, IStorable value, int priority, int initialSize)
		{
			StorageItem item = new StorageItem(reference.Id, priority, value, initialSize);
			CacheItem(reference, item, fromDeserialize: false);
		}

		private void EnqueueItem(StorageItem item)
		{
			m_cachePriority.Add(item);
			item.InQueue = InQueueState.InQueue;
		}

		protected override void FulfillInProgressFree()
		{
			m_storage.FreezeAllocations = true;
			while (m_inProgressFreeBytes > 0 && m_cachePriority.Count > 0)
			{
				StorageItem storageItem = m_cachePriority.ExtractLRU();
				storageItem.InQueue = InQueueState.None;
				if (storageItem.Item != null && storageItem.PinCount == 0)
				{
					CacheRemoveValue(storageItem.Id);
					int num = ItemSizes.SizeOf(storageItem);
					storageItem.Flush(m_storage, m_offsetMap);
					m_cacheSizeBytes -= num;
					if (m_cacheSizeBytes < 0)
					{
						m_cacheSizeBytes = 0L;
					}
					m_inProgressFreeBytes -= num;
					if (m_inProgressFreeBytes < 0)
					{
						m_inProgressFreeBytes = 0L;
					}
				}
			}
			m_storage.FreezeAllocations = false;
		}
	}
}
