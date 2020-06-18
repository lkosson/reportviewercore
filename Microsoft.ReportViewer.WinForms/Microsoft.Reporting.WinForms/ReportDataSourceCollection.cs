using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Reporting.WinForms
{
	[Serializable]
	[ComVisible(false)]
	public sealed class ReportDataSourceCollection : SyncList<ReportDataSource>, ISerializable
	{
		private EventHandler m_onChangeEventHandler;

		public ReportDataSource this[string name]
		{
			get
			{
				using (IEnumerator<ReportDataSource> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ReportDataSource current = enumerator.Current;
						if (string.Compare(name, current.Name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return current;
						}
					}
				}
				return null;
			}
		}

		internal event EventHandler Change;

		internal ReportDataSourceCollection(object syncObject)
			: base(syncObject)
		{
			m_onChangeEventHandler = OnChange;
		}

		internal ReportDataSourceCollection(SerializationInfo info, StreamingContext context)
			: this(new object())
		{
			int @int = info.GetInt32("Count");
			for (int i = 0; i < @int; i++)
			{
				string str = i.ToString(CultureInfo.InvariantCulture);
				Add(new ReportDataSource
				{
					Name = info.GetString("Name" + str)
				});
			}
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Count", base.Count);
			for (int i = 0; i < base.Count; i++)
			{
				string str = i.ToString(CultureInfo.InvariantCulture);
				ReportDataSource reportDataSource = base[i];
				info.AddValue("Name" + str, reportDataSource.Name);
			}
		}

		protected override void ClearItems()
		{
			using (IEnumerator<ReportDataSource> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ReportDataSource current = enumerator.Current;
					UnregisterItem(current);
				}
			}
			base.ClearItems();
		}

		protected override void InsertItem(int index, ReportDataSource item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			base.InsertItem(index, item);
			RegisterItem(item);
		}

		protected override void RemoveItem(int index)
		{
			UnregisterItem(base[index]);
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, ReportDataSource item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			UnregisterItem(base[index]);
			base.SetItem(index, item);
			RegisterItem(item);
		}

		private void RegisterItem(ReportDataSource item)
		{
			item.Changed += m_onChangeEventHandler;
			OnChange();
		}

		private void UnregisterItem(ReportDataSource item)
		{
			item.Changed -= m_onChangeEventHandler;
			OnChange();
		}

		private void OnChange()
		{
			if (this.Change != null)
			{
				this.Change(this, EventArgs.Empty);
			}
		}

		private void OnChange(object sender, EventArgs e)
		{
			OnChange();
		}
	}
}
