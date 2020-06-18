using System;
using System.Collections;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeStripLinesCollection_StripLinesCollection")]
	internal class StripLinesCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		internal Axis axis;

		public StripLine this[int index]
		{
			get
			{
				return (StripLine)array[index];
			}
			set
			{
				array[index] = value;
				Invalidate();
			}
		}

		object IList.this[int index]
		{
			get
			{
				return array[index];
			}
			set
			{
				array[index] = value;
			}
		}

		public bool IsFixedSize => array.IsFixedSize;

		public bool IsReadOnly => array.IsReadOnly;

		public int Count => array.Count;

		public bool IsSynchronized => array.IsSynchronized;

		public object SyncRoot => array.SyncRoot;

		private StripLinesCollection()
		{
		}

		public StripLinesCollection(Axis axis)
		{
			this.axis = axis;
		}

		public void Clear()
		{
			array.Clear();
			Invalidate();
		}

		bool IList.Contains(object value)
		{
			return array.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return array.IndexOf(value);
		}

		public bool Contains(StripLine value)
		{
			return array.Contains(value);
		}

		public int IndexOf(StripLine value)
		{
			return array.IndexOf(value);
		}

		public void Remove(StripLine value)
		{
			array.Remove(value);
		}

		public void Remove(object value)
		{
			array.Remove(value);
		}

		public void RemoveAt(int index)
		{
			array.RemoveAt(index);
			Invalidate();
		}

		public int Add(StripLine value)
		{
			return Add((object)value);
		}

		public int Add(object value)
		{
			if (!(value is StripLine))
			{
				throw new ArgumentException(SR.ExceptionStripLineAddedHasWrongType);
			}
			((StripLine)value).axis = axis;
			Invalidate();
			return array.Add(value);
		}

		public void Insert(int index, StripLine value)
		{
			Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is StripLine))
			{
				throw new ArgumentException(SR.ExceptionStripLineAddedHasWrongType);
			}
			((StripLine)value).axis = axis;
			array.Insert(index, value);
			Invalidate();
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
			Invalidate();
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}

		private void Invalidate()
		{
		}
	}
}
