using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SegmentedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		internal interface ISegmentedDictionaryEntry
		{
			SegmentedDictionaryEntryType EntryType
			{
				get;
			}
		}

		internal enum SegmentedDictionaryEntryType
		{
			Node,
			Values
		}

		internal class SegmentedDictionaryNode : ISegmentedDictionaryEntry
		{
			internal ISegmentedDictionaryEntry[] Entries;

			internal int Count;

			public SegmentedDictionaryEntryType EntryType => SegmentedDictionaryEntryType.Node;

			internal SegmentedDictionaryNode(int capacity)
			{
				Entries = new ISegmentedDictionaryEntry[capacity];
			}
		}

		internal class SegmentedDictionaryValues : ISegmentedDictionaryEntry
		{
			private TKey[] m_keys;

			private TValue[] m_values;

			private int m_count;

			public TKey[] Keys => m_keys;

			public TValue[] Values => m_values;

			public int Count
			{
				get
				{
					return m_count;
				}
				set
				{
					m_count = value;
				}
			}

			public int Capacity => m_keys.Length;

			public SegmentedDictionaryEntryType EntryType => SegmentedDictionaryEntryType.Values;

			public SegmentedDictionaryValues(int capacity)
			{
				m_count = 0;
				m_keys = new TKey[capacity];
				m_values = new TValue[capacity];
			}
		}

		internal struct SegmentedDictionaryEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
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

			private Stack<ContextItem<int, SegmentedDictionaryNode>> m_context;

			private int m_version;

			private SegmentedDictionary<TKey, TValue> m_dictionary;

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					if (m_dictionary.m_version != m_version)
					{
						Global.Tracer.Assert(condition: false, "SegmentedDictionaryEnumerator: Cannot use enumerator after modifying the underlying collection");
					}
					if (m_context.Count < 1)
					{
						Global.Tracer.Assert(condition: false, "SegmentedDictionaryEnumerator: Enumerator beyond the bounds of the underlying collection");
					}
					return m_currentPair;
				}
			}

			object IEnumerator.Current => Current;

			internal SegmentedDictionaryEnumerator(SegmentedDictionary<TKey, TValue> dictionary)
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
					Global.Tracer.Assert(condition: false, "SegmentedDictionaryEnumerator: Cannot use enumerator after modifying the underlying collection");
				}
				if (m_context.Count < 1 && m_currentValueIndex != -1)
				{
					return false;
				}
				if (m_context.Count == 0)
				{
					ContextItem<int, SegmentedDictionaryNode> item = new ContextItem<int, SegmentedDictionaryNode>(0, m_dictionary.m_root);
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
					ContextItem<int, SegmentedDictionaryNode> contextItem = m_context.Peek();
					flag = FindNext(contextItem.Value, contextItem);
				}
				return flag;
			}

			private bool FindNext(SegmentedDictionaryNode node, ContextItem<int, SegmentedDictionaryNode> curContext)
			{
				bool flag = false;
				while (!flag && curContext.Key < node.Entries.Length)
				{
					ISegmentedDictionaryEntry segmentedDictionaryEntry = node.Entries[curContext.Key];
					if (segmentedDictionaryEntry != null)
					{
						switch (segmentedDictionaryEntry.EntryType)
						{
						case SegmentedDictionaryEntryType.Node:
						{
							SegmentedDictionaryNode value = segmentedDictionaryEntry as SegmentedDictionaryNode;
							m_context.Push(new ContextItem<int, SegmentedDictionaryNode>(0, value));
							flag = FindNext();
							break;
						}
						case SegmentedDictionaryEntryType.Values:
						{
							SegmentedDictionaryValues segmentedDictionaryValues = segmentedDictionaryEntry as SegmentedDictionaryValues;
							if (m_currentValueIndex < segmentedDictionaryValues.Count)
							{
								m_currentPair = new KeyValuePair<TKey, TValue>(segmentedDictionaryValues.Keys[m_currentValueIndex], segmentedDictionaryValues.Values[m_currentValueIndex]);
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
				m_context = new Stack<ContextItem<int, SegmentedDictionaryNode>>();
				m_version = m_dictionary.m_version;
			}
		}

		internal struct SegmentedDictionaryKeysEnumerator : IEnumerator<TKey>, IDisposable, IEnumerator
		{
			private SegmentedDictionary<TKey, TValue> m_dictionary;

			private IEnumerator<KeyValuePair<TKey, TValue>> m_enumerator;

			public TKey Current => m_enumerator.Current.Key;

			object IEnumerator.Current => Current;

			internal SegmentedDictionaryKeysEnumerator(SegmentedDictionary<TKey, TValue> dictionary)
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

		internal struct SegmentedDictionaryValuesEnumerator : IEnumerator<TValue>, IDisposable, IEnumerator
		{
			private SegmentedDictionary<TKey, TValue> m_dictionary;

			private IEnumerator<KeyValuePair<TKey, TValue>> m_enumerator;

			public TValue Current => m_enumerator.Current.Value;

			object IEnumerator.Current => Current;

			internal SegmentedDictionaryValuesEnumerator(SegmentedDictionary<TKey, TValue> dictionary)
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

		internal class SegmentedDictionaryKeysCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable
		{
			private SegmentedDictionary<TKey, TValue> m_dictionary;

			public int Count => m_dictionary.Count;

			public bool IsReadOnly => true;

			internal SegmentedDictionaryKeysCollection(SegmentedDictionary<TKey, TValue> dictionary)
			{
				m_dictionary = dictionary;
			}

			public void Add(TKey item)
			{
				Global.Tracer.Assert(condition: false, "SegmentedDictionaryKeysCollection: Dictionary keys collection is read only");
			}

			public void Clear()
			{
				Global.Tracer.Assert(condition: false, "SegmentedDictionaryKeysCollection: Dictionary keys collection is read only");
			}

			public bool Contains(TKey item)
			{
				return m_dictionary.ContainsKey(item);
			}

			public void CopyTo(TKey[] array, int arrayIndex)
			{
				if (arrayIndex < 0)
				{
					Global.Tracer.Assert(condition: false, "SegmentedDictionaryKeysCollection.CopyTo: Index must be greater than 0");
				}
				if (array == null)
				{
					Global.Tracer.Assert(condition: false, "SegmentedDictionaryKeysCollection.CopyTo: Specified array must not be null");
				}
				if (array.Rank > 1)
				{
					Global.Tracer.Assert(condition: false, "SegmentedDictionaryKeysCollection.CopyTo: Specified array must be 1 dimensional");
				}
				if (arrayIndex + Count > array.Length)
				{
					Global.Tracer.Assert(condition: false, "SegmentedDictionaryKeysCollection.CopyTo: Insufficent space in destination array");
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
				Global.Tracer.Assert(condition: false, "SegmentedDictionaryKeysCollection.Remove: Dictionary keys collection is read only");
				return false;
			}

			public IEnumerator<TKey> GetEnumerator()
			{
				return new SegmentedDictionaryKeysEnumerator(m_dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		internal class SegmentedDictionaryValuesCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable
		{
			private SegmentedDictionary<TKey, TValue> m_dictionary;

			public int Count => m_dictionary.Count;

			public bool IsReadOnly => true;

			internal SegmentedDictionaryValuesCollection(SegmentedDictionary<TKey, TValue> dictionary)
			{
				m_dictionary = dictionary;
			}

			public void Add(TValue item)
			{
				Global.Tracer.Assert(condition: false, "SegmentedDictionaryValuesCollection.Add: Dictionary values collection is read only");
			}

			public void Clear()
			{
				Global.Tracer.Assert(condition: false, "SegmentedDictionaryValuesCollection.Clear: Dictionary values collection is read only");
			}

			public bool Contains(TValue item)
			{
				return m_dictionary.ContainsValue(item);
			}

			public void CopyTo(TValue[] array, int arrayIndex)
			{
				if (arrayIndex < 0)
				{
					Global.Tracer.Assert(condition: false, "SegmentedDictionaryValuesCollection.CopyTo: Index must be greater than 0");
				}
				if (array == null)
				{
					Global.Tracer.Assert(condition: false, "SegmentedDictionaryValuesCollection.CopyTo: Specified array must not be null");
				}
				if (array.Rank > 1)
				{
					Global.Tracer.Assert(condition: false, "SegmentedDictionaryValuesCollection.CopyTo: Specified array must be 1 dimensional");
				}
				if (arrayIndex + Count > array.Length)
				{
					Global.Tracer.Assert(condition: false, "SegmentedDictionaryValuesCollection.CopyTo: Insufficent space in destination array");
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
				Global.Tracer.Assert(condition: false, "SegmentedDictionaryValuesCollection.Remove: Dictionary values collection is read only");
				return false;
			}

			public IEnumerator<TValue> GetEnumerator()
			{
				return new SegmentedDictionaryValuesEnumerator(m_dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		private int m_nodeCapacity;

		private int m_valuesCapacity;

		private IEqualityComparer<TKey> m_comparer;

		private int m_count;

		private int m_version;

		private SegmentedDictionaryNode m_root;

		private SegmentedDictionaryKeysCollection m_keysCollection;

		private SegmentedDictionaryValuesCollection m_valuesCollection;

		public ICollection<TKey> Keys
		{
			get
			{
				if (m_keysCollection == null)
				{
					m_keysCollection = new SegmentedDictionaryKeysCollection(this);
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
					m_valuesCollection = new SegmentedDictionaryValuesCollection(this);
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
				if (Insert(m_root, m_comparer.GetHashCode(key), key, value, add: false, 0))
				{
					m_count++;
				}
				m_version++;
			}
		}

		public IEqualityComparer<TKey> Comparer => m_comparer;

		public int Count => m_count;

		public bool IsReadOnly => false;

		internal SegmentedDictionary(int priority, IScalabilityCache cache)
			: this(23, 5, (IEqualityComparer<TKey>)null)
		{
		}

		internal SegmentedDictionary(int nodeCapacity, int entryCapacity)
			: this(nodeCapacity, entryCapacity, (IEqualityComparer<TKey>)null)
		{
		}

		internal SegmentedDictionary(int nodeCapacity, int entryCapacity, IEqualityComparer<TKey> comparer)
		{
			m_nodeCapacity = nodeCapacity;
			m_valuesCapacity = entryCapacity;
			m_comparer = comparer;
			m_version = 0;
			m_count = 0;
			if (m_comparer == null)
			{
				m_comparer = EqualityComparer<TKey>.Default;
			}
			m_root = BuildNode(0, m_nodeCapacity);
		}

		public void Add(TKey key, TValue value)
		{
			if (Insert(m_root, m_comparer.GetHashCode(key), key, value, add: true, 0))
			{
				m_count++;
			}
			m_version++;
		}

		public bool ContainsKey(TKey key)
		{
			TValue value;
			return TryGetValue(key, out value);
		}

		public bool Remove(TKey key)
		{
			int newCount;
			bool num = Remove(m_root, m_comparer.GetHashCode(key), key, 0, out newCount);
			if (num)
			{
				m_count--;
				m_version++;
			}
			return num;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				Global.Tracer.Assert(condition: false, "SegmentedDictionary: Key cannot be null");
			}
			return Find(m_root, m_comparer.GetHashCode(key), key, 0, out value);
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

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			m_root = BuildNode(0, m_nodeCapacity);
			m_count = 0;
			m_version++;
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
				Global.Tracer.Assert(condition: false, "SegmentedDictionary.CopyTo: Index must be greater than 0");
			}
			if (array == null)
			{
				Global.Tracer.Assert(condition: false, "SegmentedDictionary.CopyTo: Specified array must not be null");
			}
			if (array.Rank > 1)
			{
				Global.Tracer.Assert(false, "SegmentedDictionary.CopyTo: Specified array must be 1 dimensional", "array");
			}
			if (arrayIndex + Count > array.Length)
			{
				Global.Tracer.Assert(condition: false, "SegmentedDictionary.CopyTo: Insufficent space in destination array");
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
			return new SegmentedDictionaryEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private SegmentedDictionaryNode BuildNode(int level, int capacity)
		{
			return new SegmentedDictionaryNode(capacity);
		}

		private bool Insert(SegmentedDictionaryNode node, int hashCode, TKey key, TValue value, bool add, int level)
		{
			bool flag = false;
			int num = HashToSlot(node, hashCode, level);
			ISegmentedDictionaryEntry segmentedDictionaryEntry = node.Entries[num];
			if (segmentedDictionaryEntry == null)
			{
				SegmentedDictionaryValues segmentedDictionaryValues = new SegmentedDictionaryValues(m_valuesCapacity);
				segmentedDictionaryValues.Keys[0] = key;
				segmentedDictionaryValues.Values[0] = value;
				segmentedDictionaryValues.Count++;
				node.Entries[num] = segmentedDictionaryValues;
				flag = true;
			}
			else
			{
				switch (segmentedDictionaryEntry.EntryType)
				{
				case SegmentedDictionaryEntryType.Node:
				{
					SegmentedDictionaryNode node2 = segmentedDictionaryEntry as SegmentedDictionaryNode;
					flag = Insert(node2, hashCode, key, value, add, level + 1);
					break;
				}
				case SegmentedDictionaryEntryType.Values:
				{
					SegmentedDictionaryValues segmentedDictionaryValues2 = segmentedDictionaryEntry as SegmentedDictionaryValues;
					bool flag2 = false;
					for (int i = 0; i < segmentedDictionaryValues2.Count; i++)
					{
						if (m_comparer.Equals(key, segmentedDictionaryValues2.Keys[i]))
						{
							if (add)
							{
								Global.Tracer.Assert(condition: false, "SegmentedDictionary: An element with the same key already exists within the Dictionary");
							}
							segmentedDictionaryValues2.Values[i] = value;
							flag2 = true;
							flag = false;
							break;
						}
					}
					if (flag2)
					{
						break;
					}
					if (segmentedDictionaryValues2.Count < segmentedDictionaryValues2.Capacity)
					{
						int count = segmentedDictionaryValues2.Count;
						segmentedDictionaryValues2.Keys[count] = key;
						segmentedDictionaryValues2.Values[count] = value;
						segmentedDictionaryValues2.Count++;
						flag = true;
						break;
					}
					SegmentedDictionaryNode segmentedDictionaryNode = BuildNode(level + 1, m_nodeCapacity);
					node.Entries[num] = segmentedDictionaryNode;
					for (int j = 0; j < segmentedDictionaryValues2.Count; j++)
					{
						TKey val = segmentedDictionaryValues2.Keys[j];
						Insert(segmentedDictionaryNode, m_comparer.GetHashCode(val), val, segmentedDictionaryValues2.Values[j], add: false, level + 1);
					}
					flag = Insert(segmentedDictionaryNode, hashCode, key, value, add, level + 1);
					break;
				}
				default:
					Global.Tracer.Assert(condition: false, "Unknown ObjectType");
					break;
				}
			}
			if (flag)
			{
				node.Count++;
			}
			return flag;
		}

		private int HashToSlot(SegmentedDictionaryNode node, int hashCode, int level)
		{
			int prime = PrimeHelper.GetPrime(level);
			int hashInputA = PrimeHelper.GetHashInputA(level);
			int hashInputB = PrimeHelper.GetHashInputB(level);
			return Math.Abs(hashInputA * hashCode + hashInputB) % prime % node.Entries.Length;
		}

		private bool Find(SegmentedDictionaryNode node, int hashCode, TKey key, int level, out TValue value)
		{
			value = default(TValue);
			bool result = false;
			int num = HashToSlot(node, hashCode, level);
			ISegmentedDictionaryEntry segmentedDictionaryEntry = node.Entries[num];
			if (segmentedDictionaryEntry != null)
			{
				switch (segmentedDictionaryEntry.EntryType)
				{
				case SegmentedDictionaryEntryType.Node:
				{
					SegmentedDictionaryNode node2 = segmentedDictionaryEntry as SegmentedDictionaryNode;
					result = Find(node2, hashCode, key, level + 1, out value);
					break;
				}
				case SegmentedDictionaryEntryType.Values:
				{
					SegmentedDictionaryValues segmentedDictionaryValues = segmentedDictionaryEntry as SegmentedDictionaryValues;
					for (int i = 0; i < segmentedDictionaryValues.Count; i++)
					{
						if (m_comparer.Equals(key, segmentedDictionaryValues.Keys[i]))
						{
							value = segmentedDictionaryValues.Values[i];
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
			return result;
		}

		private bool Remove(SegmentedDictionaryNode node, int hashCode, TKey key, int level, out int newCount)
		{
			bool flag = false;
			int num = HashToSlot(node, hashCode, level);
			ISegmentedDictionaryEntry segmentedDictionaryEntry = node.Entries[num];
			if (segmentedDictionaryEntry == null)
			{
				flag = false;
			}
			else
			{
				switch (segmentedDictionaryEntry.EntryType)
				{
				case SegmentedDictionaryEntryType.Node:
				{
					SegmentedDictionaryNode node2 = segmentedDictionaryEntry as SegmentedDictionaryNode;
					flag = Remove(node2, hashCode, key, level + 1, out int newCount2);
					if (flag && newCount2 == 0)
					{
						node.Entries[num] = null;
					}
					break;
				}
				case SegmentedDictionaryEntryType.Values:
				{
					SegmentedDictionaryValues segmentedDictionaryValues = segmentedDictionaryEntry as SegmentedDictionaryValues;
					for (int i = 0; i < segmentedDictionaryValues.Count; i++)
					{
						if (!m_comparer.Equals(key, segmentedDictionaryValues.Keys[i]))
						{
							continue;
						}
						if (segmentedDictionaryValues.Count == 1)
						{
							node.Entries[num] = null;
						}
						else
						{
							segmentedDictionaryValues.Keys[i] = default(TKey);
							segmentedDictionaryValues.Values[i] = default(TValue);
							segmentedDictionaryValues.Count--;
							int num2 = segmentedDictionaryValues.Count - i;
							if (num2 > 0)
							{
								Array.Copy(segmentedDictionaryValues.Keys, i + 1, segmentedDictionaryValues.Keys, i, num2);
								Array.Copy(segmentedDictionaryValues.Values, i + 1, segmentedDictionaryValues.Values, i, num2);
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
				node.Count--;
			}
			newCount = node.Count;
			return flag;
		}
	}
}
