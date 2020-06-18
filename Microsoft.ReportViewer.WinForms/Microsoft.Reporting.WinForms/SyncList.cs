using System;
using System.Collections.ObjectModel;

namespace Microsoft.Reporting.WinForms
{
	[Serializable]
	public class SyncList<TListType> : Collection<TListType>
	{
		[NonSerialized]
		private object m_syncObject;

		private SyncList()
		{
		}

		internal SyncList(object syncObject)
			: this()
		{
			SetSyncObject(syncObject);
		}

		internal void SetSyncObject(object syncObject)
		{
			m_syncObject = syncObject;
		}

		protected override void ClearItems()
		{
			lock (m_syncObject)
			{
				base.ClearItems();
			}
		}

		protected override void InsertItem(int index, TListType item)
		{
			lock (m_syncObject)
			{
				base.InsertItem(index, item);
			}
		}

		protected override void RemoveItem(int index)
		{
			lock (m_syncObject)
			{
				base.RemoveItem(index);
			}
		}

		protected override void SetItem(int index, TListType item)
		{
			lock (m_syncObject)
			{
				base.SetItem(index, item);
			}
		}
	}
}
