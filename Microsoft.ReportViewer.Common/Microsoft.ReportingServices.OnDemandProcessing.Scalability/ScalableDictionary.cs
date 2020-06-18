using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IStorable, IPersistable
	{
		internal struct ScalableDictionaryEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
		{
			private class ContextItem<KeyType, ValueType>
			{
				public KeyType Key;

				public ValueType Value;

				public ContextItem(KeyType key, ValueType value)
				{
					Key = key;
					Value = value;
				}
			}

			private int m_currentValueIndex;

			private KeyValuePair<TKey, TValue> m_currentPair;

			private Stack<ContextItem<int, ScalableDictionaryNodeReference>> m_context;

			private int m_version;

			private ScalableDictionary<TKey, TValue> m_dictionary;

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					if (m_dictionary.m_version != m_version)
					{
						Global.Tracer.Assert(condition: false, "ScalableDictionaryEnumerator: Cannot use enumerator after modifying the underlying collection");
					}
					if (m_context.Count < 1)
					{
						Global.Tracer.Assert(condition: false, "ScalableDictionaryEnumerator: Enumerator beyond the bounds of the underlying collection");
					}
					return m_currentPair;
				}
			}

			object IEnumerator.Current => Current;

			internal ScalableDictionaryEnumerator(ScalableDictionary<TKey, TValue> dictionary)
			{
				m_dictionary = dictionary;
				m_version = dictionary.m_version;
				m_currentValueIndex = -1;
				m_currentPair = default(KeyValuePair<TKey, TValue>);
				m_context = null;
				Reset();
			}

			public void Dispose()
			{
				m_context = null;
				m_dictionary = null;
			}

			public bool MoveNext()
			{
				if (m_dictionary.m_version != m_version)
				{
					Global.Tracer.Assert(condition: false, "ScalableDictionaryEnumerator: Cannot use enumerator after modifying the underlying collection");
				}
				if (m_context.Count < 1 && m_currentValueIndex != -1)
				{
					return false;
				}
				if (m_context.Count == 0)
				{
					ContextItem<int, ScalableDictionaryNodeReference> item = new ContextItem<int, ScalableDictionaryNodeReference>(0, m_dictionary.m_root);
					m_currentValueIndex = 0;
					m_context.Push(item);
				}
				return FindNext();
			}

			private bool FindNext()
			{
				bool flag = false;
				while (m_context.Count > 0 && !flag)
				{
					ContextItem<int, ScalableDictionaryNodeReference> contextItem = m_context.Peek();
					ScalableDictionaryNodeReference value = contextItem.Value;
					using (value.PinValue())
					{
						flag = FindNext(value.Value(), contextItem);
					}
				}
				return flag;
			}

			private bool FindNext(ScalableDictionaryNode node, ContextItem<int, ScalableDictionaryNodeReference> curContext)
			{
				bool flag = false;
				while (!flag && curContext.Key < node.Entries.Length)
				{
					IScalableDictionaryEntry scalableDictionaryEntry = node.Entries[curContext.Key];
					if (scalableDictionaryEntry != null)
					{
						switch (scalableDictionaryEntry.GetObjectType())
						{
						case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
						{
							ScalableDictionaryNodeReference value = scalableDictionaryEntry as ScalableDictionaryNodeReference;
							m_context.Push(new ContextItem<int, ScalableDictionaryNodeReference>(0, value));
							flag = FindNext();
							break;
						}
						case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues:
						{
							ScalableDictionaryValues scalableDictionaryValues = scalableDictionaryEntry as ScalableDictionaryValues;
							if (m_currentValueIndex < scalableDictionaryValues.Count)
							{
								m_currentPair = new KeyValuePair<TKey, TValue>((TKey)scalableDictionaryValues.Keys[m_currentValueIndex], (TValue)scalableDictionaryValues.Values[m_currentValueIndex]);
								m_currentValueIndex++;
								return true;
							}
							m_currentValueIndex = 0;
							break;
						}
						default:
							Global.Tracer.Assert(condition: false, "Unknown ObjectType");
							break;
						}
					}
					curContext.Key++;
				}
				if (!flag)
				{
					m_currentValueIndex = 0;
					m_context.Pop();
				}
				return flag;
			}

			public void Reset()
			{
				m_currentValueIndex = -1;
				m_context = new Stack<ContextItem<int, ScalableDictionaryNodeReference>>();
				m_version = m_dictionary.m_version;
			}
		}

		internal struct ScalableDictionaryKeysEnumerator : IEnumerator<TKey>, IDisposable, IEnumerator
		{
			private ScalableDictionary<TKey, TValue> m_dictionary;

			private IEnumerator<KeyValuePair<TKey, TValue>> m_enumerator;

			public TKey Current => m_enumerator.Current.Key;

			object IEnumerator.Current => Current;

			internal ScalableDictionaryKeysEnumerator(ScalableDictionary<TKey, TValue> dictionary)
			{
				m_dictionary = dictionary;
				m_enumerator = dictionary.GetEnumerator();
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return m_enumerator.MoveNext();
			}

			public void Reset()
			{
				m_enumerator.Reset();
			}
		}

		internal struct ScalableDictionaryValuesEnumerator : IEnumerator<TValue>, IDisposable, IEnumerator
		{
			private ScalableDictionary<TKey, TValue> m_dictionary;

			private IEnumerator<KeyValuePair<TKey, TValue>> m_enumerator;

			public TValue Current => m_enumerator.Current.Value;

			object IEnumerator.Current => Current;

			internal ScalableDictionaryValuesEnumerator(ScalableDictionary<TKey, TValue> dictionary)
			{
				m_dictionary = dictionary;
				m_enumerator = dictionary.GetEnumerator();
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return m_enumerator.MoveNext();
			}

			public void Reset()
			{
				m_enumerator.Reset();
			}
		}

		internal sealed class ScalableDictionaryKeysCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable
		{
			private ScalableDictionary<TKey, TValue> m_dictionary;

			public int Count => m_dictionary.Count;

			public bool IsReadOnly => true;

			internal ScalableDictionaryKeysCollection(ScalableDictionary<TKey, TValue> dictionary)
			{
				m_dictionary = dictionary;
			}

			public void Add(TKey item)
			{
				Global.Tracer.Assert(condition: false, "ScalableDictionaryKeysCollection: Dictionary keys collection is read only");
			}

			public void Clear()
			{
				Global.Tracer.Assert(condition: false, "ScalableDictionaryKeysCollection: Dictionary keys collection is read only");
			}

			public bool Contains(TKey item)
			{
				return m_dictionary.ContainsKey(item);
			}

			public void CopyTo(TKey[] array, int arrayIndex)
			{
				if (arrayIndex < 0)
				{
					Global.Tracer.Assert(condition: false, "ScalableDictionaryKeysCollection.CopyTo: Index must be greater than 0");
				}
				if (array == null)
				{
					Global.Tracer.Assert(condition: false, "ScalableDictionaryKeysCollection.CopyTo: Specified array must not be null");
				}
				if (array.Rank > 1)
				{
					Global.Tracer.Assert(condition: false, "ScalableDictionaryKeysCollection.CopyTo: Specified array must be 1 dimensional");
				}
				if (arrayIndex + Count > array.Length)
				{
					Global.Tracer.Assert(condition: false, "ScalableDictionaryKeysCollection.CopyTo: Insufficent space in destination array");
				}
				using (IEnumerator<TKey> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TKey val = array[arrayIndex] = enumerator.Current;
						arrayIndex++;
					}
				}
			}

			public bool Remove(TKey item)
			{
				Global.Tracer.Assert(condition: false, "ScalableDictionaryKeysCollection.Remove: Dictionary keys collection is read only");
				return false;
			}

			public IEnumerator<TKey> GetEnumerator()
			{
				return new ScalableDictionaryKeysEnumerator(m_dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		internal sealed class ScalableDictionaryValuesCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable
		{
			private ScalableDictionary<TKey, TValue> m_dictionary;

			public int Count => m_dictionary.Count;

			public bool IsReadOnly => true;

			internal ScalableDictionaryValuesCollection(ScalableDictionary<TKey, TValue> dictionary)
			{
				m_dictionary = dictionary;
			}

			public void Add(TValue item)
			{
				Global.Tracer.Assert(condition: false, "ScalableDictionaryValuesCollection.Add: Dictionary values collection is read only");
			}

			public void Clear()
			{
				Global.Tracer.Assert(condition: false, "ScalableDictionaryValuesCollection.Clear: Dictionary values collection is read only");
			}

			public bool Contains(TValue item)
			{
				return m_dictionary.ContainsValue(item);
			}

			public void CopyTo(TValue[] array, int arrayIndex)
			{
				if (arrayIndex < 0)
				{
					Global.Tracer.Assert(condition: false, "ScalableDictionaryValuesCollection.CopyTo: Index must be greater than 0");
				}
				if (array == null)
				{
					Global.Tracer.Assert(condition: false, "ScalableDictionaryValuesCollection.CopyTo: Specified array must not be null");
				}
				if (array.Rank > 1)
				{
					Global.Tracer.Assert(condition: false, "ScalableDictionaryValuesCollection.CopyTo: Specified array must be 1 dimensional");
				}
				if (arrayIndex + Count > array.Length)
				{
					Global.Tracer.Assert(condition: false, "ScalableDictionaryValuesCollection.CopyTo: Insufficent space in destination array");
				}
				using (IEnumerator<TValue> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TValue val = array[arrayIndex] = enumerator.Current;
						arrayIndex++;
					}
				}
			}

			public bool Remove(TValue item)
			{
				Global.Tracer.Assert(condition: false, "ScalableDictionaryValuesCollection.Remove: Dictionary values collection is read only");
				return false;
			}

			public IEnumerator<TValue> GetEnumerator()
			{
				return new ScalableDictionaryValuesEnumerator(m_dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		private int m_nodeCapacity;

		private int m_valuesCapacity;

		[StaticReference]
		private IEqualityComparer<TKey> m_comparer;

		private int m_count;

		private int m_version;

		private ScalableDictionaryNodeReference m_root;

		private bool m_useFixedReferences;

		private int m_priority;

		[NonSerialized]
		private IScalabilityCache m_scalabilityCache;

		[NonSerialized]
		private ScalableDictionaryKeysCollection m_keysCollection;

		[NonSerialized]
		private ScalableDictionaryValuesCollection m_valuesCollection;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		public ICollection<TKey> Keys
		{
			get
			{
				if (m_keysCollection == null)
				{
					m_keysCollection = new ScalableDictionaryKeysCollection(this);
				}
				return m_keysCollection;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				if (m_valuesCollection == null)
				{
					m_valuesCollection = new ScalableDictionaryValuesCollection(this);
				}
				return m_valuesCollection;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				if (!TryGetValue(key, out TValue value))
				{
					Global.Tracer.Assert(condition: false, "Given key is not present in the dictionary");
				}
				return value;
			}
			set
			{
				if (Insert(m_root, GetHashCode(key), key, value, add: false, 0, updateSize: true, out IDisposable cleanupRef))
				{
					m_count++;
				}
				m_version++;
				cleanupRef.Dispose();
			}
		}

		public IEqualityComparer<TKey> Comparer => m_comparer;

		public int Count => m_count;

		public bool IsReadOnly => false;

		public int Size => 8 + ItemSizes.ReferenceSize + 4 + 4 + ItemSizes.SizeOf(m_root) + ItemSizes.ReferenceSize + ItemSizes.ReferenceSize + ItemSizes.ReferenceSize + ItemSizes.ReferenceSize + 4 + 1;

		public ScalableDictionary()
		{
		}

		internal ScalableDictionary(int priority, IScalabilityCache cache)
			: this(priority, cache, 23, 5, (IEqualityComparer<TKey>)null)
		{
		}

		internal ScalableDictionary(int priority, IScalabilityCache cache, int nodeCapacity, int entryCapacity)
			: this(priority, cache, nodeCapacity, entryCapacity, (IEqualityComparer<TKey>)null)
		{
		}

		internal ScalableDictionary(int priority, IScalabilityCache cache, int nodeCapacity, int entryCapacity, IEqualityComparer<TKey> comparer)
			: this(priority, cache, nodeCapacity, entryCapacity, comparer, useFixedReferences: false)
		{
		}

		internal ScalableDictionary(int priority, IScalabilityCache cache, int nodeCapacity, int entryCapacity, IEqualityComparer<TKey> comparer, bool useFixedReferences)
		{
			m_priority = priority;
			m_scalabilityCache = cache;
			m_nodeCapacity = nodeCapacity;
			m_valuesCapacity = entryCapacity;
			m_comparer = comparer;
			m_version = 0;
			m_count = 0;
			m_useFixedReferences = useFixedReferences;
			if (m_comparer == null)
			{
				m_comparer = EqualityComparer<TKey>.Default;
			}
			m_root = BuildNode(0, m_nodeCapacity);
		}

		public void Add(TKey key, TValue value)
		{
			AddAndPin(key, value).Dispose();
		}

		public IDisposable AddAndPin(TKey key, TValue value)
		{
			if (Insert(m_root, GetHashCode(key), key, value, add: true, 0, updateSize: true, out IDisposable cleanupRef))
			{
				m_count++;
			}
			m_version++;
			return cleanupRef;
		}

		public bool ContainsKey(TKey key)
		{
			TValue value;
			return TryGetValue(key, out value);
		}

		public bool Remove(TKey key)
		{
			int newCount;
			bool num = Remove(m_root, GetHashCode(key), key, 0, out newCount);
			if (num)
			{
				m_count--;
				m_version++;
			}
			return num;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			IDisposable reference;
			bool num = TryGetAndPin(key, out value, out reference);
			if (num)
			{
				reference.Dispose();
			}
			return num;
		}

		public bool ContainsValue(TValue value)
		{
			IEqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (@default.Equals(enumerator.Current.Value, value))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool TryGetAndPin(TKey key, out TValue value, out IDisposable reference)
		{
			if (key == null)
			{
				Global.Tracer.Assert(condition: false, "ScalableDictionary: Key cannot be null");
			}
			return Find(m_root, GetHashCode(key), key, 0, out value, out reference);
		}

		public IDisposable GetAndPin(TKey key, out TValue value)
		{
			IDisposable reference;
			bool condition = TryGetAndPin(key, out value, out reference);
			Global.Tracer.Assert(condition, "Missing expected dictionary item with key");
			return reference;
		}

		public void TransferTo(IScalabilityCache scaleCache)
		{
			m_root = (ScalableDictionaryNodeReference)m_root.TransferTo(scaleCache);
			m_scalabilityCache = scaleCache;
		}

		public void UpdateComparer(IEqualityComparer<TKey> comparer)
		{
			Global.Tracer.Assert(m_comparer == null, "Cannot update equality comparer in the middle of a table computation.");
			m_comparer = comparer;
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			FreeChildren(m_root);
			m_count = 0;
			m_version++;
		}

		public void Dispose()
		{
			if (m_root != null)
			{
				Clear();
				m_root.Free();
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			IEqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			if (TryGetValue(item.Key, out TValue value))
			{
				return @default.Equals(item.Value, value);
			}
			return false;
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (arrayIndex < 0)
			{
				Global.Tracer.Assert(condition: false, "ScalableDictionary.CopyTo: Index must be greater than 0");
			}
			if (array == null)
			{
				Global.Tracer.Assert(condition: false, "ScalableDictionary.CopyTo: Specified array must not be null");
			}
			if (array.Rank > 1)
			{
				Global.Tracer.Assert(false, "ScalableDictionary.CopyTo: Specified array must be 1 dimensional", "array");
			}
			if (arrayIndex + Count > array.Length)
			{
				Global.Tracer.Assert(condition: false, "ScalableDictionary.CopyTo: Insufficent space in destination array");
			}
			using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> keyValuePair = array[arrayIndex] = enumerator.Current;
					arrayIndex++;
				}
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			if (((ICollection<KeyValuePair<TKey, TValue>>)this).Contains(item))
			{
				return Remove(item.Key);
			}
			return false;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new ScalableDictionaryEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private int GetHashCode(TKey key)
		{
			if (key == null || key is DBNull)
			{
				return DBNull.Value.GetHashCode();
			}
			return m_comparer.GetHashCode(key);
		}

		private void FreeChildren(ScalableDictionaryNodeReference nodeRef)
		{
			using (nodeRef.PinValue())
			{
				ScalableDictionaryNode scalableDictionaryNode = nodeRef.Value();
				for (int i = 0; i < scalableDictionaryNode.Entries.Length; i++)
				{
					IScalableDictionaryEntry scalableDictionaryEntry = scalableDictionaryNode.Entries[i];
					if (scalableDictionaryEntry != null)
					{
						switch (scalableDictionaryEntry.GetObjectType())
						{
						case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
						{
							ScalableDictionaryNodeReference scalableDictionaryNodeReference = scalableDictionaryEntry as ScalableDictionaryNodeReference;
							FreeChildren(scalableDictionaryNodeReference);
							scalableDictionaryNodeReference.Free();
							break;
						}
						default:
							Global.Tracer.Assert(condition: false, "Unknown ObjectType");
							break;
						case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues:
							break;
						}
					}
					scalableDictionaryNode.Entries[i] = null;
				}
				scalableDictionaryNode.Count = 0;
			}
		}

		private ScalableDictionaryNodeReference BuildNode(int level, int capacity)
		{
			ScalableDictionaryNode scalableDictionaryNode = new ScalableDictionaryNode(capacity);
			if (m_useFixedReferences)
			{
				return (ScalableDictionaryNodeReference)m_scalabilityCache.GenerateFixedReference(scalableDictionaryNode);
			}
			return (ScalableDictionaryNodeReference)m_scalabilityCache.Allocate(scalableDictionaryNode, m_priority, scalableDictionaryNode.EmptySize);
		}

		private bool Insert(ScalableDictionaryNodeReference nodeRef, int hashCode, TKey key, TValue value, bool add, int level, bool updateSize, out IDisposable cleanupRef)
		{
			IDisposable disposable = nodeRef.PinValue();
			ScalableDictionaryNode scalableDictionaryNode = nodeRef.Value();
			bool flag = false;
			int num = HashToSlot(scalableDictionaryNode, hashCode, level);
			IScalableDictionaryEntry scalableDictionaryEntry = scalableDictionaryNode.Entries[num];
			if (scalableDictionaryEntry == null)
			{
				ScalableDictionaryValues scalableDictionaryValues = new ScalableDictionaryValues(m_valuesCapacity);
				scalableDictionaryValues.Keys[0] = key;
				scalableDictionaryValues.Values[0] = value;
				scalableDictionaryValues.Count++;
				scalableDictionaryNode.Entries[num] = scalableDictionaryValues;
				flag = true;
				cleanupRef = disposable;
				if (!m_useFixedReferences && updateSize)
				{
					int sizeBytesDelta = ItemSizes.SizeOfInObjectArray(key) + ItemSizes.SizeOfInObjectArray(value) + scalableDictionaryValues.EmptySize;
					nodeRef.UpdateSize(sizeBytesDelta);
				}
			}
			else
			{
				switch (scalableDictionaryEntry.GetObjectType())
				{
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
				{
					ScalableDictionaryNodeReference nodeRef2 = scalableDictionaryEntry as ScalableDictionaryNodeReference;
					flag = Insert(nodeRef2, hashCode, key, value, add, level + 1, updateSize, out cleanupRef);
					break;
				}
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues:
				{
					ScalableDictionaryValues scalableDictionaryValues2 = scalableDictionaryEntry as ScalableDictionaryValues;
					bool flag2 = false;
					cleanupRef = null;
					for (int i = 0; i < scalableDictionaryValues2.Count; i++)
					{
						if (m_comparer.Equals(key, (TKey)scalableDictionaryValues2.Keys[i]))
						{
							if (add)
							{
								Global.Tracer.Assert(condition: false, "ScalableDictionary: An element with the same key already exists within the Dictionary");
							}
							scalableDictionaryValues2.Values[i] = value;
							flag2 = true;
							flag = false;
							cleanupRef = disposable;
							break;
						}
					}
					if (flag2)
					{
						break;
					}
					if (scalableDictionaryValues2.Count < scalableDictionaryValues2.Capacity)
					{
						int count = scalableDictionaryValues2.Count;
						scalableDictionaryValues2.Keys[count] = key;
						scalableDictionaryValues2.Values[count] = value;
						scalableDictionaryValues2.Count++;
						flag = true;
						cleanupRef = disposable;
						if (!m_useFixedReferences && updateSize)
						{
							nodeRef.UpdateSize(ItemSizes.SizeOfInObjectArray(key));
							nodeRef.UpdateSize(ItemSizes.SizeOfInObjectArray(value));
						}
						break;
					}
					ScalableDictionaryNodeReference scalableDictionaryNodeReference = BuildNode(level + 1, m_nodeCapacity);
					scalableDictionaryNode.Entries[num] = scalableDictionaryNodeReference;
					using (scalableDictionaryNodeReference.PinValue())
					{
						if (!m_useFixedReferences && updateSize)
						{
							int num2 = ItemSizes.SizeOfInObjectArray(scalableDictionaryValues2);
							nodeRef.UpdateSize(num2 * -1);
							scalableDictionaryNodeReference.UpdateSize(num2);
						}
						for (int j = 0; j < scalableDictionaryValues2.Count; j++)
						{
							TKey key2 = (TKey)scalableDictionaryValues2.Keys[j];
							Insert(scalableDictionaryNodeReference, GetHashCode(key2), key2, (TValue)scalableDictionaryValues2.Values[j], add: false, level + 1, updateSize: false, out IDisposable cleanupRef2);
							cleanupRef2.Dispose();
						}
						flag = Insert(scalableDictionaryNodeReference, hashCode, key, value, add, level + 1, updateSize, out cleanupRef);
					}
					break;
				}
				default:
					Global.Tracer.Assert(condition: false, "Unknown ObjectType");
					cleanupRef = null;
					break;
				}
			}
			if (flag)
			{
				scalableDictionaryNode.Count++;
			}
			if (disposable != cleanupRef)
			{
				disposable.Dispose();
			}
			return flag;
		}

		private int HashToSlot(ScalableDictionaryNode node, int hashCode, int level)
		{
			int prime = PrimeHelper.GetPrime(level);
			int hashInputA = PrimeHelper.GetHashInputA(level);
			int hashInputB = PrimeHelper.GetHashInputB(level);
			return Math.Abs(hashInputA * hashCode + hashInputB) % prime % node.Entries.Length;
		}

		private bool Find(ScalableDictionaryNodeReference nodeRef, int hashCode, TKey key, int level, out TValue value, out IDisposable containingNodeRef)
		{
			containingNodeRef = null;
			IDisposable disposable = nodeRef.PinValue();
			ScalableDictionaryNode scalableDictionaryNode = nodeRef.Value();
			value = default(TValue);
			bool result = false;
			int num = HashToSlot(scalableDictionaryNode, hashCode, level);
			IScalableDictionaryEntry scalableDictionaryEntry = scalableDictionaryNode.Entries[num];
			if (scalableDictionaryEntry != null)
			{
				switch (scalableDictionaryEntry.GetObjectType())
				{
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
				{
					ScalableDictionaryNodeReference nodeRef2 = scalableDictionaryEntry as ScalableDictionaryNodeReference;
					result = Find(nodeRef2, hashCode, key, level + 1, out value, out containingNodeRef);
					break;
				}
				case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues:
				{
					ScalableDictionaryValues scalableDictionaryValues = scalableDictionaryEntry as ScalableDictionaryValues;
					for (int i = 0; i < scalableDictionaryValues.Count; i++)
					{
						if (m_comparer.Equals(key, (TKey)scalableDictionaryValues.Keys[i]))
						{
							value = (TValue)scalableDictionaryValues.Values[i];
							containingNodeRef = disposable;
							return true;
						}
					}
					break;
				}
				default:
					Global.Tracer.Assert(condition: false, "Unknown ObjectType");
					break;
				}
			}
			disposable.Dispose();
			return result;
		}

		private bool Remove(ScalableDictionaryNodeReference nodeRef, int hashCode, TKey key, int level, out int newCount)
		{
			using (nodeRef.PinValue())
			{
				ScalableDictionaryNode scalableDictionaryNode = nodeRef.Value();
				bool flag = false;
				int num = HashToSlot(scalableDictionaryNode, hashCode, level);
				IScalableDictionaryEntry scalableDictionaryEntry = scalableDictionaryNode.Entries[num];
				if (scalableDictionaryEntry == null)
				{
					flag = false;
				}
				else
				{
					switch (scalableDictionaryEntry.GetObjectType())
					{
					case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
					{
						ScalableDictionaryNodeReference scalableDictionaryNodeReference = scalableDictionaryEntry as ScalableDictionaryNodeReference;
						flag = Remove(scalableDictionaryNodeReference, hashCode, key, level + 1, out int newCount2);
						if (flag && newCount2 == 0)
						{
							scalableDictionaryNode.Entries[num] = null;
							scalableDictionaryNodeReference.Free();
						}
						break;
					}
					case Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryValues:
					{
						ScalableDictionaryValues scalableDictionaryValues = scalableDictionaryEntry as ScalableDictionaryValues;
						for (int i = 0; i < scalableDictionaryValues.Count; i++)
						{
							if (!m_comparer.Equals(key, (TKey)scalableDictionaryValues.Keys[i]))
							{
								continue;
							}
							if (scalableDictionaryValues.Count == 1)
							{
								scalableDictionaryNode.Entries[num] = null;
							}
							else
							{
								scalableDictionaryValues.Keys[i] = null;
								scalableDictionaryValues.Values[i] = null;
								scalableDictionaryValues.Count--;
								int num2 = scalableDictionaryValues.Count - i;
								if (num2 > 0)
								{
									Array.Copy(scalableDictionaryValues.Keys, i + 1, scalableDictionaryValues.Keys, i, num2);
									Array.Copy(scalableDictionaryValues.Values, i + 1, scalableDictionaryValues.Values, i, num2);
								}
							}
							flag = true;
							break;
						}
						break;
					}
					default:
						Global.Tracer.Assert(condition: false, "Unknown ObjectType");
						break;
					}
				}
				if (flag)
				{
					scalableDictionaryNode.Count--;
				}
				newCount = scalableDictionaryNode.Count;
				return flag;
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.NodeCapacity:
					writer.Write(m_nodeCapacity);
					break;
				case MemberName.ValuesCapacity:
					writer.Write(m_valuesCapacity);
					break;
				case MemberName.Comparer:
				{
					int value = int.MinValue;
					if (scalabilityCache.CacheType == ScalabilityCacheType.Standard)
					{
						value = scalabilityCache.StoreStaticReference(m_comparer);
					}
					writer.Write(value);
					break;
				}
				case MemberName.Count:
					writer.Write(m_count);
					break;
				case MemberName.Version:
					writer.Write(m_version);
					break;
				case MemberName.Root:
					writer.Write(m_root);
					break;
				case MemberName.UseFixedReferences:
					writer.Write(m_useFixedReferences);
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
			IScalabilityCache scalabilityCache = m_scalabilityCache = (reader.PersistenceHelper as IScalabilityCache);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.NodeCapacity:
					m_nodeCapacity = reader.ReadInt32();
					break;
				case MemberName.ValuesCapacity:
					m_valuesCapacity = reader.ReadInt32();
					break;
				case MemberName.Comparer:
				{
					int id = reader.ReadInt32();
					if (scalabilityCache.CacheType == ScalabilityCacheType.Standard)
					{
						m_comparer = (IEqualityComparer<TKey>)scalabilityCache.FetchStaticReference(id);
					}
					break;
				}
				case MemberName.Count:
					m_count = reader.ReadInt32();
					break;
				case MemberName.Version:
					m_version = reader.ReadInt32();
					break;
				case MemberName.Root:
					m_root = (ScalableDictionaryNodeReference)reader.ReadRIFObject();
					break;
				case MemberName.UseFixedReferences:
					m_useFixedReferences = reader.ReadBoolean();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionary;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.NodeCapacity, Token.Int32));
				list.Add(new MemberInfo(MemberName.ValuesCapacity, Token.Int32));
				list.Add(new MemberInfo(MemberName.Comparer, Token.Int32));
				list.Add(new MemberInfo(MemberName.Count, Token.Int32));
				list.Add(new MemberInfo(MemberName.Version, Token.Int32));
				list.Add(new MemberInfo(MemberName.Root, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference));
				list.Add(new MemberInfo(MemberName.UseFixedReferences, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Priority, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
