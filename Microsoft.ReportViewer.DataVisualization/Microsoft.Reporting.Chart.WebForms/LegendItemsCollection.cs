using System;
using System.Collections;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeCustomLabelsCollection_CustomLabelsCollection")]
	internal class LegendItemsCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		internal CommonElements common;

		internal Legend legend;

		public LegendItem this[int index]
		{
			get
			{
				return (LegendItem)array[index];
			}
			set
			{
				value.Legend = legend;
				array[index] = value;
				Invalidate(invalidateLegendOnly: false);
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

		public int Add(Color color, string text)
		{
			LegendItem legendItem = new LegendItem(text, color, "");
			legendItem.Legend = legend;
			if (common != null)
			{
				legendItem.common = common;
			}
			Invalidate(invalidateLegendOnly: false);
			return Add(legendItem);
		}

		public void Insert(int index, Color color, string text)
		{
			LegendItem legendItem = new LegendItem(text, color, "");
			legendItem.Legend = legend;
			if (common != null)
			{
				legendItem.common = common;
			}
			Insert(index, legendItem);
			Invalidate(invalidateLegendOnly: false);
		}

		public int Add(string image, string text)
		{
			LegendItem legendItem = new LegendItem(text, Color.Empty, image);
			legendItem.Legend = legend;
			if (common != null)
			{
				legendItem.common = common;
			}
			Invalidate(invalidateLegendOnly: false);
			return Add(legendItem);
		}

		public void Insert(int index, string image, string text)
		{
			LegendItem legendItem = new LegendItem(text, Color.Empty, image);
			legendItem.Legend = legend;
			if (common != null)
			{
				legendItem.common = common;
			}
			Insert(index, legendItem);
			Invalidate(invalidateLegendOnly: false);
		}

		public void Clear()
		{
			array.Clear();
			Invalidate(invalidateLegendOnly: false);
		}

		bool IList.Contains(object value)
		{
			return array.Contains(value);
		}

		public bool Contains(LegendItem value)
		{
			return array.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return array.IndexOf(value);
		}

		public int IndexOf(LegendItem value)
		{
			return array.IndexOf(value);
		}

		void IList.Remove(object value)
		{
			array.Remove(value);
			Invalidate(invalidateLegendOnly: false);
		}

		public void Remove(LegendItem value)
		{
			array.Remove(value);
			Invalidate(invalidateLegendOnly: false);
		}

		public void RemoveAt(int index)
		{
			array.RemoveAt(index);
			Invalidate(invalidateLegendOnly: false);
		}

		public int Add(object value)
		{
			if (!(value is LegendItem))
			{
				throw new ArgumentException(SR.ExceptionLegendItemAddedHasWrongType);
			}
			if (common != null)
			{
				((LegendItem)value).common = common;
			}
			((LegendItem)value).Legend = legend;
			Invalidate(invalidateLegendOnly: false);
			return array.Add(value);
		}

		public void Insert(int index, LegendItem value)
		{
			Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is LegendItem))
			{
				throw new ArgumentException(SR.ExceptionLegendItemInsertedHasWrongType);
			}
			if (common != null)
			{
				((LegendItem)value).common = common;
			}
			((LegendItem)value).Legend = legend;
			array.Insert(index, value);
			Invalidate(invalidateLegendOnly: false);
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
			Invalidate(invalidateLegendOnly: false);
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}

		public void Reverse()
		{
			array.Reverse();
			Invalidate(invalidateLegendOnly: false);
		}

		private void Invalidate(bool invalidateLegendOnly)
		{
		}
	}
}
