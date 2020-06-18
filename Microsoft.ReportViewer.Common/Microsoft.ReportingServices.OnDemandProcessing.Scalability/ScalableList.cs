using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IStorable, IPersistable, IDisposable
	{
		private enum BucketPinState
		{
			None,
			UntilBucketFull,
			UntilListEnd
		}

		private struct ScalableListEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private int m_currentIndex;

			private ScalableList<T> m_list;

			private int m_version;

			public T Current
			{
				get
				{
					if (m_list.m_version != m_version)
					{
						Global.Tracer.Assert(condition: false, "ScalableListEnumerator: Cannot use enumerator after modifying the underlying collection");
					}
					if (m_currentIndex < 0 || m_currentIndex > m_list.Count)
					{
						Global.Tracer.Assert(condition: false, "ScalableListEnumerator: Enumerator beyond the bounds of the underlying collection");
					}
					return m_list[m_currentIndex];
				}
			}

			object IEnumerator.Current => Current;

			internal ScalableListEnumerator(ScalableList<T> list)
			{
				m_list = list;
				m_version = list.m_version;
				m_currentIndex = -1;
				Reset();
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (m_list.m_version != m_version)
				{
					Global.Tracer.Assert(condition: false, "ScalableListEnumerator: Cannot use enumerator after modifying the underlying collection");
				}
				if (m_currentIndex < m_list.Count - 1)
				{
					m_currentIndex++;
					return true;
				}
				return false;
			}

			public void Reset()
			{
				m_currentIndex = -1;
				m_version = m_list.m_version;
			}
		}

		private IScalabilityCache m_cache;

		private int m_bucketSize;

		private int m_count;

		private int m_capacity;

		private int m_priority;

		private ScalableList<IReference<StorableArray>> m_buckets;

		private IReference<StorableArray> m_array;

		private int m_version;

		private bool m_isReadOnly;

		private BucketPinState m_bucketPinState;

		private static Declaration m_declaration = GetDeclaration();

		public T this[int index]
		{
			get
			{
				CheckIndex(index, m_count - 1);
				if (m_array != null)
				{
					using (m_array.PinValue())
					{
						return (T)m_array.Value().Array[index];
					}
				}
				IReference<StorableArray> reference = m_buckets[GetBucketIndex(index)];
				using (reference.PinValue())
				{
					StorableArray bucket = reference.Value();
					return GetValueAt(index, bucket);
				}
			}
			set
			{
				CheckReadOnly("set value");
				SetValue(index, value, fromAdd: false);
			}
		}

		bool IList.IsFixedSize => false;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (T)value;
			}
		}

		int ICollection.Count => Count;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		public int Count => m_count;

		public int Capacity => m_capacity;

		public bool IsReadOnly => m_isReadOnly;

		public int Size => 12 + ItemSizes.SizeOf(m_buckets) + ItemSizes.SizeOf(m_array) + 4 + 4;

		public ScalableList()
		{
		}

		internal ScalableList(int priority, IScalabilityCache cache)
			: this(priority, cache, 10)
		{
		}

		internal ScalableList(int priority, IScalabilityCache cache, int segmentSize)
			: this(priority, cache, segmentSize, segmentSize)
		{
		}

		internal ScalableList(int priority, IScalabilityCache cache, int segmentSize, int capacity)
			: this(priority, cache, segmentSize, capacity, keepAllBucketsPinned: false)
		{
		}

		internal ScalableList(int priority, IScalabilityCache cache, int segmentSize, int capacity, bool keepAllBucketsPinned)
		{
			m_priority = priority;
			m_cache = cache;
			m_bucketSize = segmentSize;
			m_count = 0;
			if (keepAllBucketsPinned)
			{
				m_bucketPinState = BucketPinState.UntilListEnd;
			}
			else if (cache.CacheType == ScalabilityCacheType.GroupTree || cache.CacheType == ScalabilityCacheType.Lookup)
			{
				m_bucketPinState = BucketPinState.UntilBucketFull;
			}
			EnsureCapacity(capacity);
		}

		public int IndexOf(T item)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			if (m_array != null)
			{
				object[] array = m_array.Value().Array;
				for (int i = 0; i < m_count; i++)
				{
					if (@default.Equals(item, (T)array[i]))
					{
						return i;
					}
				}
			}
			else
			{
				int count = m_buckets.Count;
				for (int j = 0; j < count; j++)
				{
					IReference<StorableArray> reference = m_buckets[j];
					using (reference.PinValue())
					{
						object[] array2 = reference.Value().Array;
						int num = 0;
						if (j == count - 1)
						{
							num = m_count % m_bucketSize;
						}
						if (num == 0)
						{
							num = m_bucketSize;
						}
						for (int k = 0; k < num; k++)
						{
							if (@default.Equals(item, (T)array2[k]))
							{
								return j * m_bucketSize + k;
							}
						}
					}
				}
			}
			return -1;
		}

		public void Insert(int index, T itemToInsert)
		{
			CheckReadOnly("Insert");
			CheckIndex(index, m_count);
			EnsureCapacity(m_count + 1);
			if (m_array != null)
			{
				using (m_array.PinValue())
				{
					object[] array = m_array.Value().Array;
					Array.Copy(array, index, array, index + 1, m_count - index);
					array[index] = itemToInsert;
				}
			}
			else
			{
				object obj = itemToInsert;
				object obj2 = obj;
				for (int i = GetBucketIndex(index); i < m_buckets.Count; i++)
				{
					obj = obj2;
					int indexInBucket = GetIndexInBucket(index);
					int num = (i != m_buckets.Count - 1) ? (m_bucketSize - 1) : GetIndexInBucket(m_count);
					IReference<StorableArray> reference = m_buckets[i];
					using (reference.PinValue())
					{
						object[] array2 = reference.Value().Array;
						obj2 = array2[num];
						Array.Copy(array2, indexInBucket, array2, indexInBucket + 1, num - indexInBucket);
						array2[indexInBucket] = obj;
					}
					index = 0;
				}
			}
			m_count++;
			m_version++;
		}

		public void RemoveAt(int index)
		{
			RemoveRange(index, 1);
		}

		private T GetValueAt(int index, StorableArray bucket)
		{
			object obj = bucket.Array[GetIndexInBucket(index)];
			if (obj == null)
			{
				return default(T);
			}
			return (T)obj;
		}

		private void SetValue(int index, T value, bool fromAdd)
		{
			CheckIndex(index, m_count - 1);
			if (m_array != null)
			{
				using (m_array.PinValue())
				{
					m_array.Value().Array[index] = value;
					if (fromAdd)
					{
						m_array.UpdateSize(ItemSizes.SizeOfInObjectArray(value));
					}
				}
			}
			else
			{
				IReference<StorableArray> reference = m_buckets[GetBucketIndex(index)];
				using (reference.PinValue())
				{
					reference.Value().Array[GetIndexInBucket(index)] = value;
					if (fromAdd)
					{
						reference.UpdateSize(ItemSizes.SizeOfInObjectArray(value));
					}
				}
			}
			m_version++;
		}

		public void SetValueWithExtension(int index, T item)
		{
			CheckReadOnly("SetValueWithExtension");
			int count = Math.Max(m_count, index + 1);
			EnsureCapacity(count);
			m_count = count;
			SetValue(index, item, fromAdd: true);
		}

		public IDisposable GetAndPin(int index, out T item)
		{
			CheckIndex(index, m_count - 1);
			if (m_array != null)
			{
				m_array.PinValue();
				item = (T)m_array.Value().Array[index];
				return (IDisposable)m_array;
			}
			IReference<StorableArray> reference = m_buckets[GetBucketIndex(index)];
			reference.PinValue();
			StorableArray bucket = reference.Value();
			item = GetValueAt(index, bucket);
			return (IDisposable)reference;
		}

		public IDisposable AddAndPin(T item)
		{
			CheckReadOnly("AddAndPin");
			EnsureCapacity(m_count + 1);
			m_count++;
			m_version++;
			IDisposable result = SetAndPin(m_count - 1, item, fromAdd: true);
			CheckFilledBucket();
			return result;
		}

		public IDisposable SetAndPin(int index, T item)
		{
			CheckReadOnly("SetAndPin");
			return SetAndPin(index, item, fromAdd: false);
		}

		private IDisposable SetAndPin(int index, T item, bool fromAdd)
		{
			CheckIndex(index, m_count - 1);
			IDisposable result;
			if (m_array != null)
			{
				result = m_array.PinValue();
				m_array.Value().Array[index] = item;
				if (fromAdd)
				{
					m_array.UpdateSize(ItemSizes.SizeOfInObjectArray(item));
				}
			}
			else
			{
				int bucketIndex = GetBucketIndex(index);
				IReference<StorableArray> reference = m_buckets[bucketIndex];
				UnPinCascadeHolder unPinCascadeHolder = new UnPinCascadeHolder();
				unPinCascadeHolder.AddCleanupRef(reference.PinValue());
				m_buckets.PinContainingBucket(bucketIndex, unPinCascadeHolder);
				result = unPinCascadeHolder;
				reference.Value().Array[GetIndexInBucket(index)] = item;
				if (fromAdd)
				{
					reference.UpdateSize(ItemSizes.SizeOfInObjectArray(item));
				}
			}
			return result;
		}

		public void RemoveRange(int index, int count)
		{
			CheckReadOnly("RemoveRange");
			if (index < 0)
			{
				Global.Tracer.Assert(condition: false, "ScalableList.RemoveRange: Index may not be less than 0");
			}
			if (count < 0)
			{
				Global.Tracer.Assert(condition: false, "ScalableList.RemoveRange: Count may not be less than 0");
			}
			if (index + count > Count)
			{
				Global.Tracer.Assert(condition: false, "ScalableList.RemoveRange: Index + Count may not be larger than the number of elements in the list");
			}
			if (m_array != null)
			{
				using (m_array.PinValue())
				{
					object[] array = m_array.Value().Array;
					int length = array.Length - count - index;
					Array.Copy(array, index + count, array, index, length);
				}
				m_count -= count;
			}
			else
			{
				int num = index + count;
				int num2 = index;
				while (true)
				{
					int indexInBucket = GetIndexInBucket(num);
					int indexInBucket2 = GetIndexInBucket(num2);
					int num3 = Math.Min(m_bucketSize - indexInBucket2, Math.Min(m_bucketSize - indexInBucket, m_count - num));
					if (num3 <= 0)
					{
						break;
					}
					IReference<StorableArray> reference = m_buckets[GetBucketIndex(num2)];
					IReference<StorableArray> reference2 = m_buckets[GetBucketIndex(num)];
					using (reference.PinValue())
					{
						using (reference2.PinValue())
						{
							object[] array2 = reference2.Value().Array;
							object[] array3 = reference.Value().Array;
							Array.Copy(array2, indexInBucket, array3, indexInBucket2, num3);
						}
					}
					num2 += num3;
					num += num3;
				}
				m_count -= count;
				int num4 = GetBucketIndex(m_count);
				if (m_count % m_bucketSize != 0 || num4 == 0)
				{
					num4++;
				}
				int num5 = m_buckets.Count - num4;
				if (num5 > 0)
				{
					m_buckets.RemoveRange(num4, num5);
					m_capacity -= num5 * m_bucketSize;
				}
			}
			m_version++;
		}

		public int BinarySearch(T value, IComparer comparer)
		{
			if (comparer == null)
			{
				Global.Tracer.Assert(condition: false, "Cannot pass null comparer to BinarySearch");
			}
			if (m_array != null)
			{
				return Array.BinarySearch(m_array.Value().Array, 0, Count, value, comparer);
			}
			return ArrayList.Adapter(this).BinarySearch(value, comparer);
		}

		int IList.Add(object value)
		{
			int count = Count;
			Add((T)value);
			return count;
		}

		bool IList.Contains(object value)
		{
			return Contains((T)value);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((T)value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (T)value);
		}

		void IList.Remove(object value)
		{
			Remove((T)value);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			InternalCopyTo(array, index);
		}

		private void CheckIndex(int index, int inclusiveLimit)
		{
			if (index < 0 || index > inclusiveLimit)
			{
				Global.Tracer.Assert(false, "ScalableList: Index {0} outside the allowed range [0::{1}]", index, inclusiveLimit);
			}
		}

		private void CheckReadOnly(string operation)
		{
			if (m_isReadOnly)
			{
				Global.Tracer.Assert(false, "Cannot {0} on a read-only ScalableList", operation);
			}
		}

		private int GetBucketIndex(int index)
		{
			return index / m_bucketSize;
		}

		private int GetIndexInBucket(int index)
		{
			return index % m_bucketSize;
		}

		private void EnsureCapacity(int count)
		{
			if (count <= m_capacity)
			{
				return;
			}
			if (m_array == null && m_buckets == null)
			{
				StorableArray storableArray = new StorableArray();
				storableArray.Array = new object[count];
				int emptySize = storableArray.EmptySize;
				if (m_bucketPinState == BucketPinState.UntilBucketFull || m_bucketPinState == BucketPinState.UntilListEnd)
				{
					m_array = m_cache.AllocateAndPin(storableArray, m_priority, emptySize);
				}
				else
				{
					m_array = m_cache.Allocate(storableArray, m_priority, emptySize);
				}
				m_capacity = count;
			}
			if (m_array != null)
			{
				if (count <= m_bucketSize)
				{
					int num = Math.Min(Math.Max(count, m_capacity * 2), m_bucketSize);
					using (m_array.PinValue())
					{
						Array.Resize(ref m_array.Value().Array, num);
					}
					m_capacity = num;
				}
				else
				{
					if (m_capacity < m_bucketSize)
					{
						using (m_array.PinValue())
						{
							Array.Resize(ref m_array.Value().Array, m_bucketSize);
						}
						m_capacity = m_bucketSize;
					}
					m_buckets = new ScalableList<IReference<StorableArray>>(m_priority, m_cache, 100, 10, m_bucketPinState == BucketPinState.UntilListEnd);
					m_buckets.Add(m_array);
					m_array = null;
				}
			}
			if (m_buckets == null)
			{
				return;
			}
			while (GetBucketIndex(count - 1) >= m_buckets.Count)
			{
				StorableArray storableArray2 = new StorableArray();
				storableArray2.Array = new object[m_bucketSize];
				int emptySize2 = storableArray2.EmptySize;
				if (m_bucketPinState == BucketPinState.UntilListEnd)
				{
					IReference<StorableArray> item = m_cache.AllocateAndPin(storableArray2, m_priority, emptySize2);
					m_buckets.Add(item);
				}
				else if (m_bucketPinState == BucketPinState.UntilBucketFull)
				{
					IReference<StorableArray> item = m_cache.AllocateAndPin(storableArray2, m_priority, emptySize2);
					m_buckets.AddAndPin(item);
				}
				else
				{
					IReference<StorableArray> item = m_cache.Allocate(storableArray2, m_priority, emptySize2);
					m_buckets.Add(item);
				}
				m_capacity += m_bucketSize;
			}
		}

		private void UnPinContainingBucket(int index)
		{
			if (m_array != null)
			{
				m_array.UnPinValue();
				return;
			}
			int bucketIndex = GetBucketIndex(index);
			m_buckets[bucketIndex].UnPinValue();
			m_buckets.UnPinContainingBucket(bucketIndex);
		}

		private void PinContainingBucket(int index, UnPinCascadeHolder cascadeHolder)
		{
			if (m_array != null)
			{
				cascadeHolder.AddCleanupRef(m_array.PinValue());
				return;
			}
			int bucketIndex = GetBucketIndex(index);
			cascadeHolder.AddCleanupRef(m_buckets[bucketIndex].PinValue());
			m_buckets.PinContainingBucket(bucketIndex, cascadeHolder);
		}

		private void InternalCopyTo(Array array, int arrayIndex)
		{
			if (array == null)
			{
				Global.Tracer.Assert(condition: false, "ScalableList.CopyTo: Dest array cannot be null");
			}
			if (arrayIndex < 0)
			{
				Global.Tracer.Assert(condition: false, "ScalableList.CopyTo: Index must not be less than 0");
			}
			if (arrayIndex != 1)
			{
				Global.Tracer.Assert(condition: false, "ScalableList.CopyTo: Array must be one-dimensional");
			}
			if (arrayIndex > array.Length - 1)
			{
				Global.Tracer.Assert(condition: false, "ScalableList.CopyTo: Start index must be less than the size of the array");
			}
			if (arrayIndex + m_count > array.Length)
			{
				Global.Tracer.Assert(condition: false, "ScalableList.CopyTo: Insufficent space in the target array");
			}
			if (m_array != null)
			{
				using (m_array.PinValue())
				{
					Array.Copy(m_array.Value().Array, 0, array, arrayIndex, m_count);
				}
				return;
			}
			int num = m_buckets.Count - 1;
			IReference<StorableArray> reference;
			for (int i = 0; i < num; i++)
			{
				reference = m_buckets[i];
				using (reference.PinValue())
				{
					Array.Copy(reference.Value().Array, 0, array, arrayIndex + i * m_bucketSize, m_bucketSize);
				}
			}
			int num2 = GetIndexInBucket(m_count);
			if (num2 == 0)
			{
				num2 = m_bucketSize;
			}
			reference = m_buckets[num];
			using (reference.PinValue())
			{
				Array.Copy(reference.Value().Array, 0, array, arrayIndex + num * m_bucketSize, num2);
			}
		}

		public void Add(T item)
		{
			CheckReadOnly("Add");
			EnsureCapacity(m_count + 1);
			m_count++;
			m_version++;
			SetValue(m_count - 1, item, fromAdd: true);
			CheckFilledBucket();
		}

		private void CheckFilledBucket()
		{
			if (m_bucketPinState == BucketPinState.UntilBucketFull && m_count % m_bucketSize == 0)
			{
				if (m_array != null)
				{
					m_array.UnPinValue();
					return;
				}
				int bucketIndex = GetBucketIndex(m_count - 1);
				m_buckets[bucketIndex].UnPinValue();
				m_buckets.UnPinContainingBucket(bucketIndex);
			}
		}

		public void AddRange(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				Global.Tracer.Assert(condition: false, "ScalableList.AddRange: Collection cannot be null");
			}
			foreach (T item in collection)
			{
				Add(item);
			}
		}

		public void AddRange(IList<T> list)
		{
			CheckReadOnly("AddRange");
			if (list == null)
			{
				Global.Tracer.Assert(condition: false, "ScalableList.AddRange(IList<T>): List to add may not be null");
			}
			EnsureCapacity(m_count + list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Add(list[i]);
			}
		}

		public void Clear()
		{
			CheckReadOnly("Clear");
			m_count = 0;
			if (m_array != null)
			{
				m_array.Free();
				m_array = null;
			}
			if (m_buckets != null)
			{
				int count = m_buckets.Count;
				for (int i = 0; i < count; i++)
				{
					m_buckets[i].Free();
				}
				m_buckets.Clear();
			}
			m_buckets = null;
			m_capacity = 0;
			m_version++;
		}

		public void UnPinAll()
		{
			if (m_bucketPinState == BucketPinState.UntilListEnd)
			{
				if (m_array != null)
				{
					m_array.UnPinValue();
				}
				if (m_buckets != null)
				{
					for (int i = 0; i < m_buckets.Count; i++)
					{
						m_buckets[i].UnPinValue();
					}
					m_buckets.UnPinAll();
				}
			}
			else
			{
				if (m_bucketPinState != BucketPinState.UntilBucketFull)
				{
					return;
				}
				if (m_count < Math.Max(m_capacity, m_bucketSize))
				{
					if (m_array != null)
					{
						m_array.UnPinValue();
					}
					else
					{
						int bucketIndex = GetBucketIndex(m_count - 1);
						m_buckets[bucketIndex].UnPinValue();
						m_buckets.UnPinContainingBucket(bucketIndex);
					}
				}
				if (m_buckets != null)
				{
					m_buckets.UnPinAll();
				}
			}
		}

		public void TransferTo(IScalabilityCache scaleCache)
		{
			if (m_array != null)
			{
				m_array = (IReference<StorableArray>)m_array.TransferTo(scaleCache);
			}
			else
			{
				m_buckets.TransferTo(scaleCache);
			}
			m_cache = scaleCache;
		}

		public bool Contains(T item)
		{
			return IndexOf(item) != -1;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			InternalCopyTo(array, arrayIndex);
		}

		public void SetReadOnly()
		{
			m_isReadOnly = true;
		}

		public bool Remove(T item)
		{
			CheckReadOnly("Remove");
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			if (m_array != null)
			{
				using (m_array.PinValue())
				{
					object[] array = m_array.Value().Array;
					return Remove(item, 0, array, m_count, @default);
				}
			}
			bool flag = false;
			int count = m_buckets.Count;
			for (int i = 0; i < count; i++)
			{
				if (flag)
				{
					break;
				}
				IReference<StorableArray> reference = m_buckets[i];
				using (reference.PinValue())
				{
					int limit = (i != count - 1) ? m_bucketSize : (m_count - i * m_bucketSize);
					object[] array2 = reference.Value().Array;
					flag = Remove(item, i * m_bucketSize, array2, limit, @default);
				}
			}
			return flag;
		}

		private bool Remove(T item, int baseIndex, object[] array, int limit, IEqualityComparer<T> comparer)
		{
			for (int i = 0; i < limit; i++)
			{
				if (comparer.Equals(item, (T)array[i]))
				{
					RemoveAt(baseIndex + i);
					return true;
				}
			}
			return false;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new ScalableListEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ScalableListEnumerator(this);
		}

		public void Dispose()
		{
			Clear();
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.BucketSize:
					writer.Write(m_bucketSize);
					break;
				case MemberName.Count:
					writer.Write(m_count);
					break;
				case MemberName.Capacity:
					writer.Write(m_capacity);
					break;
				case MemberName.Buckets:
					writer.Write(m_buckets);
					break;
				case MemberName.Array:
					writer.Write(m_array);
					break;
				case MemberName.Version:
					writer.Write(m_version);
					break;
				case MemberName.Priority:
					writer.Write(m_priority);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			m_cache = (reader.PersistenceHelper as IScalabilityCache);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.BucketSize:
					m_bucketSize = reader.ReadInt32();
					break;
				case MemberName.Count:
					m_count = reader.ReadInt32();
					break;
				case MemberName.Capacity:
					m_capacity = reader.ReadInt32();
					break;
				case MemberName.Buckets:
					m_buckets = reader.ReadRIFObject<ScalableList<IReference<StorableArray>>>();
					break;
				case MemberName.Array:
					m_array = (IReference<StorableArray>)reader.ReadRIFObject();
					break;
				case MemberName.Version:
					m_version = reader.ReadInt32();
					break;
				case MemberName.Priority:
					m_priority = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.BucketSize, Token.Int32));
				list.Add(new MemberInfo(MemberName.Count, Token.Int32));
				list.Add(new MemberInfo(MemberName.Capacity, Token.Int32));
				list.Add(new MemberInfo(MemberName.Buckets, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
				list.Add(new MemberInfo(MemberName.Array, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference));
				list.Add(new MemberInfo(MemberName.Version, Token.Int32));
				list.Add(new MemberInfo(MemberName.Priority, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
