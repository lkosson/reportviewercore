using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class RdlCollectionBase<T> : Collection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IContainedObject
	{
		private IContainedObject m_parent;

		[XmlIgnore]
		public IContainedObject Parent
		{
			get
			{
				return m_parent;
			}
			set
			{
				m_parent = value;
				if (!typeof(IContainedObject).IsAssignableFrom(typeof(T)))
				{
					return;
				}
				using (IEnumerator<T> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						((IContainedObject)(object)enumerator.Current).Parent = value;
					}
				}
			}
		}

		object IList.this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				base[index] = (T)value;
			}
		}

		protected override void InsertItem(int index, T item)
		{
			if (item is IContainedObject)
			{
				((IContainedObject)(object)item).Parent = m_parent;
			}
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, T item)
		{
			if (item is IContainedObject)
			{
				((IContainedObject)(object)item).Parent = m_parent;
			}
			base.SetItem(index, item);
		}

		protected RdlCollectionBase()
		{
		}

		protected RdlCollectionBase(IContainedObject parent)
		{
			m_parent = parent;
		}

		int IList.Add(object item)
		{
			Add((T)item);
			return base.Count - 1;
		}
	}
}
