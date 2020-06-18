using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	[CLSCompliant(false)]
	public sealed class RemoteMemberArrayWrapper<TMemberType> : MarshalByRefObject, IList<IMemberNode>, ICollection<IMemberNode>, IEnumerable<IMemberNode>, IEnumerable where TMemberType : IMemberNode
	{
		private readonly TMemberType[] m_array;

		public IMemberNode this[int index]
		{
			get
			{
				return m_array[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int Count => m_array.Length;

		public bool IsReadOnly => true;

		public RemoteMemberArrayWrapper(params TMemberType[] array)
		{
			m_array = array;
		}

		public int IndexOf(IMemberNode item)
		{
			throw new NotSupportedException();
		}

		public void Insert(int index, IMemberNode item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public void Add(IMemberNode item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(IMemberNode item)
		{
			throw new NotSupportedException();
		}

		public void CopyTo(IMemberNode[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public bool Remove(IMemberNode item)
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			int i = 0;
			while (i < m_array.Length)
			{
				yield return m_array[i];
				int num = i + 1;
				i = num;
			}
		}

		public IEnumerator<IMemberNode> GetEnumerator()
		{
			int i = 0;
			while (i < m_array.Length)
			{
				yield return m_array[i];
				int num = i + 1;
				i = num;
			}
		}
	}
}
